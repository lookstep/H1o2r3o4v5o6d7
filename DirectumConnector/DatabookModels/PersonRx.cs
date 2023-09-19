using System;

namespace DirectumConnector.DatabookModels
{
    public class PersonRx : CounterpartyRx
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string ShortName { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
