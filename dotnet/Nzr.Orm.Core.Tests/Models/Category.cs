using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Core.Tests.Models
{
    public class Category
    {
        [Key("category_id")]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
