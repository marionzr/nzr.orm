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
            string sql = BuildScalarSql(typeof(T), aggregate, where);
            Parameters parameters = PrepareScalarParameters(typeof(T), where);

            return ExecuteScalar<T, U>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildScalarSql(Type type, Aggregate aggregate, Where where)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            List<string> whereParameters = where.Select(w =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == w.Item1);

                if (w.Item3 == null)
                {
                    return $"{column.Key} {w.Item2} NULL";
                }

                return $"{column.Key} {w.Item2} (@{FormatParameters(column.Key)})";
            }).ToList();

            whereParameters.Add("1=1");

            string aggregateColumn = columns.First(c => c.Value.Name == aggregate.Item2).Key;
            string table = GetTable(type);

            string sql = $"SELECT {aggregate.Item1} ({aggregateColumn}) FROM {table} WHERE {string.Join(" AND ", whereParameters)}";
            return sql;
        }

        #endregion

        #region Parameters

        private Parameters PrepareScalarParameters(Type type, Where where)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);
            Parameters whereParameters = new Parameters();

            where.ForEach(w =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == w.Item1);

                if (w.Item3 != null)
                {
                    whereParameters.Add($"@{FormatParameters(column.Key)}", w.Item3);
                }
            });

            return whereParameters;
        }

        #endregion
    }
}
