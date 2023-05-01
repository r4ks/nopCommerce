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
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.HumanResource.Services.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class PluginImportManager : IPluginImportManager
    {
        #region Fields

        private readonly EmployeeSettings _hrSettings;
        private readonly IEmployeeService _employeeService;
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

        public PluginImportManager(EmployeeSettings employeeSettings,
            IEmployeeService employeeService,
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
            _hrSettings = employeeSettings;
            _employeeService = employeeService;
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
                    var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType);
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
        protected virtual async Task<(string seName, bool isParentEmployeeExists)> UpdateEmployeeByXlsxAsync(Employee employee, PropertyManager<Employee> manager, Dictionary<string, ValueTask<Employee>> allEmployees, bool isNew)
        {
            var seName = string.Empty;
            var isParentEmployeeExists = true;
            var isParentEmployeeSet = false;

            foreach (var property in manager.GetProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        employee.Name = property.StringValue.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                        break;
                    case "Description":
                        employee.Description = property.StringValue;
                        break;
                    case "ParentEmployeeId":
                        if (!isParentEmployeeSet)
                        {
                            var parentEmployee = await await allEmployees.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                            isParentEmployeeSet = parentEmployee != null;

                            isParentEmployeeExists = isParentEmployeeSet || property.IntValue == 0;

                            employee.ParentEmployeeId = parentEmployee?.Id ?? property.IntValue;
                        }

                        break;
                    case "ParentEmployeeName":
                        if (_hrSettings.ExportImportEmployeesUsingEmployeeName && !isParentEmployeeSet)
                        {
                            var employeeName = manager.GetProperty("ParentEmployeeName").StringValue;
                            if (!string.IsNullOrEmpty(employeeName))
                            {
                                var parentEmployee = allEmployees.ContainsKey(employeeName)
                                    //try find employee by full name with all parent employee names
                                    ? await allEmployees[employeeName]
                                    //try find employee by name
                                    : await await allEmployees.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(employeeName, StringComparison.InvariantCulture));

                                if (parentEmployee != null)
                                {
                                    employee.ParentEmployeeId = parentEmployee.Id;
                                    isParentEmployeeSet = true;
                                }
                                else
                                {
                                    isParentEmployeeExists = false;
                                }
                            }
                        }

                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetProperty("Picture").StringValue, employee.Name, isNew ? null : (int?)employee.PictureId);
                        if (picture != null)
                            employee.PictureId = picture.Id;
                        break;
                    case "PageSize":
                        employee.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        employee.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        employee.PageSizeOptions = property.StringValue;
                        break;
                    case "ShowOnHomepage":
                        employee.ShowOnHomepage = property.BooleanValue;
                        break;
                    case "IncludeInTopMenu":
                        employee.IncludeInTopMenu = property.BooleanValue;
                        break;
                    case "Published":
                        employee.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        employee.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            employee.UpdatedOnUtc = DateTime.UtcNow;
            return (seName, isParentEmployeeExists);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(Employee employee, bool isNew, string curentEmployeeBreadCrumb)> GetEmployeeFromXlsxAsync(PropertyManager<Employee> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Employee>> allEmployees)
        {
            manager.ReadFromXlsx(worksheet, iRow);

            //try get employee from database by ID
            var employee = await await allEmployees.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetProperty("Id")?.IntValue);

            if (_hrSettings.ExportImportEmployeesUsingEmployeeName && employee == null)
            {
                var employeeName = manager.GetProperty("Name").StringValue;
                if (!string.IsNullOrEmpty(employeeName))
                {
                    employee = allEmployees.ContainsKey(employeeName)
                        //try find employee by full name with all parent employee names
                        ? await allEmployees[employeeName]
                        //try find employee by name
                        : await await allEmployees.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(employeeName, StringComparison.InvariantCulture));
                }
            }

            var isNew = employee == null;

            employee ??= new Employee();

            var curentEmployeeBreadCrumb = string.Empty;

            if (isNew)
            {
                employee.CreatedOnUtc = DateTime.UtcNow;
                //default values
                employee.PageSize = _hrSettings.DefaultEmployeePageSize;
                employee.PageSizeOptions = _hrSettings.DefaultEmployeePageSizeOptions;
                employee.Published = true;
                employee.IncludeInTopMenu = true;
                employee.AllowCustomersToSelectPageSize = true;
            }
            else
                curentEmployeeBreadCrumb = await _employeeService.GetFormattedBreadCrumbAsync(employee);

            return (employee, isNew, curentEmployeeBreadCrumb);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SaveEmployeeAsync(bool isNew, Employee employee, Dictionary<string, ValueTask<Employee>> allEmployees, string curentEmployeeBreadCrumb, bool setSeName, string seName)
        {
            if (isNew)
                await _employeeService.InsertEmployeeAsync(employee);
            else
                await _employeeService.UpdateEmployeeAsync(employee);

            var employeeBreadCrumb = await _employeeService.GetFormattedBreadCrumbAsync(employee);
            if (!allEmployees.ContainsKey(employeeBreadCrumb))
                allEmployees.Add(employeeBreadCrumb, new ValueTask<Employee>(employee));
            if (!string.IsNullOrEmpty(curentEmployeeBreadCrumb) && allEmployees.ContainsKey(curentEmployeeBreadCrumb) &&
                employeeBreadCrumb != curentEmployeeBreadCrumb)
                allEmployees.Remove(curentEmployeeBreadCrumb);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(employee, await _urlRecordService.ValidateSeNameAsync(employee, seName, employee.Name, true), 0);
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
        /// Import employees from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportEmployeesFromXlsxAsync(Stream stream)
        {
            using var workboox = new XLWorkbook(stream);
            // get the first worksheet in the workbook
            var worksheet = workboox.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new NopException("No worksheet found");

            //the columns
            var properties = GetPropertiesByExcelCells<Employee>(worksheet);

            var manager = new PropertyManager<Employee>(properties, _hrSettings);

            var iRow = 2;
            var setSeName = properties.Any(p => p.PropertyName == "SeName");

            //performance optimization, load all employees in one SQL request
            var allEmployees = await (await _employeeService
                .GetAllEmployeesAsync(showHidden: true))
                .GroupByAwait(async c => await _employeeService.GetFormattedBreadCrumbAsync(c))
                .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());

            var saveNextTime = new List<int>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetProperties
                    .Select(property => worksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                //get employee by data in xlsx file if it possible, or create new employee
                var (employee, isNew, currentEmployeeBreadCrumb) = await GetEmployeeFromXlsxAsync(manager, worksheet, iRow, allEmployees);

                //update employee by data in xlsx file
                var (seName, isParentEmployeeExists) = await UpdateEmployeeByXlsxAsync(employee, manager, allEmployees, isNew);

                if (isParentEmployeeExists)
                {
                    //if parent employee exists in database then save employee into database
                    await SaveEmployeeAsync(isNew, employee, allEmployees, currentEmployeeBreadCrumb, setSeName, seName);
                }
                else
                {
                    //if parent employee doesn't exists in database then try save employee into database next time
                    saveNextTime.Add(iRow);
                }

                iRow++;
            }

            var needSave = saveNextTime.Any();

            while (needSave)
            {
                var remove = new List<int>();

                //try to save unsaved employees
                foreach (var rowId in saveNextTime)
                {
                    //get employee by data in xlsx file if it possible, or create new employee
                    var (employee, isNew, currentEmployeeBreadCrumb) = await GetEmployeeFromXlsxAsync(manager, worksheet, rowId, allEmployees);
                    //update employee by data in xlsx file
                    var (seName, isParentEmployeeExists) = await UpdateEmployeeByXlsxAsync(employee, manager, allEmployees, isNew);

                    if (!isParentEmployeeExists)
                        continue;

                    //if parent employee exists in database then save employee into database
                    await SaveEmployeeAsync(isNew, employee, allEmployees, currentEmployeeBreadCrumb, setSeName, seName);
                    remove.Add(rowId);
                }

                saveNextTime.RemoveAll(item => remove.Contains(item));

                needSave = remove.Any() && saveNextTime.Any();
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportEmployees",
                string.Format(await _localizationService.GetResourceAsync(HumanResourceDefaults.Labels.ImportEmployees), iRow - 2 - saveNextTime.Count));

            if (!saveNextTime.Any())
                return;

            var employeesName = new List<string>();

            foreach (var rowId in saveNextTime)
            {
                manager.ReadFromXlsx(worksheet, rowId);
                employeesName.Add(manager.GetProperty("Name").StringValue);
            }

            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync(HumanResourceDefaults.Labels.EmployeesArentImported), string.Join(", ", employeesName)));
        }

        #endregion

        #region Nested classes

        public class EmployeeKey
        {
            /// <returns>A task that represents the asynchronous operation</returns>
            public static async Task<EmployeeKey> CreateEmployeeKeyAsync(Employee employee, IEmployeeService employeeService, IList<Employee> allEmployees, IStoreMappingService storeMappingService)
            {
                return new EmployeeKey(await employeeService.GetFormattedBreadCrumbAsync(employee, allEmployees), employee.LimitedToStores ? (await storeMappingService.GetStoresIdsWithAccessAsync(employee)).ToList() : new List<int>())
                {
                    Employee = employee
                };
            }

            public EmployeeKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public Employee Employee { get; private set; }

            public string Key { get; }

            public bool Equals(EmployeeKey y)
            {
                if (y == null)
                    return false;

                if (Employee != null && y.Employee != null)
                    return Employee.Id == y.Employee.Id;

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
                var other = obj as EmployeeKey;
                return other?.Equals(other) ?? false;
            }
        }

        #endregion
    }
}