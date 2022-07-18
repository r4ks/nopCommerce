using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a category product list model
    /// </summary>
    public partial record CategoryProductListModel : BasePagedListModel<CategoryProductModel>
    {
    }
}