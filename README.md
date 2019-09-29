# Nzr.Orm
Fast, simple, convention-based (but configurable) and extensible Micro-Orm

## Key features:
Nzr.Orm is a [NuGet library](https://www.nuget.org/packages/Nzr.Orm.Core/) that you can add in to your project providing the following features.

* CRUD Operations based on object properties: Insert, Select, Update and Delete.
* Aggregate Functions based on object properties: Max, Min, Count, Sum, Avg.
* Attributes to override table name and column names. If not provided, the elements will be mapped as lower_case names.
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

using (Dao dao = new Dao())
{
	int affectedRows = dao.Insert(state);
}
```

###### SELECT
------------------------------------------------------------
```csharp
using (Dao dao = new Dao())
{
	State state = dao.Select<State>(123);
}

using (Dao dao = new Dao())
{
	IList<State> states = dao.Select<State>(Where("Name", "CA"), OrderBy("Name"));
}
```

###### UPDATE
------------------------------------------------------------
```csharp
state.Name = "WA";
using (Dao dao = new Dao())
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
using (Dao dao = new Dao())
{
	int result = dao.Delete(state));
}

using (Dao dao = new Dao())
{
	int result = dao.Delete<State>(Where("Name", NE, "CA"));
}
```

###### AGGREGATE
------------------------------------------------------------
```csharp
using (Dao dao = new Dao())
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
Added support to transactions.

#### v0.3.0
Multi Mapping and Foreign Keys (Select only).

###### v0.3.1
Important bug fixed:
* Error when using same column in both Set and Where. [Issue](https://github.com/marionzr/Nzr.Orm/issues/4)

Added support to alias (using static) to reduce the code typing on Set, Where and Aggregate functions. See: [HowToUse](https://raw.githubusercontent.com/marionzr/Nzr.Orm/master/dotnet/Nzr.Orm.Tests/Core/HowToUseTest.cs)

#### v0.4.0
Added Order By support.
Changed the Where clause to be optional.
Renamed the class Alias to Builder since there was no alias, but builders methods there.

#### v0.4.1
Added support to property type of enum.

#### v0.5.0
Added support to inject Logger.
* This feature will be improved while resolving [Issue](https://github.com/marionzr/Nzr.Orm/issues/20). For now, here a some examples of generated log
** Log critical for errors when reading values.
** Log warning for properties not found in the query and also not decorated with NotMappedAttribute.
** Log debug for opening and closing connection/transaction operations
** Log debug before and after execute the Commands (Query and NonQuery) including the SQL, parameters and the results

Added option to automatically trim string values.
* The default is true, which means that if the column is like CHAR(10) but its content has length of 3 (like NZR), instead of returning "NZR       " it will return "NZR"

#### v0.6.0
Add support to raw sql.
* Execute NonQuery
* Execute Quey

#### v0.6.1
Code clean-up: Dao constructors and the Options class to better describe the scenarios where each constructor is applied

#### v0.6.2
Important bug fixed:
* Added support to nullable types. [Issue](https://github.com/marionzr/Nzr.Orm/issues/22)

Minor improvements:
* Added support to map private properties. [Issue](https://github.com/marionzr/Nzr.Orm/issues/19)
* Converted numeric sql types (int, bigint ...) to DateTime [Issue](https://github.com/marionzr/Nzr.Orm/issues/18)
* Included set operation in public string TypeName in the ColumnAttribute. [Issue](https://github.com/marionzr/Nzr.Orm/issues/17)
* Set INNER as a default value for JoinType join property of ForeignKeyAttribute. [Issue](https://github.com/marionzr/Nzr.Orm/issues/16)
* Removed sealed  modifier from NotMappedAttribute class. [Issue](https://github.com/marionzr/Nzr.Orm/issues/15)

#### v0.6.3
Important bug fixed:
* Error selecting double referenced entity in query. [Issue](https://github.com/marionzr/Nzr.Orm/issues/21)

#### v0.7.0
Added support to new Where operators:

* Like

```dao.Select<State>(Where("Name", LIKE,"%CA%"))```
```dao.Select<State>(Where("Name", LIKE,"%CA"))```
```dao.Select<State>(Where("Name", NOT_LIKE,"CA%"))```
```dao.Select<State>(Where("Name", IN, new string[] { "CA", "WA", "CO" }))```

* OR condition:

```dao.Select<State>(Where("Name", "CA").Or("Name", "WA"))```.

* Between clause to select values within a given range.

```dao.Select<State>(limit: 2)```
```dao.Select<State>(Where("Name", NE, "CA"), OrderBy("Name"), 10)```

Added support to Limit clause to specify the number of records to return.

```dao.Select<State>(limit: 2)```
```dao.Select<State>(Where("Name", NE, "CA"), OrderBy("Name"), 10)```

Replaced the using of System.Data.SqlClient with System.Data.Common
* This is the first step to support other databases than MSSQL.
* The Default connection manager still returning a System.Data.SqlClient.SqlConnection but you may
try (not tested) write your own IConnectionManager that returns a DbConnection.

## Upcoming features!

#### v0.7.1

Improve all the tests and documentation replacing the objects Where, Set, OrderBy and Aggregate with the Builder's methods.
This is to enforce the usage of Builders, which are less verbose and more aesthetics.

Improve the exception messages to help on the usage of this framework. [Issue](https://github.com/marionzr/Nzr.Orm/issues/20)

#### v0.7.2

Try adding a default behavior for ForeignKeyAttribute so that some POCO classes can be used without any attributes.

##### v0.8.0

Add support to Multi Mapping and Foreign Keys for Update and Delete.

##### v0.9.0

Add support to configure options with an external file (.xml or .config TBD)

## Know Issues
