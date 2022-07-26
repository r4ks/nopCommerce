using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;

namespace Nop.Plugin.Widgets.HumanResource.Data.Migrations
{
    [NopMigration("2020/03/13 09:36:08:9037677", "Nop.Data base indexes", MigrationProcessType.Installation)]
    public class Indexes : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {

            Create.Index("IX_Employee_ParentEmployeeId").OnTable(nameof(Employee))
                .OnColumn(nameof(Employee.ParentEmployeeId)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Employee_LimitedToStores").OnTable(nameof(Employee))
                .OnColumn(nameof(Employee.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Employee_DisplayOrder").OnTable(nameof(Employee))
                .OnColumn(nameof(Employee.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_Employee_Deleted_Extended").OnTable(nameof(Employee))
                .OnColumn(nameof(Employee.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Employee.Id))
                .Include(nameof(Employee.Name))
                .Include(nameof(Employee.SubjectToAcl)).Include(nameof(Employee.LimitedToStores))
                .Include(nameof(Employee.Published));

            Create.Index("IX_Employee_SubjectToAcl").OnTable(nameof(Employee))
                .OnColumn(nameof(Employee.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();

        }

        #endregion
    }
}
