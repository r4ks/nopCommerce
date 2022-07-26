using System.Threading.Tasks;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the employee model factory
    /// </summary>
    public partial interface IEmployeeModelFactory
    {
        /// <summary>
        /// Prepare employee search model
        /// </summary>
        /// <param name="searchModel">Employee search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee search model
        /// </returns>
        Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsync(EmployeeSearchModel searchModel);

        /// <summary>
        /// Prepare paged employee list model
        /// </summary>
        /// <param name="searchModel">Employee search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee list model
        /// </returns>
        Task<EmployeeListModel> PrepareEmployeeListModelAsync(EmployeeSearchModel searchModel);

        /// <summary>
        /// Prepare employee model
        /// </summary>
        /// <param name="model">Employee model</param>
        /// <param name="employee">Employee</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee model
        /// </returns>
        Task<EmployeeModel> PrepareEmployeeModelAsync(EmployeeModel model, Employee employee, bool excludeProperties = false);

    }
}