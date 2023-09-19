using Connector1CDO;
using Connector1CUpp;
using DirectumConnector;

namespace Synchronizer
{
    public static class SyncService
    {
        public static void SetConfig(ConfigDto configDto)
        {
            ConfigRx.Url = configDto.UrlRx;
            ConfigRx.Login = configDto.LoginRx;
            ConfigRx.Password = configDto.PasswordRx;
            ConfigRx.BusinessUnitIdRx = configDto.BusinessUnitIdRx;

            Config1CUpp.Url = configDto.Url1CUpp;
            Config1CUpp.Login = configDto.Login1CUpp;
            Config1CUpp.Password = configDto.Password1CUpp;

            Config1CDo.Url = configDto.Url1CDo;
            Config1CDo.Login = configDto.Login1CDo;
            Config1CDo.Password = configDto.Password1CDo;
            Config1CDo.PathToStorage = configDto.PathToStorage1CDo;

            Config1CDo.CeoPersonnelOrderId1CDo = configDto.CeoPersonnelOrderDocKindId1CDo;
            Config1CDo.DeputyCeoPersonnelOrderId1CDo = configDto.DeputyCeoPersonnelOrderDocKindId1CDo;
            Config1CDo.OrderId1CDo = configDto.OrderDocKindId1CDo;
            Config1CDo.ContractId1CDo = configDto.ContractDocKindId1CDo;
        }

        public static bool IsConnectedToRx() =>  RepositoryRx.IsConnected();

        public static bool IsConnectedTo1CDo() => Repository1CDo.IsConnected();

        public static bool IsConnectedTo1CUpp() => Repository1CUpp.IsConnected();
    }
}
