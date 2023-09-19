using Connector1CUpp;
using Connector1CUpp.DatabookModels;
using DirectumConnector;
using DirectumConnector.DatabookModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synchronizer.DatabookSynchronizers
{
    public static class EmployeeSynchronizer
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Sync()
        {
            var employeesRx = RepositoryRx.GetListSelectIdAndId1CUpp<EmployeeRx>();
            var personsRx = RepositoryRx.GetListSelectIdAndId1CUpp<PersonRx>();
            var departmentsRx = RepositoryRx.GetListSelectIdAndId1CUpp<DepartmentRx>();
            var jobTitlesRx = RepositoryRx.GetListSelectIdAndId1CUpp<JobTitleRx>();

            var count = Repository1CUpp.GetEmployee1CUppCount();
            var iterationCount = (int)Math.Ceiling(count / (double)DatabookSyncService.Top);

            for (var i = 0; i < iterationCount; i++)
            {
                var skip = i * DatabookSyncService.Top;
                var employees1CUpp = Repository1CUpp.GetEmployees1CUpps(skip, DatabookSyncService.Top);
                CreateOrUpdateEmployees(employees1CUpp, employeesRx, personsRx, departmentsRx, jobTitlesRx);
            }
        }

        private static void CreateOrUpdateEmployees(List<Employee1CUpp> employees1C,
                                                    List<EmployeeRx> employeesRx,
                                                    List<PersonRx> personsRx,
                                                    List<DepartmentRx> departmentsRx,
                                                    List<JobTitleRx> jobTitlesRx)
        {
            foreach(var employee1C in employees1C)
            {
                try
                {
                    CreateOrUpdateEmployee(employeesRx, personsRx, departmentsRx, jobTitlesRx, employee1C);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Common.TrySendNotificationToAdmins(ex);
                }
            }
        }

        private static void CreateOrUpdateEmployee(List<EmployeeRx> employeesRx, 
                                                   List<PersonRx> personsRx, 
                                                   List<DepartmentRx> departmentsRx, 
                                                   List<JobTitleRx> jobTitlesRx, 
                                                   Employee1CUpp employee1C)
        {
            var employeeRxId = employeesRx.FirstOrDefault(x => x.Id1CUpp == employee1C.Id)?.Id;

            var employee = new EmployeeRx
            {
                Name = employee1C.Name,
                Person = GetPersonRx(personsRx, employee1C),
                Department = GetDepartmentRx(departmentsRx, employee1C),
                JobTitle = GetJobTitleRx(jobTitlesRx, employee1C),
                PersonnelNumber = employee1C.PersonnelNumber
            };

            if (employeeRxId != null)
            {
                RepositoryRx.Update(employee, employeeRxId.Value);
                return;
            }

            employee.Sid = Guid.NewGuid().ToString();
            employee.Status = "Active";
            employee.Id1CUpp = employee1C.Id;
            employee.NeedNotifyExpiredAssignments = false;
            employee.NeedNotifyNewAssignments = false;

            RepositoryRx.Create(employee);
        }

        public static DepartmentRx GetDepartmentRx(List<DepartmentRx> departmentsRx, Employee1CUpp employee1CUpp)
        {
            var departmentId1CUpp = employee1CUpp.DepartmentId;

            if (departmentId1CUpp == Guid.Empty.ToString())
                return null;

            var departmentRx = departmentsRx.FirstOrDefault(x => x.Id1CUpp == departmentId1CUpp);
            if (departmentRx == null)
                throw new ApplicationException($"Сотрудник {employee1CUpp.Name} c Id1CUpp = {employee1CUpp.Id} не синхронизирован, " +
                    $"т.к. подразделение c Id1CUpp = {departmentId1CUpp} не было синхронизировано ранее");

            return departmentRx;
        }

        public static PersonRx GetPersonRx(List<PersonRx> personsRx, Employee1CUpp employee1CUpp)
        {
            var personId1CUpp = employee1CUpp.PersonId;

            if (personId1CUpp == new Guid().ToString())
                return null;

            var personRx = personsRx.FirstOrDefault(x => x.Id1CUpp == personId1CUpp);
            if (personRx == null)
                throw new ApplicationException($"Сотрудник {employee1CUpp.Name} c Id1CUpp = {employee1CUpp.Id} не синхронизирован, " +
                    $"т.к. персона c  Id1CUpp = {personId1CUpp} не была синхронизирована ранее");

            return personRx;
        }

        public static JobTitleRx GetJobTitleRx(List<JobTitleRx> jobTitlesRx, Employee1CUpp employee1CUpp)
        {
            var jobTitleId1CUpp = employee1CUpp.JobTitleId;
            if (jobTitleId1CUpp == Guid.Empty.ToString())
                return null;

            var jobTitleRx = jobTitlesRx.FirstOrDefault(x => x.Id1CUpp == jobTitleId1CUpp);
            if (jobTitleRx == null)
                throw new ApplicationException($"Сотрудник {employee1CUpp.Name} c Id1CUpp = {employee1CUpp.Id} не синхронизирован, " +
                    $"т.к. должность Id1CUpp: {jobTitleId1CUpp} не была синхронизирована ранее");

            return jobTitleRx;
        }
    }
}
