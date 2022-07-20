using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.BlankTable.Services.Catalog
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly CommonSettings _commonSettings;
        protected readonly IAclService _aclService;
        protected readonly ICustomerService _customerService;
        protected readonly IDateRangeService _dateRangeService;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IRepository<DiscountProductMapping> _discountProductMappingRepository;
        protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IRepository<ProductCategory> _productCategoryRepository;
        protected readonly IRepository<Shipment> _shipmentRepository;
        protected readonly IRepository<Warehouse> _warehouseRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IStoreService _storeService;
        protected readonly IWorkContext _workContext;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ProductService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IRepository<Category> categoryRepository,
            IRepository<DiscountProductMapping> discountProductMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<Warehouse> warehouseRepository,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _categoryRepository = categoryRepository;
            _discountProductMappingRepository = discountProductMappingRepository;
            _localizedPropertyRepository = localizedPropertyRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _shipmentRepository = shipmentRepository;
            _warehouseRepository = warehouseRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAsync(Product product)
        {
            await _productRepository.DeleteAsync(product);
        }

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductsAsync(IList<Product> products)
        {
            await _productRepository.DeleteAsync(products);
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetAllProductsDisplayedOnHomepageAsync()
        {
            var products = await _productRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted &&
                             p.ShowOnHomepage
                       select p;
            }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsHomepageCacheKey));

            return products;
        }

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        public virtual async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId, cache => default);
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetProductsByIdsAsync(int[] productIds)
        {
            return await _productRepository.GetByIdsAsync(productIds, cache => default, false);
        }

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAsync(Product product)
        {
            await _productRepository.InsertAsync(product);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        /// <summary>
        /// Gets featured products by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of featured products
        /// </returns>
        public virtual async Task<IList<Product>> GetCategoryFeaturedProductsAsync(int categoryId, int storeId = 0)
        {
            IList<Product> featuredProducts = new List<Product>();

            if (categoryId == 0)
                return featuredProducts;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryFeaturedProductsIdsKey, categoryId, customerRoleIds, storeId);

            var featuredProductIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from p in _productRepository.Table
                            join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                            where p.Published && !p.Deleted && p.VisibleIndividually &&
                                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                                pc.IsFeaturedProduct && categoryId == pc.CategoryId
                            select p;

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, customerRoleIds);

                featuredProducts = query.ToList();

                return featuredProducts.Select(p => p.Id).ToList();
            });

            if (featuredProducts.Count == 0 && featuredProductIds.Count > 0)
                featuredProducts = await _productRepository.GetByIdsAsync(featuredProductIds, cache => default, false);

            return featuredProducts;
        }

        /// <summary>
        /// Gets products which marked as new
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of new products
        /// </returns>
        public virtual async Task<IPagedList<Product>> GetProductsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _productRepository.Table
                        where p.Published && p.VisibleIndividually && p.MarkAsNew && !p.Deleted &&
                            DateTime.UtcNow >= (p.MarkAsNewStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.MarkAsNewEndDateTimeUtc ?? DateTime.MaxValue)
                        select p;

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
            query = await _aclService.ApplyAcl(query, customer);

            query = query.OrderByDescending(p => p.CreatedOnUtc);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Get number of product (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public virtual async Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _productRepository.Table.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            query = await _aclService.ApplyAcl(query, customerRoleIds);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        where categoryIds.Contains(pc.CategoryId)
                        select p;
            }

            var cacheKey = _staticCacheManager
                .PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryProductsNumberCacheKey, customerRoleIds, storeId, categoryIds);

            //only distinct products
            return await _staticCacheManager.GetAsync(cacheKey, () => query.Select(p => p.Id).Count());
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerIds">Manufacturer identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="excludeFeaturedProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured products from results</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecOptions">Specification options list to filter products; null to load all records</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IPagedList<Product>> SearchProductsAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> manufacturerIds = null,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            bool visibleIndividuallyOnly = false,
            bool excludeFeaturedProducts = false,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchManufacturerPartNumber = true,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var productsQuery = _productRepository.Table;

            if (!showHidden)
                productsQuery = productsQuery.Where(p => p.Published);
            else if (overridePublished.HasValue)
                productsQuery = productsQuery.Where(p => p.Published == overridePublished.Value);

            //apply store mapping constraints
            productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);

            //apply ACL constraints
            if (!showHidden)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                productsQuery = await _aclService.ApplyAcl(productsQuery, customer);
            }

            productsQuery =
                from p in productsQuery
                where !p.Deleted &&
                    (!visibleIndividuallyOnly || p.VisibleIndividually) &&
                    (vendorId == 0 || p.VendorId == vendorId) &&
                    (showHidden ||
                            DateTime.UtcNow >= (p.AvailableStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.AvailableEndDateTimeUtc ?? DateTime.MaxValue)
                    ) &&
                    (priceMin == null || p.Price >= priceMin) &&
                    (priceMax == null || p.Price <= priceMax)
                select p;

            if (!string.IsNullOrEmpty(keywords))
            {
                var langs = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //Set a flag which will to points need to search in localized properties. If showHidden doesn't set to true should be at least two published languages.
                var searchLocalizedValue = languageId > 0 && langs.Count >= 2 && (showHidden || langs.Count(l => l.Published) >= 2);

                IQueryable<int> productsByKeywords;

                productsByKeywords =
                        from p in _productRepository.Table
                        where p.Name.Contains(keywords) ||
                            (searchDescriptions &&
                                (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                            (searchManufacturerPartNumber && p.ManufacturerPartNumber == keywords) ||
                            (searchSku && p.Sku == keywords)
                        select p.Id;

                if (searchLocalizedValue)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from lp in _localizedPropertyRepository.Table
                        let checkName = lp.LocaleKey == nameof(Product.Name) &&
                                        lp.LocaleValue.Contains(keywords)
                        let checkShortDesc = searchDescriptions &&
                                        lp.LocaleKey == nameof(Product.ShortDescription) &&
                                        lp.LocaleValue.Contains(keywords)
                        where
                            lp.LocaleKeyGroup == nameof(Product) && lp.LanguageId == languageId && (checkName || checkShortDesc)

                        select lp.EntityId);
                }

                //search by category name if admin allows
                if (_catalogSettings.AllowCustomersToSearchWithCategoryName)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pc in _productCategoryRepository.Table
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where c.Name.Contains(keywords)
                        select pc.ProductId
                    );

                    if (searchLocalizedValue)
                    {
                        productsByKeywords = productsByKeywords.Union(
                        from pc in _productCategoryRepository.Table
                        join lp in _localizedPropertyRepository.Table on pc.CategoryId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Category) &&
                              lp.LocaleKey == nameof(Category.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pc.ProductId);
                    }
                }

                productsQuery =
                    from p in productsQuery
                    join pbk in productsByKeywords on p.Id equals pbk
                    select p;
            }

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var productCategoryQuery =
                        from pc in _productCategoryRepository.Table
                        where (!excludeFeaturedProducts || !pc.IsFeaturedProduct) &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.ProductId into pc
                        select new { 
                            ProductId = pc.Key,
                            DisplayOrder = pc.First().DisplayOrder
                        };

                    productsQuery =
                        from p in productsQuery
                        join pc in productCategoryQuery on p.Id equals pc.ProductId
                        orderby pc.DisplayOrder, p.Name
                        select p;
                }
            }

            return await productsQuery.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets number of products by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public async Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            return await _productRepository.Table.CountAsync(p => p.VendorId == vendorId && !p.Deleted);
        }

        /// <summary>
        /// Parse "required product Ids" property
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A list of required product IDs</returns>
        public virtual int[] ParseRequiredProductIds(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrEmpty(product.RequiredProductIds))
                return Array.Empty<int>();

            var ids = new List<int>();

            foreach (var idStr in product.RequiredProductIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
                if (int.TryParse(idStr, out var id))
                    ids.Add(id);

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool ProductIsAvailable(Product product, DateTime? dateTime = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            dateTime ??= DateTime.UtcNow;

            if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
        }

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        public virtual int[] ParseAllowedQuantities(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var result = new List<int>();
            if (!string.IsNullOrWhiteSpace(product.AllowedQuantities))
                product.AllowedQuantities
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(qtyStr =>
                    {
                        if (int.TryParse(qtyStr.Trim(), out var qty))
                            result.Add(qty);
                    });

            return result.ToArray();
        }

        /// <summary>
        /// Update product store mappings
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductStoreMappingsAsync(Product product, IList<int> limitedToStoresIds)
        {
            product.LimitedToStores = limitedToStoresIds.Any();

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(product);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(product, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        /// <summary>
        /// Gets the value whether the sequence contains downloadable products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyDownloadableProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsDownload);
        }

        /// <summary>
        /// Gets the value whether the sequence contains gift card products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyGiftCardProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsGiftCard);
        }

        /// <summary>
        /// Gets the value whether the sequence contains recurring products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyRecurringProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsRecurring);
        }

        /// <summary>
        /// Returns a list of sku of not existing products
        /// </summary>
        /// <param name="productSku">The sku of the products to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of sku not existing products
        /// </returns>
        public virtual async Task<string[]> GetNotExistingProductsAsync(string[] productSku)
        {
            if (productSku == null)
                throw new ArgumentNullException(nameof(productSku));

            var query = _productRepository.Table;
            var queryFilter = productSku.Distinct().ToArray();
            //filtering by SKU
            var filter = await query.Select(p => p.Sku)
                .Where(p => queryFilter.Contains(p))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #endregion
    }
}
