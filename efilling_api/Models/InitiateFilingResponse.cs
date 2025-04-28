namespace efilling_api.Models
{
    public class InitiateFilingResponse
    {

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class IdentificationID
        {
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
            public string Value { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
            public string Value { get; set; }
        }

        public class DocumentIdentification
        {
            public IdentificationID IdentificationID { get; set; }
            public Item Item { get; set; }
            public string IdentificationJurisdiction { get; set; }
            public string IdentificationSourceText { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        public class CourtName
        {
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
            public string Value { get; set; }
        }

        public class OrganizationIdentification
        {
            public IdentificationID IdentificationID { get; set; }
            public string Item { get; set; }
            public string IdentificationJurisdiction { get; set; }
            public string IdentificationSourceText { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        public class CaseCourt
        {
            public CourtName CourtName { get; set; }
            public OrganizationIdentification OrganizationIdentification { get; set; }
            public string OrganizationLocation { get; set; }
            public string OrganizationName { get; set; }
            public string OrganizationPrimaryContactInformation { get; set; }
            public string OrganizationSubUnitName { get; set; }
            public string OrganizationTaxIdentification { get; set; }
            public string OrganizationUnitName { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        public class ErrorCode
        {
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
            public string Value { get; set; }
        }

        public class ErrorText
        {
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
            public string Value { get; set; }
        }

        public class Error
        {
            public ErrorCode ErrorCode { get; set; }
            public ErrorText ErrorText { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        public class SendingMDELocationID
        {
            public IdentificationID IdentificationID { get; set; }
            public string Item { get; set; }
            public string IdentificationJurisdiction { get; set; }
            public string IdentificationSourceText { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        public class SendingMDEProfileCode
        {
            public string Value { get; set; }
        }

        public class DocumentReceivedDate
        {
            public Item Item { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }

        // Root Object Class
        public class RootObject
        {
            public CaseCourt CaseCourt { get; set; }
            public List<Error> Error { get; set; }
            public SendingMDELocationID SendingMDELocationID { get; set; }
            public SendingMDEProfileCode SendingMDEProfileCode { get; set; }
            public string DocumentApplicationName { get; set; }
            public string DocumentBinary { get; set; }
            public string DocumentCategoryText { get; set; }
            public string DocumentDescriptionText { get; set; }
            public string DocumentEffectiveDate { get; set; }
            public string DocumentFileControlID { get; set; }
            public string DocumentFiledDate { get; set; }
            public List<DocumentIdentification> DocumentIdentification { get; set; }
            public string DocumentInformationCutOffDate { get; set; }
            public string DocumentPostDate { get; set; }
            public DocumentReceivedDate DocumentReceivedDate { get; set; }
            public string DocumentSequenceID { get; set; }
            public string DocumentStatus { get; set; }
            public string DocumentTitleText { get; set; }
            public string Item { get; set; }
            public string DocumentSubmitter { get; set; }
            public string id { get; set; }
            public string metadata { get; set; }
            public string linkMetadata { get; set; }
        }


    }
}
