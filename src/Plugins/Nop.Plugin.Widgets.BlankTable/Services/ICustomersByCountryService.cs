using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.BlankTable.Models;

namespace Nop.Plugin.Widgets.BlankTable.Services
{
    public interface ICustomersByCountryService
    {
        Task<List<CustomersDistributionModel>> GetCustomersDistributionByCountryAsync();
    }
}
