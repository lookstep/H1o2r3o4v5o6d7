using Connector1CUpp;
using Connector1CUpp.DatabookModels;
using DirectumConnector;
using DirectumConnector.DatabookModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synchronizer.DatabookSynchronizers
{
    public static class CounterpartySinchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            var companiesRx = RepositoryRx.GetList<CompanyRx>();
            var personsRx = RepositoryRx.GetList<PersonRx>();

            var count = Repository1CUpp.GetCounterparty1CUppCount();
            var iterationCount = (int)Math.Ceiling(count / (double)DatabookSyncService.Top);

            for (var i = 0; i < iterationCount; i++)
            {
                int skip = DatabookSyncService.Top * i;
                var counterparties1CUpp = Repository1CUpp.GetCounterparties1CUpps(skip, DatabookSyncService.Top);
                CreateOrUpdateCounterparties(counterparties1CUpp, personsRx, companiesRx);
            }
        }

        private static void CreateOrUpdateCounterparties(List<Counterparty1CUpp> counterparties1CUpp, List<PersonRx> personsRx, List<CompanyRx> companiesRx)
        {
            Parallel.ForEach(counterparties1CUpp, counterparty1CUpp =>
            {
                try
                {
                    CreateOrUpdateCounterparty(personsRx, companiesRx, counterparty1CUpp);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrUpdateCounterparty(List<PersonRx> personsRx, List<CompanyRx> companiesRx, Counterparty1CUpp counterparty1CUpp)
        {
            // По требованию заказчика это единственный критерий отбора юр и физ лиц
            if (counterparty1CUpp.Tin.Count() == Const.CompanyTinCount && !string.IsNullOrWhiteSpace(counterparty1CUpp.Trrc))// У юридических лиц ИНН состоит из 10 цифр
            {
                var companyRxId = companiesRx.FirstOrDefault(x => x.Tin == counterparty1CUpp.Tin && x.Trrc == counterparty1CUpp.Trrc && x.Id1CUpp == counterparty1CUpp.Id)?.Id;
                CreateOrUpdateCompany(counterparty1CUpp, companyRxId);
                return;
            }

            if (counterparty1CUpp.Tin.Count() == Const.PersonTinCount) // У физических лиц ИНН состоит из 12 цифр
            {
                var personRxId = personsRx.FirstOrDefault(x => x.Tin == counterparty1CUpp.Tin && x.Id1CUpp == counterparty1CUpp.Id)?.Id;
                CreateOrUpdatePerson(counterparty1CUpp, personRxId);
                return;
            }

            throw new ApplicationException($"У контрагента {counterparty1CUpp.FullName} с Id1CUpp = {counterparty1CUpp.Id} введены некорректные данные (ИНН или КПП)");
        }

        private static void CreateOrUpdateCompany(Counterparty1CUpp counterparty1C, int? companyRxId)
        {
            var company = new CompanyRx
            {
                Code = counterparty1C.Code,
                Name = counterparty1C.Name,
                Psrn = counterparty1C.Psrn,
                LegalName = counterparty1C.FullName
            };

            if (companyRxId != null)
            {
                RepositoryRx.Update(company, companyRxId.Value);
                return;
            }

            company.Id1CUpp = counterparty1C.Id;
            company.Status = "Active";
            company.Tin = counterparty1C.Tin;
            company.Trrc = counterparty1C.Trrc;

            RepositoryRx.Create(company);
        }

        private static void CreateOrUpdatePerson(Counterparty1CUpp counterparty1C, int? personRxId)
        {
            var person = new PersonRx();
            person.FillPersonName(counterparty1C.Name);
            person.Code = counterparty1C.Code;
            person.Name = counterparty1C.Name;

            if (personRxId != null)
            {
                RepositoryRx.Update(person, personRxId.Value);
                return;
            }

            person.Tin = counterparty1C.Tin;
            person.Id1CUpp = counterparty1C.Id;
            person.Status = "Active";
            RepositoryRx.Create(person);
        }
    }
}

