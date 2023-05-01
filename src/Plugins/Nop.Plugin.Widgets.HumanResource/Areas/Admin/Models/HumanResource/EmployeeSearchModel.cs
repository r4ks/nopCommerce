using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource
{
    /// <summary>
    /// Represents a employee search model
    /// </summary>
    public partial record EmployeeSearchModel : BaseSearchModel
    {
        public const string LIST_VIEW = "~/Plugins/Nop.Plugin.Widgets.HumanResource/Areas/Admin/Views/Employee/List.cshtml";
        public const string SYSTEM_NAME = "Nop.Plugin.Widgets.HumanResource.EmployeeListMenuItem";
        #region Labels
        public static class Labels
        {

            public const string SearchEmployeeName = "Admin.HumanResource.Employees.List.SearchEmployeeName";
            public const string SearchStoreId = "Admin.HumanResource.Employees.List.SearchStore";
            public const string ImportFromExcelTip = "Admin.HumanResource.Employees.List.ImportFromExcelTip";

            public const string SearchPublishedId = "Admin.HumanResource.Employees.List.SearchPublished";
            public const string All = "Admin.HumanResource.Employees.List.SearchPublished.All";
            public const string PublishedOnly = "Admin.HumanResource.Employees.List.SearchPublished.PublishedOnly";
            public const string UnpublishedOnly = "Admin.HumanResource.Employees.List.SearchPublished.UnpublishedOnly";

            public const string DownloadPDF = "Admin.HumanResource.Employees.List.DownloadPDF";

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