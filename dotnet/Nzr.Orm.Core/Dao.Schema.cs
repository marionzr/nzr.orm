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
        private string GetTable(Type type)
        {
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();

            string schema = tableAttribute?.Schema ?? Schema;
            string table = tableAttribute?.Name ?? FormatName(type.Name);

            return $"[{schema}].{table}";
        }

        private IDictionary<string, PropertyInfo> GetColumns(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, PropertyInfo> columns = properties
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToDictionary(p => GetColumnName(type, p), p => p);

            return columns;
        }

        private string GetColumnName(Type type, PropertyInfo property)
        {
            ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            string column = columnAttribute?.Name ?? FormatName(property.Name);
            return $"{GetTable(type)}.{column}";
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetKeyColumns(Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            IEnumerable<KeyValuePair<string, PropertyInfo>> nonIdentityColumns = columns
                    .Where(c => c.Value.GetCustomAttribute<KeyAttribute>(true) != null)
                    .OrderBy(c => c.Value.GetCustomAttribute<KeyAttribute>(true).Order);

            return nonIdentityColumns;
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetNonIdentityColumns(Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            IEnumerable<KeyValuePair<string, PropertyInfo>> nonIdentityColumns = columns
                .Where(c => c.Value.GetCustomAttribute<KeyAttribute>() == null || !c.Value.GetCustomAttribute<KeyAttribute>().Identity);

            return nonIdentityColumns;
        }

        private KeyValuePair<string, PropertyInfo> GetIdentityColumn(Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            KeyValuePair<string, PropertyInfo> identityColumn = columns
                .FirstOrDefault(c => c.Value.GetCustomAttribute<KeyAttribute>() != null && c.Value.GetCustomAttribute<KeyAttribute>().Identity);

            return identityColumn;
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetNonKeyColumns(Type type)
        {
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            IEnumerable<KeyValuePair<string, PropertyInfo>> nonIdentityColumns = columns
                    .Where(c => c.Value.GetCustomAttribute<KeyAttribute>() == null);

            return nonIdentityColumns;
        }
    }
}
