# Nzr.Orm
Fast, simple, convention-based (but configurable) and extensible Micro-Orm

## Key features:
Nzr.Orm is a [NuGet library](https://www.nuget.org/packages/Nzr.Orm.Core/) that you can add in to your project providing the following features.

* CRUD Operations based on object properties: Insert, Select, Update and Delete.
* Aggregate Functions based on object properties: Max, Min, Count, Sum, Avg.
* Attributes to overridade table name and colum names. If not provided, the elements will be mapped as lower_case names.
* Support to schema: global for the DAO instance or defined for each table using attributes.
* Support to convert strings to dynamic XML or JSON objects, allowing 
`Characteristics = "<characteristic><brand>NZR</brand></characteristic>"
product.Characteristics.characteristic.brand.ToString()`

# How to use

More examples about how to use it cab be found at [HowToUse](https://raw.githubusercontent.com/marionzr/Nzr.Orm/master/dotnet/Nzr.Orm.Tests/Core/HowToUseTest.cs) and [Test Project](https://github.com/marionzr/Nzr.Orm/tree/master/dotnet/Nzr.Orm.Core.Tests).

###### USINGS
------------------------------------------------------------
```csharp
using Nzr.Orm.Core;
using static Nzr.Orm.Core.Sql.Aggregate;
using static Nzr.Orm.Core.Sql.Builders;
using static Nzr.Orm.Core.Sql.OrderBy;
using static Nzr.Orm.Core.Sql.Where;
```
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
	// IList<State> states = dao.Select<State>(new Where { { "Name", Where.EQ "CA" } }, new OrderBy { { "Name", OrderBy.DESC } });
	// IList<State> states = dao.Select<State>(Where("Name", EQ, "CA"));
	// IList<State> states = dao.Select<State>(Where("Name", "CA"), OrderBy("Name", DESC));
	IList<State> states = dao.Select<State>(Where("Name", "CA"), OrderBy("Name"));
}
```

###### UPDATE
------------------------------------------------------------
```csharp
state.Name = "WA";
using (Dao dao = new Dao(transaction, options))
{
	int result = dao.Update(state));
}

using (Dao dao = new Dao(transaction, options))
{
    int result = dao.Update<State>(Set("Name", "NY"), Where("Name", "WA").And("Description", IS_NOT, null));
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
	int result = dao.Delete<State>(Where("Name", NE, "CA"));
}
```

###### AGGREGATE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao(connectionStrings))
{
	int result = dao.Aggregate<State, int>(Aggregate(COUNT, "Id"));
}
```

## Changeset
NOTE: Please wait until version v.1.x.x is released to use this project in production.

All notable changes to this project will be documented in this file.

#### v0.1.0
Added support to following operations:
* int Insert(object entity)
* T Select<T>(int id)
* T Select<T>(Guid id)
* T Select<T>(object[] ids)
* IList<T> Select<T>(Where where, OrderBy orderBy)
* int Update(object entity)
* int Update<T>(Set set, Where where)
* int Delete(object entity)
* int Delete<T>(Where where)
* U Aggregate<T,U>(Aggregate aggregate, Where where)

#### v0.2.0
Add support to transactions.

#### v0.3.0
Multi Mapping and Foreing Keys (Select only).

###### v0.3.1
Important bug fixed:
* Error when using same column in both Set and Where
https://github.com/marionzr/Nzr.Orm/issues/4

Added support to alias (using static) to reduce the code typing on Set, Where and Aggregate functions. See: [HowToUse](https://raw.githubusercontent.com/marionzr/Nzr.Orm/master/dotnet/Nzr.Orm.Tests/Core/HowToUseTest.cs)

#### v0.4.0
Add Order By support.
Changed the Where clause to be optional.
Renamed the class Alias to Builder since there was no alias, but builders methods there.

#### v0.4.1
Added support to property type of enum.

## Upcoming features!

#### v0.5.0
Add support to inject Logger.

#### v.0.6.0
Add support to raw sql.

##### v0.7.0
Add Where("Column", "Value").Or("Column", "Value") support.
Add support to Multi Mapping and Foreing Keys for Update and Delete.

## Know Issues
