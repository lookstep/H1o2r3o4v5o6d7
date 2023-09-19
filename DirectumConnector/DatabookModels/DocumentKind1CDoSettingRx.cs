namespace DirectumConnector.DatabookModels
{
    public class DocumentKind1CDoSettingRx
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DocumentKindRx DocumentKindRx { get; set; }

        public string DocumentKindId1CDo { get; set; }

        public EmployeeRx ResponsibleEmployee { get; set; }

        public ApprovalRuleRx ApprovalRule { get; set; }
    }
}
