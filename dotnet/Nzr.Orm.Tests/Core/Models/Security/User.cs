using Nzr.Orm.Core.Attributes;

namespace Nzr.Orm.Core.Tests.Models.Security
{
    /// <summary>
    /// This entity maps to a table that doesn't follows one of supported naming convention: LowerCaseUnderlined.
    /// In this case is necessary using the table attribute to decorate the entity. Also the table this entity maps to
    /// is not in the same schema of other tables, so the schema was set to avoid changing the default schema defined
    /// in the Dao instance.
    ///
    /// Another exceptions are the properties that maps to another entities (foreign keys).
    /// </summary
    [Table("application_user", "security")]
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Set the foreign key with INNER JOIN type because
        /// the "software" does not allows User without a Profile.
        /// </summary>
        [ForeignKey("id_profile", ForeignKeyAttribute.JoinType.Inner)]
        public Profile Profile { get; set; }
    }
}
