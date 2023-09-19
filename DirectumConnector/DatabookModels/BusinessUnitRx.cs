namespace DirectumConnector.DatabookModels
{
    public class BusinessUnitRx
    {
        public int Id { get; set; }

        public string Sid { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string LegalName { get; set; }

        public string Id1CUpp { get; set; }

        public string Id1CDo { get; set; }

        public string Code { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeSid() => Sid != null;

        public bool ShouldSerializeStatus() => Status != null;

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;

        public bool ShouldSerializeId1CDo() => Id1CDo != null;
    }
}


