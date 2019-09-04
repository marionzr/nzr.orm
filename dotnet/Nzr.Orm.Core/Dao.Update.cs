using Nzr.Orm.Core.Extensions;
using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to insert operations.
    /// </summary>
    public partial class Dao
    {
        #region Operations
        private int DoUpdate(object entity)
        {
            string sql = BuildUpdateSql(entity.GetType());
            Parameters parameters = PrepareUpdateParameters(entity);

            int result = ExecuteNonQuery(sql, parameters);

            return result;
        }
        private int DoUpdate<T>(Set set, Where where)
        {
            Type type = typeof(T);
            string sql = BuildUpdateSql(type, set, where);
            Parameters parameters = PrepareUpdateParameters(type, set, where);

            int result = ExecuteNonQuery(sql, parameters);

            return result;
        }

        #endregion

        #region SQL
        private string BuildUpdateSql(Type type)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> nonkeyColumns = GetNonKeyColumns(type);
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(type);

            IList<string> setParameters = nonkeyColumns.Select(c => $"{c.Key} = @{FormatParameters(c.Key)}").ToList();
            IList<string> whereParameters = keyColumns.Select(c => $"{c.Key} = @{FormatParameters(c.Key)}").ToList();

            return BuildUpdateSql(type, setParameters, whereParameters);
        }

        private string BuildUpdateSql(Type type, Set set, Where where)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            IList<string> setParameters = set.Select(s =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == s.Item1);
                return $"{column.Key} = @{FormatParameters(column.Key)}";
            }).ToList();

            IList<string> whereParameters = where.Select(w =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == w.Item1);
                return $"{column.Key} = @{FormatParameters(column.Key)}";
            }).ToList();

            return BuildUpdateSql(type, setParameters, whereParameters);
        }

        private string BuildUpdateSql(Type type, IList<string> setParameters, IList<string> whereParameters)
        {
            string sql = $"UPDATE {GetTable(type)} SET {string.Join(", ", setParameters)} WHERE {string.Join(" AND ", whereParameters)}";
            return sql;
        }

        #endregion

        #region Parameters

        private Parameters PrepareUpdateParameters(object entity)
        {
            Parameters parameters = new Parameters();
            IEnumerable<KeyValuePair<string, PropertyInfo>> nonkeyColumns = GetNonKeyColumns(entity.GetType());
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(entity.GetType());

            nonkeyColumns.ForEach((c) => parameters.Add($"@{FormatParameters(c.Key)}", GetValue(c.Value, entity)));
            keyColumns.ForEach((c) => parameters.Add($"@{FormatParameters(c.Key)}", GetValue(c.Value, entity)));

            return parameters;
        }

        private Parameters PrepareUpdateParameters(Type type, Set set, Where where)
        {
            Parameters parameters = new Parameters();
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            set.ForEach((parameter, value) =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(c => c.Value.Name == parameter);
                parameters.Add($"@{FormatParameters(column.Key)}", value);
            });

            where.ForEach((parameter, condition, value) =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(c => c.Value.Name == parameter);
                parameters.Add($"@{FormatParameters(column.Key)}", value);
            });

            return parameters;
        }

        #endregion
    }
}
