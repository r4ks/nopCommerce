using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial interface IPluginBaseAdminModelFactory
    {
        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available employees
        /// </summary>
        /// <param name="items">Employee items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareEmployeesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    }
}