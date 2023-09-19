using Connector1CDO;
using DirectumConnector;
using DirectumConnector.DatabookModels;
using DirectumConnector.DocModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using StandardODATA;

namespace Synchronizer.DocSynchronizers
{
    public static class Common
    {

        public static void CreateVersion(int docIdRx, string Id1CDo, List<Catalog_Файлы> files1CDo)
        {
            var file1CDo = files1CDo.FirstOrDefault(x => x.ВладелецФайла == Id1CDo);
            if (file1CDo.ТекущаяВерсияПутьКФайлу == null)
                return;

            var path = Path.Combine(Config1CDo.PathToStorage, file1CDo.ТекущаяВерсияПутьКФайлу);
            RepositoryRx.CreateVersion(docIdRx, path);
        }

        internal static List<Catalog_Файлы> GetFiles1CDo(IEnumerable<Catalog_ВнутренниеДокументы> internalDocs1CDo)
        {
            var files1CDo = new List<Catalog_Файлы>();
            foreach (var internalDoc1CDo in internalDocs1CDo)
            {
                var file1CDo = Repository1CDo.GetFiles(internalDoc1CDo.Ref_Key.ToString());
                files1CDo.Add(file1CDo);
            }
            return files1CDo;
        }

        public static DocumentBase Create(DocumentBase document)
        {
            var responseMessage = RepositoryRx.CreateDocumentByKind(document);

            var jsonResult = responseMessage.Content.ReadAsStringAsync().Result;
            var createdEntity = JsonConvert.DeserializeObject<DocumentBase>(jsonResult);
            return createdEntity;
        }

        public static DocumentKindRx GetDocumentKindRx(StandardODATA.Catalog_ВнутренниеДокументы document)
        {
            var documentKind1CDoSettingRx = DocSyncService.DocumentKind1CDoSettingsRx.FirstOrDefault(x => x.DocumentKindId1CDo == document.ВидДокумента_Key.ToString());
            if (documentKind1CDoSettingRx == null)
                throw new ApplicationException($"Для вида документа 1СДО {document.ВидДокумента.Description} нет настройки вида документа в Rx");

            return documentKind1CDoSettingRx.DocumentKindRx;
        }
    }
}
