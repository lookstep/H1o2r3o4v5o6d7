using Connector1CUpp.DatabookModels;
using DirectumConnector.DatabookModels;
using Synchronizer.DocSynchronizers;
using System.Collections.Generic;
using System.Linq;
using System;
using DirectumConnector;
using Connector1CDO;
using Connector1CUpp;
using DirectumConnector.DocModels;
using Newtonsoft.Json;

namespace Synchronizer
{
    public static class DocSyncService
    {
        internal static List<CurrencyRx> CurrenciesRx;

        internal static BusinessUnitRx BusinessUnitRx;

        internal static List<DocumentKind1CUppSettingRx> DocumentKind1CUppSettingsRx;

        internal static List<DocumentKind1CDoSettingRx> DocumentKind1CDoSettingsRx;

        internal static ContractDocKind1CUppSettingRx ContractDocKind1CUppSettingRx;

        internal static List<PersonRx> PersonsRx;

        internal static List<CompanyRx> CompaniesRx;

        internal static List<ContractRx> ContractsRxSelectIdCodeId1CDo;

        internal static List<EmployeeRx> EmployeesRxSelectIdDepartmentPersonnelNumber;

        internal static List<DepartmentRx> DepartmentsRx;

        internal static List<CaseFileRx> CaseFilesRx;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SyncContracts1CDo(DateTime startSyncDate, int top = 10)
        {
            Logger.Debug($"Начата синхронизация договоров из 1С: ДО");

            SetStartSetting();
            Contract1CDoSinchronizer.Sync(startSyncDate, top);

            ContractRelation1CDoSynchronizer.Sync(startSyncDate, top);

            var successMessage = "Синхронизация договоров из 1С: ДО завершена";
            Common.SendSuccessMessage(successMessage);
        }

        public static void SyncAccountingDoc1CUpp(DateTime startSyncDate, int top = 10)
        {
            Logger.Debug($"Начата синхронизация финансовых документов из 1С: УПП");

            SetStartSetting();
            AccountingDoc1CUppSynchronizer.Sync(startSyncDate, top);

            var successMessage = "Синхронизация финансовых документов из 1С: УПП завершена";
            Common.SendSuccessMessage(successMessage);
        }

        public static void SyncOrders1CDo(DateTime startSyncDate, int top = 10)
        {
            Logger.Debug($"Начата синхронизация приказов из 1С: ДО");

            SetStartSetting();
            Order1CDoSynchronizer.CreateCeoOrders(startSyncDate, top);
            Order1CDoSynchronizer.CreateCeoPersonnelOrders(startSyncDate, top);
            Order1CDoSynchronizer.CreateDeputyCeoPersonnelOrders(startSyncDate, top);

            var successMessage = "Синхронизация приказов из 1С: ДО завершена";
            Common.SendSuccessMessage(successMessage);
        }

        private static void SetStartSetting()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            SetCurrencies();

            if (!Common.TrySetBusinessUnit(ref BusinessUnitRx))
                return;

            DocumentKind1CUppSettingsRx = RepositoryRx.GetDocumentKind1CUppSettingsRx();
            DocumentKind1CDoSettingsRx = RepositoryRx.GetDocumentKind1CDoSettingRx();
            ContractDocKind1CUppSettingRx = RepositoryRx.GetContractDocKind1CUppSettingRx().FirstOrDefault();

            PersonsRx = RepositoryRx.GetList<PersonRx>();
            CompaniesRx = RepositoryRx.GetList<CompanyRx>();
            ContractsRxSelectIdCodeId1CDo = RepositoryRx.GetContractsRxSelectIdCodeId1CDoLifeCycleState();
            EmployeesRxSelectIdDepartmentPersonnelNumber = RepositoryRx.GetEmployeeRxSelectIdDepartmentPersonnelNumber();
            DepartmentsRx = RepositoryRx.GetDepartmentsRxIdCode1CUpp();
            CaseFilesRx = RepositoryRx.GetCaseFileByIndex();
        }

        private static void SetCurrencies()
        {
            CurrenciesRx = RepositoryRx.GetList<CurrencyRx>();
            var currencies1CUpp = Repository1CUpp.GetList<Currency1CUpp>();
            var currencies1CDo = Repository1CDo.GetCurrencies();

            foreach (var currencyRx in CurrenciesRx)
            {
                currencyRx.Id1CUpp = currencies1CUpp.FirstOrDefault(y => y.Code == currencyRx.Code).Id;
                var currency1CDo = currencies1CDo.FirstOrDefault(y => y.Code == currencyRx.Code);
                currencyRx.Id1CDo = currency1CDo.Ref_Key.ToString();
            }
        }
    }
}
