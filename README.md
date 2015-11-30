# Dehydrator

Dehydrator helps you combine ORMs like [Entity Framework](https://msdn.microsoft.com/data/ef.aspx) with REST service frameworks like [WebAPI](http://www.asp.net/web-api).

By stripping navigational references in your entities down to only their IDs their serialized representation no longer contains redundant or cyclic data. When new or modified deserialized data needs to be persisted Dehydrator takes care of resolving the reference IDs again.

NuGet packages:
* [Dehydrator](https://www.nuget.org/packages/Dehydrator/)
* [Dehydrator.Core](https://www.nuget.org/packages/Dehydrator.Core/)
* [Dehydrator.EntityFramework](https://www.nuget.org/packages/Dehydrator.EntityFramework/)
* [Dehydrator.EntityFramework.Unity](https://www.nuget.org/packages/Dehydrator.EntityFramework.Unity/)
* [Dehydrator.WebApi](https://www.nuget.org/packages/Dehydrator.WebApi/)

While Dehydrator is designed primarily for use with Entity Framework and WebAPI you can also use the core package (`Dehydrator`) with any other ORM and REST framework. You'll just need to implement the parts from the other packages yourself, which should be pretty straightforward.


## Usecase sample

We'll use this simple POCO (Plain old CLR object) class modelling software packages and their dependencies as an example:
```cs
class Package : IEntity
{
  public long Id { get; set; }
  public string Name { get; set; }

  [Dehydrate]
  public virtual ICollection<Package> Dependencies { get; set; }
}
```

Without Dehydrator your REST service might return something like this for `GET /packages/1`:
```javascript
{
  "Id": 1,
  "Name": "AwesomeApp",
  "Dependencies":
  [
    {
      "Id": 2,
      "Name": "AwesomeLib",
      "Dependencies": []
    }
  ]
}
```
The entire `AwesomeLib` reference is inlined. This duplicates content from `/packages/2` and complicates `POST` and `PUT` calls. Are references expected to be existing entities? Are they created/modified together with the parent entity if no matching entity exists yet?

With Dehydrator the same entity would be serialized as:
```javascript
{
  "Id": 1,
  "Name": "AwesomeApp",
  "Dependencies":
  [
    {"Id": 2}
  ]
}
```
This removes any potential duplication and ambiguity.


## Getting started

### Data model
Install the `Dehydrator.Core` NuGet package in the project holding your data model. Make all your entity classes either implement `IEntity` or derive from `Entity`. Mark any reference properties you wish to have dehydrated with `[Dehydrate]`. If you want to have a property resolved but not dehydrated (e.g., incoming data is dehydrated and needs to be resolved but outgoing data should not be dehydrated) use `[Resolve]` instead. Combining `[Dehydrate]` and `[Resolve]` is not necessary since `[Dehydrate]` implies `[Resolve]`. To embed an Entity type within another and have the dehydration only start within the inner object annotate the containing property with `[DehydrateReferences]` or `[ResolveReferences]`.

Install the `Dehydrator` NuGet package in the project performing the data serialization (usually the frontend/webservice). You can now use the `.DehydrateReferences()` extension method to dehydrate references in entities down to only their IDs and  and `.ResolveReferences()` to resolve/restore them again.

Resolving requires an `IRepositoryFactory`, which represents a storage backend such as a database and provides `IRepository<>` instances for specific entity types.


### Entity Framework
Install the `Dehydrator.EntityFramework` NuGet package in the project you use for database access via Entity Framework. This provides `DbRepositoryFactory`, an `IRepositoryFactory` implementation based on Entity Framework's `DbSet`. Make sure to make any reference/navigational properties `virtual` to enable Entity Framework's lazy loading feature.

Use the `DehydratingRepositoryFactory` decorator provided by the `Dehydrator` NuGet package to wrap your `IRepositoryFactory` implementation (`DbRepositoryFactory` or a custom implementation). This decorator transparently dehydrates and resolve references when loading and saving entities.


### Unity
If you wish to use the [Unity Application Block](https://unity.codeplex.com/) for dependency injection in your Entity Framework application install the `Dehydrator.EntityFramework.Unity` NuGet package. This adds extension methods to the `IUnityContainer` type for configuring Unity to automatically instantiate dehydrating database-backed repositories.

If all your `DbSet`s are located in a single `DbContext`:
```cs
var container = new UnityContainer();
container.RegisterDatabase<MyDbContext>(dehydrate: true)
  .RegisterRepositories();
```

If your `DbSet`s are distributed across multiple `DbContext`s:
```cs
var container = new UnityContainer();
container.RegisterDatabase<PackageDbContext>(dehydrate: true)
  .RegisterRepository(x => x.Packages)
  .RegisterRepository(x => x.PackagesConfig);
container.RegisterDatabase<LoginDbContext>(dehydrate: true)
  .RegisterRepository(x => x.Users)
  .RegisterRepository(x => x.Groups);
```

The repositories are registered with `HierarchicalLifetimeManager`. This means you can use `container.CreateChildContainer()` to start independent sessions.


### WebAPI
For proper JSON-serialization of dehydrated content you should add the following lines to your `WebApiConfig.Register()` method:
```cs
config.Services.Clear(typeof(ModelValidatorProvider));
config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
```

Install the `Dehydrator.WebApi` NuGet package in your WebAPI project. You can then derive your controller classes from `CrudController` or `AsyncCrudController` which take an `IRepository<>` as a constructor argument. Use instances created by `DehydratingRepositoryFactory`. The easiest way to achieve this is dependency injection, e.g., using Unity as described above.

If you choose to use the `CrudController` or `AsyncCrudController` base classes you must add the following line to your `WebApiConfig.Register()` method:
```cs
config.MapHttpAttributeRoutes(new InheritanceRouteProvider());
```

You can also build your own controllers using `IRepository<>`s directly without using the `Dehydrator.WebApi` package.


## Sample project

The source code includes a sample project that uses all Dehydrator components. You can build and run it using Visual Studio 2015 and LocalDB. By default the instance will be hosted by IIS Express at `http://localhost:6297/`.

`POST /api/packages/test-data` fills the database with some initial demo entries.

`GET /api/packages/` lists all entities, showcasing dependencies being dehydrated.

`POST /api/packages/{id}` creates a new entity, showcasing dependency IDs being resolved.

`GET /api/packages/{id}` returns an existing entity, again showcasing dependencies being dehydrated.

`PUT /api/packages/{id}` updates an existing entity, again showcasing dependency IDs being resolved.

`DELETE /api/packages/{id}` deletes an existing entity.
