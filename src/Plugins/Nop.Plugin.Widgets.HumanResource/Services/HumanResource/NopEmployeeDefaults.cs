using Nop.Core.Caching;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.HumanResource
{
    /// <summary>
    /// Represents default values related to hr services
    /// </summary>
    public static partial class NopEmployeeDefaults
    {

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
        public static CacheKey EmployeeFeaturedProductsIdsKey => new("Nop.product.featured.byemployee.{0}-{1}-{2}", EmployeeFeaturedProductsIdsPrefix, FeaturedProductIdsPrefix);
        public static string EmployeeFeaturedProductsIdsPrefix => "Nop.product.featured.byemployee.{0}";

        public static string FeaturedProductIdsPrefix => "Nop.product.featured.";
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

        #region Specification attributes

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
        public static CacheKey SpecificationAttributeOptionsByEmployeeCacheKey => new("Nop.specificationattributeoption.byemployee.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string FilterableSpecificationAttributeOptionsPrefix => "Nop.filterablespecificationattributeoptions";

        #endregion

        #endregion
    }
}