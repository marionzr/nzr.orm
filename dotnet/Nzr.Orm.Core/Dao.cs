using Newtonsoft.Json;
using Nzr.Orm.Core.Attributes;
using Nzr.Orm.Core.Extensions;
using Nzr.Orm.Core.Factories;
using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
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
        /// The connection factory used to create connections.
        /// </summary>
        public IConnectionFactory ConnectionFactory { get; }

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
        /// The connection strings used to create connections
        /// </summary>
        public string ConnectionStrings { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionStrings">The connection strings used to create connections.</param>
        /// <param name="schema">The related Table Schema. (Optional. Default dbo).</param>
        /// <param name="namingStyle">The naming style used for table and columns.</param>
        public Dao(string connectionStrings, string schema = "dbo", NamingStyle namingStyle = NamingStyle.LowerCaseUnderlined)
        {
            ConnectionFactory = new ConnectionFactory();
            ConnectionStrings = connectionStrings;
            Schema = schema;
            NamingStyle = namingStyle;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionFactory">The connection factory used to create connections.</param>
        /// <param name="schema">The related Table Schema. (Optional. Default dbo).</param>
        /// <param name="namingStyle">The naming style used for table and columns.</param>
        public Dao(IConnectionFactory connectionFactory, string schema = "dbo", NamingStyle namingStyle = NamingStyle.LowerCaseUnderlined)
        {
            ConnectionFactory = connectionFactory;
            Schema = schema;
            NamingStyle = namingStyle;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The connection to be used in the operations.</param>
        /// <param name="schema">The related Table Schema. (Optional. Default dbo).</param>
        /// <param name="namingStyle">The naming style used for table and columns.</param>
        public Dao(SqlConnection connection, string schema = "dbo", NamingStyle namingStyle = NamingStyle.LowerCaseUnderlined)
        {
            Connection = connection;
            Schema = schema;
            NamingStyle = namingStyle;
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class and opens a database connection with the property settings 
        /// specified by connection strings.
        /// </summary>
        private SqlConnection GetConnection()
        {
            SqlConnection connection = Connection ?? (ConnectionStrings != null ? ConnectionFactory.Create(ConnectionStrings) : ConnectionFactory.Create());

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void CloseConnection()
        {
            if (Connection != null)
            {
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
        public int Insert(object entity)
        {
            return DoInsert(entity);
        }

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(int id)
        {
            return Select<T>(new object[] { id });
        }

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(Guid id)
        {
            return Select<T>(new object[] { id });
        }

        /// <summary>
        /// Selects an entity based on its keys.
        /// </summary>
        /// <param name="ids">The entity ids in the same order defined in OrmKey attributes.</param>
        /// <returns>The entity, if found, or null.</returns>
        public T Select<T>(object[] ids)
        {
            return DoSelect<T>(ids);
        }

        /// <summary>
        /// Selects an entity bases on given property names and values.
        /// </summary>
        /// <param name="where">Dictionary of property name and the comparison value.</param>
        /// <returns>IList with zero or more entities.</returns>
        public IList<T> Select<T>(Where where)
        {
            return DoSelect<T>(where);
        }

        /// <summary>
        /// Updates the entity in the database using the entity primary keys as where filter.
        /// </summary>
        /// <param name="entity">The entity to updated in the database</param>
        /// <returns>The number of rows affected.</returns>
        public int Update(object entity)
        {
            return DoUpdate(entity);
        }

        /// <summary>
        /// Updates the set attributes in the database using the given a where filter.
        /// </summary>
        /// <param name="set">The properties to be set</param>
        /// <param name="where">The where clause to filter.</param>
        /// <typeparam name="T">The type of the entity to be updated.</typeparam>
        /// <returns>The number of rows affected.</returns>
        public int Update<T>(Set set, Where where)
        {
            return DoUpdate<T>(set, where);
        }

        /// <summary>
        ///Deletes an entity in the database using the given entity primary keys as a where filter.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(object entity)
        {
            return DoDelete(entity);
        }

        /// <summary>
        /// Deletes an entity in the database using the given where filter.
        /// </summary>
        /// <param name="where">The where clause to filter.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete<T>(Where where)
        {
            where = where ?? new Where();
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
            return DoAggregate<T, U>(aggregate, where);
        }

        #endregion

        #region Internal Operations

        private int ExecuteNonQuery(string sql, Parameters parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, GetConnection()))
            {
                parameters.ForEach((parameter, value) => command.Parameters.AddWithValue(parameter, value));

                if (sql.Contains("output INSERTED."))
                {
                    return (int)command.ExecuteScalar();
                }

                return command.ExecuteNonQuery();
            }
        }
        private IList<T> ExecuteQuery<T>(string sql, Parameters parameters)
        {
            IList<T> results = new List<T>();

            using (SqlCommand command = new SqlCommand(sql, GetConnection()))
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

        private U ExecuteScalar<T, U>(string sql, Parameters parameters)
        {
            object result = null;

            using (SqlCommand command = new SqlCommand(sql, GetConnection()))
            {
                parameters.ForEach((parameter, value) => command.Parameters.AddWithValue(parameter, value));
                result = command.ExecuteScalar();
            }

            return (U)Convert.ChangeType(result, typeof(U));
        }

        #endregion

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

        private static string FormatParameters(string name)
        {
            return name.Replace("[", "_").Replace("]", "_").Replace(".", "_");
        }

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

            if (property.PropertyType.IsPrimitive())
            {
                return value;
            }

            throw new NotImplementedException("Non-primitive types are not supported.");
        }

        private object CreateInstance(Type type, SqlDataReader reader)
        {
            object entity = null;
            IDictionary<string, PropertyInfo> columns = GetColumns(type);

            entity = Activator.CreateInstance(type);

            columns.ForEach(kvp =>
            {
                object value = ReadValue(reader, kvp);
                kvp.Value.SetValue(entity, value);
            });

            return entity;
        }

        private object ReadValue(SqlDataReader reader, KeyValuePair<string, PropertyInfo> column)
        {
            string columnName = FormatParameters(column.Key);
            object value = reader[columnName];

            if (value == DBNull.Value)
            {
                return null;
            }
            else if (column.Value.PropertyType == typeof(Guid))
            {
                return (Guid)value;
            }
            else if (column.Value.PropertyType.IsPrimitive())
            {
                ColumnAttribute columnAttribute = column.Value.GetCustomAttribute<ColumnAttribute>();

                if (columnAttribute != null && columnAttribute.TypeName != null)
                {
                    value = ConvertDynamicValue(columnAttribute, value.ToString());
                }
                else
                {
                    value = Convert.ChangeType(value, column.Value.PropertyType);
                }

                return value;
            }

            throw new NotImplementedException("Non-primitive types are not supported.");
        }

        private dynamic ConvertDynamicValue(ColumnAttribute columnAttribute, string rawValue)
        {
            if (columnAttribute.TypeName.Equals("Json", StringComparison.OrdinalIgnoreCase))
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

            throw new NotSupportedException();
        }

        #endregion

        /// <summary>
        /// Releases the connection resource.
        /// </summary>
        public void Dispose()
        {
            CloseConnection();
        }
    }
}