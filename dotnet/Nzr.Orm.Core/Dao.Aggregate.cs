using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nzr.Orm.Core
{/// <summary>
 /// Partial class Dao.
 /// Contains all methods related to aggregate operations.
 /// </summary>
    public partial class Dao
    {
        #region Operations

        private U DoAggregate<T, U>(Aggregate aggregate, Where where)
        {
            Type type = typeof(T);
            string sql = BuildAggregateSql(type, aggregate, where);
            Parameters parameters = BuildWhereParameters(type, where);

            return DoExecuteScalar<U>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildAggregateSql(Type type, Aggregate aggregate, Where where)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);
            IList<string> whereParameters = BuildWhereFilters(columns, where);
            string aggregateColumn = columns.First(c => c.Value.Name == aggregate.Item2).Key;
            string sql = $"SELECT {aggregate.Item1} ({aggregateColumn}) FROM {GetTable(type)} WHERE {string.Join(" AND ", whereParameters)}";

            return sql;
        }

        #endregion
    }
}
