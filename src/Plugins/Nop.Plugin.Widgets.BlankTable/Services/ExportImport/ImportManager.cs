using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Plugin.Widgets.BlankTable.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INopFileProvider _fileProvider;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public ImportManager(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IHttpClientFactory httpClientFactory,
            ILocalizationService localizationService,
            ILogger logger,
            INopFileProvider fileProvider,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService
            )
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _httpClientFactory = httpClientFactory;
            _fileProvider = fileProvider;
            _localizationService = localizationService;
            _logger = logger;
            _pictureService = pictureService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        private static ExportedAttributeType GetTypeOfExportedAttribute(IXLWorksheet worksheet, PropertyManager<ExportProductAttribute> productAttributeManager, PropertyManager<ExportSpecificationAttribute> specificationAttributeManager, int iRow)
        {
            productAttributeManager.ReadFromXlsx(worksheet, iRow, ExportProductAttribute.ProducAttributeCellOffset);

            if (productAttributeManager.IsCaption)
            {
                return ExportedAttributeType.ProductAttribute;
            }

            specificationAttributeManager.ReadFromXlsx(worksheet, iRow, ExportProductAttribute.ProducAttributeCellOffset);

            if (specificationAttributeManager.IsCaption)
            {
                return ExportedAttributeType.SpecificationAttribute;
            }

            return ExportedAttributeType.NotSpecified;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private static async Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
        {
            var attributeType = (cellValue ?? string.Empty).ToString();

            if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
            {
                worksheet.Row(endRow).OutlineLevel = 1;
            }
            else
            {
                if ((await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false))
                    .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                    worksheet.Row(endRow).OutlineLevel = 1;
                else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        private static void CopyDataToNewFile(ImportProductMetadata metadata, IXLWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
        {
            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            // ok, we can run the real code of the sample now
            using var workbook = new XLWorkbook(stream);
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            var outWorksheet = workbook.Worksheets.Add(typeof(Product).Name);
            metadata.Manager.WriteCaption(outWorksheet);
            var outRow = 2;
            for (var row = startRow; row <= endRow; row++)
            {
                outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;
                for (var cell = 1; cell <= endCell; cell++)
                {
                    outWorksheet.Row(outRow).Cell(cell).Value = worksheet.Row(row).Cell(cell).Value;
                }

                outRow += 1;
            }

            workbook.Save();
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            for (var i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the image or null if the image has not changed
        /// </returns>
        protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
        {
            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, name);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(newPictureBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists)
                return null;

            var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
            return newPicture;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(string seName, bool isParentCategoryExists)> UpdateCategoryByXlsxAsync(Category category, PropertyManager<Category> manager, Dictionary<string, ValueTask<Category>> allCategories, bool isNew)
        {
            var seName = string.Empty;
            var isParentCategoryExists = true;
            var isParentCategorySet = false;

            foreach (var property in manager.GetProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        category.Name = property.StringValue.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                        break;
                    case "Description":
                        category.Description = property.StringValue;
                        break;
                    case "CategoryTemplateId":
                        category.CategoryTemplateId = property.IntValue;
                        break;
                    case "MetaKeywords":
                        category.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        category.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        category.MetaTitle = property.StringValue;
                        break;
                    case "ParentCategoryId":
                        if (!isParentCategorySet)
                        {
                            var parentCategory = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                            isParentCategorySet = parentCategory != null;

                            isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                            category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                        }

                        break;
                    case "ParentCategoryName":
                        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                        {
                            var categoryName = manager.GetProperty("ParentCategoryName").StringValue;
                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                var parentCategory = allCategories.ContainsKey(categoryName)
                                    //try find category by full name with all parent category names
                                    ? await allCategories[categoryName]
                                    //try find category by name
                                    : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));

                                if (parentCategory != null)
                                {
                                    category.ParentCategoryId = parentCategory.Id;
                                    isParentCategorySet = true;
                                }
                                else
                                {
                                    isParentCategoryExists = false;
                                }
                            }
                        }

                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                        if (picture != null)
                            category.PictureId = picture.Id;
                        break;
                    case "PageSize":
                        category.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        category.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        category.PageSizeOptions = property.StringValue;
                        break;
                    case "ShowOnHomepage":
                        category.ShowOnHomepage = property.BooleanValue;
                        break;
                    case "PriceRangeFiltering":
                        category.PriceRangeFiltering = property.BooleanValue;
                        break;
                    case "PriceFrom":
                        category.PriceFrom = property.DecimalValue;
                        break;
                    case "PriceTo":
                        category.PriceTo = property.DecimalValue;
                        break;
                    case "AutomaticallyCalculatePriceRange":
                        category.ManuallyPriceRange = property.BooleanValue;
                        break;
                    case "IncludeInTopMenu":
                        category.IncludeInTopMenu = property.BooleanValue;
                        break;
                    case "Published":
                        category.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        category.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            category.UpdatedOnUtc = DateTime.UtcNow;
            return (seName, isParentCategoryExists);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(Category category, bool isNew, string curentCategoryBreadCrumb)> GetCategoryFromXlsxAsync(PropertyManager<Category> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Category>> allCategories)
        {
            manager.ReadFromXlsx(worksheet, iRow);

            //try get category from database by ID
            var category = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetProperty("Id")?.IntValue);

            if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
            {
                var categoryName = manager.GetProperty("Name").StringValue;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category = allCategories.ContainsKey(categoryName)
                        //try find category by full name with all parent category names
                        ? await allCategories[categoryName]
                        //try find category by name
                        : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));
                }
            }

            var isNew = category == null;

            category ??= new Category();

            var curentCategoryBreadCrumb = string.Empty;

            if (isNew)
            {
                category.CreatedOnUtc = DateTime.UtcNow;
                //default values
                category.PageSize = _catalogSettings.DefaultCategoryPageSize;
                category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                category.Published = true;
                category.IncludeInTopMenu = true;
                category.AllowCustomersToSelectPageSize = true;
            }
            else
                curentCategoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);

            return (category, isNew, curentCategoryBreadCrumb);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SaveCategoryAsync(bool isNew, Category category, Dictionary<string, ValueTask<Category>> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
        {
            if (isNew)
                await _categoryService.InsertCategoryAsync(category);
            else
                await _categoryService.UpdateCategoryAsync(category);

            var categoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
            if (!allCategories.ContainsKey(categoryBreadCrumb))
                allCategories.Add(categoryBreadCrumb, new ValueTask<Category>(category));
            if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
                categoryBreadCrumb != curentCategoryBreadCrumb)
                allCategories.Remove(curentCategoryBreadCrumb);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(category, await _urlRecordService.ValidateSeNameAsync(category, seName, category.Name, true), 0);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;

            if (!_catalogSettings.ExportImportAllowDownloadImages)
                return string.Empty;

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                var fileData = await client.GetByteArrayAsync(urlString);
                await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    fs.Write(fileData, 0, fileData.Length);

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Download image failed", ex);
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get property list by excel cells
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="worksheet">Excel worksheet</param>
        /// <returns>Property list</returns>
        public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(IXLWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportCategoriesFromXlsxAsync(Stream stream)
        {
            using var workboox = new XLWorkbook(stream);
            // get the first worksheet in the workbook
            var worksheet = workboox.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new NopException("No worksheet found");

            //the columns
            var properties = GetPropertiesByExcelCells<Category>(worksheet);

            var manager = new PropertyManager<Category>(properties, _catalogSettings);

            var iRow = 2;
            var setSeName = properties.Any(p => p.PropertyName == "SeName");

            //performance optimization, load all categories in one SQL request
            var allCategories = await (await _categoryService
                .GetAllCategoriesAsync(showHidden: true))
                .GroupByAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c))
                .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());

            var saveNextTime = new List<int>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetProperties
                    .Select(property => worksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                //get category by data in xlsx file if it possible, or create new category
                var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, worksheet, iRow, allCategories);

                //update category by data in xlsx file
                var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                if (isParentCategoryExists)
                {
                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);
                }
                else
                {
                    //if parent category doesn't exists in database then try save category into database next time
                    saveNextTime.Add(iRow);
                }

                iRow++;
            }

            var needSave = saveNextTime.Any();

            while (needSave)
            {
                var remove = new List<int>();

                //try to save unsaved categories
                foreach (var rowId in saveNextTime)
                {
                    //get category by data in xlsx file if it possible, or create new category
                    var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, worksheet, rowId, allCategories);
                    //update category by data in xlsx file
                    var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                    if (!isParentCategoryExists)
                        continue;

                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);
                    remove.Add(rowId);
                }

                saveNextTime.RemoveAll(item => remove.Contains(item));

                needSave = remove.Any() && saveNextTime.Any();
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

            if (!saveNextTime.Any())
                return;

            var categoriesName = new List<string>();

            foreach (var rowId in saveNextTime)
            {
                manager.ReadFromXlsx(worksheet, rowId);
                categoriesName.Add(manager.GetProperty("Name").StringValue);
            }

            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", categoriesName)));
        }

        #endregion

        #region Nested classes

        protected class ProductPictureMetadata
        {
            public Product ProductItem { get; set; }

            public string Picture1Path { get; set; }

            public string Picture2Path { get; set; }

            public string Picture3Path { get; set; }

            public bool IsNew { get; set; }
        }

        public class CategoryKey
        {
            /// <returns>A task that represents the asynchronous operation</returns>
            public static async Task<CategoryKey> CreateCategoryKeyAsync(Category category, ICategoryService categoryService, IList<Category> allCategories, IStoreMappingService storeMappingService)
            {
                return new CategoryKey(await categoryService.GetFormattedBreadCrumbAsync(category, allCategories), category.LimitedToStores ? (await storeMappingService.GetStoresIdsWithAccessAsync(category)).ToList() : new List<int>())
                {
                    Category = category
                };
            }

            public CategoryKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public Category Category { get; private set; }

            public string Key { get; }

            public bool Equals(CategoryKey y)
            {
                if (y == null)
                    return false;

                if (Category != null && y.Category != null)
                    return Category.Id == y.Category.Id;

                if ((StoresIds.Any() || y.StoresIds.Any())
                    && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                    return false;

                return Key.Equals(y.Key);
            }

            public override int GetHashCode()
            {
                if (!StoresIds.Any())
                    return Key.GetHashCode();

                var storesIds = StoresIds.Select(id => id.ToString())
                    .Aggregate(string.Empty, (all, current) => all + current);

                return $"{storesIds}_{Key}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as CategoryKey;
                return other?.Equals(other) ?? false;
            }
        }

        #endregion
    }
}