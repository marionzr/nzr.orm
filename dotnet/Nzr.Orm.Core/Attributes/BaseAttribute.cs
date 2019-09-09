using System;

namespace Nzr.Orm.Core.Attributes
{
    /// <summary>
    /// Represents the database element.
    /// </summary>
    public abstract class BaseAttribute : Attribute
    {
        /// <summary>
        /// The element name. It can be a table name or column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The element name.</param>
        protected BaseAttribute(string name) => Name = name;
    }
}
