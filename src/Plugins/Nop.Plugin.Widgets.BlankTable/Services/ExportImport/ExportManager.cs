using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Forums;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;
using Nop.Plugin.Widgets.BlankTable.Services.Catalog;
using Nop.Plugin.Widgets.BlankTable.Services.ExportImport.Help;
using Nop.Services.Common;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;

namespace Nop.Plugin.Widgets.BlankTable.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICategoryService _categoryService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExportManager(
            CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext
            )
        {
            _catalogSettings = catalogSettings;
            _customerActivityService = customerActivityService;
            _dateTimeSettings = dateTimeSettings;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<int> WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId, int totalCategories)
        {
            var categories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parentCategoryId, true);
            if (categories == null || !categories.Any())
                return totalCategories;

            totalCategories += categories.Count;

            foreach (var category in categories)
            {
                await xmlWriter.WriteStartElementAsync("Category");

                await xmlWriter.WriteStringAsync("Id", category.Id);

                await xmlWriter.WriteStringAsync("Name", category.Name);
                await xmlWriter.WriteStringAsync("Description", category.Description);
                await xmlWriter.WriteStringAsync("SeName", await _urlRecordService.GetSeNameAsync(category, 0), await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ParentCategoryId", category.ParentCategoryId);
                await xmlWriter.WriteStringAsync("PictureId", category.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", category.PageSize, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", category.AllowCustomersToSelectPageSize, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PageSizeOptions", category.PageSizeOptions, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceRangeFiltering", category.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceFrom", category.PriceFrom, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceTo", category.PriceTo, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ManuallyPriceRange", category.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ShowOnHomepage", category.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("IncludeInTopMenu", category.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("Published", category.Published, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("Deleted", category.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", category.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", category.CreatedOnUtc, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", category.UpdatedOnUtc, await IgnoreExportCategoryPropertyAsync());

                await xmlWriter.WriteStartElementAsync("Products");

                await xmlWriter.WriteEndElementAsync();

                await xmlWriter.WriteStartElementAsync("SubCategories");
                totalCategories = await WriteCategoriesAsync(xmlWriter, category.Id, totalCategories);
                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndElementAsync();
            }

            return totalCategories;
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the path to the image file
        /// </returns>
        protected virtual async Task<string> GetPicturesAsync(int pictureId)
        {
            var picture = await _pictureService.GetPictureByIdAsync(pictureId);

            return await _pictureService.GetThumbLocalPathAsync(picture);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<bool> IgnoreExportCategoryPropertyAsync()
        {
            try
            {
                return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "category-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportCategoriesToXmlAsync()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Categories");
            await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);
            var totalCategories = await WriteCategoriesAsync(xmlWriter, 0, 0);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), totalCategories));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories)
        {
            var parentCategories = new List<Category>();
            if (_catalogSettings.ExportImportCategoriesUsingCategoryName)
                //performance optimization, load all parent categories in one SQL request
                parentCategories.AddRange(await _categoryService.GetCategoriesByIdsAsync(categories.Select(c => c.ParentCategoryId).Where(id => id != 0).ToArray()));

            //property manager 
            var manager = new PropertyManager<Category>(new[]
            {
                new PropertyByName<Category>("Id", p => p.Id),
                new PropertyByName<Category>("Name", p => p.Name),
                new PropertyByName<Category>("Description", p => p.Description),
                new PropertyByName<Category>("SeName", async p => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                new PropertyByName<Category>("ParentCategoryName", async p =>
                {
                    var category = parentCategories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                    return category != null ? await _categoryService.GetFormattedBreadCrumbAsync(category) : null;

                }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
                new PropertyByName<Category>("Picture", async p => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Category>("PageSize", p => p.PageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceRangeFiltering", p => p.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceFrom", p => p.PriceFrom, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceTo", p => p.PriceTo, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ManuallyPriceRange", p => p.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("Published", p => p.Published, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), categories.Count));

            return await manager.ExportToXlsxAsync(categories);
        }

         #endregion
    }
}
