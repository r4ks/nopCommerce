using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a product model
    /// </summary>
    public partial record ProductModel : BaseNopEntityModel, 
        IAclSupportedModel, IDiscountSupportedModel, ILocalizedModel<ProductLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public ProductModel()
        {
            Locales = new List<ProductLocalizedModel>();
            ProductEditorSettingsModel = new ProductEditorSettingsModel();

            AvailableBasepriceUnits = new List<SelectListItem>();
            AvailableBasepriceBaseUnits = new List<SelectListItem>();
            AvailableProductTemplates = new List<SelectListItem>();
            AvailableTaxCategories = new List<SelectListItem>();
            AvailableDeliveryDates = new List<SelectListItem>();
            AvailableProductAvailabilityRanges = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            ProductsTypesSupportedByProductTemplates = new Dictionary<int, IList<SelectListItem>>();

            AvailableVendors = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            SelectedManufacturerIds = new List<int>();
            AvailableManufacturers = new List<SelectListItem>();

            SelectedCategoryIds = new List<int>();
            AvailableCategories = new List<SelectListItem>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedDiscountIds = new List<int>();
            AvailableDiscounts = new List<SelectListItem>();
        }

        #endregion

        #region Labels
        public static class Labels {
            public const string PictureThumbnailUrl = "Admin.Catalog.Products.Fields.PictureThumbnailUrl";
            public const string ProductType = "Admin.Catalog.Products.Fields.ProductType";
            public const string AssociatedToProduct = "Admin.Catalog.Products.Fields.AssociatedToProductName";
            public const string VisibleIndividually = "Admin.Catalog.Products.Fields.VisibleIndividually";
            public const string ProductTemplateId = "Admin.Catalog.Products.Fields.ProductTemplate";
            public const string Name = "Admin.Catalog.Products.Fields.Name";
            public const string ShortDescription = "Admin.Catalog.Products.Fields.ShortDescription";
            public const string FullDescription = "Admin.Catalog.Products.Fields.FullDescription";
            public const string AdminComment = "Admin.Catalog.Products.Fields.AdminComment";
            public const string ShowOnHomepage = "Admin.Catalog.Products.Fields.ShowOnHomepage";
            public const string MetaKeywords = "Admin.Catalog.Products.Fields.MetaKeywords";
            public const string MetaDescription = "Admin.Catalog.Products.Fields.MetaDescription";
            public const string MetaTitle = "Admin.Catalog.Products.Fields.MetaTitle";
            public const string SeName = "Admin.Catalog.Products.Fields.SeName";
            public const string AllowCustomerReviews = "Admin.Catalog.Products.Fields.AllowCustomerReviews";
            public const string ProductTags = "Admin.Catalog.Products.Fields.ProductTags";
            public const string Sku = "Admin.Catalog.Products.Fields.Sku";
            public const string ManufacturerPartNumber = "Admin.Catalog.Products.Fields.ManufacturerPartNumber";
            public const string Gtin = "Admin.Catalog.Products.Fields.GTIN";
            public const string IsGiftCard = "Admin.Catalog.Products.Fields.IsGiftCard";
            public const string GiftCardTypeId = "Admin.Catalog.Products.Fields.GiftCardType";
            public const string OverriddenGiftCardAmount = "Admin.Catalog.Products.Fields.OverriddenGiftCardAmount";
            public const string RequireOtherProducts = "Admin.Catalog.Products.Fields.RequireOtherProducts";
            public const string RequiredProductIds = "Admin.Catalog.Products.Fields.RequiredProductIds";
            public const string AutomaticallyAddRequiredProducts = "Admin.Catalog.Products.Fields.AutomaticallyAddRequiredProducts";
            public const string IsDownload = "Admin.Catalog.Products.Fields.IsDownload";
            public const string DownloadId = "Admin.Catalog.Products.Fields.Download";
            public const string UnlimitedDownloads = "Admin.Catalog.Products.Fields.UnlimitedDownloads";
            public const string MaxNumberOfDownloads = "Admin.Catalog.Products.Fields.MaxNumberOfDownloads";
            public const string DownloadExpirationDays = "Admin.Catalog.Products.Fields.DownloadExpirationDays";
            public const string DownloadActivationTypeId = "Admin.Catalog.Products.Fields.DownloadActivationType";
            public const string HasSampleDownload = "Admin.Catalog.Products.Fields.HasSampleDownload";
            public const string SampleDownloadId = "Admin.Catalog.Products.Fields.SampleDownload";
            public const string HasUserAgreement = "Admin.Catalog.Products.Fields.HasUserAgreement";
            public const string UserAgreementText = "Admin.Catalog.Products.Fields.UserAgreementText";
            public const string IsRecurring = "Admin.Catalog.Products.Fields.IsRecurring";
            public const string RecurringCycleLength = "Admin.Catalog.Products.Fields.RecurringCycleLength";
            public const string RecurringCyclePeriodId = "Admin.Catalog.Products.Fields.RecurringCyclePeriod";
            public const string RecurringTotalCycles = "Admin.Catalog.Products.Fields.RecurringTotalCycles";
            public const string IsRental = "Admin.Catalog.Products.Fields.IsRental";
            public const string RentalPriceLength = "Admin.Catalog.Products.Fields.RentalPriceLength";
            public const string RentalPricePeriodId = "Admin.Catalog.Products.Fields.RentalPricePeriod";
            public const string IsShipEnabled = "Admin.Catalog.Products.Fields.IsShipEnabled";
            public const string IsFreeShipping = "Admin.Catalog.Products.Fields.IsFreeShipping";
            public const string ShipSeparately = "Admin.Catalog.Products.Fields.ShipSeparately";
            public const string AdditionalShippingCharge = "Admin.Catalog.Products.Fields.AdditionalShippingCharge";
            public const string DeliveryDateId = "Admin.Catalog.Products.Fields.DeliveryDate";
            public const string IsTaxExempt = "Admin.Catalog.Products.Fields.IsTaxExempt";
            public const string TaxCategoryId = "Admin.Catalog.Products.Fields.TaxCategory";
            public const string IsTelecommunicationsOrBroadcastingOrElectronicServices = "Admin.Catalog.Products.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices";
            public const string ManageInventoryMethodId = "Admin.Catalog.Products.Fields.ManageInventoryMethod";
            public const string ProductAvailabilityRangeId = "Admin.Catalog.Products.Fields.ProductAvailabilityRange";
            public const string UseMultipleWarehouses = "Admin.Catalog.Products.Fields.UseMultipleWarehouses";
            public const string WarehouseId = "Admin.Catalog.Products.Fields.Warehouse";
            public const string StockQuantity = "Admin.Catalog.Products.Fields.StockQuantity";
            public const string StockQuantityStr = "Admin.Catalog.Products.Fields.StockQuantity";
            public const string DisplayStockAvailability = "Admin.Catalog.Products.Fields.DisplayStockAvailability";
            public const string DisplayStockQuantity = "Admin.Catalog.Products.Fields.DisplayStockQuantity";
            public const string MinStockQuantity = "Admin.Catalog.Products.Fields.MinStockQuantity";
            public const string LowStockActivityId = "Admin.Catalog.Products.Fields.LowStockActivity";
            public const string NotifyAdminForQuantityBelow = "Admin.Catalog.Products.Fields.NotifyAdminForQuantityBelow";
            public const string BackorderModeId = "Admin.Catalog.Products.Fields.BackorderMode";
            public const string AllowBackInStockSubscriptions = "Admin.Catalog.Products.Fields.AllowBackInStockSubscriptions";
            public const string OrderMinimumQuantity = "Admin.Catalog.Products.Fields.OrderMinimumQuantity";
            public const string OrderMaximumQuantity = "Admin.Catalog.Products.Fields.OrderMaximumQuantity";
            public const string AllowedQuantities = "Admin.Catalog.Products.Fields.AllowedQuantities";
            public const string AllowAddingOnlyExistingAttributeCombinations = "Admin.Catalog.Products.Fields.AllowAddingOnlyExistingAttributeCombinations";
            public const string NotReturnable = "Admin.Catalog.Products.Fields.NotReturnable";
            public const string DisableBuyButton = "Admin.Catalog.Products.Fields.DisableBuyButton";
            public const string DisableWishlistButton = "Admin.Catalog.Products.Fields.DisableWishlistButton";
            public const string AvailableForPreOrder = "Admin.Catalog.Products.Fields.AvailableForPreOrder";
            public const string PreOrderAvailabilityStartDateTimeUtc = "Admin.Catalog.Products.Fields.PreOrderAvailabilityStartDateTimeUtc";
            public const string CallForPrice = "Admin.Catalog.Products.Fields.CallForPrice";
            public const string Price = "Admin.Catalog.Products.Fields.Price";
            public const string OldPrice = "Admin.Catalog.Products.Fields.OldPrice";
            public const string ProductCost = "Admin.Catalog.Products.Fields.ProductCost";
            public const string CustomerEntersPrice = "Admin.Catalog.Products.Fields.CustomerEntersPrice";
            public const string MinimumCustomerEnteredPrice = "Admin.Catalog.Products.Fields.MinimumCustomerEnteredPrice";
            public const string MaximumCustomerEnteredPrice = "Admin.Catalog.Products.Fields.MaximumCustomerEnteredPrice";
            public const string BasepriceEnabled = "Admin.Catalog.Products.Fields.BasepriceEnabled";
            public const string BasepriceAmount = "Admin.Catalog.Products.Fields.BasepriceAmount";
            public const string BasepriceUnitId = "Admin.Catalog.Products.Fields.BasepriceUnit";
            public const string BasepriceBaseAmount = "Admin.Catalog.Products.Fields.BasepriceBaseAmount";
            public const string BasepriceBaseUnitId = "Admin.Catalog.Products.Fields.BasepriceBaseUnit";
            public const string MarkAsNew = "Admin.Catalog.Products.Fields.MarkAsNew";
            public const string MarkAsNewStartDateTimeUtc = "Admin.Catalog.Products.Fields.MarkAsNewStartDateTimeUtc";
            public const string MarkAsNewEndDateTimeUtc = "Admin.Catalog.Products.Fields.MarkAsNewEndDateTimeUtc";
            public const string Weight = "Admin.Catalog.Products.Fields.Weight";
            public const string Length = "Admin.Catalog.Products.Fields.Length";
            public const string Width = "Admin.Catalog.Products.Fields.Width";
            public const string Height = "Admin.Catalog.Products.Fields.Height";
            public const string AvailableStartDateTimeUtc = "Admin.Catalog.Products.Fields.AvailableStartDateTime";
            public const string AvailableEndDateTimeUtc = "Admin.Catalog.Products.Fields.AvailableEndDateTime";
            public const string DisplayOrder = "Admin.Catalog.Products.Fields.DisplayOrder";
            public const string Published = "Admin.Catalog.Products.Fields.Published";
            public const string SelectedCustomerRoleIds = "Admin.Catalog.Products.Fields.AclCustomerRoles";
            public const string SelectedStoreIds = "Admin.Catalog.Products.Fields.LimitedToStores";
            public const string SelectedCategoryIds = "Admin.Catalog.Products.Fields.Categories";
            public const string SelectedManufacturerIds = "Admin.Catalog.Products.Fields.Manufacturers";
            public const string VendorId = "Admin.Catalog.Products.Fields.Vendor";
            public const string SelectedDiscountIds = "Admin.Catalog.Products.Fields.Discounts";
        }
        #endregion

        #region Properties
        
        //picture thumbnail
        [NopResourceDisplayName(Labels.PictureThumbnailUrl)]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName(Labels.ProductType)]
        public int ProductTypeId { get; set; }

        [NopResourceDisplayName(Labels.ProductType)]
        public string ProductTypeName { get; set; }

        [NopResourceDisplayName(Labels.AssociatedToProduct)]
        public int AssociatedToProductId { get; set; }

        [NopResourceDisplayName(Labels.AssociatedToProduct)]
        public string AssociatedToProductName { get; set; }

        [NopResourceDisplayName(Labels.VisibleIndividually)]
        public bool VisibleIndividually { get; set; }

        [NopResourceDisplayName(Labels.ProductTemplateId)]
        public int ProductTemplateId { get; set; }
        public IList<SelectListItem> AvailableProductTemplates { get; set; }

        //<product type ID, list of supported product template IDs>
        public Dictionary<int, IList<SelectListItem>> ProductsTypesSupportedByProductTemplates { get; set; }

        [NopResourceDisplayName(Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(Labels.ShortDescription)]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName(Labels.FullDescription)]
        public string FullDescription { get; set; }

        [NopResourceDisplayName(Labels.AdminComment)]
        public string AdminComment { get; set; }

        [NopResourceDisplayName(Labels.ShowOnHomepage)]
        public bool ShowOnHomepage { get; set; }

        [NopResourceDisplayName(Labels.MetaKeywords)]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName(Labels.MetaDescription)]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName(Labels.MetaTitle)]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName(Labels.SeName)]
        public string SeName { get; set; }

        [NopResourceDisplayName(Labels.AllowCustomerReviews)]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName(Labels.ProductTags)]
        public string ProductTags { get; set; }

        public string InitialProductTags { get; set; }

        [NopResourceDisplayName(Labels.Sku)]
        public string Sku { get; set; }

        [NopResourceDisplayName(Labels.ManufacturerPartNumber)]
        public string ManufacturerPartNumber { get; set; }

        [NopResourceDisplayName(Labels.Gtin)]
        public virtual string Gtin { get; set; }

        [NopResourceDisplayName(Labels.IsGiftCard)]
        public bool IsGiftCard { get; set; }

        [NopResourceDisplayName(Labels.GiftCardTypeId)]
        public int GiftCardTypeId { get; set; }

        [NopResourceDisplayName(Labels.OverriddenGiftCardAmount)]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenGiftCardAmount { get; set; }

        [NopResourceDisplayName(Labels.RequireOtherProducts)]
        public bool RequireOtherProducts { get; set; }

        [NopResourceDisplayName(Labels.RequiredProductIds)]
        public string RequiredProductIds { get; set; }

        [NopResourceDisplayName(Labels.AutomaticallyAddRequiredProducts)]
        public bool AutomaticallyAddRequiredProducts { get; set; }

        [NopResourceDisplayName(Labels.IsDownload)]
        public bool IsDownload { get; set; }

        [NopResourceDisplayName(Labels.DownloadId)]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [NopResourceDisplayName(Labels.UnlimitedDownloads)]
        public bool UnlimitedDownloads { get; set; }

        [NopResourceDisplayName(Labels.MaxNumberOfDownloads)]
        public int MaxNumberOfDownloads { get; set; }

        [NopResourceDisplayName(Labels.DownloadExpirationDays)]
        [UIHint("Int32Nullable")]
        public int? DownloadExpirationDays { get; set; }

        [NopResourceDisplayName(Labels.DownloadActivationTypeId)]
        public int DownloadActivationTypeId { get; set; }

        [NopResourceDisplayName(Labels.HasSampleDownload)]
        public bool HasSampleDownload { get; set; }

        [NopResourceDisplayName(Labels.SampleDownloadId)]
        [UIHint("Download")]
        public int SampleDownloadId { get; set; }

        [NopResourceDisplayName(Labels.HasUserAgreement)]
        public bool HasUserAgreement { get; set; }

        [NopResourceDisplayName(Labels.UserAgreementText)]
        public string UserAgreementText { get; set; }

        [NopResourceDisplayName(Labels.IsRecurring)]
        public bool IsRecurring { get; set; }

        [NopResourceDisplayName(Labels.RecurringCycleLength)]
        public int RecurringCycleLength { get; set; }

        [NopResourceDisplayName(Labels.RecurringCyclePeriodId)]
        public int RecurringCyclePeriodId { get; set; }

        [NopResourceDisplayName(Labels.RecurringTotalCycles)]
        public int RecurringTotalCycles { get; set; }

        [NopResourceDisplayName(Labels.IsRental)]
        public bool IsRental { get; set; }

        [NopResourceDisplayName(Labels.RentalPriceLength)]
        public int RentalPriceLength { get; set; }

        [NopResourceDisplayName(Labels.RentalPricePeriodId)]
        public int RentalPricePeriodId { get; set; }

        [NopResourceDisplayName(Labels.IsShipEnabled)]
        public bool IsShipEnabled { get; set; }

        [NopResourceDisplayName(Labels.IsFreeShipping)]
        public bool IsFreeShipping { get; set; }

        [NopResourceDisplayName(Labels.ShipSeparately)]
        public bool ShipSeparately { get; set; }

        [NopResourceDisplayName(Labels.AdditionalShippingCharge)]
        public decimal AdditionalShippingCharge { get; set; }

        [NopResourceDisplayName(Labels.DeliveryDateId)]
        public int DeliveryDateId { get; set; }
        public IList<SelectListItem> AvailableDeliveryDates { get; set; }

        [NopResourceDisplayName(Labels.IsTaxExempt)]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName(Labels.TaxCategoryId)]
        public int TaxCategoryId { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }

        [NopResourceDisplayName(Labels.IsTelecommunicationsOrBroadcastingOrElectronicServices)]
        public bool IsTelecommunicationsOrBroadcastingOrElectronicServices { get; set; }

        [NopResourceDisplayName(Labels.ManageInventoryMethodId)]
        public int ManageInventoryMethodId { get; set; }

        [NopResourceDisplayName(Labels.ProductAvailabilityRangeId)]
        public int ProductAvailabilityRangeId { get; set; }
        public IList<SelectListItem> AvailableProductAvailabilityRanges { get; set; }

        [NopResourceDisplayName(Labels.UseMultipleWarehouses)]
        public bool UseMultipleWarehouses { get; set; }

        [NopResourceDisplayName(Labels.WarehouseId)]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [NopResourceDisplayName(Labels.StockQuantity)]
        public int StockQuantity { get; set; }

        public int LastStockQuantity { get; set; }

        [NopResourceDisplayName(Labels.StockQuantity)]
        public string StockQuantityStr { get; set; }

        [NopResourceDisplayName(Labels.DisplayStockAvailability)]
        public bool DisplayStockAvailability { get; set; }

        [NopResourceDisplayName(Labels.DisplayStockQuantity)]
        public bool DisplayStockQuantity { get; set; }

        [NopResourceDisplayName(Labels.MinStockQuantity)]
        public int MinStockQuantity { get; set; }

        [NopResourceDisplayName(Labels.LowStockActivityId)]
        public int LowStockActivityId { get; set; }

        [NopResourceDisplayName(Labels.NotifyAdminForQuantityBelow)]
        public int NotifyAdminForQuantityBelow { get; set; }

        [NopResourceDisplayName(Labels.BackorderModeId)]
        public int BackorderModeId { get; set; }

        [NopResourceDisplayName(Labels.AllowBackInStockSubscriptions)]
        public bool AllowBackInStockSubscriptions { get; set; }

        [NopResourceDisplayName(Labels.OrderMinimumQuantity)]
        public int OrderMinimumQuantity { get; set; }

        [NopResourceDisplayName(Labels.OrderMaximumQuantity)]
        public int OrderMaximumQuantity { get; set; }

        [NopResourceDisplayName(Labels.AllowedQuantities)]
        public string AllowedQuantities { get; set; }

        [NopResourceDisplayName(Labels.AllowAddingOnlyExistingAttributeCombinations)]
        public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

        [NopResourceDisplayName(Labels.NotReturnable)]
        public bool NotReturnable { get; set; }

        [NopResourceDisplayName(Labels.DisableBuyButton)]
        public bool DisableBuyButton { get; set; }

        [NopResourceDisplayName(Labels.DisableWishlistButton)]
        public bool DisableWishlistButton { get; set; }

        [NopResourceDisplayName(Labels.AvailableForPreOrder)]
        public bool AvailableForPreOrder { get; set; }

        [NopResourceDisplayName(Labels.PreOrderAvailabilityStartDateTimeUtc)]
        [UIHint("DateTimeNullable")]
        public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

        [NopResourceDisplayName(Labels.CallForPrice)]
        public bool CallForPrice { get; set; }

        [NopResourceDisplayName(Labels.Price)]
        public decimal Price { get; set; }

        [NopResourceDisplayName(Labels.OldPrice)]
        public decimal OldPrice { get; set; }

        [NopResourceDisplayName(Labels.ProductCost)]
        public decimal ProductCost { get; set; }

        [NopResourceDisplayName(Labels.CustomerEntersPrice)]
        public bool CustomerEntersPrice { get; set; }

        [NopResourceDisplayName(Labels.MinimumCustomerEnteredPrice)]
        public decimal MinimumCustomerEnteredPrice { get; set; }

        [NopResourceDisplayName(Labels.MaximumCustomerEnteredPrice)]
        public decimal MaximumCustomerEnteredPrice { get; set; }

        [NopResourceDisplayName(Labels.BasepriceEnabled)]
        public bool BasepriceEnabled { get; set; }

        [NopResourceDisplayName(Labels.BasepriceAmount)]
        public decimal BasepriceAmount { get; set; }

        [NopResourceDisplayName(Labels.BasepriceUnitId)]
        public int BasepriceUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceUnits { get; set; }

        [NopResourceDisplayName(Labels.BasepriceBaseAmount)]
        public decimal BasepriceBaseAmount { get; set; }

        [NopResourceDisplayName(Labels.BasepriceBaseUnitId)]
        public int BasepriceBaseUnitId { get; set; }
        public IList<SelectListItem> AvailableBasepriceBaseUnits { get; set; }

        [NopResourceDisplayName(Labels.MarkAsNew)]
        public bool MarkAsNew { get; set; }

        [NopResourceDisplayName(Labels.MarkAsNewStartDateTimeUtc)]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }

        [NopResourceDisplayName(Labels.MarkAsNewEndDateTimeUtc)]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        [NopResourceDisplayName(Labels.Weight)]
        public decimal Weight { get; set; }

        [NopResourceDisplayName(Labels.Length)]
        public decimal Length { get; set; }

        [NopResourceDisplayName(Labels.Width)]
        public decimal Width { get; set; }

        [NopResourceDisplayName(Labels.Height)]
        public decimal Height { get; set; }

        [NopResourceDisplayName(Labels.AvailableStartDateTimeUtc)]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [NopResourceDisplayName(Labels.AvailableEndDateTimeUtc)]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        [NopResourceDisplayName(Labels.DisplayOrder)]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName(Labels.Published)]
        public bool Published { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public string BaseDimensionIn { get; set; }

        public string BaseWeightIn { get; set; }

        public IList<ProductLocalizedModel> Locales { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName(Labels.SelectedCustomerRoleIds)]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName(Labels.SelectedStoreIds)]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //categories
        [NopResourceDisplayName(Labels.SelectedCategoryIds)]
        public IList<int> SelectedCategoryIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        //manufacturers
        [NopResourceDisplayName(Labels.SelectedManufacturerIds)]
        public IList<int> SelectedManufacturerIds { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }

        //vendors
        [NopResourceDisplayName(Labels.VendorId)]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        //discounts
        [NopResourceDisplayName(Labels.SelectedDiscountIds)]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        //product attributes
        public bool ProductAttributesExist { get; set; }
        public bool CanCreateCombinations { get; set; }

        //specification attributes
        public bool HasAvailableSpecificationAttributes { get; set; }

        //copy product
        // public CopyProductModel CopyProductModel { get; set; }

        //editor settings
        public ProductEditorSettingsModel ProductEditorSettingsModel { get; set; }

        #endregion
    }

    public partial record ProductLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.ShortDescription)]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.FullDescription)]
        public string FullDescription { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.MetaKeywords)]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.MetaDescription)]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.MetaTitle)]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName(ProductModel.Labels.SeName)]
        public string SeName { get; set; }
    }
}