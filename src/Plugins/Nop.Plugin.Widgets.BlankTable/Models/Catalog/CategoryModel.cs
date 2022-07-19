using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a category model
    /// </summary>
    public partial record CategoryModel : BaseNopEntityModel, IAclSupportedModel, IDiscountSupportedModel,
        ILocalizedModel<CategoryLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }

            Locales = new List<CategoryLocalizedModel>();
            AvailableCategoryTemplates = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            CategoryProductSearchModel = new CategoryProductSearchModel();
        }

        #endregion
        #region Labels
        public static partial class Labels {
            public const string Name = "Admin.Catalog.Categories.Fields.Name";
            public const string Description = "Admin.Catalog.Categories.Fields.Description";
            public const string CategoryTemplateId = "Admin.Catalog.Categories.Fields.CategoryTemplate";
            public const string MetaKeywords = "Admin.Catalog.Categories.Fields.MetaKeywords";
            public const string MetaDescription = "Admin.Catalog.Categories.Fields.MetaDescription";
            public const string MetaTitle = "Admin.Catalog.Categories.Fields.MetaTitle";
            public const string SeName = "Admin.Catalog.Categories.Fields.SeName";
            public const string ParentCategoryId = "Admin.Catalog.Categories.Fields.Parent";
            public const string PictureId = "Admin.Catalog.Categories.Fields.Picture";
            public const string PageSize = "Admin.Catalog.Categories.Fields.PageSize";
            public const string AllowCustomersToSelectPageSize = "Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize";
            public const string PageSizeOptions = "Admin.Catalog.Categories.Fields.PageSizeOptions";
            public const string PriceRangeFiltering = "Admin.Catalog.Categories.Fields.PriceRangeFiltering";
            public const string PriceFrom = "Admin.Catalog.Categories.Fields.PriceFrom";
            public const string PriceTo = "Admin.Catalog.Categories.Fields.PriceTo";
            public const string ManuallyPriceRange = "Admin.Catalog.Categories.Fields.ManuallyPriceRange";
            public const string ShowOnHomepage = "Admin.Catalog.Categories.Fields.ShowOnHomepage";
            public const string IncludeInTopMenu = "Admin.Catalog.Categories.Fields.IncludeInTopMenu";
            public const string Published = "Admin.Catalog.Categories.Fields.Published";
            public const string Deleted = "Admin.Catalog.Categories.Fields.Deleted";
            public const string DisplayOrder = "Admin.Catalog.Categories.Fields.DisplayOrder";
            public const string SelectedCustomerRoleIds = "Admin.Catalog.Categories.Fields.AclCustomerRoles";
            public const string SelectedStoreIds = "Admin.Catalog.Categories.Fields.LimitedToStores";
            public const string SelectedDiscountIds = "Admin.Catalog.Categories.Fields.Discounts";
            public const string None = "Admin.Catalog.Categories.Fields.Parent.None";

            //View Labels:
            public const string NoDiscounts = "Admin.Catalog.Categories.Fields.Discounts.NoDiscounts";
            public const string EditCategoryDetails = "Admin.Catalog.Categories.EditCategoryDetails";
            public const string BackToList = "Admin.Catalog.Categories.BackToList";
            public const string AddNew = "Admin.Catalog.Categories.AddNew";
            public const string Info = "Admin.Catalog.Categories.Info";
            public const string Display = "Admin.Catalog.Categories.Display";
            public const string Mappings = "Admin.Catalog.Categories.Mappings";
            public const string Products = "Admin.Catalog.Categories.Products";
        }
        #endregion

        #region Properties

        [NopResourceDisplayName(Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(Labels.Description)]
        public string Description { get; set; }

        [NopResourceDisplayName(Labels.CategoryTemplateId)]
        public int CategoryTemplateId { get; set; }
        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

        [NopResourceDisplayName(Labels.MetaKeywords)]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName(Labels.MetaDescription)]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName(Labels.MetaTitle)]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName(Labels.SeName)]
        public string SeName { get; set; }

        [NopResourceDisplayName(Labels.ParentCategoryId)]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName(Labels.PictureId)]
        public int PictureId { get; set; }

        [NopResourceDisplayName(Labels.PageSize)]
        public int PageSize { get; set; }

        [NopResourceDisplayName(Labels.AllowCustomersToSelectPageSize)]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName(Labels.PageSizeOptions)]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName(Labels.PriceRangeFiltering)]
        public bool PriceRangeFiltering { get; set; }

        [NopResourceDisplayName(Labels.PriceFrom)]
        public decimal PriceFrom { get; set; }

        [NopResourceDisplayName(Labels.PriceTo)]
        public decimal PriceTo { get; set; }

        [NopResourceDisplayName(Labels.ManuallyPriceRange)]
        public bool ManuallyPriceRange { get; set; }

        [NopResourceDisplayName(Labels.ShowOnHomepage)]
        public bool ShowOnHomepage { get; set; }

        [NopResourceDisplayName(Labels.IncludeInTopMenu)]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName(Labels.Published)]
        public bool Published { get; set; }

        [NopResourceDisplayName(Labels.Deleted)]
        public bool Deleted { get; set; }

        [NopResourceDisplayName(Labels.DisplayOrder)]
        public int DisplayOrder { get; set; }
        
        public IList<CategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName(Labels.SelectedCustomerRoleIds)]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        
        //store mapping
        [NopResourceDisplayName(Labels.SelectedStoreIds)]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        //discounts
        [NopResourceDisplayName(Labels.SelectedDiscountIds)]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        public CategoryProductSearchModel CategoryProductSearchModel { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }

    public partial record CategoryLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName(CategoryModel.Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(CategoryModel.Labels.Description)]
        public string Description {get;set;}

        [NopResourceDisplayName(CategoryModel.Labels.MetaKeywords)]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName(CategoryModel.Labels.MetaDescription)]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName(CategoryModel.Labels.MetaTitle)]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName(CategoryModel.Labels.SeName)]
        public string SeName { get; set; }
    }
}