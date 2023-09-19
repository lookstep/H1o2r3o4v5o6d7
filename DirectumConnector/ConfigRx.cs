using DirectumConnector.DatabookModels;
using DirectumConnector.DocModels;
using System;
using System.Collections.Generic;

namespace DirectumConnector
{
    public static class ConfigRx
    {
        public static string Url { get; set; }

        public static string Login { get; set; }

        public static string Password { get; set; }

        public static int BusinessUnitIdRx { get; set; }

        private static Dictionary<Type, string> TypeNamesRxByType { get; set; } = new Dictionary<Type, string>();

        private static Dictionary<string, string> TypeNamesRxByGuid { get; set; } = new Dictionary<string, string>();

        static ConfigRx()
        {
            AddDefaultTypeNamesRxByType();
            AddDefaultTypeNamesRxByGuid();
        }

        private static void AddDefaultTypeNamesRxByType()
        {
            TypeNamesRxByType.Add(typeof(BusinessUnitRx), "IBusinessUnits");
            TypeNamesRxByType.Add(typeof(DepartmentRx), "IDepartments");
            TypeNamesRxByType.Add(typeof(PersonRx), "IPersons");
            TypeNamesRxByType.Add(typeof(JobTitleRx), "IJobTitles");
            TypeNamesRxByType.Add(typeof(EmployeeRx), "IEmployees");
            TypeNamesRxByType.Add(typeof(DepartmentOnlyHeadOfficeRx), "IDepartments");
            TypeNamesRxByType.Add(typeof(CounterpartyRx), "ICounterparties");
            TypeNamesRxByType.Add(typeof(CompanyRx), "ICompanies");
            TypeNamesRxByType.Add(typeof(CurrencyRx), "ICurrencies");
            TypeNamesRxByType.Add(typeof(ContractRx), "IContracts");
            TypeNamesRxByType.Add(typeof(DocumentKind1CUppSettingRx), "IDocumentKind1CUppSettingss");
            TypeNamesRxByType.Add(typeof(AccountingDocRx), "IAccountingDocumentBases");
            TypeNamesRxByType.Add(typeof(OrderRx), "IOrders");
            TypeNamesRxByType.Add(typeof(CeoPersonnelOrderRx), "IPersonnelOrders");
            TypeNamesRxByType.Add(typeof(IncomingTaxInvoiceRx), "IIncomingTaxInvoices");
            TypeNamesRxByType.Add(typeof(UniversalTransferDocumentRx), "IUniversalTransferDocuments");
            TypeNamesRxByType.Add(typeof(WaybillRx), "IWaybills");
            TypeNamesRxByType.Add(typeof(ContractStatementRx), "IContractStatements");
            TypeNamesRxByType.Add(typeof(DocumentKind1CDoSettingRx), "IDocumentKind1CDoSettings");
            TypeNamesRxByType.Add(typeof(ContractDocKind1CUppSettingRx), "IContractDocKind1CUppSettings");
            TypeNamesRxByType.Add(typeof(DeputyCeoPersonnelOrderRx), "IPersonnelOrders");
            TypeNamesRxByType.Add(typeof(CaseFileRx), "ICaseFiles");
        }


        public static void AddDefaultTypeNamesRxByGuid()
        {
            TypeNamesRxByGuid.Add("f2f5774d-5ca3-4725-b31d-ac618f6b8850", "IContractStatements");
            TypeNamesRxByGuid.Add("74c9ddd4-4bc4-42b6-8bb0-c91d5e21fb8a", "IIncomingTaxInvoices");
            TypeNamesRxByGuid.Add("58986e23-2b0a-4082-af37-bd1991bc6f7e", "IUniversalTransferDocuments");
            TypeNamesRxByGuid.Add("4e81f9ca-b95a-4fd4-bf76-ea7176c215a7", "IWaybills");
            TypeNamesRxByGuid.Add("f37c7e63-b134-4446-9b5b-f8811f6c9666", "IContracts");
            TypeNamesRxByGuid.Add("9570e517-7ab7-4f23-a959-3652715efad3", "IOrders");
            TypeNamesRxByGuid.Add("63afc7e4-c4d1-45c3-9ff6-151d561eaa40", "IPersonnelOrders");
        }

        public static string GetTypeNameRx<T>() => TypeNamesRxByType[typeof(T)];

        public static string GetTypeNameRxByGuid(string typeGuid) => TypeNamesRxByGuid[typeGuid];
    }
}