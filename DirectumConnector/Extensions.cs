using DirectumConnector.DatabookModels;
using System;
using System.Linq;

namespace DirectumConnector
{
    public static class Extensions
    {
        public static void FillPersonName(this PersonRx person, string name)
        {
            var nameParts = name.Split(" ");
            if (nameParts.Count() < 2)
                throw new ApplicationException($"У контрагента {name} с типом \"ФизЛицо\" должны быть Фамилия и Имя");

            person.FirstName = nameParts[1];
            person.LastName = nameParts[0];
            if (nameParts.Count() > 2)
                person.MiddleName = string.Join(" ", nameParts.Skip(2));
        }
    }
}
