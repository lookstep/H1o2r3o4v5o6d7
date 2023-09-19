using Newtonsoft.Json;

namespace DirectumConnector.DatabookModels
{
    public class CompanyRx : CounterpartyRx
    {
        public string LegalName { get; set; }

        [JsonProperty("TRRC")]
        public string Trrc { get; set; }

        [JsonProperty("PSRN")]
        public string Psrn { get; set; }

        public bool ShouldSerializeTrrc() => Trrc != null;

        public bool ShouldSerializePsrn() => Trrc != null;

    }
}

