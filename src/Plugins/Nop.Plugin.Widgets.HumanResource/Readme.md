# Basic NopCommerce plugin

## Pluging Project Organization
```
├───Areas
│   └───Admin
│       ├───Controllers
│       ├───Factories
│       ├───Models
│       │   ├───HumanResource
│       │   └───Settings
│       ├───Validators
│       │   └───HumanResource
│       └───Views
│           ├───Employee
│           └───Setting
├───Caching
├───Common
├───Components
├───Domains
│   └───HumanResource
├───Factories
├───Infrastructure
│   ├───Cache
│   └───Mapper
├───Installation
├───Mapping
│   └───Builders
├───Migrations
├───Services
│   ├───ExportImport
│   │   └───Help
│   ├───HumanResource
│   └───Security
├───Validators
└───Views
```
Basically is recommended to put all Adminitrative related things inside Areas/Admin with the exception of Domains, Migrations, Infrastructure, Mapping and Services.
All public related things you can put inside the normal Views, Controllers, Factories, Components, Validators ... folders on Plugins Root.

## Plugin Main File
Normally it's named as the Last part of the plugins project name.
There you can find the Install, Uninstall, Update methods that will be executed when the user install, uninstall or change the version.
You can find the localized strings install method, the widgets declaration and there configuration.
The Settings url getter is declared at the same file.

## Views
Create your views inside Areas/Admin/Views/{GroupName}/??????.cshtml.
Public Views can be saved inside the root folder Views if you want.
- Certify that the view file has properties set to Content and Always copy!
  Or check if it's included to be copied inside the projects .csproj file.
- Don't forget to point to the right Model if there is one!

## Models
Areas/Admin/Models is the common place to save the Views related Models
*Always decorate each property inside the Model with [NopResourceDisplayName( "put.your.special.localized.string.here")]
*Thre is a inner class named Labels witch holds the localized strings but you can put your localized strings whatever place you want, it's only for re-use purporse, to avoid misstyping, and get compilators checking for free!

## Controller
- Generally you return the View when the method is GET and return a json on the same action with POST method that represents the view component.


Many services are called on Installation process when user is redirected into the "HomePage",
  Migrations occur, routes are registered, language are set, admin accoung, etc...
  look Presentation > Nop.Web > Controllers > InstallController

## The presention model
-Look for files present at:
  \Presentation\Nop.Web\Areas\Admin\Models\...
  Assembly: Nop.Admin
  Solution Location: Nop.Web.Areas.Admin.Models.????
  *** Models are classes that implements BaseNopEntityModel at least.

-Presentation Models can have Validators which are present at:
  \Presentation\Nop.Web\Areas\Admin\Validators\...
  Assembly: Nop.Web
  Solution Location: Nop.Web.Areas.Admin.Validators.???
  *** Validators are classes that implements BaseNopValidators<Model> with generic type of the
  Model of the Presentation.

## if context injection fails
  set this to true on the prj file
  <CopyRefAssembliesToPublishDirectory>false</CopyRefAssembliesToPublishDirectory>

## The View
-Look for template files at:
  \Presentation\Nop.Web\Areas\Admin\Views\...cshtml
  Assembly: Nop.Web

## Permissions
- create a class that implements IPermissionProvider and add the record permissions:
- register the permission class like at the code bellow:
    //register default permissions
    var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
    foreach (var providerType in permissionProviders)
    {
        var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
        await _permissionService.Value.InstallPermissionsAsync(provider);
    }

## The Controller
The controller is responsible for mapping the domain data modelto our view model and vice versa.
\Presentation\Nop.Web\Areas\Admin\Controllers\???.cs
Assembly: Nop.Admin
Solution Location: Nop.Web.Areas.Admin.Controllers.???.cs

## DI
Libraries > Nop.Core > Infrastructure > INopStartup.cs
- Create a class that implements INopStartup interface to inject whatever you want, and register them!

## Routes
- Create a class that implements IRouteProvider and implement the ReisterRoutes() method.
routes are registered:
NopRoutingStartup defines UseNopEndpoints() that is called by NopEndpoints which is called by NopEngine.ConfigureServices() which
is instantiated on EngineContext.Create(); EngineContext hold the single instance of application where you can get the
IRepository, IShoppingCartService, ISettingService, IPictureService, IForumService, ILogger, ILanguageService, ...

Presentation > Nop.Web.Framework > Infrastructure > NopStartup.cs adds all services into Ioc Container.
Some Services call the EngineContext that you can get the single instance of desired other Service.

Presentation > Nop.Web.Framework > Mvc > Routing > RoutePublisher.RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder) calls
  Presentation > Nop.Web.Framework > Mvc > Routing > IRouteProvider.RegisterRoutes(IEndpointRouteBuilder endpoitRouteBuilder)

  *You can override nopCommerce default routes using Plugins routes if you want!
    - Only register the new route again with the same name but w/ different controller and action inside the RegisterRoutes(...)!
    *Don't forget to set Priority to int.MaxValue to called first!

## Events
Libraries > Nop.Data > EntityRepository inject IEventPublisher to call the desired event too.
Libraries > Nop.Services > ??? > ???Service.cs will inject EventPlublisherExtension and call the desired event.
Libraries > Nop.Core > Events > EventPlublisherExctension has:
  - EntityInsertedAsync()
  - EntityUpdatedAsync()
  - EntityDeletedAsync()
  that call PublishAsync bellow:
    Libraries > Nop.Services > Events > EventPlublisher.PublishAsync(event) calls
      Libraries > Nop.Services > Events > IConsumer.HandleEventAsync(eventMessage)
  *Event Consumer
    -create a class that implements IConsumer<EntityInsertedEvent<YourEntity>> and implement
      HandleEventAsync(EntityInsertedEvent<YourEntity> eventMessage)
      *Each HandleEventAsync paramerters will be related to the IConsumer<Entity?????Async<*****>>

// Where the Map Builder is scanning!
Libraries > Nop.Data > Extensions > FluentMigratorExtensions.cs
  -> public static void RetrieveTableExpressions()
  // it looks for classes that implements NopEntityBuilder

  -> public static void TableFor<TEntity>(this ICreateExpressionRoot expressionRoot) where TEntity : BaseEntity
  // will excecute the creation of tables.

  // look Libraries > Nop.Data > Mapping > Builders > ...
  // Models are located at Libraries > Nop.Core > Domain > ...
  // Normally the Model and the MapBuilder have a similar name but different suffix.

  TableFor() => RetrieveTableExpressions()


  // You have to add manually the migrations?
  // inside Nop.Data.Migrations.???.???


  // The procedure for updating migrations is carried out during the
  // initialization of the database at:
  // Libraries > Nop.Data > DataProviders > BaseDataProvider.cs defines InitializeDatabase()
  //   and call ApplyUpMigrations(typeof(NopDbStartup).Assembly);
  //
  // obs.:
  // Presentation > Nop.Web > Controllers > InstallContollers.cs call InitializeDatabase()
  // which is fired when users come to the main page (the route 'Homepage');
  // This controller only fires if there is connectionString defined which is checked by
  // Libraries > Nop.Data > DataSettingsManager.cs -> IsDatabaseInstalled()

## Entities
  - Create a class that implements NopEntityBuilder<Model> and Configure the Entity
    properties implementing the method MapEntity().
  NopDbStartup seeks for all classes that implements MigrationBase and excecutes them all.

  // Libraries > Nop.Data > Migrations > Installation > SchemaMigration.cs
  // will fire all migrations; classes that implements BaseEntity
  // The class SchemaMigration that implements AutoReversingMigration itself
  // is need FluentMigration configuration service will look for that!
  *When decorating the migration class with NopMigration give a validy DateTime or you will get an error!

  // FluentMigration configuration are services that implement INopStartup
  Libraries > Nop.Core > Infrastructure > NopEngine.cs
  -> public void ConfigureServices() // seek for classes that implements INopStartup

  // Migrations
  // create a class that extends AutoReversinMigration
  // Decorate with NopMigration("2022-07-03 16:33:44", "Nop.Plugin.Group.NamePlugin 1.77. Description", MigrationProcessType.Update)
  // *the NopMigration accept a valid datetime as first argument! Don't forget the zero for day or month!

  // Builders are helper classes that configure the entities by mapping the right properties.
  // Nop.Data.Mapping.Builders.????.?????Builder.cs

  *For Plugins:
    -Generaly Entities are saved on PluginFolder/Domain
    -Migrations are saved at PluginFolder/Data/SchemaMigration.cs a class that extends AutoReversingMigration class.

## Models
    * Is the view model. Generaly extends BaseNopModel record.
    - Usualy located inside Models folders
    - Use [NopResourceDisplayName("unique.string.for.localized.text")] decorator on top of the property.

## Model Factories
  look at Nop.Web/Areas/Admin/Factories/??????Factory
  Factories will help get data from database -> EntityModel(a DTO)  -> Controller -> View Components!

## Services
  -Generaly all entities data are served through a service that injects the IRepository<SomeEntity>!
  // look at: Nop.Services.?????.?????Service.cs

  *you can override services too:
    - create the class that extends the method to override and mark the method with "override"
    - certify that that the methods you are declaring are always virtual!
    *Don't forget to register it on the IOC Container with the same Interface but with the override class!

## Filtering
  - make the desired Model implements BaseSearchModel
    BaseSearchModel contains properties like Page, PageSize, Start, Length, ...

## Filters
  - process the filter before the controller action is executed.
  A class that extends IFilterProvider and ActionFilterAttribute
  override the methods like OnActionExecuting(ActionExecutingContext filterContext)
    -ActionExecutingContext has ActionDescriptor property that has the ActionName property witch you can use to do something...
    -ActionExecutingContext has Controller property witch holds the controller name intercepted.
    - You can access the session on ActionExecutingContext.HttpContext.Session which is a dictionary.
  *To Use a filter you have to register it on NopStartup:
    -let's say you defined a filter called AutoAddRolesFilterAttribute them configure mvc with:
      IServiceCollection.AddMvc(options => options.Filters.Add(typeof(AutoAddRolesFilterAttribute)));
    -it is good to register it on IOC too with AddScoped<AutoAddRolesFilterAttribute>();
  *Don't forget to specify it on dependency register inside Register method on the class that implements IDependencyRegistrar
    - builder.RegisterType<AutoAddRolesFilterAttribute>().As<IFilterProvider>().InstancePerLifetimeScope();

## Add Settings into a plugin
  -> create a class that extends ISettings
  -> On Controller; inject that class you created and the ISettingService.
  -> set your configurations and call _settingService.SaveSettingAsync(_contentSettings);
  -> implement GetConfigurationPageurl()
  *set an correct route that GetConfigurationPageurl() is returning! a wrong url will make it not show the configuration btn.

  -to add plugin into Admin Menu:
    - implement the ManageSiteMap(SiteMapNode rootNode) method
    - add the menu item into the rootNode:
      var menuItem = new SiteMapNode() { SystemName = ..., Title = ..., Url = ..., Visible = true, IconClass = "fa fa-dot-circle-o", }
      var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins")
      if(pluginNode != null) pluginNode.ChildNodes.Add(menuItem); else rootNode.ChildNodes.Add(menuItem)
    - set HideInWidgetList => false to get visible at /Admin/Widget/List

## Views
  *You can override a view file:
    - Define a class that extends IViewLocationExpander, implement the method ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
      and return the list of view files list concatenated with the list of new view files.
    - In NopStartup class at ConfigureServices(IServiceCollection services, IConfiguration configuration)
      add a view expander
      services.Configura<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()));

## Template Generator from nopCommerce for VisualStudio
  *Don't forget to update the Plugin.json "SupportedVersions"
  *Check The naming convention of the plugin, e.g.: Group.NamePlugin
  *Don't forget to override the "BasePlugin" methods like UninstallAsync(), UpdateAsync(), InstallAsync()...

## Admin
  Usualy is located at src/Presentation/Nop.Web/Areas/Admin/
  * Home Page
  > src\Presentation\Nop.Web\Areas\Admin\Views\Home\Index.cshtml
  * Side Menu
   > src\Presentation\Nop.Web\Areas\Admin\Views\Shared\Menu.cshtml
   -Plugins should implement IAdminMenuPlugin interface to have the right to be shown at Admin side menu.

## Urls and Routes
   - You can use the urlHelperfactory.GetUrlHelper(_actionContextAccessor.actionContext).Routeurl("some.user.defined.route")
    to get the right route.
  - Don't forget to define it on RouteProvider.RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder) with
    same name("some.user.defined.route") a valid pattern, right controller and the correct action inside MapControllerRoute().

## Public Site
  uses folders like:
    src/Presentation/Nop.Web/
      - Components (*are small sections)
      - Controllers (getter actions usualy pass the view and the view get the data from an action with same name but post method and some extra parameters.)
      - Extensions
      - Factories
      - Infrastructure
      - Logs
      - Models
      - obj
      - Themes
      - Validators
      - Views

## Localization
   - src/Presentation/Nop.Web/

## Controller -> ViewComponent + Model
   *Always create a get and a post for each ViewComponent!
   *Don't forget to add a name to the view if you want to locate by name! use the decorator [ViewComponent(Name = "????????")]
   -View Components have to implement NopViewComponent and implement Invoke() that returns a View object that point to a view file(.cshtml file).
   e.g.: For a Configure View there is Configure() and [HttpPost] Configure(ConfigurationModel model).

## CustomerRoles
   - PermissionRecord can be installed or uninstalled;
   - Each CustomerRole can have many PermissionRecord.

## NopResourceDisplayName
   - takes a string that represets the localized string on the model to be displayed at the view component!
   you can access using the property "resourceKey"
   *Best practices: always decorate the Models properties with NopResourceDisplayName to reduce code on the view!
   - tag helpers will use the Display Name!

## AutoMapper
   Nop.Core.Infrastructure.NopEngine declares AddAutoMapper() method that scan for classes that
    implements IOrderedMapperProfile and Create Instances and Activate the mapped configs on that classes.
  - You only have to create a class that inherits from Profile and IOrderedMapperProfile and implement what is necessary.

## Tips
  - Always check at the Plugins deploy folder: "src\Presentation\Nop.Web\Plugins\????????"
  - check if the assets are there! sometime when you move files inside your project, the
    properties aren't keep and them not copyed on the folder above.
    Certify that your views was copied correctly on views deploy folder above!

  ### Error on install: "Setup failed: Sequence contains more than one element"
  copy the old project file src/Presentation/Nop.Web/App_Data/appsettings.json to the new project at the same place.
  *you can find a similar instruction at a Readme.txt on any upgrade folder at nopCommerce folder.

  ### Add Project into Solution
  Use the following command to add the Plugin project into the nopCommerce solution:
  ```
  > dotnet sln add .\Plugins\Nop.Plugin.Widgets.HumanResource
  ```

  ### Connection String as Environment Variables
  If you wish to not edit your appsettings.json when working with many friends you can export the following environment variable other than change everytime some other friend changes it!
  ```
  ConnectionStrings__ConnectionString="valid connection string"
  ```
  *But beware that this environment variable can trigger the recreation of appsettings.json automatically when nopCommerce starts and can't find any appsettings.json.

  ### Running Configurations are stored at:
    - Presentation/Nop.Web/App_Data
      - plugins.json
      - appsettings.json
      - Localization/*
    *Sometimes are good to backup the above files, that will guarantee you to skip the install process when executing a clean nopCommerce project and you wish to reuse the old database, localized strings, and installed plugins status.