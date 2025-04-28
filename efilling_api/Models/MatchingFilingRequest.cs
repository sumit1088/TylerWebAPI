namespace efilling_api.Models
{
    public class MatchingFilingRequest
    {
        public Data data { get; set; }

        public bool success { get; set; }
    }

    public class Data
    {
        public List<MatchingFilingDTO> MatchingFiling { get; set; }
    }

    public class MatchingFilingDTO
    {
        public Field CaseTitle { get; set; }
        public Field CaseNumber { get; set; }
        public Field CaseJudge { get; set; }
        public Field FilingType { get; set; }
        public Field FilingAttorney { get; set; }
        public Field FilingCode { get; set; }
        public Field OrganizationIdentificationID { get; set; }
        public Field CaseCategoryCode { get; set; }
        public Field CaseTypeCode { get; set; }
        public Field DocumentDescriptionText { get; set; }
        public Field DocumentFileControlID { get; set; }
        public DateField DocumentFiledDate { get; set; }
        public List<DocumentIdentification> DocumentIdentification { get; set; }
        public DateField DocumentReceivedDate { get; set; }
        public DocumentSubmitter DocumentSubmitter { get; set; }
        public Field CaseTrackingID { get; set; }
        public FilingStatus FilingStatus { get; set; }
    }

    public class Field
    {
        public string Value { get; set; }
    }

    public class DateField
    {
        public Field Item { get; set; }
    }

    public class DocumentIdentification
    {
        public Field IdentificationID { get; set; }
        public Field Item { get; set; }
    }

    public class DocumentSubmitter
    {
        public PersonItem Item { get; set; }
    }

    public class PersonItem
    {
        public PersonName PersonName { get; set; }
        public List<DocumentIdentification> PersonOtherIdentification { get; set; }
    }

    public class PersonName
    {
        public Field PersonFullName { get; set; }
    }

    public class FilingStatus
    {
        public string FilingStatusCode { get; set; }
        public List<Field> StatusDescriptionText { get; set; }
    }
}
