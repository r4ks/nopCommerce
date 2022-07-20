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
            public const string AdditionalFee = Prefix + ".AdditionalFee";
            public const string AdditionalFeeHint = Prefix + ".AdditionalFee.Hint";
            public const string DescriptionText = Prefix + ".DescriptionText";
            public const string DescriptionTextHint = Prefix + ".DescriptionText.Hint";
            public const string AdditionalFeePercentage = Prefix + ".AdditionalFeePercentage";
            public const string AdditionalFeePercentageHint = Prefix + ".AdditionalFeePercentage.Hint";
            public const string PaymentMethodDescription = Prefix + ".PaymentMethodDescription";
            public const string ShippableProductRequired = Prefix + ".ShippableProductRequired";
            public const string ShippableProductRequiredHint = Prefix + ".ShippableProductRequired.Hint";

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

        [NopResourceDisplayName(Labels.AdditionalFee)]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.AdditionalFeePercentage)]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName(Labels.ShippableProductRequired)]
        public bool ShippableProductRequired { get; set; }
        public bool ShippableProductRequired_OverrideForStore { get; set; }

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