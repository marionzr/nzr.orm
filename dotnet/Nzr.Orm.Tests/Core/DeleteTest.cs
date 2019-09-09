using Nzr.Orm.Core;
using Nzr.Orm.Core.Sql;
using Nzr.Orm.Core.Tests.Models.Crm;
using System.Collections.Generic;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class DeleteTest : DaoTest
    {
        public DeleteTest() : base() { }

        [Fact]
        public void DeleteEntity_WithEntity_ShouldReturnDeleteOneEntity()
        {
            // Arrange

            State state = new State() { Name = "XX" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
            }

            int affectedRows;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                affectedRows = dao.Delete(state);
            }

            // Assert

            Assert.Equal(1, affectedRows);

            using (Dao dao = new Dao(transaction, options))
            {
                State result = dao.Select<State>(state.Id);
                Assert.Null(result);
            }
        }

        [Fact]
        public void DeleteEntity_WithWhereClause_ShouldReturnDeleteEntities()
        {
            // Arrange

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(new State() { Name = "XX" });
                dao.Insert(new State() { Name = "XX" });
            }

            int affectedRows;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                affectedRows = dao.Delete<State>(new Where { { "Name", Where.EQ, "XX" } });
            }

            // Assert

            Assert.Equal(2, affectedRows);

            using (Dao dao = new Dao(transaction, options))
            {
                IList<State> results = dao.Select<State>(new Where { { "Name", Where.EQ, "XX" } });
                Assert.Equal(0, results.Count);
            }
        }
    }
}
