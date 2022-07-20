using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;

namespace Nop.Plugin.Widgets.BlankTable.Migrations
{
    [NopMigration("2022-07-04 15:36:00", "Nop.Plugin.Widgets.BlankTable schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<Employee>();
        }
    }
}
