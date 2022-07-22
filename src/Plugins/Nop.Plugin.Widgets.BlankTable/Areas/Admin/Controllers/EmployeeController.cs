using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Factories;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Plugin.Widgets.BlankTable.Services.Hr;
using Nop.Plugin.Widgets.BlankTable.Services.ExportImport;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.BlankTable.Services.Security;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Controllers
{
    public partial class EmployeeController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IEmployeeModelFactory _categoryModelFactory;
        private readonly IEmployeeService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IPluginExportManager _exportManager;
        private readonly IPluginImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EmployeeController(IAclService aclService,
            IEmployeeModelFactory categoryModelFactory,
            IEmployeeService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IPluginExportManager exportManager,
            IPluginImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _categoryModelFactory = categoryModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Employee employee, EmployeeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(employee,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(employee,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(employee, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(employee, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Employee employee)
        {
            var picture = await _pictureService.GetPictureByIdAsync(employee.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(employee.Name));
        }

        protected virtual async Task SaveEmployeeAclAsync(Employee employee, EmployeeModel model)
        {
            employee.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await _categoryService.UpdateEmployeeAsync(employee);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(employee);
            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    //new role
                    if (!existingAclRecords.Any(acl => acl.CustomerRoleId == customerRole.Id))
                        await _aclService.InsertAclRecordAsync(employee, customerRole.Id);
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
        }

        protected virtual async Task SaveStoreMappingsAsync(Employee employee, EmployeeModel model)
        {
            employee.LimitedToStores = model.SelectedStoreIds.Any();
            await _categoryService.UpdateEmployeeAsync(employee);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(employee);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
                if (model.SelectedStoreIds.Contains(store.Id))
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(employee, store.Id);
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareEmployeeSearchModelAsync(new EmployeeSearchModel());

            return View(EmployeeSearchModel.ListView, model);
            //return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(EmployeeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _categoryModelFactory.PrepareEmployeeListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareEmployeeModelAsync(new EmployeeModel(), null);

            //return View(model);
            return View(EmployeeModel.CREATE_VIEW, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity<Employee>();
                employee.CreatedOnUtc = DateTime.UtcNow;
                employee.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.InsertEmployeeAsync(employee);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(employee, model.SeName, employee.Name, true);
                await _urlRecordService.SaveSlugAsync(employee, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(employee, model);

                await _categoryService.UpdateEmployeeAsync(employee);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(employee);

                //ACL (customer roles)
                await SaveEmployeeAclAsync(employee, model);

                //stores
                await SaveStoreMappingsAsync(employee, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewEmployee",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewEmployee"), employee.Name), employee);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Employees.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = employee.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareEmployeeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _categoryService.GetEmployeeByIdAsync(id);
            if (employee == null || employee.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _categoryModelFactory.PrepareEmployeeModelAsync(null, employee);

            //return View(model);
            return View(EmployeeModel.EditView, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _categoryService.GetEmployeeByIdAsync(model.Id);
            if (employee == null || employee.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = employee.PictureId;

                //if parent employee changes, we need to clear cache for previous parent employee
                if (employee.ParentEmployeeId != model.ParentEmployeeId)
                {
                    await _staticCacheManager.RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesByParentEmployeePrefix, employee.ParentEmployeeId);
                    await _staticCacheManager.RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesChildIdsPrefix, employee.ParentEmployeeId);
                }

                employee = model.ToEntity(employee);
                employee.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.UpdateEmployeeAsync(employee);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(employee, model.SeName, employee.Name, true);
                await _urlRecordService.SaveSlugAsync(employee, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(employee, model);

                await _categoryService.UpdateEmployeeAsync(employee);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != employee.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(employee);

                //ACL
                await SaveEmployeeAclAsync(employee, model);

                //stores
                await SaveStoreMappingsAsync(employee, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditEmployee",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditEmployee"), employee.Name), employee);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Employees.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = employee.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareEmployeeModelAsync(model, employee, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _categoryService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return RedirectToAction("List");

            await _categoryService.DeleteEmployeeAsync(employee);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteEmployee",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteEmployee"), employee.Name), employee);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Employees.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _categoryService.DeleteEmployeesAsync(await (await _categoryService.GetEmployeesByIdsAsync(selectedIds.ToArray())).WhereAwait(async p => await _workContext.GetCurrentVendorAsync() == null).ToListAsync());

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            try
            {
                var xml = await _exportManager.ExportEmployeesToXmlAsync();

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "employees.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            try
            {
                var bytes = await _exportManager
                    .ExportEmployeesToXlsxAsync((await _categoryService.GetAllEmployeesAsync(showHidden: true)).ToList());

                return File(bytes, MimeTypes.TextXlsx, "employees.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //a vendor cannot import employees
            if (await _workContext.GetCurrentVendorAsync() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                    await _importManager.ImportEmployeesFromXlsxAsync(importexcelfile.OpenReadStream());
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Employees.Imported"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

    }
}