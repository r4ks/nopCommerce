using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Controllers.Settings;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.HumanResource.Web.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: HumanResourceDefaults.ConfigurationRouteName,
                pattern: "Admin/EmployeeSetting/Configure",
                new { Controller = "EmployeeSetting", action = EmployeeSettingController.ConfigureActionName }
            );
        }
    }
}
