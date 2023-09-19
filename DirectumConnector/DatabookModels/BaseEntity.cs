namespace DirectumConnector.DatabookModels
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public bool ShouldSerializeId() => Id != 0;
    }
}
