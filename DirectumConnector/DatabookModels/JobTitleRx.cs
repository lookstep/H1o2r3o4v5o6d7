namespace DirectumConnector.DatabookModels
{
    public class JobTitleRx
    {
        public int Id { get; set; }

        public string Id1CUpp { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool ShouldSerializeId() => Id != 0;

        public bool ShouldSerializeStatus() => Status != null;

        public bool ShouldSerializeId1CUpp() => Id1CUpp != null;
    }
}
