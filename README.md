EntityReferenceStripper
=======================

EntityReferenceStripper helps you combine ORMs like [Entity Framework](https://msdn.microsoft.com/data/ef.aspx) with REST service frameworks like [WebAPI](http://www.asp.net/web-api).

By stripping navigational references in your entities down to only their IDs their serialized representation no longer contains redundant or cyclic data. When new or modified deserialized data needs to be persisted EntityReferenceStripper takes care of resolving the reference IDs again.

You can get the libraries via NuGet:
* [EntityReferenceStripper](https://www.nuget.org/packages/EntityReferenceStripper/)
* [EntityReferenceStripper.EntityFramework](https://www.nuget.org/packages/EntityReferenceStripper.EntityFramework/)
* [EntityReferenceStripper.WebAPI](https://www.nuget.org/packages/EntityReferenceStripper.WebAPI/)
