using Connector1CDO;
using DirectumConnector;
using DirectumConnector.DatabookModels;
using DirectumConnector.DocModels;
using StandardODATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synchronizer.DocSynchronizers
{
    public class Order1CDoSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void CreateCeoOrders(DateTime startSyncDate, int top)
        {
            var ordersRx = RepositoryRx.GetListSelectIdAndId1CDo<OrderRx>().Cast<OrderBaseRx>().ToList();

            var count = Repository1CDo.GetInternalDocumentsCount(startSyncDate, Config1CDo.OrderId1CDo);
            var iterationCount = (int)Math.Ceiling(count / (double)top);

            for (var i = 0; i < iterationCount; i++)
            {
                int skip = top * i;
                var orders1CDo = Repository1CDo.GetInternalDocuments(startSyncDate, Config1CDo.OrderId1CDo, skip, top);
                
                var files1CDo = Common.GetFiles1CDo(orders1CDo);
                CreateOrders(orders1CDo, ordersRx, files1CDo);
            }
        }

        public static void CreateCeoPersonnelOrders(DateTime startSyncDate, int top)
        {
            var personnelOrdersRx = RepositoryRx.GetList<CeoPersonnelOrderRx>().Cast<OrderBaseRx>().ToList();

            var count = Repository1CDo.GetInternalDocumentsCount(startSyncDate, Config1CDo.CeoPersonnelOrderId1CDo);
            var iterationCount = (int)Math.Ceiling(count / (double)top);

            for (var i = 0; i < iterationCount; i++)
            {
                int skip = top * i;
                var ceoPersonnelOrders1CDo = Repository1CDo.GetInternalDocuments(startSyncDate, Config1CDo.CeoPersonnelOrderId1CDo, skip, top).ToList();

                var files1CDo = Common.GetFiles1CDo(ceoPersonnelOrders1CDo);
                CreateOrders(ceoPersonnelOrders1CDo, personnelOrdersRx, files1CDo);
            }
        }

        public static void CreateDeputyCeoPersonnelOrders(DateTime startSyncDate, int top)
        {
            var personnelOrdersRx = RepositoryRx.GetList<DeputyCeoPersonnelOrderRx>().Cast<OrderBaseRx>().ToList();

            var count = Repository1CDo.GetInternalDocumentsCount(startSyncDate, Config1CDo.DeputyCeoPersonnelOrderId1CDo);
            var iterationCount = (int)Math.Ceiling(count / (double)top);

            for (var i = 0; i < iterationCount; i++)
            {
                int skip = top * i;
                var deputyCeoPersonnelOrders1CDo = Repository1CDo.GetInternalDocuments(startSyncDate, Config1CDo.DeputyCeoPersonnelOrderId1CDo, skip, top);

                var files1CDo = Common.GetFiles1CDo(deputyCeoPersonnelOrders1CDo);
                CreateOrders(deputyCeoPersonnelOrders1CDo, personnelOrdersRx, files1CDo);
            }
        }

        private static void CreateOrders(IEnumerable<Catalog_ВнутренниеДокументы> orders1CDo, 
                                         IEnumerable<OrderBaseRx> ordersRx,
                                         List<Catalog_Файлы> files1CDo)
        {
            Parallel.ForEach(orders1CDo, order1CDo =>
            {
                try
                {
                    CreateOrder(ordersRx, order1CDo, files1CDo);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Synchronizer.Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrder(IEnumerable<OrderBaseRx> ordersRx, Catalog_ВнутренниеДокументы order1CDo, List<Catalog_Файлы> files1CDo)
        {
            var Id1CDo = order1CDo.Ref_Key.ToString();

            if (ordersRx.Any(x => x.Id1CDo == Id1CDo))
                return;

            Validate(order1CDo);

            var baseOrderRx = new OrderBaseRx
            {
                TitleName = order1CDo.Description,
                DocumentNumber = order1CDo.РегистрационныйНомер,
                DocumentDateNew = order1CDo.ДатаРегистрации,
                DocumentKind = Common.GetDocumentKindRx(order1CDo),
                BusinessUnit = DocSyncService.BusinessUnitRx,
                PreparedBy = GetPreparedBy(order1CDo),
                Assignee1C = GetAssignee1C(order1CDo),
                Department = GetDepartment(order1CDo),
                Subject = order1CDo.Заголовок,
                Id1CDo = Id1CDo
            };

            var createdOrderRx = Common.Create(baseOrderRx);

            try
            {
                Common.CreateVersion(createdOrderRx.Id, Id1CDo, files1CDo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Synchronizer.Common.TrySendNotificationToAdmins(ex);
            }
            finally
            {
                Synchronizer.Common.TrySendApprovalTaskForDoc1CDo(createdOrderRx.Id);
            }
        }

        private static DepartmentRx GetDepartment(Catalog_ВнутренниеДокументы order1CDo)
        {
            return DocSyncService.DepartmentsRx.FirstOrDefault(x => x.Code == order1CDo.Подразделение.Код1СУПП);
        }

        private static EmployeeRx GetAssignee1C(Catalog_ВнутренниеДокументы order1CDo)
        {
            var assignee1C = DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.PersonnelNumber == order1CDo.Ответственный.ТабНомер);
            return assignee1C ?? throw new ApplicationException($"Не удалось создать карточку документа {order1CDo.Description} с id1CDo = {order1CDo.Ref_Key}. " +
                $"В Rx нет сотрудника с табельным номером {order1CDo.Ответственный.ТабНомер}");
        }

        private static EmployeeRx GetPreparedBy(Catalog_ВнутренниеДокументы order1CDo)
        {
            var preparedBy = DocSyncService.EmployeesRxSelectIdDepartmentPersonnelNumber.FirstOrDefault(x => x.PersonnelNumber == order1CDo.Подготовил.ТабНомер);
            return preparedBy ?? throw new ApplicationException($"Не удалось создать карточку документа {order1CDo.Description} с id1CDo = {order1CDo.Ref_Key}. " +
                $"В Rx нет сотрудника с табельным номером {order1CDo.Подготовил.ТабНомер}");
        }

        private static void Validate(Catalog_ВнутренниеДокументы order1CDo)
        {
            ValidateProperty(order1CDo.Подготовил, "Подготовил", order1CDo);
            ValidateProperty(order1CDo.Ответственный, "Ответственный", order1CDo);
            ValidateProperty(order1CDo.Подготовил.ТабНомер, "Подготовил.ТабНомер", order1CDo);
            ValidateProperty(order1CDo.Ответственный.ТабНомер, "Ответственный.ТабНомер", order1CDo);
            if (order1CDo.ДатаРегистрации == DateTime.MinValue)
                throw new ApplicationException($"Не удалось создать карточку документа {order1CDo.Заголовок} с id1CDo = {order1CDo.Ref_Key}. " +
                    $"Не заполнено поле ДатаРегистрации");
        }

        private static void ValidateProperty<T>(T property, string localizedName, Catalog_ВнутренниеДокументы order1CDo)
        {
            if (property == null)
                throw new ApplicationException($"Не удалось создать карточку документа {order1CDo.Description} с id1CDo = {order1CDo.Ref_Key}. Не заполнено поле {localizedName}");
        }
    }
}
