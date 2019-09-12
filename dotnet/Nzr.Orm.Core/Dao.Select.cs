using Nzr.Orm.Core.Attributes;
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
    /// Contains all methods related to select operations.
    /// </summary>
    public partial class Dao
    {
        #region Operations

        private T DoSelect<T>(object[] ids)
        {
            Where where = BuildWhereFromIds<T>(ids);

            return DoSelect<T>(where, new OrderBy()).FirstOrDefault();
        }

        private IList<T> DoSelect<T>(Where where, OrderBy orderBy)
        {
            Type type = typeof(T);
            string sql = BuildSelectSql(type, where, orderBy);
            Parameters parameters = BuildWhereParameters(type, where, true);

            return ExecuteQuery<T>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildSelectSql(Type type, Where where, OrderBy orderBy)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type, true);
            IList<string> whereParameters = BuildWhereFilters(columns, where);
            IList<string> what = BuildProjection(type);
            IList<string> joins = BuildJoinFilter(type);
            IList<string> columnsAndSorting = BuildOrderBy(columns, orderBy);
            string orderByColumns = columnsAndSorting.Any() ? $"ORDER BY {string.Join(", ", columnsAndSorting)}" : string.Empty;

            string query = $"SELECT {string.Join(", ", what)} FROM {GetTable(type)} {string.Join(" ", joins)} WHERE {string.Join(" AND ", whereParameters)} {orderByColumns}".TrimEnd();
            return query;
        }

        private IList<string> BuildProjection(Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);
            IList<string> projection = new List<string>();
            columns.Select(c => $"{c.Key} AS {FormatParameters(c.Key)}").ForEach(c => projection.Add(c));

            foreach (KeyValuePair<string, PropertyInfo> column in columns)
            {
                if (column.Value.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    BuildProjection(column.Value.PropertyType).ForEach(c => projection.Add(c));
                }
            }

            return projection;
        }

        private IList<string> BuildJoinFilter(Type type, string parentJoinType = null)
        {
            IList<string> joins = new List<string>();
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type).Where(c => c.Value.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();

            foreach (KeyValuePair<string, PropertyInfo> column in columns)
            {
                ForeignKeyAttribute fKAttribute = column.Value.GetCustomAttribute<ForeignKeyAttribute>();

                string joinType = IsLeftJoin(parentJoinType) ? ForeignKeyAttribute.JoinType.Left.ToString().ToUpper() : fKAttribute.Join.ToString().ToUpper();
                string joinTable = GetTable(column.Value.PropertyType);
                string myColumn = GetColumnName(type, column.Value);
                IDictionary<string, PropertyInfo> outerColumns = GetColumns(column.Value.PropertyType);
                KeyValuePair<string, PropertyInfo> outerColumn = outerColumns.First(cout => cout.Value.Name == fKAttribute.JoinPropertyName);
                string theirColumn = GetColumnName(column.Value.PropertyType, outerColumn.Value);

                string join = $"{joinType} JOIN {joinTable} ON {myColumn} = {theirColumn}";
                joins.Add(join);

                BuildJoinFilter(outerColumn.Value.ReflectedType, joinType).ForEach(nextJoin => joins.Add(nextJoin));
            }

            return joins;
        }

        private IList<string> BuildOrderBy(IDictionary<string, PropertyInfo> columns, OrderBy orderBy)
        {
            IEnumerable<string> orderByProperties = orderBy.Select(o =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, orderBy.ReflectedType, o.Item1);
                return $"{column.Key} {o.Item2}";
            });

            return orderByProperties.ToList();
        }

        private bool IsLeftJoin(string joinType) => joinType != null && joinType == ForeignKeyAttribute.JoinType.Left.ToString().ToUpper();

        #endregion
    }
}
