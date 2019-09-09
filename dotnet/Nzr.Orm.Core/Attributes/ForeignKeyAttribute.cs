using System;

namespace Nzr.Orm.Core.Attributes
{
    /// <summary>
    /// Denotes a property used as a foreign key in a relationship.
    /// The annotation must be placed on the foreign key property of the entity that refer another entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ForeignKeyAttribute : ColumnAttribute
    {
        /// <summary>
        /// How the left (container) entity and right (foreign) will be matched.
        /// </summary>
        public JoinType Join { get; }

        /// <summary>
        /// The type of Joins between an entity and the foreign entity.
        /// </summary>
        public enum JoinType
        {
            /// <summary>
            /// The container entity will be returned by Select queries even
            /// if the column in the left entity is null or there is no matching
            /// on the right entity.
            /// </summary>
            Left,

            /// <summary>
            /// The container entity will be returned by Select queries only
            /// if there is a matching of left and right columns.
            /// </summary>
            Inner
        }

        /// <summary>
        /// The name of the Property in the foreign entity that holds
        /// the information about the column name.
        /// </summary>
        public string JoinPropertyName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="join">How the join query should be executed.</param>
        /// <param name="joinPropertyName">The name of the property in the outer entity that refers to this Fk.</param>
        public ForeignKeyAttribute(string name, JoinType join, string joinPropertyName = "Id") : base(name)
        {
            Join = join;
            JoinPropertyName = joinPropertyName;
        }
    }
}
