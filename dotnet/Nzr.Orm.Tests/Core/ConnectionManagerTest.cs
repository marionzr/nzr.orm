using Nzr.Orm.Core;
using Nzr.Orm.Core.Connection;
using System.Data.SqlClient;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class ConnectionManagerTest : DaoTest
    {
        public ConnectionManagerTest() : base() { }

        public class TestConnectionManager : IConnectionManager
        {
            public SqlConnection Connection { get; }

            public TestConnectionManager(string connectionString) => Connection = new SqlConnection(connectionString);

            public SqlConnection Create() => Connection;
        }

        [Fact]
        public void NewDao_WithoutConnectionManager_ShouldUseDefaultConnectionManager()
        {
            // Arrange
            IConnectionManager connectionManager;

            // Act

            using (Dao dao = new Dao(options))
            {
                connectionManager = dao.ConnectionManager;
            }

            // Assert

            Assert.Equal("DefaultConnectionManager", connectionManager.GetType().Name);
        }

        [Fact]
        public void NewDao_WithConnectionManager_ShouldUseConnectionManager()
        {
            // Arrange

            TestConnectionManager connectionManager = new TestConnectionManager(connectionString);
            SqlConnection connection;

            // Act

            using (Dao dao = new Dao(connectionManager, options))
            {
                connection = dao.Connection;
            }

            // Assert

            Assert.Equal(connectionManager.Connection, connection);
        }
    }
}
