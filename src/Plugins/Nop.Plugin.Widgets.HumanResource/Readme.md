# Basic NopCommerce plugin

## Pluging Project Organization
```
├───Core
│   ├───Caching
│   └───Domains
│       └───HumanResource
├───Data
│   ├───Mapping
│   │   └───Builders
│   │       └───HumanResource
│   └───Migrations
├───Services
│   ├───Common
│   ├───ExportImport
│   │   └───Help
│   ├───HumanResource
│   ├───Installation
│   └───Security
├───Web
│   ├───Areas
│   │   └───Admin
│   │       ├───Controllers
│   │       │   ├───HumanResource
│   │       │   └───Settings
│   │       ├───Factories
│   │       ├───Models
│   │       │   ├───HumanResource
│   │       │   └───Settings
│   │       ├───Validators
│   │       │   └───HumanResource
│   │       └───Views
│   │           ├───Employee
│   │           └───Setting
│   ├───Components
│   ├───Content
│   ├───Infrastructure
│   │   ├───Cache
│   │   └───Mapper
│   ├───Validators
│   └───Views
└───Web.Framework
    └───Factories
```
Everything resembles the nopCommerce project inside the plugin here, you will find everything in place as in the nopCommerce.
You Are free to rename this HumanResource Namespace and Employee table to whatever you want.

## Plugin Main File
Normally it's named as the Last part of the plugins project name. Sometimes this file is suffixed with Plugin on the name you are free to choose one that make you happy.
This file contains a class that implements BasePlugin, IWidgetPlugin and IAdminMenuPlugin.
There you can find the Install, Uninstall, Update methods that will be executed when the user install, uninstall or change the version.
IAdminMenuPlugin interface implementors are have the right to add menu items on the admin side menu, look the method ManageSiteMapAsync(SiteMapNode rootNode).
You can find the localized strings install method InstallLocalizedStringsAsync().

GetConfigurationPageUrl() is in charge of returning a valid url that go to the Settings View.

GetWidgetZonesAsync() is where you give the WidgetZone where you want to show.

GetWidgetViewComponent() should return the class that implements NopViewComponent and implement Invoke() that should return a valid View.

InstallPermissionsAsync() method install a key into the permission system on the nopCommerce that you can use to check if after when a user is accessing the controller or something else...
### plugin.json
Certify that this file exists, and check the "SupportedVersions" property that that should be like the running nopCommerce version.

## Views
Create your views inside Areas/Admin/Views/{GroupName}/??????.cshtml.
Public Views can be saved inside the root folder Views if you want.
- Certify that the view file has properties set to Content and Always copy!
  Or check if it's included to be copied inside the projects .csproj file.
- Don't forget to point to the right Model if there is one with @model YourModel.

## Models
Models are classes that implements BaseNopEntityModel.

Areas/Admin/Models is the common place to save the Views related Models
*Always decorate each property inside the Model with [NopResourceDisplayName( "put.your.special.localized.string.here")]
*Thre is a inner class named Labels witch holds the localized strings but you can put your localized strings whatever place you want, it's only for re-use purporse, to avoid misstyping, and get compilators checking for free!

## Validators
Models can have Validator, you have to create a class that implements BaseNopValidator<YourModel>
and add the RuleFor for the properties you want or for the props that you made mandatory on your domain.

## Controller
- Generally you return the View when the method is GET and return a json on the same action with POST method that represents the view component.
Normally everybody inject a Service on the controller, that Service will get data from an Repository and return the data to be returned as json or in the case when returning a View from controller everybody injects a ModelFactory class that will be responsible of filling the ModelClass used on the View from the data of a Service that takes the data from the repository of a Domain, them this ModelClass instance will be returned together with the View address on a new View() instance.


## Permissions
Permissions are like keys that are registered on the system and you can install or uninstall.
to install a new permission, inject the PermissionService and call:
  await _permissionService.InstallPermissionsAsync(provider);
and give the instance of IPermission implementor.
To create a Permission:
- create a class that implements IPermissionProvider and add a public static record readonly permissions.
- register the permission class like at the code bellow:
```
    //register default permissions
    var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };

    foreach (var providerType in permissionProviders)
    {
        var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
        await _permissionService.Value.InstallPermissionsAsync(provider);
    }
```
To validate a Permission:
  - inject the PermissionService on your controller
  - call _permissionService.AuthorizeAsync() and pass the permission static record permission that you want to check where before some operations.

## Dependency Injection
 nopCommerce will seek for all classes that implements INopStartup interface, inside ConfigureServices(IServiceCollection services, IConfiguration configuration) you can register the classes you need to be injected.
 Configure(IApplicationBuilder application) method give you the ability to configure whatever you want. For example you want to change mvc behaviors or change a option from razor template system.
 Don't forget to set the Order property that decides witch configuration has more priority.

## Routes
Normally all routes will be created accordling to the Views directory structure but you can give a extra reinforcement on the patterns and the key routes on the IRouteProvider implementor. 
The framework will seek for any class that implements IRouteProvider and call RegisterRoutes() method on the boostrap process.
Routes are registered calling the MapControllerRoute() method.
  *You can override nopCommerce default routes using Plugins routes if you want!
    - Only register the new route again with the same name but w/ different controller and action inside the RegisterRoutes(...)!
    *Don't forget to set Priority to int.MaxValue to called first!

## Events
nopCommerce has a event system that you can use to do some side effects. You can consume, publish or listenen for a specific event.
To consule create a class that implemnts IConsumer<Entity?????Event<YourDomain>>. The ?????? will be the Operation you wish to consume like Inserted, Updated or Deleted.
You have to implement all HandleEventAsync for each Entity????Event you want to consume.
To publish inject IEventPublisher and call PublishAsync() givin the event as his parameter.

## Domains
All you have to do is create a class that extends BaseEntity at least, declare the properties and them register it on the migration file.

## Migration
To do a Migration create a class that extends AutoReversingMigration, decorate the class with [NopMigration("valid date time", "write a description")] and override Up() or Down().
Inside Up() don't forget to declare Create.TableFor<YourDomain>().
*the NopMigration accept a valid datetime as first argument! Don't forget the zero for day or month!


## Entity Configuration or Domain Mapping
You can do the mapping extending NopEntityBuilder<YourDomain> and overriding the method MapEntity() where you declare the properties of the properties of your domain entity.
-Generaly Entities are saved on PluginFolder/Domain
-Migrations are saved at PluginFolder/Data/SchemaMigration.cs a class that extends AutoReversingMigration class.

## Model Factories
  look for files with similar names like Areas/Admin/Factories/??????Factory
  Factories will help you get data from service -> database -> EntityModel(a DTO)  -> Controller -> View Components!

## Services
  -Generaly all entities data are served through a service that injects the IRepository<SomeEntity>!
  // look at: Services/?????/?????Service.cs

  *you can override services too:
    - create the class that extends the method to override and mark the method with "override"
    - certify that that the methods you are declaring are always virtual!
    *Don't forget to register it on the IOC Container with the same Interface but with the override class!

## Filtering
  Filtering by standard at nopCommerce use as a parameter of a method that excecute queries a class model that implements BaseSearchModel. You can add whatever field that represents the property you are intenting to do a search for.
    BaseSearchModel contains properties like Page, PageSize, Start, Length, ... to help on the filtering, and promoting code reuse.

## Running Configurations are stored at:
  - Presentation/Nop.Web/App_Data
    - plugins.json
    - appsettings.json
    - Localization/*
  *Sometimes it's good to backup the above files, that will guarantee you to skip the install process when executing a clean nopCommerce project and maybe you wish to reuse the old database, localized strings, and installed plugins status.