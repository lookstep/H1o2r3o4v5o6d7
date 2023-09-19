using Connector1CUpp.DatabookModels;
using Connector1CUpp.DocModels;
using System;
using System.Collections.Generic;

namespace Connector1CUpp
{
    public static class Config1CUpp
    {
        public static string Url { get; set; }

        public static string Login { get; set; }

        public static string Password { get; set; }

        private static Dictionary<Type, string> TypeNames1C { get; set; } = new Dictionary<Type, string>();

        static Config1CUpp()
        {
            TypeNames1C.Add(typeof(PersonNameRegister1CUpp), "InformationRegister_ФИОФизЛиц");
            TypeNames1C.Add(typeof(Person1CUpp), "Catalog_ФизическиеЛица");
            TypeNames1C.Add(typeof(BusinessUnit1CUpp), "Catalog_Организации");
            TypeNames1C.Add(typeof(Department1CUpp), "Catalog_ПодразделенияОрганизаций");
            TypeNames1C.Add(typeof(JobTitle1CUpp), "Catalog_ДолжностиОрганизаций");
            TypeNames1C.Add(typeof(Employee1CUpp), "Catalog_СотрудникиОрганизаций");
            TypeNames1C.Add(typeof(Counterparty1CUpp), "Catalog_Контрагенты");
            TypeNames1C.Add(typeof(AccountingDoc1CUpp), "Document_ПоступлениеТоваровУслуг");
            TypeNames1C.Add(typeof(Contract1CUpp), "Catalog_ДоговорыКонтрагентов");
            TypeNames1C.Add(typeof(Currency1CUpp), "Catalog_Валюты");
        }

        public static string GetTypeName1C(Type type) => TypeNames1C[type];
    }
}
