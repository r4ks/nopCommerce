using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;

/// ViewComponent for Widgets
/// To be used to show charts or graphs on dashboard.
namespace Nop.Plugin.Widgets.BlankTable.Components
{
    [ViewComponent(Name = "Custom")]
    public class CustomViewComponent : NopViewComponent
    {
        public CustomViewComponent()
        {

        }

        public IViewComponentResult Invoke(int productId)
        {
            return View("~/Plugins/Widgets.BlankTable/Views/CustomWidget.cshtml");
        }
    }
}
