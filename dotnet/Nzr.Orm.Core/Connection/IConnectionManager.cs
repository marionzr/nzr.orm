using System.Data.SqlClient;

namespace Nzr.Orm.Core.Connection
{
    /// <summary>
    /// Initializes a new instance of the System.Data.SqlClient.SqlConnection.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Initializes a new instance of the System.Data.SqlClient.SqlConnection class using the connection string defined in the App.Config.
        /// </summary>
        /// <returns></returns>
        SqlConnection Create();
    }
}
