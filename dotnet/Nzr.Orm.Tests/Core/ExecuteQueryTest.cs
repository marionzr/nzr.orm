using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Crm;
using System.Collections.Generic;
using Xunit;
using static Nzr.Orm.Core.Sql.Builders;


namespace Nzr.Orm.Tests.Core
{
    public class RawQueryTest : DaoTest
    {
        [Fact]
        public void ExecuteQuery_WithNoParameter_ShouldReturnEntities()
        {
            // Arrange
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
                Balance = 100.01,
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

            IList<dynamic> customers;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                string sql = @"SELECT a.id_customer AS Id, a.balance AS Balance, a.email as Email, c.name as City 
                             FROM [crm].customer a
                             LEFT JOIN [crm].address b ON a.id_address = b.id_address
                             LEFT JOIN [crm].city c ON b.id_city = c.id_city
                             LEFT JOIN [crm].state d ON c.id_state = d.id_state
                             WHERE (d.name IS NULL OR d.name IN (@state1, @state2))
                             ORDER BY Email ASC";
                customers = dao.ExecuteQuery(sql, Parameters("@state1", "WA").And("@state2", "CA"));
            }

            // Assert

            Assert.Equal("mkt@nzr.core.com", customers[0].Email);
            Assert.Equal(100.01, (double)customers[0].Balance);
            Assert.Null(customers[0].City);
            Assert.Equal("sales@nzr.core.com", customers[1].Email);
            Assert.Equal(1.55, (double)customers[1].Balance);
            Assert.Equal("Cupertino", customers[1].City);
            Assert.Equal("support@nzr.core.com", customers[2].Email);
            Assert.Equal(2.01, (double)customers[2].Balance);
            Assert.Equal("Cupertino", customers[2].City);
        }
    }
}
