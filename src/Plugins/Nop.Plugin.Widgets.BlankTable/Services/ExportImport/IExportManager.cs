using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;

namespace Nop.Plugin.Widgets.BlankTable.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {

        /// <summary>
        /// Export employee list to XML
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportEmployeesToXmlAsync();

        /// <summary>
        /// Export employees to XLSX
        /// </summary>
        /// <param name="employees">Employees</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportEmployeesToXlsxAsync(IList<Employee> employees);

    }
}
