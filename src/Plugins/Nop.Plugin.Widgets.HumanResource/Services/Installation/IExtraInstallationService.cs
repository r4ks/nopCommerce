using System.Globalization;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HumanResource.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial interface IExtraInstallationService
    {
        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InstallRequiredDataAsync(RegionInfo regionInfo, CultureInfo cultureInfo);

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InstallSampleDataAsync();
    }
}