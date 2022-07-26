using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource
{
    /// <summary>
    /// HumanResource settings
    /// </summary>
    public class EmployeeSettings : ISettings
    {
        public EmployeeSettings()
        {
        }

        /// <summary>
        /// Gets or sets a default view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether employee breadcrumb is enabled
        /// </summary>
        public bool EmployeeBreadcrumbEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether product 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size on the search products page
        /// </summary>
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options on the search products page
        /// </summary>
        public string SearchPagePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should process attribute change using AJAX. It's used for dynamical attribute change, SKU/GTIN update of combinations, conditional attributes
        /// </summary>
        public bool AjaxProcessAttributeChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Employee page size options (for new employees)
        /// </summary>
        public string DefaultEmployeePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Employee page size (for new employees)
        /// </summary>
        public int DefaultEmployeePageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool ExportImportUseDropdownlistsForAssociatedEntities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the employees need to be exported/imported using name of employee
        /// </summary>
        public bool ExportImportEmployeesUsingEmployeeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the products should be exported/imported with a full employee name including names of all its parents
        /// </summary>
        public bool ExportImportProductEmployeeBreadcrumb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the images can be downloaded from remote server
        /// </summary>
        public bool ExportImportAllowDownloadImages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the related entities need to be exported/imported using name
        /// </summary>
        public bool ExportImportRelatedEntitiesByName { get; set; }

        /// <summary>
        /// Gets or sets count of displayed years for datepicker
        /// </summary>
        public int CountDisplayedYearsDatePicker { get; set; }

        /// <summary>
        /// Get or set a value indicating whether it's necessary to show the date for pre-order availability in a public store
        /// </summary>
        public bool DisplayDatePreOrderAvailability { get; set; }

        /// <summary>
        /// Get or set a value indicating whether use standart menu in public store or use Ajax to load menu
        /// </summary>
        public bool UseAjaxLoadMenu { get; set; }

        /// <summary>
        /// Get or set a value indicating whether the specification attribute filtering is enabled on hr pages
        /// </summary>
        public bool EnableSpecificationAttributeFiltering { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customer can search with employee name
        /// </summary>
        public bool AllowCustomersToSearchWithEmployeeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all pictures will be displayed on hr pages
        /// </summary>
        public bool DisplayAllPicturesOnHumanResourcePages { get; set; }

    }
}