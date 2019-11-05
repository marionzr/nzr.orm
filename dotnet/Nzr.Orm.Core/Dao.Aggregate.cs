using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

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
            bool useMultiPartIdentifier = where.Any(w => w.Item1.Contains("."));

            Type type = typeof(T);
            BuildMap(type);
            string sql = BuildAggregateSql(type, aggregate, where, useMultiPartIdentifier);
            Parameters parameters = BuildWhereParameters(where, useMultiPartIdentifier);

            return DoExecuteScalar<U>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildAggregateSql(Type type, Aggregate aggregate, Where where, bool useMultiPartIdentifier)
        {
            string fullTableName = GetTableName(type);
            string aliasTableName = useMultiPartIdentifier ? $"AS t1" : string.Empty;

            IList<string> joins = useMultiPartIdentifier ? BuildJoinFilter(type) : new List<string>();
            string whereFilters = BuildWhereFilters(where, useMultiPartIdentifier);

            Mapping mapping = GetColumnByPropertyName(type, aggregate.Item2);
            string aggregateColumn = useMultiPartIdentifier ? mapping.SimpleColumnName : mapping.FullColumnName;
            string sql = $"SELECT {aggregate.Item1} ({aggregateColumn}) FROM {fullTableName} {aliasTableName} {string.Join(" ", joins)} WHERE {whereFilters}";

            return sql;
        }

        #endregion
    }
}
