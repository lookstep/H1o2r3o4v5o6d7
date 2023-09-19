using DirectumConnector;
using Connector1CUpp;
using System.Linq;
using DirectumConnector.DocModels;
using DirectumConnector.DatabookModels;
using System;
using Connector1CUpp.DocModels;
using Connector1CUpp.DatabookModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Synchronizer.DocSynchronizers
{
    public class AccountingDoc1CUppSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync(DateTime startSyncDate, int top)
        {
            var accountingDocsRx = RepositoryRx.GetListSelectIdAndId1CUpp<AccountingDocRx>();

            var count = Repository1CUpp.GetAccountingDocCount(startSyncDate);
            var iterationCount = (int)Math.Ceiling(count / (double)top);

            for (var i = 0; i < iterationCount; i++)
            {
                int skip = top * i;
                var accountingDocs1CUpp = Repository1CUpp.GetAccountingDocs1CUpp(skip, top, startSyncDate);
                CreateAccountingDocuments(accountingDocsRx, accountingDocs1CUpp);
            }
        }

        public static void CreateAccountingDocuments(List<AccountingDocRx> accountingDocsRx, List<AccountingDoc1CUpp> accountingDocs1CUpp)
        {
            var accountingDocGroups = accountingDocs1CUpp.GroupBy(x => x.Contract?.Code);

            Parallel.ForEach(accountingDocGroups, accountingDocGroup =>
            { 
                foreach (var accountingDoc1CUpp in accountingDocGroup)
                {
                    try
                    {
                        CreateAccountingDocument(accountingDocsRx, accountingDoc1CUpp);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Synchronizer.Common.TrySendNotificationToAdmins(ex);
                    }
                }
            });
        }

        private static void CreateAccountingDocument(List<AccountingDocRx> accountingDocsRx, AccountingDoc1CUpp accountingDoc1CUpp)
        {
            var documentKindSetting = DocSyncService.DocumentKind1CUppSettingsRx.FirstOrDefault(x => x.Name == accountingDoc1CUpp.DocumentKind);

            if (accountingDocsRx.Any(x => x.Id1CUpp == accountingDoc1CUpp.Id) || documentKindSetting == null)
                return;

            ValidateAccountingDoc(accountingDoc1CUpp);

            var accountingDocRx = new AccountingDocRx
            {
                DocumentNumber = accountingDoc1CUpp.Number,
                CustomDocumentDate = accountingDoc1CUpp.Date,
                Id1CUpp = accountingDoc1CUpp.Id,
                IncomingDocDate = accountingDoc1CUpp.IncomingDocDate,
                IncomingDocNumber = accountingDoc1CUpp.IncomingDocNumber,
                IsImportFromSbis = documentKindSetting.Sbis,
                ContractCode = accountingDoc1CUpp.Contract.Code,
                TotalAmount = accountingDoc1CUpp.TotalAmount,
                Currency = GetCurrency(accountingDoc1CUpp.CurrencyId)
            };
            accountingDocRx.Name = GetNameAccountingDoc(accountingDoc1CUpp);
            accountingDocRx.ResponsibleEmployee = GetResponsibleEmployee(accountingDoc1CUpp, accountingDocRx);
            accountingDocRx.Department = accountingDocRx.ResponsibleEmployee.Department;
            accountingDocRx.Contract = new BaseEntity { Id = GetOrCreateContract(accountingDoc1CUpp, accountingDocRx).Id };

            // Таблица documentKindsMembers в Rx обязательна для заполнения
            var documentKindsMembers = documentKindSetting.DocumentKinds;

            foreach (var docKindMember in documentKindsMembers)
            {
                // также столбец DocumentKind обязательный для заполнения
                accountingDocRx.DocumentKind = docKindMember.DocumentKind;

                var createdDocument = Common.Create(accountingDocRx);
                try
                {
                    if (!accountingDocRx.IsImportFromSbis)
                        RepositoryRx.SendApprovalTaskForAccDoc(createdDocument.Id);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Synchronizer.Common.TrySendNotificationToAdmins(ex);
                }
            }
        }

        private static CurrencyRx GetCurrency(string currencyId)
        {
            return DocSyncService.CurrenciesRx.FirstOrDefault(x => x.Id1CUpp == currencyId);
        }

        private static EmployeeRx GetResponsibleEmployee(AccountingDoc1CUpp accountingDoc1CUpp, AccountingDocRx accountingDocRx)
        {
            var responsibleEmployee = DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.Id1CUpp == accountingDoc1CUpp.ArchiveResponsibleId);
            return responsibleEmployee ??
                throw new ApplicationException($"При синхронизации документа {accountingDocRx.Name} " +
                $"c Id1CUpp = {accountingDoc1CUpp.Id} возникла ошибка. В Rx не найден сотрудник с Id1CUpp = {accountingDoc1CUpp.ArchiveResponsibleId} ");
        }

        private static void ValidateAccountingDoc(AccountingDoc1CUpp accDoc1CUpp)
        {
            ValidateProperty(accDoc1CUpp.Contract, "Ответственный", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.Number, "Номер", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.Date, "Дата", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.IncomingDocDate, "ДатаВходящегоДокумента", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.IncomingDocNumber, "НомерВходящегоДокумента", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.Counterparty, "Контрагент", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.DocumentKind, "ЗначенияАрхив", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.ArchiveResponsibleId, "ОтветственныйЗаДокументВАрхиве_Key", accDoc1CUpp);
            ValidateProperty(accDoc1CUpp.Contract, "Договор", accDoc1CUpp);
        }

        private static void ValidateProperty<T>(T property, string localizedName, AccountingDoc1CUpp accountingDoc1CUpp)
        {
            var accDocName = GetNameAccountingDoc(accountingDoc1CUpp);
            if (property == null)
                throw new ApplicationException($"В документе {accDocName} с id1CUpp = {accountingDoc1CUpp.Id} не заполнено поле {localizedName}");
        }

        public static string GetNameAccountingDoc(AccountingDoc1CUpp accountingDoc1CUpp)
        {
            return $"{accountingDoc1CUpp.DocumentKind} № {accountingDoc1CUpp.Number} от {accountingDoc1CUpp.Date} {accountingDoc1CUpp.Counterparty.Name}";
        }

        private static DocumentBase GetOrCreateContract(AccountingDoc1CUpp accountingDoc1CUpp, AccountingDocRx accountingDocRx)
        {
            var contractRx = RepositoryRx.GetContractsRx(accountingDoc1CUpp.Contract.Code).FirstOrDefault();

            if (contractRx != null)
                return contractRx;

            return Contract1CUppSynchronizer.CreateContractRxFrom1CUpp(accountingDoc1CUpp, accountingDocRx);
        }
    }
}
