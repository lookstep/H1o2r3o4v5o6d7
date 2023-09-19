using DirectumConnector.DatabookModels;

namespace DirectumConnector.DocModels
{
    public class DocumentBase
    {
        public int Id { get; set; }

        public DocumentKindRx DocumentKind { get; set; }

        public bool ShouldSerializeId() => Id != 0;
    }
}
