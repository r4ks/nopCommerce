﻿using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.BlankTable.Domains.Catalog;

namespace Nop.Plugin.Widgets.BlankTable.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a category entity builder
    /// </summary>
    public partial class CategoryBuilder : NopEntityBuilder<Category>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Category.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Category.PageSizeOptions)).AsString(200).Nullable();
        }

        #endregion
    }
}