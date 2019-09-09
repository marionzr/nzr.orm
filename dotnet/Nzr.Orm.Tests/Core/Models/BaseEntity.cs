using System;

namespace Nzr.Orm.Core.Tests.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
