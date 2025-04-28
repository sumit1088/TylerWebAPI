using System.Xml.Serialization;

namespace efilling_api.Models
{
    public class PaymentAccount
    {
        public string? PaymentAccountID { get; set; }
        //public string? PaymentAccountTypeCode { get; set; }
        public string? AccountName { get; set; }
        //public string? AccountToken { get; set; }
    }
    public class CreatePaymentAccount
    {   
        public string? AccountName { get; set; }
        public string? PaymentAccountTypeCodeId { get; set; }
        public string? AccountToken { get; set; }
        public string? CardType { get; set; }
        public string? CardLast4 { get; set; }
        public int? CardMonth { get; set; }
        public int? CardYear { get; set; }
        public string? CardHolderName { get; set; }
        public string? cvv { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zip { get; set; }
        public string? FirmID { get; set; }
    }
        
    [XmlRoot("PaymentRequest")]
    public class PaymentRequest
    {
        public string ClientKey { get; set; }
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public string RedirectURL { get; set; }
        public int GetToken { get; set; }
    }
}
