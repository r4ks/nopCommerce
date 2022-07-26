using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource
{
    /// <summary>
    /// Represents a employee list model
    /// </summary>
    public partial record EmployeeListModel : BasePagedListModel<EmployeeModel>
    {
    }
}