using Nzr.Orm.Core.Attributes;
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
        public string _BuildSelectJoinSql(Type type)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = GetKeyColumns(type);
            List<string> whereParameters = keyColumns.Select(c => $"{c.Key} = @{FormatParameters(c.Key)}").ToList();

            List<string> what = new List<string>();
            _BuildJoinProjection(ref what, type);
            List<string> how = new List<string>();
            _BuildJoinFilter(ref how, type);

            string query = $"SELECT {string.Join(", ", what)} FROM {GetTable(type)} {string.Join(", ", how)}  WHERE {string.Join(" AND ", whereParameters)}";
            return query;
        }

        /// <summary>
        /// Builds the SELECT  using INNER/LEFT JOINT statements.
        /// </summary>
        /// <param name="where">The pair of column and values to filter the select.</param>
        /// <returns></returns>
        protected virtual string _BuildSelectJoinSql(Type type, Where where = null)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type, true);

            List<string> whereParameters = where.Select(w =>
            {
                KeyValuePair<string, PropertyInfo> column = columns.First(kvp => kvp.Value.Name == w.Item1);

                if (w.Item3 == null)
                {
                    return $"{column.Key} {w.Item2} NULL";
                }

                return $"{column.Key} {w.Item2} (@{FormatParameters(column.Key)})";
            }).ToList();

            List<string> what = new List<string>();
            _BuildJoinProjection(ref what, type);
            List<string> how = new List<string>();
            _BuildJoinFilter(ref how, type);

            string projection = string.Join(", ", what);
            string join = string.Join(", ", how);
            string query = $"SELECT {projection} FROM {GetTable(type)} {join} WHERE {string.Join(" AND ", whereParameters)}";
            return query;
        }

        protected virtual void _BuildJoinProjection(ref List<string> sqlColumns, Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);
            List<string> what = columns.Select(c => $"{c.Key} AS {FormatParameters(c.Key)}").ToList();
            sqlColumns.AddRange(what);

            foreach (KeyValuePair<string, PropertyInfo> column in columns)
            {
                if (column.Value.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    _BuildJoinProjection(ref sqlColumns, column.Value.PropertyType);
                }

            }
        }

        protected virtual void _BuildJoinFilter(ref List<string> sqlJoin, Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            foreach (KeyValuePair<string, PropertyInfo> column in columns)
            {
                ForeignKeyAttribute fKAttribute = column.Value.GetCustomAttribute<ForeignKeyAttribute>();

                if (fKAttribute != null)
                {
                    IDictionary<string, PropertyInfo> outerColumns = GetColumns(column.Value.PropertyType);
                    KeyValuePair<string, PropertyInfo> outerColumn = outerColumns.First(cout => cout.Value.Name == fKAttribute.JoinPropertyName);

                    string join = $"{fKAttribute.Join.ToString().ToUpper()} JOIN {GetTable(column.Value.PropertyType)} " +
                                            $"ON {GetColumnName(type, column.Value)} = {GetColumnName(column.Value.PropertyType, outerColumn.Value)}";

                    sqlJoin.Add(join);
                    _BuildJoinFilter(ref sqlJoin, outerColumn.Value.PropertyType);
                }
            }
        }
    }
}
