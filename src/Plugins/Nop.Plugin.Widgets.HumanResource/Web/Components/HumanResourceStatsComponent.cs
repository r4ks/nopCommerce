using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;

/// ViewComponent for Widgets
/// To be used to show charts or graphs on dashboard.
namespace Nop.Plugin.Widgets.HumanResource.Web.Components
{
    [ViewComponent(Name = "StatsWidget")]
    public class HumanResourceStatsComponent : NopViewComponent
    {
        public HumanResourceStatsComponent()
        {

        }

        public IViewComponentResult Invoke(int productId)
        {
            return View("~/Plugins/Widgets.HumanResource/Web/Views/HumanResourceStatsWidget.cshtml");
        }
    }
}
