using System;

namespace Nzr.Orm.Core.Extensions
{
    /// <summary>
    /// Extensions for System.Type values.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if the type is a primitive type or a system type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>True if the type is primitive or is a System type.</returns>
        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive || type.FullName.StartsWith("System.");
        }
    }
}
