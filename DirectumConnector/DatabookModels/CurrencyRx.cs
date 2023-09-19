using Newtonsoft.Json;

namespace DirectumConnector.DatabookModels
{
    public class CurrencyRx
    {
        public int Id { get; set; } 

        [JsonProperty("NumericCode")]
        public string Code { get; set; }

        public string Id1CUpp { get; set; }

        public string Id1CDo { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeId1CUpp() => false;

        public bool ShouldSerializeId1CDo() => false;
    }
}