using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Core.Tests.Models
{
    [Table("a_mess_table")]
    public class Mess
    {
        [Key("Mess_Id", identity: true)]
        public int Id { get; set; }

        [Column("Mess_Description")]
        public string Description { get; set; }

        [Column("Mess_Detail")]
        public string Detail { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
