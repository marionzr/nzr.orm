using Nzr.Orm.Core;
using Nzr.Orm.Core.Sql;
using Nzr.Orm.Tests.Core.Models.Crm;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class SelectTest : DaoTest
    {
        public SelectTest() : base() { }

        [Fact]
        public void Select_WithId_ShouldReturnSingleEntity()
        {
            // Arrange

            State state = new State() { Name = "CA" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
            }

            State expectedState;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                expectedState = dao.Select<State>(state.Id);
            }

            // Assert

            Assert.NotNull(expectedState);
            Assert.Equal(state.Id, expectedState.Id);
            Assert.Equal(state.Name, expectedState.Name);
        }

        [Fact]
        public void Select_WithCustomWhere_ShouldReturnListOfEntity()
        {
            // Arrange

            List<string> stateNames = new List<string>() { "CA", "WA" };

            using (Dao dao = new Dao(transaction, options))
            {
                foreach (string stateName in stateNames)
                {
                    dao.Insert(new State() { Name = stateName });
                }
            }

            IList<State> resultNeNY;
            IList<State> resultEqCA;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultNeNY = dao.Select<State>(new Where { { "Name", Where.NE, "NY" } });
                resultEqCA = dao.Select<State>(new Where { { "Name", Where.EQ, "CA" } });
            }

            // Assert

            Assert.Equal(2, resultNeNY.Count);

            foreach (State state in resultNeNY)
            {
                Assert.Contains<string>(state.Name, stateNames);
            }

            Assert.Equal(1, resultEqCA.Count);
            Assert.Equal("CA", resultEqCA.First().Name);
        }

        [Fact]
        public void Select_ReferencingEntities_ShouldReturnCompleteEntities()
        {
            // Arrange

            using (Dao dao = new Dao(transaction, options))
            {
                State state = new State() { Name = "CA" };
                dao.Insert(state);

                dao.Insert(new City() { Name = "Cupertino", State = state });
                dao.Insert(new City() { Name = "Monterey", State = state });
            }

            IList<City> result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Select<City>(new Where { { "State.Name", Where.EQ, "CA" } });
            }

            // Assert

            Assert.Equal(2, result.Count);

            foreach (City city in result)
            {
                Assert.NotNull(city.State);
                Assert.Equal("CA", city.State.Name);
            }
        }

        [Fact]
        public void Select_WithInnerJoinEntitiesByGivingForeignId_ShouldReturnCompleteEntities()
        {
            // Arrange

            State state1 = new State() { Name = "CA" };
            State state2 = new State() { Name = "FL" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state1);
                dao.Insert(state2);

                dao.Insert(new City() { Name = "Cupertino", State = state1 });
                dao.Insert(new City() { Name = "Miami", State = state2 });
            }

            IList<City> result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Select<City>(new Where { { "State.Id", Where.EQ, state2.Id } });
            }

            // Assert

            Assert.Equal(1, result.Count);

            Assert.Equal("Miami", result.First().Name);
            Assert.Equal("FL", result.First().State.Name);
        }


        [Fact]
        public void Select_WithLeftJoinEntities_ShouldReturnAllCompleteEntities()
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

            IList<Customer> result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Select<Customer>(new Where { { "Balance", Where.GT, 0.99 }, { "Characteristics", Where.IS, null } });
            }

            // Assert

            // In the database, there was no constraints to null Billing Address (id_address_billing) but, the
            // Customer class defined the property BillingAddress with the ForeignKey attribute using Inner Join type
            // So, only customers 1 and 2 should be returned.
            Assert.Equal(2, result.Count);

            Assert.Null(result.FirstOrDefault(c => c.Email == "mkt@nzr.core.com"));
            Assert.Null(result.FirstOrDefault(c => c.Email == "admin@nzr.core.com"));

            foreach (Customer customer in result)
            {
                Assert.NotNull(customer.Address);
                Assert.Equal("Stevens Creek Blvd", customer.Address.AddressLine);
            }
        }

        [Fact]
        public void Select_WithLeftJoinEntitiesWithOrderBy_ShouldReturnAllCompleteEntitiesInTheSpecifiedOrder()
        {
            // Arrange
            State state = new State() { Name = "CA" };

            City city = new City() { Name = "Cupertino ", State = state };

            Address address1 = new Address()
            {
                AddressLine = "Stevens Creek Blvd",
                ZipCode = "95014",
                City = city
            };

            Customer customer1 = new Customer()
            {
                Balance = 9.55,
                Email = "sales@nzr.core.com",
                Address = address1

            };

            Address address2 = new Address()
            {
                AddressLine = "Bubb Rd",
                ZipCode = "95014",
                City = city
            };

            Customer customer2 = new Customer()
            {
                Balance = 2.01,
                Email = "support@nzr.core.com",
                Address = address2,
            };

            Customer customer3 = new Customer()
            {
                Balance = 0.01,
                Email = "mkt@nzr.core.com",
            };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
                dao.Insert(city);
                dao.Insert(address1);
                dao.Insert(address2);
                dao.Insert(customer1);
                dao.Insert(customer2);
                dao.Insert(customer3);
            }

            IList<Customer> resultOrderBalance;
            IList<Customer> resultOrderAddress;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultOrderBalance = dao.Select<Customer>(new Where { { "Balance", Where.GT, 0.01 } }, new OrderBy { { "Balance", OrderBy.DESC } });
                resultOrderAddress = dao.Select<Customer>(new Where { { "Balance", Where.GT, 0.01 } }, new OrderBy { { "Address.AddressLine" } });
            }

            // Assert

            Assert.Equal(2, resultOrderBalance.Count);
            Assert.Equal(2, resultOrderAddress.Count);

            Assert.Equal("sales@nzr.core.com", resultOrderBalance.First().Email);
            Assert.Equal("support@nzr.core.com", resultOrderAddress.First().Email);
        }
    }
}
