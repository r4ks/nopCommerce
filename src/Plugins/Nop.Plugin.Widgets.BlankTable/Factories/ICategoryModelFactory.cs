using System.Threading.Tasks;
using Nop.Plugin.Widgets.BlankTable.Models.Catalog;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;

namespace Nop.Plugin.Widgets.BlankTable.Factories
{
    /// <summary>
    /// Represents the category model factory
    /// </summary>
    public partial interface ICategoryModelFactory
    {
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category search model
        /// </returns>
        Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list model
        /// </returns>
        Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel);

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category model
        /// </returns>
        Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false);

    }
}