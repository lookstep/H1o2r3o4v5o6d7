using Newtonsoft.Json;

namespace DirectumConnector.DatabookModels
{
    public class CounterpartyRx
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        [JsonProperty("TIN")]
        public string Tin { get; set; }

        public string Code { get; set; }

        public string Id1CUpp { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeStatus() => Status != null;

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;

        public bool ShouldSerializeTin() => Tin != null;
    }
}
