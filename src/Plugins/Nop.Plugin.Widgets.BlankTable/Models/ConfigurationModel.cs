using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        #region Key Labels for Localized Text
        public static class Labels
        {
            public const string Prefix = "Plugins.Payment.CheckMoneyOrder";
            public const string DescriptionText = Prefix + ".DescriptionText";
            public const string DescriptionTextHint = Prefix + ".DescriptionText.Hint";
            public const string PaymentMethodDescription = Prefix + ".PaymentMethodDescription";
            public const string EnableDashboardWidget = Prefix + ".EnableDashboardWidget";
            public const string EnableDashboardWidgetHint = Prefix + ".EnableDashboardWidget.Hint";
            public const string InstallSampleData = Prefix + ".InstallSampleData";
            public const string InstallSampleDataHint = Prefix + ".InstallSampleData.Hint";

        }
        #endregion

        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName(Labels.DescriptionText)]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.EnableDashboardWidget)]
        public bool EnableDashboardWidget { get; set; }
        public bool EnableDashboardWidget_OverrideForStore { get; set; }
        [NopResourceDisplayName(Labels.InstallSampleData)]
        public bool InstallSampleData { get; set; }
        public bool InstallSampleData_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }
            
            [NopResourceDisplayName(Labels.DescriptionText)]
            public string DescriptionText { get; set; }
        }

        #endregion

    }
}