using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.Settings;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Controllers.Settings
{
    public partial class EmployeeSettingController : BaseAdminController
    {
        public const string ConfigureActionName = "Configure";
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IEmployeeSettingModelFactory _settingModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public EmployeeSettingController(
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IEmployeeSettingModelFactory settingModelFactory,
            ISettingService settingService,
            IStoreContext storeContext
            )
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingModelFactory = settingModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods
        [HttpGet, ActionName(ConfigureActionName)]
        public virtual async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareEmployeeSettingsModelAsync();

            return View(EmployeeSettingsModel.View, model);
        }

        [HttpPost, ActionName(ConfigureActionName)]
        public virtual async Task<IActionResult> Configure(EmployeeSettingsModel model)
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
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.EmployeeBreadcrumbEnabled, model.EmployeeBreadcrumbEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.EnableSpecificationAttributeFiltering, model.EnableSpecificationAttributeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.DisplayAllPicturesOnHumanResourcePages, model.DisplayAllPicturesOnHumanResourcePages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(employeeSettings, x => x.AllowCustomersToSearchWithEmployeeName, model.AllowCustomersToSearchWithEmployeeName_OverrideForStore, storeScope, false);

                //now settings not overridable per store
                await _settingService.SaveSettingAsync(employeeSettings, x => x.IgnoreAcl, 0, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync(EmployeeSettingsModel.Labels.EditSettings));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync(EmployeeSettingsModel.Labels.Updated));

                return RedirectToAction(ConfigureActionName);
            }

            //prepare model
            model = await _settingModelFactory.PrepareEmployeeSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(EmployeeSettingsModel.View, model);
        }

        #endregion
    }
}