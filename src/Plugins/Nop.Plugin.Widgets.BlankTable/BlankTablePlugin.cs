using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Plugin.Widgets.BlankTable.Components;
using Nop.Plugin.Widgets.BlankTable.Models;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BlankTable
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class CustomPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public bool HideInWidgetList => false;

        public CustomPlugin(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
        }

        public override async Task InstallAsync()
        {
            //settings
            var settings = new BlankTableSettings
            {
                DescriptionText = "<p>Mail Personal or Business Check, Cashier's Check or money order to:</p><p><br /><b>NOP SOLUTIONS</b> <br /><b>your address here,</b> <br /><b>New York, NY 10001 </b> <br /><b>USA</b></p><p>Notice that if you pay by Personal or Business Check, your order may be held for up to 10 days after we receive your check to allow enough time for the check to clear.  If you want us to ship faster upon receipt of your payment, then we recommend your send a money order or Cashier's check.</p><p>P.S. You can edit this text from admin panel.</p>"
            };

            await _settingService.SaveSettingAsync(settings);

            await InstallLocalizedStrings();
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<BlankTableSettings>();

            // locales
            await _localizationService.DeleteLocaleResourcesAsync(ConfigurationModel.Labels.Prefix);
            await base.UninstallAsync();
        }

        public override Task UpdateAsync(string currentVersion, string targetVersion)
        {
            return base.UpdateAsync(currentVersion, targetVersion);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(BlankTableDefaults.ConfigurationRouteName);
            //return _webHelper.GetStoreLocation() + "Admin/WidgetsBlankTable/Configure";
        }

        /// <summary>
        /// Add Menu Item on Admin area.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var showMenuInsidePluginsMenu = false; // on side menu

            var menuItem = new SiteMapNode()
            {
                SystemName = "Widgets.BlankTable",
                Title = "New Blank Title",
                ControllerName = "BlankTable",
                ActionName = "BlankTable",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            // Add localized title and parent menu item
            const string blankTableNodeTitle = "BlankTable.MainNode.Title";
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                [blankTableNodeTitle] = "Blank Table Group"
            });
            var title = await _localizationService.GetResourceAsync(blankTableNodeTitle);
            var mainNode = new SiteMapNode()
            {
                SystemName = "BlankTable.MainNode",
                Title = title,
                IconClass = "fas fa-table",
                Visible = true,
            };

            if(showMenuInsidePluginsMenu)
            {
                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");

                if (pluginNode != null)
                {
                    mainNode.ChildNodes.Add(menuItem);
                    pluginNode.ChildNodes.Add(mainNode);
                }
                else
                    rootNode.ChildNodes.Add(menuItem);
            }
            else
            {
                rootNode.ChildNodes.Add(mainNode);

                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "BlankTable.MainNode");

                if (pluginNode != null)
                {
                    pluginNode.ChildNodes.Add(menuItem);
                }
                else
                    rootNode.ChildNodes.Add(menuItem);
            }




            //return Task.CompletedTask;
        }

        /// <summary>
        /// Define here the zone of the widget you wished to be shown.
        /// </summary>
        /// <returns></returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.DashboardTop });
        }

        /// <summary>
        /// If you want a widget you return the view from this method.
        /// </summary>
        /// <param name="widgetZone"></param>
        /// <returns></returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(CustomViewComponent);
        }

        #region Helpers
        /// <summary>
        /// Add Localized Strings into the Localized Dictionary from Localization Service.
        /// </summary>
        /// <returns></returns>
        private async Task InstallLocalizedStrings()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                [ConfigurationModel.Labels.AdditionalFee] = "Additional fee",
                [ConfigurationModel.Labels.AdditionalFeeHint] = "The additional fee.",
                [ConfigurationModel.Labels.AdditionalFeePercentage] = "Additional fee. Use percentage",
                [ConfigurationModel.Labels.AdditionalFeePercentageHint] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                [ConfigurationModel.Labels.DescriptionText] = "Description",
                [ConfigurationModel.Labels.DescriptionTextHint] = "Enter info that will be shown to customers during checkout",
                [ConfigurationModel.Labels.PaymentMethodDescription] = "Pay by cheque or money order",
                [ConfigurationModel.Labels.ShippableProductRequired] = "Shippable product required",
                [ConfigurationModel.Labels.ShippableProductRequiredHint] = "An option indicating whether shippable products are required in order to display this payment method during checkout."
            });
        }
        #endregion
    }
}
