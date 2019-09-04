using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to select operations.
    /// </summary>
    public partial class Dao
    {
        #region Operations

        private T DoSelect<T>(object[] keys)
        {
            string sql = BuildSelectSql(typeof(T));
            Parameters parameters = PrepareSelectParameters(typeof(T), keys);

            return ExecuteQuery<T>(sql, parameters).FirstOrDefault();
        }

        private IList<T> DoSelect<T>(Where where)
        {
            string sql = BuildSelectSql(typeof(T), where);
            Parameters parameters = PrepareSqlParameters(typeof(T), where);

            return ExecuteQuery<T>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildSelectSql(Type type)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(type);
            List<string> whereParameters = keyColumns.Select(c => $"{c.Key} = @{FormatParameters(c.Key)}").ToList();

            return BuildSelectSql(type, whereParameters);
        }

        private string BuildSelectSql(Type type, Where where)
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

            return BuildSelectSql(type, whereParameters);
        }

        private string BuildSelectSql(Type type, IList<string> whereParameters)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);
            List<string> what = columns.Select(c => $"{c.Key} AS {FormatParameters(c.Key)}").ToList();

            string sql = $"SELECT {string.Join(", ", what)} FROM {GetTable(type)} WHERE {string.Join(" AND ", whereParameters)}";
            return sql;
        }

        #endregion

        #region Parameters

        private Parameters PrepareSelectParameters(Type type, params object[] keys)
        {
            KeyValuePair<string, PropertyInfo>[] keyColumns = GetKeyColumns(type).ToArray();
            Where where = new Where();

            for (int i = 0; i < keyColumns.Length; i++)
            {
                where.Add(keyColumns[i].Value.Name, Where.EQ, keys[i]);
            }

            return PrepareSqlParameters(type, where);
        }

        private Parameters PrepareSqlParameters(Type type, Where where)
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
