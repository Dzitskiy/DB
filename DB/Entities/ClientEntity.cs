using NHibernate.Mapping.Attributes;
using System;
using System.Collections.Generic;

namespace DB1.Entities
{   
    [Class(Table = "clients")]
    public class ClientEntity
    {
        [Id(0, Name = "Id")]
        [Generator(1, Class = "native")]
        public virtual long Id { get; set; }

        [Property(NotNull = true)]
        public virtual string FirstName { get; set; }

        [Index(0)]
        [Property(1, NotNull = true, Index = "clients_last_name_idx")]
        public virtual string LastName { get; set; }

        [Property]
        public virtual string MiddleName { get; set; }

        [Property(NotNull = true, Unique = true, UniqueKey = "clients_email_unique")]
        public virtual string Email { get; set; }

        [Bag(0, Name = "Deposits", Inverse = true)]
        [Key(1, Column = "ClientId")]
        [OneToMany(2, ClassType = typeof(DepositEntity))]
        public virtual IList<DepositEntity> Deposits { get; set; }
    }
}
