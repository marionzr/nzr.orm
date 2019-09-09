using Nzr.Orm.Core.Attributes;

namespace Nzr.Orm.Core.Tests.Models.Crm
{
    /// <summary>
    /// This entity maps to a table that follows one of supported naming convention: LowerCaseUnderlined.
    /// Also its id, defined in the Base class, also maps to a known id pattern: id_table.
    /// In this case there is no need of use attributes to decorate the entity or properties.
    ///
    /// The exception is with the properties that maps to another entities (foreign keys).
    /// </summary>
    public class Address : BaseEntity
    {
        public string AddressLine { get; set; }

        public string ZipCode { get; set; }

        /// <summary>
        /// Set the foreign key with INNER JOIN type because
        /// the "software" does not allows Address without a City.
        /// </summary>
        [ForeignKey("id_city", ForeignKeyAttribute.JoinType.Inner)]
        public City City { get; set; }
    }
}
