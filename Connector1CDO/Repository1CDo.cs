using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DirectumConnector.DocModels;
using StandardODATA;

namespace Connector1CDO
{
    public class Repository1CDo
    {
        public static bool IsConnected()
        {
            var enterprise = GetEnterprise();
            var currency = enterprise.Catalog_Валюты; //Проверяем подключение на валютах, т.к. это справочник из коробки 1с
            try
            {
               var result = ExecuteAsync(currency).Result;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static InformationRegister_СвязиДокументов GetRelation(string docId1CDo)
        {
            var enterprise = GetEnterprise();
            var internalDocuments = enterprise.InformationRegister_СвязиДокументов
                .AddQueryOption("$expand", "ТипСвязи")
                // Тип "Catalog_ВнутренниеДокументы" забиваем гвоздями, т.к. иначе увеличивается сложность и количесво кода
                // Аналогично тип связи "Дог.документ имеет доп.документы"
                .AddQueryOption("$filter", $"Документ eq cast(guid'{docId1CDo}', 'Catalog_ВнутренниеДокументы') " +
                                           $"and ТипСвязи/Description eq 'Явл.доп.дркументом к дог.документу' " +
                                           $"and ТипСвязи/DeletionMark eq false ");

            return ExecuteAsync(internalDocuments).Result.FirstOrDefault(); // Берем первую попавшуюся связь, т.к. в рамказ ПР она должна быть одна
        }

        public static IEnumerable<Catalog_ЗначенияСвойствОбъектов> GetObjectPropertyValue()
        {
            var enterprise = GetEnterprise();
            var additionalDetails = enterprise.Catalog_ЗначенияСвойствОбъектов
                .AddQueryOption("$filter", $"DeletionMark eq false");

            return ExecuteAsync(additionalDetails).Result;
        }


        public static IEnumerable<Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты> GetInternalDocumentAdditionalDetails()
        {
            var enterprise = GetEnterprise();
            var additionalDetails = enterprise.Catalog_ВнутренниеДокументы_ДополнительныеРеквизиты
                .AddQueryOption("$expand", "Свойство")
                .AddQueryOption("$select", "Ref_Key,Значение,LineNumber,Значение_Type,ТекстоваяСтрока,Свойство/Заголовок,Свойство/Ref_Key");

            return ExecuteAsync(additionalDetails).Result;
        }

        public static IEnumerable<Catalog_ВнутренниеДокументы> GetInternalDocuments(DateTime startSyncDate, string docKindId1CDo, int skip, int top)
        {
            var enterprise = GetEnterprise();
            var internalDocuments = enterprise.Catalog_ВнутренниеДокументы
                .AddQueryOption("$expand", "Контрагент,ВидДокумента,Подготовил,Ответственный,Подразделение,Дело/НоменклатураДел")
                .AddQueryOption("$filter", $"ВидДокумента_Key eq guid'{docKindId1CDo}' " +
                                           $"and DeletionMark eq false " + 
                                           $"and ДатаРегистрации ge datetime'{startSyncDate:yyyy'-'MM'-'dd'T'HH':'mm':'ss}'")
                .AddQueryOption("$skip", skip)
                .AddQueryOption("$top", top);

            return ExecuteAsync(internalDocuments).Result;
        }

        public static int GetInternalDocumentsCount(DateTime startSyncDate, string docKindId1CDo)
        {
            var enterprise = GetEnterprise();
            var internalDocuments = enterprise.Catalog_ВнутренниеДокументы
                .AddQueryOption("$expand", "Контрагент,ВидДокумента,Подготовил,Ответственный")
                .AddQueryOption("$filter", $"ВидДокумента_Key eq guid'{docKindId1CDo}' " +
                                           $"and DeletionMark eq false " +
                                           $"and ДатаРегистрации ge datetime'{startSyncDate:yyyy'-'MM'-'dd'T'HH':'mm':'ss}'");

            return ExecuteAsync(internalDocuments).Result.Count();
        }

        public static Catalog_Пользователи GetUser(string id1CDo)
        {
            var enterprise = GetEnterprise();
            var persons = enterprise.Catalog_Пользователи
                .AddQueryOption("$filter", $"Ref_Key eq guid'{id1CDo}' and DeletionMark eq false");
            return ExecuteAsync(persons).Result.FirstOrDefault();
        }

        public static Catalog_Файлы GetFiles(string docId1CDo)
        {
            var enterprise = GetEnterprise();
            var internalDocuments = enterprise.Catalog_Файлы
                .AddQueryOption("$expand", "ТекущаяВерсия")
                .AddQueryOption("$filter", $"ВладелецФайла eq cast (guid'{docId1CDo}','Catalog_ВнутренниеДокументы') " +
                                           $"and DeletionMark eq false")
                .AddQueryOption("$orderby", "ТекущаяВерсияДатаСоздания desc")
                .AddQueryOption("$select", "ВладелецФайла,ТекущаяВерсияДатаСоздания,ТекущаяВерсияПутьКФайлу")
                .AddQueryOption("$top", "1");

            return ExecuteAsync(internalDocuments).Result.FirstOrDefault();
        }


        public static IEnumerable<Catalog_Валюты> GetCurrencies()
        {
            var enterprise = GetEnterprise();
            var currencies = enterprise.Catalog_Валюты;

            return ExecuteAsync(currencies).Result;
        }

        public static async Task<IReadOnlyList<TResult>> ExecuteAsync<TResult>(System.Data.Services.Client.DataServiceQuery<TResult> query)
        {
            var queryTask = Task.Factory.FromAsync(query.BeginExecute(null, null),
                (queryAsyncResult) =>
                {
                    var results = query.EndExecute(queryAsyncResult);
                    return results;
                });

            var collection = await queryTask;
            return collection.ToList();
        }

        public static EnterpriseV8 GetEnterprise()
        {
            var uri = new Uri($"{Config1CDo.Url}/odata/standard.odata/");
            var enterprise = new EnterpriseV8(uri)
            {
                Credentials = new NetworkCredential(Config1CDo.Login, Config1CDo.Password)
            };
            return enterprise;
        }
    }
}
