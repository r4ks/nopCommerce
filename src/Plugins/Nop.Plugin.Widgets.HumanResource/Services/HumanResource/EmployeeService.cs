using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Services.HumanResource
{
    /// <summary>
    /// Employee service
    /// </summary>
    public partial class EmployeeService : IEmployeeService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EmployeeService(
            IAclService aclService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<Employee> employeeRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _localizationService = localizationService;
            _employeeRepository = employeeRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Sort employees for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent employee identifier</param>
        /// <param name="ignoreEmployeesWithoutExistingParent">A value indicating whether employees without parent employee in provided employee list (source) should be ignored</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sorted employees
        /// </returns>
        protected virtual async Task<IList<Employee>> SortEmployeesForTreeAsync(IList<Employee> source, int parentId = 0,
            bool ignoreEmployeesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<Employee>();

            foreach (var cat in source.Where(c => c.ParentEmployeeId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(await SortEmployeesForTreeAsync(source, cat.Id, true));
            }

            if (ignoreEmployeesWithoutExistingParent || result.Count == source.Count)
                return result;

            //find employees without parent in provided employee source and insert them into result
            foreach (var cat in source)
                if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                    result.Add(cat);

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteEmployeeAsync(Employee employee)
        {
            await _employeeRepository.DeleteAsync(employee);

            //reset a "Parent employee" property of all child subemployees
            var subemployees = await GetAllEmployeesByParentEmployeeIdAsync(employee.Id, true);
            foreach (var subemployee in subemployees)
            {
                subemployee.ParentEmployeeId = 0;
                await UpdateEmployeeAsync(subemployee);
            }
        }

        /// <summary>
        /// Delete Employees
        /// </summary>
        /// <param name="employees">Employees</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteEmployeesAsync(IList<Employee> employees)
        {
            if (employees == null)
                throw new ArgumentNullException(nameof(employees));

            foreach (var employee in employees)
                await DeleteEmployeeAsync(employee);
        }

        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        public virtual async Task<IList<Employee>> GetAllEmployeesAsync(int storeId = 0, bool showHidden = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeesAllCacheKey,
                storeId,
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                showHidden);

            var employees = await _staticCacheManager
                .GetAsync(key, async () => (await GetAllEmployeesAsync(string.Empty, storeId, showHidden: showHidden)).ToList());

            return employees;
        }

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
        public virtual async Task<IPagedList<Employee>> GetAllEmployeesAsync(string employeeName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null)
        {
            var unsortedEmployees = await _employeeRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                    query = query.Where(c => c.Published);
                else if (overridePublished.HasValue)
                    query = query.Where(c => c.Published == overridePublished.Value);

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                if (!showHidden)
                {
                    var customer = await _workContext.GetCurrentCustomerAsync();
                    query = await _aclService.ApplyAcl(query, customer);
                }

                if (!string.IsNullOrWhiteSpace(employeeName))
                    query = query.Where(c => c.Name.Contains(employeeName));

                query = query.Where(c => !c.Deleted);

                return query.OrderBy(c => c.ParentEmployeeId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            });

            //sort employees
            var sortedEmployees = await SortEmployeesForTreeAsync(unsortedEmployees);

            //paging
            return new PagedList<Employee>(sortedEmployees, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all employees filtered by parent employee identifier
        /// </summary>
        /// <param name="parentEmployeeId">Parent employee identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        public virtual async Task<IList<Employee>> GetAllEmployeesByParentEmployeeIdAsync(int parentEmployeeId,
            bool showHidden = false)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var employees = await _employeeRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                {
                    query = query.Where(c => c.Published);

                    //apply store mapping constraints
                    query = await _storeMappingService.ApplyStoreMapping(query, store.Id);

                    //apply ACL constraints
                    query = await _aclService.ApplyAcl(query, customer);
                }

                query = query.Where(c => !c.Deleted && c.ParentEmployeeId == parentEmployeeId);

                return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }, cache => cache.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeesByParentEmployeeCacheKey,
                parentEmployeeId, showHidden, customer, store));

            return employees;
        }

        /// <summary>
        /// Gets all employees displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        public virtual async Task<IList<Employee>> GetAllEmployeesDisplayedOnHomepageAsync(bool showHidden = false)
        {
            var employees = await _employeeRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.DisplayOrder, c.Id
                       where c.Published &&
                             !c.Deleted &&
                             c.ShowOnHomepage
                       select c;
            }, cache => cache.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeesHomepageCacheKey));

            if (showHidden)
                return employees;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeesHomepageWithoutHiddenCacheKey,
                await _storeContext.GetCurrentStoreAsync(), await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()));

            var result = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                return await employees
                    .WhereAwait(async c => await _aclService.AuthorizeAsync(c) && await _storeMappingService.AuthorizeAsync(c))
                    .ToListAsync();
            });

            return result;
        }

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
        public virtual async Task<IList<int>> GetChildEmployeeIdsAsync(int parentEmployeeId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeesChildIdsCacheKey,
                parentEmployeeId,
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                storeId,
                showHidden);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                //little hack for performance optimization
                //there's no need to invoke "GetAllEmployeesByParentEmployeeId" multiple times (extra SQL commands) to load childs
                //so we load all employees at once (we know they are cached) and process them server-side
                var employeesIds = new List<int>();
                var employees = (await GetAllEmployeesAsync(storeId: storeId, showHidden: showHidden))
                    .Where(c => c.ParentEmployeeId == parentEmployeeId)
                    .Select(c => c.Id)
                    .ToList();
                employeesIds.AddRange(employees);
                employeesIds.AddRange(await employees.SelectManyAwait(async cId => await GetChildEmployeeIdsAsync(cId, storeId, showHidden)).ToListAsync());

                return employeesIds;
            });
        }

        /// <summary>
        /// Gets a employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employee
        /// </returns>
        public virtual async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetByIdAsync(employeeId, cache => default);
        }

        /// <summary>
        /// Inserts employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertEmployeeAsync(Employee employee)
        {
            await _employeeRepository.InsertAsync(employee);
        }

        /// <summary>
        /// Updates the employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            //validate employee hierarchy
            var parentEmployee = await GetEmployeeByIdAsync(employee.ParentEmployeeId);
            while (parentEmployee != null)
            {
                if (employee.Id == parentEmployee.Id)
                {
                    employee.ParentEmployeeId = 0;
                    break;
                }

                parentEmployee = await GetEmployeeByIdAsync(parentEmployee.ParentEmployeeId);
            }

            await _employeeRepository.UpdateAsync(employee);
        }



        /// <summary>
        /// Returns a list of names of not existing employees
        /// </summary>
        /// <param name="employeeIdsNames">The names and/or IDs of the employees to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing employees
        /// </returns>
        public virtual async Task<string[]> GetNotExistingEmployeesAsync(string[] employeeIdsNames)
        {
            if (employeeIdsNames == null)
                throw new ArgumentNullException(nameof(employeeIdsNames));

            var query = _employeeRepository.Table;
            var queryFilter = employeeIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = await query.Select(c => c.Name)
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

             queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = await query.Select(c => c.Id.ToString())
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Gets employees by identifier
        /// </summary>
        /// <param name="employeeIds">Employee identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the employees
        /// </returns>
        public virtual async Task<IList<Employee>> GetEmployeesByIdsAsync(int[] employeeIds)
        {
            return await _employeeRepository.GetByIdsAsync(employeeIds, includeDeleted: false);
        }

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
        public virtual async Task<string> GetFormattedBreadCrumbAsync(Employee employee, IList<Employee> allEmployees = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = await GetEmployeeBreadCrumbAsync(employee, allEmployees, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var employeeName = await _localizationService.GetLocalizedAsync(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? employeeName : $"{result} {separator} {employeeName}";
            }

            return result;
        }

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
        public virtual async Task<IList<Employee>> GetEmployeeBreadCrumbAsync(Employee employee, IList<Employee> allEmployees = null, bool showHidden = false)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var breadcrumbCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopEmployeeDefaults.EmployeeBreadcrumbCacheKey,
                employee,
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync(),
                await _workContext.GetWorkingLanguageAsync());

            return await _staticCacheManager.GetAsync(breadcrumbCacheKey, async () =>
            {
                var result = new List<Employee>();

                //used to prevent circular references
                var alreadyProcessedEmployeeIds = new List<int>();

                while (employee != null && //not null
                       !employee.Deleted && //not deleted
                       (showHidden || employee.Published) && //published
                       (showHidden || await _aclService.AuthorizeAsync(employee)) && //ACL
                       (showHidden || await _storeMappingService.AuthorizeAsync(employee)) && //Store mapping
                       !alreadyProcessedEmployeeIds.Contains(employee.Id)) //prevent circular references
                {
                    result.Add(employee);

                    alreadyProcessedEmployeeIds.Add(employee.Id);

                    employee = allEmployees != null
                        ? allEmployees.FirstOrDefault(c => c.Id == employee.ParentEmployeeId)
                        : await GetEmployeeByIdAsync(employee.ParentEmployeeId);
                }

                result.Reverse();

                return result;
            });
        }

        /// <summary>
        /// Search employees
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
        public virtual async Task<IPagedList<Employee>> SearchEmployeesAsync(
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _employeeRepository.Table;

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        #endregion
    }
}