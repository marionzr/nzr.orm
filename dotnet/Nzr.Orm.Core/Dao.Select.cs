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

        private IList<T> DoSelect<T>(Where where, OrderBy orderBy, ulong limit = ulong.MaxValue)
        {
            Type type = typeof(T);
            string sql = BuildSelectSql(type, where, orderBy, limit);
            Parameters parameters = BuildWhereParameters(type, where, true);

            return DoExecuteQuery<T>(sql, parameters);
        }

        #endregion

        #region SQL

        private string BuildSelectSql(Type type, Where where, OrderBy orderBy, ulong limit)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type, true);

            string fullTableName = GetTable(type);
            string aliasTableName = $"t1";

            IList<string> what = BuildProjection(type);
            IList<string> joins = BuildJoinFilter(type);

            string whereFilters = BuildWhereFilters(columns, where);
            IList<string> columnsAndSorting = BuildOrderBy(columns, orderBy);
            string orderByColumns = columnsAndSorting.Any() ? $"ORDER BY {string.Join(", ", columnsAndSorting)}" : string.Empty;
            string limitRows = limit != ulong.MaxValue ? $"TOP {limit} " : string.Empty;

            string query = $"SELECT {limitRows}{string.Join(", ", what)} FROM {fullTableName} AS {aliasTableName} {string.Join(" ", joins)} WHERE {whereFilters} {orderByColumns}".TrimEnd();
            return query;
        }

        private IList<string> BuildProjection(Type type, Type parentType = null)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);
            IList<string> projection = new List<string>();
            string fullTableName = GetTable(type);

            if (Mappings.Index == 0)
            {
                Mappings.Index = 1;
                Mappings.Paths.Add(@"\");
            }

            string currPath = Mappings.Paths.Last();
            string aliasTableName = $"t{Mappings.Index}";

            columns
                .Select(c =>
                {
                    string fullColumnName = c.Key;
                    string simleColumnName = fullColumnName.Replace(fullTableName, aliasTableName);
                    string aliasColumnName = simleColumnName.Replace(".", "_");

                    Mapping mapping = new Mapping()
                    {
                        FullColumnName = fullColumnName,
                        SimpleColumnName = simleColumnName,
                        AliasColumnName = aliasColumnName,
                        Property = c.Value,
                        FullTableName = fullTableName,
                        AliasTableName = aliasTableName,
                        EntityType = type,
                        ParentEntityType = parentType,
                        TableIndex = Mappings.Index,
                        Path = currPath
                    };

                    Mappings.Add(mapping);

                    return $"{simleColumnName} AS {aliasColumnName}";
                })
                .ForEach(c => projection.Add(c));

            foreach (KeyValuePair<string, PropertyInfo> column in columns)
            {
                if (column.Value.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    Mappings.Paths.Add($"{currPath}{column.Value.Name}\\");
                    Mappings.Index++;
                    BuildProjection(column.Value.PropertyType, type).ForEach(c => projection.Add(c));
                }
            }

            return projection;
        }

        private IList<string> BuildJoinFilter(Type type, string parentJoinType = null, string path = @"\")
        {
            IList<string> joins = new List<string>();
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type).Where(c => c.Value.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();

            List<Mapping> mappingsFk = Mappings.Where(m => m.Path == path && m.EntityType == type && m.Property.GetCustomAttribute<ForeignKeyAttribute>() != null).ToList();

            foreach (Mapping mapping in mappingsFk)
            {
                ForeignKeyAttribute fKAttribute = mapping.Property.GetCustomAttribute<ForeignKeyAttribute>();
                string joinType = IsLeftJoin(parentJoinType) ? ForeignKeyAttribute.JoinType.Left.ToString().ToUpper() : fKAttribute.Join.ToString().ToUpper();

                Mapping theirMapping = Mappings.FirstOrDefault(m => m.Path.EndsWith(mapping.Path + mapping.Property.Name + "\\") && m.Property.Name == fKAttribute.JoinPropertyName);

                string join = $"{joinType} JOIN {theirMapping.FullTableName} AS {theirMapping.AliasTableName} ON {mapping.SimpleColumnName} = {theirMapping.SimpleColumnName}";
                joins.Add(join);

                BuildJoinFilter(theirMapping.Property.ReflectedType, joinType, theirMapping.Path).ForEach(nextJoin => joins.Add(nextJoin));
            }


            return joins.OrderBy(i => i.StartsWith("INNER") ? 0 : 1).ToList();
        }

        private IList<string> BuildOrderBy(IList<KeyValuePair<string, PropertyInfo>> columns, OrderBy orderBy)
        {
            IEnumerable<string> orderByProperties = orderBy.Select(o =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, orderBy.ReflectedType, o.Item1);
                Mapping mapping = Mappings.First(m => m.EntityType == column.Value.ReflectedType && m.Property.Name == column.Value.Name);
                string columnName = mapping?.SimpleColumnName ?? column.Key;

                return $"{columnName} {o.Item2}";
            });

            return orderByProperties.ToList();
        }

        private bool IsLeftJoin(string joinType) => joinType != null && joinType == ForeignKeyAttribute.JoinType.Left.ToString().ToUpper();

        #endregion
    }
}
