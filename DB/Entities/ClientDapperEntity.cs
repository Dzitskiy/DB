namespace DB1.Entities
{
    public class ClientDapperEntity
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public override string ToString()
        {
            return $"ClientDapperEntity[Id={Id},FirstName={FirstName},LastName={LastName},MiddleName={MiddleName},Email={Email}]";
        }
    }
}
