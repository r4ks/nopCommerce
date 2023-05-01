using Nop.Core.Caching;

namespace Nop.Plugin.Widgets.HumanResource.Infrastructure.Cache
{
    public static partial class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public static CacheKey OfficialNewsModelKey => new("Nop.pres.admin.official.news");

        /// <summary>
        /// Key for employees caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey EmployeesListKey => new("Nop.pres.admin.employees.list-{0}", EmployeesListPrefixCacheKey);
        public static string EmployeesListPrefixCacheKey => "Nop.pres.admin.employees.list";

    }
}
