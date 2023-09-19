using Connector1CDO;
using DirectumConnector;
using DirectumConnector.DocModels;
using StandardODATA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synchronizer.DocSynchronizers
{
    internal class ContractRelation1CDoSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static void Sync(DateTime startSyncDate, int top)
        {
            var contractsRx = RepositoryRx.GetContractsRxSelectIdCodeId1CDoLifeCycleState();

            var count = Repository1CDo.GetInternalDocumentsCount(startSyncDate, Config1CDo.ContractId1CDo);
            var iterationCount = (int)Math.Ceiling(count / (double)top);

            var additionalDetails = Repository1CDo.GetInternalDocumentAdditionalDetails();

            for (int i = 0; i < iterationCount; i++)
            {
                CreateRelations(startSyncDate, top, contractsRx, i);
            }
        }

        private static void CreateRelations(DateTime startSyncDate, int top, List<ContractRx> contractsRx, int i)
        {
            int skip = top * i;
            var contracts1CDo = Repository1CDo.GetInternalDocuments(startSyncDate, Config1CDo.ContractId1CDo, skip, top);
            foreach (var contract1CDo in contracts1CDo)
            {
                try
                {
                    CreateRelation(contractsRx, contract1CDo);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Synchronizer.Common.TrySendNotificationToAdmins(ex);
                }
            }
        }

        private static void CreateRelation(List<ContractRx> contractsRx, Catalog_ВнутренниеДокументы contract1CDo)
        {
            var id1CDo = contract1CDo.Ref_Key.ToString();

            var relation = Repository1CDo.GetRelation(id1CDo);
            if (relation == null)
                return;

            var sourceDoc = contractsRx.FirstOrDefault(x => x.Id1CDo == relation.Документ);
            var targetDoc = contractsRx.FirstOrDefault(x => x.Id1CDo == relation.СвязанныйДокумент);

            if (sourceDoc == null)
                throw new ApplicationException($"Связь между документами Id1CDo = {relation.Документ} и Id1CDo = {relation.СвязанныйДокумент} не создана. " +
                    $"Договорной документ с Id1CDo = {relation.Документ} не найден в системе Rx");
            else if (targetDoc == null)
                throw new ApplicationException($"Связь между документами Id1CDo = {relation.Документ} и Id1CDo = {relation.СвязанныйДокумент} не создана. " +
                    $"Договорной документ с Id1CDo = {relation.СвязанныйДокумент} не найден в системе Rx");

            RepositoryRx.CreateDocumentRelation(sourceDoc.Id, targetDoc.Id);
        }
    }
}
