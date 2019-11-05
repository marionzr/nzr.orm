using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Crm;
using System.Collections.Generic;
using Xunit;
using static Nzr.Orm.Core.Sql.Builders;
using static Nzr.Orm.Core.Sql.Where;

namespace Nzr.Orm.Tests.Core
{
    public class DeleteTest : DaoTest
    {
        public DeleteTest() : base() { }

        [Fact]
        public void Delete_WithEntity_ShouldDeleteOneEntity()
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
        public void Delete_WithWhereClause_ShouldDeleteEntities()
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
                affectedRows = dao.Delete<State>(Where("Name", EQ, "XX"));
            }

            // Assert

            Assert.Equal(2, affectedRows);

            using (Dao dao = new Dao(transaction, options))
            {
                IList<State> results = dao.Select<State>(Where("Name", EQ, "XX"));
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public void Delete_WithResultDiffExpectedResult_ShouldThrowException()
        {
            // Arrange

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(new State() { Name = "XX" });
                dao.Insert(new State() { Name = "XX" });
            }

            OrmException ex;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                ex = Assert.Throws<OrmException>(() => dao.Delete<State>(Where("Name", EQ, "XX"), 1));
            }

            // Assert

            // Assert

            Assert.NotNull(ex);
        }
    }
}
