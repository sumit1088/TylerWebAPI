using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tyler_web_app.Controllers
{
  //  [Produces("application/json")]
  // adentity 

    [Route("api/API")]
    public class APIController : Controller
    {
        

        // GET: API
        public ActionResult Index()
        {

            ViewBag.Title = "DEmo api";
            return View();
        }
        [Route("~/api/GetDetails")]
        [HttpPost]
        public ActionResult GetDetails(){
           // Request.Form["Email"]
                            return Json("{}", JsonRequestBehavior.AllowGet);
        }
        [Route("~/api/GetDetFullDeatilsails")]
        [HttpPost]
        public ActionResult FullDeatils() {
            return Json(Request.Form.ToString(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult RegisterUser()
        {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = EFMClient.LoadCertificateFromFile(@"C:\inetpub\wwwroot\tyler_web_app\tyler_web_app\AltrueCA\AltrueCA.pfx", "MyP@ssword1"); ;
            EFMFirm.RegistrationRequestType registrationRequestType = new EFMFirm.RegistrationRequestType();
            switch (Request.Form["RegistrationType"]) {
                case "FirmAdministrator":
                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdministrator ;
                break;
                case "FirmAdminNewMember":
                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.FirmAdminNewMember;
                    break;
                case "Individual":
                    registrationRequestType.RegistrationType = EFMFirm.RegistrationType.Individual;
                    break;
            }
            registrationRequestType.Email = Request.Form["Email"];


            var resp = efmFirmServiceClient.RegisterUser(registrationRequestType);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserRequest() {
            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = EFMClient.LoadCertificateFromFile(@"C:\inetpub\wwwroot\tyler_web_app\tyler_web_app\AltrueCA\AltrueCA.pfx", "MyP@ssword1"); ;
            EFMFirm.GetUserRequestType getUserRequestType = new EFMFirm.GetUserRequestType();
            getUserRequestType.UserID = Request.Form["UserID"];
            var resp = efmFirmServiceClient.GetUser(getUserRequestType);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserList()
        {

            EFMFirm.EfmFirmServiceClient efmFirmServiceClient = new EFMFirm.EfmFirmServiceClient();
            efmFirmServiceClient.ClientCredentials.ClientCertificate.Certificate = EFMClient.LoadCertificateFromFile(@"C:\inetpub\wwwroot\tyler_web_app\tyler_web_app\AltrueCA\AltrueCA.pfx", "MyP@ssword1"); ;
             var resp =  efmFirmServiceClient.GetUserList();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

    }
}