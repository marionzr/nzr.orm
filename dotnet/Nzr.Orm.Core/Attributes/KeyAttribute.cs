using System;

namespace Nzr.Orm.Core.Attributes
{
    /// <summary>
    /// Denotes one or more properties that uniquely identify an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : ColumnAttribute
    {
        /// <summary>
        /// Tells if the database generates a value when a row is inserted.
        /// </summary>
        public bool Identity { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="identity">If true, the database generates a value when a row is inserted.</param>
        public KeyAttribute(string name, bool identity = false) : base(name)
        {
            Identity = identity;
        }
    }
}
