using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models.Category
{
    /// <summary>
    /// Represents a category product list model
    /// </summary>
    public partial record CategoryProductListModel : BasePagedListModel<CategoryProductModel>
    {
    }
}