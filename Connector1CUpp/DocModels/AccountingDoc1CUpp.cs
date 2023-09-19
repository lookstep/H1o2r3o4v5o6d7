using Connector1CUpp.DatabookModels;
using Newtonsoft.Json;
using System;

namespace Connector1CUpp.DocModels
{
    public class AccountingDoc1CUpp
    {
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        [JsonProperty("Number")]
        public string Number { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Контрагент")]
        public Counterparty1CUpp Counterparty { get; set; }

        [JsonProperty("НомерВходящегоДокумента")]
        public string IncomingDocNumber { get; set; }

        [JsonProperty("ДатаВходящегоДокумента")]
        public DateTime IncomingDocDate { get; set; }

        [JsonProperty("ОтветственныйЗаДокументВАрхиве_Key")]
        public string ArchiveResponsibleId { get; set; }

        [JsonProperty("ДоговорКонтрагента")]
        public Contract1CUpp Contract { get; set; }

        [JsonProperty("ЗначенияАрхив")]
        public string DocumentKind { get; set; }

        [JsonProperty("СуммаДокумента")]
        public double TotalAmount { get; set; }

        [JsonProperty("ВалютаДокумента_Key")]
        public string CurrencyId { get; set; }
    }
}
