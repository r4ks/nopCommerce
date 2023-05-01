using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.HumanResource.Areas.Admin.Models.HumanResource
{
    /// <summary>
    /// Represents a employee model
    /// </summary>
    public partial record EmployeeModel : BaseNopEntityModel, IAclSupportedModel,
        ILocalizedModel<EmployeeLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Views file path
        public const string CREATE_VIEW = "~/Plugins/Nop.Plugin.Widgets.HumanResource/Areas/Admin/Views/Employee/Create.cshtml";
        public const string CREATE_OR_UPDATE_VIEW = "~/Plugins/Nop.Plugin.Widgets.HumanResource/Areas/Admin/Views/Employee/_CreateOrUpdate.cshtml";
        public const string EDIT_VIEW = "~/Plugins/Nop.Plugin.Widgets.HumanResource/Areas/Admin/Views/Employee/Edit.cshtml";
        #endregion

        #region Ctor

        public EmployeeModel()
        {
            if (PageSize < 1)
                PageSize = 5;

            Locales = new List<EmployeeLocalizedModel>();
            AvailableEmployees = new List<SelectListItem>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

        }

        #endregion
        #region Labels
        public static partial class Labels
        {
            public const string Name = "Admin.HumanResource.Employees.Fields.Name";
            public const string Description = "Admin.HumanResource.Employees.Fields.Description";
            public const string SeName = "Admin.HumanResource.Employees.Fields.SeName";
            public const string ParentEmployeeId = "Admin.HumanResource.Employees.Fields.Parent";
            public const string PictureId = "Admin.HumanResource.Employees.Fields.Picture";
            public const string PageSize = "Admin.HumanResource.Employees.Fields.PageSize";
            public const string AllowCustomersToSelectPageSize = "Admin.HumanResource.Employees.Fields.AllowCustomersToSelectPageSize";
            public const string PageSizeOptions = "Admin.HumanResource.Employees.Fields.PageSizeOptions";
            public const string ShowOnHomepage = "Admin.HumanResource.Employees.Fields.ShowOnHomepage";
            public const string IncludeInTopMenu = "Admin.HumanResource.Employees.Fields.IncludeInTopMenu";
            public const string Published = "Admin.HumanResource.Employees.Fields.Published";
            public const string Deleted = "Admin.HumanResource.Employees.Fields.Deleted";
            public const string DisplayOrder = "Admin.HumanResource.Employees.Fields.DisplayOrder";
            public const string SelectedCustomerRoleIds = "Admin.HumanResource.Employees.Fields.AclCustomerRoles";
            public const string SelectedStoreIds = "Admin.HumanResource.Employees.Fields.LimitedToStores";
            public const string None = "Admin.HumanResource.Employees.Fields.Parent.None";

            //View Labels:
            public const string Title = "Admin.HumanResource.Employees";
            public const string EditEmployeeDetails = "Admin.HumanResource.Employees.EditEmployeeDetails";
            public const string BackToList = "Admin.HumanResource.Employees.BackToList";
            public const string AddNew = "Admin.HumanResource.Employees.AddNew";
            public const string Info = "Admin.HumanResource.Employees.Info";
            public const string Display = "Admin.HumanResource.Employees.Display";
            public const string Mappings = "Admin.HumanResource.Employees.Mappings";
            public const string Products = "Admin.HumanResource.Employees.Products";

            // Events
            public const string AddedEvent = "Admin.HumanResource.Employees.Added";
            public const string UpdatedEvent = "Admin.HumanResource.Employees.Updated";
            public const string DeletedEvent = "Admin.HumanResource.Employees.Deleted";
            public const string ImportedEvent = "Admin.HumanResource.Employees.Imported";

            // Notifications
            public const string LogAddNewEmployee = "ActivityLog.AddNewEmployee";
            public const string LogEditEmployee = "ActivityLog.EditEmployee";
            public const string LogDeleteEmployee = "ActivityLog.DeleteEmployee";

            // Don't override this please
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