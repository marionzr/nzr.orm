using System.Collections.Generic;

namespace Nzr.Orm.Tests.Core.Models.Crm
{
    /// <summary>
    /// This entity will be serialized to be inserted in a TEXT column.
    /// </summary>
    public class AdditionalData
    {
        public IList<AdditionalData> RelatedAdditionalData { get; set; }

        public string Name { get; set; }
        public object Value { get; set; }
    }
}
