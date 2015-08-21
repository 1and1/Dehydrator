Dehydrator
==========

Dehydrator helps you combine ORMs like [Entity Framework](https://msdn.microsoft.com/data/ef.aspx) with REST service frameworks like [WebAPI](http://www.asp.net/web-api).

By stripping navigational references in your entities down to only their IDs their serialized representation no longer contains redundant or cyclic data. When new or modified deserialized data needs to be persisted Dehydrator takes care of resolving the reference IDs again.

You can get the libraries via NuGet:
* [Dehydrator](https://www.nuget.org/packages/Dehydrator/)
* [Dehydrator.EntityFramework](https://www.nuget.org/packages/Dehydrator.EntityFramework/)
* [Dehydrator.WebAPI](https://www.nuget.org/packages/Dehydrator.WebAPI/)
