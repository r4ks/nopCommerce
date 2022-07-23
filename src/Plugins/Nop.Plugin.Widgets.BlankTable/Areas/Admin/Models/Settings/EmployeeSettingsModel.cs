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
        #region Models View Path
        public const string View = "~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Setting/Employee.cshtml";
        #endregion

        #region Labels
        public static class Labels
        {
            public const string Title = "Admin.Configuration.Settings.Catalog";

            public const string EditSettings = "ActivityLog.EditSettings";
            public const string Updated = "Admin.Configuration.Updated";
            public const string Grid = "Admin.Catalog.ViewMode.Grid";
            public const string List = "Admin.Catalog.ViewMode.List";

            public const string DefaultViewMode = "Admin.Configuration.Settings.Catalog.DefaultViewMode";
            public const string ShowShareButton = "Admin.Configuration.Settings.Catalog.ShowShareButton";
            public const string PageShareCode = "Admin.Configuration.Settings.Catalog.PageShareCode";
            public const string EmailAFriendEnabled = "Admin.Configuration.Settings.Catalog.EmailAFriendEnabled";
            public const string AllowAnonymousUsersToEmailAFriend = "Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend";
            public const string SearchPageAllowCustomersToSelectPageSize = "Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize";
            public const string SearchPagePageSizeOptions = "Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions";
            public const string ExportImportEmployeesUsingEmployeeName = "Admin.Configuration.Settings.Catalog.ExportImportEmployeesUsingEmployeeName";
            public const string ExportImportAllowDownloadImages = "Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages";
            public const string ExportImportRelatedEntitiesByName = "Admin.Configuration.Settings.Catalog.ExportImportRelatedEntitiesByName";
            public const string CategoryBreadcrumbEnabled = "Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled";
            public const string IgnoreAcl = "Admin.Configuration.Settings.Catalog.IgnoreAcl";
            public const string IgnoreStoreLimitations = "Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations";
            public const string DisplayDatePreOrderAvailability = "Admin.Configuration.Settings.Catalog.DisplayDatePreOrderAvailability";
            public const string EnableSpecificationAttributeFiltering = "Admin.Configuration.Settings.Catalog.EnableSpecificationAttributeFiltering";
            public const string DisplayAllPicturesOnCatalogPages = "Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages";
            public const string AllowCustomersToSearchWithEmployeeName = "Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName";

            public const string Search = "Admin.Configuration.Settings.Catalog.BlockTitle.Search";
            public const string Performance = "Admin.Configuration.Settings.Catalog.BlockTitle.Performance";
            public const string Share = "Admin.Configuration.Settings.Catalog.BlockTitle.Share";
            public const string AdditionalSections = "Admin.Configuration.Settings.Catalog.BlockTitle.AdditionalSections";
            public const string CatalogPages = "Admin.Configuration.Settings.Catalog.BlockTitle.CatalogPages";
            public const string ExportImport = "Admin.Configuration.Settings.Catalog.BlockTitle.ExportImport";
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

        [NopResourceDisplayName(Labels.DefaultViewMode)]
        public string DefaultViewMode { get; set; }
        public bool DefaultViewMode_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        [NopResourceDisplayName(Labels.ShowShareButton)]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.PageShareCode)]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.EmailAFriendEnabled)]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.AllowAnonymousUsersToEmailAFriend)]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.SearchPageAllowCustomersToSelectPageSize)]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        public bool SearchPageAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.SearchPagePageSizeOptions)]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.ExportImportEmployeesUsingEmployeeName)]
        public bool ExportImportEmployeesUsingEmployeeName { get; set; }
        public bool ExportImportEmployeesUsingEmployeeName_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.ExportImportAllowDownloadImages)]
        public bool ExportImportAllowDownloadImages { get; set; }
        public bool ExportImportAllowDownloadImages_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.ExportImportRelatedEntitiesByName)]
        public bool ExportImportRelatedEntitiesByName { get; set; }
        public bool ExportImportRelatedEntitiesByName_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.CategoryBreadcrumbEnabled)]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.IgnoreAcl)]
        public bool IgnoreAcl { get; set; }

        [NopResourceDisplayName(Labels.IgnoreStoreLimitations)]
        public bool IgnoreStoreLimitations { get; set; }

        [NopResourceDisplayName(Labels.DisplayDatePreOrderAvailability)]
        public bool DisplayDatePreOrderAvailability { get; set; }
        public bool DisplayDatePreOrderAvailability_OverrideForStore { get; set; }

        public SortOptionSearchModel SortOptionSearchModel { get; set; }


        [NopResourceDisplayName(Labels.EnableSpecificationAttributeFiltering)]
        public bool EnableSpecificationAttributeFiltering { get; set; }
        public bool EnableSpecificationAttributeFiltering_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        [NopResourceDisplayName(Labels.DisplayAllPicturesOnCatalogPages)]
        public bool DisplayAllPicturesOnCatalogPages { get; set; }
        public bool DisplayAllPicturesOnCatalogPages_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.AllowCustomersToSearchWithEmployeeName)]
        public bool AllowCustomersToSearchWithEmployeeName { get; set; }
        public bool AllowCustomersToSearchWithEmployeeName_OverrideForStore { get; set; }
        #endregion
    }
}