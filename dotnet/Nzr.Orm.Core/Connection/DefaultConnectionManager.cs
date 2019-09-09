using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Nzr.Orm.Core.Connection
{
    internal class DefaultConnectionManager : IConnectionManager
    {
        public SqlConnection Create(string connectionString) => new SqlConnection(connectionString);

        public SqlConnection Create()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            return Create(connectionString);
        }

        public SqlTransaction CreateTransaction(SqlConnection sqlConnection) => sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
    }
}
