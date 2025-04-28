using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace efilling_api.Migrations
{
    public partial class gegf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttorneyDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BarId = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    FirmID = table.Column<string>(type: "text", nullable: true),
                    Suffix = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Address1 = table.Column<string>(type: "text", nullable: true),
                    Address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    MakeUserLogin = table.Column<bool>(type: "boolean", nullable: true),
                    MakeServiceContact = table.Column<bool>(type: "boolean", nullable: true),
                    MakeServiceContactPublic = table.Column<bool>(type: "boolean", nullable: true),
                    MakeFirmAdmin = table.Column<bool>(type: "boolean", nullable: true),
                    RecFilingStatusEmails = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttorneyDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseCategoryCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    ecfcasetype = table.Column<string>(type: "text", nullable: true),
                    procedureremedyinitial = table.Column<string>(type: "text", nullable: true),
                    procedureremedysubsequent = table.Column<string>(type: "text", nullable: true),
                    damageamountinitial = table.Column<string>(type: "text", nullable: true),
                    damageamountsubsequent = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseCategoryCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseTypeCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    casecategory = table.Column<string>(type: "text", nullable: true),
                    initial = table.Column<string>(type: "text", nullable: true),
                    fee = table.Column<string>(type: "text", nullable: true),
                    willfileddate = table.Column<string>(type: "text", nullable: true),
                    casestreetaddress = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTypeCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "county",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_county", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Courtdocumenttypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    filingcodeid = table.Column<string>(type: "text", nullable: true),
                    iscourtuseonly = table.Column<string>(type: "text", nullable: true),
                    isdefault = table.Column<string>(type: "text", nullable: true),
                    promptforconfirmation = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtdocumenttypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courtfilertypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    isdefault = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtfilertypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courtfiletypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    extension = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtfiletypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courtfilingcomponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    filingcodeid = table.Column<string>(type: "text", nullable: true),
                    required = table.Column<string>(type: "text", nullable: true),
                    allowmultiple = table.Column<string>(type: "text", nullable: true),
                    displayorder = table.Column<string>(type: "text", nullable: true),
                    allowedfiletypes = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtfilingcomponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourtLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    initial = table.Column<string>(type: "text", nullable: true),
                    subsequent = table.Column<string>(type: "text", nullable: true),
                    disallowcopyingenvelopemultipletimes = table.Column<string>(type: "text", nullable: true),
                    allowfilingintononindexedcase = table.Column<string>(type: "text", nullable: true),
                    allowablecardtypes = table.Column<string>(type: "text", nullable: true),
                    odysseynodeid = table.Column<string>(type: "text", nullable: true),
                    cmsid = table.Column<string>(type: "text", nullable: true),
                    sendservicebeforereview = table.Column<string>(type: "text", nullable: true),
                    parentnodeid = table.Column<string>(type: "text", nullable: true),
                    iscounty = table.Column<string>(type: "text", nullable: true),
                    restrictbankaccountpayment = table.Column<string>(type: "text", nullable: true),
                    allowmultipleattorneys = table.Column<string>(type: "text", nullable: true),
                    sendservicecontactremovednotifications = table.Column<string>(type: "text", nullable: true),
                    allowmaxfeeamount = table.Column<string>(type: "text", nullable: true),
                    transferwaivedfeestocms = table.Column<string>(type: "text", nullable: true),
                    skippreauth = table.Column<string>(type: "text", nullable: true),
                    allowhearing = table.Column<string>(type: "text", nullable: true),
                    allowreturndate = table.Column<string>(type: "text", nullable: true),
                    showdamageamount = table.Column<string>(type: "text", nullable: true),
                    hasconditionalservicetypes = table.Column<string>(type: "text", nullable: true),
                    hasprotectedcasetypes = table.Column<string>(type: "text", nullable: true),
                    protectedcasetypes = table.Column<string>(type: "text", nullable: true),
                    allowzerofeeswithoutfilingparty = table.Column<string>(type: "text", nullable: true),
                    allowserviceoninitial = table.Column<string>(type: "text", nullable: true),
                    allowaddservicecontactsoninitial = table.Column<string>(type: "text", nullable: true),
                    allowredaction = table.Column<string>(type: "text", nullable: true),
                    redactionurl = table.Column<string>(type: "text", nullable: true),
                    redactionviewerurl = table.Column<string>(type: "text", nullable: true),
                    redactionapiversion = table.Column<string>(type: "text", nullable: true),
                    enforceredaction = table.Column<string>(type: "text", nullable: true),
                    redactiondocumenttype = table.Column<string>(type: "text", nullable: true),
                    defaultdocumentdescription = table.Column<string>(type: "text", nullable: true),
                    allowwaiveronmail = table.Column<string>(type: "text", nullable: true),
                    showreturnonreject = table.Column<string>(type: "text", nullable: true),
                    protectedcasereplacementstring = table.Column<string>(type: "text", nullable: true),
                    allowchargeupdate = table.Column<string>(type: "text", nullable: true),
                    allowpartyid = table.Column<string>(type: "text", nullable: true),
                    redactionfee = table.Column<string>(type: "text", nullable: true),
                    allowwaiveronredaction = table.Column<string>(type: "text", nullable: true),
                    disallowelectronicserviceonnewcontacts = table.Column<string>(type: "text", nullable: true),
                    allowindividualregistration = table.Column<string>(type: "text", nullable: true),
                    redactiontargetconfiguration = table.Column<string>(type: "text", nullable: true),
                    bulkfilingfeeassessorconfiguration = table.Column<string>(type: "text", nullable: true),
                    envelopelevelcommentconfiguration = table.Column<string>(type: "text", nullable: true),
                    autoassignsrlservicecontact = table.Column<string>(type: "text", nullable: true),
                    autoassignattorneyservicecontact = table.Column<string>(type: "text", nullable: true),
                    partialwaiverdurationindays = table.Column<string>(type: "text", nullable: true),
                    partialwaivercourtpaymentsystemurl = table.Column<string>(type: "text", nullable: true),
                    partialwaiveravailablewaiverpercentages = table.Column<string>(type: "text", nullable: true),
                    allowrepcap = table.Column<string>(type: "text", nullable: true),
                    eserviceconsentenabled = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextblurbmain = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextblurbsecondary = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextblurbsecondaryafterconsentyes = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentyes = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentyeshelp = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentyeswithadd = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentyeswithaddhelp = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentno = table.Column<string>(type: "text", nullable: true),
                    eserviceconsenttextconsentnohelp = table.Column<string>(type: "text", nullable: true),
                    promptforconfidentialdocumentsenabled = table.Column<string>(type: "text", nullable: true),
                    promptforconfidentialdocuments = table.Column<string>(type: "text", nullable: true),
                    defaultdocumentsecurityenabled = table.Column<string>(type: "text", nullable: true),
                    defaultdocumentsecurity = table.Column<string>(type: "text", nullable: true),
                    cmsservicecontactsupdatesenabled = table.Column<string>(type: "text", nullable: true),
                    cmsservicecontactsupdatesfirmid = table.Column<string>(type: "text", nullable: true),
                    cmsservicecontactsupdatesbehavior = table.Column<string>(type: "text", nullable: true),
                    subsequentactionsenabled = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courtoptionalservices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    displayorder = table.Column<string>(type: "text", nullable: true),
                    fee = table.Column<string>(type: "text", nullable: true),
                    filingcodeid = table.Column<string>(type: "text", nullable: true),
                    multiplier = table.Column<bool>(type: "boolean", nullable: true),
                    altfeedesc = table.Column<string>(type: "text", nullable: true),
                    hasfeeprompt = table.Column<string>(type: "text", nullable: true),
                    feeprompttext = table.Column<string>(type: "text", nullable: true),
                    required = table.Column<string>(type: "text", nullable: true),
                    ismprff = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtoptionalservices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courtpartytypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    isavailablefornewparties = table.Column<bool>(type: "boolean", nullable: true),
                    casetypeid = table.Column<string>(type: "text", nullable: true),
                    isrequired = table.Column<bool>(type: "boolean", nullable: true),
                    amount = table.Column<string>(type: "text", nullable: true),
                    numberofpartiestoignore = table.Column<string>(type: "text", nullable: true),
                    sendforredaction = table.Column<string>(type: "text", nullable: true),
                    dateofdeath = table.Column<string>(type: "text", nullable: true),
                    displayorder = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courtpartytypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filingcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    fee = table.Column<string>(type: "text", nullable: true),
                    casecategory = table.Column<string>(type: "text", nullable: true),
                    casetypeid = table.Column<string>(type: "text", nullable: true),
                    filingtype = table.Column<string>(type: "text", nullable: true),
                    iscourtuseonly = table.Column<string>(type: "text", nullable: true),
                    civilclaimamount = table.Column<string>(type: "text", nullable: true),
                    probateestateamount = table.Column<string>(type: "text", nullable: true),
                    amountincontroversy = table.Column<string>(type: "text", nullable: true),
                    useduedate = table.Column<string>(type: "text", nullable: true),
                    isproposedorder = table.Column<string>(type: "text", nullable: true),
                    excludecertificateofservice = table.Column<string>(type: "text", nullable: true),
                    iswaiverrequest = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filingcodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitialCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SelectedCourt = table.Column<string>(type: "text", nullable: false),
                    SelectedCategory = table.Column<string>(type: "text", nullable: false),
                    SelectedCaseType = table.Column<string>(type: "text", nullable: false),
                    PaymentAccount = table.Column<string>(type: "text", nullable: false),
                    TotalFees = table.Column<decimal>(type: "numeric", nullable: false),
                    EnvelopeNo = table.Column<string>(type: "text", nullable: false),
                    NoteToClerk = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    SubmittedDate = table.Column<string>(type: "text", nullable: false),
                    selectedAttorneySec = table.Column<string>(type: "text", nullable: false),
                    courtesyemail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LookupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NameSuffixCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    efspcode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameSuffixCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordEncrypted = table.Column<string>(type: "text", nullable: false),
                    SecurityQuestion = table.Column<string>(type: "text", nullable: true),
                    SecurityAnswer = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Suffix = table.Column<string>(type: "text", nullable: true),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    Address1 = table.Column<string>(type: "text", nullable: true),
                    Address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    FirmName = table.Column<string>(type: "text", nullable: true),
                    IsActivated = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    RecieveFilingStatus = table.Column<bool>(type: "boolean", nullable: true),
                    CcEmails = table.Column<string>(type: "text", nullable: true),
                    RegistrationType = table.Column<string>(type: "text", nullable: true),
                    BarId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<string>(type: "text", nullable: true),
                    FirmID = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    ActivationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    ExpirationDateTime = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    password_encrypted = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    security_question = table.Column<string>(type: "text", nullable: true),
                    security_answer = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    suffix = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    organization = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    phone_no = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    address_1 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_2 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    state = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    zip_code = table.Column<string>(type: "text", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    FirmName = table.Column<string>(type: "text", nullable: true),
                    is_activated = table.Column<bool>(type: "boolean", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    filing_status = table.Column<bool>(type: "boolean", nullable: true),
                    cc_emails = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "date", nullable: true),
                    updated_at = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "court",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    has_optional_services = table.Column<bool>(type: "boolean", nullable: false),
                    transaction_fee = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    county_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_court", x => x.id);
                    table.ForeignKey(
                        name: "FK__court__county_id__3A81B327",
                        column: x => x.county_id,
                        principalTable: "county",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    EnvelopeNo = table.Column<string>(type: "text", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    DocumentDescription = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileBase64 = table.Column<string>(type: "text", nullable: false),
                    SecurityTypes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_InitialCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "InitialCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    CasesId = table.Column<int>(type: "integer", nullable: false),
                    SelectedPartyType = table.Column<string>(type: "text", nullable: false),
                    RoleType = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    Suffix = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Address2 = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Zip = table.Column<string>(type: "text", nullable: false),
                    AddressUnknown = table.Column<bool>(type: "boolean", nullable: false),
                    InternationalAddress = table.Column<string>(type: "text", nullable: false),
                    SaveToAddressBook = table.Column<bool>(type: "boolean", nullable: false),
                    SelectedAttorney = table.Column<string>(type: "text", nullable: false),
                    EnvelopeNo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_InitialCases_CasesId",
                        column: x => x.CasesId,
                        principalTable: "InitialCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedParties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    PartyName = table.Column<string>(type: "text", nullable: false),
                    PartyType = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    EnvelopeNo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedParties_InitialCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "InitialCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LookupValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LookupValue1 = table.Column<string>(type: "text", nullable: true),
                    LookupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupValues_Lookups_LookupId",
                        column: x => x.LookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "serve_only_case",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    client_matter_no = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serve_only_case", x => x.id);
                    table.ForeignKey(
                        name: "FK__serve_onl__creat__73BA3083",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_attorneys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bar_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    FirmID = table.Column<string>(type: "text", nullable: true),
                    suffix = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_1 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_2 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    state = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    zip_code = table.Column<short>(type: "smallint", nullable: true),
                    phone_no = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    make_user_login = table.Column<bool>(type: "boolean", nullable: true),
                    make_service_contact = table.Column<bool>(type: "boolean", nullable: true),
                    make_service_contact_public = table.Column<bool>(type: "boolean", nullable: true),
                    make_firm_admin = table.Column<bool>(type: "boolean", nullable: true),
                    rec_filing_status_emails = table.Column<bool>(type: "boolean", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_attorneys", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_atto__user___571DF1D5",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_parties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    role = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    type = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    suffix = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    address_1 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_2 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    state = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    zip_code = table.Column<short>(type: "smallint", nullable: true),
                    phone_no = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_unknown = table.Column<bool>(type: "boolean", nullable: true),
                    international_address = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_parties", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_part__user___5441852A",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_payment_account",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_nickname = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    account_type = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_payment_account", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_paym__user___4CA06362",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_role__user___49C3F6B7",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_service_contacts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    admin_copy_email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Address1 = table.Column<string>(type: "text", nullable: true),
                    Address2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<short>(type: "smallint", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: true),
                    Firm = table.Column<string>(type: "text", nullable: true),
                    IsFirm = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_service_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_serv__user___5165187F",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "case_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    provider_fee = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    primary_case_type = table.Column<int>(type: "integer", nullable: true),
                    case_status = table.Column<int>(type: "integer", nullable: true),
                    court_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_types", x => x.id);
                    table.ForeignKey(
                        name: "FK__case_type__court__3D5E1FD2",
                        column: x => x.court_id,
                        principalTable: "court",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "court_documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    doc_fee = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    doc_type = table.Column<int>(type: "integer", nullable: true),
                    court_id = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_court_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK__court_doc__court__4316F928",
                        column: x => x.court_id,
                        principalTable: "court",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "OptionalServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    OptionalServiceId = table.Column<string>(type: "text", nullable: false),
                    Multiplier = table.Column<bool>(type: "boolean", nullable: false),
                    EnvelopeNo = table.Column<string>(type: "text", nullable: false),
                    DocumentTypeId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionalServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionalServices_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedBarNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartyId = table.Column<int>(type: "integer", nullable: false),
                    BarNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedBarNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedBarNumbers_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_support_staff_permission",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    attorney_id = table.Column<int>(type: "integer", nullable: true),
                    is_authorized = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__admin_sup__attor__59FA5E80",
                        column: x => x.attorney_id,
                        principalTable: "user_attorneys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__admin_sup__user___59063A47",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    default_case_desc = table.Column<int>(type: "integer", nullable: true),
                    default_court_system = table.Column<int>(type: "integer", nullable: true),
                    default_court = table.Column<int>(type: "integer", nullable: true),
                    default_plaintiff = table.Column<int>(type: "integer", nullable: true),
                    default_filing_list = table.Column<int>(type: "integer", nullable: true),
                    default_screen = table.Column<int>(type: "integer", nullable: true),
                    default_service_contact_selection = table.Column<int>(type: "integer", nullable: true),
                    filing_refresh_rate = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    default_timzone = table.Column<int>(type: "integer", nullable: true),
                    filing_status_notif = table.Column<bool>(type: "boolean", nullable: true),
                    file_stamped = table.Column<bool>(type: "boolean", nullable: true),
                    detailed_filing_receipt_attached = table.Column<bool>(type: "boolean", nullable: true),
                    filing_statement_attached = table.Column<bool>(type: "boolean", nullable: true),
                    convert_to_text_pdf = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    auto_cal_fees = table.Column<bool>(type: "boolean", nullable: true),
                    filing_accepted = table.Column<bool>(type: "boolean", nullable: true),
                    filing_rejected = table.Column<bool>(type: "boolean", nullable: true),
                    filing_submitted = table.Column<bool>(type: "boolean", nullable: true),
                    service_undeliverable = table.Column<bool>(type: "boolean", nullable: true),
                    filing_submission_failed = table.Column<bool>(type: "boolean", nullable: true),
                    filing_reciepted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__user_pref__defau__76969D2E",
                        column: x => x.default_plaintiff,
                        principalTable: "user_parties",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__user_pref__defau__778AC167",
                        column: x => x.default_service_contact_selection,
                        principalTable: "user_service_contacts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__user_pref__user___75A278F5",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_type_parties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_type_id = table.Column<int>(type: "integer", nullable: true),
                    demanded_parties = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_type_parties", x => x.id);
                    table.ForeignKey(
                        name: "FK__case_type__case___403A8C7D",
                        column: x => x.case_type_id,
                        principalTable: "case_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    court_id = table.Column<int>(type: "integer", nullable: true),
                    case_type_id = table.Column<int>(type: "integer", nullable: true),
                    incident_zip_court = table.Column<int>(type: "integer", nullable: true),
                    jurisdictional_amount = table.Column<int>(type: "integer", nullable: true),
                    is_conditionally_sealed = table.Column<bool>(type: "boolean", nullable: true),
                    is_complex_case = table.Column<bool>(type: "boolean", nullable: true),
                    is_class_action = table.Column<bool>(type: "boolean", nullable: true),
                    is_asbestos = table.Column<bool>(type: "boolean", nullable: true),
                    is_cal_env_quality_act = table.Column<bool>(type: "boolean", nullable: true),
                    case_title = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    is_monetary_remedy = table.Column<bool>(type: "boolean", nullable: true),
                    is_punitive_remedy = table.Column<bool>(type: "boolean", nullable: true),
                    is_declaratory_injunctive_remedy = table.Column<bool>(type: "boolean", nullable: true),
                    no_of_causes_of_actions = table.Column<short>(type: "smallint", nullable: true),
                    rent_per_day_amount = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    street_no = table.Column<short>(type: "smallint", nullable: true),
                    street_name = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    directional = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    suffix = table.Column<int>(type: "integer", nullable: true),
                    unit_no = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    city = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    state = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    user_payment_acc_id = table.Column<int>(type: "integer", nullable: true),
                    filing_for_attorney_id = table.Column<int>(type: "integer", nullable: true),
                    total_fees = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    client_matter_no = table.Column<int>(type: "integer", nullable: true),
                    courtesy_email_notice = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    note_to_clerk = table.Column<string>(type: "text", nullable: true),
                    filing_info_is_verified = table.Column<bool>(type: "boolean", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    envelope_no = table.Column<int>(type: "integer", nullable: true),
                    Modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.id);
                    table.ForeignKey(
                        name: "FK__cases__case_type__5DCAEF64",
                        column: x => x.case_type_id,
                        principalTable: "case_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__cases__court_id__5CD6CB2B",
                        column: x => x.court_id,
                        principalTable: "court",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__cases__created_b__5EBF139D",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__cases__filing_fo__60A75C0F",
                        column: x => x.filing_for_attorney_id,
                        principalTable: "user_attorneys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__cases__user_paym__5FB337D6",
                        column: x => x.user_payment_acc_id,
                        principalTable: "user_payment_account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "court_optional_services",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    op_srv_fee = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    court_op_srv_type = table.Column<int>(type: "integer", nullable: true),
                    court_doc_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_court_optional_services", x => x.id);
                    table.ForeignKey(
                        name: "FK__court_opt__court__45F365D3",
                        column: x => x.court_doc_id,
                        principalTable: "court_documents",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "case_documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_id = table.Column<int>(type: "integer", nullable: true),
                    court_doc_id = table.Column<int>(type: "integer", nullable: true),
                    name_extension = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    file_name = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    security = table.Column<string>(type: "text", nullable: true),
                    qty = table.Column<int>(type: "integer", nullable: true),
                    court_reservation_no = table.Column<int>(type: "integer", nullable: true),
                    filed_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK__case_docu__case___6383C8BA",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_docu__court__6477ECF3",
                        column: x => x.court_doc_id,
                        principalTable: "court_documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_docu__filed__656C112C",
                        column: x => x.filed_by,
                        principalTable: "user_parties",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "case_parties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_id = table.Column<int>(type: "integer", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    company_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    suffix = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    address_1 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_2 = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    state = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    zip_code = table.Column<short>(type: "smallint", nullable: true),
                    phone_no = table.Column<string>(type: "character varying(15)", unicode: false, maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    address_unknown = table.Column<bool>(type: "boolean", nullable: true),
                    international_address = table.Column<bool>(type: "boolean", nullable: true),
                    save_to_address_book = table.Column<bool>(type: "boolean", nullable: true),
                    fee_exemption = table.Column<int>(type: "integer", nullable: true),
                    interpreter = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    representing_attorney_id = table.Column<int>(type: "integer", nullable: true),
                    filing_party = table.Column<bool>(type: "boolean", nullable: true),
                    CaseId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_parties", x => x.id);
                    table.ForeignKey(
                        name: "FK__case_part__case___68487DD7",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_part__repre__693CA210",
                        column: x => x.representing_attorney_id,
                        principalTable: "user_attorneys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_case_parties_cases_CaseId1",
                        column: x => x.CaseId1,
                        principalTable: "cases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "case_service_contacts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_id = table.Column<int>(type: "integer", nullable: true),
                    user_attorney_id = table.Column<int>(type: "integer", nullable: true),
                    user_service_contact_id = table.Column<int>(type: "integer", nullable: true),
                    e_serve = table.Column<bool>(type: "boolean", nullable: true),
                    service_type = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_service_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK__case_serv__case___6C190EBB",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_serv__user___6D0D32F4",
                        column: x => x.user_attorney_id,
                        principalTable: "user_attorneys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_serv__user___6E01572D",
                        column: x => x.user_service_contact_id,
                        principalTable: "user_service_contacts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    Created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isRead = table.Column<bool>(type: "boolean", nullable: true),
                    CaseId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK__user_noti__user___4E88ABD4",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_notifications_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "case_service_contacts_docs",
                columns: table => new
                {
                    service_contact_id = table.Column<int>(type: "integer", nullable: false),
                    case_doc_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    opened_at = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__case_serv__case___70DDC3D8",
                        column: x => x.case_doc_id,
                        principalTable: "case_documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__case_serv__servi__6FE99F9F",
                        column: x => x.service_contact_id,
                        principalTable: "case_service_contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_support_staff_permission_attorney_id",
                table: "admin_support_staff_permission",
                column: "attorney_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_support_staff_permission_user_id",
                table: "admin_support_staff_permission",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_case_id",
                table: "case_documents",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_court_doc_id",
                table: "case_documents",
                column: "court_doc_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_filed_by",
                table: "case_documents",
                column: "filed_by");

            migrationBuilder.CreateIndex(
                name: "IX_case_parties_case_id",
                table: "case_parties",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_parties_CaseId1",
                table: "case_parties",
                column: "CaseId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_case_parties_representing_attorney_id",
                table: "case_parties",
                column: "representing_attorney_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_service_contacts_case_id",
                table: "case_service_contacts",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_service_contacts_user_attorney_id",
                table: "case_service_contacts",
                column: "user_attorney_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_service_contacts_user_service_contact_id",
                table: "case_service_contacts",
                column: "user_service_contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_service_contacts_docs_case_doc_id",
                table: "case_service_contacts_docs",
                column: "case_doc_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_service_contacts_docs_service_contact_id",
                table: "case_service_contacts_docs",
                column: "service_contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_type_parties_case_type_id",
                table: "case_type_parties",
                column: "case_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_types_court_id",
                table: "case_types",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_case_type_id",
                table: "cases",
                column: "case_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_court_id",
                table: "cases",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_created_by",
                table: "cases",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_cases_filing_for_attorney_id",
                table: "cases",
                column: "filing_for_attorney_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_user_payment_acc_id",
                table: "cases",
                column: "user_payment_acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_court_county_id",
                table: "court",
                column: "county_id");

            migrationBuilder.CreateIndex(
                name: "IX_court_documents_court_id",
                table: "court_documents",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "IX_court_optional_services_court_doc_id",
                table: "court_optional_services",
                column: "court_doc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CaseId",
                table: "Documents",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupValues_LookupId",
                table: "LookupValues",
                column: "LookupId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionalServices_DocumentId",
                table: "OptionalServices",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_CasesId",
                table: "Parties",
                column: "CasesId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedBarNumbers_PartyId",
                table: "SelectedBarNumbers",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedParties_CaseId",
                table: "SelectedParties",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_serve_only_case_created_by",
                table: "serve_only_case",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_attorneys_user_id",
                table: "user_attorneys",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_CaseId",
                table: "user_notifications",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_user_id",
                table: "user_notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_parties_user_id",
                table: "user_parties",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_payment_account_user_id",
                table: "user_payment_account",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_default_plaintiff",
                table: "user_preferences",
                column: "default_plaintiff");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_default_service_contact_selection",
                table: "user_preferences",
                column: "default_service_contact_selection");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_user_id",
                table: "user_preferences",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_service_contacts_user_id",
                table: "user_service_contacts",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_support_staff_permission");

            migrationBuilder.DropTable(
                name: "AttorneyDetails");

            migrationBuilder.DropTable(
                name: "case_parties");

            migrationBuilder.DropTable(
                name: "case_service_contacts_docs");

            migrationBuilder.DropTable(
                name: "case_type_parties");

            migrationBuilder.DropTable(
                name: "CaseCategoryCodes");

            migrationBuilder.DropTable(
                name: "CaseTypeCodes");

            migrationBuilder.DropTable(
                name: "CountryCodes");

            migrationBuilder.DropTable(
                name: "court_optional_services");

            migrationBuilder.DropTable(
                name: "Courtdocumenttypes");

            migrationBuilder.DropTable(
                name: "Courtfilertypes");

            migrationBuilder.DropTable(
                name: "Courtfiletypes");

            migrationBuilder.DropTable(
                name: "Courtfilingcomponents");

            migrationBuilder.DropTable(
                name: "CourtLocations");

            migrationBuilder.DropTable(
                name: "Courtoptionalservices");

            migrationBuilder.DropTable(
                name: "Courtpartytypes");

            migrationBuilder.DropTable(
                name: "Filingcodes");

            migrationBuilder.DropTable(
                name: "LookupValues");

            migrationBuilder.DropTable(
                name: "NameSuffixCodes");

            migrationBuilder.DropTable(
                name: "OptionalServices");

            migrationBuilder.DropTable(
                name: "SelectedBarNumbers");

            migrationBuilder.DropTable(
                name: "SelectedParties");

            migrationBuilder.DropTable(
                name: "serve_only_case");

            migrationBuilder.DropTable(
                name: "user_notifications");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "UserInfos");

            migrationBuilder.DropTable(
                name: "UserResponses");

            migrationBuilder.DropTable(
                name: "case_documents");

            migrationBuilder.DropTable(
                name: "case_service_contacts");

            migrationBuilder.DropTable(
                name: "Lookups");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "court_documents");

            migrationBuilder.DropTable(
                name: "user_parties");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "user_service_contacts");

            migrationBuilder.DropTable(
                name: "InitialCases");

            migrationBuilder.DropTable(
                name: "case_types");

            migrationBuilder.DropTable(
                name: "user_attorneys");

            migrationBuilder.DropTable(
                name: "user_payment_account");

            migrationBuilder.DropTable(
                name: "court");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "county");
        }
    }
}
