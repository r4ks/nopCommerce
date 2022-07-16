using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Data.Mapping;

namespace Nop.Plugin.Widgets.BlankTable.Migrations
{
    [NopMigration("2020/03/13 09:36:08:9037677", "Nop.Data base indexes", MigrationProcessType.Installation)]
    public class Indexes : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {

            Create.Index("IX_Category_ParentCategoryId").OnTable(nameof(Category))
                .OnColumn(nameof(Category.ParentCategoryId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_LimitedToStores").OnTable(nameof(Category))
                .OnColumn(nameof(Category.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_DisplayOrder").OnTable(nameof(Category))
                .OnColumn(nameof(Category.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Category_Deleted_Extended").OnTable(nameof(Category))
                .OnColumn(nameof(Category.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Category.Id))
                .Include(nameof(Category.Name))
                .Include(nameof(Category.SubjectToAcl)).Include(nameof(Category.LimitedToStores))
                .Include(nameof(Category.Published));

            Create.Index("IX_Category_SubjectToAcl").OnTable(nameof(Category))
                .OnColumn(nameof(Category.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();

        }

        #endregion
    }
}
