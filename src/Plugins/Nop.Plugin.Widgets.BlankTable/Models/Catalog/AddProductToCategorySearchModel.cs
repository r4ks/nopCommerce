using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a product search model to add to the category
    /// </summary>
    public partial record AddProductToCategorySearchModel : BaseSearchModel
    {
        #region Labels
        public static partial class Labels
        {
            public const string SearchProductName = "Admin.Catalog.Products.List.SearchProductName";
            public const string SearchCategoryId = "Admin.Catalog.Products.List.SearchCategory";
            public const string SearchManufacturerId = "Admin.Catalog.Products.List.SearchManufacturer";
            public const string SearchStoreId = "Admin.Catalog.Products.List.SearchStore";
            public const string SearchVendorId = "Admin.Catalog.Products.List.SearchVendor";

            // View Labels:
            public const string Name = "Admin.Catalog.Products.Fields.Name";
            public const string Published = "Admin.Catalog.Products.Fields.Published";
        }
        #endregion
        #region Ctor

        public AddProductToCategorySearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName(Labels.SearchProductName)]
        public string SearchProductName { get; set; }

        [NopResourceDisplayName(Labels.SearchCategoryId)]
        public int SearchCategoryId { get; set; }

        [NopResourceDisplayName(Labels.SearchManufacturerId)]
        public int SearchManufacturerId { get; set; }

        [NopResourceDisplayName(Labels.SearchStoreId)]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName(Labels.SearchVendorId)]
        public int SearchVendorId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        #endregion
    }
}