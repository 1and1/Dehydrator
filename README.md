# Dehydrator

Dehydrator helps you combine ORMs like [Entity Framework](https://msdn.microsoft.com/data/ef.aspx) with REST service frameworks like [WebAPI](http://www.asp.net/web-api).

By stripping navigational references in your entities down to only their IDs their serialized representation no longer contains redundant or cyclic data. When new or modified deserialized data needs to be persisted Dehydrator takes care of resolving the reference IDs again.

NuGet packages:
* [Dehydrator](https://www.nuget.org/packages/Dehydrator/)
* [Dehydrator.EntityFramework](https://www.nuget.org/packages/Dehydrator.EntityFramework/)
* [Dehydrator.WebApi](https://www.nuget.org/packages/Dehydrator.WebApi/)


## Usecase sample

Without Dehydrator your REST service might return something like this for `GET /apps/1`:
```javascript
{
  "Id": 1,
  "Name": "AwesomeApp",
  "Hash": "123abc"
  "Dependencies":
  [
    {
      "Id": 2,
      "Name": "AwesomeLib",
      "Hash": "789xyz"
      "Dependencies": []
    }
  ]
}
```
The entire `AwesomeLib` reference is inlined. This duplicates content from `/apps/2` and complicates `POST` and `PUT` calls. Are references expected to be existing entities? Are they created/modified together with the parent entity if no matching entity exists yet?

With Dehydrator the same entity would be serialized as:
```javascript
{
  "Id": 1,
  "Name": "AwesomeApp",
  "Hash": "123abc"
  "Dependencies":
  [
    {"Id": 2}
  ]
}
```
This removes any potential duplication and ambiguity.


## Getting started

### Data model
Install the `Dehydrator` NuGet package in the project holding your data model. Make all your entity classes either implement `IEntity` or derive from `Entity`. You can now use the `.DehydrateReferences()` and `.ResolveReferences()` extension methods to "dehydrate" references down to only their IDs and restore them again. For resolving an `IRepositoryFactory` is required.

An `IRepositoryFactory` represents a storage backend such as a database and provides `IRepository<>` instances for specific entity types.

### Entity Framework
Install the `Dehydrator.EntityFramework` NuGet package in the project you use for database access via Entity Framework. This provides `DbRepositoryFactory`, an `IRepositoryFactory` implementation based on Entity Framework's `DbSet`.

Use the `DehydratingRepositoryFactory` decorator to wrap your `IRepositoryFactory` implementation (`DbRepositoryFactory` or a custom implementation). This decorator transparently dehydrates and resolve references when loading and saving entities.

### WebAPI
To avoid unnecessary noise in dehydrated JSON output you should add the following line to your `WebApiConfig.Register()` method:
```cs
config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
```

Install the `Dehydrator.WebApi` NuGet package in your WebAPI project. You can then derive your controller classes from `CrudController` or `AsyncCrudController` which take an `IRepository<>` as a constructor argument. Use instances created by `DehydratingRepositoryFactory`. The best way to acheive this is dependency injection. See below for Unity.

If you choose to use the `CrudController` or `AsyncCrudController` base classes you must add the following line to your `WebApiConfig.Register()` method:
```cs
config.MapHttpAttributeRoutes(new InheritanceRouteProvider());
```

You can also build your own controller using `IRepository<>`s without using the `Dehydrator.WebApi` package.

### Unity
If you wish to use the Unity dependency injection library consider using the following class as a template for registering the appropriate Dehydrator types:
```cs
public static class UnityConfig
{
  public static IUnityContainer InitContainer()
  {
    return new UnityContainer()
      .RegisterType<DbContext, YourOwnDbContext>()
      .RegisterDehydratedDbRepository()
      .EnableRepositoryFactory();
  }

  private static IUnityContainer RegisterDehydratedDbRepository(this IUnityContainer container)
  {
    return container.RegisterType<IRepositoryFactory>(new InjectionFactory(c =>
      new DehydratingRepositoryFactory(new DbRepositoryFactory(c.Resolve<DbContext>()))));
  }

  private static IUnityContainer EnableRepositoryFactory(this IUnityContainer container)
  {
    return container.RegisterType(typeof(IRepository<>), new InjectionFactory((c, t, s) =>
      c.Resolve<IRepositoryFactory>().Create(t.GetGenericArguments()[0])));
  }
}
```
