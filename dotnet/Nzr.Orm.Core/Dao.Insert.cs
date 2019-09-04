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

        private int DoInsert(object entity)
        {
            string sql = BuildInsertSql(entity.GetType());
            Parameters parameters = PrepareInsertParameters(entity);

            int result = ExecuteNonQuery(sql, parameters);

            KeyValuePair<string, PropertyInfo> identityColumn = GetIdentityColumn(entity.GetType());

            if (!identityColumn.Equals(default(KeyValuePair<string, PropertyInfo>)))
            {
                identityColumn.Value.SetValue(entity, result);
            }

            return result;
        }

        #endregion

        #region SQL

        private string BuildInsertSql(Type type)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> nonIdentityColumns = GetNonIdentityColumns(type);
            KeyValuePair<string, PropertyInfo> identityColumn = GetIdentityColumn(type);

            List<string> that = nonIdentityColumns.Select(c => c.Key).ToList();
            List<string> parameters = nonIdentityColumns.Select(c => $"@{FormatParameters(c.Key)}").ToList();
            string output = string.Empty;

            if (!identityColumn.Equals(default(KeyValuePair<string, PropertyInfo>)))
            {
                string columnName = GetColumnName(type, identityColumn.Value);
                string identityColumnName = columnName.Replace($"{GetTable(type)}.", string.Empty);
                output = $"output INSERTED.{identityColumnName}";
            }

            string sql = $"INSERT INTO {GetTable(type)} ({string.Join(", ", that)}) {output} VALUES ({string.Join(", ", parameters)})";

            return sql;
        }

        #endregion

        #region Parameters

        private Parameters PrepareInsertParameters(object entity)
        {
            Parameters parameters = new Parameters();
            IEnumerable<KeyValuePair<string, PropertyInfo>> nonIdentityColumns = GetNonIdentityColumns(entity.GetType());

            nonIdentityColumns.ForEach(c => parameters.Add($"@{FormatParameters(c.Key)}", GetValue(c.Value, entity)));

            return parameters;
        }

        #endregion
    }
}
