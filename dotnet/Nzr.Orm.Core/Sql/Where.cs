using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters to be used in Where clauses.
    /// </summary>
    public class Where : List<Tuple<string, string, object, string>>
    {
        private int index;

        /// <summary>
        /// <![CDATA[>]]>
        /// </summary>
        public const string GT = ">";

        /// <summary>
        /// <![CDATA[>=]]>
        /// </summary>
        public const string GE = ">=";

        /// <summary>
        /// <![CDATA[<]]>
        /// </summary>
        public const string LT = "<";

        /// <summary>
        /// <![CDATA[<=]]>
        /// </summary>
        public const string LE = "<=";

        /// <summary>
        /// <![CDATA[=]]>
        /// </summary>
        public const string EQ = "=";

        /// <summary>
        /// <![CDATA[<>]]>
        /// </summary>
        public const string NE = "<>";

        /// <summary>
        /// <![CDATA[IS NOT]]>
        /// </summary>
        public const string IS_NOT = "IS NOT";

        /// <summary>
        /// <![CDATA[IS]]>
        /// </summary>
        public const string IS = "IS";

        internal Type ReflectedType { get; set; }

        /// <summary>
        /// Adds a new Tuple with a Where EQ condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public void Add(string propertyName, object value) => Add(propertyName, EQ, value);

        /// <summary>
        /// Adds a new Tuple with a Where EQ condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public Where And(string propertyName, object value)
        {
            Add(propertyName, value);
            return this;
        }

        /// <summary>
        /// Adds a new Tuple with a Where condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        public void Add(string propertyName, string condition, object value) => Add(new Tuple<string, string, object, string>(propertyName, condition, value, $"w{++index}"));

        /// <summary>
        /// Adds a new Tuple with a Where condition.
        /// </summary>
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
        /// Performs the specified action on each set element.
        /// </summary>
        /// <param name="action">The System.Action delegate to perform on each set element.</param>
        public void ForEach(Action<string, string, object, string> action) => ForEach((tuple) => action.Invoke(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
    }
}
