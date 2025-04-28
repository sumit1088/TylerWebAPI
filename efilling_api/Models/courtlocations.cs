using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class courtlocations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; } 
        public string? name { get; set; } 
        public string? initial { get; set; } 
        public string? subsequent { get; set; } 
        public string? disallowcopyingenvelopemultipletimes { get; set; } 
        public string? allowfilingintononindexedcase { get; set; } 
        public string? allowablecardtypes { get; set; } 
        public string? odysseynodeid { get; set; } 
        public string? cmsid { get; set; } 
        public string? sendservicebeforereview { get; set; } 
        public string? parentnodeid { get; set; } 
        public string? iscounty { get; set; } 
        public string? restrictbankaccountpayment { get; set; } 
        public string? allowmultipleattorneys { get; set; } 
        public string? sendservicecontactremovednotifications { get; set; } 
        public string? allowmaxfeeamount { get; set; } 
        public string? transferwaivedfeestocms { get; set; } 
        public string? skippreauth { get; set; } 
        public string? allowhearing { get; set; } 
        public string? allowreturndate { get; set; } 
        public string? showdamageamount { get; set; } 
        public string? hasconditionalservicetypes { get; set; } 
        public string? hasprotectedcasetypes { get; set; } 
        public string? protectedcasetypes { get; set; } 
        public string? allowzerofeeswithoutfilingparty { get; set; } 
        public string? allowserviceoninitial { get; set; } 
        public string? allowaddservicecontactsoninitial { get; set; } 
        public string? allowredaction { get; set; } 
        public string? redactionurl { get; set; } 
        public string? redactionviewerurl { get; set; } 
        public string? redactionapiversion { get; set; } 
        public string? enforceredaction { get; set; } 
        public string? redactiondocumenttype { get; set; } 
        public string? defaultdocumentdescription { get; set; } 
        public string? allowwaiveronmail { get; set; } 
        public string? showreturnonreject { get; set; } 
        public string? protectedcasereplacementstring { get; set; } 
        public string? allowchargeupdate { get; set; } 
        public string? allowpartyid { get; set; } 
        public string? redactionfee { get; set; } 
        public string? allowwaiveronredaction { get; set; } 
        public string? disallowelectronicserviceonnewcontacts { get; set; } 
        public string? allowindividualregistration { get; set; } 
        public string? redactiontargetconfiguration { get; set; } 
        public string? bulkfilingfeeassessorconfiguration { get; set; } 
        public string? envelopelevelcommentconfiguration { get; set; } 
        public string? autoassignsrlservicecontact { get; set; } 
        public string? autoassignattorneyservicecontact { get; set; } 
        public string? partialwaiverdurationindays { get; set; } 
        public string? partialwaivercourtpaymentsystemurl { get; set; } 
        public string? partialwaiveravailablewaiverpercentages { get; set; } 
        public string? allowrepcap { get; set; } 
        public string? eserviceconsentenabled { get; set; } 
        public string? eserviceconsenttextblurbmain { get; set; } 
        public string? eserviceconsenttextblurbsecondary { get; set; } 
        public string? eserviceconsenttextblurbsecondaryafterconsentyes { get; set; } 
        public string? eserviceconsenttextconsentyes { get; set; } 
        public string? eserviceconsenttextconsentyeshelp { get; set; } 
        public string? eserviceconsenttextconsentyeswithadd { get; set; } 
        public string? eserviceconsenttextconsentyeswithaddhelp { get; set; } 
        public string? eserviceconsenttextconsentno { get; set; } 
        public string? eserviceconsenttextconsentnohelp { get; set; } 
        public string? promptforconfidentialdocumentsenabled { get; set; } 
        public string? promptforconfidentialdocuments { get; set; } 
        public string? defaultdocumentsecurityenabled { get; set; } 
        public string? defaultdocumentsecurity { get; set; } 
        public string? cmsservicecontactsupdatesenabled { get; set; } 
        public string? cmsservicecontactsupdatesfirmid { get; set; } 
        public string? cmsservicecontactsupdatesbehavior { get; set; }
        public string? subsequentactionsenabled { get; set; }



    }
}
