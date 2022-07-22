using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Widgets.BlankTable
{
    public class BlankTableDefaults
    {
        public static string SystemName => "Widgets.BlankTable";

        public static string UserAgent => $"nopcommerce-{NopVersion.CURRENT_VERSION}";

        public static string ConfigurationRouteName => "Plugin.Widgets.BlankTable.Configure";

        // Common Labels
        public static class Labels {
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

            //Others
            public const string OrderStatistics = "Admin.SalesReport.OrderStatistics";
            public const string OYear = "Admin.SalesReport.OrderStatistics.Year";
            public const string OMonth = "Admin.SalesReport.OrderStatistics.Month";
            public const string OWeek = "Admin.SalesReport.OrderStatistics.Week";

            public const string Saved = "Admin.Plugins.Saved";
            public const string ImportEmployees = "ActivityLog.ImportEmployees";
            public const string EmployeesArentImported = "Admin.Catalog.Employees.Import.EmployeesArentImported";
        }
    }
}
