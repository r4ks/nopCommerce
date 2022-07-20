using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Catalog;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Validators;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Validators.Catalog
{
    public partial class CategoryValidator : BaseNopValidator<CategoryModel>
    {
        #region Labels
        public static class Labels
        {
            public const string NameRequired = "Admin.Catalog.Categories.Fields.Name.Required";
            public const string PageSizeOptionsShouldHaveUniqueItems = "Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems";
            public const string PageSizePositive = "Admin.Catalog.Categories.Fields.PageSize.Positive";
            public const string SeNameMaxLengthValidation = "Admin.SEO.SeName.MaxLengthValidation";
            public const string PriceFromGreaterThanOrEqualZero = "Admin.Catalog.Categories.Fields.PriceFrom.GreaterThanOrEqualZero";
            public const string PriceToGreaterThanZeroOrPriceFrom = "Admin.Catalog.Categories.Fields.PriceTo.GreaterThanZeroOrPriceFrom";
        }
        #endregion
        public CategoryValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync(Labels.NameRequired));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessageAwait(localizationService.GetResourceAsync(Labels.PageSizeOptionsShouldHaveUniqueItems));
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync(Labels.PageSizePositive));
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync(Labels.SeNameMaxLengthValidation), NopSeoDefaults.SearchEngineNameLength);

            RuleFor(x => x.PriceFrom)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync(Labels.PriceFromGreaterThanOrEqualZero))
                .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

            RuleFor(x => x.PriceTo)
                .GreaterThan(x => x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero)
                .WithMessage(x => string.Format(localizationService.GetResourceAsync(Labels.PriceToGreaterThanZeroOrPriceFrom).Result, x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero))
                .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

            SetDatabaseValidationRules<Category>(mappingEntityAccessor);
        }
    }
}