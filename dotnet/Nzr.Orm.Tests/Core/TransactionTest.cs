using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Crm;
using Xunit;

namespace Nzr.Orm.Tests.Core
{
    public class TransactionTest : DaoTest
    {
        public TransactionTest() : base() { }

        [Fact]
        public void Operation_WithRollbackTransaction_ShouldNotChangeDatabase()
        {
            // Arrange

            State state = new State() { Name = "TX" };

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(state);
            }

            transaction.Commit();

            transaction = connection.BeginTransaction();

            State insertedState;

            using (Dao dao = new Dao(transaction, options))
            {
                insertedState = dao.Select<State>(state.Id);
            }

            // Act

            state.Name = "WA";

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Update(state);
            }

            transaction.Rollback();

            transaction = connection.BeginTransaction();

            State updatedState;

            using (Dao dao = new Dao(transaction, options))
            {
                updatedState = dao.Select<State>(state.Id);
            }

            // Assert

            Assert.Equal(insertedState.Name, updatedState.Name);
        }
    }
}
