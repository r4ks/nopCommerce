using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.BlankTable.Models;
using Nop.Plugin.Widgets.BlankTable.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.BlankTable.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class BlankTableController : BasePluginController
    {
        #region Fields

        private readonly ICustomersByCountryService _service;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        public BlankTableController(ICustomersByCountryService service, ILanguageService languageService, ILocalizationService localizationService, IStoreContext storeContext, ISettingService settingService, IPermissionService permissionService, INotificationService notificationService)
        {
            _service = service;
            _languageService = languageService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _settingService = settingService;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> ConfigureAsync()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<BlankTableSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = checkMoneyOrderPaymentSettings.DescriptionText
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.DescriptionText = await _localizationService
                    .GetLocalizedSettingAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = checkMoneyOrderPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = checkMoneyOrderPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = checkMoneyOrderPaymentSettings.ShippableProductRequired;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Widgets.BlankTable/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await ConfigureAsync();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<BlankTableSettings>(storeScope);

            //save settings
            checkMoneyOrderPaymentSettings.DescriptionText = model.DescriptionText;
            checkMoneyOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            checkMoneyOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            checkMoneyOrderPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(checkMoneyOrderPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await ConfigureAsync();
        }

        [HttpGet]
        public IActionResult BlankTable()
        {
            CustomersByCountrySearchModel customerSearchModel = new CustomersByCountrySearchModel { AvailablePageSizes = "10"};
            return View("~/Plugins/Widgets.BlankTable/Views/BlankTable.cshtml", customerSearchModel);

        }

        [HttpPost]
        public async Task<IActionResult> GetCustomersCountByCountry()
        {
            try
            {
                return Ok(new DataTablesModel { Data = await _service.GetCustomersDistributionByCountryAsync()});
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
