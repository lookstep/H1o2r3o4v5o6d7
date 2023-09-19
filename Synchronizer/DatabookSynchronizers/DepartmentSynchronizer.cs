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
    public class DepartmentSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            var departmentsRx = RepositoryRx.GetListSelectIdAndId1CUpp<DepartmentRx>();

            var count = Repository1CUpp.GetDepartment1CUppCount();
            var iterationCount = (int)Math.Ceiling(count / (double)DatabookSyncService.Top);

            for (var i = 0; i < iterationCount; i++)
            {
                var skip = i * DatabookSyncService.Top;
                var departments1CUpp = Repository1CUpp.GetDepartments1CUpp(skip, DatabookSyncService.Top);
                CreateOrUpdateDepartments(departments1CUpp, departmentsRx);
            }
            var departments1C = Repository1CUpp.GetDepartments1CUppIdAndParentDepartment();
            FillHeadDepartment(departments1C);
        }

        private static void CreateOrUpdateDepartments(List<Department1CUpp> departments1C, List<DepartmentRx> departmentsRx)
        {
            Parallel.ForEach(departments1C, department1C =>
            {
                try
                {
                    CreateOrUpdateDepartment(departmentsRx, department1C);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Common.TrySendNotificationToAdmins(ex);
                }
            });
        }

        private static void CreateOrUpdateDepartment(List<DepartmentRx> departmentsRx, Department1CUpp department1C)
        {
            var departmentRxId = departmentsRx.FirstOrDefault(d => d.Id1CUpp == department1C.Id)?.Id;

            var department = new DepartmentRx
            {
                Name = department1C.Name,
                ShortName = department1C.ShortName,
                Code = department1C.Code,
                BusinessUnit = DatabookSyncService.BusinessUnitRx
            };

            if (departmentRxId != null)
            {
                RepositoryRx.Update(department, departmentRxId.Value);
                return;
            }

            department.Id1CUpp = department1C.Id;
            department.Sid = Guid.NewGuid().ToString();
            department.Status = "Active";
            RepositoryRx.Create(department);
        }

        private static void FillHeadDepartment(List<Department1CUpp> departments1C)
        {
            var actualDepartmentsRx = RepositoryRx.GetListSelectIdAndId1CUpp<DepartmentRx>();

            foreach (var department1C in departments1C)
            {
                var departmentRxId = actualDepartmentsRx.FirstOrDefault(d => d.Id1CUpp == department1C.Id)?.Id;
                if (departmentRxId == null)
                    continue;
                var headDepartment = actualDepartmentsRx.FirstOrDefault(d => d.Id1CUpp == department1C.ParentDepartmentId);
                if (department1C.ParentDepartmentId != new Guid().ToString() && headDepartment == null)
                    throw new ApplicationException($"Департамент {department1C.Name} c Id1CUpp = {department1C.ParentDepartmentId} не синхронизирован");

                // Создаем новый объект с одним полем HeadOffice, чтобы при обновлении не затерлись отстальные поля (т.к. условие сериализации поставить невозможно)
                var department = new DepartmentOnlyHeadOfficeRx();
                if (headDepartment != null)
                    department.HeadOffice = new HeadOfficeRx { Id = headDepartment.Id };

                RepositoryRx.Update(department, departmentRxId.Value);
            }
        }
    }
}
