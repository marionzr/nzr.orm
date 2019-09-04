using Nzr.Orm.Core.Attributes;

namespace Orm.Core.Tests.Models
{
    public class Product
    {
        [Key("product_id", identity: true)]
        public int Id { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        [Column("characteristics", typeName: "XML")]
        public dynamic Characteristics { get; set; }

        [NotMappedAttribute]
        public object Tag { get; set; }
    }
}
