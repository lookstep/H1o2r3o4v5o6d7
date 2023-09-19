using Connector1CUpp.DatabookModels;
using Newtonsoft.Json;
using System;

namespace Connector1CUpp.DocModels
{
    public class Contract1CUpp
    {
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        [JsonProperty("Owner")]
        public Counterparty1CUpp Counterparty { get; set; }

        [JsonProperty("IsFolder")]
        public bool IsFolder { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("ВалютаВзаиморасчетов_Key")]
        public string CurrencyId { get; set; }

        [JsonProperty("Дата")]
        public DateTime? Date { get; set; }

        [JsonProperty("Номер")]
        public string Number { get; set; }

        [JsonProperty("СрокДействия")]
        public DateTime? ValidTill { get; set; }

        [JsonProperty("Description")]
        public string Name { get; set; }
    }
}
