using DirectumConnector;
using DirectumConnector.DatabookModels;
using System.Linq;
using DirectumConnector.DocModels;
using System;
using Connector1CDO;
using System.Collections.Generic;
using System.Threading.Tasks;
using StandardODATA;

namespace Synchronizer.DocSynchronizers
{
    public static class Contract1CDoSinchronizer
    {
        public static readonly IReadOnlyList<string> StatusNames = new string[] { "Заключен", "Аннулирован", "Исполнен" };

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync(DateTime startSyncDate, int top)
        {
            var contractsRx = RepositoryRx.GetContractsRxSelectIdCodeId1CDoLifeCycleState();
            var count = Repository1CDo.GetInternalDocumentsCount(startSyncDate, Config1CDo.ContractId1CDo);
            var iterationCount = (int)Math.Ceiling(count / (double)top);
            var additionalDetails = Repository1CDo.GetInternalDocumentAdditionalDetails();
            var objectPropertyValues = Repository1CDo.GetObjectPropertyValue();


            for (int i = 0; i < iterationCount; i++)
            {
                int skip = top * i;
                var contracts1CDo = Repository1CDo.GetInternalDocuments(startSyncDate, Config1CDo.ContractId1CDo, skip, top);
                var files1CDo = Common.GetFiles1CDo(contracts1CDo);

                CreateOrUpdateContracts(contractsRx, contracts1CDo, additionalDetails, objectPropertyValues, files1CDo);
            }
        }

        public static void CreateOrUpdateContracts(List<ContractRx> contractsRx,
                                                   IEnumerable<Catalog_ВнутренниеДокументы> contracts1CDo,
                                                   IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails,
                                                   IEnumerable<Catalog_ЗначенияСвойствОбъектов> objectPropertyValues,
                                                   List<Catalog_Файлы> files1CDo)
        {
            Parallel.ForEach(contracts1CDo, contract1CDo =>
            {
                try
                {
                    CreateOrUpdateContract(contractsRx, additionalDetails, contract1CDo, objectPropertyValues, files1CDo);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Synchronizer.Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrUpdateContract(List<ContractRx> contractsRx,
                                                   IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> internalDocAdditionalDetails,
                                                   Catalog_ВнутренниеДокументы contract1cDo,
                                                   IEnumerable<Catalog_ЗначенияСвойствОбъектов> objectPropertyValues,
                                                   List<Catalog_Файлы> files1CDo)
        {
            Validate(contract1cDo);

            var Id1CDo = contract1cDo.Ref_Key.ToString();

            var contractRxExists = contractsRx.FirstOrDefault(x => x.Id1CDo == Id1CDo);

            var additionalDetails = internalDocAdditionalDetails.Where(x => x.Ref_Key == contract1cDo.Ref_Key);

            var isContract = IsContract(additionalDetails, contract1cDo, objectPropertyValues);

            var status = GetStatus(additionalDetails, contract1cDo, isContract);

            var contractRx = new ContractRx();

            if (contractRxExists != null)
            {
                if (contractRxExists.LifeCycleState == status)
                    return;
                FillProperties(contractRx, contract1cDo, additionalDetails, isContract, status);
                RepositoryRx.UpdateDocumentByKind(contractRx, contractRxExists.Id);
                return;
            }

            FillProperties(contractRx, contract1cDo, additionalDetails, isContract, status);

            var createdContractRx = Common.Create(contractRx);

            try
            {
                Common.CreateVersion(createdContractRx.Id, Id1CDo, files1CDo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Synchronizer.Common.TrySendNotificationToAdmins(ex);
            }
            finally
            {
                Synchronizer.Common.TrySendApprovalTaskForDoc1CDo(createdContractRx.Id);
            }
        }

        private static void FillProperties(ContractRx contractRx, Catalog_ВнутренниеДокументы contract1cDo, IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails, bool isContract, string status)
        {
            if (isContract)
                contractRx.CodeContractualDocument = GetContractCode(additionalDetails, contract1cDo);

            contractRx.Counterparty = new CounterpartyRx { Id = GetСounterpartyRxId(contract1cDo) };
            contractRx.DocumentDateNew = contract1cDo.ДатаРегистрации;
            contractRx.DocumentNumberNew = contract1cDo.РегистрационныйНомер;
            contractRx.ValidFrom = GetNonEmptyDateOrNull(contract1cDo.ДатаНачалаДействия);
            contractRx.ValidTill = GetNonEmptyDateOrNull(contract1cDo.ДатаОкончанияДействия);
            contractRx.BusinessUnit = DocSyncService.BusinessUnitRx;
            contractRx.TotalAmount = contract1cDo.Сумма;
            contractRx.Subject = contract1cDo.Заголовок;
            contractRx.OurSignatory = GetOurSignatory(contract1cDo);
            contractRx.Currency = DocSyncService.CurrenciesRx.FirstOrDefault(x => x.Id1CDo == contract1cDo.Валюта_Key.ToString());
            contractRx.DocumentKind = Common.GetDocumentKindRx(contract1cDo);
            contractRx.Id1CDo = contract1cDo.Ref_Key.ToString();
            contractRx.DocumentNumberCounterparty = GetDocNumberCounterparty(additionalDetails);
            contractRx.DocumentDateCounterparty = GetDocumentDateCounterparty(additionalDetails);
            contractRx.ResponsibleEmployee = GetResponsibleEmployee(contract1cDo);
            contractRx.Compiler = GetCompiler(contract1cDo);
            contractRx.CaseDateSafe = contract1cDo.ДатаРегистрации;
            contractRx.CaseFile = GetCaseFile(contract1cDo);
            contractRx.Department = GetDepartment(contract1cDo);
            contractRx.LifeCycleState = status;
        }

        private static CaseFileRx GetCaseFile(Catalog_ВнутренниеДокументы contract1cDo)
        {
            return contract1cDo.Дело != null && contract1cDo.Дело.НоменклатураДел != null ?
                DocSyncService.CaseFilesRx.FirstOrDefault(x => x.Index == contract1cDo.Дело.НоменклатураДел.Индекс)
                : null;

        }

        private static EmployeeRx GetCompiler(Catalog_ВнутренниеДокументы contract1cDo)
        {
            return contract1cDo.Подготовил != null ?
                DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.PersonnelNumber == contract1cDo.Подготовил.ТабНомер)
                : null;
        }

        private static EmployeeRx GetResponsibleEmployee(Catalog_ВнутренниеДокументы contract1cDo)
        {
            return contract1cDo.Ответственный != null ?
                DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.PersonnelNumber == contract1cDo.Ответственный.ТабНомер)
                : null;
        }

        private static string GetDocNumberCounterparty(IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails)
        {
            return additionalDetails.FirstOrDefault(x => x.Свойство.Заголовок == "Номер")?.Значение;
        }

        private static DateTime? GetDocumentDateCounterparty(IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails)
        {
            var dateString = additionalDetails.FirstOrDefault(x => x.Свойство.Заголовок == "Дата документа")?.Значение;
            DateTime.TryParse(dateString, out var date);
            return GetNonEmptyDateOrNull(date);
        }

        private static DepartmentRx GetDepartment(Catalog_ВнутренниеДокументы contract1cDo)
        {
            return DocSyncService.DepartmentsRx.FirstOrDefault(x => x.Code == contract1cDo.Подразделение.Код1СУПП);
        }

        private static DateTime? GetNonEmptyDateOrNull(DateTime? date)
        {
            return date != DateTime.MinValue ? date : null;
        }

        private static bool IsContract(IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails,
                                       Catalog_ВнутренниеДокументы contract1cDo,
                                       IEnumerable<Catalog_ЗначенияСвойствОбъектов> objectPropertyValues)
        {
            var contractDocKindDetailName = additionalDetails.FirstOrDefault(x => x.Свойство.Заголовок == "Вид документа");
            var objectPropertyValue = objectPropertyValues.FirstOrDefault(x => x.Ref_Key.ToString() == contractDocKindDetailName.Значение);
            if (objectPropertyValue == null)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Не заполнено поле Вид документа");

            return objectPropertyValue.Description == "Договор";
        }

        private static void Validate(Catalog_ВнутренниеДокументы contract1cDo)
        {
            var id1CDo = contract1cDo.Ref_Key.ToString();
            ValidateProperty(contract1cDo.Контрагент, "Контрагент", contract1cDo);
            if (contract1cDo.ДатаРегистрации == DateTime.MinValue)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Не заполнено поле ДатаРегистрации");
        }

        private static EmployeeRx GetOurSignatory(Catalog_ВнутренниеДокументы contract1cDo)
        {
            var businessUnit = contract1cDo.Стороны.FirstOrDefault(x => x.Сторона.ToString() == DocSyncService.BusinessUnitRx.Id1CDo);
            ValidateProperty(businessUnit, "Наша организация", contract1cDo);
            var signatoryGuid = businessUnit?.Подписал;
            ValidateProperty(signatoryGuid, "Подписал", contract1cDo);

            // todo Реализовать поверку на право подписи. У подписывающего в Rx должно быть право подписи
            var user = Repository1CDo.GetUser(signatoryGuid);
            var ourSignatory = DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.PersonnelNumber == user.ТабНомер);
            return ourSignatory ?? throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                $"В Rx нет сотрудника с табельным номером {user.ТабНомер}");
        }

        private static void ValidateProperty<T>(T property, string localizedName, Catalog_ВнутренниеДокументы contract1cDo)
        {
            if (property == null)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Не заполнено поле {localizedName}");
        }

        public static int GetСounterpartyRxId(Catalog_ВнутренниеДокументы contract1cDo)
        {
            var counterparty1CDo = contract1cDo.Контрагент;
            if (counterparty1CDo.ИНН.Count() == Const.CompanyTinCount && !string.IsNullOrWhiteSpace(counterparty1CDo.КПП))// У юридических лиц ИНН состоит из 10 цифр
            {
                var companyRx = RepositoryRx.GetCompanyRx(counterparty1CDo.ИНН, counterparty1CDo.КПП);

                if (companyRx == null)
                    companyRx = CreateCompany(counterparty1CDo);

                return companyRx.Id;
            }

            if (counterparty1CDo.ИНН.Count() == Const.PersonTinCount)// У физических лиц ИНН состоит из 12 цифр
            {
                var personRx = RepositoryRx.GetPersonsRx(counterparty1CDo.ИНН);

                if (personRx == null)
                    personRx = CreatePerson(counterparty1CDo);

                return personRx.Id;
            }

            throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                $"Контрагент из 1CDo с id = {counterparty1CDo.Ref_Key} не является ЮрЛицом или ФизЛицом, либо некорректно заполнены ИНН или КПП.");
        }

        private static CompanyRx CreateCompany(Catalog_Контрагенты counterparty1CDo)
        {
            var company = new CompanyRx
            {
                Code = counterparty1CDo.Code,
                Name = counterparty1CDo.Description,
                Status = "Active",
                Tin = counterparty1CDo.ИНН,
                Trrc = counterparty1CDo.КПП
            };
            return RepositoryRx.Create(company);
        }

        private static PersonRx CreatePerson(Catalog_Контрагенты counterparty1CDo)
        {
            var person = new PersonRx();

            person.FillPersonName(counterparty1CDo.Description);
            person.Tin = counterparty1CDo.ИНН;
            person.Code = counterparty1CDo.Code;
            person.Name = counterparty1CDo.Description;
            person.Status = "Active";
            return RepositoryRx.Create(person);
        }

        private static string GetContractCode(IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails,
                                              Catalog_ВнутренниеДокументы contract1cDo)
        {
            var codeDetail = additionalDetails.FirstOrDefault(x => x.Свойство.Заголовок == "Код договора");
            if (codeDetail == null)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Не указан Код договора");
            return codeDetail.Значение;
        }

        public static string GetStatus(IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> additionalDetails,
                                       Catalog_ВнутренниеДокументы contract1cDo,
                                       bool isContract)
        {
            var caseDateSafe = additionalDetails.FirstOrDefault(x => x.Свойство.Заголовок == "Дата сдачи на хранение");

            if (caseDateSafe != null)
                return "Archived";

            var properties = additionalDetails.Where(x => StatusNames.Any(s => s == x.Свойство.Заголовок)).ToList();

            // Может быть 3 фолса и дата, например!!!!
            if (properties.Count > 1)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Установлено несколько статусов");

            var property = properties.FirstOrDefault();

            if (property == null && isContract)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Статус не установлен");
            else if (property == null)
                return "Active";

            if (property.Свойство.Заголовок == "Заключен")
                return "Active";

            if (property.Свойство.Заголовок == "Аннулирован")
                return "Obsolete";

            if (property.Свойство.Заголовок == "Исполнен")
                return "Closed";

            if (isContract)
                throw new ApplicationException($"Не удалось создать карточку документа {contract1cDo.Заголовок} с id1CDo = {contract1cDo.Ref_Key}. " +
                    $"Для статуса из 1CDo \"{property.Свойство.Заголовок}\" нет соответствующего статуса договора в Rx");
            else
                return "Active";
        }
    }
}
