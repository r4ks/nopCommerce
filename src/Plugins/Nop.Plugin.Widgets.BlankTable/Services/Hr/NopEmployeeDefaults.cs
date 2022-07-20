using Nop.Core.Caching;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;

namespace Nop.Plugin.Widgets.BlankTable.Services.Hr
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class NopEmployeeDefaults
    {
        #region Common

        /// <summary>
        /// Gets a default price range 'from'
        /// </summary>
        public static decimal DefaultPriceRangeFrom => 0;

        /// <summary>
        /// Gets a default price range 'to'
        /// </summary>
        public static decimal DefaultPriceRangeTo => 10000;

        #endregion

        #region Products

        /// <summary>
        /// Gets a template of product name on copying
        /// </summary>
        /// <remarks>
        /// {0} : product name
        /// </remarks>
        public static string ProductCopyNameTemplate => "Copy of {0}";

        /// <summary>
        /// Gets default prefix for product
        /// </summary>
        public static string ProductAttributePrefix => "product_attribute_";

        #endregion

        #region Caching defaults

        #region Employees

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent employee ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey EmployeesByParentEmployeeCacheKey => new("Nop.employee.byparent.{0}-{1}-{2}-{3}", EmployeesByParentEmployeePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent employee ID
        /// </remarks>
        public static string EmployeesByParentEmployeePrefix => "Nop.employee.byparent.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent employee id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static CacheKey EmployeesChildIdsCacheKey => new("Nop.employee.childids.{0}-{1}-{2}-{3}", EmployeesChildIdsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent employee ID
        /// </remarks>
        public static string EmployeesChildIdsPrefix => "Nop.employee.childids.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey EmployeesHomepageCacheKey => new("Nop.employee.homepage.", EmployeesHomepagePrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey EmployeesHomepageWithoutHiddenCacheKey => new("Nop.employee.homepage.withouthidden-{0}-{1}", EmployeesHomepagePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string EmployeesHomepagePrefix => "Nop.employee.homepage.";

        /// <summary>
        /// Key for caching of employee breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : employee id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey EmployeeBreadcrumbCacheKey => new("Nop.employee.breadcrumb.{0}-{1}-{2}-{3}", EmployeeBreadcrumbPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string EmployeeBreadcrumbPrefix => "Nop.employee.breadcrumb.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey EmployeesAllCacheKey => new("Nop.employee.all.{0}-{1}-{2}", NopEntityCacheDefaults<Employee>.AllPrefix);

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersByProductCacheKey => new("Nop.productmanufacturer.byproduct.{0}-{1}-{2}-{3}", ProductManufacturersByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductManufacturersByProductPrefix => "Nop.productmanufacturer.byproduct.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : employee ID
        /// </remarks>
        public static CacheKey ManufacturersByEmployeeCacheKey => new("Nop.manufacturer.bycategory.{0}", ManufacturersByEmployeePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturersByEmployeePrefix => "Nop.manufacturer.bycategory.";

        #endregion

        #region Products

        /// <summary>
        /// Key for "related" product displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey RelatedProductsCacheKey => new("Nop.relatedproduct.byproduct.{0}-{1}", RelatedProductsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string RelatedProductsPrefix => "Nop.relatedproduct.byproduct.{0}";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// </remarks>
        public static CacheKey TierPricesByProductCacheKey => new("Nop.tierprice.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductsHomepageCacheKey => new("Nop.product.homepage.");

        /// <summary>
        /// Key for caching identifiers of employee featured products
        /// </summary>
        /// <remarks>
        /// {0} : employee id
        /// {1} : customer role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey EmployeeFeaturedProductsIdsKey => new("Nop.product.featured.bycategory.{0}-{1}-{2}", EmployeeFeaturedProductsIdsPrefix, FeaturedProductIdsPrefix);
        public static string EmployeeFeaturedProductsIdsPrefix => "Nop.product.featured.bycategory.{0}";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : customer role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerFeaturedProductIdsKey => new("Nop.product.featured.bymanufacturer.{0}-{1}-{2}", ManufacturerFeaturedProductIdsPrefix, FeaturedProductIdsPrefix);
        public static string ManufacturerFeaturedProductIdsPrefix => "Nop.product.featured.bymanufacturer.{0}";

        public static string FeaturedProductIdsPrefix => "Nop.product.featured.";

        /// <summary>
        /// Gets a key for product prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : overridden product price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey ProductPriceCacheKey => new("Nop.totals.productprice.{0}-{1}-{2}-{3}-{4}-{5}-{6}", ProductPricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductPricePrefix => "Nop.totals.productprice.{0}";

        /// <summary>
        /// Gets a key for product multiple prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey ProductMultiplePriceCacheKey => new("Nop.totals.productprice.multiple.{0}-{1}-{2}", ProductMultiplePricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductMultiplePricePrefix => "Nop.totals.productprice.multiple.{0}";

        #endregion

        #region Product attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeMappingsByProductCacheKey => new("Nop.productattributemapping.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static CacheKey ProductAttributeValuesByAttributeCacheKey => new("Nop.productattributevalue.byattribute.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeCombinationsByProductCacheKey => new("Nop.productattributecombination.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Product attribute ID
        /// </remarks>
        public static CacheKey PredefinedProductAttributeValuesByAttributeCacheKey => new("Nop.predefinedproductattributevalue.byattribute.{0}");

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching product review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : product review ID
        /// </remarks>
        public static CacheKey ProductReviewTypeMappingByReviewTypeCacheKey => new("Nop.productreviewreviewtypemapping.byreviewtype.{0}");

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on product page
        /// {4} : specification attribute group ID
        /// </remarks>
        public static CacheKey ProductSpecificationAttributeByProductCacheKey => new("Nop.productspecificationattribute.byproduct.{0}-{1}-{2}-{3}-{4}", ProductSpecificationAttributeByProductPrefix, ProductSpecificationAttributeAllByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductSpecificationAttributeByProductPrefix => "Nop.productspecificationattribute.byproduct.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {1} (not 0, see the <ref>ProductSpecificationAttributeAllByProductIdCacheKey</ref>) :specification attribute option ID
        /// </remarks>
        public static string ProductSpecificationAttributeAllByProductPrefix => "Nop.productspecificationattribute.byproduct.";

        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static CacheKey SpecificationAttributesWithOptionsCacheKey => new("Nop.specificationattribute.withoptions.");

        /// <summary>
        /// Key for specification attribute options by employee ID caching
        /// </summary>
        /// <remarks>
        /// {0} : employee ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsByEmployeeCacheKey => new("Nop.specificationattributeoption.bycategory.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string FilterableSpecificationAttributeOptionsPrefix => "Nop.filterablespecificationattributeoptions";

        #endregion

        #endregion
    }
}