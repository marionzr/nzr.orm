using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Tests.Core.Models.Audit
{
    /// <summary>
    /// This is an example of entity that maps to table that doesn't follow
    /// any of the supported naming  styles. It can represent old legacy systems
    /// or event products with a different tabla naming style.
    ///
    /// The table and column names has to be set using the TableAttribute and ColumnAttribute
    /// because the element names are complete different from the entity and property names.
    /// </summary>
    [Table("TBL_EVENT", "audit")]
    public class AuditEvent
    {
        [Key("CLN_ID", true)]
        public int Id { get; set; }
        [Column("CLN_TLB_NAME")]
        public string Table { get; set; }
        [Column("CLN_DATE")]
        public DateTime CreatedAt { get; set; }
        [Column("CLN_DATA")]
        public dynamic Data { get; set; }
    }
}
