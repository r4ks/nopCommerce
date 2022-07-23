using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Widgets.BlankTable.Services.Hr;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the employee model factory implementation
    /// </summary>
    public partial class EmployeeModelFactory : IEmployeeModelFactory
    {
        #region Fields

        private readonly EmployeeSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IPluginBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IEmployeeService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public EmployeeModelFactory(EmployeeSettings employeeSettings,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IPluginBaseAdminModelFactory baseAdminModelFactory,
            IEmployeeService categoryService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = employeeSettings;
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlRecordService = urlRecordService;
        }

        public EmployeeModelFactory() { }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare employee search model
        /// </summary>
        /// <param name="searchModel">Employee search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee search model
        /// </returns>
        public virtual async Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsync(EmployeeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync(EmployeeSearchModel.Labels.All)
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync(EmployeeSearchModel.Labels.PublishedOnly)
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync(EmployeeSearchModel.Labels.UnpublishedOnly)
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged employee list model
        /// </summary>
        /// <param name="searchModel">Employee search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee list model
        /// </returns>
        public virtual async Task<EmployeeListModel> PrepareEmployeeListModelAsync(EmployeeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get employees
            var employees = await _categoryService.GetAllEmployeesAsync(categoryName: searchModel.SearchEmployeeName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1);

            //prepare grid model
            var model = await new EmployeeListModel().PrepareToGridAsync(searchModel, employees, () =>
            {
                return employees.SelectAwait(async employee =>
                {
                    //fill in model values from the entity
                    var categoryModel = employee.ToModel<EmployeeModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(employee);
                    categoryModel.SeName = await _urlRecordService.GetSeNameAsync(employee, 0, true, false);

                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare employee model
        /// </summary>
        /// <param name="model">Employee model</param>
        /// <param name="employee">Employee</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee model
        /// </returns>
        public virtual async Task<EmployeeModel> PrepareEmployeeModelAsync(EmployeeModel model, Employee employee, bool excludeProperties = false)
        {
            Func<EmployeeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (employee != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = employee.ToModel<EmployeeModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(employee, 0, true, false);
                }

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(employee, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(employee, entity => entity.Description, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(employee, languageId, false, false);
                };
            }

            //set default values for the new model
            if (employee == null)
            {
                model.PageSize = _catalogSettings.DefaultEmployeePageSize;
                model.PageSizeOptions = _catalogSettings.DefaultEmployeePageSizeOptions;
                model.Published = true;
                model.IncludeInTopMenu = true;
                model.AllowCustomersToSelectPageSize = true;
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available parent employees
            await _baseAdminModelFactory.PrepareEmployeesAsync(model.AvailableEmployees,
                defaultItemText: await _localizationService.GetResourceAsync(EmployeeModel.Labels.None));

            //prepare model customer roles
            await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(model, employee, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, employee, excludeProperties);

            return model;
        }
        #endregion
    }
}