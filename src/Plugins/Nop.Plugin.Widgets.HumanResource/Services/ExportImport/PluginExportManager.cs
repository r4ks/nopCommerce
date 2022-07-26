using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core;
using Nop.Plugin.Widgets.HumanResource.Services.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.ExportImport.Help;
using Nop.Services.Common;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class PluginExportManager : IPluginExportManager
    {
        #region Fields

        private readonly EmployeeSettings _hrSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEmployeeService _employeeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PluginExportManager(
            EmployeeSettings employeeSettings,
            ICustomerActivityService customerActivityService,
            IEmployeeService employeeService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext
            )
        {
            _hrSettings = employeeSettings;
            _customerActivityService = customerActivityService;
            _employeeService = employeeService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<int> WriteEmployeesAsync(XmlWriter xmlWriter, int parentEmployeeId, int totalEmployees)
        {
            var employees = await _employeeService.GetAllEmployeesByParentEmployeeIdAsync(parentEmployeeId, true);
            if (employees == null || !employees.Any())
                return totalEmployees;

            totalEmployees += employees.Count;

            foreach (var employee in employees)
            {
                await xmlWriter.WriteStartElementAsync("Employee");

                await xmlWriter.WriteStringAsync("Id", employee.Id);

                await xmlWriter.WriteStringAsync("Name", employee.Name);
                await xmlWriter.WriteStringAsync("Description", employee.Description);
                await xmlWriter.WriteStringAsync("SeName", await _urlRecordService.GetSeNameAsync(employee, 0), await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("ParentEmployeeId", employee.ParentEmployeeId);
                await xmlWriter.WriteStringAsync("PictureId", employee.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", employee.PageSize, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", employee.AllowCustomersToSelectPageSize, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("PageSizeOptions", employee.PageSizeOptions, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("ShowOnHomepage", employee.ShowOnHomepage, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("IncludeInTopMenu", employee.IncludeInTopMenu, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("Published", employee.Published, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("Deleted", employee.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", employee.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", employee.CreatedOnUtc, await IgnoreExportEmployeePropertyAsync());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", employee.UpdatedOnUtc, await IgnoreExportEmployeePropertyAsync());

                await xmlWriter.WriteStartElementAsync("Products");

                await xmlWriter.WriteEndElementAsync();

                await xmlWriter.WriteStartElementAsync("SubEmployees");
                totalEmployees = await WriteEmployeesAsync(xmlWriter, employee.Id, totalEmployees);
                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndElementAsync();
            }

            return totalEmployees;
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
        protected virtual async Task<bool> IgnoreExportEmployeePropertyAsync()
        {
            try
            {
                return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "employee-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// Export employee list to XML
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportEmployeesToXmlAsync()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Employees");
            await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);
            var totalEmployees = await WriteEmployeesAsync(xmlWriter, 0, 0);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportEmployees",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportEmployees"), totalEmployees));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export employees to XLSX
        /// </summary>
        /// <param name="employees">Employees</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<byte[]> ExportEmployeesToXlsxAsync(IList<Employee> employees)
        {
            var parentEmployees = new List<Employee>();
            if (_hrSettings.ExportImportEmployeesUsingEmployeeName)
                //performance optimization, load all parent employees in one SQL request
                parentEmployees.AddRange(await _employeeService.GetEmployeesByIdsAsync(employees.Select(c => c.ParentEmployeeId).Where(id => id != 0).ToArray()));

            //property manager 
            var manager = new PropertyManager<Employee>(new[]
            {
                new PropertyByName<Employee>("Id", p => p.Id),
                new PropertyByName<Employee>("Name", p => p.Name),
                new PropertyByName<Employee>("Description", p => p.Description),
                new PropertyByName<Employee>("SeName", async p => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("ParentEmployeeId", p => p.ParentEmployeeId),
                new PropertyByName<Employee>("ParentEmployeeName", async p =>
                {
                    var employee = parentEmployees.FirstOrDefault(c => c.Id == p.ParentEmployeeId);
                    return employee != null ? await _employeeService.GetFormattedBreadCrumbAsync(employee) : null;

                }, !_hrSettings.ExportImportEmployeesUsingEmployeeName),
                new PropertyByName<Employee>("Picture", async p => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Employee>("PageSize", p => p.PageSize, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("IncludeInTopMenu", p => p.IncludeInTopMenu, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("Published", p => p.Published, await IgnoreExportEmployeePropertyAsync()),
                new PropertyByName<Employee>("DisplayOrder", p => p.DisplayOrder)
            }, _hrSettings);

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportEmployees",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportEmployees"), employees.Count));

            return await manager.ExportToXlsxAsync(employees);
        }

         #endregion
    }
}
