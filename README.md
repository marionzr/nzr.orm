# Nzr.Orm
Fast, simple, convention-based (but configurable) and extensible Micro-Orm

## Key features:
Nzr.Orm is a [NuGet library](https://www.nuget.org/packages/Nzr.Orm.Core/) that you can add in to your project providing the following features.

* CRUD Operations based on object properties: Insert, Select, Update and Delete
* Aggregate Functions based on object properties: Max, Min, Count, Sum, Avg
* Attributes to overridade table name and colum names. If not provided, the elements will be mapped as lower_case names.
* Support to schema: global for the DAO instance or defined for each table using attributes.
* Support to convert strings to dynamic XML or JSON objects, allowing 
`Characteristics = "<characteristic><brand>NZR</brand></characteristic>"
product.Characteristics.characteristic.brand.ToString()`

# How to use

More examples about how to use it cab be found at [github](https://github.com/marionzr/Nzr.Orm/tree/master/dotnet/Nzr.Orm.Core.Tests).

###### INSERT
------------------------------------------------------------
```csharp
Category category = new Category()
{
    Id = 1,
    Description = "Sports",
    CreatedAt = DateTime.Now
};

using (Dao dao = new Dao(connectionStrings))
{	
	int result = dao.Insert(category);
}
```

###### SELECT
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(connectionStrings))
{
	Category category = dao.Select<Category>(1);
}

using (Dao dao = new Dao(connectionStrings))
{
	Category category = dao.Select<Category>(new Where { { "Description", Where.EQ, "Sports" } });
}
```

###### UPDATE
------------------------------------------------------------
```csharp
Category category = new Category()
{
    Id = 1,
    Description = "e-Sports",
    CreatedAt = DateTime.Now
};

using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Update(category);
}

using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Update<Category>(new Set { { "Description", "Sports" } }, new Where { { "Description", Where.EQ, "e-Sports" } });
}

```

###### DELETE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Delete(category);
}

using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Delete<Category>(new Where { { "Description", Where.NE, "Sports" } });
}
```

###### AGGREGATE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Aggregate<Product, double>(new Aggregate(Aggregate.COUNT, "Id"));
}
```


## Changeset
NOTE: Please wait until version v.1.x.x is released to use this project in production.

All notable changes to this project will be documented in this file.

##### v.0.1.0
Added support to following operations:
* int Insert(object entity)
* T Select<T>(int id)
* T Select<T>(Guid id)
* T Select<T>(object[] ids)
* IList<T> Select<T>(Where where)
* int Update(object entity)
* int Update<T>(Set set, Where where)
* int Delete(object entity)
* int Delete<T>(Where where)
* U Aggregate<T,U>(Aggregate aggregate, Where where)

## Upcoming features!
##### v.0.2.0
Add support to transactions.

##### v.0.3.0
Multi Mapping and Foreing Keys

##### v.0.4.0
Add support to raw sql.
