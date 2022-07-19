using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a category search model
    /// </summary>
    public partial record CategorySearchModel : BaseSearchModel
    {
        #region Labels
        public static class Labels {
            public const string Title = "Admin.Catalog.Categories";

            public const string DisplayOrder = "Admin.Catalog.Categories.Fields.DisplayOrder";
            public const string Published = "Admin.Catalog.Categories.Fields.Published";
            public const string Breadcrumb = "Admin.Catalog.Categories.Fields.Name";

            public const string SearchCategoryName = "Admin.Catalog.Categories.List.SearchCategoryName";
            public const string SearchStoreId = "Admin.Catalog.Categories.List.SearchStore";
            public const string ImportFromExcelTip = "Admin.Catalog.Categories.List.ImportFromExcelTip";

            public const string SearchPublishedId = "Admin.Catalog.Categories.List.SearchPublished";
            public const string All = "Admin.Catalog.Categories.List.SearchPublished.All";
            public const string PublishedOnly = "Admin.Catalog.Categories.List.SearchPublished.PublishedOnly";
            public const string UnpublishedOnly = "Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly";
        }

        #endregion
        #region Ctor

        public CategorySearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName(Labels.SearchCategoryName)]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName(Labels.SearchPublishedId)]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        [NopResourceDisplayName(Labels.SearchStoreId)]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}