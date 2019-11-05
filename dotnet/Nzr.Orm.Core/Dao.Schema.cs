using Nzr.Orm.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to schema information.
    /// </summary>
    public partial class Dao
    {
        /// <summary>
        /// Gets the table name for the given type.
        /// </summary>
        /// <param name="type">The type to be mapped to table.</param>
        /// <param name="includeSchema">If true, includes the [schema]. in the result.</param>
        /// <returns>The table name.</returns>
        public virtual string GetTableName(Type type, bool includeSchema = true)
        {
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            string schema = tableAttribute?.Schema ?? Options.Schema;
            string table = tableAttribute?.Name ?? FormatName(type.Name);
            string tableName = includeSchema ? $"[{schema}].{table}" : table;
            return tableName;
        }

        private IList<KeyValuePair<string, PropertyInfo>> GetColumns(Type type, bool includeForeingKeys = false)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = new List<KeyValuePair<string, PropertyInfo>>();
            GetColumns(ref columns, type, includeForeingKeys);

            return columns;
        }

        private void GetColumns(ref IList<KeyValuePair<string, PropertyInfo>> columns, Type type, bool includeForeingKeys = false)
        {
            try
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => FilterColumns(p))
                    .ToArray();

                IDictionary<string, PropertyInfo> currEntityColumns = properties
                    .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                    .ToDictionary(p => GetColumnName(type, p), p => p);

                foreach (KeyValuePair<string, PropertyInfo> column in currEntityColumns)
                {
                    columns.Add(column);

                    if (includeForeingKeys)
                    {
                        ForeignKeyAttribute foreinKeyAttribute = column.Value.GetCustomAttribute<ForeignKeyAttribute>();

                        if (foreinKeyAttribute != null)
                        {
                            GetColumns(ref columns, column.Value.PropertyType, includeForeingKeys);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OrmException($"Error on GetColumns from Type {type}.", ex);
            }
        }

        private bool FilterColumns(PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);

            return ((getMethodInfo != null && getMethodInfo.IsPublic) || propertyInfo.GetCustomAttribute<BaseAttribute>(true) != null);
        }

        /// <summary>
        /// Gets the column name of a given property
        /// </summary>
        /// <param name="type">The type to be reflected and have a mapping to a column.</param>
        /// <param name="property">The property to retrieve the column name.</param>
        /// <returns></returns>
        public virtual string GetColumnName(Type type, PropertyInfo property)
        {
            ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>(true);
            string table = GetTableName(type);

            if (columnAttribute != null && columnAttribute.Name != null)
            {
                return $"{table}.{columnAttribute.Name}";
            }

            ForeignKeyAttribute foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>(true);

            if (foreignKeyAttribute != null && Options.InferComposedIdInForeignKeys)
            {
                string foreignTableName = GetTableName(property.PropertyType, false);
                return $"{table}.id_{foreignTableName}";
            }

            string column = FormatName(property.Name);

            return column == "id" && Options.UseComposedId ?
                $"{table}.{column}_{GetTableName(type, false)}" :
                $"{table}.{column}";
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetKeyColumns(Type type)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);

            IEnumerable<KeyValuePair<string, PropertyInfo>> keyColumns = columns
                    .Where(c => c.Value.GetCustomAttribute<KeyAttribute>(true) != null)
                    .OrderBy(c => c.Value.GetCustomAttribute<KeyAttribute>(true).Order);

            if (!keyColumns.Any())
            {
                keyColumns = columns.Where(c => c.Value.Name == "Id");
            }

            return keyColumns;
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetNonAutoGeneratedColumns(Type type)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);

            IEnumerable<KeyValuePair<string, PropertyInfo>> nonAutoGenerated = columns
                .Where(c => c.Value.GetCustomAttribute<KeyAttribute>() == null || !c.Value.GetCustomAttribute<KeyAttribute>().AutoGenerated);

            if (Options.AutoGeneratedKey)
            {
                nonAutoGenerated = nonAutoGenerated.Where(c => c.Value.Name != "Id");
            }

            return nonAutoGenerated;
        }

        private KeyValuePair<string, PropertyInfo> GetIdentityColumn(Type type)
        {
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type);

            KeyValuePair<string, PropertyInfo> identityColumn = columns
                .FirstOrDefault(c => c.Value.GetCustomAttribute<KeyAttribute>() != null && c.Value.GetCustomAttribute<KeyAttribute>().AutoGenerated);

            if (identityColumn.Key == null)
            {
                identityColumn = columns.FirstOrDefault(c => c.Value.Name == "Id");
            }

            return identityColumn;
        }
    }
}
