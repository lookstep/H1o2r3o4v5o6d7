using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using DirectumConnector.DatabookModels;
using DirectumConnector.DocModels;
using System.Linq;

namespace DirectumConnector
{
    public static class RepositoryRx
    {

        public static List<CaseFileRx> GetCaseFileByIndex()
        {
            return GetList<CaseFileRx>($"?$filter=Status eq 'Active'");
        }

        public static List<DepartmentRx> GetDepartmentsRxIdCode1CUpp()
        {
            return GetList<DepartmentRx>($"?$select=Id,Code,Id1CUpp");
        }

        public static PersonRx GetPersonsRx(string tin)
        {
            return GetList<PersonRx>($"?$filter=TIN eq 'tin'").FirstOrDefault();
        }

        public static CompanyRx GetCompanyRx(string tin, string trrc)
        {
            return GetList<CompanyRx>($"?$filter=TIN eq '{tin}' and TRRC eq '{trrc}'").FirstOrDefault();
        }

        public static BusinessUnitRx GetBusinessUnit(string businessUnitId1CUpp)
        {
            return GetList<BusinessUnitRx>($"?$filter=Id1CUpp eq '{businessUnitId1CUpp}'").FirstOrDefault();
        }

        public static BusinessUnitRx GetBusinessUnit(int businessUnitIdRx)
        {
            return Get<BusinessUnitRx>(businessUnitIdRx);
        }

        public static List<DocumentKind1CUppSettingRx> GetDocumentKind1CUppSettingsRx()
        {
            return GetList<DocumentKind1CUppSettingRx>("?$expand=DocumentKinds($expand=DocumentKind($expand=DocumentType))");
        }

        public static List<DocumentKind1CDoSettingRx> GetDocumentKind1CDoSettingRx()
        {
            return GetList<DocumentKind1CDoSettingRx>("?$expand=DocumentKindRx($expand=DocumentType($select=Id,DocumentTypeGuid))");
        }

        public static List<ContractDocKind1CUppSettingRx> GetContractDocKind1CUppSettingRx()
        {
            return GetList<ContractDocKind1CUppSettingRx>("?$expand=DocumentKind($expand=DocumentType($select=Id,DocumentTypeGuid))");
        }

        public static List<ContractRx> GetContractsRxSelectIdCodeId1CDoLifeCycleState()
        {
            var selectionString = "?$select=Id,CodeContractualDocument,Id1CDo,LifeCycleState";
            return GetList<ContractRx>(selectionString);
        }
        public static List<ContractRx> GetContractsRx(string code)
        {
            var selectionString = $"?$filter=CodeContractualDocument eq '{code}'";
            return GetList<ContractRx>(selectionString);
        }

        public static List<EmployeeRx> GetEmployeeRxSelectIdDepartmentPersonnelNumber()
        {
            var selectionString = "?$expand=Department($select=Id)&$select=Id,PersonnelNumber,Id1CUpp";
            return GetList<EmployeeRx>(selectionString);
        }

        public static List<T> GetListSelectIdAndId1CUpp<T>()
        {
            var selectionString = "?$filter=Id1CUpp ne null &$select=Id,Id1CUpp";
            return GetList<T>(selectionString);
        }

        public static List<T> GetListSelectIdAndId1CDo<T>()
        {
            var selectionString = "?$filter=Id1CDo ne null &$select=Id,Id1CDo";
            return GetList<T>(selectionString);
        }

        public static List<T> GetList<T>(string selectionString = null)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRx<T>()}");

            if (selectionString != null)
                uri = new Uri(uri, selectionString);

            var response = GetHttpResponseMessage(HttpMethod.Get, uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new List<T>();

            var jsonResult = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<ResultListRx<T>>(jsonResult);

            return result.Value;
        }

        public static T Get<T>(int idRx)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRx<T>()}({idRx})");
            var response = GetHttpResponseMessage(HttpMethod.Get, uri);
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }

        public static HttpResponseMessage CreateVersion(int docId, string path)
        {
            var methodName = "CreateVersion";
            var methodParams = new { docId, path };
            return RequestInegrationMethod(methodName, methodParams);
        }
        public static HttpResponseMessage SendNotificationToAdmins(string message)
        {
            var methodName = "SendNotificationToAdmins";
            var methodParams = new { message };
            return RequestInegrationMethod(methodName, methodParams);
        }

        public static HttpResponseMessage SendApprovalTaskForAccDoc(int docId)
        {
            var methodName = "SendApprovalTaskForAccDoc";
            var methodParams = new { docId };
            return RequestInegrationMethod(methodName, methodParams);
        }

        public static HttpResponseMessage SendApprovalTaskForDoc1CDo(int docId)
        {
            var methodName = "SendApprovalTaskForDoc1CDo";
            var methodParams = new { docId };
            return RequestInegrationMethod(methodName, methodParams);
        }

        public static HttpResponseMessage CreateDocumentRelation(int sourseDocId, int targetDocId)
        {
            var methodName = "CreateDocumentRelation";
            var methodParams = new { sourseDocId, targetDocId };
            return RequestInegrationMethod(methodName, methodParams);
        }

        public static HttpResponseMessage RequestInegrationMethod(string methodName, object methodParams)
        {
            var uri = new Uri($"{GetBaseUri()}Integration1C/{methodName}");
            return SendRequest(uri, HttpMethod.Post, methodParams);
        }


        public static T Create<T>(T entity)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRx<T>()}");
            var response = SendRequest(uri, HttpMethod.Post, entity);
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }


        public static HttpResponseMessage CreateDocumentByKind(DocumentBase document)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRxByGuid(document.DocumentKind.DocumentType.DocumentTypeGuid)}");
            return SendRequest(uri, HttpMethod.Post, document);
        }

        public static HttpResponseMessage Update<T>(T entity, int idRx)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRx<T>()}({idRx})");
            return SendRequest(uri, HttpMethod.Patch, entity);
        }

        public static HttpResponseMessage UpdateDocumentByKind(DocumentBase document, int idRx)
        {
            var uri = new Uri($"{GetBaseUri()}{ConfigRx.GetTypeNameRxByGuid(document.DocumentKind.DocumentType.DocumentTypeGuid)}({idRx})");
            return SendRequest(uri, HttpMethod.Patch, document);
        }

        private static HttpResponseMessage SendRequest(Uri uri, HttpMethod metod, object entity)
        {
            var body = JsonConvert.SerializeObject(entity);
            return GetHttpResponseMessage(metod, uri, body);
        }

        public static bool IsConnected()
        {
            var responseMessage = GetHttpResponseMessage(HttpMethod.Get, GetBaseUri());
            return responseMessage.IsSuccessStatusCode;
        }

        private static HttpResponseMessage GetHttpResponseMessage(HttpMethod httpMethod, Uri uri, string body = null)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            HttpResponseMessage responseMessage;

            using (HttpClient client = new HttpClient())
            {
                var authBase64 = Base64Encode($"{ConfigRx.Login}:{ConfigRx.Password}");
                request.Headers.Add("Authorization", $"Basic {authBase64}");
                request.Headers.Add("Accept", "application/json");
                if (request.Method != HttpMethod.Post)
                    request.Headers.Add("Return", "minimal");

                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Patch)
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                responseMessage = client.SendAsync(request).Result;

                if (!responseMessage.IsSuccessStatusCode)
                {
                    var withBodyString = body != null ? $"с телом {body}" : null;
                    throw new Exception($"При выполнении запроса {request.RequestUri} с методом {request.Method} {withBodyString}" +
                        $" возникла ошибка: {responseMessage.StatusCode} {responseMessage.Content.ReadAsStringAsync().Result}");
                }
            }
            return responseMessage;
        }

        private static Uri GetBaseUri()
        {
            return new Uri($"{ConfigRx.Url}/odata/");
        }

        private static string Base64Encode(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }
    }
}
