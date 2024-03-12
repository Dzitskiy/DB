using NHibernate.Mapping.Attributes;
using System;

namespace DB1.Entities
{
    [Class(Table = "deposits")]
    public class DepositEntity
    {
        [Id(0, Name = "Id")]
        [Generator(1, Class = "native")]
        public virtual long Id { get; set; }

        //[Property(NotNull = true)]
        //public virtual long ClientId { get; set; }

        [ManyToOne(Column = "client_id", ForeignKey = "deposits_fk_client_id", Cascade = "all")]
        public virtual ClientEntity Client { get; set; }

        [Property(NotNull = true)]
        public virtual DateTime CreatedAt { get; set; }
    }
}
