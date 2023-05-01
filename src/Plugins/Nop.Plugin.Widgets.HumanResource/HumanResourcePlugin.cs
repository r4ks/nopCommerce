using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.Settings;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Validators.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Components;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.Installation;
using Nop.Plugin.Widgets.HumanResource.Services.Security;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HumanResource
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class HumanResourcePlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IExtraInstallationService _extraInstallationService;

        public bool HideInWidgetList => false;

        public HumanResourcePlugin(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService, IPermissionService permissionService, IExtraInstallationService extraInstallationService)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _extraInstallationService = extraInstallationService;
        }

        public override async Task InstallAsync()
        {
            //settings
            var regionInfo = new RegionInfo(NopCommonDefaults.DefaultLanguageCulture);
            var cultureInfo = new CultureInfo(NopCommonDefaults.DefaultLanguageCulture);
            await _extraInstallationService.InstallRequiredDataAsync(regionInfo, cultureInfo);

            await InstallLocalizedStringsAsync();
            await InstallPermissionsAsync();



            // Comment the following line to disable the installation of sample Data.
            //await _extraInstallationService.InstallSampleDataAsync();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<EmployeeSettings>();

            // locales
            await _localizationService.DeleteLocaleResourcesAsync("Admin.HumanResource.Employees");
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
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(HumanResourceDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Add Menu Item on Admin area.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            // Menu Item for Settings bellow Settings Category
            var humanResourceSettingMenuItem = new SiteMapNode()
            {
                SystemName = EmployeeSettingsModel.SYSTEM_NAME,
                Title = "Employees Setings",
                ActionName = "Configure",
                ControllerName = "EmployeeSetting",
                IconClass = "far fa-circle",
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            // Menu item for Employee List below Human Resource Category
            var employeeListMenuItem = new SiteMapNode()
            {
                SystemName = EmployeeSearchModel.SYSTEM_NAME,
                Title = "Employee List",
                ControllerName = "Employee",
                ActionName = "List",
                IconClass = "fas fa-male",
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            // Add localized title and parent menu item
            var title = await _localizationService.GetResourceAsync(HumanResourceDefaults.Labels.HumanResourceNodeTitle);
            // Human Resource Category Menu Item
            var mainNode = new SiteMapNode()
            {
                SystemName = "HumanResource.MainNode",
                Title = title,
                IconClass = "fas fa-table",
                Visible = true,
            };

            rootNode.ChildNodes.Add(mainNode);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "HumanResource.MainNode");
            var configurationNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
            var settingsNode = configurationNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Settings");

            // Add human resource settings at Configuration > Settings
            // and before All Settings.
            if (settingsNode != null)
            {
                var last = settingsNode.ChildNodes.FirstOrDefault((i) => i.SystemName == "All settings");
                var index = settingsNode.ChildNodes.IndexOf(last);
                if(index != null)
                {
                    settingsNode.ChildNodes.Insert(index, humanResourceSettingMenuItem);
                }
                else
                {
                    settingsNode.ChildNodes.Add(humanResourceSettingMenuItem);
                }
            }


            if (pluginNode != null)
            {
                pluginNode.ChildNodes.Add(employeeListMenuItem);
            }
            else
            {
                rootNode.ChildNodes.Add(employeeListMenuItem);
            }
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
            return typeof(HumanResourceStatsComponent);
        }

        /// <summary>
        /// Defines the Widget component name.
        /// </summary>
        /// <param name="widgetZone"></param>
        /// <returns></returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "StatsWidget";
        }

        #region Helpers
        /// <summary>
        /// Add Localized Strings into the Localized Dictionary from Localization Service.
        /// </summary>
        /// <returns></returns>
        private async Task InstallLocalizedStringsAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                [EmployeeModel.Labels.Name] = "Name",
                [EmployeeModel.Labels.Description] = "Description",
                [EmployeeModel.Labels.SeName] = "Search engine friendly page name",
                [EmployeeModel.Labels.ParentEmployeeId] = "Parent employee",
                [EmployeeModel.Labels.PictureId] = "Picture",
                [EmployeeModel.Labels.PageSize] = "Page size",
                [EmployeeModel.Labels.AllowCustomersToSelectPageSize] = "Allow customers to select page size",
                [EmployeeModel.Labels.PageSizeOptions] = "Page size options",
                [EmployeeModel.Labels.ShowOnHomepage] = "Show on home page",
                [EmployeeModel.Labels.IncludeInTopMenu] = "Include in top menu",
                [EmployeeModel.Labels.Published] = "Published",
                [EmployeeModel.Labels.Deleted] = "Deleted",
                [EmployeeModel.Labels.DisplayOrder] = "Display order",
                [EmployeeModel.Labels.SelectedCustomerRoleIds] = "Limited to customer roles",
                [EmployeeModel.Labels.SelectedStoreIds] = "Limited to stores",
                [EmployeeModel.Labels.None] = "[None]",
                [EmployeeModel.Labels.EditEmployeeDetails] = "Edit employee details",
                [EmployeeModel.Labels.BackToList] = "back to employee list",
                [EmployeeModel.Labels.AddNew] = "Add a new employee",
                [EmployeeModel.Labels.Info] = "Employee info",
                [EmployeeModel.Labels.Display] = "Display",
                [EmployeeModel.Labels.Mappings] = "Mappings",
                [EmployeeModel.Labels.Products] = "Products",
                [EmployeeModel.Labels.AddedEvent] = "The new employee has been added successfully.",
                [EmployeeModel.Labels.UpdatedEvent] = "The employee has been updated successfully.",
                [EmployeeModel.Labels.DeletedEvent] = "The employee has been deleted successfully.",
                [EmployeeModel.Labels.ImportedEvent] = "Employees have been imported successfully.",
                [EmployeeModel.Labels.LogAddNewEmployee] = "Added a new employee ('{0}')",
                [EmployeeModel.Labels.LogEditEmployee] = "Edited a employee ('{0}')",
                [EmployeeModel.Labels.LogDeleteEmployee] = "Deleted a employee ('{0}')",

                [EmployeeModel.Labels.Title] = "Employees",

                [EmployeeSearchModel.Labels.SearchEmployeeName] = "Employee name",
                [EmployeeSearchModel.Labels.SearchStoreId] = "Store",
                [EmployeeSearchModel.Labels.ImportFromExcelTip] = "Imported employees are distinguished by ID. If the ID already exists, then its corresponding employee will be updated. You should not specify ID (leave 0) for new employees.",
                [EmployeeSearchModel.Labels.SearchPublishedId] = "Published",
                [EmployeeSearchModel.Labels.All] = "All",
                [EmployeeSearchModel.Labels.PublishedOnly] = "Published only",
                [EmployeeSearchModel.Labels.UnpublishedOnly] = "Unpublished only",
                [EmployeeSearchModel.Labels.DownloadPDF] = "Download List as PDF",

                [EmployeeSettingsModel.Labels.Title] = "HumanResource settings",
                [EmployeeSettingsModel.Labels.EditSettings] = "Edited settings",
                [EmployeeSettingsModel.Labels.Updated] = "The settings have been updated successfully.",
                [EmployeeSettingsModel.Labels.Grid] = "Grid",
                [EmployeeSettingsModel.Labels.List] = "List",
                [EmployeeSettingsModel.Labels.DefaultViewMode] = "Default view mode",
                [EmployeeSettingsModel.Labels.ShowShareButton] = "Show a share button",
                [EmployeeSettingsModel.Labels.PageShareCode] = "Share button code",
                [EmployeeSettingsModel.Labels.EmailAFriendEnabled] = "'Email a friend' enabled",
                [EmployeeSettingsModel.Labels.AllowAnonymousUsersToEmailAFriend] = "Allow anonymous users to email a friend",
                [EmployeeSettingsModel.Labels.SearchPageAllowCustomersToSelectPageSize] = "Search page. Allow customers to select page size",
                [EmployeeSettingsModel.Labels.SearchPagePageSizeOptions] = "Search page. Page size options",
                [EmployeeSettingsModel.Labels.ExportImportEmployeesUsingEmployeeName] = "Export/Import employees using name of employee",
                [EmployeeSettingsModel.Labels.ExportImportAllowDownloadImages] = "Export/Import products. Allow download images",
                [EmployeeSettingsModel.Labels.ExportImportRelatedEntitiesByName] = "Export/Import related entities using name",
                [EmployeeSettingsModel.Labels.EmployeeBreadcrumbEnabled] = "Employee breadcrumb enabled",
                [EmployeeSettingsModel.Labels.IgnoreAcl] = "Ignore ACL rules (sitewide)",
                [EmployeeSettingsModel.Labels.IgnoreStoreLimitations] = "Ignore \"limit per store\" rules (sitewide)",
                [EmployeeSettingsModel.Labels.DisplayDatePreOrderAvailability] = "Display the date for a pre-order availability",
                [EmployeeSettingsModel.Labels.EnableSpecificationAttributeFiltering] = "Enable specification attribute filtering",
                [EmployeeSettingsModel.Labels.DisplayAllPicturesOnHumanResourcePages] = "Display all pictures on hr pages",
                [EmployeeSettingsModel.Labels.AllowCustomersToSearchWithEmployeeName] = "Allow customers to search with employee name",
                [EmployeeSettingsModel.Labels.Search] = "Search",
                [EmployeeSettingsModel.Labels.Performance] = "Performance",
                [EmployeeSettingsModel.Labels.Share] = "Share",
                [EmployeeSettingsModel.Labels.AdditionalSections] = "Additional sections",
                [EmployeeSettingsModel.Labels.HumanResourcePages] = "HumanResource pages",
                [EmployeeSettingsModel.Labels.ExportImport] = "Export/Import",

                [EmployeeValidator.Labels.NameRequired] = "Please provide a name.",
                [EmployeeValidator.Labels.PageSizeOptionsShouldHaveUniqueItems] = "Page size options should not have duplicate items.",
                [EmployeeValidator.Labels.PageSizePositive] = "Page size should have a positive value.",
                [EmployeeValidator.Labels.SeNameMaxLengthValidation] = "Max length of search name is {0} chars",

                // Plugin's Dashboard Widget
                [HumanResourceDefaults.Labels.EmployeeStatistics] = "Employees",
                [HumanResourceDefaults.Labels.OYear] = "Year",
                [HumanResourceDefaults.Labels.OMonth] = "Month",
                [HumanResourceDefaults.Labels.OWeek] = "Week",
                [HumanResourceDefaults.Labels.ImportEmployees] = "{0} employees were imported",
                [HumanResourceDefaults.Labels.EmployeesArentImported] = "Employees with the following names aren't imported - {0}",

                [HumanResourceDefaults.Labels.EmployeeStatistics] = "Employee Numbers",

                [HumanResourceDefaults.Labels.HumanResourceNodeTitle] = "Human Resource"

            });
        }

        /// <summary>
        /// Install Plugins Permissions
        /// </summary>
        /// <returns></returns>
        private async Task InstallPermissionsAsync()
        {
            //register default permissions
            var permissionProviders = new List<Type> { typeof(APermissionProvider) };
                    foreach (var providerType in permissionProviders)
                    {
                        var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
            await _permissionService.InstallPermissionsAsync(provider);
        }
    }
    #endregion
}
}
