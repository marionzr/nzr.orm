using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters to be used in Where clauses.
    /// </summary>
    public class Where : List<Tuple<string, string, object, string, string>>
    {
        private int index;

        #region Constants

        internal const string AND = "AND";
        internal const string OR = "OR";

        /// <summary>
        /// Greater than.
        /// </summary>
        public const string GT = ">";

        /// <summary>
        /// Greater than or equal to.
        /// </summary>
        public const string GE = ">=";

        /// <summary>
        /// Less than.
        /// </summary>
        public const string LT = "<";

        /// <summary>
        /// Less than or equal to.
        /// </summary>
        public const string LE = "<=";

        /// <summary>
        /// Equal to.
        /// </summary>
        public const string EQ = "=";

        /// <summary>
        /// Not equal to.
        /// </summary>
        public const string NE = "<>";

        /// <summary>
        /// <![CDATA[IS]]>
        /// </summary>
        public const string IS = "IS";

        /// <summary>
        /// <![CDATA[IS NOT]]>
        /// </summary>
        public const string IS_NOT = "IS NOT";

        /// <summary>
        /// Used to check if the operand matches a pattern.
        /// </summary>
        public const string LIKE = "LIKE";

        /// <summary>
        /// Used to check if the operand does not matches a pattern.
        /// </summary>
        public const string NOT_LIKE = "NOT " + LIKE;

        /// <summary>
        /// Used to check if the operand is equal to one of a list of expressions.
        /// </summary>
        public const string IN = "IN";

        /// <summary>
        /// Used to check if the operand is not equal to one of a list of expressions.
        /// </summary>
        public const string NOT_IN = "NOT IN";

        /// <summary>
        /// Used to check if the operand is within the range of comparisons
        /// </summary>
        public const string BETWEEN = "BETWEEN";

        #endregion

        internal Type ReflectedType { get; set; }

        /// <summary>
        /// Adds a new Tuple with a Where EQ condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public void Add(string propertyName, object value) => Add(propertyName, EQ, value);

        /// <summary>
        /// Adds a new Tuple with a Where condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        /// <param name="conjunction">used to indicate that one or more of the condition it connects may occur.</param>
        public void Add(string propertyName, string condition, object value, string conjunction = AND) => Add(new Tuple<string, string, object, string, string>(propertyName, condition, value, $"w{++index}", conjunction));

        /// <summary>
        /// Adds a new Tuple with a Where EQ condition in an AND conjunction..
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public Where And(string propertyName, object value)
        {
            And(propertyName, EQ, value);
            return this;
        }

        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        /// <returns>The Where instance as a builder pattern.</returns>
        public Where And(string propertyName, string condition, object value)
        {
            Add(propertyName, condition, value);
            return this;
        }

        /// <summary>
        /// Adds a new Tuple with a Where EQ condition in an OR conjunction.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public Where Or(string propertyName, object value)
        {
            Or(propertyName, EQ, value);
            return this;
        }

        /// <summary>
        /// Adds a new Tuple with a Where condition in an OR conjunction..
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        /// <returns>The Where instance as a builder pattern.</returns>
        public Where Or(string propertyName, string condition, object value)
        {
            Add(propertyName, condition, value, OR);
            return this;
        }

        /// <summary>
        /// Performs the specified action on each set element.
        /// </summary>
        /// <param name="action">The System.Action delegate to perform on each set element.</param>
        public void ForEach(Action<string, string, object, string, string> action) => ForEach((tuple) => action.Invoke(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5));
    }
}
