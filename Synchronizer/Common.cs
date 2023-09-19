using Connector1CUpp;
using DirectumConnector;
using DirectumConnector.DatabookModels;
using System;

namespace Synchronizer
{
    public class Common
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool TrySetBusinessUnit(ref BusinessUnitRx businessUnitRx)
        {
            try
            {
                businessUnitRx = RepositoryRx.GetBusinessUnit(ConfigRx.BusinessUnitIdRx);

                if (businessUnitRx == null)
                    throw new ApplicationException($"Наша организация в Rx с id = {ConfigRx.BusinessUnitIdRx} не найдена");

                if (string.IsNullOrWhiteSpace(businessUnitRx.Id1CUpp))
                    throw new ApplicationException("У Нашей огранизации в Rx не заполнено свойство Ид записи 1С: УПП");

                if (string.IsNullOrWhiteSpace(businessUnitRx.Id1CDo))
                    throw new ApplicationException("У Нашей огранизации в Rx не заполнено свойство Ид записи 1С: ДО");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Common.TrySendNotificationToAdmins(ex);
                return false;
            }
            return true;
        }

        public static void SendSuccessMessage(string successMessage)
        {
            Logger.Debug(successMessage);
            try
            {
                RepositoryRx.SendNotificationToAdmins(successMessage);
            }
            catch (Exception excep)
            {
                Logger.Error(excep);
            }
        }

        public static void TrySendNotificationToAdmins(Exception ex)
        {
            try
            {
                RepositoryRx.SendNotificationToAdmins(ex.Message);
            }
            catch (Exception exсep)
            {
                Logger.Error(exсep);
            }
        }

        internal static void TrySendApprovalTaskForDoc1CDo(int id)
        {
            try
            {
                RepositoryRx.SendApprovalTaskForDoc1CDo(id);
            }
            catch (Exception excep)
            {
                Logger.Error(excep);
                Synchronizer.Common.TrySendNotificationToAdmins(excep);
            }
        }
    }
}
