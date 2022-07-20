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
            public const string Prefix = "Admin.Catalog.Products.Fields";
            public const string PictureThumbnailUrl = Prefix + ".PictureThumbnailUrl";
            public const string ProductType = Prefix + ".ProductType";
            public const string AssociatedToProduct = Prefix + ".AssociatedToProductName";
            public const string VisibleIndividually = Prefix + ".VisibleIndividually";
            public const string ProductTemplateId = Prefix + ".ProductTemplate";
            public const string Name = Prefix + ".Name";
            public const string ShortDescription = Prefix + ".ShortDescription";
            public const string FullDescription = Prefix + ".FullDescription";
            public const string AdminComment = Prefix + ".AdminComment";
            public const string ShowOnHomepage = Prefix + ".ShowOnHomepage";
            public const string MetaKeywords = Prefix + ".MetaKeywords";
            public const string MetaDescription = Prefix + ".MetaDescription";
            public const string MetaTitle = Prefix + ".MetaTitle";
            public const string SeName = Prefix + ".SeName";
            public const string AllowCustomerReviews = Prefix + ".AllowCustomerReviews";
            public const string ProductTags = Prefix + ".ProductTags";
            public const string Sku = Prefix + ".Sku";
            public const string ManufacturerPartNumber = Prefix + ".ManufacturerPartNumber";
            public const string Gtin = Prefix + ".GTIN";
            public const string IsGiftCard = Prefix + ".IsGiftCard";
            public const string GiftCardTypeId = Prefix + ".GiftCardType";
            public const string OverriddenGiftCardAmount = Prefix + ".OverriddenGiftCardAmount";
            public const string RequireOtherProducts = Prefix + ".RequireOtherProducts";
            public const string RequiredProductIds = Prefix + ".RequiredProductIds";
            public const string AutomaticallyAddRequiredProducts = Prefix + ".AutomaticallyAddRequiredProducts";
            public const string IsDownload = Prefix + ".IsDownload";
            public const string DownloadId = Prefix + ".Download";
            public const string UnlimitedDownloads = Prefix + ".UnlimitedDownloads";
            public const string MaxNumberOfDownloads = Prefix + ".MaxNumberOfDownloads";
            public const string DownloadExpirationDays = Prefix + ".DownloadExpirationDays";
            public const string DownloadActivationTypeId = Prefix + ".DownloadActivationType";
            public const string HasSampleDownload = Prefix + ".HasSampleDownload";
            public const string SampleDownloadId = Prefix + ".SampleDownload";
            public const string HasUserAgreement = Prefix + ".HasUserAgreement";
            public const string UserAgreementText = Prefix + ".UserAgreementText";
            public const string IsRecurring = Prefix + ".IsRecurring";
            public const string RecurringCycleLength = Prefix + ".RecurringCycleLength";
            public const string RecurringCyclePeriodId = Prefix + ".RecurringCyclePeriod";
            public const string RecurringTotalCycles = Prefix + ".RecurringTotalCycles";
            public const string IsRental = Prefix + ".IsRental";
            public const string RentalPriceLength = Prefix + ".RentalPriceLength";
            public const string RentalPricePeriodId = Prefix + ".RentalPricePeriod";
            public const string IsShipEnabled = Prefix + ".IsShipEnabled";
            public const string IsFreeShipping = Prefix + ".IsFreeShipping";
            public const string ShipSeparately = Prefix + ".ShipSeparately";
            public const string AdditionalShippingCharge = Prefix + ".AdditionalShippingCharge";
            public const string DeliveryDateId = Prefix + ".DeliveryDate";
            public const string IsTaxExempt = Prefix + ".IsTaxExempt";
            public const string TaxCategoryId = Prefix + ".TaxCategory";
            public const string IsTelecommunicationsOrBroadcastingOrElectronicServices = Prefix + ".IsTelecommunicationsOrBroadcastingOrElectronicServices";
            public const string ManageInventoryMethodId = Prefix + ".ManageInventoryMethod";
            public const string ProductAvailabilityRangeId = Prefix + ".ProductAvailabilityRange";
            public const string UseMultipleWarehouses = Prefix + ".UseMultipleWarehouses";
            public const string WarehouseId = Prefix + ".Warehouse";
            public const string StockQuantity = Prefix + ".StockQuantity";
            public const string StockQuantityStr = Prefix + ".StockQuantity";
            public const string DisplayStockAvailability = Prefix + ".DisplayStockAvailability";
            public const string DisplayStockQuantity = Prefix + ".DisplayStockQuantity";
            public const string MinStockQuantity = Prefix + ".MinStockQuantity";
            public const string LowStockActivityId = Prefix + ".LowStockActivity";
            public const string NotifyAdminForQuantityBelow = Prefix + ".NotifyAdminForQuantityBelow";
            public const string BackorderModeId = Prefix + ".BackorderMode";
            public const string AllowBackInStockSubscriptions = Prefix + ".AllowBackInStockSubscriptions";
            public const string OrderMinimumQuantity = Prefix + ".OrderMinimumQuantity";
            public const string OrderMaximumQuantity = Prefix + ".OrderMaximumQuantity";
            public const string AllowedQuantities = Prefix + ".AllowedQuantities";
            public const string AllowAddingOnlyExistingAttributeCombinations = Prefix + ".AllowAddingOnlyExistingAttributeCombinations";
            public const string NotReturnable = Prefix + ".NotReturnable";
            public const string DisableBuyButton = Prefix + ".DisableBuyButton";
            public const string DisableWishlistButton = Prefix + ".DisableWishlistButton";
            public const string AvailableForPreOrder = Prefix + ".AvailableForPreOrder";
            public const string PreOrderAvailabilityStartDateTimeUtc = Prefix + ".PreOrderAvailabilityStartDateTimeUtc";
            public const string CallForPrice = Prefix + ".CallForPrice";
            public const string Price = Prefix + ".Price";
            public const string OldPrice = Prefix + ".OldPrice";
            public const string ProductCost = Prefix + ".ProductCost";
            public const string CustomerEntersPrice = Prefix + ".CustomerEntersPrice";
            public const string MinimumCustomerEnteredPrice = Prefix + ".MinimumCustomerEnteredPrice";
            public const string MaximumCustomerEnteredPrice = Prefix + ".MaximumCustomerEnteredPrice";
            public const string BasepriceEnabled = Prefix + ".BasepriceEnabled";
            public const string BasepriceAmount = Prefix + ".BasepriceAmount";
            public const string BasepriceUnitId = Prefix + ".BasepriceUnit";
            public const string BasepriceBaseAmount = Prefix + ".BasepriceBaseAmount";
            public const string BasepriceBaseUnitId = Prefix + ".BasepriceBaseUnit";
            public const string MarkAsNew = Prefix + ".MarkAsNew";
            public const string MarkAsNewStartDateTimeUtc = Prefix + ".MarkAsNewStartDateTimeUtc";
            public const string MarkAsNewEndDateTimeUtc = Prefix + ".MarkAsNewEndDateTimeUtc";
            public const string Weight = Prefix + ".Weight";
            public const string Length = Prefix + ".Length";
            public const string Width = Prefix + ".Width";
            public const string Height = Prefix + ".Height";
            public const string AvailableStartDateTimeUtc = Prefix + ".AvailableStartDateTime";
            public const string AvailableEndDateTimeUtc = Prefix + ".AvailableEndDateTime";
            public const string DisplayOrder = Prefix + ".DisplayOrder";
            public const string Published = Prefix + ".Published";
            public const string SelectedCustomerRoleIds = Prefix + ".AclCustomerRoles";
            public const string SelectedStoreIds = Prefix + ".LimitedToStores";
            public const string SelectedCategoryIds = Prefix + ".Categories";
            public const string SelectedManufacturerIds = Prefix + ".Manufacturers";
            public const string VendorId = Prefix + ".Vendor";
            public const string SelectedDiscountIds = Prefix + ".Discounts";
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