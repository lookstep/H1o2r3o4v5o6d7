namespace DirectumConnector.DatabookModels
{
    public class EmployeeRx
    {
        public int Id { get; set; }

        public string Sid { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Id1CUpp { get; set; }

        public DepartmentRx Department { get; set; }

        public JobTitleRx JobTitle { get; set; }

        public PersonRx Person { get; set; }

        public bool NeedNotifyExpiredAssignments { get; set; }

        public bool NeedNotifyNewAssignments { get; set; } 

        public string PersonnelNumber { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeStatus() => Status != null;

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;

        public bool ShouldSerializeSid() => Sid != null;
    }
}

