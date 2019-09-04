using System.Configuration;
using System.Data.SqlClient;

namespace Nzr.Orm.Core.Factories
{
    /// <summary>
    /// Initializes a new instance of the System.Data.SqlClient.SqlConnection.
    /// </summary>
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class when given a string that contains the connection string.
        /// </summary>
        /// <param name="connectionStrings">The connection used to open the SQL Server database.</param>
        /// <returns></returns>
        public SqlConnection Create(string connectionStrings)
        {
            return new SqlConnection(connectionStrings);
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class using the connection string defined in the App.Config.
        /// </summary>
        /// <returns></returns>
        public SqlConnection Create()
        {
            ConnectionStringSettings connectionStrings = ConfigurationManager.ConnectionStrings[0];
            return Create(connectionStrings.ConnectionString);
        }
    }
}
