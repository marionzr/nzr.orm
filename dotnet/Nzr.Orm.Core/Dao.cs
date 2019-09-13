using Newtonsoft.Json;
using Nzr.Orm.Core.Attributes;
using Nzr.Orm.Core.Connection;
using Nzr.Orm.Core.Extensions;
using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Encapsulates the SqlClient to performs Query and Non-Query operation as an Orm.
    /// </summary>
    public partial class Dao : IDisposable
    {
        /// <summary>
        /// The connection used in the operations.
        /// </summary>
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// The related Table Schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The naming style used for table and columns.
        /// It can be overridden by the Orm Attributes.
        /// </summary>
        public NamingStyle NamingStyle { get; set; }

        /// <summary>
        /// The Transact-SQL transaction to be made in a SQL Server database
        /// </summary>
        public SqlTransaction Transaction { get; private set; }

        /// <summary>
        /// The connection manager used to create connections and transactions.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// The options used to perform operations.
        /// </summary>
        public Options Options { get; }

        private readonly bool isConnectionOwner = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">A connection to be used by this DAO.</param>
        /// <param name="options">The options used to perform operations.</param>
        public Dao(SqlConnection connection, Options options = null) : this(options) => Connection = connection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transaction">A transaction to be used by this DAO.</param>
        /// <param name="options">The options used to perform operations.</param>
        public Dao(SqlTransaction transaction, Options options = null) : this(options)
        {
            Transaction = transaction;
            Connection = GetConnection();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionManager">A connection manager to provide connections and transactions.</param>
        /// <param name="options">The options used to perform operations.</param>
        public Dao(IConnectionManager connectionManager, Options options = null) : this(options)
        {
            isConnectionOwner = true;
            ConnectionManager = connectionManager;
            Connection = GetConnection();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The options used to perform operations.</param>
        public Dao(Options options = null)
        {
            Options = options ?? new Options();

            ConnectionManager = new DefaultConnectionManager(Options.ConnectionStrings);
            Schema = Options.Schema;
            NamingStyle = Options.NamingStyle;
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class and opens a database connection with the property settings
        /// specified by connection strings.
        /// </summary>
        private SqlConnection GetConnection()
        {
            if (Connection == null)
            {
                if (Transaction != null)
                {
                    Connection = Transaction.Connection;
                }
                else if (ConnectionManager != null)
                {
                    Connection = ConnectionManager.Create();
                }
            }

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();

                if (isConnectionOwner && Transaction == null)
                {
                    Transaction = Connection.BeginTransaction(Options.IsolationLevel);
                }
            }

            return Connection;
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void CloseConnection()
        {
            if (Connection != null && isConnectionOwner)
            {
                if (Transaction != null)
                {
                    Transaction.Commit();
                }

                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                }

                Connection.Dispose();
            }
        }

        #region Operations

        /// <summary>
        /// Inserts a new entity in the database.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>The rows affected or the identity (if enabled).</returns>
        public int Insert(object entity) => DoInsert(entity);

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(int id) => Select<T>(new object[] { id });

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(Guid id) => Select<T>(new object[] { id });

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="ids">The entity ids in the same order defined in OrmKey attributes.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(object[] ids) => DoSelect<T>(ids);

        /// <summary>
        /// Selects an entity bases on given property names and values.
        /// </summary>
        /// <param name="where">List of parameters to be used in Where clauses.</param>
        /// <param name="orderBy">List of parameters used to sort the fetched data in either ascending or descending.</param>
        /// <returns>IList with zero or more entities.</returns>
        public IList<T> Select<T>(Where where = null, OrderBy orderBy = null)
        {
            where = where ?? new Where();
            where.ReflectedType = (typeof(T));
            orderBy = orderBy ?? new OrderBy();
            orderBy.ReflectedType = where.ReflectedType;
            return DoSelect<T>(where, orderBy);
        }

        /// <summary>
        /// Updates the entity in the database using the entity primary keys as where filter.
        /// </summary>
        /// <param name="entity">The entity to updated in the database</param>
        /// <returns>The number of rows affected.</returns>
        public int Update(object entity) => DoUpdate(entity);

        /// <summary>
        /// Updates the set attributes in the database using the given a where filter.
        /// </summary>
        /// <param name="set">The properties to be set</param>
        /// <param name="where">List of parameters to be used in Where clauses.</param>
        /// <typeparam name="T">The type of the entity to be updated.</typeparam>
        /// <returns>The number of rows affected.</returns>
        public int Update<T>(Set set, Where where = null)
        {
            set.ReflectedType = (typeof(T));
            where = where ?? new Where();
            where.ReflectedType = set.ReflectedType;
            return DoUpdate<T>(set, where);
        }

        /// <summary>
        ///Deletes an entity in the database using the given entity primary keys as a where filter.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(object entity) => DoDelete(entity);

        /// <summary>
        /// Deletes an entity in the database using the given where filter.
        /// </summary>
        /// <param name="where">List of parameters to be used in Where clauses.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete<T>(Where where = null)
        {
            where = where ?? new Where();
            where.ReflectedType = (typeof(T));
            return DoDelete<T>(where);
        }

        /// <summary>
        /// Perform the calculation on a set of values to return the single scalar value.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="U">The primitive return type.</typeparam>
        /// <param name="aggregate">The aggregate information.</param>
        /// <param name="where">The where clause to filter.</param>
        /// <returns>The single scalar value</returns>
        public U Aggregate<T, U>(Aggregate aggregate, Where where = null)
        {
            where = where ?? new Where();
            where.ReflectedType = (typeof(T));
            return DoAggregate<T, U>(aggregate, where);
        }

        #endregion

        #region Internal Operations

        private dynamic ExecuteNonQuery(string sql, Parameters parameters)
        {
            dynamic result;

            using (SqlCommand command = new SqlCommand(sql, Connection, Transaction))
            {
                parameters.ForEach((parameter, value) => command.Parameters.AddWithValue(parameter, value));
                bool identityExpected = sql.Contains("output INSERTED.");

                result = identityExpected ? command.ExecuteScalar() : command.ExecuteNonQuery();
            }

            return result;
        }
        private IList<T> ExecuteQuery<T>(string sql, Parameters parameters)
        {
            IList<T> results = new List<T>();

            using (SqlCommand command = new SqlCommand(sql, Connection, Transaction))
            {
                parameters.ForEach((parameter, value) => command.Parameters.AddWithValue(parameter, value));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T entity = (T)CreateInstance(typeof(T), reader);

                        if (entity != null)
                        {
                            results.Add(entity);
                        }
                    }
                }
            }

            return results;
        }

        private U ExecuteScalar<U>(string sql, Parameters parameters)
        {
            object result = null;

            using (SqlCommand command = new SqlCommand(sql, Connection, Transaction))
            {
                parameters.ForEach((parameter, value) => command.Parameters.AddWithValue(parameter, value));
                result = command.ExecuteScalar();
            }

            if (result == DBNull.Value)
            {
                result = default(U);
            }

            return (U)Convert.ChangeType(result, typeof(U));
        }

        #endregion

        #region Bind

        private object GetValue(PropertyInfo property, object entity)
        {
            if (entity == null)
            {
                return DBNull.Value;
            }

            object value = property.GetValue(entity);

            if (value == null)
            {
                return DBNull.Value;
            }
            else if (property.PropertyType.IsPrimitive())
            {
                return value;
            }
            else if (property.PropertyType.IsEnum)
            {
                return (int)value;
            }


            ForeignKeyAttribute foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();

            if (foreignKeyAttribute != null)
            {
                PropertyInfo outerProperty = value.GetType().GetProperty(foreignKeyAttribute.JoinPropertyName);
                value = GetValue(outerProperty, value);
                return value;
            }

            throw new NotSupportedException($"Type \"{property.PropertyType}\" is not supported.");
        }

        private object CreateInstance(Type type, SqlDataReader reader)
        {
            object entity = null;
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            entity = Activator.CreateInstance(type);

            columns.ForEach(column =>
            {
                object value = ReadValue(reader, column);
                column.Value.SetValue(entity, value);
            });

            return entity;
        }

        private object ReadValue(SqlDataReader reader, KeyValuePair<string, PropertyInfo> column)
        {
            object value = TryReadValue(reader, column);

            if (value == DBNull.Value)
            {
                return null;
            }

            Type propertyType = column.Value.PropertyType;

            if (propertyType == typeof(Guid))
            {
                return (Guid)value;
            }
            else if (propertyType.IsPrimitive())
            {
                ColumnAttribute columnAttribute = column.Value.GetCustomAttribute<ColumnAttribute>();
                bool convertToDynamic = columnAttribute != null && columnAttribute.TypeName != null && "JSON|XML".Contains(columnAttribute.TypeName.ToUpper());

                value = convertToDynamic
                    ? (object)ConvertDynamicValue(columnAttribute, value.ToString())
                    : Convert.ChangeType(value, propertyType);

                return value;
            }
            else if (propertyType.IsEnum)
            {
                if (Enum.IsDefined(propertyType, value))
                {
                    return Enum.ToObject(propertyType, value);
                }

                throw new InvalidCastException($"Cannot get an enum of type {propertyType.Name} from {value}");
            }

            ForeignKeyAttribute foreignKeyAttribute = column.Value.GetCustomAttribute<ForeignKeyAttribute>();

            if (foreignKeyAttribute != null)
            {
                return CreateInstance(column.Value.PropertyType, reader);
            }


            throw new NotSupportedException($"Type \"{column.Value.PropertyType}\" is not supported.");
        }

        private object TryReadValue(SqlDataReader reader, KeyValuePair<string, PropertyInfo> column)
        {
            try
            {
                string columnName = FormatParameters(column.Key);
                return reader[columnName];
            }
            catch (Exception)
            {
                if (column.Value.GetCustomAttribute<ColumnAttribute>(true) == null)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"The property {column.Value.Name} doesn't map to valid column. To avoid this message, please use {typeof(NotMappedAttribute).Name}.", "Warning");
#endif
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        private dynamic ConvertDynamicValue(ColumnAttribute columnAttribute, string rawValue)
        {
            if (columnAttribute.TypeName.Equals("JSON", StringComparison.OrdinalIgnoreCase))
            {
                return JsonConvert.DeserializeObject<dynamic>(rawValue.ToString());
            }
            else if (columnAttribute.TypeName.Equals("XML", StringComparison.OrdinalIgnoreCase))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(rawValue.ToString());
                string json = JsonConvert.SerializeXmlNode(xmlDocument);
                return JsonConvert.DeserializeObject<dynamic>(json);
            }

            return rawValue;
        }

        #endregion

        #region Parameters

        private Where BuildWhereFromIds<T>(object[] ids) => BuildWhereFromIds(typeof(T), ids);

        private Where BuildWhereFromIds(Type type, object[] ids)
        {
            IList<PropertyInfo> keyColumns = GetKeyColumns(type).Select(c => c.Value).ToList();
            Where where = new Where
            {
                ReflectedType = (type)
            };

            for (int i = 0; i < ids.Length; i++)
            {
                where.Add(keyColumns[i].Name, Where.EQ, ids[i]);
            }

            return where;
        }

        private Where BuildWhereFromIds(object entity)
        {
            Type type = entity.GetType();
            List<PropertyInfo> keyColumns = GetKeyColumns(type).Select(c => c.Value).ToList();
            Where where = new Where
            {
                ReflectedType = (type)
            };

            for (int i = 0; i < keyColumns.Count; i++)
            {
                where.Add(keyColumns[i].Name, Where.EQ, GetValue(keyColumns[i], entity));
            }

            return where;
        }

        private IList<string> BuildWhereFilters(IEnumerable<KeyValuePair<string, PropertyInfo>> columns, Where where)
        {
            List<string> whereFilters = where.Select(w =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, where.ReflectedType, w.Item1);
                StringBuilder whereFilter = new StringBuilder($"{column.Key} {w.Item2} ");

                if (w.Item3 == null)
                {
                    whereFilter.Append("NULL");
                }
                else
                {
                    whereFilter.Append($"@{FormatParameters(column.Key)}_{w.Item4}");
                }

                return whereFilter.ToString();
            }).ToList();

            whereFilters.Add("1 = 1");

            return whereFilters;
        }

        private bool FilterColumnName(PropertyInfo propertyInfo, string name) =>
            name.Contains(".") ? $"{propertyInfo.ReflectedType.Name}.{propertyInfo.Name}" == name : propertyInfo.Name == name;

        private Parameters BuildWhereParameters(Type type, Where where, bool includeForeignKeys = false)
        {
            Parameters whereParameters = new Parameters();
            IDictionary<string, PropertyInfo> columns = GetColumns(type, includeForeignKeys);

            where.ForEach((parameter, condition, value, index) =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, where.ReflectedType, parameter);

                if (value != null)
                {
                    whereParameters.Add($"@{FormatParameters(column.Key)}_{index}", value);
                }
            });


            return whereParameters;
        }

        private KeyValuePair<string, PropertyInfo> GetColumnByPropertyName(IEnumerable<KeyValuePair<string, PropertyInfo>> columns, Type type, string name)
        {
            IEnumerable<KeyValuePair<string, PropertyInfo>> matchingColumns = columns.Where(kvp => FilterColumnName(kvp.Value, name));

            if (matchingColumns.Count() > 1)
            {
                matchingColumns = matchingColumns.Where(c => c.Value.ReflectedType == type);

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Two or more properties with the name {name} were found. The property from {type.Name} was selected\r\n" +
                    "To avoid this warning, provide the property name in the Where object using EntityTypeName.PropertyName (ex: User.Id)", "Warning");
#endif
            }

            return matchingColumns.First();
        }

        private string FormatName(string name)
        {
            switch (NamingStyle)
            {
                case NamingStyle.LowerCase:
                    return name.ToLower();
                case NamingStyle.LowerCaseUnderlined:
                    return name.AddWordSeparator('_').ToLower();
                case NamingStyle.PascalCase:
                    return name;
                case NamingStyle.PascalCaseUnderlined:
                    return name.AddWordSeparator('_');
                default:
                    return name;
            }
        }

        private static string FormatParameters(string name) => name.Replace("[", "_").Replace("]", "_").Replace(".", "_");

        #endregion

        #region Dispose

        /// <summary>
        /// Releases the connection resource.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the connection resource.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) =>
            // Cleanup
            CloseConnection();

        /// <summary>
        /// Destructor
        /// </summary>
        ~Dao()
        {
            Dispose(false);
        }

        #endregion
    }
}