using System.Threading.Tasks;
using Nop.Core.Domain.Gdpr;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory
    /// </summary>
    public partial interface IEmployeeSettingModelFactory
    {
        Task<EmployeeSettingsModel> PrepareEmployeeSettingsModelAsync(EmployeeSettingsModel model = null);

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sort option list model
        /// </returns>
    }
}