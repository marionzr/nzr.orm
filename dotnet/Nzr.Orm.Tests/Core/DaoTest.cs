﻿using Nzr.Orm.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    [CollectionDefinition("DAO", DisableParallelization = true)]
    [Collection("Database collection")]
    public abstract class DaoTest : IDisposable
    {
        protected Options options;
        protected SqlTransaction transaction;
        private SqlConnection connection;

        protected DaoTest()
        {
            string projectDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            string dbFile = projectDirectory + @"\Core\Assets\Database\Nzr.Orm.Core.mdf";
            string connectionStrings = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True; AttachDbFilename=" + dbFile;

            options = new Options()
            {
                ConnectionStrings = connectionStrings,
                NamingStyle = NamingStyle.LowerCaseUnderlined, // default value
                Schema = "crm",
                UseComposedId = true // default value
            };

            ResetDatabase();
        }

        private void ResetDatabase()
        {
            // Clean up all tables before each test run.
            IList<string> tables = new List<string>() { "[crm].customer", "[crm].address", "[crm].city", "[crm].state", "[security].application_user", "[security].profile", "[audit].TBL_EVENT" };
            connection = new SqlConnection(options.ConnectionStrings);
            connection.Open();
            transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

            foreach (string table in tables)
            {
                string sql = $"DELETE FROM {table} WHERE 1=1";

                using (SqlCommand delete = new SqlCommand(sql, connection, transaction))
                {
                    int result = delete.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void WarmUp()
        {
            // This is just to warm up the engine and prevent the execution time of the first test from being wrongly measured.
        }

        public void Dispose()
        {
            transaction?.Commit();
            connection?.Close();
            transaction?.Dispose();
            connection?.Dispose();
        }
    }
}