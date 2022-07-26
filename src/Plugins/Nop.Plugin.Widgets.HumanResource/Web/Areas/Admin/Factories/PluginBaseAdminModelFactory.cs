using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.HumanResource.Services.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Web.Infrastructure.Cache;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the implementation of the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial class PluginBaseAdminModelFactory : IPluginBaseAdminModelFactory
    {
        #region Fields

        private readonly IEmployeeService _employeeService;

        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public PluginBaseAdminModelFactory(IEmployeeService employeeService,
            ILocalizationService localizationService,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService
            )
        {
            _employeeService = employeeService;
            _localizationService = localizationService;
            _staticCacheManager = staticCacheManager;
            _storeService = storeService;
        }

        public PluginBaseAdminModelFactory() { }
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
            defaultItemText ??= await _localizationService.GetResourceAsync(HumanResourceDefaults.Labels.All);

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }

        /// <summary>
        /// Get employee list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee list
        /// </returns>
        protected virtual async Task<List<SelectListItem>> GetEmployeeListAsync(bool showHidden = true)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.EmployeesListKey, showHidden);
            var listItems = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var employees = await _employeeService.GetAllEmployeesAsync(showHidden: showHidden);
                return await employees.SelectAwait(async c => new SelectListItem
                {
                    Text = await _employeeService.GetFormattedBreadCrumbAsync(c, employees),
                    Value = c.Id.ToString()
                }).ToListAsync();
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });

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
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }


        /// <summary>
        /// Prepare available employees
        /// </summary>
        /// <param name="items">Employee items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareEmployeesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available employees
            var availableEmployeeItems = await GetEmployeeListAsync();
            foreach (var employeeItem in availableEmployeeItems)
                items.Add(employeeItem);

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }

        #endregion
    }
}