using efilling_api.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace efilling_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseRelatedController : ControllerBase
    {
        //DI
        private readonly EFilling_DBContext _dbContext;

        public CaseRelatedController(EFilling_DBContext dbContext)
        {
            _dbContext = dbContext;
        }


        // 1
        // get all counties
        // GET: api/CaseRelated/GetAllCounties
        [HttpGet]
        [Route("GetAllCounties")]
        public async Task<ActionResult<IEnumerable<County>>> GetAllCounties()
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            return await _dbContext.Counties.ToListAsync();
        }

        // get county by id
        // GET: api/CaseRelated/GetCountyById/{id}
        [HttpGet("GetCountyById/{id}")]
        public async Task<ActionResult<County>> GetCountyById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var county = await _dbContext.Counties.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (county == null)
            {
                return BadRequest("No county found for that id");
            }
            else
            {
                return Ok(county);
            }
        }

        // 2
        // get all courts
        // GET: api/CaseRelated/GetAllCourts
        [HttpGet]
        [Route("GetAllCourts")]
        public async Task<ActionResult<IEnumerable<Court>>> GetAllCourts()
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            return await _dbContext.Courts.ToListAsync();
        }

        // get court by id
        // GET: api/CaseRelated/GetCourtById/{id}
        [HttpGet("GetCourtById/{id}")]
        public async Task<ActionResult<Court>> GetCourtById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var court = await _dbContext.Courts.Where(x => x.Id == id).FirstOrDefaultAsync();
            //var court = await _dbContext.Courts.Include(i => i.County).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (court == null)
            {
                return BadRequest("No court found for that id");
            }
            else
            {
                return Ok(court);
            }
        }

        // get/ list all courts of specified county ( by county id )
        // GET: api/CaseRelated/GetCourtsOfCounty/{countytId}
        [HttpGet("GetCourtsOfCounty/{countyId}")]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourtsOfCounty(int? countyId)
        {
            if (countyId == null)
            {
                return NotFound();
            }

            var courts_list = await _dbContext.Courts.Where(x => x.CountyId == countyId).ToListAsync();

            if (courts_list == null)
            {
                return BadRequest("No courts found for that county");
            }
            else
            {
                return Ok(courts_list);
            }
        }

        // 3
        // get all case types
        // GET: api/CaseRelated/GetAllCaseTypes
        [HttpGet]
        [Route("GetAllCaseTypes")]
        public async Task<ActionResult<IEnumerable<CaseType>>> GetAllCaseTypes()
        {
            if (_dbContext == null)
            {
                return NotFound();
            }
            return await _dbContext.CaseTypes.ToListAsync();
        }

        // get case type by id
        // GET: api/CaseRelated/GetCaseTypeById/{id}
        [HttpGet("GetCaseTypeById/{id}")]
        public async Task<ActionResult<CaseType>> GetCaseTypeById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var case_type = await _dbContext.CaseTypes.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (case_type == null)
            {
                return BadRequest("No case_type found for that id");
            }
            else
            {
                return Ok(case_type);
            }
        }

        // get/ list all case types of specified court ( by court id )
        // GET: api/CaseRelated/GetCaseTypesOfCourt/{courtId}
        [HttpGet("GetCaseTypesOfCourt/{courtId}")]
        public async Task<ActionResult<IEnumerable<CaseType>>> GetCaseTypesOfCourt(int? courtId)
        {
            if (courtId == null)
            {
                return NotFound();
            }

            var case_types_list = await _dbContext.CaseTypes.Where(x => x.CourtId == courtId).ToListAsync();

            if (case_types_list == null)
            {
                return BadRequest("No case types found for that court");
            }
            else
            {
                return Ok(case_types_list);
            }
        }



        //  ---------------------- Filling Status Page ----------------------------

        // 1
        // list all cases along with status, last changed, envelope, lead documents
        // GET: api/CaseRelated/GetCasesStatusDocuments/{userId}
        [HttpGet("GetCasesStatusDocuments/{userId}")]
        public async Task<ActionResult<IEnumerable<Case>>> GetCasesStatusDocuments(int userId)
        {
            //    using query syntax
            //var cases = await (from cas in _dbContext.Cases.Include(c => c.CaseDocuments)
            //                   join lvalue in _dbContext.LookupValues
            //                   on cas.Status equals lvalue.Id
            //                   select new 
            //                   {
            //                        CaseID = cas.Id,
            //                        LastChanged = cas.Modified_at,
            //                        Envelope = cas.EnvelopeNo,
            //                        Status = lvalue.LookupValue1,
            //                        LeadDocuments = cas.CaseDocuments 
            //                   }).ToListAsync();

            // using method syntax (fluent) //modified to include userId
            var cases = await (_dbContext.Cases.Include(c => c.CaseDocuments)
                                .Where(c => c.CreatedBy == userId)
                                .Join(_dbContext.LookupValues,
                                        cas => cas.Status,
                                        lval => lval.Id,
                                        (cas, lval) => new
                                        {
                                            userId = cas.CreatedBy,
                                            CaseID = cas.Id,
                                            LastChanged = cas.Modified_at,
                                            Envelope = cas.EnvelopeNo,
                                            Status = lval.LookupValue1,
                                            LeadDocuments = cas.CaseDocuments
                                        })
                                        .ToListAsync());

            if (cases == null)
            {
                NotFound("no cases found");
            }

            return Ok(cases);
        }


        // 2
        // Filter Filing Status - that will be using "datalist" along with the textbox in frontend
        // GET: api/CaseRelated/SearchCase
        [HttpGet("SearchCase")]
        public async Task<ActionResult<IEnumerable<Case>>> SearchCase(string selectedFilter,int userId)
        {
            // first get the list of all filling of that user "without any filtering"
            var casesList = await (_dbContext.Cases.Include(c => c.CaseDocuments)
                                .Where(c => c.CreatedBy == userId)
                                .Join(_dbContext.LookupValues,
                                        cas => cas.Status,
                                        lval => lval.Id,
                                        (cas, lval) => new {cas,lval}
                                        //{
                                        //    userId = ,
                                        //    CaseID = cas.Id,
                                        //    LastChanged = cas.Modified_at,
                                        //    Envelope = cas.EnvelopeNo,
                                        //    Status = lval.LookupValue1,
                                        //    LeadDocuments = cas.CaseDocuments
                                        //}
                                        )
                                        .ToListAsync());

            selectedFilter = selectedFilter.ToLower();
            var filteredCasesList = new object ();

            switch (selectedFilter)
            {
                case "all filings":
                case "my filings":
                    filteredCasesList = casesList.Select(c => new
                                                    {
                                                        userId = c.cas.CreatedBy,
                                                        CaseID = c.cas.Id,
                                                        LastChanged = c.cas.Modified_at,
                                                        Envelope = c.cas.EnvelopeNo,
                                                        Status = c.lval.LookupValue1,
                                                        LeadDocuments = c.cas.CaseDocuments
                                                    }).ToList();
                    break;
                case "status: draft":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 33)
                                                .Select(c => new
                                                    {
                                                        userId = c.cas.CreatedBy,
                                                        CaseID = c.cas.Id,
                                                        LastChanged = c.cas.Modified_at,
                                                        Envelope = c.cas.EnvelopeNo,
                                                        Status = c.lval.LookupValue1,
                                                        LeadDocuments = c.cas.CaseDocuments
                                                    }).ToList();
                        break;
                case "status: accepted":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 2)
                                                .Select(c => new
                                                {
                                                    userId = c.cas.CreatedBy,
                                                    CaseID = c.cas.Id,
                                                    LastChanged = c.cas.Modified_at,
                                                    Envelope = c.cas.EnvelopeNo,
                                                    Status = c.lval.LookupValue1,
                                                    LeadDocuments = c.cas.CaseDocuments
                                                }).ToList();
                    break;
                case "status: pending":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 1)
                                                .Select(c => new
                                                {
                                                    userId = c.cas.CreatedBy,
                                                    CaseID = c.cas.Id,
                                                    LastChanged = c.cas.Modified_at,
                                                    Envelope = c.cas.EnvelopeNo,
                                                    Status = c.lval.LookupValue1,
                                                    LeadDocuments = c.cas.CaseDocuments
                                                }).ToList();
                    break;
                case "status: rejected":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 0)
                                                .Select(c => new
                                                {
                                                    userId = c.cas.CreatedBy,
                                                    CaseID = c.cas.Id,
                                                    LastChanged = c.cas.Modified_at,
                                                    Envelope = c.cas.EnvelopeNo,
                                                    Status = c.lval.LookupValue1,
                                                    LeadDocuments = c.cas.CaseDocuments
                                                }).ToList();
                    break;
                case "status: serving":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 0)
                                                .Select(c => new
                                                {
                                                    userId = c.cas.CreatedBy,
                                                    CaseID = c.cas.Id,
                                                    LastChanged = c.cas.Modified_at,
                                                    Envelope = c.cas.EnvelopeNo,
                                                    Status = c.lval.LookupValue1,
                                                    LeadDocuments = c.cas.CaseDocuments
                                                }).ToList();
                    break;
                case "invoice: balance due":
                    filteredCasesList = casesList.Where(c => c.cas.Status == 0)
                                                .Select(c => new
                                                {
                                                    userId = c.cas.CreatedBy,
                                                    CaseID = c.cas.Id,
                                                    LastChanged = c.cas.Modified_at,
                                                    Envelope = c.cas.EnvelopeNo,
                                                    Status = c.lval.LookupValue1,
                                                    LeadDocuments = c.cas.CaseDocuments
                                                }).ToList();
                    break;
            }

            return Ok(filteredCasesList);
        }


        // -------------------------------- Intiate new case Page ---------------------------------

            // 1
            // dropdown list of (court name - primary case type - county name)
            // GET: api/CaseRelated/selectCourt
            [HttpGet("selectCourt")]
        public async Task<ActionResult<IEnumerable<Court>>> selectCourt()
        {
            // **Result should be from tables: county, court, case_types
            // Result = court.name - case_types.primary_case_type(distinct) - county.name
            // (cannot be selected from lookup values table because not all courts have all primary case types for e - filing)

            //new one with primary case types seperated 
            var result = await (_dbContext.Courts

                                .Join(_dbContext.CaseTypes,
                                        courts1 => courts1.Id,
                                        cstypes => cstypes.CourtId,
                                        (courts1, cstypes) => new { courts1, cstypes })
                                .Join(_dbContext.LookupValues,
                                        courts2 => courts2.cstypes.PrimaryCaseType,
                                        lval => lval.Id,
                                        (courts2, lval) => new { courts2, lval })
                                .Select(c => new
                                {
                                    // to form the result of : (court name - primary case type - county name) 
                                    courtName = c.courts2.courts1.Name,
                                    primaryCaseType = c.lval.LookupValue1,
                                    countyName = c.courts2.courts1.County.Name
                                })
                                .Where(c => c.countyName != null)
                                .Distinct()
                                .OrderBy(c => c.countyName)
                                )
                                .ToListAsync();

            if (result == null) { NotFound(); }

            return Ok(result);
        }


        // 2  (get all for testing)
        // dropdown list of (case type - primary case type - case status or Liability type (limited/unlimited))
        // GET: api/CaseRelated/selectCaseType
        [HttpGet("selectCaseType")]
        public async Task<ActionResult<IEnumerable<Court>>> selectCaseType()
        {
            //**case_types.name, case_types.primary_case_type(not distinct - repeated), case_types.case_status(all in Case_Types Table)
            var result = await (_dbContext.CaseTypes.Join(_dbContext.LookupValues,
                                                            c1 => c1.PrimaryCaseType,
                                                            lval1 => lval1.Id,
                                                            (c1, lval1) => new { c1, lval1 })
                                                    .Join(_dbContext.LookupValues,
                                                            c2 => c2.c1.CaseStatus,
                                                            lval2 => lval2.Id,
                                                            (c2, lval2) => new { c2, lval2 })
                                                    .Select(c => new
                                                    {
                                                        case_Type = c.c2.c1.Name,
                                                        primary_case_type = c.c2.lval1.LookupValue1,
                                                        case_liability_status = c.lval2.LookupValue1,
                                                    })
                                                    .ToListAsync());

            if (result == null) { NotFound(); }

            return Ok(result);
        }


        // 2 (get by court id)
        // dropdown list of (case type - primary case type - case status or Liability type (limited/unlimited))
        // select case types based on the selected court id selected in the 1st dropdownlist
        // GET: api/CaseRelated/selectCaseType/{courtId}
        [HttpGet("selectCaseType/{courtId}")]
        public async Task<ActionResult<IEnumerable<Court>>> selectCaseType(int? courtId)
        {
            //** Result should be in the following form:
            //   case_types.name, case_types.primary_case_type(not distinct - repeated), case_types.case_status(all in Case_Types Table)
            var result = await (_dbContext.CaseTypes.Where(c => c.CourtId == courtId)
                                                    .Join(_dbContext.LookupValues,
                                                            c1 => c1.PrimaryCaseType,
                                                            lval1 => lval1.Id,
                                                            (c1, lval1) => new { c1, lval1 })
                                                    .Join(_dbContext.LookupValues,
                                                            c2 => c2.c1.CaseStatus,
                                                            lval2 => lval2.Id,
                                                            (c2, lval2) => new { c2, lval2 })
                                                    .Select(c => new
                                                    {
                                                        case_Type = c.c2.c1.Name,
                                                        primary_case_type = c.c2.lval1.LookupValue1,
                                                        case_liability_status = c.lval2.LookupValue1,
                                                    })
                                                    .ToListAsync());

            if (result == null) { NotFound(); }

            return Ok(result);
        }


        // 3
        // dropdown list of Incident Zip & Court
        // # will display after selecting particular court and case type??
        // GET: api/CaseRelated/selectZipCodes
        [HttpGet("selectZipCodes")]
        public async Task<ActionResult<IEnumerable<LookupValue>>> selectZipCodes()
        {
            // select the zip courts from lookupValues table by the lookupId that holds the value of "incident zip and court" which is 4
            // this 4 depends on where the "Incident Zip & Court", So for my testing DB it is 4
            var result = await (_dbContext.LookupValues.Where(c => c.Lookup.Id == 4).Select(c => c.LookupValue1).ToListAsync());

            if (result == null) { NotFound(); }

            return Ok(result);
        }

        // 4
        // dropdown list of Jurisdictional Amount 
        // GET: api/CaseRelated/selectJurAmount
        [HttpGet("selectJurAmount")]
        public async Task<ActionResult<IEnumerable<LookupValue>>> selectJurAmount()
        {
            // select the Jurisdictional Amounts from lookupValues table by the lookupId that holds the value of "Jurisdictional Amount" which is 5
            var result = await (_dbContext.LookupValues.Where(c => c.Lookup.Id == 5).Select(c => c.LookupValue1).ToListAsync());

            if (result == null) { NotFound(); }

            return Ok(result);
        }


        // 5
        // dropdown list of Documents Types for the selected court 
        // GET: api/CaseRelated/selectDocTypes
        [HttpGet("selectDocTypes/{courtId}")]
        public async Task<ActionResult<IEnumerable<CourtDocument>>> selectDocTypes(int? courtId)
        {
            if (courtId == null) { NotFound("no selected court id"); }

            var documentTypes = await (_dbContext.CourtDocuments
                                        .Where(c => c.CourtId == courtId)
                                        .Join(_dbContext.LookupValues,
                                                c => c.DocType,
                                                lval => lval.Id,
                                                (c, lval) => new { c, lval })
                                        .Select(c => c.lval.LookupValue1)
                                        .ToListAsync());

            if (documentTypes == null) {
                return NotFound("no document types found for this court");
            }

            return Ok(documentTypes);
        }


        // 6
        // dropdown list of Case Party Roles
        // GET: api/CaseRelated/selectCPRole
        [HttpGet("selectCPRole")]
        public async Task<ActionResult<IEnumerable<LookupValue>>> selectCPRole()
        {
            // select the party roles from lookupValues table by the lookupId that holds the value of "Party Roles" which is 7
            var cpRoles = await (_dbContext.LookupValues.Where(c => c.Lookup.Id == 7).Select(c => c.LookupValue1).ToListAsync());

            if (cpRoles == null) { NotFound("no case party roles found for this court"); }

            return Ok(cpRoles);
        }

        // 7
        // Show/display the demanded parties for each case type( the tip box shown in Req Doc at the right side)
        // GET: api/CaseRelated/selectCPRole/{caseTypeId}
        [HttpGet("caseDemandedCPs/{caseTypeId}")]
        public async Task<ActionResult<IEnumerable<CaseTypeParty>>> caseDemandedCPs(int? caseTypeId)
        {
            // select the party roles from lookupValues table by the lookupId that holds the value of "Party Roles" which is 7
            var result = await (_dbContext.CaseTypeParties
                                    .Where(c => c.CaseTypeId == caseTypeId)
                                    .Join(_dbContext.LookupValues,
                                                c => c.DemandedParties,
                                                lval => lval.Id,
                                                (c, lval) => new { c, lval })
                                    .Select(c => c.lval.LookupValue1)
                                    .ToListAsync());

            if (result == null) { NotFound("no demanded case party roles for this case type"); }

            return Ok(result);
        }


        // 8
        // dropdown list of Case Party Types
        // GET: api/CaseRelated/selectCPType
        [HttpGet("selectCPType")]
        public async Task<ActionResult<IEnumerable<LookupValue>>> selectCPType()
        {
            // select the party type from lookupValues table by the lookupId that holds the value of "Party Types" which is 7
            var cpTypes = await (_dbContext.LookupValues.Where(c => c.Lookup.Id == 9).Select(c => c.LookupValue1).ToListAsync());

            if (cpTypes == null)
            {
                return NotFound("no party types found");
            }

            return Ok(cpTypes);
        }


        // 9
        // dropdown list of Suffix in Case Party
        // GET: api/CaseRelated/selectSuffix
        [HttpGet("selectSuffix")]
        public async Task<ActionResult<IEnumerable<LookupValue>>> selectSuffix()
        {
            // select the party type from lookupValues table by the lookupId that holds the value of "Party Types" which is 10
            var suffix = await (_dbContext.LookupValues.Where(c => c.Lookup.Id == 10).Select(c => c.LookupValue1).ToListAsync());

            if (suffix == null)
            {
                return NotFound("no party types found");
            }

            return Ok(suffix);
        }


        // 11
        // dropdown list to select from payment accounts of the user (by userPaymentAccount Nickname)
        // GET: api/CaseRelated/selectPayment/{userId} 
        [HttpGet("selectPayment/{userId}")]
        public async Task<ActionResult<IEnumerable<UserPaymentAccount>>> selectPayment(int? userId)
        {
            var result = await (_dbContext.UserPaymentAccounts
                                .Where(c => c.UserId == userId)
                                .Select(c => c.AccountNickname)
                                .ToListAsync());

            if (result == null) { NotFound("no payment accounts found"); }

            return Ok(result);
        }


        // 12
        // Create/add/post a new case
        // POST: api/CaseRelated/PostCase
        [HttpPost("PostCase")]
        public async Task<ActionResult<IEnumerable<Case>>> PostCase(Case case1)
        {
            _dbContext.Cases.Add(case1);
            await _dbContext.SaveChangesAsync();

            //return CreatedAtAction("GetCase", new { id = case1.Id }, case1);
            return Ok(case1);
        }


        // GET: api/CaseRelated/GetCase/{Id} 
        [HttpGet("GetCase/{Id}")]
        public async Task<ActionResult<Case>> GetCase(int? Id)
        {
            var case1 = await (_dbContext.Cases
                                .Where(c => c.Id == Id)
                                .FirstAsync());

            if (case1 == null)
            {
                return NotFound("no case found");
            }

            return Ok(case1);
        }


        // 13
        // Save case as a draft (change status of case to draft)
        // Patch: api/CaseRelated/PatchCase
        [HttpPatch("PatchCase/{caseId:int}")]
        public async Task<ActionResult> PatchCase(int caseId, JsonPatchDocument<Case> patchCase)
        {
            var updateCase = await _dbContext.Cases.Where(c => c.Id == caseId).FirstOrDefaultAsync();


            if (updateCase == null)
            {
                //display error if the case does not exist in the DB
                return NotFound();

                //add the case to DB
                //_dbContext.Cases.Add(updateCase);
            }

            patchCase.ApplyTo(updateCase, ModelState);

            await _dbContext.SaveChangesAsync();

            return Ok(updateCase);
        }

        //-------------- start of Service Contacts ---------------------

        //14
        // list Service contacts to case (From My firm ) in modal
        // and in " ------ service contact page ------" too
        // show service contacts added by the same user
        // GET: api/CaseRelated/GetSCMyFirm/{userId}
        [HttpGet("GetSCMyFirm/{userId}")]
        public async Task<ActionResult<IEnumerable<UserServiceContact>>> GetSCMyFirm(int? userId)
        {
            if(userId == null) 
            {
                return BadRequest("no user id passed!");
            }

            var SCList = await (_dbContext.UserServiceContacts
                                .Where(c => c.UserId == userId)
                                .Select ( c => new 
                                {
                                    lastName = c.LastName,
                                    firstName = c.FirstName,
                                    email = c.Email,
                                    firmName = c.Firm,
                                    publicContact = c.IsPublic
                                })
                                .ToListAsync());

            if (SCList == null)
            {
                return NotFound("no service contact found");
            }

            return Ok(SCList);
        }

        //15
        // list Service contact to case (From Public list)    -- (search button) in modal
        //   ##but by searching with three possible inputs (lastName, firstName, firmName)
        //  show service contacts that are public and statisfies the condition of searching button
        // GET: api/CaseRelated/GetSCPublic
        [HttpGet("GetSCPublic")]
        public async Task<ActionResult<IEnumerable<UserServiceContact>>> GetSCPublic(string? lName, string? fName, string? firm)
        {
            // get list of all service contacts that are public 
            var SCList = await (_dbContext.UserServiceContacts
                                .Where(c => c.IsPublic == true)
                                .ToListAsync());

            if (SCList != null)
            {
                // get list of service contacts based on the three inputs of the search
                var filteredList = SCList
                                    //.AsEnumerable()
                                    .Where(s => (String.IsNullOrWhiteSpace(lName) ? 1 == 1 : s.LastName.Contains( lName.ToLower() ) )
                                             && (String.IsNullOrWhiteSpace(fName) ? 1 == 1 : s.FirstName.Contains(fName.ToLower() ) )
                                             && (String.IsNullOrWhiteSpace(firm)  ? 1 == 1 : s.Firm.Contains     (firm.ToLower()  ) )  )
                                    .Select( s => new
                                    {
                                        lastName = s.LastName,
                                        firstName = s.FirstName,
                                        email = s.Email,
                                        firmName = s.Firm
                                    } )
                                    .ToList();
                if ((filteredList == null) || (filteredList.Count == 0))
                {
                    return BadRequest("no service contact found by this input");
                }

                return Ok(filteredList);
            }
            else
            {
                return BadRequest("no public service contact found");
            }

        }

        //16
        // Add new Service contact to case & user (in modal)
        // POST: api/CaseRelated/PostServiceContactInModal/{caseId} or ?caseId=2
        [HttpPost("PostServiceContactInModal")]
        public ActionResult PostServiceContactInModal(UserServiceContact userServiceContact, int caseId)
        {
            if (userServiceContact == null) 
            {
                return BadRequest("please fill the details of the service contact");
            }

            // check if user selected "yes"=true option in "Firm", then assign "individual" to Firm name
            if (userServiceContact.IsFirm == true)
            {
                userServiceContact.Firm = "individual";
            }

            // add the user service contact
            _dbContext.UserServiceContacts.Add(userServiceContact);
            // save changes
            _dbContext.SaveChanges();

            //create a new case service contact, then assign the corresponding values to it
            CaseServiceContact newCaseSC = new CaseServiceContact
            {
                UserServiceContactId = userServiceContact.Id,
                CaseId = caseId
            };
            // add the new case service contact 
            _dbContext.CaseServiceContacts.Add(newCaseSC);
            // save changes
            _dbContext.SaveChanges();
            
            return Ok("user and case service contact added");
        }


        //17
        // (Add/Attach) button in modal
        // add the selected service contact 
        //POST: api/CaseRelated/PostServiceContacts/{caseId}
        [HttpPost("PostServiceContacts/{caseId}")]
        public async Task<ActionResult> PostServiceContacts([FromBody] int[] userSCId, int caseId)
        {
            List<CaseServiceContact> newCaseCSList = new List<CaseServiceContact>();

            foreach (var id in userSCId)
            {
                CaseServiceContact newCaseSC = new CaseServiceContact();

                newCaseSC.UserServiceContactId = id;
                newCaseSC.CaseId = caseId;

                newCaseCSList.Add(newCaseSC);
            }

            await _dbContext.CaseServiceContacts.AddRangeAsync(newCaseCSList);
            _dbContext.SaveChanges();

            return Ok("Sucuessfully added the selected user Service Contacts to the Case");
        }



        //18
        // show/display case service contact of case (by case id) -- outside modal
        // GET: api/CaseRelated/GetCaseSCs/{caseId}
        [HttpGet("GetCaseSCs/{caseId}")]
        public async Task<ActionResult<IEnumerable<CaseServiceContact>>> GetCaseSCs(int caseId)
        {
            var CaseSCsList = await( _dbContext.CaseServiceContacts
                                        .Where(s => s.CaseId == caseId)
                                        .Join( _dbContext.UserServiceContacts,
                                                u => u.UserServiceContactId,
                                                c => c.Id,
                                                (u,c) => new {u,c})
                                        .Select( s => new
                                        {
                                            id = s.u.Id,
                                            fName = s.c.FirstName,
                                            lName = s.c.LastName,
                                            address = s.c.Address1,
                                            city = s.c.City,
                                            state = s.c.State,
                                            zip = s.c.ZipCode,
                                            email = s.c.Email,
                                            party = s.u.ServiceType
                                        })
                                        .ToListAsync());
            if (CaseSCsList.Count > 0)
            {
                return Ok(CaseSCsList);
            }
            else
            {
                return BadRequest("no SC to show!");
            }
        }
        //-------------- end of Service Contacts ---------------------



    }
}
