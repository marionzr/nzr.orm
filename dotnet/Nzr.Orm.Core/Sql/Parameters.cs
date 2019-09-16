using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters to be used in the Prepared Queries.
    /// </summary>
    public class Parameters : List<Tuple<string, object>>
    {
        /// <summary>
        /// Adds a new Tuple with a pair of parameter and value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public void Add(string name, object value) => Add(new Tuple<string, object>(name, value));

        /// <summary>
        /// Adds a new Tuple with a pair of parameter and value.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns>The Parameters instance as a builder pattern.</returns>
        public Parameters And(string name, object value)
        {
            Add(name, value);
            return this;
        }

        /// <summary>
        /// Performs the specified action on each set element.
        /// </summary>
        /// <param name="action">The System.Action delegate to perform on each set element.</param>
        public void ForEach(Action<string, object> action) => base.ForEach(tuple => action.Invoke(tuple.Item1, tuple.Item2));
    }
}
