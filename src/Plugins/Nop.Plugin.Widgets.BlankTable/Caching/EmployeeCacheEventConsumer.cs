using System.Threading.Tasks;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Plugin.Widgets.BlankTable.Services.Hr;
using Nop.Services.Caching;

namespace Nop.Plugin.Widgets.BlankTable.Caching
{
    /// <summary>
    /// Represents a employee cache event consumer
    /// </summary>
    public partial class EmployeeCacheEventConsumer : CacheEventConsumer<Employee>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Employee entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesByParentEmployeePrefix, entity);
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesByParentEmployeePrefix, entity.ParentEmployeeId);
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesChildIdsPrefix, entity);
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesChildIdsPrefix, entity.ParentEmployeeId);
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeesHomepagePrefix);
            await RemoveByPrefixAsync(NopEmployeeDefaults.EmployeeBreadcrumbPrefix);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
