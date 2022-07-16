﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.BlankTable.Models.Category
{
    /// <summary>
    /// Represents a product model to add to the category
    /// </summary>
    public partial record AddProductToCategoryModel : BaseNopModel
    {
        #region Ctor

        public AddProductToCategoryModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}