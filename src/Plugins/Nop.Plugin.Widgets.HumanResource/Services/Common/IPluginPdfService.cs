using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.Common
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface IPluginPdfService
    {
        /// <summary>
        /// Print products to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintEmployeesToPdfAsync(Stream stream, IList<Employee> products);
    }
}