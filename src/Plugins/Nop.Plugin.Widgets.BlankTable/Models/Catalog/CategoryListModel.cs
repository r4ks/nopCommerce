using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models.Catalog
{
    /// <summary>
    /// Represents a category list model
    /// </summary>
    public partial record CategoryListModel : BasePagedListModel<CategoryModel>
    {
    }
}