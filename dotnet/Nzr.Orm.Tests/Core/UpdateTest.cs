using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Audit;
using Nzr.Orm.Tests.Core.Models.Crm;
using System;
using System.Collections.Generic;
using Xunit;
using static Nzr.Orm.Core.Sql.Builders;
using static Nzr.Orm.Core.Sql.Where;


namespace Nzr.Orm.Tests.Core
{
    public class UpdateTest : DaoTest
    {
        public UpdateTest() : base() { }

        [Fact]
        public void Update_WithEntity_ShouldUpdateAllChangedProperties()
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
        public void Update_WithSetStatement_ShouldUpdateOnlySpecifiedProperties()
        {
            // Arrange

            AuditEvent auditEvent1 = new AuditEvent()
            {
                Table = "customer",
                Data = "email changed from a@b.com to c@b.com",
                CreatedAt = new DateTime(1999, 1, 1),
            };

            AuditEvent auditEvent2 = new AuditEvent()
            {
                Table = "user",
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
                result = dao.Update<AuditEvent>(Set("CreatedAt", new DateTime(2019, 1, 1)), Where());
            }

            // Assert

            Assert.Equal(2, result);

            using (Dao dao = new Dao(transaction, options))
            {
                IList<AuditEvent> updatedAuditEvents = dao.Select<AuditEvent>(Where("CreatedAt", EQ, new DateTime(2019, 1, 1)));
                Assert.Equal(2, updatedAuditEvents.Count);

                foreach (AuditEvent auditEvent in updatedAuditEvents)
                {
                    Assert.True(auditEvent.Table.Equals("user") || auditEvent.Table.Equals("customer"));
                    Assert.Equal(new DateTime(2019, 1, 1), auditEvent.CreatedAt);
                }
            }
        }

        [Fact]
        public void Update_WithSetAndWhere_ShouldUpdateOnlySpecifiedProperties()
        {
            // Arrange

            AuditEvent auditEvent1 = new AuditEvent()
            {
                Table = "user",
                Data = "email changed from a@b.com to c@b.com",
                CreatedAt = DateTime.Now
            };

            AuditEvent auditEvent2 = new AuditEvent()
            {
                Table = "user",
                Data = "password changed from *** to *******",
                CreatedAt = DateTime.Now
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
                result = dao.Update<AuditEvent>(Set("Table", "application_user").And("Data", null), Where("Table", EQ, "user"));
            }

            // Assert

            Assert.Equal(2, result);

            using (Dao dao = new Dao(transaction, options))
            {
                IList<AuditEvent> updatedAuditEvents = dao.Select<AuditEvent>(Where("Table", EQ, "application_user"));
                Assert.Equal(2, updatedAuditEvents.Count);

                foreach (AuditEvent auditEvent in updatedAuditEvents)
                {
                    Assert.Null(auditEvent.Data);
                }
            }
        }

        [Fact]
        public void Update_WithForeignKey_ShouldUpdateOnlyFirstLevelEntity()
        {
            // Arrange

            State state = new State() { Name = "CA" };

            City city = new City() { Name = "Cupertino", State = state };

            Address address = new Address()
            {
                AddressLine = "Stevens Creek Blvd",
                ZipCode = "95014",
                City = city
            };

            Customer customer1 = new Customer()
            {
                Balance = 1.55,
                Email = "sales@nzr.core.com",
                Address = address

            };

            Customer customer2 = new Customer()
            {
                Balance = 2.01,
                Email = "support@nzr.core.com",
                Address = address,
            };

            Customer customer3 = new Customer()
            {
                Balance = 2.01,
                Email = "mkt@nzr.core.com",
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
                dao.Insert(city);
                dao.Insert(address);
                dao.Insert(customer1);
                dao.Insert(customer2);
                dao.Insert(customer3);
            }

            // Act

            customer1.Balance = 987.65;
            customer2.Address = null;
            customer3.Address = address;

            using (Dao dao = new Dao(transaction, options))
            {
                Assert.NotNull(dao.Select<Customer>(customer1.Id));
                Assert.NotNull(dao.Select<Customer>(customer2.Id));
                Assert.Null(dao.Select<Customer>(customer3.Id)); // Customer must have a Address (inner join)

                Assert.Equal(1, dao.Update(customer1));
                Assert.Equal(1, dao.Update(customer2));
                Assert.Equal(1, dao.Update(customer3));
            }

            // Assert

            using (Dao dao = new Dao(transaction, options))
            {
                customer1 = dao.Select<Customer>(customer1.Id);
                Assert.Equal(987.65, customer1.Balance);

                customer2 = dao.Select<Customer>(customer2.Id);
                Assert.Null(customer2); // Customer must have a Address (inner join)

                customer3 = dao.Select<Customer>(customer3.Id);
                Assert.Equal("95014", customer3.Address.ZipCode);
            }
        }

        [Fact]
        public void Update_WithResultDiffExpectedResult_ShouldThrowException()
        {
            // Arrange

            AuditEvent auditEvent = new AuditEvent()
            {
                Table = "user",
                Data = "email changed from a@b.com to c@b.com",
                CreatedAt = DateTime.Now
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(auditEvent);
            }

            OrmException ex;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                ex = Assert.Throws<OrmException>(() => dao.Update<AuditEvent>(Set("Table", "application_user"), Where("Id", -1), 1));
            }

            // Assert

            Assert.NotNull(ex);
        }
    }
}
