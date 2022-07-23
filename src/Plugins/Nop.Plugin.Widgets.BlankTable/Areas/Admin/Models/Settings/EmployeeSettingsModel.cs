using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a catalog settings model
    /// </summary>
    public partial record EmployeeSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Labels
        public static class Labels
        {

        }
        #endregion

        #region Ctor

        public EmployeeSettingsModel()
        {
            AvailableViewModes = new List<SelectListItem>();
            SortOptionSearchModel = new SortOptionSearchModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DefaultViewMode")]
        public string DefaultViewMode { get; set; }
        public bool DefaultViewMode_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PageShareCode")]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize")]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        public bool SearchPageAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportEmployeesUsingEmployeeName")]
        public bool ExportImportEmployeesUsingEmployeeName { get; set; }
        public bool ExportImportEmployeesUsingEmployeeName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages")]
        public bool ExportImportAllowDownloadImages { get; set; }
        public bool ExportImportAllowDownloadImages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName")]
        public bool ExportImportRelatedEntitiesByName { get; set; }
        public bool ExportImportRelatedEntitiesByName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability")]
        public bool DisplayDatePreOrderAvailability { get; set; }
        public bool DisplayDatePreOrderAvailability_OverrideForStore { get; set; }

        public SortOptionSearchModel SortOptionSearchModel { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering")]
        public bool EnableSpecificationAttributeFiltering { get; set; }
        public bool EnableSpecificationAttributeFiltering_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages")]
        public bool DisplayAllPicturesOnCatalogPages { get; set; }
        public bool DisplayAllPicturesOnCatalogPages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName")]
        public bool AllowCustomersToSearchWithEmployeeName { get; set; }
        public bool AllowCustomersToSearchWithEmployeeName_OverrideForStore { get; set; }
        #endregion
    }
}