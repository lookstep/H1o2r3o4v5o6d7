using System.Collections.Generic;

namespace DirectumConnector.DatabookModels
{
    public class DocumentKind1CUppSettingRx
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Sbis { get; set; }

        public List<DocumentKind1CUppSettingMemberRx> DocumentKinds { get; set; }
    }
}
