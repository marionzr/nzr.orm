using Nzr.Orm.Core;
using Nzr.Orm.Core.Tests.Models.Crm;
using System.Linq;
using Xunit;
using static Nzr.Orm.Core.Sql.Aggregate;
using static Nzr.Orm.Core.Sql.Alias;
using static Nzr.Orm.Core.Sql.Where;

namespace Nzr.Orm.Tests.Core
{
    public class HowToUseTest : DaoTest
    {
        public HowToUseTest() : base() { }

        [Fact]
        public void Crud_WithEntity_ShouldInsertSelectUpdateSumAndDelete()
        {
            // Arrange

            State state = new State() { Name = "CX" };
            int result;
            State stateFromDb;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Insert(state);
            }

            // Assert

            Assert.Equal(1, result);

            // Act (Select by Id)

            using (Dao dao = new Dao(transaction, options))
            {
                stateFromDb = dao.Select<State>(state.Id);
            }

            // Assert

            Assert.Equal(state.Id, stateFromDb.Id);
            Assert.Equal(state.Name, stateFromDb.Name);

            // Act (Select using Where)

            using (Dao dao = new Dao(transaction, options))
            {
                stateFromDb = dao.Select<State>(Where("Name", "CX")).FirstOrDefault();
            }

            // Assert

            Assert.Equal(state.Id, stateFromDb.Id);
            Assert.Equal(state.Name, stateFromDb.Name);

            // Act (Update entity)

            state.Name = "CA";

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Update(state);
                stateFromDb = dao.Select<State>(Where("Name", "CA")).FirstOrDefault();
            }

            // Assert

            Assert.Equal(1, result);
            Assert.Equal(state.Id, stateFromDb.Id);
            Assert.Equal(state.Name, stateFromDb.Name);

            // Act (Aggregate)

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(new State() { Name = "WA" }); // Insert another state
                result = dao.Aggregate<State, int>(Aggregate(COUNT, "Id"), Where("Name", IS_NOT, null));
            }

            // Assert

            Assert.Equal(2, result);

            // Act (Delete by entity)

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Delete(state);
            }

            // Assert

            Assert.Equal(1, result);

            // Act (Delete using Where)

            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Delete<State>(Where("Name", "WA"));
            }

            // Assert

            Assert.Equal(1, result);
            using (Dao dao = new Dao(transaction, options))
            {
                result = dao.Aggregate<State, int>(Aggregate(COUNT, "Id"), Where("Name", EQ, "WA"));
                Assert.Equal(0, result);
            }
        }
    }
}
