using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to exceptions
    /// </summary>
    public partial class Dao
    {
        /// <summary>
        /// Checks if the Exception is due an unique key violation.
        /// </summary>
        /// <param name="ex">Exception to be checked.</param>
        /// <returns>True, if the Exception is due an unique key violation.</returns>
        public bool IsUniqueKeyViolation(Exception ex) => ex is SqlException && IsUniqueKeyViolation((DbException)ex);

        /// <summary>
        /// Checks if the DbException is due an unique key violation
        /// </summary>
        /// <param name="ex">DbException to be checked.</param>
        /// <returns>True, if the Exception is due an unique key violation.</returns>
        public bool IsUniqueKeyViolation(DbException ex) => ex is SqlException && IsUniqueKeyViolation((SqlException)ex);

        private bool IsUniqueKeyViolation(SqlException ex) => ex.Errors.Cast<SqlError>().Any(e => e.Class == 14 && (e.Number == 2601 || e.Number == 2627));
    }
}
