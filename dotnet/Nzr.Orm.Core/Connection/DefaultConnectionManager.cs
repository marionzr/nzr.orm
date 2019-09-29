using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Nzr.Orm.Core.Connection
{
    internal class DefaultConnectionManager : IConnectionManager
    {
        public string ConnectionString { get; }

        public DefaultConnectionManager(string connectionString = null) => ConnectionString = connectionString;

        public DbConnection Create()
        {
            string connectionString = ConnectionString ?? ConfigurationManager.ConnectionStrings[0].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}
