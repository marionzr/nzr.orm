using System;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters to be used in Where clauses.
    /// </summary>
    public class Aggregate : Tuple<string, string>
    {
        /// <summary>
        /// <![CDATA[AVG]]>
        /// </summary>
        public const string AVG = "AVG";

        /// <summary>
        /// <![CDATA[COUNT]]>
        /// </summary>
        public const string COUNT = "COUNT";

        /// <summary>
        /// <![CDATA[MAX]]>
        /// </summary>
        public const string MAX = "MAX";

        /// <summary>
        /// <![CDATA[MIN]]>
        /// </summary>
        public const string MIN = "MIN";

        /// <summary>
        /// <![CDATA[SUM]]>
        /// </summary>
        public const string SUM = "SUM";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregate">The aggregate function name (Ex. sum, max, min).</param>
        /// <param name="property">The property that will be aggregated.</param>
        public Aggregate(string aggregate, string property)
            : base(aggregate, property)
        {
        }
    }
}