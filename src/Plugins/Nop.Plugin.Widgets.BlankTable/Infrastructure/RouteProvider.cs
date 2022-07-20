using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.BlankTable.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: BlankTableDefaults.ConfigurationRouteName,
                pattern: "Admin/BlankTable/Configure",
                new { Controller = "BlankTable", action = "GetCustomersCountByCountry" }
            );
        }
    }
}
