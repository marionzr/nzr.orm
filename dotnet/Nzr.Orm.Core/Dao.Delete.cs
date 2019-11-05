using Nzr.Orm.Core.Sql;
using System;
using System.Linq;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to update operations.
    /// </summary>
    public partial class Dao
    {
        #region Operations

        private int DoDelete(object entity, int? expectedResult = null)
        {
            Where where = BuildWhereFromIds(entity);
            return DoDelete(entity.GetType(), where, expectedResult);
        }

        private int DoDelete<T>(Where where, int? expectedResult = null) => DoDelete(typeof(T), where, expectedResult);

        private int DoDelete(Type type, Where where, int? expectedResult = null)
        {
            string multiPartIdentifier = where.FirstOrDefault(w => w.Item1.Contains("."))?.Item1;

            if (multiPartIdentifier != null)
            {
                throw new NotSupportedException($"Multi Part Identifier ({multiPartIdentifier}) is not yet supported for delete operation. Use ExecuteNonQuery with a custom SQL.");
            }

            BuildMap(type);
            string sql = BuildDeleteSql(type, where);
            Parameters parameters = BuildWhereParameters(where);
            int result = DoExecuteNonQuery(sql, parameters, expectedResult);

            return result;
        }

        #endregion

        #region SQL

        private string BuildDeleteSql(Type type, Where where)
        {
            string whereFilters = BuildWhereFilters(where);
            string sql = $"DELETE FROM {GetTableName(type)} WHERE {whereFilters}";

            return sql;
        }

        #endregion
    }
}
