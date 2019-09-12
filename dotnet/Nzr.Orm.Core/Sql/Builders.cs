namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Alias to common Set, Where and Aggregate usages.
    /// </summary>
    public static class Builders
    {
        /// <summary>
        /// Creates a single pair of property and value to be updated.
        /// </summary>
        /// <param name="propertyName">The name of property.</param>
        /// <param name="value">The value to be update in the property.</param>
        /// <returns>The Set instance as a builder pattern.</returns>
        public static Set Set(string propertyName, object value) => new Set() { { propertyName, value } };

        /// <summary>
        /// Creates single a empty where clause.
        /// </summary>
        /// <returns>The Where instance as a builder pattern.</returns>
        public static Where Where() => new Where();

        /// <summary>
        /// Creates single a where clause with a single EQ Where condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        /// <returns>The Where instance as a builder pattern.</returns>
        public static Where Where(string propertyName, object value) => new Where() { { propertyName, Nzr.Orm.Core.Sql.Where.EQ, value } };

        /// <summary>
        /// Creates single a where clause.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        /// <returns>The Where instance as a builder pattern.</returns>
        public static Where Where(string propertyName, string condition, object value) => new Where() { { propertyName, condition, value } };

        /// <summary>
        /// Creates single an aggregate clause.
        /// </summary>
        /// <param name="aggregate">The aggregate function name (Ex. sum, max, min).</param>
        /// <param name="property">The property that will be aggregated.</param>
        /// <returns>The Aggregate instance as a builder pattern.</returns>
        public static Aggregate Aggregate(string aggregate, string property) => new Aggregate(aggregate, property);

        /// <summary>
        /// Adds a new Tuple with a pair of property and the sorting.
        /// </summary>
        /// <param name="propertyName">The name of property on which to sort the query result set.</param>
        /// <param name="sorting">Specifies that the values in the specified column should be sorted in ascending or descending order.</param>
        /// <returns>The OrderBy instance as a builder pattern.</returns>
        public static OrderBy OrderBy(string propertyName, string sorting = Nzr.Orm.Core.Sql.OrderBy.ASC) => new OrderBy() { { propertyName, sorting } };
    }
}
