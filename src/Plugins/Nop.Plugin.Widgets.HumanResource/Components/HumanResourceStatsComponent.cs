﻿using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;

/// ViewComponent for Widgets
/// To be used to show charts or graphs on dashboard.
namespace Nop.Plugin.Widgets.HumanResource.Components
{
    [ViewComponent(Name = "Custom")]
    public class HumanResourceStatsComponent : NopViewComponent
    {
        public HumanResourceStatsComponent()
        {

        }

        public IViewComponentResult Invoke(int productId)
        {
            return View("~/Plugins/Widgets.HumanResource/Views/HumanResourceStatsWidget.cshtml");
        }
    }
}