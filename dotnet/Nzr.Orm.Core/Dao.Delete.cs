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
    /// Contains all methods related to update operations.
    /// </summary>
    public partial class Dao
    {
        #region Operations

        private int DoDelete(object entity)
        {
            string sql = BuildDeleteSql(entity.GetType());
            Parameters parameters = PrepareDeleteParameters(entity);

            int result = ExecuteNonQuery(sql, parameters);

            return result;
        }

        private int DoDelete<T>(Where where)
        {
            Type type = typeof(T);
            string sql = BuildDeleteSql(type, where);
            Parameters parameters = PrepareDeleteParameters(type, where);

            int result = ExecuteNonQuery(sql, parameters);

            return result;
        }

        #endregion

        #region SQL 

        private string BuildDeleteSql(Type type)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(type);

            IList<string> whereParameters = keyColumns.Select(c => $"{c.Key} = @{FormatParameters(c.Key)}").ToList();

            return BuildDeleteSql(type, whereParameters);
        }

        private string BuildDeleteSql(Type type, Where where)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            IList<string> whereParameters = where.Select(w =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == w.Item1);
                return $"{column.Key} = @{FormatParameters(column.Key)}";
            }).ToList();

            return BuildDeleteSql(type, whereParameters);
        }

        private string BuildDeleteSql(Type type, IList<string> whereParameters)
        {
            string sql = $"DELETE FROM {GetTable(type)} WHERE {string.Join(" AND ", whereParameters)}";
            return sql;
        }

        #endregion

        #region Parameters

        private Parameters PrepareDeleteParameters(object entity)
        {
            Parameters parameters = new Parameters();
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(entity.GetType());

            keyColumns.ForEach(c =>
            {
                parameters.Add($"@{FormatParameters(c.Key)}", GetValue(c.Value, entity));
            });

            return parameters;
        }

        private Parameters PrepareDeleteParameters(Type type, Where where)
        {
            Parameters parameters = new Parameters();
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

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
