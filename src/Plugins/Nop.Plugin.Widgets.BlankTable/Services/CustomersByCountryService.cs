using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.BlankTable.Models;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;

namespace Nop.Plugin.Widgets.BlankTable.Services
{
    public class CustomersByCountryService : ICustomersByCountryService
    {
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;

        public CustomersByCountryService(IAddressService addressService, ICountryService countryService, ICustomerService customerService)
        {
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
        }

        public async Task<List<CustomersDistributionModel>> GetCustomersDistributionByCountryAsync()
        {
            return await (await _customerService.GetAllCustomersAsync())
                .Where(c => c.ShippingAddressId != null)
                .Select(async c => new
                {
                     (await _countryService.GetCountryByAddressAsync(await _addressService.GetAddressByIdAsync(c.ShippingAddressId ?? 0))).Name,
                    c.Username
                })
                .GroupBy(c => c.Result.Name)
                .Select(cbc => new CustomersDistributionModel
                {
                    Country = cbc.Key,
                    NoOfCustomers = cbc.Count()
                }).ToListAsync();
        }
    }
}
