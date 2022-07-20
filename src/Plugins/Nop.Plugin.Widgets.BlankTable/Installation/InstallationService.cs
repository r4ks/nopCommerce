using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Core.Security;
using Nop.Data;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;
using Nop.Plugin.Widgets.BlankTable.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Installation;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Widgets.BlankTable.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        private readonly INopDataProvider _dataProvider;
        private readonly INopFileProvider _fileProvider;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;

        #endregion

        #region Ctor

        public InstallationService(INopDataProvider dataProvider,
            INopFileProvider fileProvider,
            IRepository<Category> categoryRepository,
            IRepository<UrlRecord> urlRecordRepository
            )
        {
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _categoryRepository = categoryRepository;
            _urlRecordRepository = urlRecordRepository;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            return await _dataProvider.InsertEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : BaseEntity
        {
            await _dataProvider.BulkInsertEntitiesAsync(entities);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            await InsertInstallationDataAsync(entities.ToArray());
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            foreach (var entity in entities)
                await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
        {
            //duplicate of ValidateSeName method of \Nop.Services\Seo\UrlRecordService.cs (we cannot inject it here)
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            seName = seName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in seName.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }

            seName = sb.ToString();
            seName = seName.Replace(" ", "-");
            while (seName.Contains("--"))
                seName = seName.Replace("--", "-");
            while (seName.Contains("__"))
                seName = seName.Replace("__", "_");

            //max length
            seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

            //ensure this seName is not reserved yet
            var i = 2;
            var tempSeName = seName;
            while (true)
            {
                //check whether such slug already exists (and that is not the current entity)

                var query = from ur in _urlRecordRepository.Table
                            where tempSeName != null && ur.Slug == tempSeName
                            select ur;
                var urlRecord = await query.FirstOrDefaultAsync();

                var entityName = entity.GetType().Name;
                var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
                if (!reserved)
                    break;

                tempSeName = $"{seName}-{i}";
                i++;
            }

            seName = tempSeName;

            return seName;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
        }


        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InstallSettingsAsync(RegionInfo regionInfo)
        {
            var isMetric = regionInfo?.IsMetric ?? false;
            var country = regionInfo?.TwoLetterISORegionName ?? string.Empty;
            var isGermany = country == "DE";
            var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            await settingService.SaveSettingAsync(new CatalogSettings
            {
                AllowViewUnpublishedProductPage = true,
                DisplayDiscontinuedMessageForUnpublishedProducts = true,
                PublishBackProductWhenCancellingOrders = false,
                ShowSkuOnProductDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                ShowManufacturerPartNumber = false,
                ShowGtin = false,
                ShowFreeShippingNotification = true,
                ShowShortDescriptionOnCatalogPages = false,
                AllowProductSorting = true,
                AllowProductViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowProductsFromSubcategories = false,
                ShowCategoryProductNumber = false,
                ShowCategoryProductNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode =
                    "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                ProductReviewsMustBeApproved = false,
                OneReviewPerProductFromCustomer = false,
                DefaultProductRatingValue = 5,
                AllowAnonymousUsersToReviewProduct = false,
                ProductReviewPossibleOnlyAfterPurchasing = false,
                NotifyStoreOwnerAboutNewProductReviews = false,
                NotifyCustomerAboutProductReviewReply = false,
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedProductsNumber = 3,
                RecentlyViewedProductsEnabled = true,
                NewProductsEnabled = true,
                NewProductsPageSize = 6,
                NewProductsAllowCustomersToSelectPageSize = true,
                NewProductsPageSizeOptions = "6, 3, 9",
                CompareProductsEnabled = true,
                CompareProductsNumber = 4,
                ProductSearchAutoCompleteEnabled = true,
                ProductSearchEnabled = true,
                ProductSearchAutoCompleteNumberOfProducts = 10,
                ShowLinkToAllResultInSearchAutoComplete = false,
                ProductSearchTermMinimumLength = 3,
                ShowProductImagesInSearchAutoComplete = false,
                ShowBestsellersOnHomepage = false,
                NumberOfBestsellersOnHomepage = 4,
                SearchPageProductsPerPage = 6,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                SearchPagePriceRangeFiltering = true,
                SearchPageManuallyPriceRange = true,
                SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                ProductsAlsoPurchasedEnabled = true,
                ProductsAlsoPurchasedNumber = 4,
                AjaxProcessAttributeChange = true,
                NumberOfProductTags = 15,
                ProductsByTagPageSize = 6,
                IncludeShortDescriptionInCompareProducts = false,
                IncludeFullDescriptionInCompareProducts = false,
                IncludeFeaturedProductsInNormalLists = false,
                UseLinksInRequiredProductWarnings = true,
                DisplayTierPricesWithDiscounts = true,
                IgnoreDiscounts = false,
                IgnoreFeaturedProducts = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                CacheProductPrices = false,
                ProductsByTagAllowCustomersToSelectPageSize = true,
                ProductsByTagPageSizeOptions = "6, 3, 9, 18",
                ProductsByTagPriceRangeFiltering = true,
                ProductsByTagManuallyPriceRange = true,
                ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                MaximumBackInStockSubscriptions = 200,
                ManufacturersBlockItemsToDisplay = 2,
                DisplayTaxShippingInfoFooter = isGermany,
                DisplayTaxShippingInfoProductDetailsPage = isGermany,
                DisplayTaxShippingInfoProductBoxes = isGermany,
                DisplayTaxShippingInfoShoppingCart = isGermany,
                DisplayTaxShippingInfoWishlist = isGermany,
                DisplayTaxShippingInfoOrderDetailsPage = isGermany,
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                DefaultManufacturerPageSizeOptions = "6, 3, 9",
                DefaultManufacturerPageSize = 6,
                ShowProductReviewsTabOnAccountPage = true,
                ProductReviewsPageSizeOnAccountPage = 10,
                ProductReviewsSortByCreatedDateAscending = false,
                ExportImportProductAttributes = true,
                ExportImportUseDropdownlistsForAssociatedEntities = true,
                ExportImportProductsCountInOneFile = 500,
                ExportImportSplitProductsFile = false,
                ExportImportRelatedEntitiesByName = true,
                CountDisplayedYearsDatePicker = 1,
                UseAjaxLoadMenu = false,
                UseAjaxCatalogProductsLoading = true,
                EnableManufacturerFiltering = true,
                EnablePriceRangeFiltering = true,
                EnableSpecificationAttributeFiltering = true,
                DisplayFromPrices = false,
                AllowCustomersToSearchWithCategoryName = true,
                AllowCustomersToSearchWithManufacturerName = true,
                DisplayAllPicturesOnCatalogPages = false,
            });


            await settingService.SaveSettingAsync(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
            });

        }
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InstallCategoriesAsync()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();


            //categories
            var allCategories = new List<Category>();
            var categoryComputers = new Category
            {
                Name = "Computers",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_computers.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Computers"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryComputers);

            await InsertInstallationDataAsync(categoryComputers);

            var categoryDesktops = new Category
            {
                Name = "Desktops",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_desktops.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Desktops"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDesktops);

            await InsertInstallationDataAsync(categoryDesktops);

            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_notebooks.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Notebooks"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryNotebooks);

            await InsertInstallationDataAsync(categoryNotebooks);

            var categorySoftware = new Category
            {
                Name = "Software",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_software.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Software"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categorySoftware);

            await InsertInstallationDataAsync(categorySoftware);

            var categoryElectronics = new Category
            {
                Name = "Electronics",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_electronics.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Electronics"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryElectronics);

            await InsertInstallationDataAsync(categoryElectronics);

            var categoryCameraPhoto = new Category
            {
                Name = "Camera & photo",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_camera_photo.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Camera, photo"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCameraPhoto);

            await InsertInstallationDataAsync(categoryCameraPhoto);

            var categoryCellPhones = new Category
            {
                Name = "Cell phones",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_cell_phones.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Cell phones"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCellPhones);

            await InsertInstallationDataAsync(categoryCellPhones);

            var categoryOthers = new Category
            {
                Name = "Others",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Accessories"))).Id,
                IncludeInTopMenu = true,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryOthers);

            await InsertInstallationDataAsync(categoryOthers);

            var categoryApparel = new Category
            {
                Name = "Apparel",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_apparel.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Apparel"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryApparel);

            await InsertInstallationDataAsync(categoryApparel);

            var categoryShoes = new Category
            {
                Name = "Shoes",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_shoes.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Shoes"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryShoes);

            await InsertInstallationDataAsync(categoryShoes);

            var categoryClothing = new Category
            {
                Name = "Clothing",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_clothing.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Clothing"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryClothing);

            await InsertInstallationDataAsync(categoryClothing);

            var categoryAccessories = new Category
            {
                Name = "Accessories",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_apparel_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Apparel Accessories"))).Id,
                IncludeInTopMenu = true,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryAccessories);

            await InsertInstallationDataAsync(categoryAccessories);

            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_digital_downloads.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Digital downloads"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDigitalDownloads);

            await InsertInstallationDataAsync(categoryDigitalDownloads);

            var categoryBooks = new Category
            {
                Name = "Books",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_book.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Book"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryBooks);

            await InsertInstallationDataAsync(categoryBooks);

            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_jewelry.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Jewelry"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryJewelry);

            await InsertInstallationDataAsync(categoryJewelry);

            var categoryGiftCards = new Category
            {
                Name = "Gift Cards",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_gift_cards.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Gift Cards"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryGiftCards);

            await InsertInstallationDataAsync(categoryGiftCards);

            //search engine names
            foreach (var category in allCategories)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = nameof(Category),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(category, category.Name)
                });
        }


        #endregion

        #region Methods

        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="languagePackInfo">Language pack info</param>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword,
            (string languagePackDownloadLink, int languagePackProgress) languagePackInfo, RegionInfo regionInfo, CultureInfo cultureInfo)
        {
            await InstallSettingsAsync(regionInfo);
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallSampleDataAsync(string defaultUserEmail)
        {
            await InstallCategoriesAsync();

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = false,
                DisplayNewProductsMenuItem = false,
                DisplayProductSearchMenuItem = false,
                DisplayCustomerInfoMenuItem = false,
                DisplayBlogMenuItem = false,
                DisplayForumsMenuItem = false,
                DisplayContactUsMenuItem = false
            });
        }

        #endregion
    }
}

