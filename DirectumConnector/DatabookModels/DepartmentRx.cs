namespace DirectumConnector.DatabookModels
{
    public class DepartmentRx
    {
        public int Id { get; set; }

        public string Sid { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string ShortName { get; set; }

        public string Code { get; set; }

        public string Id1CUpp { get; set; }

        public DepartmentRx HeadOffice { get; set; }

        public BusinessUnitRx BusinessUnit { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;


        public bool ShouldSerializeStatus() => Status != null;

        public bool ShouldSerializeSid() => Sid != null;
    }
}

