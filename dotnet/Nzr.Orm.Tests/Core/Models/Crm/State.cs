using System;

namespace Nzr.Orm.Core.Tests.Models.Crm
{
    /// <summary>
    /// This entity maps to a table that follows one of supported naming convention: LowerCaseUnderlined.
    /// Also its id, defined in the Base class, also maps to a known id pattern: id_table.
    /// In this case there is no need of use attributes to decorate the entity or properties.
    /// </summary>
    public class State
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public State() => Id = Guid.NewGuid();
    }
}
