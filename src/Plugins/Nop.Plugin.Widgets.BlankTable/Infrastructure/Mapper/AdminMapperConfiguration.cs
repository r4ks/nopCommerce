using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Settings;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Services.Plugins;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {
            //create specific maps
            CreateCatalogMaps();


            //add some generic mapping rules
            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseNopModel
                if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                //exclude some properties from mapping configuration and models
                if (typeof(IConfig).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IConfig.Name), options => options.Ignore());

                //exclude Locales from mapping ILocalizedModel
                if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());

                //exclude some properties from mapping store mapping supported entities and models
                if (typeof(IStoreMappingSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IStoreMappingSupported.LimitedToStores), options => options.Ignore());
                if (typeof(IStoreMappingSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IStoreMappingSupportedModel.AvailableStores), options => options.Ignore());
                    map.ForMember(nameof(IStoreMappingSupportedModel.SelectedStoreIds), options => options.Ignore());
                }

                //exclude some properties from mapping ACL supported entities and models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableCustomerRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedCustomerRoleIds), options => options.Ignore());
                }

                //exclude some properties from mapping discount supported entities and models
                if (typeof(IDiscountSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IDiscountSupportedModel.AvailableDiscounts), options => options.Ignore());
                    map.ForMember(nameof(IDiscountSupportedModel.SelectedDiscountIds), options => options.Ignore());
                }

                if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //exclude some properties from mapping plugin models
                    map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

                    //define specific rules for mapping plugin models
                    if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
                    {
                        map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                        map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                        map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
                    }
                }
            });
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Create catalog maps 
        /// </summary>
        protected virtual void CreateCatalogMaps()
        {
            CreateMap<EmployeeSettings, EmployeeSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowAnonymousUsersToReviewProduct_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowProductSorting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowProductViewModeChanging_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowViewUnpublishedProductPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AvailableViewModes, options => options.Ignore())
                .ForMember(model => model.CategoryBreadcrumbEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CompareProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultViewMode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoProductBoxes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EmailAFriendEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportAllowDownloadImages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportCategoriesUsingCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductCategoryBreadcrumb_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductSpecificationAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportRelatedEntitiesByName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportProductUseLimitedToStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportSplitProductsFile_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeFullDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeShortDescriptionInCompareProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturersBlockItemsToDisplay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewProductsPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyCustomerAboutProductReviewReply_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfProductTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageShareCode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OneReviewPerProductFromCustomer_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsPageSizeOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductReviewsSortByCreatedDateAscending_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsAlsoPurchasedNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductsByTagPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchAutoCompleteEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductSearchTermMinimumLength_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedProductsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedProductsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RemoveRequiredProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPagePageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageProductsPerPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryProductNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowFreeShippingNotification_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShortDescriptionOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowGtin_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowManufacturerPartNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductImagesInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductReviewsOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductReviewsPerStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowProductsFromSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShareButton_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnProductDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDatePreOrderAvailability_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.UseAjaxCatalogProductsLoading_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPageManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagPriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductsByTagManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableManufacturerFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnablePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableSpecificationAttributeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.DisplayFromPrices_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayTypes, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayType_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SortOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReviewTypeSearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToSearchWithManufacturerName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCustomersToSearchWithCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayAllPicturesOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ProductUrlStructureTypeId_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.ProductUrlStructureTypes, mo => mo.Ignore());
            CreateMap<EmployeeSettingsModel, EmployeeSettings>()
                .ForMember(settings => settings.AjaxProcessAttributeChange, options => options.Ignore())
                .ForMember(settings => settings.CompareProductsNumber, options => options.Ignore())
                .ForMember(settings => settings.CountDisplayedYearsDatePicker, options => options.Ignore())
                .ForMember(settings => settings.DefaultEmployeePageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultEmployeePageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultProductRatingValue, options => options.Ignore())
                .ForMember(settings => settings.DisplayTierPricesWithDiscounts, options => options.Ignore())
                .ForMember(settings => settings.ExportImportProductsCountInOneFile, options => options.Ignore())
                .ForMember(settings => settings.ExportImportUseDropdownlistsForAssociatedEntities, options => options.Ignore())
                .ForMember(settings => settings.IncludeFeaturedProductsInNormalLists, options => options.Ignore())
                .ForMember(settings => settings.MaximumBackInStockSubscriptions, options => options.Ignore())
                .ForMember(settings => settings.PublishBackProductWhenCancellingOrders, options => options.Ignore())
                .ForMember(settings => settings.UseAjaxLoadMenu, options => options.Ignore())
                .ForMember(settings => settings.UseLinksInRequiredProductWarnings, options => options.Ignore());


            CreateMap<Employee, EmployeeModel>()
                .ForMember(model => model.AvailableEmployees, options => options.Ignore())
                .ForMember(model => model.Breadcrumb, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<EmployeeModel, Employee>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

        }

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}