using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr
{
    /// <summary>
    /// Represents a employee search model
    /// </summary>
    public partial record EmployeeSearchModel : BaseSearchModel
    {
        public const string LIST_VIEW = "~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Employee/List.cshtml";
        #region Labels
        public static class Labels
        {

            public const string SearchEmployeeName = "Admin.Hr.Employees.List.SearchEmployeeName";
            public const string SearchStoreId = "Admin.Hr.Employees.List.SearchStore";
            public const string ImportFromExcelTip = "Admin.Hr.Employees.List.ImportFromExcelTip";

            public const string SearchPublishedId = "Admin.Hr.Employees.List.SearchPublished";
            public const string All = "Admin.Hr.Employees.List.SearchPublished.All";
            public const string PublishedOnly = "Admin.Hr.Employees.List.SearchPublished.PublishedOnly";
            public const string UnpublishedOnly = "Admin.Hr.Employees.List.SearchPublished.UnpublishedOnly";
        }

        #endregion
        #region Ctor

        public EmployeeSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName(Labels.SearchEmployeeName)]
        public string SearchEmployeeName { get; set; }

        [NopResourceDisplayName(Labels.SearchPublishedId)]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        [NopResourceDisplayName(Labels.SearchStoreId)]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}