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
State state = new State() { Name = "CA" };

using (Dao dao = new Dao(transaction, options))
{
	int affectedRows = dao.Insert(state);
}
```

###### SELECT
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(transaction, options))
{
	State state = dao.Select<State>(123);
}

using (Dao dao = new Dao(transaction, options))
{
	List<State> states = dao.Select<State>(new Where { { "Name", Where.	EQ "CA" } });
}
```

###### UPDATE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(transaction, options))
{
	int result = dao.Update(state));
}

```

###### DELETE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(transaction, options))
{
	int result = dao.Delete(state));
}

using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Delete<State>(new Where { { "Name", Where.NE, "CA" } });
}
```

###### AGGREGATE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Aggregate<State, int>(new Aggregate(Aggregate.COUNT, "Id"));
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

##### v.0.2.0
Add support to transactions.

##### v.0.3.0
Multi Mapping and Foreing Keys (Select only)

## Upcoming features!

##### v.0.4.0
Add support to Multi Mapping and Foreing Keys for Update and Delete

#### v.0.5.0
Add support to raw sql.

## Know Issues
Error when using same column in both Set and Where
https://github.com/marionzr/Nzr.Orm/issues/4
https://github.com/marionzr/Nzr.Orm/issues/5