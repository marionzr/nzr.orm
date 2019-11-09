using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Tests.Core.Models.Conversion
{
    public partial class MappingField
    {
        public Guid Id { get; set; }

        [ForeignKey("id_mapping_template")]
        public MappingTemplate MappingTemplate { get; set; }

        public string Name { get; set; }

        public MappingField() => Id = Guid.NewGuid();
    }
}