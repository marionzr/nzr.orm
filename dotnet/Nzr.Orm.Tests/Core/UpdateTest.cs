using Nzr.Orm.Core;
using Nzr.Orm.Core.Sql;
using Nzr.Orm.Tests.Core.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class UpdateTest : DaoTest
    {
        public UpdateTest() : base() { }

        [Fact]
        public void UpdateAllProperties_WithEntity_ShouldReturnOneAsAffectedRow()
        {
            // Arrange

            AuditEvent auditEvent = new AuditEvent()
            {
                Table = "Customer",
                Data = "email changed from a@b.com to c@b.com",
                CreatedAt = DateTime.Now
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(auditEvent);
            }

            int result;

            // Act

            auditEvent.Data = "email changed from a@b to x@y.com";

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Update(auditEvent);
            }

            // Assert

            Assert.Equal(1, result);

            using (Dao dao = new Dao(transaction, options))
            {
                AuditEvent updatedAuditEvent = dao.Select<AuditEvent>(auditEvent.Id);
                Assert.Equal(auditEvent.Data, updatedAuditEvent.Data);
                Assert.Equal(auditEvent.Table, updatedAuditEvent.Table);
            }
        }

        [Fact]
        public void UpdateSpecificProperties_WithSetAndWhere_ShouldReturnAffectedRows()
        {
            // Arrange

            AuditEvent auditEvent1 = new AuditEvent()
            {
                Table = "Customer",
                Data = "email changed from a@b.com to c@b.com",
                CreatedAt = new DateTime(1999, 1, 1)
            };

            AuditEvent auditEvent2 = new AuditEvent()
            {
                Table = "User",
                Data = "password changed from *** to *******",
                CreatedAt = new DateTime(1999, 1, 1)
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(auditEvent1);
                dao.Insert(auditEvent2);
            }

            int result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Update<AuditEvent>(new Set { { "CreatedAt", new DateTime(2019, 1, 1) } }, new Where { { "Table", Where.EQ, "User" } });
            }

            // Assert

            Assert.Equal(1, result);

            using (Dao dao = new Dao(transaction, options))
            {
                IList<AuditEvent> updatedAuditEvents = dao.Select<AuditEvent>(new Where { { "CreatedAt", Where.EQ, new DateTime(2019, 1, 1) } });
                Assert.Equal(1, updatedAuditEvents.Count);
                Assert.Equal("User", updatedAuditEvents.First().Table);
                Assert.Equal(new DateTime(2019, 1, 1), updatedAuditEvents.First().CreatedAt);
            }
        }
    }
}
