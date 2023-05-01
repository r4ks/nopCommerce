using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.ExportImport;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.HumanResource.Services.Security;
using Nop.Services.Helpers;
using System.Globalization;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System.IO;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.Common;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Controllers.HumanResource
{
    public partial class EmployeeController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IEmployeeModelFactory _employeeModelFactory;
        private readonly IEmployeeService _employeeService;
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
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPluginPdfService _pdfService;

        #endregion

        #region Ctor

        public EmployeeController(IAclService aclService,
            IEmployeeModelFactory employeeModelFactory,
            IEmployeeService employeeService,
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
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            IPluginPdfService pdfService)
        {
            _aclService = aclService;
            _employeeModelFactory = employeeModelFactory;
            _employeeService = employeeService;
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
            _dateTimeHelper = dateTimeHelper;
            _pdfService = pdfService;
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
            await _employeeService.UpdateEmployeeAsync(employee);

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
            await _employeeService.UpdateEmployeeAsync(employee);

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
            var model = await _employeeModelFactory.PrepareEmployeeSearchModelAsync(new EmployeeSearchModel());

            return View(EmployeeSearchModel.LIST_VIEW, model);
            //return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(EmployeeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeModelAsync(new EmployeeModel(), null);

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
                await _employeeService.InsertEmployeeAsync(employee);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(employee, model.SeName, employee.Name, true);
                await _urlRecordService.SaveSlugAsync(employee, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(employee, model);

                await _employeeService.UpdateEmployeeAsync(employee);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(employee);

                //ACL (customer roles)
                await SaveEmployeeAclAsync(employee, model);

                //stores
                await SaveStoreMappingsAsync(employee, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewEmployee",
                    string.Format(await _localizationService.GetResourceAsync(EmployeeModel.Labels.LogAddNewEmployee), employee.Name), employee);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync(EmployeeModel.Labels.AddedEvent));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = employee.Id });
            }

            //prepare model
            model = await _employeeModelFactory.PrepareEmployeeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null || employee.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeModelAsync(null, employee);

            //return View(model);
            return View(EmployeeModel.EDIT_VIEW, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
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
                await _employeeService.UpdateEmployeeAsync(employee);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(employee, model.SeName, employee.Name, true);
                await _urlRecordService.SaveSlugAsync(employee, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(employee, model);

                await _employeeService.UpdateEmployeeAsync(employee);

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
                    string.Format(await _localizationService.GetResourceAsync(EmployeeModel.Labels.LogEditEmployee), employee.Name), employee);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync(EmployeeModel.Labels.UpdatedEvent));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = employee.Id });
            }

            //prepare model
            model = await _employeeModelFactory.PrepareEmployeeModelAsync(model, employee, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            //try to get a employee with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return RedirectToAction("List");

            await _employeeService.DeleteEmployeeAsync(employee);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteEmployee",
                string.Format(await _localizationService.GetResourceAsync(EmployeeModel.Labels.LogDeleteEmployee), employee.Name), employee);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync(EmployeeModel.Labels.DeletedEvent));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(APermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _employeeService.DeleteEmployeesAsync(await (await _employeeService.GetEmployeesByIdsAsync(selectedIds.ToArray())).ToListAsync());

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("DownloadHumanResourcePDF")]
        [FormValueRequired("download-humanresource-pdf")]
        public virtual async Task<IActionResult> DownloadHumanResourcePDF(EmployeeSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await _employeeService.SearchEmployeesAsync();

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintEmployeesToPdfAsync(stream, products);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "employees.pdf");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

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
                    .ExportEmployeesToXlsxAsync((await _employeeService.GetAllEmployeesAsync(showHidden: true)).ToList());

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

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                    await _importManager.ImportEmployeesFromXlsxAsync(importexcelfile.OpenReadStream());
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync(EmployeeModel.Labels.LogUploadFile));
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync(EmployeeModel.Labels.ImportedEvent));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> LoadEmployeeStatistics(string period)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();

            var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", culture),
                            value = (await _employeeService.SearchEmployeesAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", culture),
                            value = (await _employeeService.SearchEmployeesAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", culture),
                            value = (await _employeeService.SearchEmployeesAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }
        #endregion

    }
}