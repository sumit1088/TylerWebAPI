namespace efilling_api.Models
{
    public class CourtCodelist
    {
        public ECFElementName ECFElementName { get; set; }
        public EffectiveDate EffectiveDate { get; set; }
        public object ExpirationDate { get; set; }
        public CourtCodelistURI CourtCodelistURI { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class CourtCodelistURI
    {
        public IdentificationID IdentificationID { get; set; }
        public object Item { get; set; }
        public object IdentificationJurisdiction { get; set; }
        public object IdentificationSourceText { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class ECFElementName
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class EffectiveDate
    {
        public Item Item { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class IdentificationID
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class Item
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public DateTime Value { get; set; }
    }

    public class PolicyLastUpdateDate
    {
        public Item Item { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class PolicyVersionID
    {
        public IdentificationID IdentificationID { get; set; }
        public object Item { get; set; }
        public object IdentificationJurisdiction { get; set; }
        public object IdentificationSourceText { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class Root
    {
        public PolicyVersionID PolicyVersionID { get; set; }
        public PolicyLastUpdateDate PolicyLastUpdateDate { get; set; }
        public RuntimePolicyParameters RuntimePolicyParameters { get; set; }
    }

    public class RuntimePolicyParameters
    {
        public object PublicKeyInformation { get; set; }
        public List<CourtCodelist> CourtCodelist { get; set; }
    }

}
