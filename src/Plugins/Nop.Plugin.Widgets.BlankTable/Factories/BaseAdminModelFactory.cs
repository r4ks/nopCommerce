using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Cache;

namespace Nop.Plugin.Widgets.BlankTable.Factories
{
    /// <summary>
    /// Represents the implementation of the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial class BaseAdminModelFactory : IBaseAdminModelFactory
    {
        #region Fields

        private readonly ICategoryService _categoryService;

        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public BaseAdminModelFactory(ICategoryService categoryService,
            ILocalizationService localizationService,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService
            )
        {
            _categoryService = categoryService;
            _localizationService = localizationService;
            _staticCacheManager = staticCacheManager;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        /// <param name="defaultItemValue">Default item value; defaults 0</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list
        /// </returns>
        protected virtual async Task<List<SelectListItem>> GetCategoryListAsync(bool showHidden = true)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoriesListKey, showHidden);
            var listItems = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var categories = await _categoryService.GetAllCategoriesAsync(showHidden: showHidden);
                return await categories.SelectAwait(async c => new SelectListItem
                {
                    Text = await _categoryService.GetFormattedBreadCrumbAsync(c, categories),
                    Value = c.Id.ToString()
                }).ToListAsync();
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var availableStores = await _storeService.GetAllStoresAsync();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }


        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available categories
            var availableCategoryItems = await GetCategoryListAsync();
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        #endregion
    }
}