using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Tests.Core.Models.Crm
{
    /// <summary>
    /// This entity maps to a View in the database. The behavior is almost the same as table
    /// except that only query operation supported.
    /// </summary>
    [Table("vw_active_customer")]
    public class ActiveCustomer
    {
        [Column("id_customer")]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }
    }
}
