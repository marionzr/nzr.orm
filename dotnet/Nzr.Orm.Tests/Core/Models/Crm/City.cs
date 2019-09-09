using Nzr.Orm.Core.Attributes;
using System;

namespace Nzr.Orm.Core.Tests.Models.Crm
{
    /// <summary>
    /// This entity maps to a table that follows one of supported naming convention: LowerCaseUnderlined.
    /// Also its id, defined in the Base class, also maps to a known id pattern: id_table.
    /// In this case there is no need of use attributes to decorate the entity or properties.
    ///
    /// The exception is with the properties that maps to another entities (foreign keys).
    /// </summary>
    public class City
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Set the foreign key with INNER JOIN type because
        /// the "software" does not allows City without a State.
        /// </summary>
        [ForeignKey("id_state", ForeignKeyAttribute.JoinType.Inner)]
        public State State { get; set; }

        /// <summary>
        /// This property doesn't map to any column.
        /// You can leave it as it is or add a NotMapped attribute.
        /// In case you do not provide the NotMapped, while debugging a Debug Message will be displayed in the console.
        /// </summary>
        [NotMapped]
        public object Tag { get; set; }

        public City() => Id = Guid.NewGuid();
    }
}
