using Nzr.Orm.Core.Attributes;
using Nzr.Orm.Tests.Core.Models.Security;

namespace Nzr.Orm.Tests.Core.Models.Crm
{
    /// <summary>
    /// This entity maps to a table that follows one of supported naming convention: LowerCaseUnderlined.
    /// Also its id, defined in the Base class, also maps to a known id pattern: id_table.
    /// In this case there is no need of use attributes to decorate the entity or properties.
    ///
    /// The exception is with the properties that maps to another entities (foreign keys).
    /// </summary>
    public class Customer : BaseEntity
    {
        public string Email { get; set; }

        public double Balance { get; set; }

        /// <summary>
        /// Set the foreign key with LEFT JOIN type because
        /// the "software" allows Customers without a User to access the system.
        /// </summary>
        [ForeignKey("id_application_user", ForeignKeyAttribute.JoinType.Left)]
        public User User { get; set; }

        /// <summary>
        /// Set the foreign key with INNER JOIN type because
        /// the "software" does not allows Customers without Billing Address.
        /// </summary>
        [ForeignKey("id_address", ForeignKeyAttribute.JoinType.Inner)]
        public Address Address { get; set; }

        /// <summary>
        /// Set the foreign key with INNER JOIN type because
        /// the "software" does not allows Customers without Billing Address.
        /// </summary>
        [ForeignKey("id_billing_address", ForeignKeyAttribute.JoinType.Left)]
        public Address BillingAddress { get; set; }

        public string Characteristics { get; set; }
    }
}
