using Nzr.Orm.Core;
using Nzr.Orm.Core.Sql;
using Nzr.Orm.Tests.Core.Models.Crm;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class AggregateTest : DaoTest
    {
        public AggregateTest() : base() { }

        [Fact]
        public void Aggregate_WithoutWhereClauseShouldReturnCalculatedValuesForAllEntities()
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
                actualMaxValue = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.MAX, "Balance"));
                actualMinValue = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.MIN, "Balance"));
                actualAverageValues = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.AVG, "Balance"));
                actualSumValues = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.SUM, "Balance"));
                actualCountValues = dao.Aggregate<Customer, int>(new Aggregate(Aggregate.COUNT, "Id"));
            }

            Assert.Equal(7D, actualMaxValue);
            Assert.Equal(1D, actualMinValue);
            Assert.Equal(4D, actualAverageValues);
            Assert.Equal(16D, actualSumValues);
            Assert.Equal(4, actualCountValues);
        }

        [Fact]
        public void Aggregate_WithWhereClause_ShouldReturnCalculatedValuesForSpecificEntities()
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
                actualMaxValue = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.MAX, "Balance"), new Where { { "Balance", Where.LT, 7.00 } });
                actualMinValue = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.MIN, "Balance"), new Where { { "Balance", Where.GT, 1.00 } });
                actualAverageValues = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.AVG, "Balance"), new Where { { "Characteristics", Where.IS, null } });
                actualSumValues = dao.Aggregate<Customer, double>(new Aggregate(Aggregate.SUM, "Balance"), new Where { { "Characteristics", Where.IS, null } });
                actualCountValues = dao.Aggregate<Customer, int>(new Aggregate(Aggregate.COUNT, "Id"), new Where { { "Characteristics", Where.IS_NOT, null } });
            }

            Assert.Equal(5D, actualMaxValue);
            Assert.Equal(3D, actualMinValue);
            Assert.Equal(3D, actualAverageValues);
            Assert.Equal(9D, actualSumValues);
            Assert.Equal(1, actualCountValues);
        }
    }
}
