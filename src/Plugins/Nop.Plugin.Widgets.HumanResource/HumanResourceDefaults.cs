using Nop.Core;

namespace Nop.Plugin.Widgets.HumanResource
{
    public class HumanResourceDefaults
    {
        public static string SystemName => "Widgets.HumanResource";

        public static string UserAgent => $"nopcommerce-{NopVersion.CURRENT_VERSION}";

        public static string ConfigurationRouteName => "Plugin.Widgets.HumanResource.Areas.Admin.EmployeeSetting.Configure";

        // Common Labels
        public static class Labels {
            // Standard Localized String
            // *Recommended to not override these keys:
            public const string All = "Admin.Common.All";
            public const string AddNew = "Admin.Common.AddNew";
            public const string Export = "Admin.Common.Export";
            public const string ExportToXml = "Admin.Common.ExportToXml";
            public const string ExportToExcel = "Admin.Common.ExportToExcel";
            public const string Import = "Admin.Common.Import";
            public const string Selected = "Admin.Common.Delete.Selected";
            public const string Search = "Admin.Common.Search";
            public const string Edit = "Admin.Common.Edit";
            public const string NothingSelected = "Admin.Common.Alert.NothingSelected";
            public const string ImportFromExcel = "Admin.Common.ImportFromExcel";
            public const string ManyRecordsWarning = "Admin.Common.ImportFromExcel.ManyRecordsWarning";
            public const string ExcelFile = "Admin.Common.ExcelFile";
            public const string Save = "Admin.Common.Save";
            public const string SaveContinue = "Admin.Common.SaveContinue";
            public const string Preview = "Admin.Common.Preview";
            public const string Delete = "Admin.Common.Delete";
            public const string View = "Admin.Common.View";
            public const string SEO = "Admin.Common.SEO";
            public const string Saved = "Admin.Plugins.Saved";

            // Others
            public const string EmployeeStatistics = "Admin.HumanResource.Employees.EmployeesStatistics";
            public const string OYear = "Admin.HumanResource.EmployeeStatistics.Year";
            public const string OMonth = "Admin.HumanResource.EmployeeStatistics.Month";
            public const string OWeek = "Admin.HumanResource.EmployeeStatistics.Week";

            public const string ImportEmployees = "ActivityLog.ImportEmployees";
            public const string EmployeesArentImported = "Admin.HumanResource.Employees.Import.EmployeesArentImported";

            // Menu Item
            public const string HumanResourceNodeTitle = "HumanResource.MainNode.Title";
        }
    }
}
