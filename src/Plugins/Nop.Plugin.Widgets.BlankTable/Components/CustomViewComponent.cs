using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;

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
