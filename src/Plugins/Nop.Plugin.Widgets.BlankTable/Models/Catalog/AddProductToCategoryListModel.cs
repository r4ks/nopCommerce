using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a product list model to add to the category
    /// </summary>
    public partial record AddProductToCategoryListModel : BasePagedListModel<ProductModel>
    {
    }
}