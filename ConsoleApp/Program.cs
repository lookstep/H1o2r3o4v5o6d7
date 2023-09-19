using Connector1CDO;
using Connector1CUpp;
using DirectumConnector;
using Newtonsoft.Json;
using Synchronizer;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            ConfigRx.Url = "http://172.21.23.126/Integration";
            ConfigRx.Login = "Administrator";
            ConfigRx.Password = "1Qwerty";
            ConfigRx.BusinessUnitIdRx = 106;

            Config1CUpp.Url = "http://192.168.247.247/UPP";
            Config1CUpp.Login = "Админ";
            Config1CUpp.Password = "123321";

            Config1CDo.Url = "http://192.168.247.247/DOBR";
            Config1CDo.Login = "Администратор";
            Config1CDo.Password = "123";
            Config1CDo.PathToStorage = "C:\\Users\\Администратор\\Desktop\\DoStorage\\";

            Config1CDo.DeputyCeoPersonnelOrderId1CDo = "691271c1-8291-11ec-932d-00259049bc28";
            Config1CDo.CeoPersonnelOrderId1CDo = "5614941f-42f2-11ec-9bdd-00259049bc28";
            Config1CDo.ContractId1CDo = "3e6b0abb-42f2-11ec-9bdd-00259049bc28";
            Config1CDo.OrderId1CDo = "5971f9cc-8efc-11ec-a0bd-00259049bc28";

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var isConnectedRx = SyncService.IsConnectedToRx();
            var isConnected1CUpp = SyncService.IsConnectedTo1CUpp();
            var isConnected1CDo = SyncService.IsConnectedTo1CDo();

            DatabookSyncService.Sync();
            var startSyncDate = new DateTime(2021, 01, 01, 0, 0, 0);
            
            DocSyncService.SyncContracts1CDo(startSyncDate, 10);
            DocSyncService.SyncAccountingDoc1CUpp(startSyncDate, 10);
            DocSyncService.SyncOrders1CDo(startSyncDate, 10);
        }
    }
}
