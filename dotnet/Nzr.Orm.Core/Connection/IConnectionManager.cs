using System.Data.SqlClient;

namespace Nzr.Orm.Core.Connection
{
    /// <summary>
    /// Initializes a new instance of the System.Data.SqlClient.SqlConnection.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class when given a string that contains the connection string.
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <returns></returns>
        SqlConnection Create(string connectionString);

        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class using the connection string defined in the App.Config.
        /// </summary>
        /// <returns></returns>
        SqlConnection Create();

        /// <summary>
        /// Creates a new transaction for the connection.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        SqlTransaction CreateTransaction(SqlConnection sqlConnection);
    }
}
