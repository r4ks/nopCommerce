using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Widgets.HumanResource.Services.Security
{
        public class APermissionProvider : IPermissionProvider
        {
            private readonly PermissionRecord _permissionRecord = new()
            {
                Name = "Test name", SystemName = "Test permission record system name", Category = "Test employee"
            };

            public IEnumerable<PermissionRecord> GetPermissions()
            {
                return new[] {
                    ManageEmployees,
                    _permissionRecord};
            }

            public static readonly PermissionRecord ManageEmployees = new() { Name = "Admin area. Manage Employees", SystemName = "ManageEmployees", Category = "HumanResource" };
            public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
            {
                return new()
                {
                    (
                        NopCustomerDefaults.AdministratorsRoleName,
                        new[] {
                            ManageEmployees,
                            _permissionRecord
                        }
                    )
                };
            }
        }
}
