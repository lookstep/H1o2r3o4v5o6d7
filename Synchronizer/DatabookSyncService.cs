using DirectumConnector.DatabookModels;
using Newtonsoft.Json;
using Synchronizer.DatabookSynchronizers;

namespace Synchronizer
{
    public static class DatabookSyncService
    {
        public static int Top = 100;

        internal static BusinessUnitRx BusinessUnitRx;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            Logger.Debug($"Начата синхронизация справочников");

            if (!Common.TrySetBusinessUnit(ref BusinessUnitRx))
                return;

            DepartmentSynchronizer.Sync();

            JobTitleSynchronizer.Sync();

            PersonSynchronizer.Sync();

            EmployeeSynchronizer.Sync();

            CounterpartySinchronizer.Sync();

            var successMessage = "Синхронизация справочников завершена";
            Common.SendSuccessMessage(successMessage);
        }
    }
}

