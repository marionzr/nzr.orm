using System;

namespace Nzr.Orm.Core.Attributes
{
    /// <summary>
    /// Denotes that a property or class should be excluded from database mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class NotMappedAttribute : Attribute
    {
    }
}
