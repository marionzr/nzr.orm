using System;
using System.Data.Common;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// OrmException
    /// </summary>
    public class OrmException : DbException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message.</param>
        public OrmException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public OrmException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
