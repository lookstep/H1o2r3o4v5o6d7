using DirectumConnector.DatabookModels;
using Newtonsoft.Json;
using System;

namespace DirectumConnector.DocModels
{
    public class AccountingDocRx : DocumentBase
    {
        public string Id1CUpp { get; set; }

        [JsonProperty("LeadingDocument")]
        public BaseEntity Contract { get; set; }

        public EmployeeRx ResponsibleEmployee { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime CustomDocumentDate { get; set; }

        [JsonProperty("Number")]
        public string IncomingDocNumber { get; set; }

        [JsonProperty("Date")]
        public DateTime IncomingDocDate { get; set; }

        public double TotalAmount { get; set; }

        public CurrencyRx Currency { get; set; }

        public string ContractCode { get; set; }

        public bool IsImportFromSbis { get; set; }

        public DepartmentRx Department { get; set; }

        public string Subject { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;
    }
}
