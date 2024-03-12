using System;

namespace DB1.Entities
{
    public class DepositDapperEntity
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

        public ClientDapperEntity Client { get; set; }

        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"DepositDapperEntity[Id={Id},Client={Client},CreatedAt={CreatedAt}]";
        }
    }
}
