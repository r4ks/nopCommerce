using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.HumanResource.Core.Domains.HumanResource;
using Nop.Services.Configuration;
using Nop.Services.Installation;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Widgets.HumanResource.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class ExtraInstallationService : IExtraInstallationService
    {
        #region Fields

        private readonly INopDataProvider _dataProvider;
        private readonly INopFileProvider _fileProvider;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;

        #endregion

        #region Ctor

        public ExtraInstallationService(INopDataProvider dataProvider,
            INopFileProvider fileProvider,
            IRepository<Employee> employeeRepository,
            IRepository<UrlRecord> urlRecordRepository
            )
        {
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _employeeRepository = employeeRepository;
            _urlRecordRepository = urlRecordRepository;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            return await _dataProvider.InsertEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : BaseEntity
        {
            await _dataProvider.BulkInsertEntitiesAsync(entities);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            await InsertInstallationDataAsync(entities.ToArray());
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            foreach (var entity in entities)
                await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
        {
            //duplicate of ValidateSeName method of \Nop.Services\Seo\UrlRecordService.cs (we cannot inject it here)
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            seName = seName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in seName.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }

            seName = sb.ToString();
            seName = seName.Replace(" ", "-");
            while (seName.Contains("--"))
                seName = seName.Replace("--", "-");
            while (seName.Contains("__"))
                seName = seName.Replace("__", "_");

            //max length
            seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

            //ensure this seName is not reserved yet
            var i = 2;
            var tempSeName = seName;
            while (true)
            {
                //check whether such slug already exists (and that is not the current entity)

                var query = from ur in _urlRecordRepository.Table
                            where tempSeName != null && ur.Slug == tempSeName
                            select ur;
                var urlRecord = await query.FirstOrDefaultAsync();

                var entityName = entity.GetType().Name;
                var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
                if (!reserved)
                    break;

                tempSeName = $"{seName}-{i}";
                i++;
            }

            seName = tempSeName;

            return seName;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
        }


        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InstallSettingsAsync(RegionInfo regionInfo)
        {
            var isMetric = regionInfo?.IsMetric ?? false;
            var country = regionInfo?.TwoLetterISORegionName ?? string.Empty;
            var isGermany = country == "DE";
            var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            await settingService.SaveSettingAsync(new EmployeeSettings
            {
                DefaultViewMode = "grid",
                EmployeeBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode =
                    "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                AjaxProcessAttributeChange = true,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                DefaultEmployeePageSizeOptions = "6, 3, 9",
                DefaultEmployeePageSize = 6,
                ExportImportUseDropdownlistsForAssociatedEntities = true,
                ExportImportRelatedEntitiesByName = true,
                CountDisplayedYearsDatePicker = 1,
                UseAjaxLoadMenu = false,
                EnableSpecificationAttributeFiltering = true,
                AllowCustomersToSearchWithEmployeeName = true,
                DisplayAllPicturesOnHumanResourcePages = false,
            });


            await settingService.SaveSettingAsync(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
            });

        }
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InstallEmployeesAsync()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();


            //employees
            var allEmployees = new List<Employee>();
            var employeeComputers = new Employee
            {
                Name = "Computers",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_computers.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Computers"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeComputers);

            await InsertInstallationDataAsync(employeeComputers);

            var employeeDesktops = new Employee
            {
                Name = "Desktops",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_desktops.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Desktops"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeDesktops);

            await InsertInstallationDataAsync(employeeDesktops);

            var employeeNotebooks = new Employee
            {
                Name = "Notebooks",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_notebooks.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Notebooks"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeNotebooks);

            await InsertInstallationDataAsync(employeeNotebooks);

            var employeeSoftware = new Employee
            {
                Name = "Software",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_software.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Software"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeSoftware);

            await InsertInstallationDataAsync(employeeSoftware);

            var employeeElectronics = new Employee
            {
                Name = "Electronics",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_electronics.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Electronics"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeElectronics);

            await InsertInstallationDataAsync(employeeElectronics);

            var employeeCameraPhoto = new Employee
            {
                Name = "Camera & photo",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_camera_photo.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Camera, photo"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeCameraPhoto);

            await InsertInstallationDataAsync(employeeCameraPhoto);

            var employeeCellPhones = new Employee
            {
                Name = "Cell phones",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_cell_phones.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Cell phones"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeCellPhones);

            await InsertInstallationDataAsync(employeeCellPhones);

            var employeeOthers = new Employee
            {
                Name = "Others",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Accessories"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeOthers);

            await InsertInstallationDataAsync(employeeOthers);

            var employeeApparel = new Employee
            {
                Name = "Apparel",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_apparel.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Apparel"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeApparel);

            await InsertInstallationDataAsync(employeeApparel);

            var employeeShoes = new Employee
            {
                Name = "Shoes",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_shoes.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Shoes"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeShoes);

            await InsertInstallationDataAsync(employeeShoes);

            var employeeClothing = new Employee
            {
                Name = "Clothing",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_clothing.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Clothing"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeClothing);

            await InsertInstallationDataAsync(employeeClothing);

            var employeeAccessories = new Employee
            {
                Name = "Accessories",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentEmployeeId = employeeApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_apparel_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Apparel Accessories"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeAccessories);

            await InsertInstallationDataAsync(employeeAccessories);

            var employeeDigitalDownloads = new Employee
            {
                Name = "Digital downloads",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_digital_downloads.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Digital downloads"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeDigitalDownloads);

            await InsertInstallationDataAsync(employeeDigitalDownloads);

            var employeeBooks = new Employee
            {
                Name = "Books",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_book.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Book"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeBooks);

            await InsertInstallationDataAsync(employeeBooks);

            var employeeJewelry = new Employee
            {
                Name = "Jewelry",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_jewelry.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Jewelry"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeJewelry);

            await InsertInstallationDataAsync(employeeJewelry);

            var employeeGiftCards = new Employee
            {
                Name = "Gift Cards",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "employee_gift_cards.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Gift Cards"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allEmployees.Add(employeeGiftCards);

            await InsertInstallationDataAsync(employeeGiftCards);

            //search engine names
            foreach (var employee in allEmployees)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = employee.Id,
                    EntityName = nameof(Employee),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(employee, employee.Name)
                });
        }


        #endregion

        #region Methods

        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallRequiredDataAsync(RegionInfo regionInfo, CultureInfo cultureInfo)
        {
            await InstallSettingsAsync(regionInfo);
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallSampleDataAsync()
        {
            await InstallEmployeesAsync();

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            // TODO: delete this
            await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = false,
                DisplayNewProductsMenuItem = false,
                DisplayProductSearchMenuItem = false,
                DisplayCustomerInfoMenuItem = false,
                DisplayBlogMenuItem = false,
                DisplayForumsMenuItem = false,
                DisplayContactUsMenuItem = false
            });
        }

        #endregion
    }
}

