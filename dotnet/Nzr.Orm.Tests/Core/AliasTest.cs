using Nzr.Orm.Core;
using Nzr.Orm.Core.Tests.Models.Crm;
using Nzr.Orm.Tests.Core.Models.Audit;
using System;
using Xunit;
using static Nzr.Orm.Core.Sql.Aggregate;
using static Nzr.Orm.Core.Sql.Alias;
using static Nzr.Orm.Core.Sql.Where;

namespace Nzr.Orm.Tests.Core
{
    public class AliasTest : DaoTest
    {
        public AliasTest() : base() { }

        [Fact]
        public void NonQuery_UsingAlias_ShouldWork()
        {
            // Arrange

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(new AuditEvent() { Table = "application_user", CreatedAt = DateTime.Now });
                dao.Insert(new AuditEvent() { Table = "application_user", CreatedAt = DateTime.Now, Data = "data" });
            }

            int result1;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                // The following comment code can be simplified using using static and Alias
                // <c>result1 = dao.Update<AuditEvent>(new Set { { "Table", "profile" } }, new Where { { "Table", "application_user" }, { "Data", IS_NOT, null } }); </c>
                result1 = dao.Update<AuditEvent>(Set("Table", "profile"), Where("Table", "application_user").And("Data", IS_NOT, null));
            }

            // Assert

            Assert.Equal(1, result1);
        }

        [Fact]
        public void Query_UsingAlias_ShouldWork()
        {
            // Arrange
            State state = new State() { Name = "CA" };

            City city = new City() { Name = "Cupertino ", State = state };

            Address address = new Address()
            {
                AddressLine = "Stevens Creek Blvd",
                ZipCode = "95014",
                City = city
            };

            Customer customer1 = new Customer()
            {
                Balance = 1.00,
                Email = "sales@nzr.core.com",
                Address = address

            };

            Customer customer2 = new Customer()
            {
                Balance = 3.00,
                Email = "support@nzr.core.com",
                Address = address,
            };

            Customer customer3 = new Customer()
            {
                Balance = 5.00,
                Email = "mkt@nzr.core.com",
            };

            Customer customer4 = new Customer()
            {
                Balance = 7.00,
                Email = "admin@nzr.core.com",
                Address = address,
                Characteristics = "<bar><foo>1</foo></bar>"
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
                dao.Insert(city);
                dao.Insert(address);
                dao.Insert(customer1);
                dao.Insert(customer2);
                dao.Insert(customer3);
                dao.Insert(customer4);
            }

            double actualMaxValue;
            double actualMinValue;
            double actualAverageValues;
            double actualSumValues;
            int actualCountValues;

            // Act
            using (Dao dao = new Dao(transaction, options))
            {
                actualMaxValue = dao.Aggregate<Customer, double>(Aggregate(MAX, "Balance"), Where("Balance", LT, 7.00));
                actualMinValue = dao.Aggregate<Customer, double>(Aggregate(MIN, "Balance"), Where("Balance", GT, 1.00));
                actualAverageValues = dao.Aggregate<Customer, double>(Aggregate(AVG, "Balance"), Where("Characteristics", IS, null));
                actualSumValues = dao.Aggregate<Customer, double>(Aggregate(SUM, "Balance"), Where("Characteristics", IS, null));
                actualCountValues = dao.Aggregate<Customer, int>(Aggregate(COUNT, "Id"), Where("Characteristics", IS_NOT, null));
            }

            Assert.Equal(5D, actualMaxValue);
            Assert.Equal(3D, actualMinValue);
            Assert.Equal(3D, actualAverageValues);
            Assert.Equal(9D, actualSumValues);
            Assert.Equal(1, actualCountValues);
        }
    }
}
