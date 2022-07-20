using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.BlankTable.Domains.Hr;

namespace Nop.Plugin.Widgets.BlankTable.Mapping.Builders.Hr
{
    /// <summary>
    /// Represents a employee entity builder
    /// </summary>
    public partial class EmployeeBuilder : NopEntityBuilder<Employee>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Employee.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Employee.PageSizeOptions)).AsString(200).Nullable();
        }

        #endregion
    }
}