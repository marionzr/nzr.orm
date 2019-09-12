using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters used to sort the fetched data in either ascending or descending.
    /// </summary>
    public class OrderBy : List<Tuple<string, string>>
    {
        /// <summary>
        /// ASC
        /// </summary>
        public const string ASC = "ASC";

        /// <summary>
        /// DESC
        /// </summary>
        public const string DESC = "DESC";

        internal Type ReflectedType { get; set; }

        /// <summary>
        /// Adds a new Tuple with a pair of property and the sorting.
        /// </summary>
        /// <param name="propertyName">The name of property on which to sort the query result set.</param>
        /// <param name="sorting">Specifies that the values in the specified column should be sorted in ascending or descending order.</param>
        public void Add(string propertyName, string sorting = ASC) => base.Add(new Tuple<string, string>(propertyName, sorting));

        /// <summary>
        /// Adds a new Tuple with a pair of property and the sorting.
        /// </summary>
        /// <param name="propertyName">The name of property on which to sort the query result set.</param>
        /// <param name="sorting">Specifies that the values in the specified column should be sorted in ascending or descending order.</param>
        /// <returns>The OrderBy instance as a builder pattern.</returns>
        public OrderBy And(string propertyName, string sorting = ASC)
        {
            Add(propertyName, sorting);
            return this;
        }

        /// <summary>
        /// Performs the specified action on each set element.
        /// </summary>
        /// <param name="action">The System.Action delegate to perform on each set element.</param>
        public void ForEach(Action<string, string> action) => base.ForEach((tuple) => action.Invoke(tuple.Item1, tuple.Item2));
    }
}
