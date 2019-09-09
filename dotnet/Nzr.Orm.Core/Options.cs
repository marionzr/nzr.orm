using System.Data;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// DAO options
    /// </summary>
    public class Options
    {
        /// <summary>
        /// If true, when no ColumnAttribute is defined for a property Id
        /// then the column name will be set as id_table
        /// </summary>
        public bool UseComposedId { get; set; }

        /// <summary>
        /// The naming style to be used by the DAO.
        /// </summary>
        public NamingStyle NamingStyle { get; set; }

        /// <summary>
        /// The default table schema to be used by the DAO.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The connection strings used to create connections.
        /// </summary>
        public string ConnectionStrings { get; set; }

        /// <summary>
        /// The isolation level used in the Transactions
        /// </summary>
        public IsolationLevel IsolationLevel { get; internal set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Options()
        {
            Schema = "dbo";
            NamingStyle = NamingStyle.LowerCaseUnderlined;
            UseComposedId = true;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }

        #region Builders

        /// <summary>
        /// Sets the Schema and return this instance as a builder set style.
        /// </summary>
        /// <param name="schema">The default table schema to be used by the DAO</param>
        /// <returns>The Options instance.</returns>
        public Options WithSchema(string schema)
        {
            Schema = schema;
            return this;
        }

        #endregion
    }
}
