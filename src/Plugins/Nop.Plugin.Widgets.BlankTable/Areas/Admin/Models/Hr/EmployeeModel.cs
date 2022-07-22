using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.BlankTable.Areas.Admin.Models.Hr
{
    /// <summary>
    /// Represents a employee model
    /// </summary>
    public partial record EmployeeModel : BaseNopEntityModel, IAclSupportedModel, IDiscountSupportedModel,
        ILocalizedModel<EmployeeLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Views file path
        public const string CREATE_VIEW = "~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Employee/Create.cshtml";
        public const string CREATE_OR_UPDATE_VIEW = "~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Employee/_CreateOrUpdate.cshtml";
        public const string EDIT_VIEW = "~/Plugins/Widgets.BlankTable/Areas/Admin/Views/Employee/Edit.cshtml";
        #endregion

        #region Ctor

        public EmployeeModel()
        {
            if (PageSize < 1)
                PageSize = 5;

            Locales = new List<EmployeeLocalizedModel>();
            AvailableEmployees = new List<SelectListItem>();
            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

        }

        #endregion
        #region Labels
        public static partial class Labels
        {
            public const string Name = "Admin.Catalog.Employees.Fields.Name";
            public const string Description = "Admin.Catalog.Employees.Fields.Description";
            public const string SeName = "Admin.Catalog.Employees.Fields.SeName";
            public const string ParentEmployeeId = "Admin.Catalog.Employees.Fields.Parent";
            public const string PictureId = "Admin.Catalog.Employees.Fields.Picture";
            public const string PageSize = "Admin.Catalog.Employees.Fields.PageSize";
            public const string AllowCustomersToSelectPageSize = "Admin.Catalog.Employees.Fields.AllowCustomersToSelectPageSize";
            public const string PageSizeOptions = "Admin.Catalog.Employees.Fields.PageSizeOptions";
            public const string PriceRangeFiltering = "Admin.Catalog.Employees.Fields.PriceRangeFiltering";
            public const string PriceFrom = "Admin.Catalog.Employees.Fields.PriceFrom";
            public const string PriceTo = "Admin.Catalog.Employees.Fields.PriceTo";
            public const string ManuallyPriceRange = "Admin.Catalog.Employees.Fields.ManuallyPriceRange";
            public const string ShowOnHomepage = "Admin.Catalog.Employees.Fields.ShowOnHomepage";
            public const string IncludeInTopMenu = "Admin.Catalog.Employees.Fields.IncludeInTopMenu";
            public const string Published = "Admin.Catalog.Employees.Fields.Published";
            public const string Deleted = "Admin.Catalog.Employees.Fields.Deleted";
            public const string DisplayOrder = "Admin.Catalog.Employees.Fields.DisplayOrder";
            public const string SelectedCustomerRoleIds = "Admin.Catalog.Employees.Fields.AclCustomerRoles";
            public const string SelectedStoreIds = "Admin.Catalog.Employees.Fields.LimitedToStores";
            public const string SelectedDiscountIds = "Admin.Catalog.Employees.Fields.Discounts";
            public const string None = "Admin.Catalog.Employees.Fields.Parent.None";

            //View Labels:
            public const string NoDiscounts = "Admin.Catalog.Employees.Fields.Discounts.NoDiscounts";
            public const string EditEmployeeDetails = "Admin.Catalog.Employees.EditEmployeeDetails";
            public const string BackToList = "Admin.Catalog.Employees.BackToList";
            public const string AddNew = "Admin.Catalog.Employees.AddNew";
            public const string Info = "Admin.Catalog.Employees.Info";
            public const string Display = "Admin.Catalog.Employees.Display";
            public const string Mappings = "Admin.Catalog.Employees.Mappings";
            public const string Products = "Admin.Catalog.Employees.Products";

            // Events
            public const string AddedEvent = "Admin.Catalog.Employees.Added";
            public const string UpdatedEvent = "Admin.Catalog.Employees.Updated";
            public const string DeletedEvent = "Admin.Catalog.Employees.Deleted";
            public const string ImportedEvent = "Admin.Catalog.Employees.Imported";

            // Notifications
            public const string LogAddNewEmployee = "ActivityLog.AddNewEmployee";
            public const string LogEditEmployee = "ActivityLog.EditEmployee";
            public const string LogDeleteEmployee = "ActivityLog.DeleteEmployee";
            public const string LogUploadFile = "Admin.Common.UploadFile";
        }
        #endregion

        #region Properties

        [NopResourceDisplayName(Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(Labels.Description)]
        public string Description { get; set; }

        [NopResourceDisplayName(Labels.SeName)]
        public string SeName { get; set; }

        [NopResourceDisplayName(Labels.ParentEmployeeId)]
        public int ParentEmployeeId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName(Labels.PictureId)]
        public int PictureId { get; set; }

        [NopResourceDisplayName(Labels.PageSize)]
        public int PageSize { get; set; }

        [NopResourceDisplayName(Labels.AllowCustomersToSelectPageSize)]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName(Labels.PageSizeOptions)]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName(Labels.PriceRangeFiltering)]
        public bool PriceRangeFiltering { get; set; }

        [NopResourceDisplayName(Labels.PriceFrom)]
        public decimal PriceFrom { get; set; }

        [NopResourceDisplayName(Labels.PriceTo)]
        public decimal PriceTo { get; set; }

        [NopResourceDisplayName(Labels.ManuallyPriceRange)]
        public bool ManuallyPriceRange { get; set; }

        [NopResourceDisplayName(Labels.ShowOnHomepage)]
        public bool ShowOnHomepage { get; set; }

        [NopResourceDisplayName(Labels.IncludeInTopMenu)]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName(Labels.Published)]
        public bool Published { get; set; }

        [NopResourceDisplayName(Labels.Deleted)]
        public bool Deleted { get; set; }

        [NopResourceDisplayName(Labels.DisplayOrder)]
        public int DisplayOrder { get; set; }

        public IList<EmployeeLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName(Labels.SelectedCustomerRoleIds)]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName(Labels.SelectedStoreIds)]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableEmployees { get; set; }

        //discounts
        [NopResourceDisplayName(Labels.SelectedDiscountIds)]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }

    public partial record EmployeeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName(EmployeeModel.Labels.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(EmployeeModel.Labels.Description)]
        public string Description { get; set; }

        [NopResourceDisplayName(EmployeeModel.Labels.SeName)]
        public string SeName { get; set; }
    }
}