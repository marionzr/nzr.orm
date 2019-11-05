using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Crm;
using System;
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

        [Fact]
        public void Operation_WithExceptionButManualHAndling_ShouldNotRollback()
        {
            // Arrange

            State state = new State() { Name = "TX" };
            State insertedState;
            Exception ex;

            // Act

            using (Dao dao = new Dao(options))
            {
                dao.Insert(state);
                dao.RollbackOnError = false;
                ex = Assert.Throws<NullReferenceException>(() => dao.Insert(null));
            }

            // Assert

            using (Dao dao = new Dao(options))
            {
                insertedState = dao.Select<State>(state.Id);
            }

            Assert.NotNull(ex);
            Assert.Equal(insertedState.Name, insertedState.Name);
        }
    }
}
