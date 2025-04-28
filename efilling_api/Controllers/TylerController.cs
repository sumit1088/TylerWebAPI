using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using efilling_api.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using Method = RestSharp.Method;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System;
using System.Reflection.Emit;
using System.Xml.Linq;
using Npgsql;
using System.Collections.Generic;
using Microsoft.DotNet.MSIdentity.Shared;
using System.IO.Compression;
using NuGet.Packaging;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using efilling_api.Services;
using static efilling_api.Models.InitiateFilingResponse;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using static efilling_api.Models.InitialCaseDetails;
using Microsoft.CodeAnalysis.Operations;
using static efilling_api.Models.ServeFilingRequest;
using Newtonsoft.Json.Linq;

#nullable disable

namespace efilling_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TylerController : ControllerBase
    {
        //DI of DB Context
        private readonly EFilling_DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "Server=192.168.1.28;Port=5432;Database=EFilling_DB_Dev;User Id=postgres;Password=root;";
        private readonly UserService _userService;
        public TylerController(EFilling_DBContext context, IConfiguration iConfig, UserService userService)
        {
            _context = context;
            _configuration = iConfig;
            _userService = userService;
        }

        // WORKING
        [HttpPost]
        [Authorize]
        [Route("GetCaseList")]
        public async Task<ActionResult> GetCaseList(CaseRequest caseRequest)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CaseListUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseListUrl' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("CaseNumber", caseRequest.CaseNumber.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        //[HttpPost]
        //[Authorize]
        //[Route("GetCaseList_Business")]
        //public async Task<ActionResult> GetCaseList_Business()
        //{
        //    CommonResponse resp = new CommonResponse();

        //    string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
        //    string url = ip + _configuration.GetValue<string>("Tyler:GetCaseList_Business");

        //    if (String.IsNullOrWhiteSpace(url))
        //    {
        //        resp.success = false;
        //        resp.message = "'Tyler:GetCaseList_Business' is null or empty.";
        //        resp.data = null;

        //        return Ok(resp);
        //    }


        //    try
        //    {
        //        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        //        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        //            return BadRequest("Token is missing or invalid.");

        //        // Extract the token (remove "Bearer " prefix)
        //        var token = authHeader.Substring("Bearer ".Length);
        //        var UserName = _userService.GetEmailFromToken(token);

        //        // Query database for password hash
        //        var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
        //        var client = new RestClient(url);

        //        //var request = new RestRequest(Method.Get.ToString());
        //        var request = new RestRequest(Method.Get.ToString())
        //            .AddParameter("UserName", UserName.ToString())
        //            .AddParameter("Password", Password.ToString())
        //            .AddParameter("CaseNumber", caseRequest.CaseNumber.ToString()
        //            );

        //        RestResponse response = await client.ExecuteAsync(request);

        //        object jsonData = JsonConvert.DeserializeObject(response.Content);

        //        resp.success = true;
        //        resp.status = 200;
        //        resp.message = "Success!";
        //        resp.data = jsonData;

        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.success = false;
        //        resp.message = "Failed! " + ex.ToString();
        //        resp.data = null;
        //        return Ok(resp);
        //    }

        //}

        [HttpPost]
        [Authorize]
        [Route("GetCaseList_Business")]
        public async Task<ActionResult> GetCaseList_Business()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string getCaseListUrl = ip + _configuration.GetValue<string>("Tyler:GetCaseList_Business");
            string getFirmUrl = ip + _configuration.GetValue<string>("Tyler:GetFirm");

            if (String.IsNullOrWhiteSpace(getCaseListUrl) || String.IsNullOrWhiteSpace(getFirmUrl))
            {
                resp.success = false;
                resp.message = "'Tyler:GetCaseList_Business' or 'Tyler:GetFirm' is null or empty.";
                resp.data = null;
                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                // Step 1: Call GetFirm API to get FirmName
                var firmClient = new RestClient(getFirmUrl);
                var firmRequest = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString());

                RestResponse firmResponse = await firmClient.ExecuteAsync(firmRequest);               

                dynamic firmJsonData = JsonConvert.DeserializeObject(firmResponse.Content);

                var jsonObj = JObject.Parse(firmResponse.Content);
                var firmData = jsonObj["Firm"];

                if (firmData == null)
                {
                    resp.success = false;
                    resp.message = "Firm data is missing from the response.";
                    resp.data = null;
                    return Ok(resp);
                }

                string firmName = firmData["FirmName"]?.ToString();

                if (string.IsNullOrEmpty(firmName))
                {
                    resp.success = false;
                    resp.message = "FirmName is missing.";
                    resp.data = null;
                    return Ok(resp);
                }              

                // Step 2: Call GetCaseList_Business API, passing UserName, Password, and FirmName
                var caseListClient = new RestClient(getCaseListUrl);
                var caseListRequest = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("FirmName", firmName);  // Pass FirmName from GetFirm

                RestResponse caseListResponse = await caseListClient.ExecuteAsync(caseListRequest);
                object caseListJsonData = JsonConvert.DeserializeObject(caseListResponse.Content);

                // Step 3: Return the combined response or just the Case List data
                var combinedData = new
                {
                    FirmData = firmJsonData,  // Firm data from GetFirm
                    CaseListData = caseListJsonData  // Case list data from GetCaseList_Business
                };

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = caseListJsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("GetAttorneyList")]
        public async Task<ActionResult> GetAttorneyList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetAttorneyList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetAttorneyList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password);

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("GetAttorneyManageUserList")]
        public async Task<ActionResult> GetAttorneyManageUserList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetAttorneyList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetAttorneyList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password);

                RestResponse response = await client.ExecuteAsync(request);

                //object jsonData = JsonConvert.DeserializeObject(response.Content);


                //resp.success = true;
                //resp.status = 200;
                //resp.message = "Success!";
                //resp.data = jsonData;

                //return Ok(resp);


                // Deserialize the response as a list of attorneys
                dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Extract the list of attorneys
                var attorneysList = jsonResponse?.Attorney as JArray;

                if (attorneysList == null || attorneysList.Count == 0)
                {
                    resp.success = false;
                    resp.message = "No attorney data found in response.";
                    resp.data = null;
                    return Ok(resp);
                }

                // Extract all AttorneyIDs from the list
                List<string> attorneyIDs = attorneysList
                    .Where(attorney => attorney["AttorneyID"] != null) // Ensure AttorneyID exists
                    .Select(attorney => attorney["AttorneyID"].ToString())
                    .Distinct()
                    .ToList();

                Console.WriteLine($"Extracted AttorneyIDs: {string.Join(", ", attorneyIDs)}");

                // Fetch all AttorneyDetails matching the extracted AttorneyIDs
                List<AttorneyDetails> attorneyDetailsList = await _context.AttorneyDetails
                    .Where(a => attorneyIDs.Contains(a.AttorneyID))
                    .ToListAsync();

                // Combine response data and AttorneyDetails
                var combinedData = new
                {
                    ResponseData = jsonResponse,   // Original API response
                    AttorneyDetails = attorneyDetailsList // Matched attorney records
                };

                // Construct response
                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = combinedData;

                return Ok(resp);

            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetAttorney")]
        public async Task<ActionResult> GetAttorney(GetAttorney getAttorney)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetAttorney");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetAttorney' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password)
                    .AddParameter("AttorneyID", getAttorney.AttorneyID.ToString());

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                AttorneyDetails attorneyDetails = await _context.AttorneyDetails.FirstOrDefaultAsync(c => c.AttorneyID == getAttorney.AttorneyID);

                resp.success = true;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("GetAttorneyManage")]
        public async Task<ActionResult> GetAttorneyManage([FromBody] GetAttorney getAttorney)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetAttorney");

            if (string.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetAttorney' is null or empty.";
                resp.data = null;
                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password)
                    .AddParameter("AttorneyID", getAttorney.AttorneyID.ToString());

                RestResponse response = await client.ExecuteAsync(request);

                // Deserialize response content
                dynamic jsonData = JsonConvert.DeserializeObject(response.Content);

                if (jsonData == null)
                {
                    resp.success = false;
                    resp.message = "Failed to retrieve attorney data from external API.";
                    resp.data = null;
                    return Ok(resp);
                }

                // Fetch attorney details from database
                AttorneyDetails attorneyDetails = await _context.AttorneyDetails
                    .FirstOrDefaultAsync(a => a.AttorneyID == getAttorney.AttorneyID);

                // Combine external API data with database data
                var responseData = new
                {
                    ApiResponse = jsonData,
                    AttorneyDetails = attorneyDetails
                };

                resp.success = true;
                resp.message = "Success!";
                resp.data = responseData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }
        }



        // WORKING
        [HttpPost]
        [Authorize]
        [Route("GetCase")]
        public async Task<ActionResult> GetCase(GetCase getCase)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CaseUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseUrl' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url); //("http://localhost/Home/CaseUrl?id=dbed64e6-2379-4290-87b8-a42f958a3e39");

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("selectedCourt", getCase.selectedCourt.ToString())
                    .AddParameter("CaseTrackingID", getCase.CaseTrackingID.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }
        }



        // WORKING t8@gmail.com
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<ActionResult> RegisterUser(UserInfo userInfo)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RegisterUser");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RegisterUser' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                                    .AddParameter("Email", userInfo.Email.ToString())
                                    .AddParameter("Password", userInfo.PasswordEncrypted.ToString())
                                    .AddParameter("FirstName", userInfo.FirstName.ToString())
                                    .AddParameter("MiddleName", userInfo.MiddleName.ToString())
                                    .AddParameter("LastName", userInfo.LastName.ToString())
                                    .AddParameter("PasswordQuestion", userInfo.SecurityQuestion.ToString())
                                    .AddParameter("PasswordAnswer", userInfo.SecurityAnswer.ToString())
                                    .AddParameter("StreetAddressLine1", userInfo.Address1.ToString())
                                    .AddParameter("StreetAddressLine2", userInfo.Address2.ToString())
                                    .AddParameter("City", userInfo.City.ToString())
                                    .AddParameter("StateCode", userInfo.State.ToString())
                                    .AddParameter("ZipCode", userInfo.ZipCode.ToString())
                                    .AddParameter("CountryCode", userInfo.CountryCode.ToString())
                                    .AddParameter("FirmName", userInfo.FirmName.ToString())
                                    .AddParameter("PhoneNumber", userInfo.PhoneNo.ToString())
                                    .AddParameter("RegistrationType", userInfo.RegistrationType.ToString()
                                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);
                string result = jsonData.ToString();
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(result);
                userResponse.Email = userInfo.Email;

                if (userResponse.UserID != null && userResponse.Error.ErrorCode == "0")
                {
                    await _context.UserResponses.AddAsync(userResponse);
                    await _context.SaveChangesAsync();

                    await _context.UserInfos.AddAsync(userInfo);
                    await _context.SaveChangesAsync();
                }

                if (userResponse.Error.ErrorCode == "0")
                {
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;
                }
                else
                {
                    resp.success = false;
                    resp.message = "Failed!";
                    resp.data = jsonData;
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }


        // WORKING
        [HttpPost]
        [Authorize]
        [Route("GetFirm")]
        public async Task<ActionResult> GetFirm()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetFirm");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetFirm' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                                  .AddParameter("UserName", UserName.ToString())
                                  .AddParameter("Password", Password.ToString()
                                  );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                UserInfo userInfo = await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == UserName);

                if (userInfo != null)
                {
                    // Convert jsonData to JObject to modify it
                    JObject jsonObject = jsonData as JObject ?? new JObject();

                    // Add CreatedAt to jsonData
                    jsonObject["CreatedAt"] = userInfo.CreatedAt;

                    // Assign back the modified JSON
                    jsonData = jsonObject;
                }

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }
        }

        // WORKING
        [HttpPost]
        [Authorize]
        [Route("GetUserList")]
        public async Task<ActionResult> GetUserList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetUserList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetUserList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString()
                   );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);


                UserInfo userInfo = await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == UserName);

                if (userInfo != null)
                {
                    // Convert jsonData to JObject to modify it
                    JObject jsonObject = jsonData as JObject ?? new JObject();

                    // Add CreatedAt to jsonData
                    jsonObject["CreatedAt"] = userInfo.CreatedAt;

                    // Assign back the modified JSON
                    jsonData = jsonObject;
                }

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        // WORKING
        [HttpPost]
        [Route("RegisterFirm")]
        public async Task<ActionResult> RegisterFirm(String Email = "t10@gmail.com", String Password = "Tyler@123")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RegisterFirm");


            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseListUrl' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                                   .AddParameter("Email", Email.ToString())
                                   .AddParameter("Password", Password.ToString()
                                   );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }

        // WORKING
        // ID:5955cc5a-c5bb-4366-add4-3e14843eabbf
        [HttpPost]
        [Authorize]
        [Route("AddUserRole")]
        public async Task<ActionResult> AddUserRole(string UserID)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:AddUserRole");


            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:AddUserRole' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                     .AddParameter("UserName", UserName.ToString())
                     .AddParameter("Password", Password.ToString())
                     .AddParameter("UserID", UserID.ToString()
                     );
                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }


        //[HttpPost]
        //[Route("GetUserRequest")]
        //public async Task<ActionResult> GetUserRequest(String UserID)
        //{
        //    CommonResponse resp = new CommonResponse();

        //    string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
        //    string url = ip + _configuration.GetValue<string>("Tyler:GetUserRequest");


        //    if (String.IsNullOrWhiteSpace(url))
        //    {
        //        resp.success = false;
        //        resp.message = "'Tyler:GetUserRequest' is null or empty.";
        //        resp.data = null;

        //        return Ok(resp);
        //    }


        //    try
        //    {
        //        var client = new RestClient(url);

        //        var request = new RestRequest(Method.Get.ToString()).AddParameter("UserID", UserID.ToString());

        //        RestResponse response = await client.ExecuteAsync(request);

        //        object jsonData = JsonConvert.DeserializeObject(response.Content);

        //        resp.success = true;
        //        resp.message = "Success!";
        //        resp.data = jsonData;

        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.success = false;
        //        resp.message = "Failed! " + ex.ToString();
        //        resp.data = null;
        //        return Ok(resp);
        //    }


        //}


        //[HttpPost]
        //[Route("ReviewFiling")]
        //public async Task<ActionResult> ReviewFiling()
        //{
        //    CommonResponse resp = new CommonResponse();

        //    string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
        //    string url = ip + _configuration.GetValue<string>("Tyler:ReviewFiling");

        //    if (String.IsNullOrWhiteSpace(url))
        //    {
        //        resp.success = false;
        //        resp.message = "'Tyler:ReviewFiling' is null or empty.";
        //        resp.data = null;

        //        return Ok(resp);
        //    }

        //    try
        //    {
        //        var client = new RestClient(url);
        //        var request = new RestRequest(Method.Get.ToString());

        //        RestResponse response = await client.ExecuteAsync(request);

        //        object jsonData = JsonConvert.DeserializeObject(response.Content);

        //        resp.success = true;
        //        resp.status = 200;
        //        resp.message = "Success!";
        //        resp.data = jsonData;

        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.success = false;
        //        resp.message = "Failed! " + ex.ToString();
        //        resp.data = null;
        //        return Ok(resp);
        //    }

        //}


        [HttpPost]
        [Authorize]
        [Route("CreateAttorney")]
        public async Task<ActionResult> CreateAttorney(AttorneyDetails attorneyDetails)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CreateAttorney");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CreateAttorney' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());

                //var request = new RestRequest("/Home/CreateAttorney", Method.Get);

                var request = new RestRequest(Method.Get.ToString())
                .AddParameter("UserName", UserName.ToString())//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password.ToString())//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("BarNumber", attorneyDetails.BarId.ToString())//"900009")
                .AddParameter("FirstName", attorneyDetails.FirstName.ToString())//"sam")
                .AddParameter("MiddleName", attorneyDetails.MiddleName.ToString())//"zac")
                .AddParameter("LastName", attorneyDetails.LastName.ToString())//"van")
                .AddParameter("FirmID", attorneyDetails.FirmID.ToString());//"83fe15e1-2896-478c-b4ec-a1996b4e2f46");

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                string jsonResponse = response.Content;

                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                string attorneyId = responseObject.AttorneyID;

                attorneyDetails.AttorneyID = attorneyId;

                if (responseObject.AttorneyID != null && responseObject.Error.ErrorCode == "0")
                {
                    await _context.AttorneyDetails.AddAsync(attorneyDetails);
                    await _context.SaveChangesAsync();
                }

                //if (responseObject.Error.ErrorCode == "0")
                //{
                //    resp.success = true;
                //    resp.status = 200;
                //    resp.message = "Success!";
                //    resp.data = responseObject;
                //}
                //else
                //{
                //    resp.success = false;
                //    resp.message = "Failed!";
                //    resp.data = responseObject;
                //}               

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("UpdateAttorney")]
        public async Task<ActionResult> UpdateAttorney(AttorneyDetails updateAttorney)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateAttorney");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateAttorney' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("AttorneyID", updateAttorney.AttorneyID)//"1a1d114b-f751-49ce-bec3-5379dfe45b18")
                .AddParameter("BarNumber", updateAttorney.BarId.ToString())//"900009")
                .AddParameter("FirstName", updateAttorney.FirstName)//"sam")
                .AddParameter("MiddleName", updateAttorney.MiddleName)//"zac")
                .AddParameter("LastName", updateAttorney.LastName)//"van")
                .AddParameter("FirmID", updateAttorney.FirmID);//"83fe15e1-2896-478c-b4ec-a1996b4e2f46");;

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                string jsonResponse = response.Content;

                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                if (responseObject?.AttorneyID != null && responseObject.Error?.ErrorCode == "0")
                {
                    // Find the existing attorney by ID
                    var existingAttorney = await _context.AttorneyDetails
                                                         .FirstOrDefaultAsync(a => a.AttorneyID == responseObject.AttorneyID);

                    if (existingAttorney != null)
                    {
                        existingAttorney.BarId = updateAttorney.BarId ?? existingAttorney.BarId;
                        existingAttorney.FirstName = updateAttorney.FirstName ?? existingAttorney.FirstName;
                        existingAttorney.MiddleName = updateAttorney.MiddleName ?? existingAttorney.MiddleName;
                        existingAttorney.LastName = updateAttorney.LastName ?? existingAttorney.LastName;
                        existingAttorney.FirmID = updateAttorney.FirmID ?? existingAttorney.FirmID;
                        existingAttorney.Suffix = updateAttorney.Suffix ?? existingAttorney.Suffix;
                        existingAttorney.Email = updateAttorney.Email ?? existingAttorney.Email;
                        existingAttorney.Address1 = updateAttorney.Address1 ?? existingAttorney.Address1;
                        existingAttorney.Address2 = updateAttorney.Address2 ?? existingAttorney.Address2;
                        existingAttorney.PhoneNo = updateAttorney.PhoneNo ?? existingAttorney.PhoneNo;
                        existingAttorney.ZipCode = updateAttorney.ZipCode ?? existingAttorney.ZipCode;
                        existingAttorney.City = updateAttorney.City ?? existingAttorney.City;
                        existingAttorney.State = updateAttorney.State ?? existingAttorney.State;
                        //existingAttorney.makeUserLogin = updateAttorney.makeUserLogin ?? existingAttorney.makeUserLogin;
                        //existingAttorney.makeServiceContact = updateAttorney.makeServiceContact ?? existingAttorney.makeServiceContact;
                        //existingAttorney.makeServiceContactPublic = updateAttorney.makeServiceContactPublic ?? existingAttorney.makeServiceContactPublic;
                        //existingAttorney.makeFirmAdmin = updateAttorney.makeFirmAdmin ?? existingAttorney.makeFirmAdmin;
                        //existingAttorney.recFilingStatusEmails = updateAttorney.recFilingStatusEmails ?? existingAttorney.recFilingStatusEmails;

                        // Save changes to the database
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // If attorney doesn't exist, insert a new record
                        await _context.AttorneyDetails.AddAsync(updateAttorney);
                        await _context.SaveChangesAsync();
                    }
                }


                if (responseObject.Error.ErrorCode == "0")
                {
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;
                }
                else
                {
                    resp.success = false;
                    resp.message = "Failed!";
                    resp.data = jsonData;
                }

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }



        [HttpPost]
        [Authorize]
        [Route("RemoveAttorney")]
        public async Task<ActionResult> RemoveAttorney(string AttorneyID)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RemoveAttorney");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RemoveAttorney' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                    .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                    .AddParameter("AttorneyID", AttorneyID);//"1a1d114b-f751-49ce-bec3-5379dfe45b18";

                RestResponse response = await client.ExecuteAsync(request);
                object jsonData = JsonConvert.DeserializeObject(response.Content);

                var jsonDat = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // ✅ Check if API returned a success response (ErrorCode == "0")
                if (jsonDat != null && jsonDat.Error != null && jsonDat.Error.ErrorCode == "0")
                {
                    // Remove attorney from local database
                    // Convert AttorneyID to integer before querying
                    if (!int.TryParse(AttorneyID, out int attorneyIdInt))
                    {
                        //return BadRequest("Invalid Attorney ID.");
                    }

                    var attorney = _context.AttorneyDetails.Where(a => a.AttorneyID == AttorneyID).FirstOrDefault();

                    //var attorney = await _context.AttorneyDetails.FindAsync(attorneyIdInt);
                    if (attorney == null)
                    {
                        return NotFound();
                    }

                    _context.AttorneyDetails.Remove(attorney);
                    await _context.SaveChangesAsync();


                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Attorney removed successfully!";
                    resp.data = jsonData;
                }

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        // WORKING
        [HttpPost]
        [Authorize]
        [Route("GetServiceContactList")]
        public async Task<ActionResult> GetServiceContactList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetServiceContactList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetServiceContactList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString()
                   );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetServiceContact")]
        public async Task<ActionResult> GetServiceContact(string ServiceContactID)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetServiceContact");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetServiceContact' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password)
                    .AddParameter("ServiceContactID", ServiceContactID);

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("CreateServiceContact")]
        public async Task<ActionResult> CreateServiceContact(ServiceContact serviceContact)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CreateServiceContact");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CreateServiceContact' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("FirmID", serviceContact.FirmID.ToString())//"900009")
                .AddParameter("FirstName", serviceContact.FirstName.ToString())//"sam")
                .AddParameter("MiddleName", serviceContact.MiddleName.ToString())//"zac")
                .AddParameter("LastName", serviceContact.LastName.ToString())//"van")
                .AddParameter("Email", serviceContact.Email.ToString())//"83fe15e1-2896-478c-b4ec-a1996b4e2f46");
                .AddParameter("AdministrativeCopy", serviceContact.AdministrativeCopy.ToString())
                .AddParameter("AddressLine1", serviceContact.AddressLine1.ToString())
                .AddParameter("AddressLine2", serviceContact.AddressLine2.ToString())
                .AddParameter("City", serviceContact.City.ToString())
                .AddParameter("State", serviceContact.State.ToString())
                .AddParameter("ZipCode", serviceContact.ZipCode.ToString())
                .AddParameter("Country", serviceContact.Country.ToString())
                .AddParameter("PhoneNumber", serviceContact.PhoneNumber.ToString())
                .AddParameter("AddByFirmName", serviceContact.AddByFirmName.ToString());


                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("RemoveServiceContact")]
        public async Task<ActionResult> RemoveServiceContact(string ServiceContactID)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RemoveServiceContact");


            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RemoveServiceContact' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                  .AddParameter("UserName", UserName)
                  .AddParameter("Password", Password)
                  .AddParameter("ServiceContactID", ServiceContactID);

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }

        [HttpPost]
        [Authorize]
        [Route("UpdateServiceContact")]
        public async Task<ActionResult> UpdateServiceContact(ServiceContact serviceContact)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateServiceContact");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateServiceContact' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("FirmID", serviceContact.FirmID.ToString())//"900009")
                .AddParameter("ServiceContactID", serviceContact.ServiceContactID.ToString())//"900009")
                .AddParameter("FirstName", serviceContact.FirstName.ToString())//"sam")
                .AddParameter("MiddleName", serviceContact.MiddleName.ToString())//"zac")
                .AddParameter("LastName", serviceContact.LastName.ToString())//"van")
                .AddParameter("Email", serviceContact.Email.ToString())//"83fe15e1-2896-478c-b4ec-a1996b4e2f46");
                .AddParameter("AdministrativeCopy", serviceContact.AdministrativeCopy.ToString())
                .AddParameter("AddressLine1", serviceContact.AddressLine1.ToString())
                .AddParameter("AddressLine2", serviceContact.AddressLine2.ToString())
                .AddParameter("City", serviceContact.City.ToString())
                .AddParameter("State", serviceContact.State.ToString())
                .AddParameter("ZipCode", serviceContact.ZipCode.ToString())
                .AddParameter("Country", serviceContact.Country.ToString())
                .AddParameter("PhoneNumber", serviceContact.PhoneNumber.ToString());


                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("GetPaymentAccountTypeList")]
        public async Task<ActionResult> GetPaymentAccountTypeList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetPaymentAccountTypeList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPaymentAccountTypeList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetPaymentAccountList")]
        public async Task<ActionResult> GetPaymentAccountList()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetPaymentAccountList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPaymentAccountList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("CreatePaymentAccount")]
        public async Task<ActionResult> CreatePaymentAccount(CreatePaymentAccount paymentAccount)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CreatePaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CreatePaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("FirmID", paymentAccount.FirmID.ToString())
                    .AddParameter("AccountName", paymentAccount.AccountName.ToString())
                    .AddParameter("PaymentAccountTypeCodeId", paymentAccount.PaymentAccountTypeCodeId.ToString())
                    .AddParameter("AccountToken", paymentAccount.AccountToken.ToString())
                    .AddParameter("CardType", paymentAccount.CardType.ToString())
                    .AddParameter("CardLast4", paymentAccount.CardLast4.ToString())
                    .AddParameter("CardMonth", paymentAccount.CardMonth.Value)
                    .AddParameter("CardYear", paymentAccount.CardYear.Value)
                    .AddParameter("CardHolderName", paymentAccount.CardHolderName.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetPaymentAccount")]
        public async Task<ActionResult> GetPaymentAccount(PaymentAccountRequest paymentAccount)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetPaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("PaymentAccountID", paymentAccount.PaymentAccountID.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("RemovePaymentAccount")]
        public async Task<ActionResult> RemovePaymentAccount(string PaymentAccountID)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RemovePaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RemovePaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("PaymentAccountID", PaymentAccountID.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("UpdatePaymentAccount")]
        public async Task<ActionResult> UpdatePaymentAccount(PaymentAccount paymentAccount)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdatePaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdatePaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("PaymentAccountID", paymentAccount.PaymentAccountID.ToString())
                    //.AddParameter("PaymentAccountTypeCode", paymentAccount.PaymentAccountTypeCode.ToString())
                    .AddParameter("AccountName", paymentAccount.AccountName.ToString()
                    //.AddParameter("AccountToken", paymentAccount.AccountToken.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("GetPolicy")]
        public async Task<ActionResult> GetPolicy(string Courtlocation)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetPolicy");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPolicy' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString())
                   .AddParameter("Courtlocation", Courtlocation.ToString()
                   );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response.Content);

                string courtelementname;
                foreach (var myjson in myDeserializedClass.RuntimePolicyParameters.CourtCodelist)
                {
                    if (myjson.ECFElementName.Value == "nc:CaseCategoryText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCaseCategoryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:CaseTypeText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:LocationCountryName")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:DamageAmountCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:DisclaimerRequirementCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:BinaryFormatStandardName")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:FilerTypeText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "j:RegisterActionDescriptionText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:BinaryCategoryText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:DocumentOptionalService")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "ecf:CaseParticipantRoleCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:RemedyCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:LocationStateName")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "ecf:FilingStatusCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonNameSuffixText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:DataFieldConfigCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "ecf:ServiceRecipientID/nc:IdentificationSourceText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:BinaryLocationURI")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "j:/ArrestCharge/j:ArrestLocation/nc:LocationName")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:BondTypeText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:Charge/cext:ChargeStatute/j:StatuteLevelText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "j:ArrestOfficial/j:EnforcementOfficialUnit/nc:OrganizationIdentification/nc:IdentificationID")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:DocumentIdentification/nc:IdentificationSourceText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:GeneralOffenseText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:StatuteTypeText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "ecf:PersonDriverLicense/nc:DriverLicenseIdentification/nc:IdentificationCategoryText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:Charge/cext:ChargeStatute/j:StatuteCodeIdentification/nc:IdentificationID")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PrimaryLanguage\\nc:LanguageCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonPhysicalFeature\\nc:PhysicalFeatureCategoryCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonHairColorCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonEyeColorCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonEthnicityText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:PersonRaceText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:CaseSubTypeText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "cext:Charge\\cext:ChargePhaseText")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:VehicleTypeCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "nc:VehicleMakeCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "VehicleColorPrimaryCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "j:Citation\\nc:ActivityIdentification\\tyler:JurisdictionCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:MotionTypeCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:QuestionCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);
                    }
                    else if (myjson.ECFElementName.Value == "tyler:AnswerCode")
                    {
                        courtelementname = myjson.CourtCodelistURI.IdentificationID.Value.ToString();
                        await DownloadCountryCodes(courtelementname);

                    }

                }


                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("GetFilingList")]
        public async Task<ActionResult> GetFilingList(FilingListRequest listRequest)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetFilingList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetFilingList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("Courtlocation", listRequest.courtlocation.ToString())
                    .AddParameter("userId", listRequest.userId.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("NotificationPreferences")]
        public async Task<ActionResult> GetNotificationPreferences(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:NotificationPreferences");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:NotificationPreferences' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("UpdateNotificationPreferences")]
        public async Task<ActionResult> UpdateNotificationPreferences(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateNotificationPreferences");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateNotificationPreferences' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Route("GetNotificationPreferencesList")]
        public async Task<ActionResult> GetNotificationPreferencesList(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetNotificationPreferencesList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetNotificationPreferencesList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Route("GetUserRequest")]
        public async Task<ActionResult> GetUserRequest(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92", string UserID = "2d00a052-3290-492c-ae04-887be3f7a5b4")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetUserRequest");


            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetUserRequest' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString())
                   .AddParameter("UserID", UserID.ToString()
                   );

                //var request = new RestRequest(Method.Get.ToString()).AddParameter("UserID", UserID.ToString());

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }

        [HttpPost]
        [Route("RemoveUser")]
        public async Task<ActionResult> RemoveUser(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92", string UserID = "2d00a052-3290-492c-ae04-887be3f7a5b4")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RemoveUser");


            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RemoveUser' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }


            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString())
                   .AddParameter("UserID", UserID.ToString()
                   );

                //var request = new RestRequest(Method.Get.ToString()).AddParameter("UserID", UserID.ToString());

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }


        }

        [HttpPost]
        [Route("AuthenticateUser")]
        public async Task<ActionResult> AuthenticateUser(string Email = "ghorpadesumit471@gmail.com", string Password = "Sug123!@")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:AuthenticateUser");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:AuthenticateUser' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("Email", Email.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("UpdateUserUserService")]
        public async Task<ActionResult> UpdateUserUserService(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92",
                                                              string Email = "ghorpadesumit471@gmail.com", string UserID = "d8cdd3b4-6965-4b4a-8b42-7af925271afc",
                                                              string FirstName = "Ujjwal", string MiddleName = "Deoraoji", string LastName = "Nikam")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateUserUserService");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateUserUserService' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("Email", Email)//"1a1d114b-f751-49ce-bec3-5379dfe45b18")
                .AddParameter("UserID", UserID)//"900009")
                .AddParameter("FirstName", FirstName)//"sam")
                .AddParameter("MiddleName", MiddleName)//"zac")
                .AddParameter("LastName", LastName);//"van")               

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        [HttpPost]
        [Authorize]
        [Route("UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUser updateUser)
        //string Email = "ghorpadesumit471@gmail.com", 
        //string FirmID = "83fe15e1-2896-478c-b4ec-a1996b4e2f46",
        //string FirstName = "Sumit", string MiddleName = "S", 
        //string LastName = "Ghorpade",
        //string UserID = "d8cdd3b4-6965-4b4a-8b42-7af925271afc"
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateUser");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateUser' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest("Token is missing or invalid.");

            // Extract the token (remove "Bearer " prefix)
            var token = authHeader.Substring("Bearer ".Length);
            var UserName = _userService.GetEmailFromToken(token);

            // Query database for password hash
            var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("Email", updateUser.Email)
                .AddParameter("FirmID", updateUser.FirmID)
                .AddParameter("FirstName", updateUser.FirstName)
                .AddParameter("MiddleName", updateUser.MiddleName)
                .AddParameter("LastName", updateUser.LastName)
                .AddParameter("UserID", updateUser.UserID);
                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        [HttpPost]
        [Authorize]
        [Route("UpdateFirm")]
        public async Task<ActionResult> UpdateFirm(UpdateFirm updateFirm)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:UpdateFirm");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:UpdateFirm' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest("Token is missing or invalid.");

            // Extract the token (remove "Bearer " prefix)
            var token = authHeader.Substring("Bearer ".Length);
            var UserName = _userService.GetEmailFromToken(token);

            // Query database for password hash
            var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("FirmName", updateFirm.FirmName)
                .AddParameter("FirmID", updateFirm.FirmID)
                .AddParameter("AddressLine1", updateFirm.AddressLine1)
                .AddParameter("AddressLine2", updateFirm.AddressLine2)
                .AddParameter("City", updateFirm.City)
                .AddParameter("State", updateFirm.State)
                .AddParameter("ZipCode", updateFirm.ZipCode)
                .AddParameter("Country", updateFirm.Country)
                .AddParameter("PhoneNumber", updateFirm.PhoneNumber);
                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        [HttpPost]
        [Route("GetPublicList")]
        public async Task<ActionResult> GetPublicList(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92",
                                                              string Email = "ghorpadesumit471@gmail.com", string FirmName = "ASPL",
                                                              string FirstName = "zahoor", string LastName = "ahmed")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetPublicList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPublicList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("Email", Email)//"1a1d114b-f751-49ce-bec3-5379dfe45b18")
                .AddParameter("FirmName", FirmName)//"900009")
                .AddParameter("FirstName", FirstName)//"sam")     
                .AddParameter("LastName", LastName);//"van")               

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        [HttpPost]
        [Route("GetDocument")]
        public async Task<ActionResult> GetDocument(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92", string CaseTrackingID = "dbed64e6-2379-4290-87b8-a42f958a3e39", string CaseDocketID = "227702")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetDocument");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetPolicy' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("UserName", UserName.ToString())
                   .AddParameter("Password", Password.ToString())
                   .AddParameter("CaseTrackingID", CaseTrackingID.ToString())
                    .AddParameter("CaseDocketID", CaseDocketID.ToString()
                   );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        //[HttpPost]
        //[Authorize]
        //[Route("GetFeesCalculation")]
        //public async Task<ActionResult> GetFeesCalculation()
        //{
        //    CommonResponse resp = new CommonResponse();

        //    string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
        //    string url = ip + _configuration.GetValue<string>("Tyler:GetFeesCalculation");

        //    if (String.IsNullOrWhiteSpace(url))
        //    {
        //        resp.success = false;
        //        resp.message = "'Tyler:GetFeesCalculation' is null or empty.";
        //        resp.data = null;

        //        return Ok(resp);
        //    }

        //    try
        //    {
        //        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        //        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        //            return BadRequest("Token is missing or invalid.");

        //        // Extract the token (remove "Bearer " prefix)
        //        var token = authHeader.Substring("Bearer ".Length);
        //        var UserName = _userService.GetEmailFromToken(token);

        //        // Query database for password hash
        //        var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

        //        var client = new RestClient(url);

        //        //var request = new RestRequest(Method.Get.ToString());
        //        var request = new RestRequest(Method.Get.ToString())
        //            .AddParameter("UserName", UserName.ToString())
        //            .AddParameter("Password", Password.ToString()
        //            );

        //        RestResponse response = await client.ExecuteAsync(request);

        //        //new logic for sending param to tyler api
        //        //var restRequest = new RestRequest(Method.Get.ToString());
        //        //restRequest.AddHeader("Content-Type", "application/json");

        //        //// Serialize the request object to JSON
        //        //var requestBody = JsonConvert.SerializeObject(request);
        //        //restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

        //        //var responseq = await client.ExecuteAsync(restRequest);


        //        object jsonData = JsonConvert.DeserializeObject(response.Content);

        //        resp.success = true;
        //        resp.status = 200;
        //        resp.message = "Success!";
        //        resp.data = jsonData;

        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.success = false;
        //        resp.message = "Failed! " + ex.ToString();
        //        resp.data = null;
        //        return Ok(resp);
        //    }

        //}

        [HttpPost]
        [Authorize]
        [Route("GetFeesCalculation")]
        public async Task<ActionResult> GetFeesCalculation(FeesCalculationRequest calculationRequest)
        {

            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetFeesCalculation");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetFeesCalculation' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(calculationRequest);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);


                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetSubSequentFeesCalculation")]
        public async Task<ActionResult> GetSubSequentFeesCalculation(SubSequentFeeCalRequest subseqcalRequest)
        {

            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetSubSequentFeesCalculation");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetSubSequentFeesCalculation' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(subseqcalRequest);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);


                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("ServiceInformationHistory")]
        public async Task<ActionResult> GetServiceInformationHistory(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ServiceInformationHistory");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ServiceInformationHistory' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("ServiceAttachCaseList")]
        public async Task<ActionResult> GetServiceAttachCaseList(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ServiceAttachCaseList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ServiceAttachCaseList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("GetServiceInformation")]
        public async Task<ActionResult> GetServiceInformation(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ServiceInformation");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ServiceInformation' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("CreateGlobalPaymentAccount")]
        public async Task<ActionResult> CreateGlobalPaymentAccount(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CreateGlobalPaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CreateGlobalPaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("GlobalPaymentAccount")]
        public async Task<ActionResult> GetGlobalPaymentAccount(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GlobalPaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GlobalPaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("GlobalPaymentAccountList")]
        public async Task<ActionResult> GetGlobalPaymentAccountList(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GlobalPaymentAccountList");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GlobalPaymentAccountList' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("RemoveGlobalPaymentAccount")]
        public async Task<ActionResult> RemoveGlobalPaymentAccount(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:RemoveGlobalPaymentAccount");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:RemoveGlobalPaymentAccount' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("FilingDetails")]
        public async Task<ActionResult> GetFilingDetails(FilingDetails filingDetails)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:FilingDetails");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:FilingDetails' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password)
                    .AddParameter("filingId", filingDetails.filingId.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("FilingStatus")]
        public async Task<ActionResult> GetFilingStatus(FilingDetails filingDetails)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:FilingStatus");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:FilingStatus' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName)
                    .AddParameter("Password", Password)
                    .AddParameter("filingId", filingDetails.filingId.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("CancelFiling")]
        public async Task<ActionResult> CancelFiling(string CourtLocation, string FilingId)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CancelFiling");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CancelFiling' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("CourtLocation", CourtLocation.ToString())
                    .AddParameter("FilingId", FilingId.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("FilingService")]
        public async Task<ActionResult> GetFilingService(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:FilingService");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:FilingService' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("ServiceTypes")]
        public async Task<ActionResult> GetServiceTypes(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ServiceTypes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ServiceTypes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("NotifyCaseAssignmentComplete")]
        public async Task<ActionResult> NotifyCaseAssignmentComplete(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:NotifyCaseAssignmentComplete");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:NotifyCaseAssignmentComplete' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Route("SecureCase")]
        public async Task<ActionResult> SecureCase(string UserName = "ghorpadesumit471@gmail.com", string Password = "175803a9-f490-4645-b76f-c9337210ff92")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:SecureCase");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:SecureCase' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Route("DownloadCourtLocations")]
        public async Task<ActionResult> DownloadCourtLocations()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CourtLocations");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CourtLocations' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtLocations.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtLocations\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtLocations\locations.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {

                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var initial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "initial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var subsequent = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "subsequent").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var disallowcopyingenvelopemultipletimes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "disallowcopyingenvelopemultipletimes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowfilingintononindexedcase = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowfilingintononindexedcase").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowablecardtypes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowablecardtypes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var odysseynodeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "odysseynodeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var cmsid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "cmsid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var sendservicebeforereview = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "sendservicebeforereview").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var parentnodeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "parentnodeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var iscounty = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "iscounty").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var restrictbankaccountpayment = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "restrictbankaccountpayment").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowmultipleattorneys = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowmultipleattorneys").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var sendservicecontactremovednotifications = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "sendservicecontactremovednotifications").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowmaxfeeamount = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowmaxfeeamount").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var transferwaivedfeestocms = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "transferwaivedfeestocms").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var skippreauth = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "skippreauth").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowhearing = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowhearing").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowreturndate = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowreturndate").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var showdamageamount = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "showdamageamount").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var hasconditionalservicetypes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "hasconditionalservicetypes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var hasprotectedcasetypes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "hasprotectedcasetypes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var protectedcasetypes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "protectedcasetypes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowzerofeeswithoutfilingparty = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowzerofeeswithoutfilingparty").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowserviceoninitial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowserviceoninitial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowaddservicecontactsoninitial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowaddservicecontactsoninitial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowredaction = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowredaction").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactionurl = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactionurl").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactionviewerurl = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactionviewerurl").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactionapiversion = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactionapiversion").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var enforceredaction = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "enforceredaction").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactiondocumenttype = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactiondocumenttype").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var defaultdocumentdescription = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "defaultdocumentdescription").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowwaiveronmail = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowwaiveronmail").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var showreturnonreject = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "showreturnonreject").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var protectedcasereplacementstring = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "protectedcasereplacementstring").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowchargeupdate = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowchargeupdate").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowpartyid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowpartyid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactionfee = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactionfee").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowwaiveronredaction = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowwaiveronredaction").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var disallowelectronicserviceonnewcontacts = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "disallowelectronicserviceonnewcontacts").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowindividualregistration = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowindividualregistration").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var redactiontargetconfiguration = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "redactiontargetconfiguration").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var bulkfilingfeeassessorconfiguration = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "bulkfilingfeeassessorconfiguration").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var envelopelevelcommentconfiguration = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "envelopelevelcommentconfiguration").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var autoassignsrlservicecontact = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "autoassignsrlservicecontact").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var autoassignattorneyservicecontact = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "autoassignattorneyservicecontact").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var partialwaiverdurationindays = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "partialwaiverdurationindays").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var partialwaivercourtpaymentsystemurl = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "partialwaivercourtpaymentsystemurl").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var partialwaiveravailablewaiverpercentages = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "partialwaiveravailablewaiverpercentages").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowrepcap = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowrepcap").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsentenabled = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsentenabled").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextblurbmain = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextblurbmain").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextblurbsecondary = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextblurbsecondary").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextblurbsecondaryafterconsentyes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextblurbsecondaryafterconsentyes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentyes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentyes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentyeshelp = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentyeshelp").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentyeswithadd = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentyeswithadd").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentyeswithaddhelp = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentyeswithaddhelp").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentno = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentno").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var eserviceconsenttextconsentnohelp = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "eserviceconsenttextconsentnohelp").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var promptforconfidentialdocumentsenabled = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "promptforconfidentialdocumentsenabled").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var promptforconfidentialdocuments = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "promptforconfidentialdocuments").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var defaultdocumentsecurityenabled = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "defaultdocumentsecurityenabled").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var defaultdocumentsecurity = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "defaultdocumentsecurity").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var cmsservicecontactsupdatesenabled = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "cmsservicecontactsupdatesenabled").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var cmsservicecontactsupdatesfirmid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "cmsservicecontactsupdatesfirmid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var cmsservicecontactsupdatesbehavior = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "cmsservicecontactsupdatesbehavior").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var subsequentactionsenabled = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "subsequentactionsenabled").Elements("SimpleValue").FirstOrDefault()?.Value;


                    //if (initial == "false" && subsequent == "false")
                    //{
                    //    await InsertCourtLocationsAsync(code, name, initial, subsequent, disallowcopyingenvelopemultipletimes, allowfilingintononindexedcase, allowablecardtypes, odysseynodeid, cmsid, sendservicebeforereview, parentnodeid, iscounty, restrictbankaccountpayment, allowmultipleattorneys, sendservicecontactremovednotifications, allowmaxfeeamount, transferwaivedfeestocms, skippreauth, allowhearing, allowreturndate, showdamageamount, hasconditionalservicetypes, hasprotectedcasetypes, protectedcasetypes, allowzerofeeswithoutfilingparty, allowserviceoninitial, allowaddservicecontactsoninitial, allowredaction, redactionurl, redactionviewerurl, redactionapiversion, enforceredaction, redactiondocumenttype, defaultdocumentdescription, allowwaiveronmail, showreturnonreject, protectedcasereplacementstring, allowchargeupdate, allowpartyid, redactionfee, allowwaiveronredaction, disallowelectronicserviceonnewcontacts, allowindividualregistration, redactiontargetconfiguration, bulkfilingfeeassessorconfiguration, envelopelevelcommentconfiguration, autoassignsrlservicecontact, autoassignattorneyservicecontact, partialwaiverdurationindays, partialwaivercourtpaymentsystemurl, partialwaiveravailablewaiverpercentages, allowrepcap, eserviceconsentenabled, eserviceconsenttextblurbmain, eserviceconsenttextblurbsecondary, eserviceconsenttextblurbsecondaryafterconsentyes, eserviceconsenttextconsentyes, eserviceconsenttextconsentyeshelp, eserviceconsenttextconsentyeswithadd, eserviceconsenttextconsentyeswithaddhelp, eserviceconsenttextconsentno, eserviceconsenttextconsentnohelp, promptforconfidentialdocumentsenabled, promptforconfidentialdocuments, defaultdocumentsecurityenabled, defaultdocumentsecurity, cmsservicecontactsupdatesenabled, cmsservicecontactsupdatesfirmid, cmsservicecontactsupdatesbehavior, codeKey);
                    //}

                    await InsertCourtLocationsAsync(code, name, initial, subsequent, disallowcopyingenvelopemultipletimes, allowfilingintononindexedcase, allowablecardtypes, odysseynodeid, cmsid, sendservicebeforereview, parentnodeid, iscounty, restrictbankaccountpayment, allowmultipleattorneys, sendservicecontactremovednotifications, allowmaxfeeamount, transferwaivedfeestocms, skippreauth, allowhearing, allowreturndate, showdamageamount, hasconditionalservicetypes, hasprotectedcasetypes, protectedcasetypes, allowzerofeeswithoutfilingparty, allowserviceoninitial, allowaddservicecontactsoninitial, allowredaction, redactionurl, redactionviewerurl, redactionapiversion, enforceredaction, redactiondocumenttype, defaultdocumentdescription, allowwaiveronmail, showreturnonreject, protectedcasereplacementstring, allowchargeupdate, allowpartyid, redactionfee, allowwaiveronredaction, disallowelectronicserviceonnewcontacts, allowindividualregistration, redactiontargetconfiguration, bulkfilingfeeassessorconfiguration, envelopelevelcommentconfiguration, autoassignsrlservicecontact, autoassignattorneyservicecontact, partialwaiverdurationindays, partialwaivercourtpaymentsystemurl, partialwaiveravailablewaiverpercentages, allowrepcap, eserviceconsentenabled, eserviceconsenttextblurbmain, eserviceconsenttextblurbsecondary, eserviceconsenttextblurbsecondaryafterconsentyes, eserviceconsenttextconsentyes, eserviceconsenttextconsentyeshelp, eserviceconsenttextconsentyeswithadd, eserviceconsenttextconsentyeswithaddhelp, eserviceconsenttextconsentno, eserviceconsenttextconsentnohelp, promptforconfidentialdocumentsenabled, promptforconfidentialdocuments, defaultdocumentsecurityenabled, defaultdocumentsecurity, cmsservicecontactsupdatesenabled, cmsservicecontactsupdatesfirmid, cmsservicecontactsupdatesbehavior);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);
                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtLocationsAsync(string code, string name, string initial, string subsequent, string disallowcopyingenvelopemultipletimes, string allowfilingintononindexedcase, string allowablecardtypes, string odysseynodeid, string cmsid, string sendservicebeforereview, string parentnodeid, string iscounty, string restrictbankaccountpayment, string allowmultipleattorneys, string sendservicecontactremovednotifications, string allowmaxfeeamount, string transferwaivedfeestocms, string skippreauth, string allowhearing, string allowreturndate, string showdamageamount, string hasconditionalservicetypes, string hasprotectedcasetypes, string protectedcasetypes, string allowzerofeeswithoutfilingparty, string allowserviceoninitial, string allowaddservicecontactsoninitial, string allowredaction, string redactionurl, string redactionviewerurl, string redactionapiversion, string enforceredaction, string redactiondocumenttype, string defaultdocumentdescription, string allowwaiveronmail, string showreturnonreject, string protectedcasereplacementstring, string allowchargeupdate, string allowpartyid, string redactionfee, string allowwaiveronredaction, string disallowelectronicserviceonnewcontacts, string allowindividualregistration, string redactiontargetconfiguration, string bulkfilingfeeassessorconfiguration, string envelopelevelcommentconfiguration, string autoassignsrlservicecontact, string autoassignattorneyservicecontact, string partialwaiverdurationindays, string partialwaivercourtpaymentsystemurl, string partialwaiveravailablewaiverpercentages, string allowrepcap, string eserviceconsentenabled, string eserviceconsenttextblurbmain, string eserviceconsenttextblurbsecondary, string eserviceconsenttextblurbsecondaryafterconsentyes, string eserviceconsenttextconsentyes, string eserviceconsenttextconsentyeshelp, string eserviceconsenttextconsentyeswithadd, string eserviceconsenttextconsentyeswithaddhelp, string eserviceconsenttextconsentno, string eserviceconsenttextconsentnohelp, string promptforconfidentialdocumentsenabled, string promptforconfidentialdocuments, string defaultdocumentsecurityenabled, string defaultdocumentsecurity, string cmsservicecontactsupdatesenabled, string cmsservicecontactsupdatesfirmid, string cmsservicecontactsupdatesbehavior)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    string checkExistenceQuery = $"SELECT EXISTS (SELECT 1 FROM \"CourtLocations\" WHERE \"code\" = '{@code}')";
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"CourtLocations\"(code,name,initial,subsequent,disallowcopyingenvelopemultipletimes,allowfilingintononindexedcase,allowablecardtypes,odysseynodeid,cmsid,sendservicebeforereview,parentnodeid,iscounty,restrictbankaccountpayment,allowmultipleattorneys,sendservicecontactremovednotifications,allowmaxfeeamount,transferwaivedfeestocms,skippreauth,allowhearing,allowreturndate,showdamageamount,hasconditionalservicetypes,hasprotectedcasetypes,protectedcasetypes,allowzerofeeswithoutfilingparty,allowserviceoninitial,allowaddservicecontactsoninitial,allowredaction,redactionurl,redactionviewerurl,redactionapiversion,enforceredaction,redactiondocumenttype,defaultdocumentdescription,allowwaiveronmail,showreturnonreject,protectedcasereplacementstring,allowchargeupdate,allowpartyid,redactionfee,allowwaiveronredaction,disallowelectronicserviceonnewcontacts,allowindividualregistration,redactiontargetconfiguration,bulkfilingfeeassessorconfiguration,envelopelevelcommentconfiguration,autoassignsrlservicecontact,autoassignattorneyservicecontact,partialwaiverdurationindays,partialwaivercourtpaymentsystemurl,partialwaiveravailablewaiverpercentages,allowrepcap,eserviceconsentenabled,eserviceconsenttextblurbmain,eserviceconsenttextblurbsecondary,eserviceconsenttextblurbsecondaryafterconsentyes,eserviceconsenttextconsentyes, eserviceconsenttextconsentyeshelp,eserviceconsenttextconsentyeswithadd,eserviceconsenttextconsentyeswithaddhelp,eserviceconsenttextconsentno,eserviceconsenttextconsentnohelp,promptforconfidentialdocumentsenabled,promptforconfidentialdocuments,defaultdocumentsecurityenabled,defaultdocumentsecurity,cmsservicecontactsupdatesenabled,cmsservicecontactsupdatesfirmid,cmsservicecontactsupdatesbehavior) VALUES (@code,@name,@initial,@subsequent,@disallowcopyingenvelopemultipletimes,@allowfilingintononindexedcase,@allowablecardtypes,@odysseynodeid,@cmsid,@sendservicebeforereview,@parentnodeid,@iscounty,@restrictbankaccountpayment,@allowmultipleattorneys,@sendservicecontactremovednotifications,@allowmaxfeeamount,@transferwaivedfeestocms,@skippreauth,@allowhearing,@allowreturndate,@showdamageamount,@hasconditionalservicetypes,@hasprotectedcasetypes,@protectedcasetypes,@allowzerofeeswithoutfilingparty,@allowserviceoninitial,@allowaddservicecontactsoninitial,@allowredaction,@redactionurl,@redactionviewerurl,@redactionapiversion,@enforceredaction,@redactiondocumenttype,@defaultdocumentdescription,@allowwaiveronmail,@showreturnonreject,@protectedcasereplacementstring,@allowchargeupdate,@allowpartyid,@redactionfee ,@allowwaiveronredaction,@disallowelectronicserviceonnewcontacts ,@allowindividualregistration ,@redactiontargetconfiguration,@bulkfilingfeeassessorconfiguration,@envelopelevelcommentconfiguration,@autoassignsrlservicecontact,@autoassignattorneyservicecontact ,@partialwaiverdurationindays,@partialwaivercourtpaymentsystemurl ,@partialwaiveravailablewaiverpercentages,@allowrepcap ,@eserviceconsentenabled ,@eserviceconsenttextblurbmain ,@eserviceconsenttextblurbsecondary ,@eserviceconsenttextblurbsecondaryafterconsentyes,@eserviceconsenttextconsentyes ,@eserviceconsenttextconsentyeshelp ,@eserviceconsenttextconsentyeswithadd,@eserviceconsenttextconsentyeswithaddhelp,@eserviceconsenttextconsentno ,@eserviceconsenttextconsentnohelp ,@promptforconfidentialdocumentsenabled ,@promptforconfidentialdocuments ,@defaultdocumentsecurityenabled ,@defaultdocumentsecurity,@cmsservicecontactsupdatesenabled ,@cmsservicecontactsupdatesfirmid ,@cmsservicecontactsupdatesbehavior)";

                    using (var command = new NpgsqlCommand(checkExistenceQuery, conn))
                    {
                        // Add the courtLocExists parameter to the command
                        command.Parameters.AddWithValue("code", @code);

                        // Execute the query to check if the courtLoc Exists 
                        //var courtLocExists = (bool)command.ExecuteScalar();
                        bool courtLocExists = (bool)await command.ExecuteScalarAsync();

                        if (courtLocExists)
                        {
                            Console.WriteLine("Court location already exists. Insertion aborted.");
                        }
                        else
                        {
                            // If the courtLoc Exists does not exist, insert the new location

                            using (var cmd = new NpgsqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("code", code);
                                cmd.Parameters.AddWithValue("name", name);
                                cmd.Parameters.AddWithValue("initial", initial);
                                cmd.Parameters.AddWithValue("subsequent", subsequent);
                                cmd.Parameters.AddWithValue("disallowcopyingenvelopemultipletimes", disallowcopyingenvelopemultipletimes);
                                cmd.Parameters.AddWithValue("allowfilingintononindexedcase", allowfilingintononindexedcase);
                                cmd.Parameters.AddWithValue("allowablecardtypes", allowablecardtypes);
                                cmd.Parameters.AddWithValue("odysseynodeid", odysseynodeid);
                                cmd.Parameters.AddWithValue("cmsid", cmsid);
                                cmd.Parameters.AddWithValue("sendservicebeforereview", sendservicebeforereview);
                                cmd.Parameters.AddWithValue("parentnodeid", parentnodeid);
                                cmd.Parameters.AddWithValue("iscounty", iscounty);
                                cmd.Parameters.AddWithValue("restrictbankaccountpayment", restrictbankaccountpayment);
                                cmd.Parameters.AddWithValue("allowmultipleattorneys", allowmultipleattorneys);
                                cmd.Parameters.AddWithValue("sendservicecontactremovednotifications", sendservicecontactremovednotifications);
                                cmd.Parameters.AddWithValue("allowmaxfeeamount", allowmaxfeeamount);
                                cmd.Parameters.AddWithValue("transferwaivedfeestocms", transferwaivedfeestocms);
                                cmd.Parameters.AddWithValue("skippreauth", skippreauth);
                                cmd.Parameters.AddWithValue("allowhearing", allowhearing);
                                cmd.Parameters.AddWithValue("allowreturndate", allowreturndate);
                                cmd.Parameters.AddWithValue("showdamageamount", showdamageamount);
                                cmd.Parameters.AddWithValue("hasconditionalservicetypes", hasconditionalservicetypes);
                                cmd.Parameters.AddWithValue("hasprotectedcasetypes", hasprotectedcasetypes);
                                cmd.Parameters.AddWithValue("protectedcasetypes", protectedcasetypes);
                                cmd.Parameters.AddWithValue("allowzerofeeswithoutfilingparty", allowzerofeeswithoutfilingparty);
                                cmd.Parameters.AddWithValue("allowserviceoninitial", allowserviceoninitial);
                                cmd.Parameters.AddWithValue("allowaddservicecontactsoninitial", allowaddservicecontactsoninitial);
                                cmd.Parameters.AddWithValue("allowredaction", allowredaction);
                                cmd.Parameters.AddWithValue("redactionurl", redactionurl);
                                cmd.Parameters.AddWithValue("redactionviewerurl", redactionviewerurl);
                                cmd.Parameters.AddWithValue("redactionapiversion", redactionapiversion);
                                cmd.Parameters.AddWithValue("enforceredaction", enforceredaction);
                                cmd.Parameters.AddWithValue("redactiondocumenttype", redactiondocumenttype);
                                cmd.Parameters.AddWithValue("defaultdocumentdescription", defaultdocumentdescription);
                                cmd.Parameters.AddWithValue("allowwaiveronmail", allowwaiveronmail);
                                cmd.Parameters.AddWithValue("showreturnonreject", showreturnonreject);
                                cmd.Parameters.AddWithValue("protectedcasereplacementstring", protectedcasereplacementstring);
                                cmd.Parameters.AddWithValue("allowchargeupdate", allowchargeupdate);
                                cmd.Parameters.AddWithValue("allowpartyid", allowpartyid);
                                cmd.Parameters.AddWithValue("redactionfee", redactionfee);
                                cmd.Parameters.AddWithValue("allowwaiveronredaction", allowwaiveronredaction);
                                cmd.Parameters.AddWithValue("disallowelectronicserviceonnewcontacts", disallowelectronicserviceonnewcontacts);
                                cmd.Parameters.AddWithValue("allowindividualregistration", allowindividualregistration);
                                cmd.Parameters.AddWithValue("redactiontargetconfiguration", redactiontargetconfiguration);
                                cmd.Parameters.AddWithValue("bulkfilingfeeassessorconfiguration", bulkfilingfeeassessorconfiguration);
                                cmd.Parameters.AddWithValue("envelopelevelcommentconfiguration", envelopelevelcommentconfiguration);
                                cmd.Parameters.AddWithValue("autoassignsrlservicecontact", autoassignsrlservicecontact);
                                cmd.Parameters.AddWithValue("autoassignattorneyservicecontact", autoassignattorneyservicecontact);
                                cmd.Parameters.AddWithValue("partialwaiverdurationindays", partialwaiverdurationindays);
                                cmd.Parameters.AddWithValue("partialwaivercourtpaymentsystemurl", partialwaivercourtpaymentsystemurl);
                                cmd.Parameters.AddWithValue("partialwaiveravailablewaiverpercentages", partialwaiveravailablewaiverpercentages);
                                cmd.Parameters.AddWithValue("allowrepcap", allowrepcap);
                                cmd.Parameters.AddWithValue("eserviceconsentenabled", eserviceconsentenabled);
                                cmd.Parameters.AddWithValue("eserviceconsenttextblurbmain", eserviceconsenttextblurbmain);
                                cmd.Parameters.AddWithValue("eserviceconsenttextblurbsecondary", eserviceconsenttextblurbsecondary);
                                cmd.Parameters.AddWithValue("eserviceconsenttextblurbsecondaryafterconsentyes", eserviceconsenttextblurbsecondaryafterconsentyes);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentyes", eserviceconsenttextconsentyes);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentyeshelp", eserviceconsenttextconsentyeshelp);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentyeswithadd", eserviceconsenttextconsentyeswithadd);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentyeswithaddhelp", eserviceconsenttextconsentyeswithaddhelp);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentno", eserviceconsenttextconsentno);
                                cmd.Parameters.AddWithValue("eserviceconsenttextconsentnohelp", eserviceconsenttextconsentnohelp);
                                cmd.Parameters.AddWithValue("promptforconfidentialdocumentsenabled", promptforconfidentialdocumentsenabled);
                                cmd.Parameters.AddWithValue("promptforconfidentialdocuments", promptforconfidentialdocuments);
                                cmd.Parameters.AddWithValue("defaultdocumentsecurityenabled", defaultdocumentsecurityenabled);
                                cmd.Parameters.AddWithValue("defaultdocumentsecurity", defaultdocumentsecurity);
                                cmd.Parameters.AddWithValue("cmsservicecontactsupdatesenabled", cmsservicecontactsupdatesenabled);
                                cmd.Parameters.AddWithValue("cmsservicecontactsupdatesfirmid", cmsservicecontactsupdatesfirmid);
                                cmd.Parameters.AddWithValue("cmsservicecontactsupdatesbehavior", cmsservicecontactsupdatesbehavior);

                                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("Court locations inserted successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Insertion failed.");
                                }

                            }

                        }
                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }


        [HttpPost]
        [Route("DownloadCourtFilingCodes")]
        public async Task<ActionResult> DownloadCourtFilingCodes()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CourtFilingCodes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CourtFilingCodes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtFilingCodes.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtFilingCodes\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtFilingCodes\filingcodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var fee = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "fee").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var casecategory = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "casecategory").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var casetypeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "casetypeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var filingtype = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "filingtype").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var iscourtuseonly = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "iscourtuseonly").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var civilclaimamount = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "civilclaimamount").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var probateestateamount = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "probateestateamount").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var amountincontroversy = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "amountincontroversy").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var useduedate = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "useduedate").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var isproposedorder = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "isproposedorder").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var excludecertificateofservice = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "excludecertificateofservice").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var iswaiverrequest = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "iswaiverrequest").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtFilingCodesAsync(code, name, fee, casecategory, casetypeid, filingtype, iscourtuseonly, civilclaimamount, probateestateamount, amountincontroversy, useduedate, isproposedorder, excludecertificateofservice, iswaiverrequest, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtFilingCodesAsync(string code, string name, string fee, string casecategory, string casetypeid, string filingtype, string iscourtuseonly, string civilclaimamount, string probateestateamount, string amountincontroversy, string useduedate, string isproposedorder, string excludecertificateofservice, string iswaiverrequest, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Filingcodes\"(code, name, fee, casecategory, casetypeid, filingtype, iscourtuseonly, civilclaimamount, probateestateamount, amountincontroversy, useduedate, isproposedorder, excludecertificateofservice, iswaiverrequest, efspcode) VALUES (@code,@name,@fee,@casecategory,@casetypeid,@filingtype,@iscourtuseonly,@civilclaimamount,@probateestateamount,@amountincontroversy,@useduedate,@isproposedorder,@excludecertificateofservice,@iswaiverrequest,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("fee", fee);
                        cmd.Parameters.AddWithValue("casecategory", casecategory);
                        cmd.Parameters.AddWithValue("casetypeid", casetypeid);
                        cmd.Parameters.AddWithValue("filingtype", filingtype);
                        cmd.Parameters.AddWithValue("iscourtuseonly", iscourtuseonly);
                        cmd.Parameters.AddWithValue("civilclaimamount", civilclaimamount);
                        cmd.Parameters.AddWithValue("probateestateamount", probateestateamount);
                        cmd.Parameters.AddWithValue("amountincontroversy", amountincontroversy);
                        cmd.Parameters.AddWithValue("useduedate", useduedate);
                        cmd.Parameters.AddWithValue("isproposedorder", isproposedorder);
                        cmd.Parameters.AddWithValue("excludecertificateofservice", excludecertificateofservice);
                        cmd.Parameters.AddWithValue("iswaiverrequest", iswaiverrequest);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("filing codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }


        [HttpPost]
        [Route("DownloadCaseCategoryCodes")]
        public async Task<ActionResult> DownloadCaseCategoryCodes(string CourtCodelistURI)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CaseCategoryCodes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseCategoryCodes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CaseCategoryCodes.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CaseCategoryCodes\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString())
                   .AddParameter("CourtCodelistURI", CourtCodelistURI.ToString());

                RestResponse response = await client.ExecuteAsync(request);
                //await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CaseCategoryCodes\casecategorycodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var ecfcasetype = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "ecfcasetype").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var procedureremedyinitial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "procedureremedyinitial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var procedureremedysubsequent = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "procedureremedysubsequent").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var damageamountinitial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "damageamountinitial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var damageamountsubsequent = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "damageamountsubsequent").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCaseCategoryCodesAsync(code, name, ecfcasetype, procedureremedyinitial, procedureremedysubsequent, damageamountinitial, damageamountsubsequent, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCaseCategoryCodesAsync(string code, string name, string ecfcasetype, string procedureremedyinitial, string procedureremedysubsequent, string damageamountinitial, string damageamountsubsequent, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"CaseCategoryCodes\"(code, name, ecfcasetype, procedureremedyinitial, procedureremedysubsequent, damageamountinitial, damageamountsubsequent, efspcode) VALUES (@code,@name,@ecfcasetype,@procedureremedyinitial,@procedureremedysubsequent,@damageamountinitial,@damageamountsubsequent,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("ecfcasetype", ecfcasetype);
                        cmd.Parameters.AddWithValue("procedureremedyinitial", procedureremedyinitial);
                        cmd.Parameters.AddWithValue("procedureremedysubsequent", procedureremedysubsequent);
                        cmd.Parameters.AddWithValue("damageamountinitial", damageamountinitial);
                        cmd.Parameters.AddWithValue("damageamountsubsequent", damageamountsubsequent);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Case Category codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }


        [HttpPost]
        [Route("DownloadCaseTypeCodes")]
        public async Task<ActionResult> DownloadCaseTypeCodes()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CaseTypeCodes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseTypeCodes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CaseTypeCodes\casetypecodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var casecategory = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "casecategory").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var initial = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "initial").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var fee = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "fee").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var willfileddate = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "willfileddate").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var casestreetaddress = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "casestreetaddress").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertcasetypecodesAsync(code, name, casecategory, initial, fee, willfileddate, casestreetaddress, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertcasetypecodesAsync(string code, string name, string casecategory, string initial, string fee, string willfileddate, string casestreetaddress, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"CaseTypeCodes\"(code, name, casecategory, initial, fee, willfileddate, casestreetaddress, efspcode) VALUES (@code,@name,@casecategory,@initial,@fee,@willfileddate,@casestreetaddress,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("casecategory", casecategory);
                        cmd.Parameters.AddWithValue("initial", initial);
                        cmd.Parameters.AddWithValue("fee", fee);
                        cmd.Parameters.AddWithValue("willfileddate", willfileddate);
                        cmd.Parameters.AddWithValue("casestreetaddress", casestreetaddress);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Case Type codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCountryCodes")]
        public async Task<ActionResult> DownloadCountryCodes(string CourtCodelistURI)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CountryCodes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CountryCodes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                  .AddParameter("CourtCodelistURI", CourtCodelistURI.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CountryCodes\countrycodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;

                    await InsertcountrycodesAsync(code, name);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertcountrycodesAsync(string code, string name)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"CountryCodes\"(code, name) VALUES (@code,@name)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Country codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCourtdocumenttype")]
        public async Task<ActionResult> DownloadCourtdocumenttype()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtdocumenttype");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtdocumenttype' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtdocumenttype.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtdocumenttype\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtdocumenttype\documenttypecodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var filingcodeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "filingcodeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var iscourtuseonly = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "iscourtuseonly").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var isdefault = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "isdefault").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var promptforconfirmation = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "promptforconfirmation").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtdocumenttypeAsync(code, name, filingcodeid, iscourtuseonly, isdefault, promptforconfirmation, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtdocumenttypeAsync(string code, string name, string filingcodeid, string iscourtuseonly, string isdefault, string promptforconfirmation, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtdocumenttypes\"(code, name, filingcodeid, iscourtuseonly, isdefault, promptforconfirmation, efspcode) VALUES (@code,@name,@filingcodeid,@iscourtuseonly,@isdefault,@promptforconfirmation,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("filingcodeid", filingcodeid);
                        cmd.Parameters.AddWithValue("iscourtuseonly", iscourtuseonly);
                        cmd.Parameters.AddWithValue("isdefault", isdefault);
                        cmd.Parameters.AddWithValue("promptforconfirmation", promptforconfirmation);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court document type codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCourtfilertype")]
        public async Task<ActionResult> DownloadCourtfilertype()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtfilertype");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtfilertype' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtfilertype\filertypecodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var isdefault = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "default").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtfilertypeAsync(code, name, isdefault, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtfilertypeAsync(string code, string name, string isdefault, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtfilertypes\"(code, name, isdefault, efspcode) VALUES (@code,@name,@isdefault,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("isdefault", isdefault);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court filer type codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCourtfiletype")]
        public async Task<ActionResult> DownloadCourtfiletype()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtfiletype");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtfiletype' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtfiletype\filetypecodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var extension = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "extension").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtfiletypeAsync(code, name, extension, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtfiletypeAsync(string code, string name, string extension, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtfiletypes\"(code, name, extension, efspcode) VALUES (@code,@name,@extension,@efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("extension", extension);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court file type codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCourtfilingcomponent")]
        public async Task<ActionResult> DownloadCourtfilingcomponent()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtfilingcomponent");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtfilingcomponent' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtfilingcomponent.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtfilingcomponent\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtfilingcomponent\filingcomponentcodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var filingcodeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "filingcodeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var required = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "required").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowmultiple = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowmultiple").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var displayorder = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "displayorder").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var allowedfiletypes = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "allowedfiletypes").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtfilingcomponentAsync(code, name, filingcodeid, required, allowmultiple, displayorder, allowedfiletypes, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtfilingcomponentAsync(string code, string name, string filingcodeid, string required, string allowmultiple, string displayorder, string allowedfiletypes, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtfilingcomponents\"(code, name, filingcodeid, required, allowmultiple, displayorder,allowedfiletypes, efspcode) VALUES (@code, @name, @filingcodeid, @required, @allowmultiple, @displayorder,@allowedfiletypes, @efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("filingcodeid", filingcodeid);
                        cmd.Parameters.AddWithValue("required", required);
                        cmd.Parameters.AddWithValue("allowmultiple", allowmultiple);
                        cmd.Parameters.AddWithValue("displayorder", displayorder);
                        cmd.Parameters.AddWithValue("allowedfiletypes", allowedfiletypes);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court filing component codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }


        [HttpPost]
        [Route("DownloadCourtoptionalservice")]
        public async Task<ActionResult> DownloadCourtoptionalservice()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtoptionalservice");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtoptionalservice' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtoptionalservice.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtoptionalservice\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtoptionalservice\optionalservicescodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var displayorder = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "displayorder").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var fee = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "fee").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var filingcodeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "filingcodeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var multiplierValue = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "multiplier").Descendants("SimpleValue").FirstOrDefault()?.Value;
                    bool resmultiplier = false;
                    resmultiplier = bool.TryParse(multiplierValue, out resmultiplier);
                    //var multiplier = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "multiplier").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var altfeedesc = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "altfeedesc").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var hasfeeprompt = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "hasfeeprompt").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var feeprompttext = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "feeprompttext").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var required = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "required").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var ismprff = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "ismprff").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertCourtoptionalserviceAsync(code, name, displayorder, fee, filingcodeid, resmultiplier, altfeedesc, hasfeeprompt, feeprompttext, required, ismprff, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertCourtoptionalserviceAsync(string code, string name, string displayorder, string fee, string filingcodeid, bool multiplier, string altfeedesc, string hasfeeprompt, string feeprompttext, string required, string ismprff, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtoptionalservices\"(code, name, displayorder, fee, filingcodeid, multiplier, altfeedesc, hasfeeprompt, feeprompttext, required, ismprff, efspcode) VALUES (@code, @name, @displayorder, @fee, @filingcodeid, @multiplier, @altfeedesc, @hasfeeprompt, @feeprompttext, @required, @ismprff, @efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("displayorder", displayorder);
                        cmd.Parameters.AddWithValue("fee", fee);
                        cmd.Parameters.AddWithValue("filingcodeid", filingcodeid);
                        cmd.Parameters.AddWithValue("multiplier", multiplier);
                        cmd.Parameters.AddWithValue("altfeedesc", altfeedesc);
                        cmd.Parameters.AddWithValue("hasfeeprompt", hasfeeprompt);
                        cmd.Parameters.AddWithValue("feeprompttext", feeprompttext);
                        cmd.Parameters.AddWithValue("required", required);
                        cmd.Parameters.AddWithValue("ismprff", ismprff);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court optional service codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadCourtpartytype")]
        public async Task<ActionResult> DownloadCourtpartytype()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:Courtpartytype");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:Courtpartytype' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtpartytype.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtpartytype\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\Courtpartytype\partytypecodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var isavailablefornewparties = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "isavailablefornewparties").Elements("SimpleValue").FirstOrDefault()?.Value;
                    bool resisavailablefornewparties = false;
                    resisavailablefornewparties = bool.TryParse(isavailablefornewparties, out resisavailablefornewparties);
                    var casetypeid = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "casetypeid").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var isrequired = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "isrequired").Elements("SimpleValue").FirstOrDefault()?.Value;
                    bool resisrequired = false;
                    resisrequired = bool.TryParse(isrequired, out resisrequired);
                    var amount = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "amount").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var numberofpartiestoignore = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "numberofpartiestoignore").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var sendforredaction = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "sendforredaction").Elements("SimpleValue").FirstOrDefault()?.Value;
                    bool ressendforredaction = false;
                    ressendforredaction = bool.TryParse(sendforredaction, out ressendforredaction);
                    var dateofdeath = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "dateofdeath").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var displayorder = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "displayorder").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InserCourtpartytypeAsync(code, name, resisavailablefornewparties, casetypeid, resisrequired, amount, numberofpartiestoignore, ressendforredaction, dateofdeath, displayorder, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InserCourtpartytypeAsync(string code, string name, bool isavailablefornewparties, string casetypeid, bool isrequired, string amount, string numberofpartiestoignore, bool sendforredaction, string dateofdeath, string displayorder, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"Courtpartytypes\"(code, name, isavailablefornewparties, casetypeid, isrequired, amount, numberofpartiestoignore, sendforredaction, dateofdeath, displayorder, efspcode) VALUES (@code, @name, @isavailablefornewparties, @casetypeid, @isrequired, @amount, @numberofpartiestoignore, @sendforredaction, @dateofdeath, @displayorder, @efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("isavailablefornewparties", isavailablefornewparties);
                        cmd.Parameters.AddWithValue("casetypeid", casetypeid);
                        cmd.Parameters.AddWithValue("isrequired", isrequired);
                        cmd.Parameters.AddWithValue("amount", amount);
                        cmd.Parameters.AddWithValue("numberofpartiestoignore", numberofpartiestoignore);
                        cmd.Parameters.AddWithValue("sendforredaction", sendforredaction);
                        cmd.Parameters.AddWithValue("dateofdeath", dateofdeath);
                        cmd.Parameters.AddWithValue("displayorder", displayorder);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court party type  codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }

        [HttpPost]
        [Route("DownloadNameSuffixCodes")]
        public async Task<ActionResult> DownloadNameSuffixCodes()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CourtNameSuffixCodes");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:NameSuffixCodes' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                string zipFilePath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtNameSuffixCodes.zip";
                string extractPath = @"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtNameSuffixCodes\";

                var client = new RestClient(url);

                var request = new RestRequest(Method.Get.ToString());

                //RestResponse response = await client.ExecuteAsync(request);
                await client.ExecuteAsync(request);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);

                string xmlContent = System.IO.File.ReadAllText(@"D:\inetpub\wwwroot\tyler_api_live\AltrueCA\CourtNameSuffixCodes\namesuffixcodes.xml");

                XDocument xdoc = XDocument.Parse(xmlContent);
                var columns = xdoc.Descendants("Row");

                foreach (var column in columns)
                {
                    var code = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "code").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var name = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "name").Elements("SimpleValue").FirstOrDefault()?.Value;
                    var efspcode = column.Descendants("Value").Where(v => (string)v.Attribute("ColumnRef") == "efspcode").Elements("SimpleValue").FirstOrDefault()?.Value;


                    await InsertNameSuffixCodesAsync(code, name, efspcode);
                }

                //object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        private async Task InsertNameSuffixCodesAsync(string code, string name, string efspcode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Insert the CourtLocations data into the database                  
                    string sql = "INSERT INTO \"NameSuffixCodes\"(code, name, efspcode) VALUES (@code, @name, @efspcode)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("code", code);
                        cmd.Parameters.AddWithValue("name", name);
                        cmd.Parameters.AddWithValue("efspcode", efspcode);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Court Name Suffix Codes inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Insertion failed.");
                        }

                    }


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }
        }


        [HttpPost]
        [Authorize]
        [Route("CoreFilingNewCivil")]
        public async Task<ActionResult> CoreFilingNewCivil(FeeCalculationRequest casedetail)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CoreFilingNewCivil");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CoreFilingNewCivil' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(casedetail);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);

                string jsonResponse = response.Content;

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                // Deserialize the JSON content to the RootObject
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                var envelopeId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "ENVELOPEID")?.IdentificationID?.Value;
                var CaseId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "CASEID")?.IdentificationID?.Value;
                var CaseFilingId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "FILINGID")?.IdentificationID?.Value;
                var receivedDateStr = rootObject?.DocumentReceivedDate?.Item?.Value;
                string formattedDate = null;
                if (!string.IsNullOrEmpty(receivedDateStr))
                {
                    // Extract numeric part from /Date(1738734406000)/
                    Match match = Regex.Match(receivedDateStr, @"\d+");
                    if (match.Success && long.TryParse(match.Value, out long milliseconds))
                    {
                        // Convert from Unix timestamp (milliseconds) to DateTime
                        DateTime receivedDate = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;

                        // Format DateTime as "yyyy-MM-dd HH:mm:ss"
                        formattedDate = receivedDate.ToString("yyyy-MM-dd HH:mm:ss");

                        Console.WriteLine("Formatted Date: " + formattedDate);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in JSON.");
                }

                // Check if there's any error and handle it
                if (rootObject.Error != null && rootObject.Error.Count > 0)
                {
                    var errorText = rootObject.Error[0].ErrorText.Value;

                    if (errorText == "No Error" && rootObject.Error[0].ErrorCode.Value == "0")
                    {
                        int filingID = _context.InitialCases.OrderByDescending(ic => ic.FilingID).Select(ic => ic.FilingID).FirstOrDefault();

                        if (filingID == 0)
                        {
                            // If no record exists, initialize with 11111111
                            filingID = 11111110;
                        }

                        filingID++; // Increment ID

                        var newCase = new InitialCaseDetails.Cases
                        {
                            SelectedCourt = casedetail.selectedCourt,
                            SelectedCategory = casedetail.selectedCategory,
                            SelectedCaseType = casedetail.selectedCaseType,
                            PaymentAccount = casedetail.paymentAccount,
                            EnvelopeNo = envelopeId,
                            SubmittedDate = formattedDate,
                            //TotalFees = casedetail.totalFees,
                            NoteToClerk = casedetail.note,
                            CreatedBy = casedetail.createdBy,
                            selectedAttorneySec = casedetail.selectedAttorneySec,
                            courtesyemail = casedetail.courtesyemail,
                            FilingID = filingID,
                            CaseIDResp = CaseId,
                            caseFilingId = CaseFilingId,
                            // Add Documents
                            Documents = casedetail.documents.Select(doc => new Documents
                            {
                                EnvelopeNo = envelopeId,
                                DocumentType = doc.documentType,
                                DocumentDescription = doc.documentDescription,
                                FileName = doc.fileName,
                                FileBase64 = doc.fileBase64,
                                SecurityTypes = doc.securityTypes,
                                OptionalServices = doc.optionalServicesSelections.Select(opt => new OptionalServices
                                {
                                    OptionalServiceId = opt.value,
                                    Quantity = opt.Quantity,
                                    EnvelopeNo = envelopeId,
                                    DocumentTypeId = doc.documentType,
                                }).ToList()
                            }).ToList(),

                            // Add Parties
                            Parties = casedetail.parties.Select(party => new Parties
                            {
                                EnvelopeNo = envelopeId,
                                SelectedPartyType = party.selectedPartyType,
                                RoleType = party.roleType,
                                LastName = party.lastName,
                                FirstName = party.firstName,
                                MiddleName = party.middleName,
                                Suffix = party.suffix,
                                CompanyName = party.companyName,
                                Address = party.address,
                                Address2 = party.address2,
                                City = party.city,
                                State = party.state,
                                Zip = party.zip,
                                AddressUnknown = party.addressUnknown,
                                InternationalAddress = party.internationalAddress,
                                SaveToAddressBook = party.saveToAddressBook,
                                SelectedAttorney = party.selectedAttorney,
                                //SelectedBarNumbers = party.selectedBarNumbers.Select(bar => new SelectedBarNumber
                                //{
                                //    BarNumber = bar
                                //}).ToList()
                            }).ToList(),

                            // Add Selected Parties
                            SelectedParties = casedetail.selectedParties.Select(sp => new SelectedParties
                            {
                                PartyName = sp.partyName,
                                PartyType = sp.partyType,
                                Role = sp.role,
                                EnvelopeNo = envelopeId,
                            }).ToList()

                        };

                        await _context.InitialCases.AddAsync(newCase);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine("Error found: " + errorText);
                    }
                }
                else
                {
                    Console.WriteLine("No error details in the response.");
                }

                if (rootObject.Error[0].ErrorCode.Value == "0")
                {
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;
                }
                else
                {
                    resp.success = false;
                    resp.message = "Failed!";
                    resp.data = jsonData;
                }

                //resp.success = true;
                //resp.status = 200;
                //resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("CoreFilingSubsequentCivil")]
        public async Task<ActionResult> CoreFilingSubsequentCivil(SubSequentFeeCalRequest subSequentcase)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CoreFilingSubsequentCivil");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CoreFilingNewCivil' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {


                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(subSequentcase);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);

                string jsonResponse = response.Content;

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                // Deserialize the JSON content to the RootObject
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                var envelopeId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "ENVELOPEID")?.IdentificationID?.Value;
                var CaseId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "CASEID")?.IdentificationID?.Value;
                var CaseFilingId = rootObject?.DocumentIdentification?.FirstOrDefault(d => d.Item?.Value == "FILINGID")?.IdentificationID?.Value;
                var receivedDateStr = rootObject?.DocumentReceivedDate?.Item?.Value;
                string formattedDate = null;
                if (!string.IsNullOrEmpty(receivedDateStr))
                {
                    // Extract numeric part from /Date(1738734406000)/
                    Match match = Regex.Match(receivedDateStr, @"\d+");
                    if (match.Success && long.TryParse(match.Value, out long milliseconds))
                    {
                        // Convert from Unix timestamp (milliseconds) to DateTime
                        DateTime receivedDate = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;

                        // Format DateTime as "yyyy-MM-dd HH:mm:ss"
                        formattedDate = receivedDate.ToString("yyyy-MM-dd HH:mm:ss");

                        Console.WriteLine("Formatted Date: " + formattedDate);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in JSON.");
                }

                // Check if there's any error and handle it
                if (rootObject.Error != null && rootObject.Error.Count > 0)
                {
                    var errorText = rootObject.Error[0].ErrorText.Value;

                    if (errorText == "No Error" && rootObject.Error[0].ErrorCode.Value == "0")
                    {
                        int filingID = _context.InitialCases.OrderByDescending(ic => ic.FilingID).Select(ic => ic.FilingID).FirstOrDefault();

                        if (filingID == 0)
                        {
                            // If no record exists, initialize with 11111111
                            filingID = 11111110;
                        }

                        filingID++; // Increment ID

                        var newCase = new InitialCaseDetails.Cases
                        {
                            SelectedCourt = subSequentcase.selectedCourt,
                            CaseNumber = subSequentcase.caseNumber,
                            SelectedCaseType = subSequentcase.caseNumber,
                            PaymentAccount = subSequentcase.paymentAccount,
                            EnvelopeNo = envelopeId,
                            SubmittedDate = formattedDate,
                            //TotalFees = subSequentcase.totalFees,
                            NoteToClerk = subSequentcase.note,
                            CreatedBy = subSequentcase.createdBy,
                            selectedAttorneySec = subSequentcase.selectedAttorneySec,
                            courtesyemail = subSequentcase.courtesyemail,
                            FilingID = filingID,
                            CaseIDResp = CaseId,
                            IsExistingCase = true,
                            caseFilingId = CaseFilingId,

                            // Add Documents
                            Documents = subSequentcase.documents.Select(doc => new Documents
                            {
                                EnvelopeNo = envelopeId,
                                DocumentType = doc.documentType,
                                DocumentDescription = doc.documentDescription,
                                FileName = doc.fileName,
                                FileBase64 = doc.fileBase64,
                                SecurityTypes = doc.securityTypes,
                                OptionalServices = doc.optionalServicesSelections.Select(opt => new OptionalServices
                                {
                                    OptionalServiceId = opt.value,
                                    Quantity = opt.Quantity,
                                    EnvelopeNo = envelopeId,
                                    DocumentTypeId = doc.documentType,
                                }).ToList()
                            }).ToList(),

                            // Add Parties
                            Parties = subSequentcase.parties.Select(party => new Parties
                            {
                                EnvelopeNo = envelopeId,
                                SelectedPartyType = party.selectedPartyType,
                                RoleType = party.roleType,
                                LastName = party.lastName,
                                FirstName = party.firstName,
                                MiddleName = party.middleName,
                                Suffix = party.suffix,
                                CompanyName = party.companyName,
                                Address = party.address,
                                Address2 = party.address2,
                                City = party.city,
                                State = party.state,
                                Zip = party.zip,
                                AddressUnknown = party.addressUnknown,
                                InternationalAddress = party.internationalAddress,
                                SaveToAddressBook = party.saveToAddressBook,
                                SelectedAttorney = party.selectedAttorney,
                                //SelectedBarNumbers = party.selectedBarNumbers.Select(bar => new SelectedBarNumber
                                //{
                                //    BarNumber = bar
                                //}).ToList()
                            }).ToList(),

                            // Add Selected Parties
                            SelectedParties = subSequentcase.selectedParties.Select(sp => new SelectedParties
                            {
                                PartyName = sp.partyName,
                                PartyType = sp.partyType,
                                Role = sp.role,
                                EnvelopeNo = envelopeId,
                            }).ToList()

                        };

                        await _context.InitialCases.AddAsync(newCase);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine("Error found: " + errorText);
                    }
                }
                else
                {
                    Console.WriteLine("No error details in the response.");
                }

                if (rootObject.Error[0].ErrorCode.Value == "0")
                {
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;
                }
                else
                {
                    resp.success = false;
                    resp.message = "Failed!";
                    resp.data = jsonData;
                }

                //resp.success = true;
                //resp.status = 200;
                //resp.message = "Success!";
                //resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("ReviewFiling")]
        public async Task<ActionResult> ReviewFiling(FeeCalculationRequest casedetail)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ReviewFiling");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ReviewFiling' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {


                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(casedetail);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }

        [HttpGet]
        [Route("GetCourtLocations")]
        public async Task<IActionResult> GetCourtLocations()
        {
            var locations = await _context.CourtLocations
                                        .Select(u => new { u.code, u.name, u.initial, u.subsequent })
                                        .ToListAsync();

            return Ok(locations);
        }

        [HttpGet("GetCategoty")]
        public async Task<IActionResult> GetCategotyType()
        {
            //var caseCategory = await _context.CaseCategoryCodes
            //                            .Where(u => u.Id == id)
            //                            .ToListAsync();

            var caseCategory = await _context.CaseCategoryCodes
                                       .Select(u => new { u.code, u.name })
                                       .ToListAsync();

            if (caseCategory == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(caseCategory);
        }


        [HttpGet("GetCaseType")]
        public async Task<IActionResult> GetCaseType(string categoryId)
        {
            var caseType = await _context.CaseTypeCodes
                                        .Where(u => u.casecategory == categoryId)
                                        .Select(u => new { u.code, u.name, u.fee })
                                        .ToListAsync();


            if (caseType == null)
            {
                return NotFound(new { Message = "Case type not found" });
            }

            return Ok(caseType);
        }


        [HttpGet("GetFilingCode")]
        public async Task<IActionResult> GetFilingCode(string categoryId)
        {
            var filingCode = await _context.Filingcodes
                                        .Where(u => u.casecategory == categoryId)
                                        .Select(u => new { u.code, u.name, u.fee })
                                        .ToListAsync();


            if (filingCode == null)
            {
                return NotFound(new { Message = "Filing code not found" });
            }

            return Ok(filingCode);
        }

        [HttpGet("GetDocumentCode")]
        public async Task<IActionResult> GetDocumentCode(string filingcode)
        {
            var documentTypeCode = await _context.Courtdocumenttypes
                                        .Where(u => u.filingcodeid == filingcode)
                                        .Select(u => new { u.code, u.name })
                                        .ToListAsync();


            if (documentTypeCode == null)
            {
                return NotFound(new { Message = "Document Type code not found" });
            }

            return Ok(documentTypeCode);
        }

        [HttpGet("GetCourtOptionalServices")]
        public async Task<IActionResult> GetCourtoptionalservices(string filingcode)
        {
            var optionalServicecode = await _context.Courtoptionalservices
                                        .Where(u => u.filingcodeid == filingcode)
                                        .Select(u => new { u.code, u.name, u.fee, u.multiplier })
                                        .ToListAsync();


            if (optionalServicecode == null)
            {
                return NotFound(new { Message = "Optional Services Type code not found" });
            }

            return Ok(optionalServicecode);
        }

        [HttpGet("GetPartyTypeCode")]
        public async Task<IActionResult> GetPartyTypeCode(string caseTypeId)
        {
            var partyTypeCode = await _context.Courtpartytypes
                                        .Where(u => u.casetypeid == caseTypeId)
                                        .Select(u => new { u.code, u.name })
                                        .ToListAsync();


            if (partyTypeCode == null)
            {
                return NotFound(new { Message = "Party Type code not found" });
            }

            return Ok(partyTypeCode);
        }

        [HttpGet("GetCourtNameSuffixCode")]
        public async Task<IActionResult> GetCourtNameSuffixCode()
        {
            var nameSuffixCode = await _context.NameSuffixCodes
                                      .Select(u => new { u.code, u.name })
                                      .ToListAsync();


            if (nameSuffixCode == null)
            {
                return NotFound(new { Message = "Name Suffix Codes Type not found" });
            }

            return Ok(nameSuffixCode);
        }


        [HttpGet("GetOptionalServices")]
        public async Task<IActionResult> GetOptionalServices(string ID)
        {
            var optionalService = await _context.Courtoptionalservices
                                  .Where(u => u.code == ID)
                                  .Select(u => new { u.code, u.name, u.fee })
                                  .FirstOrDefaultAsync();


            if (optionalService == null)
            {
                return NotFound(new { Message = "Optional Services Type code not found" });
            }

            return Ok(optionalService);
        }


        [HttpGet("GetAllPartyTypes")]
        public async Task<IActionResult> GetAllPartyTypes()
        {

            var courtpartytypes = await _context.Courtpartytypes
                                       .Select(u => new { u.code, u.name })
                                       .Distinct()
                                       .ToListAsync();

            if (courtpartytypes == null)
            {
                return NotFound(new { Message = "Court party types not found" });
            }

            return Ok(courtpartytypes);
        }

        [HttpPost]
        [Authorize]
        [Route("GetCaseTest")]
        public async Task<ActionResult> GetCaseTest(string CaseTrackingID = "dbed64e6-2379-4290-87b8-a42f958a3e39")
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:CaseTestUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:CaseTestUrl' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var client = new RestClient(url); //("http://localhost/Home/CaseUrl?id=dbed64e6-2379-4290-87b8-a42f958a3e39");

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();

                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);

                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString())
                    .AddParameter("CaseTrackingID", CaseTrackingID.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }
        }


        [HttpGet("GetCaseUserList")]
        [Authorize]
        public async Task<ActionResult<List<Cases>>> GetCaseUserList()
        {
            try
            {
                //var caseData = await _context.InitialCases                    
                //    .Include(c => c.Documents)
                //        .ThenInclude(d => d.OptionalServices)
                //    .Include(c => c.Parties)
                //    .Include(c => c.SelectedParties)
                //    .ToListAsync();

                var caseData = await _context.InitialCases
                      .Include(c => c.Documents)
                          .ThenInclude(d => d.OptionalServices)
                      .Include(c => c.Parties)
                      .Include(c => c.SelectedParties)
                      .Select(c => new
                      {
                          c.Id,
                          c.SelectedCourt,
                          c.SelectedCategory,
                          c.SelectedCaseType,
                          c.PaymentAccount,
                          c.EnvelopeNo,
                          c.SubmittedDate,
                          c.NoteToClerk,
                          c.CreatedBy,
                          c.selectedAttorneySec,
                          c.courtesyemail,
                          c.FilingID,
                          DraftSavedAt = string.IsNullOrEmpty(c.DraftSavedAt) ? "N/A" : c.DraftSavedAt, // Fix here
                          c.Documents,
                          c.Parties,
                          c.SelectedParties
                      })
                      .ToListAsync();


                return caseData.Any() ? Ok(caseData) : NotFound("No cases found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }


        // Get a case by ID
        [HttpGet("GetCaseDetails")]
        [Authorize]
        public async Task<ActionResult<Cases>> GetCaseDetails(string envelopeID)
        {
            if (string.IsNullOrEmpty(envelopeID))
            {
                return BadRequest("Envelope ID is required.");
            }

            try
            {
                var caseData = await _context.InitialCases
                    .Include(c => c.Documents)
                        .ThenInclude(d => d.OptionalServices)  // Include nested OptionalServices
                    .Include(c => c.Parties)
                    //.ThenInclude(p => p.SelectedBarNumbers) // Uncomment if needed
                    .Include(c => c.SelectedParties)
                    .FirstOrDefaultAsync(c => c.EnvelopeNo == envelopeID);

                if (caseData == null)
                {
                    return NotFound($"No case found for Envelope ID: {envelopeID}");
                }

                // Assuming you have these related entities (CourtLocations, Categories, CaseTypes)
                // Replace the codes with actual names
                var courtLocation = await _context.CourtLocations
                    .Where(cl => cl.code == caseData.SelectedCourt)
                    .FirstOrDefaultAsync();

                var category = await _context.CaseCategoryCodes
                    .Where(c => c.code == caseData.SelectedCategory)
                    .FirstOrDefaultAsync();

                var caseType = await _context.CaseTypeCodes
                    .Where(ct => ct.code == caseData.SelectedCaseType)
                    .FirstOrDefaultAsync();



                // Now, set the actual name values instead of codes
                if (courtLocation != null)
                    caseData.SelectedCourt = courtLocation.name;

                if (category != null)
                    caseData.SelectedCategory = category.name;

                if (caseType != null)
                    caseData.SelectedCaseType = caseType.name;

                var attorneyIds = caseData.Parties
                          .Where(p => !string.IsNullOrEmpty(p.SelectedAttorney))
                          .Select(p => p.SelectedAttorney)
                          .Distinct()
                          .ToList();

                var attorneys = await _context.AttorneyDetails
                                             .Where(a => attorneyIds.Contains(a.AttorneyID))
                                             .ToDictionaryAsync(a => a.AttorneyID, a => a.FirstName + " " + a.LastName);

                foreach (var document in caseData.Documents)
                {
                    var filingCode = await _context.Filingcodes
                        .Where(f => f.code == document.DocumentType)
                        .FirstOrDefaultAsync();

                    if (filingCode != null)
                    {
                        document.DocumentType = filingCode.name;
                    }

                    var courtDocumentType = await _context.Courtdocumenttypes
                        .Where(cd => cd.code == document.SecurityTypes)
                        .FirstOrDefaultAsync();

                    if (courtDocumentType != null)
                    {
                        document.SecurityTypes = courtDocumentType.name;
                    }

                    foreach (var optionalService in document.OptionalServices)
                    {
                        var optionalServiceDetail = await _context.Courtoptionalservices
                            .Where(co => co.code == optionalService.OptionalServiceId)
                            .FirstOrDefaultAsync();

                        if (optionalServiceDetail != null)
                        {
                            optionalService.OptionalServiceId = optionalServiceDetail.name;
                        }
                    }
                }

                foreach (var party in caseData.Parties)
                {
                    // Replace selectedPartyType with name from Courtpartytypes
                    var partyType = await _context.Courtpartytypes
                        .Where(pt => pt.code == party.SelectedPartyType)
                        .FirstOrDefaultAsync();

                    if (partyType != null)
                    {
                        party.SelectedPartyType = partyType.name;  // Replace code with name
                    }

                    // Replace roleType code with actual value
                    if (party.RoleType == "1")
                    {
                        party.RoleType = "Individual";
                    }
                    else if (party.RoleType == "2")
                    {
                        party.RoleType = "Business";
                    }

                    if (!string.IsNullOrEmpty(party.SelectedAttorney) && attorneys.ContainsKey(party.SelectedAttorney))
                    {
                        party.SelectedAttorney = attorneys[party.SelectedAttorney];
                    }
                }

                foreach (var selectedParty in caseData.SelectedParties)
                {
                    // Replace partyType with name from Courtpartytypes
                    var partyType = await _context.Courtpartytypes
                        .Where(pt => pt.code == selectedParty.PartyType)
                        .FirstOrDefaultAsync();

                    if (partyType != null)
                    {
                        selectedParty.PartyType = partyType.name;  // Replace code with name
                    }

                    // Replace role with actual value
                    if (selectedParty.Role == "1")
                    {
                        selectedParty.Role = "Individual";
                    }
                    else if (selectedParty.Role == "2")
                    {
                        selectedParty.Role = "Business";
                    }
                }


                return Ok(caseData);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An unexpected error occurred while retrieving the case details.");
            }
        }


        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword(UpdatePassword updatePassword)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ChangePassword");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ChangePassword' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest("Token is missing or invalid.");

            // Extract the token (remove "Bearer " prefix)
            var token = authHeader.Substring("Bearer ".Length);
            var UserName = _userService.GetEmailFromToken(token);

            // Query database for password hash
            var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("UserName", UserName)//"ghorpadesumit471@gmail.com")
                .AddParameter("Password", Password)//"175803a9-f490-4645-b76f-c9337210ff92")
                .AddParameter("OldPassword", updatePassword.OldPassword)
                .AddParameter("NewPassword", updatePassword.NewPassword);
                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);
                string result = jsonData.ToString();
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(result);
                UserInfo userInfo = new UserInfo();
                userInfo.PasswordEncrypted = updatePassword.NewPassword;

                if (userResponse.PasswordHash != null && userResponse.Error.ErrorCode == "0")
                {
                    // Find the existing user in UserResponses
                    var existingUser = await _context.UserResponses
                        .Where(u => u.Email == UserName)
                        .FirstOrDefaultAsync();

                    // Find the existing user in UserInfo
                    var existingUserInfo = await _context.UserInfos
                        .Where(u => u.Email == UserName)
                        .FirstOrDefaultAsync();

                    if (existingUser != null)
                    {
                        existingUser.PasswordHash = userResponse.PasswordHash;
                        _context.Entry(existingUser).Property(u => u.PasswordHash).IsModified = true;
                    }

                    if (existingUserInfo != null)
                    {
                        existingUserInfo.PasswordEncrypted = updatePassword.NewPassword;
                        _context.Entry(existingUserInfo).Property(u => u.PasswordEncrypted).IsModified = true;
                    }

                    await _context.SaveChangesAsync();
                }



                if (userResponse.Error.ErrorCode == "0")
                {
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;
                }
                else
                {
                    resp.success = false;
                    resp.message = "Failed!";
                    resp.data = jsonData;
                }

                return Ok(resp);

            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }


        //[HttpPost("savedraft")]
        //[Authorize]
        //public async Task<IActionResult> SaveDraft([FromBody] FeeCalculationRequest request)
        //{
        //    // Validate required fields
        //    if (string.IsNullOrEmpty(request.selectedCourt))
        //    {
        //        return BadRequest(new { message = "Required fields are missing" });
        //    }

        //    // Generate required data
        //    //var envelopeId = Guid.NewGuid().ToString(); // Example unique identifier
        //    var formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        //    // Get last Filing ID from the database
        //    var lastFiling = await _context.InitialCases
        //                                   .OrderByDescending(c => c.FilingID)
        //                                   .Select(c => c.FilingID)
        //                                   .FirstOrDefaultAsync();
        //    var filingID = (lastFiling != null) ? lastFiling + 1 : 1000; // Start from 1000 if no records exist

        //    // Create case object with only mandatory fields
        //    var newCase = new InitialCaseDetails.Cases
        //    {
        //        SelectedCourt = request.selectedCourt,
        //        SelectedCategory = request.selectedCategory,
        //        SelectedCaseType = request.selectedCaseType,
        //        PaymentAccount = request.paymentAccount,
        //        //EnvelopeNo = envelopeId,
        //        SubmittedDate = formattedDate,
        //        NoteToClerk = request.note,
        //        CreatedBy = request.createdBy,
        //        selectedAttorneySec = request.selectedAttorneySec,
        //        courtesyemail = request.courtesyemail,
        //        FilingID = filingID,
        //        IsDraft = true,
        //        DraftSavedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")

        //    };

        //    // Filter and add documents (skip if documentType is empty/null)
        //    var validDocuments = request.documents?
        //        .Where(doc => !string.IsNullOrEmpty(doc.documentType))
        //        .Select(doc => new InitialCaseDetails.Documents
        //        {
        //            //EnvelopeNo = envelopeId,
        //            DocumentType = doc.documentType,
        //            DocumentDescription = doc.documentDescription,
        //            FileName = doc.fileName,
        //            FileBase64 = doc.fileBase64,
        //            SecurityTypes = doc.securityTypes,
        //            OptionalServices = doc.optionalServicesSelections?
        //                .Select(opt => new InitialCaseDetails.OptionalServices
        //                {
        //                    OptionalServiceId = opt.value,
        //                    Quantity = opt.Quantity,
        //                    //EnvelopeNo = envelopeId,
        //                    DocumentTypeId = doc.documentType,
        //                }).ToList() ?? new List<InitialCaseDetails.OptionalServices>()
        //        }).ToList();

        //    if (validDocuments != null && validDocuments.Any())
        //    {
        //        newCase.Documents = validDocuments;
        //    }

        //    // Filter and add parties (skip if selectedPartyType is empty/null)
        //    var validParties = request.parties?
        //        .Where(party => !string.IsNullOrEmpty(party.selectedPartyType))
        //        .Select(party => new InitialCaseDetails.Parties
        //        {
        //            //EnvelopeNo = envelopeId,
        //            SelectedPartyType = party.selectedPartyType,
        //            RoleType = party.roleType,
        //            LastName = party.lastName,
        //            FirstName = party.firstName,
        //            MiddleName = party.middleName,
        //            Suffix = party.suffix,
        //            CompanyName = party.companyName,
        //            Address = party.address,
        //            Address2 = party.address2,
        //            City = party.city,
        //            State = party.state,
        //            Zip = party.zip,
        //            AddressUnknown = party.addressUnknown,
        //            InternationalAddress = party.internationalAddress,
        //            SaveToAddressBook = party.saveToAddressBook,
        //            SelectedAttorney = party.selectedAttorney
        //        }).ToList();

        //    if (validParties != null && validParties.Any())
        //    {
        //        newCase.Parties = validParties;
        //    }

        //    // Filter and add selected parties (skip if partyName is empty/null)
        //    var validSelectedParties = request.selectedParties?
        //        .Where(sp => !string.IsNullOrEmpty(sp.partyName))
        //        .Select(sp => new InitialCaseDetails.SelectedParties
        //        {
        //            PartyName = sp.partyName,
        //            PartyType = sp.partyType,
        //            Role = sp.role,
        //            //EnvelopeNo = envelopeId,
        //        }).ToList();

        //    if (validSelectedParties != null && validSelectedParties.Any())
        //    {
        //        newCase.SelectedParties = validSelectedParties;
        //    }

        //    // If no additional data exists, save only basic case details
        //    await _context.InitialCases.AddAsync(newCase);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Draft saved successfully", caseId = newCase.Id });
        //}

        //[HttpPost("SaveDraftInitial")]
        //[Authorize]
        //public async Task<IActionResult> SaveDraftInitial([FromBody] FeeCalculationRequest request)
        //{
        //    try
        //    {
        //        // Validate required fields
        //        if (string.IsNullOrEmpty(request.selectedCourt))
        //        {
        //            return BadRequest(new { message = "Required fields are missing" });
        //        }

        //        var existingCase = await _context.InitialCases
        //                                 .FirstOrDefaultAsync(c => c.FilingID == request.filingID);

        //        int filingID;

        //        if (existingCase != null)
        //        {
        //            // ✅ If filingID exists, use the existing one
        //            filingID = existingCase.FilingID;
        //        }
        //        else
        //        {
        //            // ✅ If filingID does not exist, generate a new one
        //            var lastFiling = await _context.InitialCases
        //                                           .OrderByDescending(c => c.FilingID)
        //                                           .Select(c => c.FilingID)
        //                                           .FirstOrDefaultAsync();
        //            filingID = (lastFiling != null) ? lastFiling + 1 : 1000;
        //        }

        //        // Generate a new Filing ID
        //        var formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        //        //var lastFiling = await _context.InitialCases
        //        //                               .OrderByDescending(c => c.FilingID)
        //        //                               .Select(c => c.FilingID)
        //        //                               .FirstOrDefaultAsync();
        //        //var filingID = (lastFiling != null) ? lastFiling + 1 : 1000;

        //        InitialCaseDetails.Cases newCase = new InitialCaseDetails.Cases
        //        {
        //            SelectedCourt = request.selectedCourt,
        //            SelectedCategory = request.selectedCategory,
        //            SelectedCaseType = request.selectedCaseType,
        //            PaymentAccount = request.paymentAccount,
        //            SubmittedDate = formattedDate,
        //            NoteToClerk = request.note,
        //            CreatedBy = request.createdBy,
        //            selectedAttorneySec = request.selectedAttorneySec,
        //            courtesyemail = request.courtesyemail,
        //            FilingID = filingID,
        //            IsDraft = true,  // Mark as draft
        //            DraftSavedAt = formattedDate,
        //            IsExistingCase = request.isExistingCase, // ✅ Handle existing case
        //            CaseNumber = request.isExistingCase ? request.caseNumber : null, // ✅ CaseNumber logic
        //            caseTitle = request.caseTitle,
        //            courtLocation = request.courtLocation,
        //            caseTrackingID = request.caseTrackingID,


        //        };

        //        // Handle documents
        //        if (request.documents != null && request.documents.Any())
        //        {
        //            newCase.Documents = request.documents
        //                .Where(doc => !string.IsNullOrEmpty(doc.documentType))
        //                .Select(doc => new InitialCaseDetails.Documents
        //                {
        //                    DocumentType = doc.documentType,
        //                    DocumentDescription = doc.documentDescription,
        //                    FileName = doc.fileName,
        //                    FileBase64 = doc.fileBase64,
        //                    SecurityTypes = doc.securityTypes,
        //                    fee = doc.fee,
        //                    OptionalServices = doc.optionalServicesSelections?
        //                        .Where(opt => opt != null) // Ensure valid OptionalServices
        //                        .Select(opt => new InitialCaseDetails.OptionalServices
        //                        {
        //                            OptionalServiceId = opt.value,
        //                            Quantity = opt.Quantity ?? 0, // Default to 0 if null
        //                            DocumentTypeId = doc.documentType,
        //                            fee = opt.fee,
        //                            label = opt.label,
        //                            multiplier = opt.multiplier,
        //                        }).ToList() ?? new List<InitialCaseDetails.OptionalServices>()
        //                }).ToList();
        //        }

        //        // Handle parties
        //        if (request.parties != null && request.parties.Any())
        //        {
        //            newCase.Parties = request.parties
        //                .Where(party => !string.IsNullOrEmpty(party.selectedPartyType))
        //                .Select(party => new InitialCaseDetails.Parties
        //                {
        //                    SelectedPartyType = party.selectedPartyType,
        //                    RoleType = party.roleType,
        //                    LastName = party.lastName,
        //                    FirstName = party.firstName,
        //                    MiddleName = party.middleName,
        //                    Suffix = party.suffix,
        //                    CompanyName = party.companyName,
        //                    Address = party.address,
        //                    Address2 = party.address2,
        //                    City = party.city,
        //                    State = party.state,
        //                    Zip = party.zip,
        //                    AddressUnknown = party.addressUnknown,
        //                    InternationalAddress = party.internationalAddress,
        //                    SaveToAddressBook = party.saveToAddressBook,
        //                    SelectedAttorney = party.selectedAttorney
        //                }).ToList();
        //        }

        //        // Handle selected parties
        //        if (request.selectedParties != null && request.selectedParties.Any())
        //        {
        //            newCase.SelectedParties = request.selectedParties
        //                .Where(sp => !string.IsNullOrEmpty(sp.partyName))
        //                .Select(sp => new InitialCaseDetails.SelectedParties
        //                {
        //                    PartyName = sp.partyName,
        //                    PartyType = sp.partyType,
        //                    Role = sp.role
        //                }).ToList();
        //        }

        //        // Save the draft case
        //        await _context.InitialCases.AddAsync(newCase);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { message = "Draft saved successfully", caseId = newCase.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        //    }
        //}


        [HttpPost("SaveDraftInitial")]
        [Authorize]
        public async Task<IActionResult> SaveDraftInitial([FromBody] FeeCalculationRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(request.selectedCourt))
                {
                    return BadRequest(new { message = "Required fields are missing" });
                }

                var existingCase = await _context.InitialCases
                                         .FirstOrDefaultAsync(c => c.FilingID == request.filingID);

                int filingID;

                if (existingCase != null)
                {
                    // ✅ If filingID exists, use the existing one
                    filingID = existingCase.FilingID;
                }
                else
                {
                    // ✅ If filingID does not exist, generate a new one
                    var lastFiling = await _context.InitialCases
                                                   .OrderByDescending(c => c.FilingID)
                                                   .Select(c => c.FilingID)
                                                   .FirstOrDefaultAsync();
                    filingID = (lastFiling != null) ? lastFiling + 1 : 1000;
                }

                var formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                InitialCaseDetails.Cases newCase = new InitialCaseDetails.Cases
                {
                    SelectedCourt = request.selectedCourt,
                    SelectedCategory = request.selectedCategory,
                    SelectedCaseType = request.selectedCaseType,
                    PaymentAccount = request.paymentAccount,
                    SubmittedDate = formattedDate,
                    NoteToClerk = request.note,
                    CreatedBy = request.createdBy,
                    selectedAttorneySec = request.selectedAttorneySec,
                    courtesyemail = request.courtesyemail,
                    FilingID = filingID,
                    IsDraft = true,
                    DraftSavedAt = formattedDate,
                    IsExistingCase = request.isExistingCase,
                    CaseNumber = request.isExistingCase ? request.caseNumber : null,
                    caseTitle = request.caseTitle,
                    courtLocation = request.courtLocation,
                    caseTrackingID = request.caseTrackingID,
                };

                // ✅ Save case first so it gets an ID
                await _context.InitialCases.AddAsync(newCase);
                await _context.SaveChangesAsync();

                // ✅ Delete rows where SelectedCourt is 'Reserved'
                var reservedCases = await _context.InitialCases
                    .Where(c => c.SelectedCourt == "Reserved")
                    .ToListAsync();

                if (reservedCases.Any())
                {
                    _context.InitialCases.RemoveRange(reservedCases);
                    await _context.SaveChangesAsync();
                }


                // ✅ Handle documents and optional services
                if (request.documents != null && request.documents.Any())
                {
                    var documentList = new List<InitialCaseDetails.Documents>();

                    foreach (var doc in request.documents.Where(d => !string.IsNullOrEmpty(d.documentType)))
                    {
                        var newDocument = new InitialCaseDetails.Documents
                        {
                            CaseId = newCase.Id, // ✅ Auto-assign CaseId
                            DocumentType = doc.documentType,
                            DocumentDescription = doc.documentDescription,
                            FileName = doc.fileName,
                            FileBase64 = doc.fileBase64,
                            SecurityTypes = doc.securityTypes,
                            fee = doc.fee
                        };

                        // ✅ Handle Optional Services
                        if (doc.optionalServicesSelections != null && doc.optionalServicesSelections.Any())
                        {
                            newDocument.OptionalServices = doc.optionalServicesSelections
                                .Where(opt => opt != null)
                                .Select(opt => new InitialCaseDetails.OptionalServices
                                {
                                    DocumentId = newDocument.Id, // ✅ Will be linked after SaveChanges
                                    CaseId = newCase.Id, // ✅ Assign CaseId
                                    OptionalServiceId = opt.value,
                                    Quantity = opt.Quantity ?? 0,
                                    DocumentTypeId = doc.documentType,
                                    fee = opt.fee,
                                    label = opt.label,
                                    multiplier = opt.multiplier
                                }).ToList();
                        }

                        documentList.Add(newDocument);
                    }

                    await _context.Documents.AddRangeAsync(documentList);
                    await _context.SaveChangesAsync(); // ✅ Save documents first to ensure IDs
                }

                // ✅ Handle parties
                if (request.parties != null && request.parties.Any())
                {
                    var partyList = request.parties
                        .Where(p => !string.IsNullOrEmpty(p.selectedPartyType))
                        .Select(p => new InitialCaseDetails.Parties
                        {
                            CaseId = newCase.Id, // ✅ Auto-assign CaseId
                            CasesId = newCase.Id,
                            SelectedPartyType = p.selectedPartyType,
                            RoleType = p.roleType,
                            LastName = p.lastName,
                            FirstName = p.firstName,
                            MiddleName = p.middleName,
                            Suffix = p.suffix,
                            CompanyName = p.companyName,
                            Address = p.address,
                            Address2 = p.address2,
                            City = p.city,
                            State = p.state,
                            Zip = p.zip,
                            AddressUnknown = p.addressUnknown,
                            InternationalAddress = p.internationalAddress,
                            SaveToAddressBook = p.saveToAddressBook,
                            SelectedAttorney = p.selectedAttorney
                        }).ToList();

                    await _context.Parties.AddRangeAsync(partyList);
                    await _context.SaveChangesAsync();
                }

                // ✅ Handle selected parties
                if (request.selectedParties != null && request.selectedParties.Any())
                {
                    var selectedPartyList = request.selectedParties
                        .Where(sp => !string.IsNullOrEmpty(sp.partyName))
                        .Select(sp => new InitialCaseDetails.SelectedParties
                        {
                            CaseId = newCase.Id, // ✅ Auto-assign CaseId
                            PartyName = sp.partyName,
                            PartyType = sp.partyType,
                            Role = sp.role
                        }).ToList();

                    await _context.SelectedParties.AddRangeAsync(selectedPartyList);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Draft saved successfully", caseId = newCase.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }



        //[HttpPost("savedraft")]
        //[Authorize]
        //public async Task<IActionResult> SaveDraft([FromBody] FeeCalculationRequest request)
        //{
        //    try
        //    {
        //        // Validate required fields
        //        if (string.IsNullOrEmpty(request.selectedCourt))
        //        {
        //            return BadRequest(new { message = "Required fields are missing" });
        //        }

        //        var existingCase = await _context.InitialCases
        //                                 .FirstOrDefaultAsync(c => c.FilingID == request.filingID);

        //        int filingID;
        //        string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        //        if (existingCase != null)
        //        {
        //            // ✅ Update existing case if filingID exists
        //            filingID = existingCase.FilingID;

        //            existingCase.SelectedCourt = request.selectedCourt;
        //            existingCase.SelectedCategory = request.selectedCategory;
        //            existingCase.SelectedCaseType = request.selectedCaseType;
        //            existingCase.PaymentAccount = request.paymentAccount;
        //            existingCase.SubmittedDate = formattedDate;
        //            existingCase.NoteToClerk = request.note;
        //            existingCase.CreatedBy = request.createdBy;
        //            existingCase.selectedAttorneySec = request.selectedAttorneySec;
        //            existingCase.courtesyemail = request.courtesyemail;
        //            existingCase.IsDraft = true;
        //            existingCase.DraftSavedAt = formattedDate;
        //            existingCase.IsExistingCase = request.isExistingCase;
        //            existingCase.CaseNumber = request.isExistingCase ? request.caseNumber : null;
        //            existingCase.caseTitle = request.caseTitle;
        //            existingCase.courtLocation = request.courtLocation;
        //            existingCase.caseTrackingID = request.caseTrackingID;

        //            // Update Documents
        //            if (request.documents != null && request.documents.Any())
        //            {
        //                _context.Documents.RemoveRange(existingCase.Documents); // Remove old documents
        //                existingCase.Documents = request.documents
        //                    .Where(doc => !string.IsNullOrEmpty(doc.documentType))
        //                    .Select(doc => new InitialCaseDetails.Documents
        //                    {
        //                        DocumentType = doc.documentType,
        //                        DocumentDescription = doc.documentDescription,
        //                        FileName = doc.fileName,
        //                        FileBase64 = doc.fileBase64,
        //                        SecurityTypes = doc.securityTypes,
        //                        fee = doc.fee,
        //                        OptionalServices = doc.optionalServicesSelections?
        //                            .Where(opt => opt != null)
        //                            .Select(opt => new InitialCaseDetails.OptionalServices
        //                            {
        //                                OptionalServiceId = opt.value,
        //                                Quantity = opt.Quantity ?? 0,
        //                                DocumentTypeId = doc.documentType,
        //                                fee = opt.fee,
        //                                label = opt.label,
        //                                multiplier = opt.multiplier,
        //                            }).ToList() ?? new List<InitialCaseDetails.OptionalServices>()
        //                    }).ToList();
        //            }

        //            // Update Parties
        //            if (request.parties != null && request.parties.Any())
        //            {
        //                _context.Parties.RemoveRange(existingCase.Parties); // Remove old parties
        //                existingCase.Parties = request.parties
        //                    .Where(party => !string.IsNullOrEmpty(party.selectedPartyType))
        //                    .Select(party => new InitialCaseDetails.Parties
        //                    {
        //                        SelectedPartyType = party.selectedPartyType,
        //                        RoleType = party.roleType,
        //                        LastName = party.lastName,
        //                        FirstName = party.firstName,
        //                        MiddleName = party.middleName,
        //                        Suffix = party.suffix,
        //                        CompanyName = party.companyName,
        //                        Address = party.address,
        //                        Address2 = party.address2,
        //                        City = party.city,
        //                        State = party.state,
        //                        Zip = party.zip,
        //                        AddressUnknown = party.addressUnknown,
        //                        InternationalAddress = party.internationalAddress,
        //                        SaveToAddressBook = party.saveToAddressBook,
        //                        SelectedAttorney = party.selectedAttorney
        //                    }).ToList();
        //            }

        //            // Update Selected Parties
        //            if (request.selectedParties != null && request.selectedParties.Any())
        //            {
        //                _context.SelectedParties.RemoveRange(existingCase.SelectedParties); // Remove old selected parties
        //                existingCase.SelectedParties = request.selectedParties
        //                    .Where(sp => !string.IsNullOrEmpty(sp.partyName))
        //                    .Select(sp => new InitialCaseDetails.SelectedParties
        //                    {
        //                        PartyName = sp.partyName,
        //                        PartyType = sp.partyType,
        //                        Role = sp.role
        //                    }).ToList();
        //            }

        //            _context.InitialCases.Update(existingCase);
        //        }
        //        else
        //        {
        //            // ✅ Create a new case if filingID does not exist
        //            var lastFiling = await _context.InitialCases
        //                                           .OrderByDescending(c => c.FilingID)
        //                                           .Select(c => c.FilingID)
        //                                           .FirstOrDefaultAsync();
        //            filingID = (lastFiling != null) ? lastFiling + 1 : 1000;

        //            var newCase = new InitialCaseDetails.Cases
        //            {
        //                SelectedCourt = request.selectedCourt,
        //                SelectedCategory = request.selectedCategory,
        //                SelectedCaseType = request.selectedCaseType,
        //                PaymentAccount = request.paymentAccount,
        //                SubmittedDate = formattedDate,
        //                NoteToClerk = request.note,
        //                CreatedBy = request.createdBy,
        //                selectedAttorneySec = request.selectedAttorneySec,
        //                courtesyemail = request.courtesyemail,
        //                FilingID = filingID,
        //                IsDraft = true,
        //                DraftSavedAt = formattedDate,
        //                IsExistingCase = request.isExistingCase,
        //                CaseNumber = request.isExistingCase ? request.caseNumber : null,
        //                caseTitle = request.caseTitle,
        //                courtLocation = request.courtLocation,
        //                caseTrackingID = request.caseTrackingID,
        //                Documents = request.documents?.Select(doc => new InitialCaseDetails.Documents
        //                {
        //                    DocumentType = doc.documentType,
        //                    DocumentDescription = doc.documentDescription,
        //                    FileName = doc.fileName,
        //                    FileBase64 = doc.fileBase64,
        //                    SecurityTypes = doc.securityTypes,
        //                    fee = doc.fee,
        //                    OptionalServices = doc.optionalServicesSelections?
        //                        .Select(opt => new InitialCaseDetails.OptionalServices
        //                        {
        //                            OptionalServiceId = opt.value,
        //                            Quantity = opt.Quantity ?? 0,
        //                            DocumentTypeId = doc.documentType,
        //                            fee = opt.fee,
        //                            label = opt.label,
        //                            multiplier = opt.multiplier,
        //                        }).ToList()
        //                }).ToList(),

        //                Parties = request.parties?.Select(party => new InitialCaseDetails.Parties
        //                {
        //                    SelectedPartyType = party.selectedPartyType,
        //                    RoleType = party.roleType,
        //                    LastName = party.lastName,
        //                    FirstName = party.firstName,
        //                    MiddleName = party.middleName,
        //                    Suffix = party.suffix,
        //                    CompanyName = party.companyName,
        //                    Address = party.address,
        //                    Address2 = party.address2,
        //                    City = party.city,
        //                    State = party.state,
        //                    Zip = party.zip,
        //                    AddressUnknown = party.addressUnknown,
        //                    InternationalAddress = party.internationalAddress,
        //                    SaveToAddressBook = party.saveToAddressBook,
        //                    SelectedAttorney = party.selectedAttorney
        //                }).ToList(),

        //                SelectedParties = request.selectedParties?.Select(sp => new InitialCaseDetails.SelectedParties
        //                {
        //                    PartyName = sp.partyName,
        //                    PartyType = sp.partyType,
        //                    Role = sp.role
        //                }).ToList()
        //            };

        //            await _context.InitialCases.AddAsync(newCase);
        //        }

        //        await _context.SaveChangesAsync();
        //        return Ok(new { message = existingCase != null ? "Draft updated successfully" : "Draft saved successfully", caseId = existingCase?.Id ?? filingID });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        //    }
        //}


        [HttpPost("savedraft")]
        [Authorize]
        public async Task<IActionResult> SaveDraft([FromBody] FeeCalculationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.selectedCourt))
                {
                    return BadRequest(new { message = "Required fields are missing" });
                }

                var existingCase = await _context.InitialCases
                    .Include(c => c.Documents)
                    .Include(c => c.Parties)
                    .Include(c => c.SelectedParties)
                    .FirstOrDefaultAsync(c => c.FilingID == request.filingID);

                int filingID;
                string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                string envelopeNo = existingCase?.EnvelopeNo ?? Guid.NewGuid().ToString(); // Ensure EnvelopeNo

                if (existingCase != null)
                {
                    filingID = existingCase.FilingID;

                    // Check if EnvelopeNo exists in related tables
                    bool envelopeExists = await _context.InitialCases.AnyAsync(i => i.FilingID == request.filingID);

                    if (envelopeExists)
                    {
                        // ✅ Update existing records
                        existingCase.SelectedCourt = request.selectedCourt;
                        existingCase.SelectedCategory = request.selectedCategory;
                        existingCase.SelectedCaseType = request.selectedCaseType;
                        existingCase.PaymentAccount = request.paymentAccount;
                        existingCase.SubmittedDate = formattedDate;
                        existingCase.NoteToClerk = request.note;
                        existingCase.CreatedBy = request.createdBy;
                        existingCase.selectedAttorneySec = request.selectedAttorneySec;
                        existingCase.courtesyemail = request.courtesyemail;
                        existingCase.IsDraft = true;
                        existingCase.DraftSavedAt = formattedDate;
                        existingCase.IsExistingCase = request.isExistingCase;
                        existingCase.CaseNumber = request.isExistingCase ? request.caseNumber : null;
                        existingCase.caseTitle = request.caseTitle;
                        existingCase.courtLocation = request.courtLocation;
                        existingCase.caseTrackingID = request.caseTrackingID;

                        // ✅ Update Documents
                        _context.Documents.RemoveRange(existingCase.Documents);
                        existingCase.Documents = request.documents?.Select(doc => new InitialCaseDetails.Documents
                        {
                            CaseId = existingCase.Id, // ✅ Assign CaseId
                            DocumentType = doc.documentType,
                            DocumentDescription = doc.documentDescription,
                            FileName = doc.fileName,
                            FileBase64 = doc.fileBase64,
                            SecurityTypes = doc.securityTypes,
                            fee = doc.fee,
                            EnvelopeNo = envelopeNo, // Retain EnvelopeNo
                            OptionalServices = doc.optionalServicesSelections?
                                .Select(opt => new InitialCaseDetails.OptionalServices
                                {
                                    CaseId = existingCase.Id, // ✅ Ensure CaseId is set
                                    DocumentId = 0, // Will be updated after SaveChanges
                                    OptionalServiceId = opt.value,
                                    Quantity = opt.Quantity ?? 0,
                                    DocumentTypeId = doc.documentType,
                                    fee = opt.fee,
                                    label = opt.label,
                                    multiplier = opt.multiplier,
                                }).ToList()
                        }).ToList();

                        // ✅ Update Parties
                        _context.Parties.RemoveRange(existingCase.Parties);
                        existingCase.Parties = request.parties?.Select(party => new InitialCaseDetails.Parties
                        {
                            CaseId = existingCase.Id, // ✅ Assign CaseId
                            SelectedPartyType = party.selectedPartyType,
                            RoleType = party.roleType,
                            LastName = party.lastName,
                            FirstName = party.firstName,
                            MiddleName = party.middleName,
                            Suffix = party.suffix,
                            CompanyName = party.companyName,
                            Address = party.address,
                            City = party.city,
                            State = party.state,
                            Zip = party.zip,
                            EnvelopeNo = envelopeNo, 
                            SelectedAttorney = party.selectedAttorney,
                        }).ToList();

                        // ✅ Update Selected Parties
                        _context.SelectedParties.RemoveRange(existingCase.SelectedParties);
                        existingCase.SelectedParties = request.selectedParties?.Select(sp => new InitialCaseDetails.SelectedParties
                        {
                            CaseId = existingCase.Id, // ✅ Assign CaseId
                            PartyName = sp.partyName,
                            PartyType = sp.partyType,
                            Role = sp.role,
                            EnvelopeNo = envelopeNo, // Retain EnvelopeNo
                        }).ToList();

                        _context.InitialCases.Update(existingCase);
                    }
                    else
                    {
                        // ✅ EnvelopeNo does not exist in related tables, insert as new data
                        return await InsertNewCase(request, formattedDate);
                    }
                }
                else
                {
                    // ✅ If filingID does not exist, insert new data
                    return await InsertNewCase(request, formattedDate);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Draft updated successfully", caseId = existingCase.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        private async Task<IActionResult> InsertNewCase(FeeCalculationRequest request, string formattedDate)
        {
            int lastFiling = await _context.InitialCases
                                           .OrderByDescending(c => c.FilingID)
                                           .Select(c => c.FilingID)
                                           .FirstOrDefaultAsync();
            int filingID = (lastFiling != null) ? lastFiling + 1 : 1000;

            //var filingID = await _context.InitialCases
            //    .OrderByDescending(c => c.FilingID)
            //    .Select(c => c.FilingID)
            //    .FirstOrDefaultAsync() + 1 ?? 1000;

            string newEnvelopeNo = Guid.NewGuid().ToString(); // Generate new EnvelopeNo

            var newCase = new InitialCaseDetails.Cases
            {
                SelectedCourt = request.selectedCourt,
                SelectedCategory = request.selectedCategory,
                SelectedCaseType = request.selectedCaseType,
                PaymentAccount = request.paymentAccount,
                SubmittedDate = formattedDate,
                NoteToClerk = request.note,
                CreatedBy = request.createdBy,
                selectedAttorneySec = request.selectedAttorneySec,
                courtesyemail = request.courtesyemail,
                FilingID = filingID,
                IsDraft = true,
                DraftSavedAt = formattedDate,
                IsExistingCase = request.isExistingCase,
                CaseNumber = request.isExistingCase ? request.caseNumber : null,
                caseTitle = request.caseTitle,
                courtLocation = request.courtLocation,
                caseTrackingID = request.caseTrackingID,
                EnvelopeNo = newEnvelopeNo,
                Documents = request.documents?.Select(doc => new InitialCaseDetails.Documents
                {
                    DocumentType = doc.documentType,
                    DocumentDescription = doc.documentDescription,
                    FileName = doc.fileName,
                    FileBase64 = doc.fileBase64,
                    SecurityTypes = doc.securityTypes,
                    fee = doc.fee,
                    EnvelopeNo = newEnvelopeNo,
                    OptionalServices = doc.optionalServicesSelections ?
                                .Select(opt => new InitialCaseDetails.OptionalServices
                                {
                                    OptionalServiceId = opt.value,
                                    Quantity = opt.Quantity ?? 0,
                                    DocumentTypeId = doc.documentType,
                                    fee = opt.fee,
                                    label = opt.label,
                                    multiplier = opt.multiplier,
                                }).ToList()
                }).ToList(),
                Parties = request.parties?.Select(party => new InitialCaseDetails.Parties
                {
                    SelectedPartyType = party.selectedPartyType,
                    RoleType = party.roleType,
                    LastName = party.lastName,
                    FirstName = party.firstName,
                    MiddleName = party.middleName,
                    Suffix = party.suffix,
                    CompanyName = party.companyName,
                    Address = party.address,
                    City = party.city,
                    State = party.state,
                    Zip = party.zip,
                    EnvelopeNo = newEnvelopeNo
                }).ToList(),
                SelectedParties = request.selectedParties?.Select(sp => new InitialCaseDetails.SelectedParties
                {
                    PartyName = sp.partyName,
                    PartyType = sp.partyType,
                    Role = sp.role,
                    EnvelopeNo = newEnvelopeNo
                }).ToList()
            };

            await _context.InitialCases.AddAsync(newCase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Draft saved successfully", caseId = filingID });
        }


        [HttpPost("CreateReserveFiling")]
        [Authorize]
        public async Task<IActionResult> CreateReserveFiling([FromBody] ReservedRequest request)
        {
            try
            {
                var reservedCases = _context.InitialCases.Where(c => c.SelectedCourt == "Reserved");
                _context.InitialCases.RemoveRange(reservedCases);
                await _context.SaveChangesAsync();

                var lastFiling = await _context.InitialCases
                                               .OrderByDescending(c => c.FilingID)
                                               .Select(c => c.FilingID)
                                               .FirstOrDefaultAsync();
                var filingID = (lastFiling != null) ? lastFiling + 1 : 1000;

                InitialCaseDetails.Cases newCase = new InitialCaseDetails.Cases
                {
                    FilingID = filingID,
                    SelectedCourt = "Reserved",
                    CreatedBy = "Reserved",
                    
                };

                _context.InitialCases.Add(newCase);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Case created successfully", filingID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("GetDraftCases")]
        [Authorize]
        public async Task<ActionResult<List<Cases>>> GetDraftCases()
        {
            CommonResponse resp = new CommonResponse();

            try
            {
                var draftCases = await _context.InitialCases
                      .Where(c => c.IsDraft == true) // Filter for draft cases
                      .Include(c => c.Documents)
                          .ThenInclude(d => d.OptionalServices)
                      .Include(c => c.Parties)
                      .Include(c => c.SelectedParties)
                      .OrderByDescending(c => c.Id)
                      .Select(c => new
                      {
                          c.Id,
                          c.SelectedCourt,
                          c.SelectedCategory,
                          c.SelectedCaseType,
                          c.PaymentAccount,
                          c.EnvelopeNo,
                          c.SubmittedDate,
                          c.NoteToClerk,
                          c.CreatedBy,
                          c.selectedAttorneySec,
                          c.courtesyemail,
                          c.FilingID,
                          DraftSavedAt = string.IsNullOrEmpty(c.DraftSavedAt) ? "N/A" : c.DraftSavedAt, // Fix null issue
                          c.CaseNumber,
                          c.Documents,
                          c.Parties,
                          c.SelectedParties
                      })
                      .ToListAsync();

                //return draftCases.Any() ? Ok(draftCases) : NotFound("No draft cases found.");
                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = draftCases;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("GetCaseDraftDetails/{caseId}")]
        [Authorize]
        public async Task<ActionResult<Cases>> GetCaseDraftDetails(int caseId)
        {
            try
            {
                var draftCase = await _context.InitialCases
                      .Where(c => c.Id == caseId && c.IsDraft == true) // Filter by CaseId and IsDraft = true
                      .Include(c => c.Documents)
                          .ThenInclude(d => d.OptionalServices)
                      .Include(c => c.Parties)
                      .Include(c => c.SelectedParties)
                      .Select(c => new
                      {
                          c.Id,
                          c.SelectedCourt,
                          c.SelectedCategory,
                          c.SelectedCaseType,
                          c.PaymentAccount,
                          c.EnvelopeNo,
                          c.SubmittedDate,
                          c.NoteToClerk,
                          c.CreatedBy,
                          c.selectedAttorneySec,
                          c.courtesyemail,
                          c.FilingID,
                          DraftSavedAt = string.IsNullOrEmpty(c.DraftSavedAt) ? "N/A" : c.DraftSavedAt,
                          c.CaseNumber,
                          c.caseTitle,
                          c.courtLocation,
                          c.caseTrackingID,
                          c.Documents,
                          c.Parties,
                          c.SelectedParties
                      })
                      .FirstOrDefaultAsync();

                return draftCase != null ? Ok(draftCase) : NotFound($"No draft found for CaseId: {caseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("DiscardDraft/{caseId}")]
        [Authorize]
        public async Task<ActionResult> DiscardDraft(int caseId)
        {
            try
            {
                // Find the draft case by caseId and ensure it is marked as a draft.
                var draftCase = await _context.InitialCases
                    .Where(c => c.Id == caseId && c.IsDraft == true) // Filter by CaseId and IsDraft = true
                    .Include(c => c.Documents) // Include associated documents
                    .Include(c => c.Documents).ThenInclude(d => d.OptionalServices) // Include documents' optional services
                    .Include(c => c.Parties) // Include associated parties
                    .Include(c => c.SelectedParties) // Include selected parties
                    .FirstOrDefaultAsync();

                if (draftCase == null)
                {
                    return NotFound($"No draft found for CaseId: {caseId}.");
                }

                // Delete associated documents and their optional services
                if (draftCase.Documents != null)
                {
                    foreach (var document in draftCase.Documents)
                    {
                        if (document.OptionalServices != null)
                        {
                            _context.OptionalServices.RemoveRange(document.OptionalServices); // Remove associated optional services
                        }
                    }
                    _context.Documents.RemoveRange(draftCase.Documents); // Remove associated documents
                }

                // Remove associated parties and selected parties
                if (draftCase.Parties != null)
                {
                    _context.Parties.RemoveRange(draftCase.Parties); // Remove associated parties
                }

                if (draftCase.SelectedParties != null)
                {
                    _context.SelectedParties.RemoveRange(draftCase.SelectedParties); // Remove selected parties
                }

                // Finally, remove the draft case itself
                _context.InitialCases.Remove(draftCase);

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return Ok($"All data related to CaseId {caseId} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"An error occurred while deleting the draft case and its related data: {ex.Message}");
            }
        }

        [HttpGet("GetDraftsCount")]
        [Authorize]
        public async Task<IActionResult> GetDraftsCount()
        {
            try
            {
                // Get the count of draft records
                int draftCount = await _context.InitialCases
                                               .Where(c => c.IsDraft == true)
                                               .CountAsync();

                return Ok(new
                {
                    success = true,
                    message = "Draft count fetched successfully.",
                    count = draftCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Internal server error",
                    details = ex.Message
                });
            }
        }

        [HttpPost("DiscardAllDrafts")]
        [Authorize]
        public async Task<IActionResult> DiscardAllDrafts()
        {
            try
            {
                // Fetch all drafts where IsDraft = true
                var draftCases = await _context.InitialCases
                    .Where(c => c.IsDraft == true)
                    .Include(c => c.Documents)
                        .ThenInclude(d => d.OptionalServices)
                    .Include(c => c.Parties)
                    .Include(c => c.SelectedParties)
                    .ToListAsync();

                if (!draftCases.Any())
                {
                    return NotFound(new { message = "No drafts found to delete." });
                }

                // Loop through each draft case and remove associated entities
                foreach (var draftCase in draftCases)
                {
                    if (draftCase.Documents != null)
                    {
                        foreach (var document in draftCase.Documents)
                        {
                            if (document.OptionalServices != null)
                            {
                                _context.OptionalServices.RemoveRange(document.OptionalServices);
                            }
                        }
                        _context.Documents.RemoveRange(draftCase.Documents);
                    }

                    if (draftCase.Parties != null)
                    {
                        _context.Parties.RemoveRange(draftCase.Parties);
                    }

                    if (draftCase.SelectedParties != null)
                    {
                        _context.SelectedParties.RemoveRange(draftCase.SelectedParties);
                    }
                }

                // Remove all draft cases
                _context.InitialCases.RemoveRange(draftCases);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "All draft cases and related data have been successfully deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting drafts.", details = ex.Message });
            }
        }

        [HttpGet("GeSupportStaffList")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<supportstaff>>> GeSupportStaffList()
        {
            return await _context.supportStaff.ToListAsync();
        }

        //[HttpGet("{id}")]
        [HttpGet("GetSupportStaffById/{id}")]
        [Authorize]
        public async Task<ActionResult<supportstaff>> GetSupportStaffById(int id)
        {
            CommonResponse resp = new CommonResponse();

            var supportStaff = await _context.supportStaff.FindAsync(id);

            if (supportStaff == null)
            {
                return NotFound();
            }

            resp.success = true;
            resp.status = 200;
            resp.message = "Success!";
            resp.data = supportStaff;

            return Ok(resp);
        }

        [HttpPost]
        [Route("CreateSupportStaff")]
        [Authorize]
        public async Task<ActionResult<User>> CreateSupportStaff(supportstaff staff)
        {
            _context.supportStaff.Add(staff);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupportStaffById), new { id = staff.Id }, staff);
        }

        [HttpPut]
        [Route("UpdateSupportStaff/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSupportStaff(int id, [FromBody] supportstaff staff)
        {
            CommonResponse resp = new CommonResponse();

            try
            {
                if (staff == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload" });
                }

                if (id != staff.Id)
                {
                    return BadRequest(new { success = false, message = "Mismatched ID" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
                }

                // Ensure DateTime is in UTC before saving
                if (staff.UserCreated.Kind == DateTimeKind.Unspecified)
                {
                    staff.UserCreated = DateTime.SpecifyKind(staff.UserCreated, DateTimeKind.Utc);
                }
                else
                {
                    staff.UserCreated = staff.UserCreated.ToUniversalTime();
                }


                _context.Entry(staff).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = "User Updated Successfully!";

                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server Error: " + ex.Message);  // Log error to the console
                return StatusCode(500, new { success = false, message = "Internal Server Error", error = ex.Message });
            }
        }


        [HttpPost]
        [Route("DeleteUser/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var support = await _context.supportStaff.FindAsync(id);
            if (support == null)
            {
                return NotFound();
            }

            _context.supportStaff.Remove(support);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveMatchingFiling([FromBody] MatchingFilingRequest request)
        {
            if (request == null || request.data?.MatchingFiling == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                List<Filinglist> filings = request.data.MatchingFiling.Select(f => new Filinglist
                {
                    CaseTitle = f.CaseTitle?.Value,
                    CaseNumber = f.CaseNumber?.Value,
                    CaseJudge = f.CaseJudge?.Value,
                    FilingType = f.FilingType?.Value,
                    FilingAttorney = f.FilingAttorney?.Value,
                    FilingCode = f.FilingCode?.Value,
                    OrganizationIdentificationID = f.OrganizationIdentificationID?.Value,
                    CaseCategoryCode = f.CaseCategoryCode?.Value,
                    CaseTypeCode = f.CaseTypeCode?.Value,
                    DocumentDescriptionText = f.DocumentDescriptionText?.Value,
                    DocumentFileControlID = f.DocumentFileControlID?.Value,
                    DocumentFiledDate = f.DocumentFiledDate?.Item?.Value,
                    DocumentReceivedDate = f.DocumentReceivedDate?.Item?.Value,
                    DocumentSubmitterName = f.DocumentSubmitter?.Item?.PersonName?.PersonFullName?.Value,
                    DocumentSubmitterID = f.DocumentSubmitter?.Item?.PersonOtherIdentification?.FirstOrDefault()?.IdentificationID?.Value,
                    CaseTrackingID = f.CaseTrackingID?.Value,
                    FilingStatusCode = f.FilingStatus?.FilingStatusCode,
                    StatusDescriptionText = f.FilingStatus?.StatusDescriptionText?.FirstOrDefault()?.Value,
                    ENVELOPEID = f.DocumentIdentification?.FirstOrDefault(x => x.Item?.Value == "ENVELOPEID")?.IdentificationID?.Value,
                    FILINGID = f.DocumentIdentification?.FirstOrDefault(x => x.Item?.Value == "FILINGID")?.IdentificationID?.Value
                }).ToList();

                _context.Filinglists.AddRange(filings);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Data saved successfully", count = filings.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Authorize]
        [Route("GetFilingListAll")]
        public async Task<ActionResult> GetFilingListAll()
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:GetFilingListAll");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:GetFilingListAll' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
               
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);
                var client = new RestClient(url);

                //var request = new RestRequest(Method.Get.ToString());
                var request = new RestRequest(Method.Get.ToString())
                    .AddParameter("UserName", UserName.ToString())
                    .AddParameter("Password", Password.ToString()
                    );

                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("ServeFiling")]
        public async Task<ActionResult> ServeFilingForExistingCase(ServeFilingRequest casedetail)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:ServeFilingForExistingCase");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:ServeFilingForExistingCase' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }

            try
            {

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Token is missing or invalid.");

                // Extract the token (remove "Bearer " prefix)
                var token = authHeader.Substring("Bearer ".Length);
                var UserName = _userService.GetEmailFromToken(token);

                // Query database for password hash
                var Password = await _userService.GetPasswordHashByEmailAsync(UserName);

                var client = new RestClient(url);


                //new logic for sending param to tyler api
                var restRequest = new RestRequest(Method.Get.ToString());
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("Accept", "application/json");

                // Serialize the request object to JSON
                var requestBody = JsonConvert.SerializeObject(casedetail);
                restRequest.AddParameter("UserName", UserName);
                restRequest.AddParameter("Password", Password);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(restRequest);

                string jsonResponse = response.Content;

                object jsonData = JsonConvert.DeserializeObject(response.Content);               

                resp.success = true;
                resp.status = 200;
                resp.message = "Success!";
                resp.data = jsonData;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);
            }

        }


        [HttpGet("GetCourtName")]
        [Authorize]
        public async Task<IActionResult> GetCourtName([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Court code is required.");

            var court = await _context.CourtLocations
                                      .Where(c => c.code == code)
                                      .Select(c => c.name)
                                      .FirstOrDefaultAsync();

            if (court == null)
                return NotFound("Court not found.");

            return Ok(new { courtName = court });
        }


        [HttpPost]
        [Route("SelfResendActivationEmail")]
        public async Task<ActionResult> SelfResendActivationEmail(string Email)
        {
            CommonResponse resp = new CommonResponse();

            string ip = _configuration.GetValue<string>("Tyler:TylerServicesIp");
            string url = ip + _configuration.GetValue<string>("Tyler:SelfResendActivationEmail");

            if (String.IsNullOrWhiteSpace(url))
            {
                resp.success = false;
                resp.message = "'Tyler:SelfResendActivationEmail' is null or empty.";
                resp.data = null;

                return Ok(resp);
            }           

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.Put.ToString())
                .AddParameter("Email", Email);
                RestResponse response = await client.ExecuteAsync(request);

                object jsonData = JsonConvert.DeserializeObject(response.Content);
                string result = jsonData.ToString();
               
                    resp.success = true;
                    resp.status = 200;
                    resp.message = "Success!";
                    resp.data = jsonData;               

                return Ok(resp);

            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = "Failed! " + ex.ToString();
                resp.data = null;
                return Ok(resp);

            }
        }

    }
}
