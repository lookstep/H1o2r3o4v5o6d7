using DirectumConnector.DatabookModels;
using System.Linq;
using DirectumConnector.DocModels;
using System;
using Connector1CUpp.DatabookModels;
using Connector1CUpp.DocModels;

namespace Synchronizer.DocSynchronizers
{
    public class Contract1CUppSynchronizer
    {
        public static DocumentBase CreateContractRxFrom1CUpp(AccountingDoc1CUpp accountingDoc1CUpp, AccountingDocRx accountingDocRx)
        {
            var contract1CUpp = accountingDoc1CUpp.Contract;
            var contractRx = new ContractRx
            {
                CodeContractualDocument = contract1CUpp.Code,
                DocumentDateNew = contract1CUpp.Date ?? DateTime.Today, // заполняем текущей датой, т.к. в Rx поле "Дата договора" обязательное
                DocumentNumberNew = contract1CUpp.Number,
                ValidTill = contract1CUpp?.ValidTill,
                ValidFrom = contract1CUpp.Date,
                BusinessUnit = DocSyncService.BusinessUnitRx,
                Counterparty = new CounterpartyRx { Id = GetСounterpartyRxId(accountingDoc1CUpp) },
                Currency = DocSyncService.CurrenciesRx.FirstOrDefault(x => x.Id1CUpp == contract1CUpp.CurrencyId),
                DocumentKind = DocSyncService.ContractDocKind1CUppSettingRx.DocumentKind,
                ResponsibleEmployee = accountingDocRx.ResponsibleEmployee,
                Subject = contract1CUpp.Name,
                LifeCycleState = "Active"
            };

            return Common.Create(contractRx);
        }

        private static int GetСounterpartyRxId(AccountingDoc1CUpp accountingDoc1CUpp)
        {
            var counterparty1CUpp = accountingDoc1CUpp.Counterparty;
            // По требованию заказчика это единственный критерий отбора юри физ лиц
            if (counterparty1CUpp.Tin.Count() == Const.CompanyTinCount && !string.IsNullOrWhiteSpace(counterparty1CUpp.Trrc))// У юридических лиц ИНН состоит из 10 цифр
            {
                var companyRx = DocSyncService.CompaniesRx.FirstOrDefault(x => x.Tin == counterparty1CUpp.Tin && x.Trrc == counterparty1CUpp.Trrc);
                if (companyRx == null)
                    throw new ApplicationException($"Не удалось создать карточку документа {GetNameAccountingDoc(accountingDoc1CUpp)} с id1CDo = {accountingDoc1CUpp.Id}. " +
                        $"Организация c ИНН = {counterparty1CUpp.Tin} и КПП = {counterparty1CUpp.Trrc} в Rx не найдена");
                return companyRx.Id;
            }

            if (counterparty1CUpp.Tin.Count() == Const.PersonTinCount) // У физических лиц ИНН состоит из 12 цифр
            {
                var personRx = DocSyncService.PersonsRx.FirstOrDefault(x => x.Tin == counterparty1CUpp.Tin);
                if (personRx == null)
                    throw new ApplicationException($"Не удалось создать карточку документа {GetNameAccountingDoc(accountingDoc1CUpp)} с id1CDo = {accountingDoc1CUpp.Id}. " +
                        $"Персона c ИНН = {counterparty1CUpp.Tin} в Rx не найдена");
                return personRx.Id;
            }

            throw new ApplicationException($"Не удалось создать карточку документа {GetNameAccountingDoc(accountingDoc1CUpp)} с id1CDo = {accountingDoc1CUpp.Id}. " +
                $"У контрагента с Id1CUpp = {counterparty1CUpp.Id} введены некорректные данные (ИНН или КПП)");
        }

        public static string GetNameAccountingDoc(AccountingDoc1CUpp accountingDoc1CUpp)
        {
            return $"{accountingDoc1CUpp.DocumentKind} № {accountingDoc1CUpp.Number} от {accountingDoc1CUpp.Date} {accountingDoc1CUpp.Counterparty.Name}";
        }
    }
}
