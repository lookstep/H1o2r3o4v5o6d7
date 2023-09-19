using Connector1CUpp.DatabookModels;
using Connector1CUpp.DocModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace Connector1CUpp
{
    public static class Repository1CUpp
    {

        public static List<AccountingDoc1CUpp> GetAccountingDocs1CUpp(int skip, int top, DateTime startSyncDate)
        {
            return GetList<AccountingDoc1CUpp>($"?$filter=DeletionMark eq false and Date ge datetime'{startSyncDate:yyyy'-'MM'-'dd'T'HH':'mm':'ss}'" +
                                                  $"&$expand=ДоговорКонтрагента,Контрагент" + 
                                                  $"&$select=Ref_Key,Number,Date,НомерВходящегоДокумента,ДатаВходящегоДокумента," +
                                                           $"ОтветственныйЗаДокументВАрхиве_Key,СуммаДокумента,ЗначенияАрхив," +
                                                           $"ДоговорКонтрагента/Ref_Key,ДоговорКонтрагента/Code,ДоговорКонтрагента/ВалютаВзаиморасчетов_Key,ДоговорКонтрагента/Description," +
                                                           $"Контрагент/Ref_Key,Контрагент/ИНН,Контрагент/КПП,ВалютаДокумента_Key" +
                                                  $"&$skip={skip}" +
                                                  $"&$top={top}");
        }

        public static int GetAccountingDocCount(DateTime startSyncDate)
        {
            return GetCount<AccountingDoc1CUpp>($"?$filter=DeletionMark eq false and Date ge datetime'{startSyncDate:yyyy'-'MM'-'dd'T'HH':'mm':'ss}'");
        }

        public static List<Person1CUpp> GetPersons1CUpps(List<PersonNameRegister1CUpp> registerNameEntries1С, int skip, int top)
        {
            var persons1C = GetList<Person1CUpp>($"?$filter=DeletionMark eq false" +
                                                 $"&$orderby=Ref_Key" +
                                                 $"&$skip={skip}" +
                                                 $"&$top={top}");
            return persons1C;
        }

        public static List<Counterparty1CUpp> GetCounterparties1CUpps(int skip, int top)
        {
            return GetList<Counterparty1CUpp>($"?$filter=ЮрФизЛицо eq 'ЮрЛицо' and DeletionMark eq false " +
                                              $"&$orderby=Ref_Key" +
                                              $"&$skip={skip}" +
                                              $"&$top={top}");
        }

        public static List<Department1CUpp> GetDepartments1CUpp(int skip, int top)
        {
            return GetList<Department1CUpp>($"?$filter=DeletionMark eq false" +
                                            $"&$orderby=Ref_Key" +
                                            $"&$skip={skip}" +
                                            $"&$top={top}");
        }

        public static List<Department1CUpp> GetDepartments1CUppIdAndParentDepartment()
        {
            return GetList<Department1CUpp>("?$filter=DeletionMark eq false" +
                                            "&$select=Ref_Key,Parent_Key");
        }

        public static List<JobTitle1CUpp> GetJobTitles1CUpp(int skip, int top)
        {
            return GetList<JobTitle1CUpp>($"?$filter=DeletionMark eq false" +
                                          $"&$orderby=Ref_Key" +
                                          $"&$skip={skip}" +
                                          $"&$top={top}");
        }

        public static List<Employee1CUpp> GetEmployees1CUpps(int skip, int top)
        {
            return GetList<Employee1CUpp>($"?$filter=DeletionMark eq false and ВидДоговора eq 'ТрудовойДоговор'" +
                                          $"&$orderby=Ref_Key" +
                                          $"&$skip={skip}" +
                                          $"&$top={top}");
        }

        public static int GetCounterparty1CUppCount()
        {
            return GetCount<Counterparty1CUpp>($"?$filter=ЮрФизЛицо eq 'ЮрЛицо' or ЮрФизЛицо eq 'ФизЛицо' and DeletionMark eq false");
        }

        public static int GetPerson1CUppCount()
        {
            return GetCount<Person1CUpp>("?$filter=DeletionMark eq false");
        }

        public static int GetDepartment1CUppCount()
        {
            return GetCount<Department1CUpp>("?$filter=DeletionMark eq false");
        }

        public static int GetJobTitle1CUppCount()
        {
            return GetCount<JobTitle1CUpp>("?$filter=DeletionMark eq false");
        }

        public static int GetEmployee1CUppCount()
        {
            return GetCount<Employee1CUpp>("?$filter=DeletionMark eq false");
        }


        public static List<T> GetList<T>(string selectionString = null)
        {
            var baseUri = GetBaseUri();
            var uri = new Uri(baseUri, Config1CUpp.GetTypeName1C(typeof(T)));
            if (selectionString != null)
                uri = new Uri(uri, selectionString);
            var responseMessage = GetHttpResponseMessage(uri);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            var entitiesResult = JsonConvert.DeserializeObject<ResultList1CUpp<T>>(result);
            return entitiesResult.Value;
        }

        private static int GetCount<T>(string selectionString = null)
        {
            var baseUri = GetBaseUri();
            var typeName = Config1CUpp.GetTypeName1C(typeof(T));

            // В odata 3.0 (1CUpp) запрос на получение количества объектов формируется как
            // http://{baseUrl}/{typeName}/$count?$filter=...
            var uri = new Uri($"{baseUri}{typeName}/$count{selectionString}");
            var responseMessage = GetHttpResponseMessage(uri);
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<int>(result);
        }

        public static bool IsConnected()
        {
            var responseMessage = GetHttpResponseMessage(GetBaseUri());
            return responseMessage.IsSuccessStatusCode;
        }

        private static HttpResponseMessage GetHttpResponseMessage(Uri uri)
        {
            using HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var authBase64 = Base64Encode($"{Config1CUpp.Login}:{Config1CUpp.Password}");
            request.Headers.Add("Authorization", $"Basic {authBase64}");
            request.Headers.Add("Accept", "application/json");

            var responseMessage = client.SendAsync(request).Result;

            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception($"При выполнении запроса {request.RequestUri} с методом {request.Method}" +
                    $" возникла ошибка: {responseMessage.StatusCode} {responseMessage.Content.ReadAsStringAsync().Result}");
            return responseMessage;
        }

        private static Uri GetBaseUri()
        {
            return new Uri($"{Config1CUpp.Url}/odata/standard.odata/");
        }

        private static string Base64Encode(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }
    }
}
