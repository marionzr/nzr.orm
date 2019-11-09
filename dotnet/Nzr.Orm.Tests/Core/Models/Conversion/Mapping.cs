using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Tests.Core.Models.Conversion
{
    public partial class Mapping
    {
        public Guid Id { get; set; }

        [ForeignKey("id_mapping_field_source")]
        public MappingField MappingFieldSource { get; set; }

        [ForeignKey("id_mapping_field_dest")]
        public MappingField MappingFieldDest { get; set; }

        public Mapping() => Id = Guid.NewGuid();
    }
}