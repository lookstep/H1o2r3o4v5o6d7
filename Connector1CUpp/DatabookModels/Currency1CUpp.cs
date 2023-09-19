using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    public class Currency1CUpp
    {
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}
