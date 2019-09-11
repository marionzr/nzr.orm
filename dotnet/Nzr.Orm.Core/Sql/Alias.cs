namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Alias to common Set, Where and Aggregate usages.
    /// </summary>
    public static class Alias
    {
        /// <summary>
        /// Creates a single pair of property and value to be updated.
        /// </summary>
        /// <param name="propertyName">The name of property.</param>
        /// <param name="value">The value to be update in the property.</param>
        public static Set Set(string propertyName, object value) => new Set() { { propertyName, value } };

        /// <summary>
        /// Creates single a where clause with a single EQ Where condition.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="value">The value used in the filter.</param>
        public static Where Where(string propertyName, object value) => new Where() { { propertyName, Nzr.Orm.Core.Sql.Where.EQ, value } };

        /// <summary>
        /// Creates single a where clause.
        /// </summary>
        /// <param name="propertyName">The property name that will be used in the where clause.</param>
        /// <param name="condition">The filter condition (e.g. >, =, IS NOT)</param>
        /// <param name="value">The value used in the filter.</param>
        public static Where Where(string propertyName, string condition, object value) => new Where() { { propertyName, condition, value } };

        /// <summary>
        /// Creates single an aggregate clause.
        /// </summary>
        /// <param name="aggregate">The aggregate function name (Ex. sum, max, min).</param>
        /// <param name="property">The property that will be aggregated.</param>
        public static Aggregate Aggregate(string aggregate, string property) => new Aggregate(aggregate, property);
    }
}
