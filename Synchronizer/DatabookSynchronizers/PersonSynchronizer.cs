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
    public static class PersonSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            var personsRx = RepositoryRx.GetList<PersonRx>();
            var registerNameEntries1С = Repository1CUpp.GetList<PersonNameRegister1CUpp>();

            var count = Repository1CUpp.GetPerson1CUppCount();
            var iterationCount = (int)Math.Ceiling(count / (double)DatabookSyncService.Top);

            for (var i = 0; i < iterationCount; i++)
            {
                var skip = i * DatabookSyncService.Top;
                var persons1C = GetPersons1CUpp(registerNameEntries1С, skip, DatabookSyncService.Top);
                CreateOrUpdatePersons(persons1C, personsRx);
            }
        }

        private static List<Person1CUpp> GetPersons1CUpp(List<PersonNameRegister1CUpp> registerNameEntries1С, int skip, int top)
        {
            var persons1C = Repository1CUpp.GetPersons1CUpps(registerNameEntries1С, skip, top);

            foreach (var person1C in persons1C)
            {
                var nameEntry = registerNameEntries1С.Where(x => x.Person1CUppId == person1C.Id).OrderBy(x => x.Period).LastOrDefault();
                if (nameEntry == null)
                {
                    var exception = new ApplicationException($"В РеестреФИОФизЛиц нет записи для физического лица {person1C.FullName} c Id1CUpp = {person1C.Id}");
                    Logger.Error(exception);
                    Common.TrySendNotificationToAdmins(exception);
                    continue;
                }

                person1C.FirstName = nameEntry.FirstName;
                person1C.LastName = nameEntry.LastName;
                person1C.MiddleName = nameEntry.MiddleName;
            }
            return persons1C;
        }



        private static void CreateOrUpdatePersons(List<Person1CUpp> persons1C, List<PersonRx> personsRx)
        {
            Parallel.ForEach(persons1C, person1C =>
            {
                try
                {
                    CreateOrUpdatePerson(personsRx, person1C);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrUpdatePerson(List<PersonRx> personsRx, Person1CUpp person1C)
        {
            var personRxId = personsRx.FirstOrDefault(p => p.Id1CUpp == person1C.Id)?.Id;
            var person = new PersonRx
            {
                Name = person1C.FullName,
                FirstName = person1C.FirstName,
                LastName = person1C.LastName,
                MiddleName = person1C.MiddleName,
                DateOfBirth = person1C.DateOfBirth == DateTime.MinValue ? null : person1C.DateOfBirth
            };

            if (personRxId != null)
            {
                RepositoryRx.Update(person, personRxId.Value);
                return;
            }

            person.Id1CUpp = person1C.Id;
            person.Status = "Active";

            RepositoryRx.Create(person);
        }
    }
}
