using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.HumanResource.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
                viewLocations = new[] { $"/Plugins/Nop.Plugin.Widgets.HumanResource/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
            else
                viewLocations = new[] { $"/Plugins/Nop.Plugin.Widgets.HumanResource/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);

            return viewLocations;
        }
    }
}
