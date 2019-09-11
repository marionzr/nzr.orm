using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Sql
{
    /// <summary>
    /// Represents a list of parameters to be used in Set values of update operations.
    /// </summary>
    public class Set : List<Tuple<string, object, string>>
    {
        private int index = 0;

        /// <summary>
        /// Adds a new Tuple with a pair of property and value to be updated.
        /// </summary>
        /// <param name="propertyName">The name of property.</param>
        /// <param name="value">The value to be update in the property.</param>
        public void Add(string propertyName, object value) => base.Add(new Tuple<string, object, string>(propertyName, value, $"p{++index}"));

        /// <summary>
        /// Adds a new Tuple with a pair of property and value to be updated.
        /// </summary>
        /// <param name="propertyName">The name of property.</param>
        /// <param name="value">The value to be update in the property.</param>
        public Set And(string propertyName, object value)
        {
            Add(propertyName, value);
            return this;
        }

        /// <summary>
        /// Performs the specified action on each set element.
        /// </summary>
        /// <param name="action">The System.Action delegate to perform on each set element.</param>
        public void ForEach(Action<string, object, string> action) => base.ForEach((tuple) => action.Invoke(tuple.Item1, tuple.Item2, tuple.Item3));
    }
}