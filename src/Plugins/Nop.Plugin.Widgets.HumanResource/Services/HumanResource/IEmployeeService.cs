using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.HumanResource
{
    /// <summary>
    /// Employee service interface
    /// </summary>
    public partial interface IEmployeeService
    {
        /// <summary>
        /// Delete employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteEmployeeAsync(Employee employee);

        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IList<Employee>> GetAllEmployeesAsync(int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <param name="employeeName">Employee name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IPagedList<Employee>> GetAllEmployeesAsync(string employeeName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null);

        /// <summary>
        /// Gets all employees filtered by parent employee identifier
        /// </summary>
        /// <param name="parentEmployeeId">Parent employee identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IList<Employee>> GetAllEmployeesByParentEmployeeIdAsync(int parentEmployeeId, bool showHidden = false);

        /// <summary>
        /// Gets all employees displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IList<Employee>> GetAllEmployeesDisplayedOnHomepageAsync(bool showHidden = false);

        /// <summary>
        /// Gets child employee identifiers
        /// </summary>
        /// <param name="parentEmployeeId">Parent employee identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee identifiers
        /// </returns>
        Task<IList<int>> GetChildEmployeeIdsAsync(int parentEmployeeId, int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee
        /// </returns>
        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        /// <summary>
        /// Inserts employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertEmployeeAsync(Employee employee);

        /// <summary>
        /// Updates the employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateEmployeeAsync(Employee employee);

        /// <summary>
        /// Delete a list of employees
        /// </summary>
        /// <param name="employees">Employees</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteEmployeesAsync(IList<Employee> employees);

        /// <summary>
        /// Returns a list of names of not existing employees
        /// </summary>
        /// <param name="employeeIdsNames">The names and/or IDs of the employees to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing employees
        /// </returns>
        Task<string[]> GetNotExistingEmployeesAsync(string[] employeeIdsNames);

        /// <summary>
        /// Gets employees by identifier
        /// </summary>
        /// <param name="employeeIds">Employee identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IList<Employee>> GetEmployeesByIdsAsync(int[] employeeIds);


        /// <summary>
        /// Get formatted employee breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <param name="allEmployees">All employees</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted breadcrumb
        /// </returns>
        Task<string> GetFormattedBreadCrumbAsync(Employee employee, IList<Employee> allEmployees = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get employee breadcrumb 
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <param name="allEmployees">All employees</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee breadcrumb 
        /// </returns>
        Task<IList<Employee>> GetEmployeeBreadCrumbAsync(Employee employee, IList<Employee> allEmployees = null, bool showHidden = false);

        /// <summary>
        /// Search Employees
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        Task<IPagedList<Employee>> SearchEmployeesAsync(
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

    }
}