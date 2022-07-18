using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a category product model
    /// </summary>
    public partial record CategoryProductModel : BaseNopEntityModel
    {
        #region Labels
        public static class Labels {
            public const string ProductName = "Admin.Catalog.Categories.Products.Fields.Product";
            public const string IsFeaturedProduct = "Admin.Catalog.Categories.Products.Fields.IsFeaturedProduct";
            public const string DisplayOrder = "Admin.Catalog.Categories.Products.Fields.DisplayOrder";
            public const string AddNew = "Admin.Catalog.Categories.Products.AddNew";
            public const string SaveBeforeEdit = "Admin.Catalog.Categories.Products.SaveBeforeEdit";
        }
        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public int ProductId { get; set; }

        [NopResourceDisplayName(Labels.ProductName)]
        public string ProductName { get; set; }

        [NopResourceDisplayName(Labels.IsFeaturedProduct)]
        public bool IsFeaturedProduct { get; set; }

        [NopResourceDisplayName(Labels.DisplayOrder)]
        public int DisplayOrder { get; set; }

        #endregion
    }
}