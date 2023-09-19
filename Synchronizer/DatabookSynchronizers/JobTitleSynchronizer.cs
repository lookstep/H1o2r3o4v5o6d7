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
    public static class JobTitleSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            var jobTitlesRx = RepositoryRx.GetListSelectIdAndId1CUpp<JobTitleRx>();

            var count = Repository1CUpp.GetJobTitle1CUppCount();
            var iterationCount = (int)Math.Ceiling(count / (double)DatabookSyncService.Top);

            for (var i = 0; i < iterationCount; i++)
            {
                var skip = i * DatabookSyncService.Top;
                var jobTitles1CUpp = Repository1CUpp.GetJobTitles1CUpp(skip, DatabookSyncService.Top);
                CreateOrUpdateJobTitles(jobTitles1CUpp, jobTitlesRx);
            }
        }

        private static void CreateOrUpdateJobTitles(List<JobTitle1CUpp> jobTitles1C, List<JobTitleRx> jobTitlesRx)
        {
            Parallel.ForEach(jobTitles1C, jobTitle1C =>
            {
                try
                {
                    CreateOrUpdateJobTitle(jobTitlesRx, jobTitle1C);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrUpdateJobTitle(List<JobTitleRx> jobTitlesRx, JobTitle1CUpp jobTitle1C)
        {
            var jobTitleRxId = jobTitlesRx.FirstOrDefault(j => j.Id1CUpp == jobTitle1C.Id)?.Id;

            var jobTitle = new JobTitleRx 
            { 
                Name = jobTitle1C.Name,
                Code = jobTitle1C.Code
            };

            if (jobTitleRxId != null)
            {
                RepositoryRx.Update(jobTitle, jobTitleRxId.Value);
                return;
            }

            jobTitle.Id1CUpp = jobTitle1C.Id;
            jobTitle.Status = "Active";
            RepositoryRx.Create(jobTitle);
        }
    }
}
