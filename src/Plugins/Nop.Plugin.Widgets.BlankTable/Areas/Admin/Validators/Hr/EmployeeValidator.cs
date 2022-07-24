using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Validators;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Validators.Hr
{
    public partial class EmployeeValidator : BaseNopValidator<EmployeeModel>
    {
        #region Labels
        public static class Labels
        {
            public const string NameRequired = "Admin.Hr.Employees.Fields.Name.Required";
            public const string PageSizeOptionsShouldHaveUniqueItems = "Admin.Hr.Employees.Fields.PageSizeOptions.ShouldHaveUniqueItems";
            public const string PageSizePositive = "Admin.Hr.Employees.Fields.PageSize.Positive";
            public const string SeNameMaxLengthValidation = "Admin.SEO.SeName.MaxLengthValidation";
        }
        #endregion
        public EmployeeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
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

            SetDatabaseValidationRules<Employee>(mappingEntityAccessor);
        }
    }
}