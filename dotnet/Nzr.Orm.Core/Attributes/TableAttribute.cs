using System;

namespace Nzr.Orm.Core.Attributes
{
    /// <summary>
    /// Specifies the table that a class is mapped to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : BaseAttribute
    {
        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The schema of the table.</param>
        public TableAttribute(string name = null, string schema = null) : base(name) => Schema = schema;
    }
}
