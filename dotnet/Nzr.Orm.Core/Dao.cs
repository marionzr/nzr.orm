using Nzr.Orm.Core.Attributes;
using Nzr.Orm.Core.Connection;
using Nzr.Orm.Core.Extensions;
using Nzr.Orm.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Encapsulates the SqlClient to performs Query and Non-Query operation as an Orm.
    /// </summary>
    public partial class Dao : IDisposable
    {
        private MappingList Mappings { get; set; }

        private bool IsConnectionOwner { get; set; }

        /// <summary>
        /// The connection used in the operations.
        /// </summary>
        public DbConnection Connection { get; set; }

        /// <summary>
        /// The Transact-SQL transaction to be made in a SQL Server database
        /// </summary>
        public DbTransaction Transaction { get; private set; }

        /// <summary>
        /// The connection manager used to create connections and transactions.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// The options used to perform operations.
        /// </summary>
        public Options Options { get; private set; }

        /// <summary>
        /// Changes the default schema and returns the Dao instance as a builder setter.
        /// </summary>
        /// <param name="schema">The schema to be used in the next operations.</param>
        /// <returns>This instance.</returns>
        public Dao WithSchema(string schema)
        {
            Options.Schema = schema;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="useComposedId"></param>
        /// <returns></returns>
        public Dao WithUseComposedId(bool useComposedId)
        {
            Options.UseComposedId = useComposedId;
            return this;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">A connection to be used by this DAO.</param>
        /// <param name="options">The options with the values to be used in internal configurations.</param>
        public Dao(DbConnection connection, Options options = null)
        {
            Configure(options ?? new Options());
            Connection = connection;
            Connection = GetConnection();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transaction">A transaction to be used by this DAO.</param>
        /// <param name="options">The options with the values to be used in internal configurations.</param>
        public Dao(DbTransaction transaction, Options options = null)
        {
            Configure(options ?? new Options());
            Transaction = transaction;
            Connection = GetConnection();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionManager">A connection manager to create connections.</param>
        /// <param name="options">The options with the values to be used in internal configurations.</param>
        public Dao(IConnectionManager connectionManager, Options options = null)
        {
            Configure(options ?? new Options());
            ConnectionManager = connectionManager;
            Connection = GetConnection();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The options with the values to be used in internal configurations.</param>
        public Dao(Options options = null)
        {
            Configure(options ?? new Options());
            ConnectionManager = new DefaultConnectionManager(Options.ConnectionStrings);
            Connection = GetConnection();
        }

        /// <summary>
        /// Configures the DAO using the given Options.
        /// </summary>
        /// <param name="options">The options with the values to be used in internal configurations.</param>
        /// <returns>The DAO instance to be used in fluent code.</returns>
        public Dao Configure(Options options)
        {
            Options = new Options
            {
                AutoTrimStrings = options.AutoTrimStrings,
                ConnectionStrings = options.ConnectionStrings,
                IsolationLevel = options.IsolationLevel,
                Logger = options.Logger,
                NamingStyle = options.NamingStyle,
                Schema = options.Schema,
                UseComposedId = options.UseComposedId
            };

            Mappings = new MappingList();

            return this;
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class and opens a database connection with the property settings
        /// specified by connection strings.
        /// </summary>
        private DbConnection GetConnection()
        {
            if (Connection == null)
            {
                if (Transaction != null)
                {
                    Connection = Transaction.Connection;
                }
                else if (ConnectionManager != null)
                {
                    LogDebug("Creating a connection using ConnectionManager {ConnectionManager}.", ConnectionManager);
                    Connection = ConnectionManager.Create();
                    IsConnectionOwner = true;
                    LogInformation("Connection {Connection} created using ConnectionManager {ConnectionManager}.", Connection.GetHashCode(), ConnectionManager);
                }
                else
                {
                    string errorNoConnectionSource = "You must provide a connection, transaction, or ConnectionManager";
                    LogError(errorNoConnectionSource);
                    throw new NotSupportedException(errorNoConnectionSource);
                }
            }

            if (Connection.State != ConnectionState.Open)
            {
                LogDebug("Opening Connection {Connection}. Current state is {}.", Connection.GetHashCode(), Connection.State);
                Connection.Open();
                LogInformation("Connection {Connection} is now open.", Connection.GetHashCode());

                if (IsConnectionOwner && Transaction == null)
                {
                    LogDebug("Creating Transaction for Connection {Connection}.", Connection.GetHashCode());
                    Transaction = Connection.BeginTransaction(Options.IsolationLevel);
                    LogInformation("Transaction created for Connection {connection}.", Connection.GetHashCode());
                }
            }

            return Connection;
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void CloseConnection()
        {
            if (Connection != null && IsConnectionOwner)
            {
                LogDebug("Closing Connection {Connection}.", Connection.GetHashCode());

                if (Transaction != null)
                {
                    if (Transaction.Connection != null)
                    {
                        LogDebug("Committing Transaction for Connection {Connection}.", Connection.GetHashCode());
                        Transaction.Commit();
                        LogInformation("Transaction committed for Connection {connection}.", Connection.GetHashCode());
                    }

                    Transaction.Dispose();
                }

                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    LogInformation("Connection {Connection} is now closed.", Connection.GetHashCode());
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
        /// <param name="limit">Used to specify the number of records to return</param>
        /// <returns>IList with zero or more entities.</returns>
        public IList<T> Select<T>(Where where = null, OrderBy orderBy = null, ulong limit = ulong.MaxValue)
        {
            where = where ?? new Where();
            where.ReflectedType = (typeof(T));
            orderBy = orderBy ?? new OrderBy();
            orderBy.ReflectedType = where.ReflectedType;
            return DoSelect<T>(where, orderBy, limit);
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

        /// <summary>
        /// Executes the Transact-SQL statement and returns a list of dynamic object.
        /// </summary>
        /// <param name="sql">The Transact-SQL statement.</param>
        /// <param name="parameters">The parameters of the Transact-SQL statement</param>
        /// <returns>List of dynamic object.</returns>
        public IList<dynamic> ExecuteQuery(string sql, Parameters parameters) => DoExecuteQuery<dynamic>(sql, parameters, true);

        /// <summary>
        /// Executes the Transact-SQL statement and returns a list of dynamic object.
        /// </summary>
        /// <param name="sql">The Transact-SQL statement.</param>
        /// <param name="parameters">The parameters of the Transact-SQL statement</param>
        /// <returns>List of dynamic object.</returns>
        public int ExecuteNonQuery(string sql, Parameters parameters) => DoExecuteNonQuery(sql, parameters);

        #endregion

        #region Internal Operations

        private dynamic DoExecuteNonQuery(string sql, Parameters parameters)
        {
            try
            {
                LogOperation(sql, parameters);
                dynamic result;

                using (DbCommand command = CreateCommand(sql, parameters))
                {
                    bool identityExpected = sql.Contains("output INSERTED.");
                    result = identityExpected ? command.ExecuteScalar() : command.ExecuteNonQuery();
                }

                LogOperation(sql, parameters, result);
                return result;
            }
            catch (Exception e)
            {
                LogError(e, sql, parameters);
                Transaction?.Rollback();
                throw;
            }
            finally
            {
                Mappings.Clear();
            }
        }

        private IList<T> DoExecuteQuery<T>(string sql, Parameters parameters, bool rawQuery = false)
        {
            try
            {
                LogOperation(sql, parameters);
                IList<T> results = new List<T>();

                using (DbCommand command = CreateCommand(sql, parameters))
                using (DbDataReader reader = command.ExecuteReader())
                {
                    LogOperation(sql, parameters, results);

                    while (reader.Read())
                    {
                        if (rawQuery)
                        {
                            T entity = CreateInstance(reader);
                            results.Add(entity);
                        }
                        else
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
            catch (Exception e)
            {
                LogError(e, sql, parameters);
                Transaction?.Rollback();
                throw;
            }
            finally
            {
                Mappings.Clear();
            }
        }

        private U DoExecuteScalar<U>(string sql, Parameters parameters)
        {
            try
            {
                LogOperation(sql, parameters);
                object result = null;

                using (DbCommand command = CreateCommand(sql, parameters))
                {
                    result = command.ExecuteScalar();
                }

                LogOperation(sql, parameters, result);

                if (result == DBNull.Value)
                {
                    result = default(U);
                }

                return (U)Convert.ChangeType(result, typeof(U));
            }
            catch (Exception e)
            {
                LogError(e, sql, parameters);
                Transaction?.Rollback();
                throw;
            }
            finally
            {
                Mappings.Clear();
            }
        }

        private DbCommand CreateCommand(string sql, Parameters parameters)
        {
            DbCommand command = Connection.CreateCommand();
            command.Transaction = Transaction;
            command.CommandText = sql;

            parameters.ForEach((parameterName, value) =>
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;
                command.Parameters.Add(parameter);
            });

            return command;
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

            Type propertyType = ResolveType(property.PropertyType);

            if (propertyType.IsPrimitive())
            {
                if (propertyType == typeof(DateTime))
                {
                    ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                    if (!string.IsNullOrEmpty(columnAttribute?.TypeName) && "bigint|int".Contains(columnAttribute?.TypeName))
                    {
                        return (long)(((DateTime)value) - new DateTime(1970, 1, 1)).TotalSeconds;
                    }
                }

                return value;
            }
            else if (propertyType.IsEnum)
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

        private object CreateInstance(Type type, DbDataReader reader, string path = "\\")
        {
            object entity = Activator.CreateInstance(type);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string readerColumnName = reader.GetName(i);

                Mapping mapping = Mappings.FirstOrDefault(m => m.Path.Equals(path) && m.AliasColumnName == readerColumnName && m.EntityType == type);

                if (mapping != null)
                {
                    object value = ReadValue(reader, mapping);
                    mapping.Property.SetValue(entity, value);
                }
            }

            return entity;
        }

        private object ReadValue(DbDataReader reader, Mapping mapping)
        {
            object value = TryReadValue(reader, mapping);

            if (value == DBNull.Value || value == null)
            {
                return null;
            }

            Type propertyType = ResolveType(mapping.Property.PropertyType);

            if (propertyType == typeof(Guid))
            {
                return (Guid)value;
            }
            else if (propertyType.IsPrimitive())
            {
                value = ChangeType(value, propertyType);

                if (Options.AutoTrimStrings && value != null && (value is string))
                {
                    value = value.ToString().Trim();
                }

                return value;
            }
            else if (propertyType.IsEnum)
            {
                if (Enum.IsDefined(propertyType, value))
                {
                    return Enum.ToObject(propertyType, value);
                }

                LogError("Cannot get an Enum of type {propertyType} from {value}.", propertyType.Name, value);
                throw new InvalidCastException($"Cannot get an Enum of type {propertyType.Name} from {value}.");
            }

            ForeignKeyAttribute foreignKeyAttribute = mapping.Property.GetCustomAttribute<ForeignKeyAttribute>();

            if (foreignKeyAttribute != null)
            {
                return CreateInstance(mapping.Property.PropertyType, reader, $"{mapping.Path}{mapping.Property.Name}\\");
            }

            LogError("Type {type} is not supported.", mapping.Property.PropertyType);
            throw new NotSupportedException($"Type \"{mapping.Property.PropertyType}\" is not supported.");
        }

        private object ChangeType(object value, Type type)
        {
            if (value == null)
            {
                return null;
            }

            type = ResolveType(type);

            if (type == typeof(DateTime) && IsNumber(value))
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds((long)value);
            }

            return Convert.ChangeType(value, type);
        }

        private Type ResolveType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        private bool IsNumber(object value) =>
                       value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;

        private object TryReadValue(DbDataReader reader, Mapping mapping)
        {
            try
            {
                return reader[mapping.AliasColumnName];
            }
            catch (Exception)
            {
                DataColumn column = reader.GetSchemaTable().Columns[mapping.AliasColumnName];

                if (column == null && mapping.Property.GetCustomAttribute<ColumnAttribute>(true) == null)
                {
                    LogWarning("The property {property} doesn't map to valid column. To avoid this message, please use {NotMappedAttribute}.", mapping.Property.Name, typeof(NotMappedAttribute).Name);
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        private dynamic CreateInstance(DbDataReader reader)
        {
            IDictionary<string, object> expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                Type type = reader.GetFieldType(i);
                object rawValue = reader[i];
                object value = Convert.ChangeType(rawValue == DBNull.Value ? null : rawValue, type);

                if (Options.AutoTrimStrings && value != null && value is string)
                {
                    value = value.ToString().Trim();
                }

                expandoObject.Add(reader.GetName(i), value);
            }

            return expandoObject;
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

        private string BuildWhereFilters(IEnumerable<KeyValuePair<string, PropertyInfo>> columns, Where where)
        {
            StringBuilder whereFilterSql = new StringBuilder();
            bool firstConjunction = true;

            where.ForEach((propertyName, condition, value, index, conjunction) =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, where.ReflectedType, propertyName);
                Mapping mapping = Mappings.FirstOrDefault(m => m.EntityType == column.Value.ReflectedType && m.Property.Name == column.Value.Name);
                string columnName = mapping?.SimpleColumnName ?? column.Key;
                StringBuilder whereFilter = new StringBuilder($"{columnName} {condition} ");

                if (value == null)
                {
                    whereFilter.Append("NULL");
                }
                else
                {
                    string parameter = $"@{FormatParameters(columnName)}_{index}";

                    if (condition.Contains(Where.LIKE))
                    {
                        string likeL = value.ToString().StartsWith("%") ? "'%' + " : string.Empty;
                        string likeR = value.ToString().EndsWith("%") ? " + '%'" : string.Empty;
                        whereFilter.Append($"{likeL} {parameter} {likeR}".TrimStart());
                    }
                    else if (condition.Contains(Where.IN))
                    {
                        Array inParam = (Array)value;
                        int inParamIndex = 0;
                        List<string> parameters = inParam
                            .Cast<object>()
                            .Select(o => $"@{FormatParameters(columnName)}_{index}_{++inParamIndex}")
                            .ToList();
                        whereFilter.Append($"({string.Join(", ", parameters)})");
                    }
                    else if (condition == Where.BETWEEN)
                    {
                        whereFilter.Append($"@{FormatParameters(columnName)}_{index}_{1} AND @{FormatParameters(columnName)}_{index}_{2}");
                    }
                    else
                    {
                        whereFilter.Append(parameter);
                    }
                }

                string conjunctionFix = firstConjunction ? string.Empty : conjunction;
                firstConjunction = false;

                if (Where.OR == conjunction)
                {
                    whereFilterSql.Append($") {conjunction} ({whereFilter.ToString()}");
                }
                else
                {
                    whereFilterSql.Append($" {conjunctionFix} {whereFilter.ToString()}");
                }
            });

            if (whereFilterSql.Length == 0)
            {
                whereFilterSql.Append("(1 = 1)");
            }
            else
            {
                whereFilterSql.Insert(0, "( ");
                whereFilterSql.Append(" )");
            }

            return whereFilterSql.ToString();
        }

        private bool FilterColumnName(PropertyInfo propertyInfo, string name)
        {
            if (!name.Contains("."))
            {
                return propertyInfo.Name == name;
            }
            else
            {
                string[] names = name.Split(".");

                if (propertyInfo.Name == names[0])
                {
                    propertyInfo = propertyInfo.PropertyType.GetProperty(names[1]);

                    if (propertyInfo != null)
                    {
                        List<string> newNames = new List<string>();

                        for (int i = 1; i < names.Length; i++)
                        {
                            newNames.Add(names[i]);
                        }

                        return FilterColumnName(propertyInfo, string.Join(".", newNames));
                    }
                }

                return false;
            }
        }

        private Parameters BuildWhereParameters(Type type, Where where, bool includeForeignKeys = false)
        {
            Parameters whereParameters = new Parameters();
            IList<KeyValuePair<string, PropertyInfo>> columns = GetColumns(type, includeForeignKeys);

            where.ForEach((parameter, condition, value, index, conjunction) =>
            {
                KeyValuePair<string, PropertyInfo> column = GetColumnByPropertyName(columns, where.ReflectedType, parameter);

                if (value != null)
                {
                    if (value is string && condition.Contains(Where.LIKE))
                    {
                        value = value.ToString().Replace("%", string.Empty);
                    }

                    Mapping mapping = Mappings.FirstOrDefault(m => m.EntityType == column.Value.ReflectedType && m.Property.Name == column.Value.Name);
                    string columnName = mapping?.SimpleColumnName ?? column.Key;

                    if (condition.Contains(Where.IN))
                    {
                        Array inParam = (Array)value;
                        int inParamIndex = 0;
                        inParam
                            .Cast<object>()
                            .ForEach(o => whereParameters.Add($"@{FormatParameters(columnName)}_{index}_{++inParamIndex}", o));
                    }
                    else if (condition == Where.BETWEEN)
                    {
                        Array between = (Array)value;
                        whereParameters.Add($"@{FormatParameters(columnName)}_{index}_{1}", between.GetValue(0));
                        whereParameters.Add($"@{FormatParameters(columnName)}_{index}_{2}", between.GetValue(1));
                    }
                    else
                    {
                        whereParameters.Add($"@{FormatParameters(columnName)}_{index}", value);
                    }
                }
            });


            return whereParameters;
        }

        private KeyValuePair<string, PropertyInfo> GetColumnByPropertyName(IEnumerable<KeyValuePair<string, PropertyInfo>> columns, Type type, string name)
        {
            if (Mappings.Count > 0 && name.Contains("."))
            {
                IEnumerable<Mapping> matchingColumns = Mappings.Where(m =>
                {
                    string match = m.Path + m.Property.Name;
                    string search = $"\\{name.Replace(".", "\\")}";
                    return match == search;
                }).ToList();

                if (matchingColumns.Count() > 1)
                {
                    matchingColumns = matchingColumns.Where(m => m.Property.ReflectedType == type).ToList();
                    LogWarning("Two or more properties with the name {name} were found. The property from {type.Name} was selected. To avoid this warning, provide the property name in the Where object using EntityTypeName.PropertyName (ex: User.Id).", "Warning", name, type.Name);
                }

                Mapping column = matchingColumns.FirstOrDefault();

                if (column == null)
                {
                    LogError("No property was found with the name {name}.", name);
                    throw new ArgumentException($"No property was found with the name {name}.");

                }

                return new KeyValuePair<string, PropertyInfo>(column.FullColumnName, column.Property);
            }
            else if (!name.Contains("."))
            {
                IList<KeyValuePair<string, PropertyInfo>> matchingColumns = columns.Where(kvp =>
                 {
                     bool result = FilterColumnName(kvp.Value, name);
                     return result;
                 }).ToList();

                if (matchingColumns.Count() > 1)
                {
                    matchingColumns = matchingColumns.Where(c => c.Value.ReflectedType == type).ToList();
                    LogWarning("Two or more properties with the name {name} were found. The property from {type.Name} was selected. To avoid this warning, provide the property name in the Where object using EntityTypeName.PropertyName (ex: User.Id).", "Warning", name, type.Name);
                }

                KeyValuePair<string, PropertyInfo> column = matchingColumns.FirstOrDefault();

                if (column.Key == null)
                {
                    LogError("No property was found with the name {name}.", name);
                    throw new ArgumentException($"No property was found with the name {name}.");

                }

                return column;
            }
            else
            {
                throw new NotSupportedException($"Nested properties ({name}) are not supported for this operation.");
            }
        }

        private string FormatName(string name)
        {
            switch (Options.NamingStyle)
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

        private class MappingList : List<Mapping>
        {
            public List<string> Paths { get; }
            public int Index { get; set; }

            public MappingList() : base() => Paths = new List<string>();

            public new void Clear()
            {
                base.Clear();
                Paths.Clear();
                Index = 0;
            }
        }


        /// <summary>
        /// Holds information about the mapping used while building queries.
        /// </summary>
        private class Mapping
        {
            /// <summary>
            /// <code>[dbo].[table]</code>
            /// </summary>
            public string FullTableName { get; set; }

            /// <summary>
            /// <code>a1 -> [dbo].[table] AS a1</code>
            /// </summary>
            public string AliasTableName { get; set; }

            /// <summary>
            /// <code>[schema].[table].[column]</code>
            /// </summary>
            public string FullColumnName { get; set; }

            /// <summary>
            /// <code>[column]</code>
            /// </summary>
            public string SimpleColumnName { get; set; }

            /// <summary>
            /// <code>a1_column -> a1.columnas AS a1_column</code>
            /// </summary>
            public string AliasColumnName { get; set; }

            /// <summary>
            /// The type of the entity that has the properties.
            /// </summary>
            public Type EntityType { get; set; }

            /// <summary>
            /// The mapped property.
            /// </summary>
            public PropertyInfo Property { get; set; }

            /// <summary>
            /// The table index.
            /// </summary>
            public int TableIndex { get; set; }

            /// <summary>
            /// The type of the entity that holds the EntityType in this mapping.
            /// </summary>
            public Type ParentEntityType { get; set; }

            /// <summary>
            /// The Path of the Property.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// ParentEntityType | EntityType | Property | FullColumnName | AliasColumnName.
            /// </summary>
            /// <returns>An string with some properties of this mapping.</returns>
            public override string ToString() => $"Path: {Path} | ParentEntityType: {ParentEntityType?.Name ?? "-"} | EntityType: {EntityType.Name} | Property: {Property.Name} | FullColumnName: {FullColumnName} | AliasColumnName: {AliasColumnName}";
        }
    }
}