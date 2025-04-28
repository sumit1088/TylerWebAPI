using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.PeerToPeer;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using tyler_web_app.CourtPolicyMDE;
using tyler_web_app.CourtPolicyServiceV5;
using tyler_web_app.CourtRecordServiceV5;
using tyler_web_app.FilingAssemblyMDEService;
using tyler_web_app.FilingReviewMDEV5;
using tyler_web_app.Models;
using tyler_web_app.TylerCourtRecordMDEV5;
using tyler_web_app.TylerServiceMDEService;

namespace tyler_web_app.Controllers
{
    public class CustomHeader : MessageHeader
    {
        private const string HeaderName = "UserNameHeader";
        private const string HeaderNamespace = "";
        public override string Name => HeaderName;
        public override string Namespace => HeaderNamespace;
        public string headerString;

        public CustomHeader()
        {
            this.headerString = @"<innerTag>content</innerTag>";
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteXmlnsAttribute("p", "urn:tyler:efm:services");
            var r = XmlReader.Create(new StringReader(headerString));
            r.MoveToContent();
            writer.WriteNode(r, false);
        }
    }


    public class SecurityHeader : MessageHeader
    {
        private const string HeaderName = "Security";
        private const string HeaderNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public override string Name => HeaderName;
        public override string Namespace => HeaderNamespace;
        public string headerString;

        public SecurityHeader(string username, string password)
        {
            this.headerString = @"<UsernameToken xmlns=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""> 
        <Username>" + username + @"</Username> 
        <Password>" + password + @"</Password> </UsernameToken>";
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var r = XmlReader.Create(new StringReader(headerString));
            r.MoveToContent();
            writer.WriteNode(r, false);
        }
    }

    public class HomeController : Controller
    {
        public static string AltrusCAPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\AltrueCA\AltrueCA.pfx");
        public static string AltrusCAPassword = "MyP@ssword1";
        public static string AltrusBASEURL = "https://california-efm-stage.tylertech.cloud/"; //"https://california-stage.tylerhost.net";
        public X509Certificate2 x509Certificate2 = EFMClient.LoadCertificateFromFile(AltrusCAPath, AltrusCAPassword);
        static HttpClient client = new HttpClient();

        public object ServiceRecipientID { get; private set; }

        public ActionResult testPayment()
        {
            //client.BaseAddress = new Uri("https://togatest.tylerhost.net/EPayments/Webs/EPayment.aspx");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("x-www-form-urlencoded"));
            //HttpResponseMessage response = await client.PostAsync();
            //;
            //if (response.IsSuccessStatusCode)
            //{
            //    return response.Content.ReadAsStringAsync().Result;
            //}
            //else
            //{
            //    return "error";
            //    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            //}

            var client = new RestClient("https://togatest.tylerhost.net/EPayments/Webs/EPayment.aspx");
            var request = new RestRequest(new Uri("https://togatest.tylerhost.net/EPayments/Webs/EPayment.aspx"), Method.Post);
            //   request.AddHeader("postman-token", "2aca121e-61c1-e7cd-42cd-f4ded333a901");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "x-www-form-urlencoded");
            request.AddParameter("<PaymentRequest><ClientKey>CJOGF-CAStage</ClientKey><TransactionID>suraj_2022-06-29T11:12:01.6492900Z</TransactionID><Amount>-1</Amount><RedirectURL>https://california-stage.tylertech.cloud/OfsEfsp/Payment?ValidationToken=YmVaVUVVeWJkSWF0REswNVJIb2xDakJYeXBhY0pTVG5ZTlFIUjlXK3pMK1dOd05US3BKVUc1RXdxRXV2OFBrem10ekdabEFSVzJ5VUw2ZkZycE1FcEFIK0FHeGROZkd0dWxJelREei9VbjNtV015eHg0UnpYZXNMekZqQ2QrYmJEWFJyWkM1M3FTdHRiYXh0YUx4SXJ3NTFad253WXJia1MxRWlWSmVTTlNheE9xUVhzdTZyVEhqSWFGeFNZaUVFa201R2JzM21NV2g0Z1FURmtpYmNIQi93MmMxdzQ2STNxT2gxZFlBbFMxcnJnUVZ3OXFWcDN5ZVRJRjdKRE1qRS8yK0FCYmJWSzRRRU9ORTZSb0d1c2MyNTNkR2NhWFlnbkRZL0hZaGRnb3ozUTVWMHZ0NW5jQ09TY0I5QURxTDZLV0YweTRha0ZGYUR3Qld6NjJZNWl4cTBMZmc0NFVnTitYVmhYU1BaM2NwaUppWGFxcXBNZEQyUXVOdVQ0Q3RaOVJBTlVHM1JEOXp1eVVLNUQ0NmYwbjRDaUsvejIzZjIrU2FESXI5bEkybjVzUmlMR2F1Z0l3aVFZUmVBMEJGL2tRNFBJOFpMdnFkSG1FbVkwTUhpbEhaMjdqYUR2K1dQRGRSVEZvY3hjOTFmSzlVdW1qU0VXUXBtdXJza3pjbzYvT0NIb2lrQm4xNDR0bFY1MlV4cnpsVURySG1pMEJJbWlTUXRldU9HeWV3ZEVhN3REdDNwNitZY3Z3bHVPSW94Znl0MWlIa2ZnOWEwTzJScHRBVVRRVzJQa0ZqYkhIQXJyRkcvbklsTWRlM01TYk9RblQ5eXNUblZ5VjNJSG9oNnZSL0xtMTBIQTJLSUFkNmk2dUhaWWNzeWY5S0tNZk55a0w1OEcrRGZIcnV3cnIzUUpZa001OG1aVDBsM2FmNzFmR2hkUndoN0JwVEhId1VCenc0Z1pxU1hSclBSekxRYVFmL3ZwMmtzYWttT2Y0dmdnR01ZL3F5TUpxM1lJVDR5QUhlbEJtWE9kRnUycjNndWorOURNTmd3VG52RmxGT0ZqZDBKUVlMWXdpS2tad2VqSDlKd3RxKzAvU05SYUZmakk0aUYxK2RlU1E4ckJxdVlNMTVRUmJRMTNJVHU4bmdZTVpDWjgyNEt2WkloWWNoZmNVSUdxUGpCMHZEeVFpbC9VODNlZk40aHJ1SmNXZGd5Q2hzL0NvRzlKRXo0WnZESC9neTVPRTBJZWtRcldNcmJkcWg3Y2tnSWRwR29WNm1LK2xWcGx4dytnY0VYMFNXeGtkcUhKWGJQUWpjWWl6dzRQVzFlZFdSZ2E1ZjZQTlFsUDFreXgxZVdFdXdnWEJLOXlBYk9uUWpJY3V0ODY2Nk5pUWxIano4eXJvOHZwSUdKQlUvZmdDVE1CYlRyVlZKR2k4b0dYRmhmcWpnNVBGdlZaNFlFYTIvMWR1b3gvODhLRWdUSEJhbTNXVHpsb1VLald5MXBwd0RYSXVrQS92UTRLdUppZWs5NGdUZVBKVU01OHYwbGZndVdyT2hGV1N4SXY4dHQxZz09</RedirectURL><GetToken>1</GetToken></PaymentRequest>", ParameterType.GetOrPost);
            var response = client.Execute(request);
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public ActionResult TestGetUserRequest()
        {
            var email = "akashkulkarni1313@gmail.com";

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = email;
            efmFirmServiceClient.ClientCredentials.UserName.Password = "2d697c3e-5c31-429e-b61a-9f127839184f";
            EFMFirm.GetUserRequestType getUserRequestType = new EFMFirm.GetUserRequestType();
            getUserRequestType.UserID = "71e68b26-552d-4783-b494-8700c021e166";
            efmFirmServiceClient.Open();
            var resp = efmFirmServiceClient.GetUser(getUserRequestType);
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestGetServiceContactList()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "Zahoorahmed481+1@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.QueryString["Password"];
            //   EFMFirm.CreateServiceContactRequestType createServiceContactRequestType = new EFMFirm.CreateServiceContactRequestType();

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetServiceContactList();
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestAuthenticateUser()
        {
            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            EFMUser.AuthenticateRequestType authenticateRequestType = new EFMUser.AuthenticateRequestType();
            authenticateRequestType.Password = "Zahoor1234#";
            authenticateRequestType.Email = "Zahoorahmed481+1@gmail.com";
            efmUserServiceClient.Open();
            var resp = efmUserServiceClient.AuthenticateUser(authenticateRequestType);
            efmUserServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }



        public ActionResult RegisterFirm(String Email, String Password)
        {

            //EfmFirmServiceClient client = new EfmFirmServiceClient(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\AltrueCA.pfx", "MyP@ssword1");

            //EFMClient efm;

            ///   EfmUserServiceClient client  = efm.CreateUserService();
            //  client.GetUserAsync
            // MapPath
            //EFMClient eFMClient = new EFMClient(Server.MapPath(@"~/AltrueCA.pfx"), "MyP@ssword1");            
            EFMFirm.EfmFirmServiceClient efmfirm_altru = new EFMFirm.EfmFirmServiceClient();
            // efmfirm_altru.ClientCredentials.ClientCertificate.Certificate = eFMClient;
            efmfirm_altru.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmfirm_altru.ClientCredentials.ServiceCertificate.DefaultCertificate = EFMClient.LoadCertificateFromFile(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\AltrueCA.pfx", "MyP@ssword1");
            //efmfirm_altru.add
            EFMFirm.RegistrationRequestType req_type = new EFMFirm.RegistrationRequestType();
            req_type.City = "Plano";
            req_type.CountryCode = "US";
            req_type.Email = Email; //"akashkulkarni1313@gmail.com";
            req_type.FirstName = "zahoor";
            req_type.LastName = "ahmd";
            req_type.MiddleName = "shaikh";
            req_type.Password = Password; // "Surajk@1234";
            req_type.PasswordAnswer = "blue";
            req_type.PasswordQuestion = "What color is the sky?";
            req_type.PhoneNumber = "9727133770";
            //  req_type.RegistrationType = RegistrationType.FirmAdministrator;
            req_type.StateCode = "TX";
            req_type.StreetAddressLine1 = "6500 International Pkwy";
            req_type.ZipCode = "75093";
            req_type.StreetAddressLine2 = "Suite 2000";
            req_type.FirmName = "aspl";
            req_type.RegistrationType = EFMFirm.RegistrationType.FirmAdministrator;

            var resp = efmfirm_altru.RegisterUser(req_type);
            return Json(resp, JsonRequestBehavior.AllowGet);


        }

        public ActionResult FullDeatils()
        {
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(Request.Form), JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegisterUser()
        {

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            EFMFirm.RegistrationRequestType registrationRequestType = new EFMFirm.RegistrationRequestType();
            switch (Request.Form["RegistrationType"])
            {
                case "FirmAdministrator":

                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdministrator;
                    registrationRequestType.FirmName = Request.Form["FirmName"];
                    registrationRequestType.PhoneNumber = Request.Form["PhoneNumber"];
                    break;
                case "FirmAdminNewMember":
                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdminNewMember;

                    registrationRequestType.FirmID = Request.Form["FirmID"];
                    //EFMFirm.EfmFirmServiceClient.
                    break;
                case "Individual":
                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.Individual;
                    registrationRequestType.PhoneNumber = Request.Form["PhoneNumber"];
                    break;
            }
            registrationRequestType.Email = Request.Form["Email"];
            registrationRequestType.FirstName = Request.Form["FirstName"];
            registrationRequestType.MiddleName = Request.Form["MiddleName"];
            registrationRequestType.LastName = Request.Form["LastName"];
            registrationRequestType.Password = Request.Form["Password"];
            registrationRequestType.PasswordQuestion = Request.Form["PasswordQuestion"];
            registrationRequestType.PasswordAnswer = Request.Form["PasswordAnswer"];
            registrationRequestType.StreetAddressLine1 = Request.Form["StreetAddressLine1"];
            registrationRequestType.StreetAddressLine2 = Request.Form["StreetAddressLine2"];
            registrationRequestType.City = Request.Form["City"];
            registrationRequestType.StateCode = Request.Form["StateCode"];
            registrationRequestType.ZipCode = Request.Form["ZipCode"];
            registrationRequestType.CountryCode = Request.Form["CountryCode"];
            // registrationRequestType.FirmID = Request.Form["FirmID"];



            if ((Request.Form["RegistrationType"]) == "FirmAdminNewMember")
            {
                using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
                {
                    UserNameHeader userNameHeader = new UserNameHeader();
                    userNameHeader.UserName = Request.Form["HeaderUserName"];
                    userNameHeader.Password = Request.Form["HeaderPassword"];
                    var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", userNameHeader);
                    OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                    var resp = efmFirmServiceClient.RegisterUser(registrationRequestType);

                    return Json(resp, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var resp = efmFirmServiceClient.RegisterUser(registrationRequestType);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult RegisterUserV2(String Email, String Password)
        //{
        //    string RegistrationType = "Individual";

        //    EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    EFMFirm.RegistrationRequestType registrationRequestType = new EFMFirm.RegistrationRequestType();
        //    switch (RegistrationType)
        //    {
        //        case "FirmAdministrator":

        //            registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdministrator;
        //            registrationRequestType.FirmName = Request.Form["FirmName"];
        //            registrationRequestType.PhoneNumber = Request.Form["PhoneNumber"];
        //            break;
        //        case "FirmAdminNewMember":
        //            registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdminNewMember;

        //            registrationRequestType.FirmID = Request.Form["FirmID"];
        //            //EFMFirm.EfmFirmServiceClient.
        //            break;
        //        case "Individual":
        //            registrationRequestType.RegistrationType = EFMFirm.RegistrationType.Individual;
        //            registrationRequestType.PhoneNumber = "1234567890";//Request.Form["PhoneNumber"];
        //            break;
        //    }

        //    registrationRequestType.Email = Email;          //Request.Form["Email"];
        //    registrationRequestType.FirstName = "zahoor";   //Request.Form["FirstName"];
        //    registrationRequestType.MiddleName = "shaikh";  //Request.Form["MiddleName"];
        //    registrationRequestType.LastName = "ahmd";      //Request.Form["LastName"];
        //    registrationRequestType.Password = Password;    //"Surajk@1234";//Request.Form["Password"];
        //    registrationRequestType.PasswordQuestion = "What color is the sky?"; // Request.Form["PasswordQuestion"];
        //    registrationRequestType.PasswordAnswer = "blue"; // Request.Form["PasswordAnswer"];
        //    registrationRequestType.StreetAddressLine1 = "6500 International Pkwy"; //Request.Form["StreetAddressLine1"];
        //    registrationRequestType.StreetAddressLine2 = "Suite 2000"; //Request.Form["StreetAddressLine2"];
        //    registrationRequestType.City = "Plano";     //Request.Form["City"];
        //    registrationRequestType.StateCode = "TX";   //Request.Form["StateCode"];
        //    registrationRequestType.ZipCode = "75093"; //Request.Form["ZipCode"];
        //    registrationRequestType.CountryCode = "US";//Request.Form["CountryCode"];
        //    registrationRequestType.FirmName = "apsl"; //Request.Form["FirmID"];



        //    if (RegistrationType == "FirmAdminNewMember")
        //    {
        //        using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //        {
        //            UserNameHeader userNameHeader = new UserNameHeader();
        //            userNameHeader.UserName = Request.Form["HeaderUserName"];
        //            userNameHeader.Password = Request.Form["HeaderPassword"];
        //            var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", userNameHeader);
        //            OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

        //            var resp = efmFirmServiceClient.RegisterUser(registrationRequestType);

        //            return Json(resp, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    else
        //    {
        //        var resp = efmFirmServiceClient.RegisterUser(registrationRequestType);
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //}



        //public ActionResult CaseInitiation()
        //{

        //    TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();


        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];

        //    TylerCourtRecordMDEService.CaseFilingType serviceCaseAugmentationType = new TylerCourtRecordMDEService.CaseFilingType();


        //    serviceCaseAugmentationType. = "";






        //    TylerCourtRecordMDEService.CourtType CaseCourt = new TylerCourtRecordMDEService.CourtType();
        //    CaseCourt.id = Request.Form["CourtLocationID"];
        //    serviceInformationQueryMessageType.CaseCourt = CaseCourt;
        //    //serviceInformationQueryMessageType.CaseTrackingID =new ; 
        //    //        
        //    //
        //    //    getPolicyRequestMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id= Request.Form["CourtLocationIdentifier"]; ;
        //    //            getPolicyRequestMessageType.GetPolicyRequestMessage.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationIdentifier"]; ;
        //    using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //    {
        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
        //        var resp = efmFirmServiceClient.GetDocument(serviceInformationQueryMessageType);
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //    // efmFirmServiceClient.Open();
        //    // // efmFirmServiceClient.GetUserList
        //    // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //    //     var resp = "";

        //    // efmFirmServiceClient.Close();
        //    // return Json(resp, JsonRequestBehavior.AllowGet);
        //}

        
        public ActionResult GetUserRequest(string UserName, string Password, string UserID)
        {
            EFMUser.EfmUserServiceClient efmFirmServiceClient = new EFMUser.EfmUserServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //EFMFirm.
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //Request.Form["Password"];
            EFMUser.GetUserRequestType getUserRequestType = new EFMUser.GetUserRequestType();
            getUserRequestType.UserID = UserID; //Request.Form["UserID"];
            //var resp = efmFirmServiceClient.GetUser(getUserRequestType);
            //efmFirmServiceClient.Close();
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetUser(getUserRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult testCustome()
        {

            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            //r svc = tyler_web_app.ServiceClient(binding, new EndpointAddress("https://california-stage.tylerhost.net/EFM/EFMFirmService.svc"));
            //var svc =FMFirm.EfmFirmServiceClient(binding);

            var svc = new EFMFirm.EfmFirmServiceClient(binding, new EndpointAddress("https://california-stage.tylerhost.net/EFM/EFMFirmService.svc"));
            try
            {
                using (var scope = new OperationContextScope(svc.InnerChannel))
                {
                    //      svc.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
                    var cus = new CustomHeader();
                    OperationContext.Current.OutgoingMessageHeaders.Add(cus);
                    var sec = new SecurityHeader("Zahoorahmed481+1@gmail.com", "e0fc9ebc-bba0-40c9-bfcd-83c7c6f9a50c");
                    OperationContext.Current.OutgoingMessageHeaders.Add(sec);


                    //turn await svc.retrieveResponseAsync(req); ;


                    var result = svc.GetUserList();
                    //   resp = result;
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //trun
                }
            }
            catch (Exception ex)
            {
                // log error
                return null;
            }
            finally
            {
                svc.Close();
            }

        }
        public ActionResult TestGetUserList()
        {

            //System.ServiceModel.Channels.Binding bindin;
            //ServiceSoapBindingStub binding = (ServiceSoapBindingStub)service;
            //binding.setUsername("Username");
            //binding.setPassword("Password");
            //79c4ece5-03e5-4981-9c5c-e77dd401088a
            var email = "Zahoorahmed481+1@gmail.com";
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;

            efmFirmServiceClient.ClientCredentials.UserName.UserName = email;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.QueryString["passwordhash"];



            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetUserList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            /*
            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //            efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];

            efmUserServiceClient.ClientCredentials.UserName.UserName = "Zahoorahmed481+1@gmail.com";
            efmUserServiceClient.ClientCredentials.UserName.Password = Request.QueryString["passwordhash"];

            EFMUser.GetPasswordQuestionRequestType getPasswordQuestionRequestType = new EFMUser.GetPasswordQuestionRequestType();

            getPasswordQuestionRequestType.Email = "Zahoorahmed481+1@gmail.com";

            efmUserServiceClient.Open();
            var resp = efmUserServiceClient.GetPasswordQuestion(getPasswordQuestionRequestType);
            efmUserServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
            */



        }
        public ActionResult GetUserList(String UserName, String Password)
        {

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+1@gmail.com";
            //efmFirmServiceClient.ClientCredentials.UserName.Password = "f36ba3c3-8e11-416f-a641-55fd0fdd2dd1";
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; // "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; // "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";
            //efmFirmServiceClient.ClientCredentials.ClientCertificate.
            // efmFirmServiceClient.ClientCredentials.ServiceCertificate.Authentication.
            //   efmFirmServiceClient.Get
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetUserList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetUserList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveUser(string UserName, string Password, string UserID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;//
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; // Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; // Request.Form["Password"];
            //Change 
            EFMFirm.RemoveUserRequestType removeUserRequestType = new EFMFirm.RemoveUserRequestType();
            removeUserRequestType.UserID = UserID; // Request.Form["UserID"];
            //Change 
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.RemoveUser(removeUserRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //var resp = efmFirmServiceClient.RemoveUser(removeUserRequestType);
            //efmFirmServiceClient.Close();

            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AuthenticateUser(string Email, string Password)
        {
            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            // efmUserServiceClient.ClientCr.
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //UserNameHeader
            EFMUser.AuthenticateRequestType authenticateRequestType = new EFMUser.AuthenticateRequestType();

            authenticateRequestType.Password = Password; // Request.Form["Password"];
            authenticateRequestType.Email = Email; // Request.Form["Email"];
            efmUserServiceClient.Open();

            var resp = efmUserServiceClient.AuthenticateUser(authenticateRequestType);
            efmUserServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ChangePassword()
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMUser.ChangePasswordRequestType changePasswordRequestType = new EFMUser.ChangePasswordRequestType();

            changePasswordRequestType.NewPassword = Request.Form["NewPassword"];
            changePasswordRequestType.OldPassword = Request.Form["OldPassword"];
            changePasswordRequestType.PasswordAnswer = Request.Form["PasswordAnswer"];
            changePasswordRequestType.PasswordQuestion = Request.Form["PasswordQuestion"];
            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.ChangePassword(changePasswordRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.ChangePassword(changePasswordRequestType);
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPasswordQuestion()
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMUser.GetPasswordQuestionRequestType getPasswordQuestionRequestType = new EFMUser.GetPasswordQuestionRequestType();

            getPasswordQuestionRequestType.Email = Request.Form["Email"];

            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.GetPasswordQuestion(getPasswordQuestionRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.GetPasswordQuestion(getPasswordQuestionRequestType);
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetPassword()
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            // efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMUser.ResetPasswordRequestType resetPasswordRequestType = new EFMUser.ResetPasswordRequestType();

            resetPasswordRequestType.Email = Request.Form["Email"];
            resetPasswordRequestType.PasswordAnswer = Request.Form["PasswordAnswer"];

            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.ResetPassword(resetPasswordRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.ResetPassword(resetPasswordRequestType);
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateUserUserService()
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMUser.UserType user = new EFMUser.UserType();
            user.Email = Request.Form["Email"];
            user.UserID = Request.Form["UserID"];
            user.FirstName = Request.Form["FirstName"];
            user.MiddleName = Request.Form["MiddleName"];
            user.LastName = Request.Form["LastName"];
            // updateUserRequestType.User.Email = Request.Form["Email"];
            //     updateUserRequestType.User.UserID = Request.Form["UserID"];
            // updateUserRequestType.User.FirstName = Request.Form["FirstName"];
            //updateUserRequestType.User.MiddleName = Request.Form["MiddleName"];
            //updateUserRequestType.User.LastName = Request.Form["LastName"];
            EFMUser.UpdateUserRequestType updateUserRequestType = new EFMUser.UpdateUserRequestType();
            updateUserRequestType.User = user;
            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.UpdateUser(updateUserRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.UpdateUser(updateUserRequestType);
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNotificationPreferences(string UserName, string Password)
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmUserServiceClient.ClientCredentials.UserName.UserName = UserName; // Request.Form["UserName"];
            efmUserServiceClient.ClientCredentials.UserName.Password = Password; // Request.Form["Password"];

            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.GetNotificationPreferences();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.GetNotificationPreferences();
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateNotificationPreferences(string UserName, string Password)
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmUserServiceClient.ClientCredentials.UserName.UserName = UserName; // Request.Form["UserName"];
            efmUserServiceClient.ClientCredentials.UserName.Password = Password; // Request.Form["Password"];
            EFMUser.UpdateNotificationPreferencesRequestType updateNotificationPreferencesRequestType = new EFMUser.UpdateNotificationPreferencesRequestType();

            //  updateNotificationPreferencesRequestType.Notification
            using (new OperationContextScope(efmUserServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmUserServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmUserServiceClient.UpdateNotificationPreferences(updateNotificationPreferencesRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmUserServiceClient.Open();
            //var resp = efmUserServiceClient.UpdateNotificationPreferences(updateNotificationPreferencesRequestType);
            //efmUserServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SelfResendActivationEmail()
        {

            EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
            efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //          efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];

            EFMUser.SelfResendActivationEmailRequestType selfResendActivationEmailRequestType = new EFMUser.SelfResendActivationEmailRequestType();
            selfResendActivationEmailRequestType.Email = Request.Form["Email"]; ;
            efmUserServiceClient.Open();
            var resp = efmUserServiceClient.SelfResendActivationEmail(selfResendActivationEmailRequestType);
            efmUserServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        /* public ActionResult SelfResendActivationEmail()
         {

             EFMUser.EfmUserServiceClient efmUserServiceClient = new EFMUser.EfmUserServiceClient();
             efmUserServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
             //            efmUserServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
             //          efmUserServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];

             EFMUser.SelfResendActivationEmailRequestType selfResendActivationEmailRequestType = new EFMUser.SelfResendActivationEmailRequestType();
             selfResendActivationEmailRequestType.Email = Request.Form["Email"]; ;
             efmUserServiceClient.Open();
             var resp = efmUserServiceClient.SelfResendActivationEmail(selfResendActivationEmailRequestType);
             efmUserServiceClient.Close();
             return Json(resp, JsonRequestBehavior.AllowGet);
         }*/

        public ActionResult AddUserRole(string UserName, string Password, string UserID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; // "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; // "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";

            EFMFirm.AddUserRoleRequestType addUserRoleRequestType = new EFMFirm.AddUserRoleRequestType();
            addUserRoleRequestType.UserID = UserID; //"42d80559-1181-46bd-b118-d6494980ae52"; //Request.Form["UserID"];
            switch (Request.Form["Role"])
            {
                case "Filer":
                    addUserRoleRequestType.Role = EFMFirm.RoleType.Filer;
                    break;
                case "FirmAdmin":
                    addUserRoleRequestType.Role = EFMFirm.RoleType.FirmAdmin;
                    break;
                case "CriminalFilingFiler":
                    addUserRoleRequestType.Role = EFMFirm.RoleType.CriminalFilingFiler;
                    break;
                case "CriminalFilingFirmAdmin":
                    addUserRoleRequestType.Role = EFMFirm.RoleType.CriminalFilingFirmAdmin;
                    break;
            }

            addUserRoleRequestType.Location = "slo"; // "California";  //Request.Form["Location"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.AddUserRole(addUserRoleRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult RemoveUserRole(string UserName, string Password, string UserID)
        {

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; // Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //Request.Form["Password"];
            EFMFirm.RemoveUserRoleRequestType removeUserRoleRequestType = new EFMFirm.RemoveUserRoleRequestType();
            removeUserRoleRequestType.UserID = UserID; //Request.Form["UserID"];
            switch (Request.Form["Role"])
            {
                case "Filer":
                    removeUserRoleRequestType.Role = EFMFirm.RoleType.Filer;
                    break;
                case "FirmAdmin":
                    removeUserRoleRequestType.Role = EFMFirm.RoleType.FirmAdmin;
                    break;
                case "CriminalFilingFiler":
                    removeUserRoleRequestType.Role = EFMFirm.RoleType.CriminalFilingFiler;
                    break;
                case "CriminalFilingFirmAdmin":
                    removeUserRoleRequestType.Role = EFMFirm.RoleType.CriminalFilingFirmAdmin;
                    break;
            }
            //removeUserRoleRequestType.Location = Request.Form["Location"];
            removeUserRoleRequestType.Location = "slo"; // "California";  //Request.Form["Location"];
            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.RemoveUserRole(removeUserRoleRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //var resp = efmFirmServiceClient.RemoveUserRole(removeUserRoleRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResendActivationEmail()
        {

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.ResendActivationEmailRequestType resendActivationEmailRequestType = new EFMFirm.ResendActivationEmailRequestType();
            resendActivationEmailRequestType.UserID = Request.Form["UserID"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.ResendActivationEmail(resendActivationEmailRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.ResendActivationEmail(resendActivationEmailRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetUserPassword()
        {

            /*UserID
UserName
Password
Email
NewPassword*/
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.ResetUserPasswordRequestType resetPasswordRequestType = new EFMFirm.ResetUserPasswordRequestType();
            resetPasswordRequestType.Email = Request.Form["Email"];
            resetPasswordRequestType.Password = Request.Form["NewPassword"];
            resetPasswordRequestType.UserID = Request.Form["UserID"];


            //ResetUserPassword
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                //EFMFirm.ResetPasswordRequestType resetPasswordRequestType1 = resetPasswordRequestType;
                var resp = efmFirmServiceClient.ResetUserPassword(resetPasswordRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            // resetPasswordRequestType
            // efmFirmServiceClient.Open();
            // // efmFirmServiceClient.GetUserList
            // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //// var resp = efmFirmServiceClient.ResetUserPassword(resetPasswordRequestType);
            // efmFirmServiceClient.Close();
            // return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUser()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.UpdateUserRequestType updateUserRequestType = new EFMFirm.UpdateUserRequestType();
            EFMFirm.UserType user = new EFMFirm.UserType();
            // user
            user.Email = Request.Form["Email"];
            user.FirmID = Request.Form["FirmID"];
            user.FirstName = Request.Form["FirstName"];
            user.MiddleName = Request.Form["MiddleName"];
            user.LastName = Request.Form["LastName"];
            user.UserID = Request.Form["UserID"];
            updateUserRequestType.User = user;
            //updateUserRequestType.User.Email = Request.Form["Email"];
            //updateUserRequestType.User.FirmID = Request.Form["FirmID"];
            //updateUserRequestType.User.FirstName = Request.Form["FirstName"];
            //updateUserRequestType.User.MiddleName = Request.Form["MiddleName"];
            //updateUserRequestType.User.LastName = Request.Form["LastName"];
            //updateUserRequestType.User.UserID = Request.Form["UserID"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.UpdateUser(updateUserRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.UpdateUser(updateUserRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNotificationPreferencesList()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetNotificationPreferencesList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //         efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetNotificationPreferencesList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFirm(String UserName, String Password)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();

            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //"zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";


            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetFirm();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult UpdateFirm()
        {
            /*
UserName:Zahoorahmed481+1@gmail.com
Password:e0fc9ebc-bba0-40c9-bfcd-83c7c6f9a50c
FirmID:0aeced3d-138d-4dbc-8e0a-f963256d14ac
FirmName:ASPL
AddressLine1:dasd
AddressLine2:asdad
City:asdsa
State:CA
ZipCode:90001
Country:US
PhoneNumber:9012312321
                         */
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //EFMFirm.UpdateFirmRequestType
            EFMFirm.UpdateFirmRequestType updateFirmRequestType = new EFMFirm.UpdateFirmRequestType();
            EFMFirm.FirmType firm = new EFMFirm.FirmType();
            firm.FirmName = Request.Form["FirmName"];
            firm.FirmID = Request.Form["FirmID"];

            firm.Address.AddressLine1 = Request.Form["AddressLine1"];
            firm.Address.AddressLine2 = Request.Form["AddressLine2"];
            firm.Address.City = Request.Form["City"];
            firm.Address.Country = Request.Form["Country"];
            firm.Address.State = Request.Form["State"];
            firm.Address.ZipCode = Request.Form["ZipCode"];

            firm.PhoneNumber = Request.Form["PhoneNumber"];
            updateFirmRequestType.Firm = firm;
            //updateFirmRequestType.Firm.FirmName = Request.Form["FirmName"];
            //updateFirmRequestType.Firm.FirmID = Request.Form["FirmID"];

            //updateFirmRequestType.Firm.Address.AddressLine1 = Request.Form["AddressLine1"];
            //updateFirmRequestType.Firm.Address.AddressLine2 = Request.Form["AddressLine2"];
            //updateFirmRequestType.Firm.Address.City = Request.Form["City"];
            //updateFirmRequestType.Firm.Address.Country = Request.Form["Country"]; 
            //updateFirmRequestType.Firm.Address.State = Request.Form["State"];
            //updateFirmRequestType.Firm.Address.ZipCode = Request.Form["ZipCode"];

            //updateFirmRequestType.Firm.PhoneNumber = Request.Form["PhoneNumber"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.UpdateFirm(updateFirmRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;


            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.UpdateFirm(updateFirmRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult CreateAttorney()
        //{
        //    EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //    EFMFirm.CreateAttorneyRequestType createAttorneyRequestType = new EFMFirm.CreateAttorneyRequestType();
        //    EFMFirm.AttorneyType attorneyType = new EFMFirm.AttorneyType();

        //    attorneyType.BarNumber = Request.Form["BarNumber"];
        //    attorneyType.FirstName = Request.Form["FirstName"];
        //    attorneyType.MiddleName = Request.Form["MiddleName"];
        //    attorneyType.LastName = Request.Form["LastName"];
        //    attorneyType.FirmID = Request.Form["FirmID"];
        //    createAttorneyRequestType.Attorney = attorneyType;

        //    using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //    {

        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

        //        var resp = efmFirmServiceClient.CreateAttorney(createAttorneyRequestType);

        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }

        //}


        public ActionResult CreateAttorney(string UserName, string Password, string BarNumber, string FirstName, string MiddleName, string LastName, string FirmID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;//"zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;//"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";
            EFMFirm.CreateAttorneyRequestType createAttorneyRequestType = new EFMFirm.CreateAttorneyRequestType();
            EFMFirm.AttorneyType attorneyType = new EFMFirm.AttorneyType();

            attorneyType.BarNumber = BarNumber;//"900009";
            attorneyType.FirstName = FirstName;//"Ram";
            attorneyType.MiddleName = MiddleName;//"shyam";
            attorneyType.LastName = LastName;//"nam";
            attorneyType.FirmID = FirmID;//"83fe15e1-2896-478c-b4ec-a1996b4e2f46";
            createAttorneyRequestType.Attorney = attorneyType;

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.CreateAttorney(createAttorneyRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAttorneyList(String UserName, String Password)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();

            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;

            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;//"zahoorahmed481+26@gmail.com";

            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;//"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetAttorneyList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAttorney(String UserName, String Password, string AttorneyID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;//"zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;//"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";
            EFMFirm.GetAttorneyRequestType getAttorneyRequestType = new EFMFirm.GetAttorneyRequestType();
            getAttorneyRequestType.AttorneyID = AttorneyID;//"1a1d114b-f751-49ce-bec3-5379dfe45b18";

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetAttorney(getAttorneyRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveAttorney(string UserName, string Password, string AttorneyID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; // "zahoorahmed481+26@gmail.com";//request.form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; // "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";//Request.Form["Password"];
            EFMFirm.RemoveAttorneyRequestType removeAttorneyRequestType = new EFMFirm.RemoveAttorneyRequestType();
            removeAttorneyRequestType.AttorneyID = AttorneyID;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.RemoveAttorney(removeAttorneyRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateAttorney(string UserName, string Password, string AttorneyID, string BarNumber, string FirstName, string MiddleName, string LastName, string FirmID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //"zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";
            EFMFirm.UpdateAttorneyRequestType updateAttorneyRequestType = new EFMFirm.UpdateAttorneyRequestType();
            EFMFirm.AttorneyType attorneyType = new EFMFirm.AttorneyType();

            attorneyType.AttorneyID = AttorneyID; //"384ba7ce-1c41-4ea8-bd31-f7ed2df92f5f";
            attorneyType.BarNumber = BarNumber;// "900009";
            attorneyType.FirstName = FirstName; //"TestFirstName2";
            attorneyType.MiddleName = MiddleName; // "TestMiddleName2";
            attorneyType.LastName = LastName;//"TestLastName2";
            attorneyType.FirmID = FirmID;//"83fe15e1-2896-478c-b4ec-a1996b4e2f46";

            updateAttorneyRequestType.Attorney = attorneyType;
            //updateAttorneyRequestType.Attorney.AttorneyID = Request.Form["AttorneyID"];
            //updateAttorneyRequestType.Attorney.BarNumber = Request.Form["BarNumber"];
            //updateAttorneyRequestType.Attorney.FirstName = Request.Form["FirstName"];
            //updateAttorneyRequestType.Attorney.MiddleName = Request.Form["MiddleName"];
            //updateAttorneyRequestType.Attorney.LastName = Request.Form["LastName"];
            //updateAttorneyRequestType.Attorney.FirmID = Request.Form["FirmID"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.UpdateAttorney(updateAttorneyRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.UpdateAttorney(updateAttorneyRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }


        // We have added parameters here
        public ActionResult CreateServiceContact(string UserName, string Password, string FirmID,
            string FirstName, string MiddleName, string LastName, string Email, string AdministrativeCopy,
            string AddByFirmName, string AddressLine1, string AddressLine2,
            string City, string State, string ZipCode, string Country, string PhoneNumber)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.CreateServiceContactRequestType createServiceContactRequestType = new EFMFirm.CreateServiceContactRequestType();
            EFMFirm.ServiceContactType serviceContactType = new EFMFirm.ServiceContactType();

            //serviceContactType.FirmID = Request.Form["FirmID"];
            //serviceContactType.FirstName = Request.Form["FirstName"];
            //serviceContactType.MiddleName = Request.Form["MiddleName"];
            //serviceContactType.LastName = Request.Form["LastName"];
            //serviceContactType.Email = Request.Form["Email"];
            //serviceContactType.AdministrativeCopy = Request.Form["AdministrativeCopy"];
            //serviceContactType.AddByFirmName = Request.Form["AddByFirmName"];
            //EFMFirm.AddressType addressType = new EFMFirm.AddressType();

            //addressType.AddressLine1 = Request.Form["AddressLine1"];
            //addressType.AddressLine2 = Request.Form["AddressLine2"];
            //addressType.City = Request.Form["City"];
            //addressType.State = Request.Form["State"];
            //addressType.ZipCode = Request.Form["ZipCode"];
            //addressType.Country = Request.Form["Country"];
            //serviceContactType.Address = addressType;

            //serviceContactType.PhoneNumber = Request.Form["PhoneNumber"];

            serviceContactType.FirmID = FirmID;
            serviceContactType.FirstName = FirstName;
            serviceContactType.MiddleName = MiddleName;
            serviceContactType.LastName = LastName;
            serviceContactType.Email = Email;
            serviceContactType.AdministrativeCopy = AdministrativeCopy;
            serviceContactType.AddByFirmName = AddByFirmName;
            EFMFirm.AddressType addressType = new EFMFirm.AddressType();

            addressType.AddressLine1 = AddressLine1;
            addressType.AddressLine2 = AddressLine2;
            addressType.City = City;
            addressType.State = State;
            addressType.ZipCode = ZipCode;
            addressType.Country = Country;
            serviceContactType.Address = addressType;

            serviceContactType.PhoneNumber = PhoneNumber;

            // if (Request.Form.AllKeys.Contains("IsPublic"))
            serviceContactType.IsPublic = true;
            //  else
            //    serviceContactType.IsPublic = true;

            //    if (Request.Form.AllKeys.Contains("IsInFirmMasterList"))
            //        serviceContactType.IsInFirmMasterList = false;
            //   else
            serviceContactType.IsInFirmMasterList = true;

            createServiceContactRequestType.ServiceContact = serviceContactType;
            //createServiceContactRequestType.ServiceContact.FirmID = Request.Form["FirmID"];
            //createServiceContactRequestType.ServiceContact.FirstName = Request.Form["FirstName"];
            //createServiceContactRequestType.ServiceContact.MiddleName = Request.Form["MiddleName"];
            //createServiceContactRequestType.ServiceContact.LastName = Request.Form["LastName"];
            //createServiceContactRequestType.ServiceContact.Email = Request.Form["Email"];
            //createServiceContactRequestType.ServiceContact.AdministrativeCopy = Request.Form["AdministrativeCopy"];
            //createServiceContactRequestType.ServiceContact.Address.AddressLine1 = Request.Form["AddressLine1"];
            //createServiceContactRequestType.ServiceContact.Address.AddressLine2 = Request.Form["AddressLine2"];
            //createServiceContactRequestType.ServiceContact.Address.City = Request.Form["City"];
            //createServiceContactRequestType.ServiceContact.Address.State = Request.Form["State"];
            //createServiceContactRequestType.ServiceContact.Address.ZipCode = Request.Form["ZipCode"];
            //createServiceContactRequestType.ServiceContact.Address.Country = Request.Form["Country"];

            //createServiceContactRequestType.ServiceContact.PhoneNumber = Request.Form["PhoneNumber"];
            //createServiceContactRequestType.ServiceContact.IsPublic =Boolean.Parse( Request.Form["IsPublic"]);
            //createServiceContactRequestType.ServiceContact.IsInFirmMasterList = Boolean.Parse(Request.Form["IsInFirmMasterList"]);
            createServiceContactRequestType.ServiceContact.IsPublic = true;
            createServiceContactRequestType.ServiceContact.IsInFirmMasterList = true;

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.CreateServiceContact(createServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.CreateServiceContact(createServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetServiceContactList(string UserName, string Password) // We have added parameters here
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //Request.Form["Password"];
            //         efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; //Request.Form["Password"];
            //   EFMFirm.CreateServiceContactRequestType createServiceContactRequestType = new EFMFirm.CreateServiceContactRequestType();

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetServiceContactList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetServiceContactList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetServiceContact(string UserName, string Password, string ServiceContactID) // We have added parameters here
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            //efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //EFMFirm.GetServiceContactRequestType getServiceContactRequestType = new EFMFirm.GetServiceContactRequestType();
            //getServiceContactRequestType.ServiceContactID = Request.Form["ServiceContactID"];
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            EFMFirm.GetServiceContactRequestType getServiceContactRequestType = new EFMFirm.GetServiceContactRequestType();
            getServiceContactRequestType.ServiceContactID = ServiceContactID;

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetServiceContact(getServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetServiceContact(getServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveServiceContact(string UserName, string Password, string ServiceContactID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.RemoveServiceContactRequestType getServiceContactRequestType = new EFMFirm.RemoveServiceContactRequestType();
            //getServiceContactRequestType.ServiceContactID = Request.Form["ServiceContactID"];
            getServiceContactRequestType.ServiceContactID = ServiceContactID;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.RemoveServiceContact(getServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.RemoveServiceContact(getServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateServiceContact(string UserName, string Password, string ServiceContactID, string FirmID,
            string FirstName, string MiddleName, string LastName, string Email, string AdministrativeCopy,
            string AddByFirmName, string AddressLine1, string AddressLine2,
            string City, string State, string ZipCode, string Country, string PhoneNumber)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.UpdateServiceContactRequestType updateServiceContactRequestType = new EFMFirm.UpdateServiceContactRequestType();
            EFMFirm.ServiceContactType serviceContactType = new EFMFirm.ServiceContactType();

            //serviceContactType.ServiceContactID = Request.Form["ServiceContactID"];
            //serviceContactType.FirmID = Request.Form["FirmID"];
            //serviceContactType.FirstName = Request.Form["FirstName"];
            //serviceContactType.MiddleName = Request.Form["MiddleName"];
            //serviceContactType.LastName = Request.Form["LastName"];
            //serviceContactType.Email = Request.Form["Email"];
            //serviceContactType.AddByFirmName = Request.Form["AddByFirmName"];
            //EFMFirm.AddressType addressType = new EFMFirm.AddressType();

            //addressType.AddressLine1 = Request.Form["AddressLine1"];
            //addressType.AddressLine2 = Request.Form["AddressLine2"];
            //addressType.City = Request.Form["City"];
            //addressType.State = Request.Form["State"];
            //addressType.ZipCode = Request.Form["ZipCode"];
            //addressType.Country = Request.Form["Country"];

            serviceContactType.ServiceContactID = ServiceContactID;
            serviceContactType.FirmID = FirmID;
            serviceContactType.FirstName = FirstName;
            serviceContactType.MiddleName = MiddleName;
            serviceContactType.LastName = LastName;
            serviceContactType.Email = Email;
            serviceContactType.AdministrativeCopy = AdministrativeCopy;
            serviceContactType.AddByFirmName = AddByFirmName;
            EFMFirm.AddressType addressType = new EFMFirm.AddressType();

            addressType.AddressLine1 = AddressLine1;
            addressType.AddressLine2 = AddressLine2;
            addressType.City = City;
            addressType.State = State;
            addressType.ZipCode = ZipCode;
            addressType.Country = Country;
            serviceContactType.Address = addressType;

            serviceContactType.PhoneNumber = PhoneNumber;

            if (Request.Form.AllKeys.Contains("IsPublic"))
                serviceContactType.IsPublic = false;
            else
                serviceContactType.IsPublic = true;

            if (Request.Form.AllKeys.Contains("IsInFirmMasterList"))
                serviceContactType.IsInFirmMasterList = false;
            else
                serviceContactType.IsInFirmMasterList = true;

            updateServiceContactRequestType.ServiceContact = serviceContactType;
            //updateServiceContactRequestType.ServiceContact.ServiceContactID = Request.Form["ServiceContactID"];
            //updateServiceContactRequestType.ServiceContact.FirmID = Request.Form["FirmID"];
            //updateServiceContactRequestType.ServiceContact.FirstName = Request.Form["FirstName"];
            //updateServiceContactRequestType.ServiceContact.MiddleName = Request.Form["MiddleName"];
            //updateServiceContactRequestType.ServiceContact.LastName = Request.Form["LastName"];
            //updateServiceContactRequestType.ServiceContact.Email = Request.Form["Email"];
            //updateServiceContactRequestType.ServiceContact.AdministrativeCopy = Request.Form["AdministrativeCopy"];
            //updateServiceContactRequestType.ServiceContact.Address.AddressLine1 = Request.Form["AddressLine1"];
            //updateServiceContactRequestType.ServiceContact.Address.AddressLine2 = Request.Form["AddressLine2"];
            //updateServiceContactRequestType.ServiceContact.Address.City = Request.Form["City"];
            //updateServiceContactRequestType.ServiceContact.Address.State = Request.Form["State"];
            //updateServiceContactRequestType.ServiceContact.Address.ZipCode = Request.Form["ZipCode"];
            //updateServiceContactRequestType.ServiceContact.Address.Country = Request.Form["Country"];

            //updateServiceContactRequestType.ServiceContact.PhoneNumber = Request.Form["PhoneNumber"];
            //updateServiceContactRequestType.ServiceContact.IsPublic = Boolean.Parse(Request.Form["IsPublic"]);
            //updateServiceContactRequestType.ServiceContact.IsInFirmMasterList = Boolean.Parse(Request.Form["IsInFirmMasterList"]);

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.UpdateServiceContact(updateServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.UpdateServiceContact(updateServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AttachServiceContact()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.AttachServiceContactRequestType attachServiceContactRequestType = new EFMFirm.AttachServiceContactRequestType();
            attachServiceContactRequestType.ServiceContactID = Request.Form["ServiceContactID"];
            attachServiceContactRequestType.CaseID = Request.Form["CaseID"];
            attachServiceContactRequestType.CasePartyID = Request.Form["CasePartyID"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.AttachServiceContact(attachServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.AttachServiceContact(attachServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetachServiceContact()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.DetachServiceContactRequestType attachServiceContactRequestType = new EFMFirm.DetachServiceContactRequestType();
            attachServiceContactRequestType.ServiceContactID = Request.Form["ServiceContactID"];
            attachServiceContactRequestType.CaseID = Request.Form["CaseID"];
            attachServiceContactRequestType.CasePartyID = Request.Form["CasePartyID"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.DetachServiceContact(attachServiceContactRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.DetachServiceContact(attachServiceContactRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPublicList()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.GetPublicListRequestType getPublicListRequestType = new EFMFirm.GetPublicListRequestType();
            getPublicListRequestType.Email = Request.Form["Email"];
            getPublicListRequestType.FirmName = Request.Form["FirmName"];
            getPublicListRequestType.FirstName = Request.Form["FirstName"];
            getPublicListRequestType.LastName = Request.Form["LastName"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetPublicList(getPublicListRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetPublicList(getPublicListRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPaymentAccountTypeList(string UserName, string Password)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetPaymentAccountTypeList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetPaymentAccountTypeList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePaymentAccount()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.CreatePaymentAccountRequestType createPaymentAccountRequestType = new EFMFirm.CreatePaymentAccountRequestType();
            EFMFirm.PaymentAccountType paymentAccountType = new EFMFirm.PaymentAccountType();
            paymentAccountType.PaymentAccountTypeCode = Request.Form["PaymentAccountTypeCode"];
            paymentAccountType.PaymentAccountID = Request.Form["PaymentAccountID"];
            paymentAccountType.PaymentAccountTypeCodeID = Request.Form["PaymentAccountTypeCodeID"];

            //paymentAccountType.PaymentAccountTypeCodeId = Request.Form["PaymentAccountID"];

            paymentAccountType.AccountName = Request.Form["AccountName"];
            paymentAccountType.AccountToken = Request.Form["AccountToken"];
            paymentAccountType.CardType = Request.Form["CardType"];
            paymentAccountType.CardLast4 = Request.Form["CardLast4"];
            paymentAccountType.CardMonth = Int16.Parse(Request.Form["CardMonth"]);
            paymentAccountType.CardYear = Int16.Parse(Request.Form["CardYear"]);
            paymentAccountType.CardHolderName = Request.Form["CardHolderName"];
            createPaymentAccountRequestType.PaymentAccount = paymentAccountType;
            //createPaymentAccountRequestType.PaymentAccount.PaymentAccountTypeCode = Request.Form["PaymentAccountTypeCode"];
            //createPaymentAccountRequestType.PaymentAccount.AccountName = Request.Form["AccountName"];
            //createPaymentAccountRequestType.PaymentAccount.AccountToken = Request.Form["AccountToken"];
            //createPaymentAccountRequestType.PaymentAccount.CardType = Request.Form["CardType"];
            //createPaymentAccountRequestType.PaymentAccount.CardLast4 = Request.Form["CardLast4"];
            //createPaymentAccountRequestType.PaymentAccount.CardMonth = Int16.Parse(Request.Form["CardMonth"]);
            //createPaymentAccountRequestType.PaymentAccount.CardYear = Int16.Parse(Request.Form["CardYear"]);
            //createPaymentAccountRequestType.PaymentAccount.CardHolderName = Request.Form["CardHolderName"];

            //createPaymentAccountRequestType.PaymentAccount.PaymentAccountTypeCode.i = Request.Form["CourtIdentifier"];

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                //OperationContext.Current.OutgoingMessageProperties.Add();
                var resp = efmFirmServiceClient.CreatePaymentAccount(createPaymentAccountRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetPaymentAccountTypeList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPaymentAccountList(string UserName, string Password)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //         efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetPaymentAccountList();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetPaymentAccountList();
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPaymentAccount(string UserName, string Password, string PaymentAccountID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.GetPaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.GetPaymentAccountRequestType();
            //getPaymentAccountRequestType.PaymentAccountID = Request.Form["PaymentAccountID"];
            getPaymentAccountRequestType.PaymentAccountID = PaymentAccountID;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetPaymentAccount(getPaymentAccountRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetPaymentAccount(getPaymentAccountRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemovePaymentAccount(string UserName, string Password, string PaymentAccountID)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            EFMFirm.RemovePaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.RemovePaymentAccountRequestType();
            getPaymentAccountRequestType.PaymentAccountID = PaymentAccountID;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.RemovePaymentAccount(getPaymentAccountRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.RemovePaymentAccount(getPaymentAccountRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePaymentAccount(string UserName, string Password, string PaymentAccountTypeCode, string PaymentAccountID, string AccountName, string AccountToken)
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName;
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password;
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            EFMFirm.UpdatePaymentAccountRequestType updatePaymentAccountRequestType = new EFMFirm.UpdatePaymentAccountRequestType();
            EFMFirm.PaymentAccountType paymentAccountType = new EFMFirm.PaymentAccountType();
            paymentAccountType.PaymentAccountTypeCode = PaymentAccountTypeCode;

            paymentAccountType.PaymentAccountID = PaymentAccountID;
            paymentAccountType.AccountName = AccountName;
            //  updatePaymentAccountRequestType.PaymentAccount.CourtIdentifier = Request.Form["CourtIdentifier"];
            paymentAccountType.AccountToken = AccountToken;
            //paymentAccountType.PaymentAccountTypeCode = Request.Form["PaymentAccountTypeCode"];

            //paymentAccountType.PaymentAccountID = Request.Form["PaymentAccountID"];
            //paymentAccountType.AccountName = Request.Form["PaymentAccountID"];
            ////  updatePaymentAccountRequestType.PaymentAccount.CourtIdentifier = Request.Form["CourtIdentifier"];
            //paymentAccountType.AccountToken = Request.Form["AccountToken"];
            paymentAccountType.Active = true; // Boolean.Parse(Request.Form["Active"]);
            updatePaymentAccountRequestType.PaymentAccount = paymentAccountType;
            //  paymentAccountType.PaymentAccountTypeCode = Request.Form["PaymentAccountTypeCode"];

            //  updatePaymentAccountRequestType.PaymentAccount.PaymentAccountID = Request.Form["PaymentAccountID"];
            //  updatePaymentAccountRequestType.PaymentAccount.AccountName = Request.Form["PaymentAccountID"];
            ////  updatePaymentAccountRequestType.PaymentAccount.CourtIdentifier = Request.Form["CourtIdentifier"];
            //  updatePaymentAccountRequestType.PaymentAccount.AccountToken = Request.Form["AccountToken"];
            //  updatePaymentAccountRequestType.PaymentAccount.Active = Boolean.Parse( Request.Form["Active"]);
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.UpdatePaymentAccount(updatePaymentAccountRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.UpdatePaymentAccount(updatePaymentAccountRequestType);
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPolicy()
        {
            CourtPolicyMDE.CourtPolicyMDEClient efmFirmServiceClient = new CourtPolicyMDE.CourtPolicyMDEClient();
            //tPolicyMDE.CourtPolicyMDEClient

            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //EFMFirm.RemovePaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.RemovePaymentAccountRequestType();
            //getPaymentAccountRequestType.PaymentAccountID = Request.Form["PaymentAccountID"];
            CourtPolicyMDE.GetPolicyRequestType getPolicyRequestMessageType = new CourtPolicyMDE.GetPolicyRequestType();
            //    getPolicyRequestMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id= Request.Form["CourtLocationIdentifier"]; ;

            getPolicyRequestMessageType.GetPolicyRequestMessage.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationIdentifier"]; ;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.GetPolicy(getPolicyRequestMessageType);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            // efmFirmServiceClient.Open();
            // // efmFirmServiceClient.GetUserList
            // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            //     var resp = "";

            // efmFirmServiceClient.Close();
            // return Json(resp, JsonRequestBehavior.AllowGet);
        }


        //// WORKING
        //public ActionResult GetPolicyV5()
        //{
        //    CourtPolicyServiceV5.CourtPolicyMDEClient CourtPolicyMDEClient = new CourtPolicyServiceV5.CourtPolicyMDEClient();
        //    //CourtPolicyMDE.CourtPolicyMDEClient efmFirmServiceClient = new CourtPolicyMDE.CourtPolicyMDEClient();
        //    //tPolicyMDE.CourtPolicyMDEClient

        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    CourtPolicyMDEClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    CourtPolicyMDEClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
        //    CourtPolicyMDEClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8";
        //    //EFMFirm.RemovePaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.RemovePaymentAccountRequestType();
        //    //getPaymentAccountRequestType.PaymentAccountID = Request.Form["PaymentAccountID"];

        //    CourtPolicyServiceV5.GetPolicyRequestType getPolicyRequestType = new CourtPolicyServiceV5.GetPolicyRequestType()
        //    {
        //        CaseCourt = new CourtPolicyServiceV5.CourtType()
        //        {
        //            OrganizationIdentification = new CourtPolicyServiceV5.OrganizationType().OrganizationIdentification
        //        }
        //    };


        //    //    getPolicyRequestMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id= Request.Form["CourtLocationIdentifier"]; ;

        //    //SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = "S0x30" },

        //    //getPolicyRequestType.GetPolicyRequestMessage.CaseCourt.OrganizationIdentification.IdentificationID.id = "harris:dc"; //Request.Form["CourtLocationIdentifier"]; ;

        //    using (new OperationContextScope(CourtPolicyMDEClient.InnerChannel))
        //    {
        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", CourtPolicyMDEClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
        //        var resp = CourtPolicyMDEClient.GetPolicy(getPolicyRequestType);
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //    // efmFirmServiceClient.Open();
        //    // // efmFirmServiceClient.GetUserList
        //    // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //    //     var resp = "";

        //    // efmFirmServiceClient.Close();
        //    // return Json(resp, JsonRequestBehavior.AllowGet);
        //}
        ////

        //public ActionResult GetPolicyV5()
        //{
        //    CourtPolicyServiceV5.CourtPolicyMDEClient CourtPolicyMDEClient = new CourtPolicyServiceV5.CourtPolicyMDEClient();
        //    //CourtPolicyMDE.CourtPolicyMDEClient efmFirmServiceClient = new CourtPolicyMDE.CourtPolicyMDEClient();
        //    //tPolicyMDE.CourtPolicyMDEClient

        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    CourtPolicyMDEClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    CourtPolicyMDEClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
        //    CourtPolicyMDEClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8";
        //    //EFMFirm.RemovePaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.RemovePaymentAccountRequestType();
        //    //getPaymentAccountRequestType.PaymentAccountID = Request.Form["PaymentAccountID"];

        //    CourtPolicyServiceV5.GetPolicyRequestType getPolicyRequestType = new CourtPolicyServiceV5.GetPolicyRequestType()
        //    {
        //        DocumentIdentification = new CourtPolicyServiceV5.IdentificationType[]
        //        {                    
        //            //IdentificationID = new CourtPolicyServiceV5.@string() { Value=""}
        //            new CourtPolicyServiceV5.IdentificationType()
        //            {
        //                IdentificationID = new CourtPolicyServiceV5.@string() { Value = "1" },
        //                IdentificationCategoryDescriptionText = new CourtPolicyServiceV5.TextType(),
        //                IdentificationSourceText = new CourtPolicyServiceV5.TextType()                        
        //            }
        //        },
        //        SendingMDELocationID = new CourtPolicyServiceV5.IdentificationType() 
        //        { 
        //            IdentificationID = new CourtPolicyServiceV5.@string() { Value = "https://localhost:44392/" }
        //        },
        //        ServiceInteractionProfileCode = new CourtPolicyServiceV5.normalizedString() { Value= "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-5.0" },
        //        CaseCourt = new CourtPolicyServiceV5.CourtType()
        //        {
        //            OrganizationIdentification = new CourtPolicyServiceV5.IdentificationType()
        //            {
        //                IdentificationID = new CourtPolicyServiceV5.@string() { id = "harris:dc", Value = "harris:dc" }
        //                //IdentificationID = new CourtPolicyServiceV5.@string() { id = "harris:dc", Value = "harris:dc" }

        //            },
        //            //OrganizationIdentification = new CourtPolicyServiceV5.OrganizationType().OrganizationIdentification

        //        },
        //        DocumentPostDate = new CourtPolicyServiceV5.DateType(),
        //        PolicyQueryCriteria = new CourtPolicyServiceV5.PolicyQueryCriteriaType()
        //    };


        //    //    getPolicyRequestMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id= Request.Form["CourtLocationIdentifier"]; ;

        //    //SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = "S0x30" },

        //    //getPolicyRequestType.GetPolicyRequestMessage.CaseCourt.OrganizationIdentification.IdentificationID.id = "harris:dc"; //Request.Form["CourtLocationIdentifier"]; ;

        //    using (new OperationContextScope(CourtPolicyMDEClient.InnerChannel))
        //    {
        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", CourtPolicyMDEClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
        //        var resp = CourtPolicyMDEClient.GetPolicy(getPolicyRequestType);
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //    // efmFirmServiceClient.Open();
        //    // // efmFirmServiceClient.GetUserList
        //    // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //    //     var resp = "";

        //    // efmFirmServiceClient.Close();
        //    // return Json(resp, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetPolicyV5()
        {
            CourtPolicyServiceV5.CourtPolicyMDEClient CourtPolicyMDEClient = new CourtPolicyServiceV5.CourtPolicyMDEClient();

            CourtPolicyMDEClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            CourtPolicyMDEClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            CourtPolicyMDEClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8";

            CourtPolicyServiceV5.GetPolicyRequestType getPolicyRequestType = new CourtPolicyServiceV5.GetPolicyRequestType()
            {
                DocumentIdentification = new CourtPolicyServiceV5.IdentificationType[]
                {                    
                    new CourtPolicyServiceV5.IdentificationType()
                    {
                        IdentificationID = new CourtPolicyServiceV5.@string() { Value = "1" },
                        IdentificationCategoryDescriptionText = new CourtPolicyServiceV5.TextType(),
                        IdentificationSourceText = new CourtPolicyServiceV5.TextType()
                    }
                },
                SendingMDELocationID = new CourtPolicyServiceV5.IdentificationType()
                {
                    IdentificationID = new CourtPolicyServiceV5.@string() { Value = "https://localhost:44392/" }
                },
                ServiceInteractionProfileCode = new CourtPolicyServiceV5.normalizedString() { Value = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-5.0" },
                CaseCourt = new CourtPolicyServiceV5.CourtType()
                {
                    OrganizationIdentification = new CourtPolicyServiceV5.IdentificationType()
                    {
                        //IdentificationID = new CourtPolicyServiceV5.@string() { id = "harris:dc", Value = "harris:dc" },
                        //id = "harris:dc"

                        IdentificationID = new CourtPolicyServiceV5.@string() { id = "alameda:crfre", Value = "alameda:crfre" },
                        id = "alameda:crfre"

                        //IdentificationID = new CourtPolicyServiceV5.@string() { id = "CA", Value = "CA" },
                        //id = "CA"

                        //IdentificationID = new CourtPolicyServiceV5.@string() { id = "HIGK", Value = "HIGK" },
                        //id = "HIGK"

                    },
                    //OrganizationIdentification = new CourtPolicyServiceV5.OrganizationType().OrganizationIdentification

                },
                DocumentPostDate = new CourtPolicyServiceV5.DateType(),
                PolicyQueryCriteria = new CourtPolicyServiceV5.PolicyQueryCriteriaType(),
                
            };


            using (new OperationContextScope(CourtPolicyMDEClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", CourtPolicyMDEClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = CourtPolicyMDEClient.GetPolicy(getPolicyRequestType);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        //public ActionResult GetDocument()
        //{


        //    TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
        //    //CourtPolicyMDE.CourtPolicyMDEClient efmFirmServiceClient = new CourtPolicyMDE.CourtPolicyMDEClient();
        //    //tPolicyMDE.CourtPolicyMDEClient

        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //    //EFMFirm.RemovePaymentAccountRequestType getPaymentAccountRequestType = new EFMFirm.RemovePaymentAccountRequestType();
        //    //getPaymentAccountRequestType.PaymentAccountID = Request.Form["PaymentAccountID"];
        //    TylerCourtRecordMDEService.DocumentQueryMessageType serviceInformationQueryMessageType = new TylerCourtRecordMDEService.DocumentQueryMessageType();
        //    TylerCourtRecordMDEService.CourtType CaseCourt = new TylerCourtRecordMDEService.CourtType();
        //    CaseCourt.id = Request.Form["CourtLocationID"];
        //    serviceInformationQueryMessageType.CaseCourt = CaseCourt;
        //    //serviceInformationQueryMessageType.CaseTrackingID =new ; 
        //    //        
        //    //
        //    //    getPolicyRequestMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id= Request.Form["CourtLocationIdentifier"]; ;
        //    //            getPolicyRequestMessageType.GetPolicyRequestMessage.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationIdentifier"]; ;
        //    using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //    {
        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
        //        var resp = efmFirmServiceClient.GetDocument(serviceInformationQueryMessageType);
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //    // efmFirmServiceClient.Open();
        //    // // efmFirmServiceClient.GetUserList
        //    // //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //    //     var resp = "";

        //    // efmFirmServiceClient.Close();
        //    // return Json(resp, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetFeesCalculation()
        {
            FilingReviewMDEV5.FilingReviewMDEClient efmFirmServiceClient = new FilingReviewMDEV5.FilingReviewMDEClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            FilingReviewMDEV5.GetFeesCalculationRequestType feesCalculationQueryMessageType = new FilingReviewMDEV5.GetFeesCalculationRequestType();
            //FilingReviewMDEV5.FilingMessageType[] filingMessageType = new FilingReviewMDEV5.FilingMessageType()[



            //];
            //feesCalculationQueryMessageType.GetFeesCalculationRequestMessage.FilingMessage = filingMessageType;            //FilingReviewMDEV5.FilingMessageType[] filingMessageType = new FilingReviewMDEV5.FilingMessageType()[



            //];
            //feesCalculationQueryMessageType.GetFeesCalculationRequestMessage.FilingMessage = filingMessageType;

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetFeesCalculation(feesCalculationQueryMessageType);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            //var resp = "";
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReviewFiling()
        {
            //GetCaseList();

            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";


            //TylerCourtRecordMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessageType = new TylerCourtRecordMDEService.ReviewFilingRequestMessageType();
            //reviewFilingRequestMessageType.ElectronicServiceInformation 

            TylerFilingReviewMDEService.ReviewFilingRequest reviewFilingRequest = new TylerFilingReviewMDEService.ReviewFilingRequest();

            TylerFilingReviewMDEService.DocumentAttachmentIdentificationType documentAttachmentIdentificationType = new TylerFilingReviewMDEService.DocumentAttachmentIdentificationType();
            //documentAttachmentIdentificationType.DocumentID.Value = "1234";
            TylerFilingReviewMDEService.TaxCategoryType[] taxcat = new TylerFilingReviewMDEService.TaxCategoryType[] { };
            TylerFilingReviewMDEService.AllowanceChargeType allowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType
            {
                AccountingCost = new TylerFilingReviewMDEService.AccountingCostType(),
                AccountingCostCode = new TylerFilingReviewMDEService.AccountingCostCodeType(),
                AllowanceChargeReason = new TylerFilingReviewMDEService.AllowanceChargeReasonType(),
                AllowanceChargeReasonCode = new TylerFilingReviewMDEService.AllowanceChargeReasonCodeType(),
                Amount = new TylerFilingReviewMDEService.AmountType3(),
                BaseAmount = new TylerFilingReviewMDEService.BaseAmountType(),
                ChargeIndicator = new TylerFilingReviewMDEService.ChargeIndicatorType(),
                ID = new TylerFilingReviewMDEService.IDType(),
                MultiplierFactorNumeric = new TylerFilingReviewMDEService.MultiplierFactorNumericType(),
                PaymentMeans = new TylerFilingReviewMDEService.PaymentMeansType[] { },
                PerUnitAmount = new TylerFilingReviewMDEService.PerUnitAmountType(),
                PrepaidIndicator = new TylerFilingReviewMDEService.PrepaidIndicatorType(),
                SequenceNumeric = new TylerFilingReviewMDEService.SequenceNumericType(),
                TaxCategory = new TylerFilingReviewMDEService.TaxCategoryType[] { },
                TaxTotal = new TylerFilingReviewMDEService.TaxTotalType()
            };

            //EFMFirm.GetServiceContactRequestType getServiceContactRequestType = new EFMFirm.GetServiceContactRequestType();
            //getServiceContactRequestType.ServiceContactID = Request.Form["ServiceContactID"];

            TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType()
            {
                CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType()
                {
                    FilingConnectedDocument = new TylerFilingReviewMDEService.DocumentType1[] { },
                    FilingLeadDocument = new TylerFilingReviewMDEService.DocumentType1[] { },
                    Case = new TylerFilingReviewMDEService.CaseType()
                    {
                    },
                    SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = "S0x30" },
                    SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0",
                    //= new TylerFilingReviewMDEService.CaseType() { id = "12", metadata = "meta" , Items = new TylerFilingReviewMDEService.DispositionType[] { }, linkMetadata = "Link Meta",
                    //ActivityIdentification = new TylerFilingReviewMDEService.IdentificationType[] { }, CaseCategoryText = new TylerFilingReviewMDEService.TextType(), CaseTitleText = new TylerFilingReviewMDEService.TextType(),
                    //CaseDocketID = new TylerFilingReviewMDEService.@string(), CaseTrackingID = new TylerFilingReviewMDEService.@string() }
                },


                SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = string.Empty },
                DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType(),

                PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
                {
                    Address = new TylerFilingReviewMDEService.AddressType1(),
                    PayerName = "",
                    AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[] { allowanceCharge },

                    Payment = new TylerFilingReviewMDEService.PaymentType() { ID = new TylerFilingReviewMDEService.IDType() { } }
                }
            };
            TylerFilingReviewMDEService.ElectronicServiceInformationType electronicServiceInformationType = new TylerFilingReviewMDEService.ElectronicServiceInformationType()
            {
                ReceivingMDELocationID = new TylerFilingReviewMDEService.IdentificationType()
                {
                    IdentificationID = new TylerFilingReviewMDEService.@string(),
                    id = string.Empty
                },
                ReceivingMDEProfileCode = "",
                ServiceRecipientID = new TylerFilingReviewMDEService.IdentificationType()
                {
                    IdentificationID = new TylerFilingReviewMDEService.@string(),
                    id = string.Empty
                },
            };

            //electronicServiceInformationType.ServiceRecipientID.IdentificationID.id = "";
            //electronicServiceInformationType.ReceivingMDELocationID.IdentificationID.id = "";

            // electronicServiceInformationType.ReceivingMDEProfileCode

            //TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType()
            //{ CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType(), PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
            //{ AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[] { } } };
            //TylerFilingReviewMDEService.PaymentMessageType paymentMessageType = new TylerFilingReviewMDEService.PaymentMessageType() { };

            //TylerFilingReviewMDEService.AllowanceChargeType allowanceChargeType = new TylerFilingReviewMDEService.AllowanceChargeType();

            //reviewFilingRequest.ReviewFilingRequestMessage = reviewFilingRequestMessage;
            //TylerCourtRecordMDEService.ReviewFilingRequest reviewFiling
            TylerFilingReviewMDEService.CancelFilingMessageType cancelFilingMessageType = new TylerFilingReviewMDEService.CancelFilingMessageType();
            TylerFilingReviewMDEService.TransportEquipmentType transportEquipmentType = new TylerFilingReviewMDEService.TransportEquipmentType();
            //allowanceChargeType.AccountingCost = new TylerFilingReviewMDEService.AccountingCostType() {  Value = 6 };

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.ReviewFiling(reviewFilingRequestMessage);

                //efmFirmServiceClient.CancelFiling(cancelFilingMessageType);
                //efmFirmServiceClient.ReviewFiling(reviewFilingRequest.ReviewFilingRequestMessage);
                //  var resp = efmFirmServiceClient.GetFeesCalculation(getPaymentAccountRequestType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult ReviewFiling1()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; // "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";


            TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType();

            //reviewFilingRequestMessage.CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType();

            reviewFilingRequestMessage.CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType()
            {
                SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = "S0x30" },
                SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0",
                FilingConfidentialityIndicator = new TylerFilingReviewMDEService.boolean() { Value = true },

                FilingLeadDocument = new TylerFilingReviewMDEService.DocumentType1[1],
                //DocumentFiledDate = new TylerFilingReviewMDEService.DateType(),  
                Case = new TylerFilingReviewMDEService.CaseType()
                {
                    CaseTitleText = new TylerFilingReviewMDEService.TextType() { Value = "Case title" },
                    ActivityDescriptionText = new TylerFilingReviewMDEService.TextType() { Value = "Case activity" },
                    ActivityIdentification = new TylerFilingReviewMDEService.IdentificationType[1],
                    ActivityStatus = new TylerFilingReviewMDEService.StatusType()
                    {
                        //StatusDate = new TylerFilingReviewMDEService.DateType(),
                        StatusDescriptionText = new TylerFilingReviewMDEService.TextType[1],
                        StatusText = new TylerFilingReviewMDEService.TextType(),
                    },
                }

                //DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType(),
                //DocumentBinary = new TylerFilingReviewMDEService.BinaryType(),
                //DocumentCategoryText = new TylerFilingReviewMDEService.TextType[1],
                //DocumentDescriptionText = new TylerFilingReviewMDEService.TextType(),


            };

            //reviewFilingRequestMessage.CoreFilingMessage.DocumentFiledDate = "";



            //reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType()
            //{
            //    IdentificationID = new TylerFilingReviewMDEService.@string(),
            //    id = "http://example.com/efsp1"
            //};

            reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
            {
                FeeExceptionReasonCode = string.Empty,
                FeeExceptionSupportingText = string.Empty,
                AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[1],
                Address = new TylerFilingReviewMDEService.AddressType1(),
                PayerName = string.Empty,
                Payment = new TylerFilingReviewMDEService.PaymentType(),
            };

            reviewFilingRequestMessage.DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType();
            reviewFilingRequestMessage.DocumentBinary = new TylerFilingReviewMDEService.BinaryType();
            reviewFilingRequestMessage.DocumentCategoryText = new TylerFilingReviewMDEService.TextType[0];
            reviewFilingRequestMessage.DocumentDescriptionText = new TylerFilingReviewMDEService.TextType();
            reviewFilingRequestMessage.DocumentEffectiveDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentFileControlID = new TylerFilingReviewMDEService.@string();
            reviewFilingRequestMessage.DocumentFiledDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentIdentification = new TylerFilingReviewMDEService.DocumentType().DocumentIdentification;
            reviewFilingRequestMessage.DocumentInformationCutOffDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentPostDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentReceivedDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentSequenceID = new TylerFilingReviewMDEService.@string();
            reviewFilingRequestMessage.DocumentStatus = new TylerFilingReviewMDEService.StatusType();
            reviewFilingRequestMessage.DocumentSubmitter = new TylerFilingReviewMDEService.EntityType();
            reviewFilingRequestMessage.DocumentTitleText = new TylerFilingReviewMDEService.TextType();
            reviewFilingRequestMessage.ElectronicServiceInformation = new TylerFilingReviewMDEService.ElectronicServiceInformationType[0];
            reviewFilingRequestMessage.id = "";
            reviewFilingRequestMessage.Item = new TylerFilingReviewMDEService.LanguageCodeType();
            reviewFilingRequestMessage.linkMetadata = "";
            reviewFilingRequestMessage.metadata = "";
            //reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType();
            //reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType();



            reviewFilingRequestMessage.SendingMDEProfileCode = "";


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0] = new TylerFilingReviewMDEService.DocumentType1();

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentDescriptionText = new TylerFilingReviewMDEService.TextType()
            {
                Value = "Test Filing",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentFileControlID = new TylerFilingReviewMDEService.@string()
            {
                Value = "1",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentSequenceID = new TylerFilingReviewMDEService.@string()
            {
                Value = "0",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata = new TylerFilingReviewMDEService.DocumentMetadataType()
            {
                RegisterActionDescriptionText = new TylerFilingReviewMDEService.TextType() { Value = "8672" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingAttorneyID = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "037BAEC1-F6E8-43ED-A102-B8C5E76748EB" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "IDENTIFICATION" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID = new TylerFilingReviewMDEService.IdentificationType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID[0] = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "Party1" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "REFERENCE" },
            };


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition = new TylerFilingReviewMDEService.DocumentRenditionType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0] = new TylerFilingReviewMDEService.DocumentRenditionType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata = new TylerFilingReviewMDEService.DocumentRenditionMetadataType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment = new TylerFilingReviewMDEService.DocumentAttachmentType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment[0] = new TylerFilingReviewMDEService.DocumentAttachmentType()
            {
                BinaryDescriptionText = new TylerFilingReviewMDEService.TextType { Value = "test.pdf" },
                BinaryFormatStandardName = new TylerFilingReviewMDEService.TextType { Value = "2965" },
                BinaryLocationURI = new TylerFilingReviewMDEService.anyURI { Value = "test.pdf" },
                BinaryCategoryText = new TylerFilingReviewMDEService.TextType { Value = "332" },
                AttachmentSequenceID = new TylerFilingReviewMDEService.@string { Value = "0" },
            };








            ////XmlTextReader reader = new XmlTextReader(@"D:\tyler\xml\ReviewFiling.xml");

            //var filePath = @"D:\tyler\xml\ReviewFiling.xml";

            //// Declare this outside the 'using' block so we can access it later
            ////Config config;

            //XmlRootAttribute xRoot = new XmlRootAttribute();
            //xRoot.ElementName = "ReviewFilingRequestMessage";
            //// xRoot.Namespace = "http://www.cpandl.com";
            //xRoot.IsNullable = true;


            //using (var reader = new StreamReader(filePath))
            //{
            //    reviewFilingRequestMessage = (TylerFilingReviewMDEService.ReviewFilingRequestMessageType)new XmlSerializer(typeof(TylerFilingReviewMDEService.ReviewFilingRequestMessageType), xRoot).Deserialize(reader);
            //}

            //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(reviewFilingRequestMessage);

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.ReviewFiling(reviewFilingRequestMessage);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

            return null;
        }



        public ActionResult ReviewFiling2()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";

            TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType();




            reviewFilingRequestMessage.CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType()
            {
                SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType()
                {
                    IdentificationID = new TylerFilingReviewMDEService.@string() { Value = "https://localhost:44392/" },
                    //id = "S0x30" 
                },
                SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0",

                DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType(),
                DocumentBinary = new TylerFilingReviewMDEService.BinaryType(),
                DocumentCategoryText = new TylerFilingReviewMDEService.TextType[0],
                DocumentDescriptionText = new TylerFilingReviewMDEService.TextType(),
                DocumentEffectiveDate = new TylerFilingReviewMDEService.DateType(),
                DocumentFileControlID = new TylerFilingReviewMDEService.@string(),
                DocumentFiledDate = new TylerFilingReviewMDEService.DateType(),
                DocumentIdentification = new TylerFilingReviewMDEService.DocumentType().DocumentIdentification,
                DocumentInformationCutOffDate = new TylerFilingReviewMDEService.DateType(),
                DocumentPostDate = new TylerFilingReviewMDEService.DateType(),
                DocumentReceivedDate = new TylerFilingReviewMDEService.DateType(),
                DocumentSequenceID = new TylerFilingReviewMDEService.@string(),
                DocumentStatus = new TylerFilingReviewMDEService.StatusType(),
                DocumentSubmitter = new TylerFilingReviewMDEService.EntityType(),
                DocumentTitleText = new TylerFilingReviewMDEService.TextType(),
                ElectronicServiceInformation = new TylerFilingReviewMDEService.ElectronicServiceInformationType[0],


                FilingConfidentialityIndicator = new TylerFilingReviewMDEService.boolean() { Value = true },

                FilingLeadDocument = new TylerFilingReviewMDEService.DocumentType1[1],
                //DocumentFiledDate = new TylerFilingReviewMDEService.DateType(),  
                Case = new TylerFilingReviewMDEService.CaseType()
                {
                    CaseTitleText = new TylerFilingReviewMDEService.TextType() { Value = "Case title" },
                    ActivityDescriptionText = new TylerFilingReviewMDEService.TextType() { Value = "Case activity" },
                    ActivityIdentification = new TylerFilingReviewMDEService.IdentificationType[1],
                    ActivityStatus = new TylerFilingReviewMDEService.StatusType()
                    {
                        //StatusDate = new TylerFilingReviewMDEService.DateType(),
                        StatusDescriptionText = new TylerFilingReviewMDEService.TextType[1],
                        StatusText = new TylerFilingReviewMDEService.TextType(),
                    },
                }

                //DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType(),
                //DocumentBinary = new TylerFilingReviewMDEService.BinaryType(),
                //DocumentCategoryText = new TylerFilingReviewMDEService.TextType[1],
                //DocumentDescriptionText = new TylerFilingReviewMDEService.TextType(),


            };

            //reviewFilingRequestMessage.CoreFilingMessage.DocumentFiledDate = "";



            //reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType()
            //{
            //    IdentificationID = new TylerFilingReviewMDEService.@string(),
            //    id = "http://example.com/efsp1"
            //};

            reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
            {
                FeeExceptionReasonCode = string.Empty,
                FeeExceptionSupportingText = string.Empty,
                AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[1],
                Address = new TylerFilingReviewMDEService.AddressType1(),
                PayerName = string.Empty,
                Payment = new TylerFilingReviewMDEService.PaymentType(),
            };

            reviewFilingRequestMessage.id = "";
            reviewFilingRequestMessage.Item = new TylerFilingReviewMDEService.LanguageCodeType();
            reviewFilingRequestMessage.linkMetadata = "";
            reviewFilingRequestMessage.metadata = "";
            //reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType();
            //reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType();



            reviewFilingRequestMessage.SendingMDEProfileCode = "";


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0] = new TylerFilingReviewMDEService.DocumentType1();

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentDescriptionText = new TylerFilingReviewMDEService.TextType()
            {
                Value = "Test Filing",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentFileControlID = new TylerFilingReviewMDEService.@string()
            {
                Value = "1",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentSequenceID = new TylerFilingReviewMDEService.@string()
            {
                Value = "0",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata = new TylerFilingReviewMDEService.DocumentMetadataType()
            {
                RegisterActionDescriptionText = new TylerFilingReviewMDEService.TextType() { Value = "8672" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingAttorneyID = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "037BAEC1-F6E8-43ED-A102-B8C5E76748EB" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "IDENTIFICATION" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID = new TylerFilingReviewMDEService.IdentificationType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID[0] = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "Party1" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "REFERENCE" },
            };


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition = new TylerFilingReviewMDEService.DocumentRenditionType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0] = new TylerFilingReviewMDEService.DocumentRenditionType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata = new TylerFilingReviewMDEService.DocumentRenditionMetadataType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment = new TylerFilingReviewMDEService.DocumentAttachmentType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment[0] = new TylerFilingReviewMDEService.DocumentAttachmentType()
            {
                BinaryDescriptionText = new TylerFilingReviewMDEService.TextType { Value = "test.pdf" },
                BinaryFormatStandardName = new TylerFilingReviewMDEService.TextType { Value = "2965" },
                BinaryLocationURI = new TylerFilingReviewMDEService.anyURI { Value = "test.pdf" },
                BinaryCategoryText = new TylerFilingReviewMDEService.TextType { Value = "332" },
                AttachmentSequenceID = new TylerFilingReviewMDEService.@string { Value = "0" },
            };






            reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
            {
                FeeExceptionReasonCode = string.Empty,
                FeeExceptionSupportingText = string.Empty,
                AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[1],
                Address = new TylerFilingReviewMDEService.AddressType1(),
                PayerName = string.Empty,
                Payment = new TylerFilingReviewMDEService.PaymentType(),
            };

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0] = new TylerFilingReviewMDEService.AllowanceChargeType();

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].ChargeIndicator = new TylerFilingReviewMDEService.ChargeIndicatorType() { Value = true };

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans = new TylerFilingReviewMDEService.PaymentMeansType[1];

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans[0] = new TylerFilingReviewMDEService.PaymentMeansType();

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans[0].PaymentID = new TylerFilingReviewMDEService.PaymentIDType[1];

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans[0].PaymentID[0] = new TylerFilingReviewMDEService.PaymentIDType();

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans[0].PaymentID[0].Value = "e74adb9f-f3fd-4a18-ad2b-662001ae97fa";

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].PaymentMeans[0].PaymentMeansCode = new TylerFilingReviewMDEService.PaymentMeansCodeType()
            {
                Value = String.Empty
            };

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].TaxTotal = new TylerFilingReviewMDEService.TaxTotalType();

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].TaxTotal.TaxAmount = new TylerFilingReviewMDEService.TaxAmountType()
            {
                Value = Convert.ToDecimal(0.06),
                currencyID = "USD"
            };

            reviewFilingRequestMessage.PaymentMessage.AllowanceCharge[0].Amount = new TylerFilingReviewMDEService.AmountType3()
            {
                currencyID = "USD",
                Value = Convert.ToDecimal(1.00),
                currencyCodeListVersionID = String.Empty,
            };




            ////XmlTextReader reader = new XmlTextReader(@"D:\tyler\xml\ReviewFiling.xml");

            //var filePath = @"D:\tyler\xml\ReviewFiling.xml";

            //// Declare this outside the 'using' block so we can access it later
            ////Config config;

            //XmlRootAttribute xRoot = new XmlRootAttribute();
            //xRoot.ElementName = "ReviewFilingRequestMessage";
            //// xRoot.Namespace = "http://www.cpandl.com";
            //xRoot.IsNullable = true;


            //using (var reader = new StreamReader(filePath))
            //{
            //    reviewFilingRequestMessage = (TylerFilingReviewMDEService.ReviewFilingRequestMessageType)new XmlSerializer(typeof(TylerFilingReviewMDEService.ReviewFilingRequestMessageType), xRoot).Deserialize(reader);
            //}

            //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(reviewFilingRequestMessage);

            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.ReviewFiling(reviewFilingRequestMessage);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

            return null;
        }



        public ActionResult ReviewFiling3()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";

            TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType();


            var json = System.IO.File.ReadAllText(@"D:\tyler\xml\1.json");
            reviewFilingRequestMessage = JsonConvert.DeserializeObject<TylerFilingReviewMDEService.ReviewFilingRequestMessageType>(json);

            reviewFilingRequestMessage.CoreFilingMessage.SendingMDELocationID.IdentificationID = new TylerFilingReviewMDEService.@string { Value = "https://localhost:44392" };
            reviewFilingRequestMessage.CoreFilingMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            reviewFilingRequestMessage.CoreFilingMessage.Case.CaseCategoryText = new TylerFilingReviewMDEService.TextType { Value = "2202" };




            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.ReviewFiling(reviewFilingRequestMessage);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

            return null;
        }



        public ActionResult ReviewFiling4()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com";
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13";

            TylerFilingReviewMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessage = new TylerFilingReviewMDEService.ReviewFilingRequestMessageType();



            #region Input

            reviewFilingRequestMessage.CoreFilingMessage = new TylerFilingReviewMDEService.CoreFilingMessageType()
            {
                FilingConfidentialityIndicator = new TylerFilingReviewMDEService.boolean() { Value = true },

                FilingLeadDocument = new TylerFilingReviewMDEService.DocumentType1[1],

                SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType() { IdentificationID = new TylerFilingReviewMDEService.@string(), id = "S0x30" },
                SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0",

            };

            reviewFilingRequestMessage.CoreFilingMessage.Case = new TylerFilingReviewMDEService.CaseType();



            reviewFilingRequestMessage.DocumentApplicationName = new TylerFilingReviewMDEService.ApplicationNameType();
            reviewFilingRequestMessage.DocumentBinary = new TylerFilingReviewMDEService.BinaryType();
            reviewFilingRequestMessage.DocumentCategoryText = new TylerFilingReviewMDEService.TextType[1];
            reviewFilingRequestMessage.DocumentDescriptionText = new TylerFilingReviewMDEService.TextType();
            reviewFilingRequestMessage.DocumentEffectiveDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentFileControlID = new TylerFilingReviewMDEService.@string();
            reviewFilingRequestMessage.DocumentFiledDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentIdentification = new TylerFilingReviewMDEService.IdentificationType[1];
            reviewFilingRequestMessage.DocumentInformationCutOffDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentPostDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentReceivedDate = new TylerFilingReviewMDEService.DateType();
            reviewFilingRequestMessage.DocumentSequenceID = new TylerFilingReviewMDEService.@string();
            reviewFilingRequestMessage.DocumentStatus = new TylerFilingReviewMDEService.StatusType();
            reviewFilingRequestMessage.DocumentSubmitter = new TylerFilingReviewMDEService.EntityType();
            reviewFilingRequestMessage.DocumentTitleText = new TylerFilingReviewMDEService.TextType();
            reviewFilingRequestMessage.ElectronicServiceInformation = new TylerFilingReviewMDEService.ElectronicServiceInformationType[1];
            reviewFilingRequestMessage.id = "";
            reviewFilingRequestMessage.Item = new TylerFilingReviewMDEService.LanguageCodeType();
            reviewFilingRequestMessage.linkMetadata = "";
            reviewFilingRequestMessage.metadata = "";
            reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType();
            //reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType();
            reviewFilingRequestMessage.SendingMDEProfileCode = "";


            reviewFilingRequestMessage.SendingMDELocationID = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string() { Value = "https:localhost" }
            };


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0] = new TylerFilingReviewMDEService.DocumentType1();

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentDescriptionText = new TylerFilingReviewMDEService.TextType()
            {
                Value = "Test Filing",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentFileControlID = new TylerFilingReviewMDEService.@string()
            {
                Value = "1",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentSequenceID = new TylerFilingReviewMDEService.@string()
            {
                Value = "0",
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata = new TylerFilingReviewMDEService.DocumentMetadataType()
            {
                RegisterActionDescriptionText = new TylerFilingReviewMDEService.TextType() { Value = "8672" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingAttorneyID = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "037BAEC1-F6E8-43ED-A102-B8C5E76748EB" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "IDENTIFICATION" },
            };

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID = new TylerFilingReviewMDEService.IdentificationType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentMetadata.FilingPartyID[0] = new TylerFilingReviewMDEService.IdentificationType()
            {
                IdentificationID = new TylerFilingReviewMDEService.@string { Value = "Party1" },
                IdentificationSourceText = new TylerFilingReviewMDEService.TextType { Value = "REFERENCE" },
            };


            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition = new TylerFilingReviewMDEService.DocumentRenditionType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0] = new TylerFilingReviewMDEService.DocumentRenditionType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata = new TylerFilingReviewMDEService.DocumentRenditionMetadataType();
            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment = new TylerFilingReviewMDEService.DocumentAttachmentType[1];

            reviewFilingRequestMessage.CoreFilingMessage.FilingLeadDocument[0].DocumentRendition[0].DocumentRenditionMetadata.DocumentAttachment[0] = new TylerFilingReviewMDEService.DocumentAttachmentType()
            {
                BinaryDescriptionText = new TylerFilingReviewMDEService.TextType { Value = "test.pdf" },
                BinaryFormatStandardName = new TylerFilingReviewMDEService.TextType { Value = "2965" },
                BinaryLocationURI = new TylerFilingReviewMDEService.anyURI { Value = "test.pdf" },
                BinaryCategoryText = new TylerFilingReviewMDEService.TextType { Value = "332" },
                AttachmentSequenceID = new TylerFilingReviewMDEService.@string { Value = "0" },
            };


            reviewFilingRequestMessage.PaymentMessage = new TylerFilingReviewMDEService.PaymentMessageType()
            {
                FeeExceptionReasonCode = string.Empty,
                FeeExceptionSupportingText = string.Empty,
                AllowanceCharge = new TylerFilingReviewMDEService.AllowanceChargeType[1],
                Address = new TylerFilingReviewMDEService.AddressType1(),
                PayerName = string.Empty,
                Payment = new TylerFilingReviewMDEService.PaymentType(),
            };

            #endregion



            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.ReviewFiling(reviewFilingRequestMessage);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

            return null;
        }


        //        public ActionResult ReviewFiling()
        //        {
        //            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
        //            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //            TylerFilingReviewMDEService.ReviewFilingRequest reviewFilingRequest = new TylerFilingReviewMDEService.ReviewFilingRequest();
        //            //reviewFilingRequest.ReviewFilingRequestMessage.ElectronicServiceInformation.Ele
        //            // reviewFilingRequest.ReviewFilingRequestMessage
        //         //    reviewFilingRequest.ReviewFilingRequestMessage.ElectronicServiceInformation.
        //            TylerFilingReviewCriminalMDEService.ReviewFilingRequestMessageType reviewFilingRequestMessageType = new TylerFilingReviewCriminalMDEService.ReviewFilingRequestMessageType();
        //            reviewFilingRequestMessageType.SendingMDELocationID.IdentificationID.id = Request.Form["SendingMDELocationID"];
        //         //   reviewFilingRequestMessageType.ElectronicServiceInformation.
        ////reviewFilingRequestMessageType.
        ////            reviewFilingRequestMessageType.CoreFilingMessage
        //            //TylerFilingReviewCriminalMDEService.i reviewFilingRequestMessageType = new TylerFilingReviewCriminalMDEService.ReviewFilingRequestMessageType()
        //            //reviewFilingRequestMessageType.ElectronicServiceInformation
        //            //  TylerCourtRecordMDEService.ServiceReceiptMessageType serviceReceiptMessageType = new TylerCourtRecordMDEService.ServiceReceiptMessageType();
        //            //serviceReceiptMessageType.SendingMDELocationID.IdentificationID.id
        //            //reviewFilingRequestMessageType.DocumentIdentification.id
        //            //TylerFilingReviewCriminalMDEService.DocumentIDType documentIDType = new TylerFilingReviewCriminalMDEService.DocumentIDType();
        //            //TylerCourtRecordMDEService.DocumentReferenceType documentReferenceType = new TylerCourtRecordMDEService.DocumentReferenceType();
        //            //documentReferenceType.DocumentTy
        //            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
        //            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
        //            // reviewFilingRequest.ReviewFilingRequestMessage.
        //            //= new TylerFilingReviewMDEService.ElectronicServiceInformationType();
        //            //reviewFilingRequest.ReviewFilingRequestMessage.ElectronicServiceInformation = new TylerFilingReviewMDEService.ElectronicServiceInformationType();

        //            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //            {

        //                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

        //                 //  var resp = efmFirmServiceClient.GetFeesCalculation(getPaymentAccountRequestType);

        //                // return Json(resp, JsonRequestBehavior.AllowGet);
        //            }
        //            return null;
        //            //efmFirmServiceClient.Open();
        //            //// efmFirmServiceClient.GetUserList
        //            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //            //// var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //            //var resp = "";
        //            //efmFirmServiceClient.Close();
        //            //return Json(resp, JsonRequestBehavior.AllowGet);
        //        }

        public ActionResult ReviewFilingAppellateCaseAugmentation()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            TylerFilingReviewMDEService.GetFilingListRequest filingListQueryMessageType = new TylerFilingReviewMDEService.GetFilingListRequest();
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReviewFilingCriminalCaseAugmentation()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            TylerFilingReviewMDEService.GetFilingListRequest filingListQueryMessageType = new TylerFilingReviewMDEService.GetFilingListRequest();
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReviewFilingCitationCaseAugmentation()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            TylerFilingReviewMDEService.GetFilingListRequest filingListQueryMessageType = new TylerFilingReviewMDEService.GetFilingListRequest();
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetFilingList()
        //{
        //    TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //    TylerFilingReviewMDEService.GetFilingListRequest filingListQueryMessageType = new TylerFilingReviewMDEService.GetFilingListRequest();
        //    //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
        //    //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
        //    efmFirmServiceClient.Open();
        //    // efmFirmServiceClient.GetUserList
        //    //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
        //    var resp = "";
        //    efmFirmServiceClient.Close();
        //    return Json(resp, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetFilingStatus()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            TylerFilingReviewMDEService.FilingStatusResponseMessageType filingStatusResponseMessageType = new TylerFilingReviewMDEService.FilingStatusResponseMessageType();
            filingStatusResponseMessageType.CaseCourt.id = "";

            filingStatusResponseMessageType.Case.CaseTrackingID.id = "";
            //filingStatusResponseMessageType.i
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            //     filingStatusQueryMessageType.FilingStatusQueryMessage.SendingMDELocationID = "";

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelFiling()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            TylerFilingReviewMDEService.CancelFilingMessageType filingStatusResponseMessageType = new TylerFilingReviewMDEService.CancelFilingMessageType();
            filingStatusResponseMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            filingStatusResponseMessageType.DocumentIdentification.IdentificationID.id = Request.Form["FilingID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.CancelFiling(filingStatusResponseMessageType);
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotifyFilingReviewComplete()
        {
            TylerFilingReviewMDEService.FilingReviewMDEPortClient efmFirmServiceClient = new TylerFilingReviewMDEService.FilingReviewMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerFilingReviewMDEService.CancelFilingMessageType filingStatusResponseMessageType = new TylerFilingReviewMDEService.CancelFilingMessageType();
            filingStatusResponseMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            filingStatusResponseMessageType.DocumentIdentification.IdentificationID.id = Request.Form["FilingID"];
            //filingStatusResponseMessageType.i
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            //     filingStatusQueryMessageType.FilingStatusQueryMessage.SendingMDELocationID = "";

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.GetCase(getPaymentAccountRequestType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotifyCourtDate()
        {
            FilingAssemblyMDEService.FilingAssemblyMDEClient efmFirmServiceClient = new FilingAssemblyMDEService.FilingAssemblyMDEClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //    efmFirmServiceClient.ClientCredentials.HttpDigest.ClientCredential.UserName = Request.Form["UserName"];
            //    efmFirmServiceClient.ClientCredentials.HttpDigest.ClientCredential.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            FilingAssemblyMDEService.NotifyCourtDateMessageType notifyCourtDateMessageType = new FilingAssemblyMDEService.NotifyCourtDateMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //// FilingAssemblyMDEService.NotifyCourtDateA
            //  notifyCourtDateMessageType.Case.ActivityDate.ObjectAugmentati = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // filingStatusResponseMessageType.DocumentIdentification.IdentificationID.id = Request.Form["FilingID"];
            //filingStatusResponseMessageType.i
            //           filingListQueryMessageType.FilingListQueryMessage.SendingMDELocationID.IdentificationID = Request.Form["id"];
            //         filingListQueryMessageType.SendingMDELocationID = filingListQueryMessageType.QuerySubmitter.id;
            //     filingStatusQueryMessageType.FilingStatusQueryMessage.SendingMDELocationID = "";

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            // var resp = efmFirmServiceClient.NotifyCourtDate(notifyCourtDateMessageType);
            var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCaseListV5()
        {

            CourtRecordServiceV5.CourtRecordMDEClient courtRecordMDEClient = new CourtRecordServiceV5.CourtRecordMDEClient();

            courtRecordMDEClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            courtRecordMDEClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; 
            courtRecordMDEClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8";


            using (new OperationContextScope(courtRecordMDEClient.InnerChannel))
            {
          
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", courtRecordMDEClient.ClientCredentials.UserName);

                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
      
                var resp = courtRecordMDEClient.GetCaseList(new CourtRecordServiceV5.GetCaseListRequestType()
                {
                    GetCaseListRequestMessage = new CourtRecordServiceV5.GetCaseListRequestMessageType()
                    {
                        CaseCourt = new CourtRecordServiceV5.CourtType()
                        {
                            OrganizationIdentification = new CourtRecordServiceV5.IdentificationType()
                            {
                                IdentificationID = new CourtRecordServiceV5.@string() { Value = "alameda:crfre" }
                            }
                        },
                        CaseListQueryCriteria = new CourtRecordServiceV5.CaseListQueryCriteriaType()
                        {
                            ObjectAugmentationPoint = new object[]
                            {
                                new CourtRecordServiceV5.CaseListQueryCriteriaAugmentationType()
                                {
                                    CaseNumberText = new CourtRecordServiceV5.TextType() { Value= "227702" }
                                }
                            }
                        }
                    }
                });

                return Json(resp, JsonRequestBehavior.AllowGet);
            }

            return null;


        }


        //public ActionResult GetCaseListV5()
        //{
        //    #region CODE FOR CODE LEVEL CONFIGURATION
        //    //EndpointAddress endpointAddress = new EndpointAddress("https://california-efm-stage.tylertech.cloud/efm/v5/courtrecordservice.svc");
        //    //EndpointAddress endpointAddress = new EndpointAddress("https://california-efm-stage.tylertech.cloud/EFM/Schema/ecf5.0/wsi/schema/CourtRecordMDE.wsdl");


        //    //Uri epUri = new Uri(endpointAddress.Uri.ToString());
        //    //CustomBinding binding = new CustomBinding();
        //    //Binding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.TransportWithMessageCredential);


        //    //SecurityBindingElement sbe = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
        //    //sbe.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
        //    //sbe.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
        //    //sbe.SecurityHeaderLayout = SecurityHeaderLayout.Strict;
        //    //sbe.IncludeTimestamp = false;
        //    //sbe.SetKeyDerivation(true);
        //    //sbe.KeyEntropyMode = System.ServiceModel.Security.SecurityKeyEntropyMode.ServerEntropy;

        //    //binding.Elements.Add(sbe);
        //    //binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11, System.Text.Encoding.Default));
        //    //binding.Elements.Add(new AsymmetricSecurityBindingElement());
        //    //binding.Elements.Add(new HttpsTransportBindingElement());

        //    //TylerCourtRecordMDEV5.TylerCourtRecordMDEClient efmFirmServiceClient = new TylerCourtRecordMDEV5.TylerCourtRecordMDEClient(binding, endpointAddress);

        //    //CourtRecordServiceV5.CourtRecordMDEClient courtRecordMDEClient = new CourtRecordServiceV5.CourtRecordMDEClient("CourtRecordServiceSOAPBinding", "https://california-efm-stage.tylertech.cloud/efm/v5/courtrecordservice.svc");
        //    CourtRecordServiceV5.CourtRecordMDEClient courtRecordMDEClient = new CourtRecordServiceV5.CourtRecordMDEClient();

        //    #endregion



        //    //TylerCourtRecordMDEV5.TylerCourtRecordMDEClient efmFirmServiceClient = new TylerCourtRecordMDEV5.TylerCourtRecordMDEClient();


        //    //  CourtPolicyMDE.Re

        //    //TylerCourtRecordMDEV5.TylerCourtRecordMDEClient efmFirmServiceClient = new TylerCourtRecordMDEV5.TylerCourtRecordMDEClient();
        //    //FilingReviewMDEV5.FilingReviewMDEClient efmFirmServiceClient = new FilingReviewMDEV5.FilingReviewMDEClient();

        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    courtRecordMDEClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //    courtRecordMDEClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
        //    courtRecordMDEClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8"; //Request.Form["Password"];

        //    //caseListQuery.CaseCourt.CourtName.Value = "";


        //    //EFMFirm. getAttorneyRequestType = new EFMFirm.GetAttorneyRequestType();
        //    //getAttorneyRequestType.AttorneyID = Request.Form["AttorneyID"];

        //    //TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
        //    //TylerCourtRecordMDEV5.DocumentType documentType = new TylerCourtRecordMDEV5.DocumentType();
        //    //TylerCourtRecordMDEV5.GetCaseListRequestMessageType CaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();
        //    //TylerCourtRecordMDEV5.CaseType CaseType = new TylerCourtRecordMDEV5.CaseType();
        //    //TylerCourtRecordMDEV5.CaseListQueryCriteriaType CaseListQueryCriteriaType = new TylerCourtRecordMDEV5.CaseListQueryCriteriaType();
        //    //TylerCourtRecordMDEV5.GetServiceCaseListMessageType ServiceCaseListMessageType = new TylerCourtRecordMDEV5.GetServiceCaseListMessageType();
        //    //TylerCourtRecordMDEV5.GetServiceCaseListRequestType ServiceCaseListRequestType = new TylerCourtRecordMDEV5.GetServiceCaseListRequestType();
        //    //TylerCourtRecordMDEV5.GetServiceCaseListRequest ServiceCaseListRequest = new TylerCourtRecordMDEV5.GetServiceCaseListRequest();

        //    //TylerCourtRecordMDEV5.GetCaseListRequestMessageType getCaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();



        //    //TylerCourtRecordMDEService.CourtType court = new TylerCourtRecordMDEService.CourtType();
        //    //court.id = Request.Form["CourtLocationID"]; ;
        //    //caseListQueryMessageType.CaseCourt = court;
        //    //caseListQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType() {
        //    //    id = Request.Form["CourtLocationID"]
        //    //};
        //    //caseListQueryMessageType.CaseCourt.OrganizationLocation.id = Request.Form["CourtLocationID"];
        //    //caseListQueryMessageType.CaseDocketID.id =  Request.Form["CaseDocketID"];
        //    //caseListQueryMessageType.CaseTrackingID.id = Request.Form["CaseTrackingID"];

        //    //caseListQueryMessageType.CaseListQueryCaseParticipant = new TylerCourtRecordMDEService.CaseParticipantType()
        //    //{
        //    //    Item
        //    //};
        //    // TylerCourtRecordMDEService;
        //    //Request.Form["CourtLocationID"];
        //    //caseListQueryMessageType.CaseListQueryCaseParticipant. = Request.Form["CourtLocationID"];
        //    //caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
        //    //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
        //    //courtType.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
        //    //caseListQueryMessageType.CaseCourt = courtType;
        //    using (new OperationContextScope(courtRecordMDEClient.InnerChannel))
        //    {
        //        //var msgHeaderCintent = MessageHeader.CreateHeader("Content-Type", "multipart/related; type='application/xop+xml'", efmFirmServiceClient);
        //        //var msgHeaderCintent = MessageHeader.CreateHeader("Content-Type", "application/xml", efmFirmServiceClient);
        //        //var msgHeaderCintent = MessageHeader.CreateHeader("Content-Type", "application/xop+xml", efmFirmServiceClient);
        //        //var msgHeaderCintent = MessageHeader.CreateHeader("Content-Type", "multipart/related; type='application/xop+xml'", efmFirmServiceClient);

        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", courtRecordMDEClient.ClientCredentials.UserName);

        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
        //        //OperationContext.Current.OutgoingMessageHeaders.Add(msgHeaderCintent);
        //        var resp = courtRecordMDEClient.GetCaseList(new CourtRecordServiceV5.GetCaseListRequestType()
        //        {
        //            GetCaseListRequestMessage = new CourtRecordServiceV5.GetCaseListRequestMessageType()
        //            {
        //                CaseCourt = new CourtRecordServiceV5.CourtType()
        //                {
        //                    OrganizationIdentification = new CourtRecordServiceV5.IdentificationType()
        //                    {
        //                        IdentificationID = new CourtRecordServiceV5.@string() { Value = "alameda:crfre" }
        //                    }
        //                },
        //                CaseListQueryCriteria = new CourtRecordServiceV5.CaseListQueryCriteriaType()
        //                {
        //                    ObjectAugmentationPoint = new object[]
        //                    {
        //                        new CourtRecordServiceV5.CaseListQueryCriteriaAugmentationType()
        //                        {
        //                            CaseNumberText = new CourtRecordServiceV5.TextType() { Value= "227702" }
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }

        //    return null;


        //    //efmFirmServiceClient.Open();
        //    //// efmFirmServiceClient.GetUserList
        //    ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
        //    ////var resp = "";
        //    //efmFirmServiceClient.Close();
        //    //return Json(resp, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetCaseListV5()
        //{
        //    TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    //efmFirmServiceClient.ClientCredentials.UserName.UserName = objuserlogin.UserName;
        //    //efmFirmServiceClient.ClientCredentials.UserName.Password = objuserlogin.Password;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; //Request.Form["Password"];

        //    //if (objuserlogin.UserName == null || objuserlogin.Password == null)
        //    //{
        //    //    ViewBag.Status = "INCORRECT UserNAme and Password";
        //    //}
        //    //else
        //    //{
        //    List<Root> ObjEmp = new List<Root>();

        //    //TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessage = new TylerCourtRecordMDEService.CaseListQueryMessageType();

        //    //TylerCourtRecordMDEService.IdentificationType identificationType1 = new TylerCourtRecordMDEService.IdentificationType();
        //    //caseListQueryMessage.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType
        //    //{
        //    //    IdentificationID = new TylerCourtRecordMDEService.@string()
        //    //};
        //    //caseListQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";
        //    //caseListQueryMessage.QuerySubmitter =new TylerCourtRecordMDEService.EntityType
        //    //{

        //    //};
        //    //caseListQueryMessage.CaseCourt = new TylerCourtRecordMDEService.CourtType();
        //    //caseListQueryMessage.CaseCourt.OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType
        //    //{
        //    //    IdentificationID = new TylerCourtRecordMDEService.@string()
        //    //};
        //    //caseListQueryMessage.CaseListQueryCase = new TylerCourtRecordMDEService.CaseType[4];

        //    //// new code
        //    ///
        //    TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();

        //    string casenumber = "227702";

        //    caseListQueryMessageType.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
        //    {
        //        IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
        //    };

        //    caseListQueryMessageType.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

        //    caseListQueryMessageType.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
        //    {
        //        ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
        //        Item = new TylerCourtRecordMDEService.PersonType2()
        //    };

        //    caseListQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType()
        //    {
        //        OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
        //        {
        //            IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "alameda:crfre" }
        //        }
        //    };


        //    caseListQueryMessageType.CaseListQueryCase = new TylerCourtRecordMDEService.CaseType[] {
        //    new TylerCourtRecordMDEService.CaseType(){  CaseDocketID = new TylerCourtRecordMDEService.@string(){ Value = casenumber} }
        //    };


        //    efmFirmServiceClient.Open();

        //    //caseListQueryMessage.CaseListQueryCase[0].CaseTitleText = new TylerCourtRecordMDEService.TextType();
        //    //caseListQueryMessage.CaseListQueryCase[1].CaseCategoryText = new TylerCourtRecordMDEService.TextType();
        //    //caseListQueryMessage.CaseListQueryCase[2].CaseTrackingID = new TylerCourtRecordMDEService.@string();
        //    //caseListQueryMessage.CaseListQueryCase[3].CaseDocketID = new TylerCourtRecordMDEService.@string();

        //    //TylerCourtRecordMDEService.QueryMessageType queryMessageType = new TylerCourtRecordMDEService.QueryMessageType() { SendingMDELocationID="";}
        //    ////queryMessageType.SendingMDELocationID.id = "";


        //    //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();
        //    //TylerCourtRecordMDEService.@string @string1 = new TylerCourtRecordMDEService.@string();
        //    //@string1.id = "harris";
        //    //identificationType.IdentificationID = @string1;

        //    //TylerCourtRecordMDEService.EntityType entityType = new TylerCourtRecordMDEService.EntityType();
        //    //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
        //    //courtType.OrganizationIdentification = identificationType;
        //    //TylerCourtRecordMDEService.OrganizationType organizationType = new TylerCourtRecordMDEService.OrganizationType();
        //    //organizationType.OrganizationIdentification.IdentificationID.id = "harris:dc";

        //    //TylerCourtRecordMDEService.CaseType[] casetype = new TylerCourtRecordMDEService.CaseType[4];

        //    //TylerCourtRecordMDEService.CaseType caseType1 = new TylerCourtRecordMDEService.CaseType();


        //    //casetype[0].CaseTitleText = null;
        //    //casetype[1].CaseCategoryText = null;
        //    //casetype[2].CaseTrackingID= null;
        //    //casetype[3].CaseDocketID.Value= "2011-CV-0044";

        //    //caseListQueryMessage.SendingMDELocationID sendingMDELocationID = new   caseListQueryMessage.SendingMDELocationID();
        //    //caseListQueryMessage.SendingMDEProfileCode sendingMDEProfile = new caseListQueryMessage.SendingMDEProfileCode(); 
        //    //caseListQueryMessage.QuerySubmitter querysubmit=new caseListQueryMessage.QuerySubmitter();

        //    //caseListQueryMessage.CaseCourt courtType=new caseListQueryMessage.CaseCourt();
        //    //caseListQueryMessage.CaseListQueryCase caseTypes = new caseListQueryMessage.CaseListQueryCase() ;
        //    try
        //    {
        //        using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //        {

        //            var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //            OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

        //            var resp = efmFirmServiceClient.GetCaseList(caseListQueryMessageType);


        //            // foreach(var @case in resp.Case)
        //            //{
        //            //   var CaseCategoryText =  @case.CaseCategoryText.Value;

        //            //}

        //            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);

        //            //System.IO.File.WriteAllText("D:\\Test.html", jsonString);

        //            Root myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonString);

        //            ////var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CaseSample>(jsonString);

        //            efmFirmServiceClient.Close();

        //            //if (myDeserializedClass.Error[0].ErrorCode.Value == "0")
        //            //{
        //            //    foreach (var data1 in myDeserializedClass.Case)
        //            //    {
        //            //        string IdentificationID = data1.CaseAugmentation.CaseCourt.OrganizationIdentification.IdentificationID.Value;
        //            //        string CaseTypeText = data1.CaseAugmentation1.CaseTypeText.Value;
        //            //    }
        //            //}

        //            //if (resp.Error?.Length > 0 && resp.Error[0].ErrorCode.Value == "0")
        //            //{
        //            //    foreach (var @case in resp.Case)
        //            //    {


        //            //    }

        //            //}
        //            ViewBag.Status = "";
        //            return View(myDeserializedClass.Case);
        //            //return Json(resp, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Status = "Enter correct username and Password";
        //    }

        //    //}          


        //    return View("Index");

        //    //TylerCourtRecordMDEService.TextType textType = new TylerCourtRecordMDEService.TextType();
        //    //textType.id = "abc";

        //    //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
        //    //courtType.CourtName = textType;
        //    ////courtType.CourtName.Value = "abc";
        //    //caseListQueryMessage.CaseCourt = courtType;

        //    //TylerCourtRecordMDEService.@string @string = new TylerCourtRecordMDEService.@string();



        //    //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();


        //    //caseListQueryMessage.SendingMDELocationID = identificationType;

        //    //caseListQueryMessage.SendingMDELocationID
        //    //caseListQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";
        //    //caseListQueryMessage.QuerySubmitter.id = "";
        //    //caseListQueryMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";


        //    //TylerCourtRecordMDEV5.GetCaseListRequestMessageType getCaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();

        //    //getCaseListRequestMessageType.CaseCourt.OrganizationIdentification.id = "harris:dc";

        //    //  TylerCourtRecordMDEV5.GetServiceCaseListRequestType getServiceCaseListRequestType  = new TylerCourtRecordMDEV5.GetServiceCaseListRequestType();
        //    //getServiceCaseListRequestType.GetServiceCaseListMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";

        //    // //caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = "harris:dc";
        //    // // caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.Value = "harris:dc";

        //    //caseListQueryMessageType.

        //    //var list = caseListQueryMessageType.CaseListQueryCase;
        //    // foreach(var itm in list)
        //    //     {
        //    //     itm.CaseTitleText = null;
        //    //     itm.CaseCategoryText = null;
        //    //     itm.CaseTrackingID = null;
        //    //     itm.CaseDocketID.id = "2011-CV-0044";


        //    // }
        //    // //request xml write

        //}



        public ActionResult GetCaseList(String UserName, String Password, String CaseNumber)
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //"zahoorahmed481+26@gmail.com"; 
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; // "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; 

            List<Root> ObjEmp = new List<Root>();


            TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();


            string casenumber = CaseNumber; //"227702";

            caseListQueryMessageType.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
            {
                IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
            };

            caseListQueryMessageType.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            caseListQueryMessageType.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
            {
                ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
                Item = new TylerCourtRecordMDEService.PersonType2()
            };

            caseListQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType()
            {
                OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
                {
                    IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "alameda:crfre" }
                }
            };


            caseListQueryMessageType.CaseListQueryCase = new TylerCourtRecordMDEService.CaseType[] {
            new TylerCourtRecordMDEService.CaseType(){  CaseDocketID = new TylerCourtRecordMDEService.@string(){ Value = casenumber} }
            };


            efmFirmServiceClient.Open();


            try
            {
                using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
                {

                    var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                    OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                    var resp = efmFirmServiceClient.GetCaseList(caseListQueryMessageType);


                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);


                    Root myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonString);


                    efmFirmServiceClient.Close();


                    //return Json(myDeserializedClass.Case, JsonRequestBehavior.AllowGet);
                    return Json(resp, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetCase(String UserName, String Password, String CaseTrackingID)
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = UserName; //"zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Password; //"ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; //Request.Form["Password"];



            TylerCourtRecordMDEService.CaseQueryMessageType caseQueryMessage = new TylerCourtRecordMDEService.CaseQueryMessageType();

            caseQueryMessage.CaseQueryCriteria = new TylerCourtRecordMDEService.CaseQueryCriteriaType()
            {
                IncludeCalendarEventIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeDocketEntryIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeParticipantsIndicator = new TylerCourtRecordMDEService.boolean() { Value = true },
                DocketEntryTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" },
                CalendarEventTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" }
            };

            caseQueryMessage.CaseTrackingID = new TylerCourtRecordMDEService.@string() { Value = CaseTrackingID };//Request.QueryString["id"] };


            caseQueryMessage.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
            {
                IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
            };

            caseQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            caseQueryMessage.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
            {
                ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
                Item = new TylerCourtRecordMDEService.PersonType2()
                
            };

            caseQueryMessage.CaseCourt = new TylerCourtRecordMDEService.CourtType()
            {
                OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
                {
                    IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "alameda:crfre" }
                }
            };

            efmFirmServiceClient.Open();




            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.GetCase(caseQueryMessage);

                CaseDemo model = new CaseDemo();

                if (resp.Error?.Length > 0 && resp.Error[0].ErrorCode.Value == "0")
                {
                    model.CaseTitleText = resp.Case.CaseTitleText.Value;
                    model.CaseCategoryText = resp.Case.CaseCategoryText.Value;
                    model.CaseTrackingID = resp.Case.CaseTrackingID.Value;
                    model.CaseDocketID = resp.Case.CaseDocketID.Value;

                    var civilCaseType = (TylerCourtRecordMDEService.CriminalCaseType)resp.Case;
                    foreach (var caseParticipant in civilCaseType.CaseAugmentation1.CaseParticipant)
                    {
                        if (caseParticipant.Item is TylerCourtRecordMDEService.PersonType2)
                        {
                            var person = (TylerCourtRecordMDEService.PersonType2)caseParticipant.Item;

                            model.PersonGivenName = person.PersonName.PersonGivenName.Value;
                            model.PersonMiddleName = person.PersonName.PersonMiddleName.Value;
                            model.PersonSurName = person.PersonName.PersonSurName.Value;
                        }

                    }

                    foreach (var CaseJudge in civilCaseType.CaseAugmentation.CaseJudge)
                    {
                        if (CaseJudge.JudicialOfficialBarMembership is TylerCourtRecordMDEService.JudicialOfficialBarMembershipType)
                        {
                            model.memberShip = CaseJudge.JudicialOfficialBarMembership.JudicialOfficialBarIdentification.IdentificationID.Value;
                        }
                    }

                }


                efmFirmServiceClient.Close();

                return Json(model, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult GetCaseList5()
        {
            //  CourtPolicyMDE.Re

            TylerCourtRecordMDEV5.TylerCourtRecordMDEClient efmFirmServiceClient = new TylerCourtRecordMDEV5.TylerCourtRecordMDEClient();
            //FilingReviewMDEV5.FilingReviewMDEClient efmFirmServiceClient = new FilingReviewMDEV5.FilingReviewMDEClient();

            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = "27d0dff1-71d2-40b2-87a2-1292a153dac8"; //Request.Form["Password"];

            //caseListQuery.CaseCourt.CourtName.Value = "";


            //EFMFirm. getAttorneyRequestType = new EFMFirm.GetAttorneyRequestType();
            //getAttorneyRequestType.AttorneyID = Request.Form["AttorneyID"];

            //TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            TylerCourtRecordMDEV5.GetCaseListRequestMessageType CaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();
            TylerCourtRecordMDEV5.CaseType CaseType = new TylerCourtRecordMDEV5.CaseType();
            TylerCourtRecordMDEV5.CaseListQueryCriteriaType CaseListQueryCriteriaType = new TylerCourtRecordMDEV5.CaseListQueryCriteriaType();
            TylerCourtRecordMDEV5.GetServiceCaseListMessageType ServiceCaseListMessageType = new TylerCourtRecordMDEV5.GetServiceCaseListMessageType();
            TylerCourtRecordMDEV5.GetServiceCaseListRequestType ServiceCaseListRequestType = new TylerCourtRecordMDEV5.GetServiceCaseListRequestType();
            TylerCourtRecordMDEV5.GetServiceCaseListRequest ServiceCaseListRequest = new TylerCourtRecordMDEV5.GetServiceCaseListRequest();

            TylerCourtRecordMDEV5.GetCaseListRequestMessageType getCaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();


            //TylerCourtRecordMDEService.CourtType court = new TylerCourtRecordMDEService.CourtType();
            //court.id = Request.Form["CourtLocationID"]; ;
            //caseListQueryMessageType.CaseCourt = court;
            //caseListQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType() {
            //    id = Request.Form["CourtLocationID"]
            //};
            //caseListQueryMessageType.CaseCourt.OrganizationLocation.id = Request.Form["CourtLocationID"];
            //caseListQueryMessageType.CaseDocketID.id =  Request.Form["CaseDocketID"];
            //caseListQueryMessageType.CaseTrackingID.id = Request.Form["CaseTrackingID"];

            //caseListQueryMessageType.CaseListQueryCaseParticipant = new TylerCourtRecordMDEService.CaseParticipantType()
            //{
            //    Item
            //};
            // TylerCourtRecordMDEService;
            //Request.Form["CourtLocationID"];
            //caseListQueryMessageType.CaseListQueryCaseParticipant. = Request.Form["CourtLocationID"];
            //caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
            //courtType.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //caseListQueryMessageType.CaseCourt = courtType;
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.GetServiceCaseList(ServiceCaseListRequestType);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            ////var resp = "";
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetCase()
        //{
        //    TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
        //    // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
        //    efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
        //    efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
        //    efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
        //    //   UserName
        //    //Password
        //    //CourtLocationID
        //    //FilingID
        //    TylerCourtRecordMDEService.CaseQueryMessageType caseQueryMessageType = new TylerCourtRecordMDEService.CaseQueryMessageType();
        //    //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
        //    // caseQueryMessageType.CaseTrackingID = 
        //    //string V = "";
        //    //caseQueryMessageType.CaseTrackingID = new StringBuilder("").ToString();
        //    //caseQueryMessageType.CaseCourt = new StringBuilder("").ToString();
        //    //string caseid = Request.Form["CaseID"];
        //    //caseQueryMessageType.CaseTrackingID = new TylerCourtRecordMDEService.@string()
        //    //{
        //    //    id = caseid
        //    //};
        //    //caseQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType()
        //    //{
        //    //    id = Request.Form["CourtLocationID"],
        //    //};
        //    // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
        //    //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];



        //    TylerCourtRecordMDEService.CaseListQueryMessageType caseQueryMessageType1 = new TylerCourtRecordMDEService.CaseListQueryMessageType();
        //    caseQueryMessageType1.CaseCourt.OrganizationIdentification.IdentificationID.id = "slo:cv";

        //    // caseQueryMessageType1.CaseQueryCriteria.

        //    ///caselistrequest:GetCaseListRequestMessage/caselistrequest:CaseListQueryCriteria/tylercommon:CaseListQueryCriteriaAugmentation/j:CaseNumberText

        //    using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
        //    {

        //        var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
        //        OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

        //        var resp = efmFirmServiceClient.GetCaseList(caseQueryMessageType1);


        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;

        //    //efmFirmServiceClient.Open();
        //    //// efmFirmServiceClient.GetUserList
        //    ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
        //    //var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
        //    ////var resp = "";
        //    //efmFirmServiceClient.Close();
        //    //return Json(resp, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ServeFiling()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RequestCourtDate()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            ////var resp = "";
            //efmFirmServiceClient.Close();
            // return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReserveCourtDate()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            //            Estimated Duration

            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            //efmFirmServiceClient.Open();
            //// efmFirmServiceClient.GetUserList
            ////  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            //var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            ////var resp = "";
            //efmFirmServiceClient.Close();
            //return Json(resp, JsonRequestBehavior.AllowGet);
            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {

                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);

                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult GetFilingService()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBatchList()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBatchDetail()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult NotifyBatchComplete()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }




       


        public ActionResult GetServiceInformationHistory()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReturnDate()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            // efmFirmServiceClient.ClientCredentials.ClientCertificate.SetCertificate(x509Certificate2);
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = Request.Form["Password"];
            //   UserName
            //Password
            //CourtLocationID
            //FilingID
            TylerCourtRecordMDEService.CaseListQueryMessageType notifyCourtDateMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();
            //notifyCourtDateMessageType.DocumentIdentification = Request.Form["CourtLocationID"];
            notifyCourtDateMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            // notifyCourtDateMessageType.CaseListQueryCase.Crite //.OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];
            //   notifyCourtDateMessageType.CaseListQueryCriteria. // .OrganizationIdentification.IdentificationID.id = Request.Form["CourtLocationID"];

            efmFirmServiceClient.Open();
            // efmFirmServiceClient.GetUserList
            //  efmFirmServiceClient.ClientCredentials.ServiceCertificate.DefaultCertificate = x509Certificate2;
            var resp = efmFirmServiceClient.GetCaseList(notifyCourtDateMessageType);
            //var resp = "";
            efmFirmServiceClient.Close();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }



        public ActionResult demotest()
        {

            var _clientHandler = new HttpClientHandler();
            _clientHandler.ClientCertificates.Add(x509Certificate2);
            _clientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            HttpClient client = new HttpClient(_clientHandler);
            client.BaseAddress = new Uri("https://california-stage.tylerhost.net/CodeService/codes/location/");
            HttpResponseMessage response = client.GetAsync($"https://california-stage.tylerhost.net/CodeService/codes/location/").Result;
            //var client = new RestClient("https://california-stage.tylerhost.net/CodeService/codes/location/");

            //client.ClientCertificates = x509Certificate2;
            //client.Proxy = new WebProxy();
            //var restrequest = new RestRequest(Method.POST);
            //restrequest.AddHeader("Cache-Control", "no-cache");
            //restrequest.AddHeader("Accept", "application/json");
            //restrequest.AddHeader("Content-Type", "application/json");
            //restrequest.AddParameter("here i have added the request parameter", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(restrequest);
            var resp = response;
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult demotestlocation()
        {


            var cert = new X509Certificate2(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\AltrueCA.pfx", "MyP@ssword1");
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile("https://california-stage.tylerhost.net/CodeService/codes/location/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations.zip");
                resp = "{'file_path':'" + "AltrueCA/mylocations.zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CourtLocations()
        {

            var filename = "CourtLocations";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;


            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                //client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/location/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/location/", @"D:\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CodeVersions()
        {

            var filename = "CodeVersions";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/versions/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ErrorCodes()
        {

            var filename = "ErrorCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/error/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CountryCodes()
        {

            var filename = "CountryCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/country/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StateCodes()
        {

            var filename = "StateCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/state/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FilingStatusCodes()
        {

            var filename = "FilingStatusCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/filingstatus/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DataFieldConfigurationCodes()
        {

            var filename = "DataFieldConfigurationCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/datafield/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CaseCategoryCodes()
        {

            var filename = "CaseCategoryCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/casecategory/slo:crdc/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CaseTypeCodes()
        {

            var filename = "CaseTypeCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/casetype/slo:cr/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CaseSubTypeCodes()
        {

            var filename = "CaseSubTypeCodes";
            var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword);
            var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
            ContentInfo info = new ContentInfo(data);
            SignedCms cms = new SignedCms(info, false);
            CmsSigner signer = new CmsSigner(cert);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            var b64 = Convert.ToBase64String(signed);
            //  File.WriteAllText(b64, "apikey.txt");
            var resp = "";

            using (WebClient client1 = new WebClient())
            {
                client1.Headers["tyl-efm-api"] = b64;
                client1.DownloadFile(AltrusBASEURL + "/CodeService/codes/filingstatus/", @"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                resp = "{'file_path':'" + "AltrueCA/" + filename + ".zip" + "'}";
                // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                //  XmlDocument doc = new XmlDocument();
                //  doc.LoadXml(xml);
                //resp = JsonConvert.SerializeXmlNode(doc);

            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConfigurationAPIS()
        {
            var resp = "{'status':'faild'}";
            if (Request.Form["FileName"] != null && Request.Form["ApiUrl"] != null)
            {



                var filename = Request.Form["FileName"];

                if (Request.Form["County"] != null && Request.Form["Court"] != null)
                {
                    filename = Request.Form["FileName"] + "_" + Request.Form["County"] + "_" + Request.Form["Court"];
                }


                var apiUrl = Request.Form["ApiUrl"];
                var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword, X509KeyStorageFlags.MachineKeySet);
                var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
                ContentInfo info = new ContentInfo(data);
                SignedCms cms = new SignedCms(info, false);
                CmsSigner signer = new CmsSigner(cert);
                signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

                cms.ComputeSignature(signer);
                var signed = cms.Encode();
                var b64 = Convert.ToBase64String(signed);
                //  File.WriteAllText(b64, "apikey.txt");


                using (WebClient client1 = new WebClient())
                {
                    client1.Headers["tyl-efm-api"] = b64;
                    client1.DownloadFile(AltrusBASEURL + "/" + apiUrl, @"C:\inetpub\wwwroot\tyler_api_live\downloaded_code_zip_files\" + filename + ".zip");
                    resp = "{'zip_file_path':'" + "downloaded_code_zip_files/" + filename + ".zip" + "'}";
                    // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                    //  XmlDocument doc = new XmlDocument();
                    //  doc.LoadXml(xml);
                    //resp = JsonConvert.SerializeXmlNode(doc);

                }
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConfigurationAPISGET()
        {
            var resp = "{'status':'faild'}";
            if (Request.QueryString["FileName"] != null && Request.QueryString["ApiUrl"] != null)
            {


                var filename = Request.QueryString["FileName"];
                var apiUrl = Request.QueryString["ApiUrl"];
                var cert = new X509Certificate2(AltrusCAPath, AltrusCAPassword, X509KeyStorageFlags.MachineKeySet);
                var data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.0Z"));// DateTime.UtcNow.ToString("yyyy -MM-ddTHH:mm:ss.0Z"));
                ContentInfo info = new ContentInfo(data);
                SignedCms cms = new SignedCms(info, false);
                CmsSigner signer = new CmsSigner(cert);
                signer.IncludeOption = X509IncludeOption.EndCertOnly;// Use if Error: A certificate chain could not be built to a trusted root authority.

                cms.ComputeSignature(signer);
                var signed = cms.Encode();
                var b64 = Convert.ToBase64String(signed);
                //  File.WriteAllText(b64, "apikey.txt");


                using (WebClient client1 = new WebClient())
                {
                    client1.Headers["tyl-efm-api"] = b64;
                    client1.DownloadFile(AltrusBASEURL + "/" + apiUrl, Server.MapPath("~/Download/" + filename + ".zip"));
                    //@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\" + filename + ".zip");
                    resp = "{'file_path':'" + ("/Download/" + filename + ".zip") + "'}";
                    // string xml = System.IO.File.ReadAllText(@"C:\inetpub\wwwroot\tyler_api_live\AltrueCA\mylocations\locations.xml");

                    //  XmlDocument doc = new XmlDocument();
                    //  doc.LoadXml(xml);
                    //resp = JsonConvert.SerializeXmlNode(doc);

                }
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult demotest12()
        {
            var resp = "{'status':'demo'}";
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            UserNameHeader obj = new UserNameHeader();
            return View(obj);
        }

        [HttpPost]
        public ActionResult Index(UserNameHeader objuserlogin)
        {
            var display = Userloginvalues().Where(m => m.UserName == objuserlogin.UserName && m.Password == objuserlogin.Password).FirstOrDefault();
            if (display != null)
            {
                ViewBag.Status = "CORRECT UserNAme and Password";
            }
            else
            {
                ViewBag.Status = "INCORRECT UserName or Password";
            }
            return View(objuserlogin);
        }

        public List<UserNameHeader> Userloginvalues()
        {
            List<UserNameHeader> objModel = new List<UserNameHeader>();
            objModel.Add(new UserNameHeader { UserName = "user1", Password = "password1" });
            objModel.Add(new UserNameHeader { UserName = "user2", Password = "password2" });
            objModel.Add(new UserNameHeader { UserName = "user3", Password = "password3" });
            objModel.Add(new UserNameHeader { UserName = "user4", Password = "password4" });
            objModel.Add(new UserNameHeader { UserName = "user5", Password = "password5" });
            return objModel;
        }


        public ActionResult GetCaseList_V4()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //efmFirmServiceClient.ClientCredentials.UserName.UserName = objuserlogin.UserName;
            //efmFirmServiceClient.ClientCredentials.UserName.Password = objuserlogin.Password;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; //Request.Form["Password"];

            //if (objuserlogin.UserName == null || objuserlogin.Password == null)
            //{
            //    ViewBag.Status = "INCORRECT UserNAme and Password";
            //}
            //else
            //{
            List<Root> ObjEmp = new List<Root>();

            //TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessage = new TylerCourtRecordMDEService.CaseListQueryMessageType();

            //TylerCourtRecordMDEService.IdentificationType identificationType1 = new TylerCourtRecordMDEService.IdentificationType();
            //caseListQueryMessage.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType
            //{
            //    IdentificationID = new TylerCourtRecordMDEService.@string()
            //};
            //caseListQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";
            //caseListQueryMessage.QuerySubmitter =new TylerCourtRecordMDEService.EntityType
            //{

            //};
            //caseListQueryMessage.CaseCourt = new TylerCourtRecordMDEService.CourtType();
            //caseListQueryMessage.CaseCourt.OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType
            //{
            //    IdentificationID = new TylerCourtRecordMDEService.@string()
            //};
            //caseListQueryMessage.CaseListQueryCase = new TylerCourtRecordMDEService.CaseType[4];

            //// new code
            ///
            TylerCourtRecordMDEService.CaseListQueryMessageType caseListQueryMessageType = new TylerCourtRecordMDEService.CaseListQueryMessageType();

            string casenumber = "227702";

            caseListQueryMessageType.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
            {
                IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
            };

            caseListQueryMessageType.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            caseListQueryMessageType.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
            {
                ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
                Item = new TylerCourtRecordMDEService.PersonType2()
            };

            caseListQueryMessageType.CaseCourt = new TylerCourtRecordMDEService.CourtType()
            {
                OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
                {
                    IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "alameda:crfre" }
                }
            };



            caseListQueryMessageType.CaseListQueryCase = new TylerCourtRecordMDEService.CaseType[] {
            new TylerCourtRecordMDEService.CaseType(){  CaseDocketID = new TylerCourtRecordMDEService.@string(){ Value = casenumber} }
            };


            efmFirmServiceClient.Open();

            //caseListQueryMessage.CaseListQueryCase[0].CaseTitleText = new TylerCourtRecordMDEService.TextType();
            //caseListQueryMessage.CaseListQueryCase[1].CaseCategoryText = new TylerCourtRecordMDEService.TextType();
            //caseListQueryMessage.CaseListQueryCase[2].CaseTrackingID = new TylerCourtRecordMDEService.@string();
            //caseListQueryMessage.CaseListQueryCase[3].CaseDocketID = new TylerCourtRecordMDEService.@string();

            //TylerCourtRecordMDEService.QueryMessageType queryMessageType = new TylerCourtRecordMDEService.QueryMessageType() { SendingMDELocationID="";}
            ////queryMessageType.SendingMDELocationID.id = "";


            //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();
            //TylerCourtRecordMDEService.@string @string1 = new TylerCourtRecordMDEService.@string();
            //@string1.id = "harris";
            //identificationType.IdentificationID = @string1;

            //TylerCourtRecordMDEService.EntityType entityType = new TylerCourtRecordMDEService.EntityType();
            //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
            //courtType.OrganizationIdentification = identificationType;
            //TylerCourtRecordMDEService.OrganizationType organizationType = new TylerCourtRecordMDEService.OrganizationType();
            //organizationType.OrganizationIdentification.IdentificationID.id = "harris:dc";

            //TylerCourtRecordMDEService.CaseType[] casetype = new TylerCourtRecordMDEService.CaseType[4];

            //TylerCourtRecordMDEService.CaseType caseType1 = new TylerCourtRecordMDEService.CaseType();


            //casetype[0].CaseTitleText = null;
            //casetype[1].CaseCategoryText = null;
            //casetype[2].CaseTrackingID= null;
            //casetype[3].CaseDocketID.Value= "2011-CV-0044";

            //caseListQueryMessage.SendingMDELocationID sendingMDELocationID = new   caseListQueryMessage.SendingMDELocationID();
            //caseListQueryMessage.SendingMDEProfileCode sendingMDEProfile = new caseListQueryMessage.SendingMDEProfileCode(); 
            //caseListQueryMessage.QuerySubmitter querysubmit=new caseListQueryMessage.QuerySubmitter();

            //caseListQueryMessage.CaseCourt courtType=new caseListQueryMessage.CaseCourt();
            //caseListQueryMessage.CaseListQueryCase caseTypes = new caseListQueryMessage.CaseListQueryCase() ;
            try
            {
                using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
                {

                    var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                    OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);

                    var resp = efmFirmServiceClient.GetCaseList(caseListQueryMessageType);


                    // foreach(var @case in resp.Case)
                    //{
                    //   var CaseCategoryText =  @case.CaseCategoryText.Value;

                    //}

                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);

                    //System.IO.File.WriteAllText("D:\\Test.html", jsonString);

                    Root myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonString);

                    ////var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CaseSample>(jsonString);

                    efmFirmServiceClient.Close();

                    //if (myDeserializedClass.Error[0].ErrorCode.Value == "0")
                    //{
                    //    foreach (var data1 in myDeserializedClass.Case)
                    //    {
                    //        string IdentificationID = data1.CaseAugmentation.CaseCourt.OrganizationIdentification.IdentificationID.Value;
                    //        string CaseTypeText = data1.CaseAugmentation1.CaseTypeText.Value;
                    //    }
                    //}

                    //if (resp.Error?.Length > 0 && resp.Error[0].ErrorCode.Value == "0")
                    //{
                    //    foreach (var @case in resp.Case)
                    //    {


                    //    }

                    //}
                    ViewBag.Status = "";
                    //return View(myDeserializedClass.Case);
                    return Json(resp, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Status = "Enter correct username and Password";
            }

            //}          


            return null;

            //TylerCourtRecordMDEService.TextType textType = new TylerCourtRecordMDEService.TextType();
            //textType.id = "abc";

            //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
            //courtType.CourtName = textType;
            ////courtType.CourtName.Value = "abc";
            //caseListQueryMessage.CaseCourt = courtType;

            //TylerCourtRecordMDEService.@string @string = new TylerCourtRecordMDEService.@string();



            //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();


            //caseListQueryMessage.SendingMDELocationID = identificationType;

            //caseListQueryMessage.SendingMDELocationID
            //caseListQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";
            //caseListQueryMessage.QuerySubmitter.id = "";
            //caseListQueryMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";


            //TylerCourtRecordMDEV5.GetCaseListRequestMessageType getCaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();

            //getCaseListRequestMessageType.CaseCourt.OrganizationIdentification.id = "harris:dc";

            //  TylerCourtRecordMDEV5.GetServiceCaseListRequestType getServiceCaseListRequestType  = new TylerCourtRecordMDEV5.GetServiceCaseListRequestType();
            //getServiceCaseListRequestType.GetServiceCaseListMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";

            // //caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = "harris:dc";
            // // caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.Value = "harris:dc";

            //caseListQueryMessageType.

            //var list = caseListQueryMessageType.CaseListQueryCase;
            // foreach(var itm in list)
            //     {
            //     itm.CaseTitleText = null;
            //     itm.CaseCategoryText = null;
            //     itm.CaseTrackingID = null;
            //     itm.CaseDocketID.id = "2011-CV-0044";


            // }
            // //request xml write

        }

        public ActionResult GetCase_V4()
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            efmFirmServiceClient.ClientCredentials.UserName.UserName = "zahoorahmed481+26@gmail.com"; ///Request.Form["UserName"];
            efmFirmServiceClient.ClientCredentials.UserName.Password = "ca2b3086-4aa2-41f8-9b43-5402c5fbbc13"; //Request.Form["Password"];



            TylerCourtRecordMDEService.CaseQueryMessageType caseQueryMessage = new TylerCourtRecordMDEService.CaseQueryMessageType();

            caseQueryMessage.CaseQueryCriteria = new TylerCourtRecordMDEService.CaseQueryCriteriaType()
            {
                IncludeCalendarEventIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeDocketEntryIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeParticipantsIndicator = new TylerCourtRecordMDEService.boolean() { Value = true },
                DocketEntryTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" },
                CalendarEventTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" }
            };

            //caseQueryMessage.CaseTrackingID = new TylerCourtRecordMDEService.@string() { Value = Request.QueryString["id"] };

            caseQueryMessage.CaseTrackingID = new TylerCourtRecordMDEService.@string() { Value = "dbed64e6-2379-4290-87b8-a42f958a3e39" };

            caseQueryMessage.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
            {
                IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
            };

            caseQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            caseQueryMessage.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
            {
                ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
                Item = new TylerCourtRecordMDEService.PersonType2()
            };

            caseQueryMessage.CaseCourt = new TylerCourtRecordMDEService.CourtType()
            {
                OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
                {
                    IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "alameda:crfre" }
                }
            };

            efmFirmServiceClient.Open();


            //TylerCourtRecordMDEService.QueryMessageType queryMessageType = new TylerCourtRecordMDEService.QueryMessageType();
            //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();



            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.GetCase(caseQueryMessage);

                CaseDemo model = new CaseDemo();

                if (resp.Error?.Length > 0 && resp.Error[0].ErrorCode.Value == "0")
                {
                    model.CaseTitleText = resp.Case.CaseTitleText.Value;
                    model.CaseCategoryText = resp.Case.CaseCategoryText.Value;
                    model.CaseTrackingID = resp.Case.CaseTrackingID.Value;
                    model.CaseDocketID = resp.Case.CaseDocketID.Value;



                    var civilCaseType = (TylerCourtRecordMDEService.CriminalCaseType)resp.Case;
                    foreach (var caseParticipant in civilCaseType.CaseAugmentation1.CaseParticipant)
                    {
                        if (caseParticipant.Item is TylerCourtRecordMDEService.PersonType2)
                        {

                            var person = (TylerCourtRecordMDEService.PersonType2)caseParticipant.Item;
                            model.itemId = person.id;
                            model.PersonGivenName = person.PersonName.PersonGivenName.Value;
                            model.PersonMiddleName = person.PersonName.PersonMiddleName.Value;
                            model.PersonSurName = person.PersonName.PersonSurName.Value;

                            foreach (var contactInfo in person.PersonAugmentation.ContactInformation)
                            {
                                var Item = (TylerCourtRecordMDEService.TelephoneNumberType)contactInfo.Items[0];
                                var fulltelephone = (TylerCourtRecordMDEService.FullTelephoneNumberType)Item.Item;
                                model.TelephoneNumberFullID = fulltelephone.TelephoneNumberFullID.Value;

                                var Item1 = (TylerCourtRecordMDEService.@string)contactInfo.Items[1];
                                var value = Item1.Value;

                                var Item2 = (TylerCourtRecordMDEService.AddressType)contactInfo.Items[2];
                                var Item22 = (TylerCourtRecordMDEService.StructuredAddressType)Item2.Item;

                                var streetDetails = (TylerCourtRecordMDEService.StreetType)Item22.Items[0];
                                model.StreetFullText = streetDetails.StreetFullText.Value;

                                var textType = (TylerCourtRecordMDEService.TextType)Item22.Items[1];
                                model.textTypeValue = textType.Value;

                                var itemElementName = (TylerCourtRecordMDEService.ItemChoiceType)Item22.ItemsElementName[1];

                                var LocationCityName = (TylerCourtRecordMDEService.ProperNameTextType)Item22.LocationCityName;
                                model.LocationCityNameValue = LocationCityName.Value;

                                var LocationPostalCode = (TylerCourtRecordMDEService.@string)Item22.LocationPostalCode;
                                var LocationPostalCodeValue = LocationPostalCode.Value;

                                var ItemProperName = (TylerCourtRecordMDEService.@string)Item22.Item1;
                                model.ItemProperNameValue = ItemProperName.Value;

                                var CountryCode = (TylerCourtRecordMDEService.@string)Item22.Item2;
                                model.CountryCodeValue = CountryCode.Value;

                            }

                            foreach (var PersonOtherIdentification in person.PersonOtherIdentification)
                            {
                                var PersonOtherIdentificationDetails = PersonOtherIdentification.IdentificationID.Value;
                                var ItemValuePerson = (TylerCourtRecordMDEService.TextType)PersonOtherIdentification.Item;
                                var ItemValuePersonValue = ItemValuePerson.Value;
                            }




                        }

                    }



                    foreach (var CaseJudge in civilCaseType.CaseAugmentation.CaseJudge)
                    {
                        if (CaseJudge.JudicialOfficialBarMembership is TylerCourtRecordMDEService.JudicialOfficialBarMembershipType)
                        {
                            model.memberShip = CaseJudge.JudicialOfficialBarMembership.JudicialOfficialBarIdentification.IdentificationID.Value;
                        }
                    }

                }

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);

                efmFirmServiceClient.Close();

                //return View(model);
                return Json(resp, JsonRequestBehavior.AllowGet);
            }


            return null;








            //TylerCourtRecordMDEService.TextType textType = new TylerCourtRecordMDEService.TextType();
            //textType.id = "abc";

            //TylerCourtRecordMDEService.CourtType courtType = new TylerCourtRecordMDEService.CourtType();
            //courtType.CourtName = textType;
            ////courtType.CourtName.Value = "abc";
            //caseListQueryMessage.CaseCourt = courtType;

            //TylerCourtRecordMDEService.@string @string = new TylerCourtRecordMDEService.@string();



            //TylerCourtRecordMDEService.IdentificationType identificationType = new TylerCourtRecordMDEService.IdentificationType();


            //caseListQueryMessage.SendingMDELocationID = identificationType;

            //caseListQueryMessage.SendingMDELocationID
            //caseListQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";
            //caseListQueryMessage.QuerySubmitter.id = "";
            //caseListQueryMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";


            //TylerCourtRecordMDEV5.GetCaseListRequestMessageType getCaseListRequestMessageType = new TylerCourtRecordMDEV5.GetCaseListRequestMessageType();

            //getCaseListRequestMessageType.CaseCourt.OrganizationIdentification.id = "harris:dc";

            //  TylerCourtRecordMDEV5.GetServiceCaseListRequestType getServiceCaseListRequestType  = new TylerCourtRecordMDEV5.GetServiceCaseListRequestType();
            //getServiceCaseListRequestType.GetServiceCaseListMessage.CaseCourt.OrganizationIdentification.id = "harris:dc";

            // //caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.id = "harris:dc";
            // // caseListQueryMessageType.CaseCourt.OrganizationIdentification.IdentificationID.Value = "harris:dc";

            //caseListQueryMessageType.

            //var list = caseListQueryMessageType.CaseListQueryCase;
            // foreach(var itm in list)
            //     {
            //     itm.CaseTitleText = null;
            //     itm.CaseCategoryText = null;
            //     itm.CaseTrackingID = null;
            //     itm.CaseDocketID.id = "2011-CV-0044";


            // }
            // //request xml write

        }

        [Authorize]
        public ActionResult GetCaseTest(string CaseTrackingID)
        {
            TylerCourtRecordMDEService.CourtRecordMDEPortClient efmFirmServiceClient = new TylerCourtRecordMDEService.CourtRecordMDEPortClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = x509Certificate2;
            //   efmFirmServiceClient.ClientCredentials.UserName.UserName = "ghorpadesumit471@gmail.com"; ///Request.Form["UserName"];
            //efmFirmServiceClient.ClientCredentials.UserName.Password = "ebacc0ab-c1a3-495e-9d39-c5eb19cd5947"; //Request.Form["Password"];



            TylerCourtRecordMDEService.CaseQueryMessageType caseQueryMessage = new TylerCourtRecordMDEService.CaseQueryMessageType();

            caseQueryMessage.CaseQueryCriteria = new TylerCourtRecordMDEService.CaseQueryCriteriaType()
            {
                IncludeCalendarEventIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeDocketEntryIndicator = new TylerCourtRecordMDEService.boolean() { Value = false },
                IncludeParticipantsIndicator = new TylerCourtRecordMDEService.boolean() { Value = true },
                DocketEntryTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" },
                CalendarEventTypeCodeFilterText = new TylerCourtRecordMDEService.TextType() { Value = "false" }
            };
            CaseTrackingID = "9b937fb2-a35c-4221-835e-334ce39d522e";

            caseQueryMessage.CaseTrackingID = new TylerCourtRecordMDEService.@string() { Value = CaseTrackingID };//Request.QueryString["id"] };


            caseQueryMessage.SendingMDELocationID = new TylerCourtRecordMDEService.IdentificationType()
            {
                IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "https:localhost" }
            };

            caseQueryMessage.SendingMDEProfileCode = "urn:oasis:names:tc:legalxml-courtfiling:schema:xsd:WebServicesMessaging-2.0";

            caseQueryMessage.QuerySubmitter = new TylerCourtRecordMDEService.EntityType()
            {
                ItemElementName = TylerCourtRecordMDEService.ItemChoiceType2.EntityPerson,
                Item = new TylerCourtRecordMDEService.PersonType2()
            };

            caseQueryMessage.CaseCourt = new TylerCourtRecordMDEService.CourtType()
            {
                OrganizationIdentification = new TylerCourtRecordMDEService.IdentificationType()
                {
                    IdentificationID = new TylerCourtRecordMDEService.@string() { Value = "fresno:cv" }
                }
            };

            efmFirmServiceClient.Open();




            using (new OperationContextScope(efmFirmServiceClient.InnerChannel))
            {
                //var msgHeader = MessageHeader.CreateHeader("UserNameHeader", "urn:tyler:efm:services", efmFirmServiceClient.ClientCredentials.UserName);
                //OperationContext.Current.OutgoingMessageHeaders.Add(msgHeader);
                var resp = efmFirmServiceClient.GetCase(caseQueryMessage);


                efmFirmServiceClient.Close();

                return Json(resp, JsonRequestBehavior.AllowGet);
            }


        }
    }

}

