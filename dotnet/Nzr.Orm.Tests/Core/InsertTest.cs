﻿using Nzr.Orm.Core;
using Nzr.Orm.Tests.Core.Models.Crm;
using Nzr.Orm.Tests.Core.Models.Security;
using System;
using Xunit;



namespace Nzr.Orm.Tests.Core
{
    public class InsertTest : DaoTest
    {
        public InsertTest() : base() { }

        [Fact]
        public void Insert_WithNullEntity_ShouldThrowNullReferenceException()
        {
            // Arrange
            Customer customer = null;

            // Act
            // Assert
            Assert.Throws<NullReferenceException>(() =>
            {
                using (Dao dao = new Dao(transaction, options))
                {
                    dao.Insert(customer);
                }
            });
        }

        [Fact]
        public void Insert_WithNotDecoratedEntity_ShouldInsertBasedOnNamingStyle()
        {
            // Arrange

            // State property "Id" is mapped to column "id_state".
            // Due the Option.UseComposedId = true configuration and the naming style
            // the table name will be set as "state" and the column to "id" + "state"
            State state = new State() { Name = "CA" };

            int affectedRows;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                // Since the "Id" attribute is not decorated as auto-generated the return of the Insert method
                // will be the affected rows (1 is expected)
                affectedRows = dao.Insert(state);
            }

            // Assert

            Assert.Equal(1, affectedRows);

            using (Dao dao = new Dao(transaction, options))
            {
                State insertedState = dao.Select<State>(state.Id);
                Assert.Equal(state.Id, insertedState.Id);
                Assert.Equal(state.Name, insertedState.Name);
            }
        }

        [Fact]
        public void Insert_WithSchemaDefined_ShouldInsertOverridingDaoSchema()
        {
            // Arrange

            // The Profile entity maps to a "profile" table in the "security" schema.
            Profile profile = new Profile()
            {
                Permissions = "C|R|U|D"
            };

            string daoSchema;
            int result;

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                daoSchema = dao.Options.Schema;
                result = dao.Insert(profile);
            }

            // Assert

            Assert.Equal(options.Schema, daoSchema);
            Assert.Equal(1, result);

            using (Dao dao = new Dao(transaction, options))
            {
                Profile insertedProfile = dao.Select<Profile>(profile.Id);
                Assert.NotNull(insertedProfile);
                Assert.Equal(profile.Id, insertedProfile.Id);
                Assert.Equal(profile.Permissions, insertedProfile.Permissions);
            }
        }

        [Fact]
        public void Insert_WithAutoGeneratedId_ShouldInsertAndSetId()
        {
            // Arrange

            // The Profile Id is auto-generated. After insert, the property Id should have a valid value.
            Profile profile = new Profile()
            {
                Id = Guid.Empty,
                Permissions = "C"
            };

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(profile);
            }

            // Assert

            Assert.NotEqual(Guid.Empty, profile.Id);

            using (Dao dao = new Dao(transaction, options))
            {
                Profile insertedProfile = dao.Select<Profile>(profile.Id);
                Assert.Equal(profile.Id, insertedProfile.Id);
                Assert.Equal(profile.Permissions, insertedProfile.Permissions);
            }
        }

        [Fact]
        public void Insert_ReferencingOtherEntities_ShouldRetriveForeignIdsAndInsert()
        {
            // Arrange

            Profile profile = new Profile()
            {
                Id = Guid.Empty,
                Permissions = "C"
            };

            User user = new User()
            {
                Username = "marionzr",
                Password = "*******",
                Profile = profile
            };

            // Act

            using (Dao dao = new Dao(transaction, options))
            {
                dao.Insert(profile);
                dao.Insert(user);
            }

            // Assert

            Assert.NotEqual(Guid.Empty, user.Id);

            using (Dao dao = new Dao(transaction, options))
            {
                User insertedUser = dao.Select<User>(user.Id);
                Assert.NotNull(insertedUser);
                Assert.Equal(user.Id, insertedUser.Id);
                Assert.Equal(user.Id, insertedUser.Id);
                Assert.NotNull(insertedUser.Profile);
                Assert.Equal(profile.Permissions, insertedUser.Profile.Permissions);
            }
        }
    }
}
