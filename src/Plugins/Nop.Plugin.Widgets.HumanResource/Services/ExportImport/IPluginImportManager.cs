using System.IO;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HumanResource.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IPluginImportManager
    {
        /// <summary>
        /// Import employees from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportEmployeesFromXlsxAsync(Stream stream);

    }
}
