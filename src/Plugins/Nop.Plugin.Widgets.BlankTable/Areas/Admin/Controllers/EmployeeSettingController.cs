using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Factories;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Settings;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Gdpr;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Controllers
{
    public partial class EmployeeSettingController : BaseAdminController
    {
        #region Fields

        private readonly AppSettings _appSettings;
        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _dataProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGdprService _gdprService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IRoxyFilemanService _roxyFilemanService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEmployeeSettingModelFactory _settingModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IUploadService _uploadService;

        #endregion

        #region Ctor

        public EmployeeSettingController(
            AppSettings appSettings,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGdprService gdprService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IRoxyFilemanService roxyFilemanService,
            IServiceScopeFactory serviceScopeFactory,
            IEmployeeSettingModelFactory settingModelFactory,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IUploadService uploadService)
        {
            _appSettings = appSettings;
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _gdprService = gdprService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _roxyFilemanService = roxyFilemanService;
            _serviceScopeFactory = serviceScopeFactory;
            _settingModelFactory = settingModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _uploadService = uploadService;
        }

        #endregion

        #region Methods
        public virtual async Task<IActionResult> Catalog()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareEmployeeSettingsModelAsync();

            return View("~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Setting/Employee.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Catalog(EmployeeSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var employeeSettings = await _settingService.LoadSettingAsync<EmployeeSettings>(storeScope);
                employeeSettings = model.ToSettings(employeeSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.DefaultViewMode, model.DefaultViewMode_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.ShowShareButton, model.ShowShareButton_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.PageShareCode, model.PageShareCode_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.EmailAFriendEnabled, model.EmailAFriendEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.AllowAnonymousUsersToEmailAFriend, model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.SearchPageAllowCustomersToSelectPageSize, model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.SearchPagePageSizeOptions, model.SearchPagePageSizeOptions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.ExportImportEmployeesUsingEmployeeName, model.ExportImportEmployeesUsingEmployeeName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.ExportImportAllowDownloadImages, model.ExportImportAllowDownloadImages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.ExportImportRelatedEntitiesByName, model.ExportImportRelatedEntitiesByName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.DisplayDatePreOrderAvailability, model.DisplayDatePreOrderAvailability_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.EmployeeBreadcrumbEnabled, model.CategoryBreadcrumbEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.EnableSpecificationAttributeFiltering, model.EnableSpecificationAttributeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.DisplayAllPicturesOnCatalogPages, model.DisplayAllPicturesOnCatalogPages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.AllowCustomersToSearchWithEmployeeName, model.AllowCustomersToSearchWithEmployeeName_OverrideForStore, storeScope, false);

                //now settings not overridable per store
                await _settingService.SaveSettingAsync(employeeSettings, x => x.IgnoreAcl, 0, false);
                await _settingService.SaveSettingAsync(employeeSettings, x => x.IgnoreStoreLimitations, 0, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Catalog");
            }

            //prepare model
            model = await _settingModelFactory.PrepareEmployeeSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            // return View(model);
            return View("~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Setting/Employee.cshtml", model);
        }

        #endregion
    }
}