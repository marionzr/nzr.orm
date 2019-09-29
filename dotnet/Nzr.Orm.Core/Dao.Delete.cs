using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
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
            Where where = BuildWhereFromIds(entity);
            return DoDelete(entity.GetType(), where);
        }

        private int DoDelete<T>(Where where) => DoDelete(typeof(T), where);

        private int DoDelete(Type type, Where where)
        {
            string sql = BuildDeleteSql(type, where);
            Parameters parameters = BuildWhereParameters(type, where);
            int result = DoExecuteNonQuery(sql, parameters);

            return result;
        }

        #endregion

        #region SQL

        private string BuildDeleteSql(Type type, Where where)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);
            string whereFilters = BuildWhereFilters(columns, where);
            string sql = $"DELETE FROM {GetTable(type)} WHERE {whereFilters}";

            return sql;
        }

        #endregion
    }
}
