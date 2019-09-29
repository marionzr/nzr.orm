using Nzr.Orm.Core;
using Nzr.Orm.Core.Sql;
using Nzr.Orm.Tests.Core.Models.Audit;
using Nzr.Orm.Tests.Core.Models.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Nzr.Orm.Core.Sql.Builders;
using static Nzr.Orm.Core.Sql.OrderBy;
using static Nzr.Orm.Core.Sql.Where;

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
                resultNeNY = dao.Select<State>(new Where { { "Name", NE, "NY" } });
                resultEqCA = dao.Select<State>(new Where { { "Name", EQ, "CA" } });
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
                result = dao.Select<City>(new Where { { "State.Name", EQ, "CA" } });
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
                result = dao.Select<City>(new Where { { "State.Id", EQ, state2.Id } });
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

            IList<Customer> result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Select<Customer>(new Where { { "Balance", GT, 0.99 }, { "Characteristics", IS, null } });
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

            City city = new City() { Name = "Cupertino", State = state };

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
                resultOrderBalance = dao.Select<Customer>(new Where { { "Balance", GT, 0.01 } }, new OrderBy { { "Balance", DESC } });
                resultOrderAddress = dao.Select<Customer>(new Where { { "Balance", GT, 0.01 } }, new OrderBy { { "Address.AddressLine" } });
            }

            // Assert

            Assert.Equal(2, resultOrderBalance.Count);
            Assert.Equal(2, resultOrderAddress.Count);

            Assert.Equal("sales@nzr.core.com", resultOrderBalance.First().Email);
            Assert.Equal("support@nzr.core.com", resultOrderAddress.First().Email);
        }

        [Fact]
        public void Select_UsingLike_ShouldReturnEntitiesBaseOnSubstrings()
        {
            // Arrange

            State state = new State() { Name = "CA" };

            City city1 = new City() { Name = "ABC", State = state };
            City city2 = new City() { Name = "AXC", State = state };
            City city3 = new City() { Name = "XBC", State = state };
            City city4 = new City() { Name = "ABX", State = state };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
                dao.Insert(city1);
                dao.Insert(city2);
                dao.Insert(city3);
                dao.Insert(city4);
            }

            IList<City> resultLike;
            IList<City> resultLikeL;
            IList<City> resultLikeR;
            IList<City> resultNotLike;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultLike = dao.Select<City>(Where("Name", LIKE, "%X%"));
                resultLikeL = dao.Select<City>(Where("Name", LIKE, "%X"));
                resultLikeR = dao.Select<City>(Where("Name", LIKE, "X%"));
                resultNotLike = dao.Select<City>(Where("Name", NOT_LIKE, "%X%"));
            }

            // Assert

            Assert.False(resultLike.Any(c => c.Name == "ABC"));
            Assert.Equal("ABX", resultLikeL.First().Name);
            Assert.Equal("XBC", resultLikeR.First().Name);
            Assert.Equal("ABC", resultNotLike.First().Name);
        }

        [Fact]
        public void Select_WithInClause_ShouldReturnEntitiesWithPropertyValuesInRange()
        {
            // Arrange
            State state = new State() { Name = "CA" };

            City city = new City() { Name = "Cupertino", State = state };

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
                ZipCode = "95015",
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
                Address = address2,
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

            IList<Customer> resultBalanceIn;
            IList<Customer> resultZipCodeIn;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultBalanceIn = dao.Select<Customer>(Where("Balance", IN, new double[] { 2.01, 0.01, 111.1 }));
                resultZipCodeIn = dao.Select<Customer>(Where("ZipCode", NOT_IN, new string[] { "95015", "95016" }));
            }

            // Assert

            Assert.Equal(2, resultBalanceIn.Count);
            Assert.Equal(1, resultZipCodeIn.Count);
        }

        [Fact]
        public void Select_WithAndOrCondition_ShouldReturnEntitiesWithOneOrOtherConditions()
        {
            // Arrange

            State state1 = new State() { Name = "CA" };
            State state2 = new State() { Name = "WA" };
            State state3 = new State() { Name = "NY" };
            State state4 = new State() { Name = "AL" };
            State state5 = new State() { Name = "CD" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state1);
                dao.Insert(state2);
                dao.Insert(state3);
                dao.Insert(state4);
                dao.Insert(state5);
            }

            // Act

            IList<State> result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Select<State>(Where("Name", IN, new string[] { "CA", "WA", "CO" }).Or("Name", "CD"));
            }

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Select_WithLimit_ShouldReturnEntitiesUpToLimit()
        {
            // Arrange

            State state1 = new State() { Name = "CA" };
            State state2 = new State() { Name = "WA" };
            State state3 = new State() { Name = "NY" };
            State state4 = new State() { Name = "AL" };
            State state5 = new State() { Name = "CD" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state1);
                dao.Insert(state2);
                dao.Insert(state3);
                dao.Insert(state4);
                dao.Insert(state5);
            }

            // Act

            IList<State> resultWithLimit;
            IList<State> resultWithoutLimit;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultWithLimit = dao.Select<State>(limit: 2);
                resultWithoutLimit = dao.Select<State>();
            }

            Assert.Equal(2, resultWithLimit.Count);
            Assert.Equal(5, resultWithoutLimit.Count);
        }

        [Fact]
        public void Select_WithBetweenClause_ShouldReturnEntitiesWithPropertyValuesInRange()
        {
            // Arrange
            State state = new State() { Name = "CA" };

            City city = new City() { Name = "Cupertino", State = state };

            Address address1 = new Address()
            {
                AddressLine = "Stevens Creek Blvd",
                ZipCode = "95014",
                City = city
            };

            Customer customer1 = new Customer()
            {
                Balance = 1.00,
                Email = "sales@nzr.core.com",
                Address = address1

            };

            Address address2 = new Address()
            {
                AddressLine = "Bubb Rd",
                ZipCode = "95015",
                City = city
            };

            Customer customer2 = new Customer()
            {
                Balance = 2.00,
                Email = "support@nzr.core.com",
                Address = address2,
            };

            Customer customer3 = new Customer()
            {
                Balance = 3.00,
                Email = "mkt@nzr.core.com",
                Address = address2,
            };

            AuditEvent auditEvent1 = new AuditEvent()
            {
                CreatedAt = new DateTime(2018, 8, 11, 00, 23, 59),
                Data = "test",
                Table = "aTable"
            };
            AuditEvent auditEvent2 = new AuditEvent()
            {
                CreatedAt = new DateTime(2019, 1, 1, 14, 00, 35),
                Data = "test",
                Table = "aTable"
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

                dao.Insert(auditEvent1);
                dao.Insert(auditEvent2);
            }

            IList<Customer> resultBalanceBetween1And2;
            IList<AuditEvent> resultEVentDateBetweenAug2018AndDec2018;
            DateTime begin = new DateTime(2018, 8, 1, 00, 00, 00);
            DateTime end = new DateTime(2018, 12, 31, 23, 59, 59);

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                resultBalanceBetween1And2 = dao.Select<Customer>(Where("Balance", BETWEEN, new double[] { 1, 2 }));
                resultEVentDateBetweenAug2018AndDec2018 =
                    dao.Select<AuditEvent>(Where("CreatedAt", BETWEEN, new DateTime[] { begin, end }));
            }

            // Assert

            Assert.Equal(2, resultBalanceBetween1And2.Count);
            Assert.Equal(1, resultEVentDateBetweenAug2018AndDec2018.Count);
        }
    }
}
