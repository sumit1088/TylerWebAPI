using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tyler_web_app.Models
{    
    public class Case
    {
        public CaseAugmentation CaseAugmentation { get; set; }
        public CaseAugmentation1 CaseAugmentation1 { get; set; }
        public List<CauseOfActionCode> CauseOfActionCode { get; set; }
        public object AmountInControversy { get; set; }
        public ClassActionIndicator ClassActionIndicator { get; set; }
        public object JurisdictionalGroundsCode { get; set; }
        public JuryDemandIndicator JuryDemandIndicator { get; set; }
        public List<ReliefTypeCode> ReliefTypeCode { get; set; }
        public object Item1 { get; set; }
        public CaseTitleText CaseTitleText { get; set; }
        public CaseCategoryText CaseCategoryText { get; set; }
        public CaseTrackingID CaseTrackingID { get; set; }
        public CaseDocketID CaseDocketID { get; set; }
        public object ActivityIdentification { get; set; }
        public object Item { get; set; }
        public object ActivityDescriptionText { get; set; }
        public object ActivityStatus { get; set; }
        public object Items { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class CaseAugmentation
    {
        public object CaseCharge { get; set; }
        public CaseCourt1 CaseCourt { get; set; }
        public object CaseCourtEvent { get; set; }
        public object CaseDefendantParty { get; set; }
        public object CaseDefenseAttorney { get; set; }
        public object CaseInitiatingParty { get; set; }
        public object CaseJudge { get; set; }
        public object CaseLineageCase { get; set; }
        public object CaseOfficial { get; set; }
        public object CaseOtherEntity { get; set; }
        public object CaseProsecutionAttorney { get; set; }
        public object CaseRespondentAttorney { get; set; }
        public object CaseRespondentParty { get; set; }
        public object CaseInitiatingAttorney { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
    }

    public class CaseAugmentation1
    {
        public CaseTypeText CaseTypeText { get; set; }
        public object FilerTypeText { get; set; }
        public object LowerCourtText { get; set; }
        public object LowerCourtJudgeText { get; set; }
        public object AttachServiceContactIndicator { get; set; }
        public object ProcedureRemedy { get; set; }
        public object ProviderCharge { get; set; }
        public object PropertyAccountNumber { get; set; }
        public object CivilClaimAmount { get; set; }
        public object ProbateEstateAmount { get; set; }
        public object FilingAssociation { get; set; }
        public object PartyService { get; set; }
        public object Items { get; set; }
        public object MaxFeeAmount { get; set; }
        public int CaseSecurity { get; set; }
        public bool CaseSecuritySpecified { get; set; }
        public object CaseSubTypeText { get; set; }
        public object PhysicalFeature { get; set; }
        public object ReturnDate { get; set; }
        public object HearingSchedule { get; set; }
        public object OutOfStateIndicator { get; set; }
        public object CitationAugmentation { get; set; }
        public object WillFiledDate { get; set; }
        public object DecedentPartyAssociation { get; set; }
        public object QuestionAnswer { get; set; }
        public object Agency { get; set; }
        public object LowerCourtCaseTypeText { get; set; }
        public object CaseAddress { get; set; }
        public object FeeSplit { get; set; }
        public object Item { get; set; }
        public object Alias { get; set; }
        public object CaseOfficial { get; set; }
        public object CaseOtherEntityAttorney { get; set; }
        public object CaseParticipant { get; set; }
        public object CaseShortTitleText { get; set; }
        public object OrganizationAssociation { get; set; }
        public object PersonAssociation { get; set; }
        public object PersonOrganizationAssociation { get; set; }
        public object RelatedCaseAssociation { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class CaseCategoryText
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class CaseCourt1
    {
        public CourtName CourtName { get; set; }
        public OrganizationIdentification OrganizationIdentification { get; set; }
        public object OrganizationLocation { get; set; }
        public object OrganizationName { get; set; }
        public object OrganizationPrimaryContactInformation { get; set; }
        public object OrganizationSubUnitName { get; set; }
        public object OrganizationTaxIdentification { get; set; }
        public object OrganizationUnitName { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class CaseDocketID
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class CaseTitleText
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class CaseTrackingID
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class CaseTypeText
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class CauseOfActionCode
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public object Value { get; set; }
    }

    public class ClassActionIndicator
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public bool Value { get; set; }
    }

    public class CourtName
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public object Value { get; set; }
    }

    public class Error
    {
        public ErrorCode ErrorCode { get; set; }
        public ErrorText ErrorText { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class ErrorCode
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    public class ErrorText
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public string Value { get; set; }
    }

    //public class IdentificationID
    //{
    //    public object id { get; set; }
    //    public object metadata { get; set; }
    //    public object linkMetadata { get; set; }
    //    public string Value { get; set; }
    //}

    public class JuryDemandIndicator
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public bool Value { get; set; }
    }

    public class OrganizationIdentification
    {
        //public IdentificationID IdentificationID { get; set; }
        public object Item { get; set; }
        public object IdentificationJurisdiction { get; set; }
        public object IdentificationSourceText { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class ReliefTypeCode
    {
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
        public object Value { get; set; }
    }

    public class Root
    {
        public List<Case> Case { get; set; }
        public SendingMDELocationID SendingMDELocationID { get; set; }
        public string SendingMDEProfileCode { get; set; }
        public CaseCourt1 CaseCourt { get; set; }
        public List<Error> Error { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }

    public class SendingMDELocationID
    {
        //public IdentificationID IdentificationID { get; set; }
        public object Item { get; set; }
        public object IdentificationJurisdiction { get; set; }
        public object IdentificationSourceText { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object linkMetadata { get; set; }
    }


}

