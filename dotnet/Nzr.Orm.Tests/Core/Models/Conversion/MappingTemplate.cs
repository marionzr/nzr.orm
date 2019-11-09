using System;

namespace Nzr.Orm.Tests.Core.Models.Conversion
{
    public partial class MappingTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public MappingTemplate() => Id = Guid.NewGuid();
    }
}