using System.Linq;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;

namespace Nop.Plugin.Widgets.BlankTable.Services.Catalog
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in order according to a product sorting rule
        /// </summary>
        /// <param name="productsQuery">A sequence of products to order</param>
        /// <param name="currentLanguage">Current language</param>
        /// <param name="orderBy">Product sorting rule</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
        /// <remarks>
        /// If <paramref name="orderBy"/> is set to <c>Position</c> and passed <paramref name="productsQuery"/> is
        /// ordered sorting rule will be skipped
        /// </remarks>
        public static IQueryable<Product> OrderBy(this IQueryable<Product> productsQuery, IRepository<LocalizedProperty> localizedPropertyRepository, Language currentLanguage)
        {
                var currentLanguageId = currentLanguage.Id;
                
                var query =
                    from product in productsQuery
                    join localizedProperty in localizedPropertyRepository.Table on new
                        {
                            product.Id,
                            languageId = currentLanguageId,
                            keyGroup = nameof(Product),
                            key = nameof(Product.Name)
                        }
                        equals new
                        {
                            Id = localizedProperty.EntityId,
                            languageId = localizedProperty.LanguageId,
                            keyGroup = localizedProperty.LocaleKeyGroup,
                            key = localizedProperty.LocaleKey
                        } into localizedProperties
                    from localizedProperty in localizedProperties.DefaultIfEmpty(new LocalizedProperty { LocaleValue = product.Name })
                    select new { localizedProperty, product };

                    productsQuery = from item in query
                        orderby item.localizedProperty.LocaleValue, item.product.Name
                        select item.product;

                return productsQuery;
        }
    }
}