using System;
using System.Collections.Generic;

namespace Nzr.Orm.Core.Extensions
{
    /// <summary>
    /// Extensions for Collections values.
    /// </summary>
    internal static class CollectionsExtensions
    {
        /// <summary>
        /// Performs an action for each element of this IDictionary.
        /// </summary>
        /// <typeparam name="K">The type of the Key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dictionary">The instance where the ForEach will be executed.</param>
        /// <param name="action">A action to be invoke for each KeyValuePair in the IDictionary.</param>
        public static void ForEach<K, V>(this IDictionary<K, V> dictionary, Action<KeyValuePair<K, V>> action)
        {
            foreach (KeyValuePair<K, V> kvp in dictionary)
            {
                action.Invoke(kvp);
            }
        }

        /// <summary>
        /// Performs an action for each element of this ICollection.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="collection">The instance where the ForEach will be executed.</param>
        /// <param name="action">A action to be invoke for each KeyValuePair in the IDictionary.</param>
        public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action.Invoke(item);
            }
        }

        /// <summary>
        /// Performs an action for each element of this IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="enumerable">The instance where the ForEach will be executed.</param>
        /// <param name="action">A action to be invoke for each KeyValuePair in the IDictionary.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action.Invoke(item);
            }
        }
    }
}