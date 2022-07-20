using System.IO;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BlankTable.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        /// <summary>
        /// Import employees from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportEmployeesFromXlsxAsync(Stream stream);

    }
}
