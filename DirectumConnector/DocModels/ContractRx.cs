using DirectumConnector.DatabookModels;
using System;

namespace DirectumConnector.DocModels
{
    public class ContractRx : DocumentBase
    {
        public CounterpartyRx Counterparty { get; set; }

        public string DocumentNumberNew { get; set; }

        public DateTime? DocumentDateNew { get; set; }

        public DateTime? ValidTill { get; set; }

        public DateTime? ValidFrom { get; set; }

        public string CodeContractualDocument { get; set; }

        public double? TotalAmount { get; set; }

        public CurrencyRx Currency { get; set; }

        public BusinessUnitRx BusinessUnit { get; set; }

        public DepartmentRx Department { get; set; }

        public string Id1CDo { get; set; }

        public EmployeeRx OurSignatory { get; set; }

        public string LifeCycleState { get; set; }

        public string Subject { get; set; }

        public EmployeeRx ResponsibleEmployee { get; set; }

        public string DocumentNumberCounterparty { get; set; }

        public DateTime? DocumentDateCounterparty { get; set; }

        public EmployeeRx Compiler { get; set; }

        public CaseFileRx CaseFile { get; set; }

        public DateTime? CaseDateSafe { get; set; }


        public bool ShouldSerializeId1CDo() => Id1CDo != null;
    }
}