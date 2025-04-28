using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static efilling_api.Models.InitialCaseDetails;

namespace efilling_api.Models
{
    public partial class EFilling_DBContext : DbContext
    {

        public EFilling_DBContext(DbContextOptions<EFilling_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminSupportStaffPermission> AdminSupportStaffPermissions { get; set; } = null!;
        public virtual DbSet<Case> Cases { get; set; } = null!;
        public virtual DbSet<CaseDocument> CaseDocuments { get; set; } = null!;
        public virtual DbSet<CaseParty> CaseParties { get; set; } = null!;
        public virtual DbSet<CaseServiceContact> CaseServiceContacts { get; set; } = null!;
        public virtual DbSet<CaseServiceContactsDoc> CaseServiceContactsDocs { get; set; } = null!;
        public virtual DbSet<CaseType> CaseTypes { get; set; } = null!;
        public virtual DbSet<CaseTypeParty> CaseTypeParties { get; set; } = null!;
        public virtual DbSet<County> Counties { get; set; } = null!;
        public virtual DbSet<Court> Courts { get; set; } = null!;
        public virtual DbSet<CourtDocument> CourtDocuments { get; set; } = null!;
        public virtual DbSet<CourtOptionalService> CourtOptionalServices { get; set; } = null!;
        public virtual DbSet<ServeOnlyCase> ServeOnlyCases { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserAttorney> UserAttorneys { get; set; } = null!;
        public virtual DbSet<UserNotification> UserNotifications { get; set; } = null!;
        public virtual DbSet<UserParty> UserParties { get; set; } = null!;
        public virtual DbSet<UserPaymentAccount> UserPaymentAccounts { get; set; } = null!;
        public virtual DbSet<UserPreference> UserPreferences { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<UserServiceContact> UserServiceContacts { get; set; } = null!;
        //new
        public virtual DbSet<Lookup> Lookups { get; set; } = null!;
        public virtual DbSet<LookupValue> LookupValues { get; set; } = null!;
        public virtual DbSet<UserInfo> UserInfos { get; set; } = null!;
        public virtual DbSet<UserResponse> UserResponses { get; set; } = null!;
        public virtual DbSet<courtlocations> CourtLocations { get; set; } = null!;
        public virtual DbSet<filingcodes> Filingcodes { get; set; } = null!;
        public virtual DbSet<CaseCategoryCodes> CaseCategoryCodes { get; set; } = null!;
        public virtual DbSet<CaseTypeCodes> CaseTypeCodes { get; set; } = null!;
        public virtual DbSet<CountryCodes> CountryCodes { get; set; } = null!;
        public virtual DbSet<Courtdocumenttype> Courtdocumenttypes { get; set; } = null!;
        public virtual DbSet<Courtfilertype> Courtfilertypes { get; set; } = null!;
        public virtual DbSet<Courtfiletype> Courtfiletypes { get; set; } = null!;
        public virtual DbSet<Courtfilingcomponent> Courtfilingcomponents { get; set; } = null!;
        public virtual DbSet<Courtoptionalservices>  Courtoptionalservices { get; set; } = null!;
        //public virtual DbSet<UserLogin> UserLogins { get; set; } = null!;
        public virtual DbSet<AttorneyDetails> AttorneyDetails { get; set; } = null!;
        public virtual DbSet<Courtpartytype> Courtpartytypes { get; set; } = null!;
        public virtual DbSet<NameSuffixCodes> NameSuffixCodes { get; set; }
        public DbSet<Cases> InitialCases { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<OptionalServices> OptionalServices { get; set; }
        public DbSet<Parties> Parties { get; set; }
        public DbSet<SelectedBarNumber> SelectedBarNumbers { get; set; }
        public DbSet<SelectedParties> SelectedParties { get; set; }
        public DbSet<supportstaff> supportStaff { get; set; }
        public DbSet<Filinglist> Filinglists { get; set; } // Check this name


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=MAHA\\SQLEXPRESS01;Database=EFilling_DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminSupportStaffPermission>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("admin_support_staff_permission");

                entity.Property(e => e.AttorneyId).HasColumnName("attorney_id");

                entity.Property(e => e.IsAuthorized).HasColumnName("is_authorized");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Attorney)
                    .WithMany()
                    .HasForeignKey(d => d.AttorneyId)
                    .HasConstraintName("FK__admin_sup__attor__59FA5E80");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__admin_sup__user___59063A47");
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.ToTable("cases");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaseTitle)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("case_title");

                entity.Property(e => e.CaseTypeId).HasColumnName("case_type_id");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.ClientMatterNo).HasColumnName("client_matter_no");

                entity.Property(e => e.CourtId).HasColumnName("court_id");

                entity.Property(e => e.CourtesyEmailNotice)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("courtesy_email_notice");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Directional)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("directional");

                entity.Property(e => e.EnvelopeNo).HasColumnName("envelope_no");

                entity.Property(e => e.FilingForAttorneyId).HasColumnName("filing_for_attorney_id");

                entity.Property(e => e.FilingInfoIsVerified).HasColumnName("filing_info_is_verified");

                entity.Property(e => e.IncidentZipCourt).HasColumnName("incident_zip_court");

                entity.Property(e => e.IsAsbestos).HasColumnName("is_asbestos");

                entity.Property(e => e.IsCalEnvQualityAct).HasColumnName("is_cal_env_quality_act");

                entity.Property(e => e.IsClassAction).HasColumnName("is_class_action");

                entity.Property(e => e.IsComplexCase).HasColumnName("is_complex_case");

                entity.Property(e => e.IsConditionallySealed).HasColumnName("is_conditionally_sealed");

                entity.Property(e => e.IsDeclaratoryInjunctiveRemedy).HasColumnName("is_declaratory_injunctive_remedy");

                entity.Property(e => e.IsMonetaryRemedy).HasColumnName("is_monetary_remedy");

                entity.Property(e => e.IsPunitiveRemedy).HasColumnName("is_punitive_remedy");

                entity.Property(e => e.JurisdictionalAmount).HasColumnName("jurisdictional_amount");

                entity.Property(e => e.NoOfCausesOfActions).HasColumnName("no_of_causes_of_actions");

                entity.Property(e => e.NoteToClerk)
                    .HasColumnType("text")
                    .HasColumnName("note_to_clerk");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("postal_code");

                entity.Property(e => e.RentPerDayAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("rent_per_day_amount");

                entity.Property(e => e.State)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StreetName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("street_name");

                entity.Property(e => e.StreetNo).HasColumnName("street_no");

                entity.Property(e => e.Suffix)
                    //.HasMaxLength(10)
                    //.IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.TotalFees)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("total_fees");

                entity.Property(e => e.UnitNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("unit_no");

                entity.Property(e => e.UserPaymentAccId).HasColumnName("user_payment_acc_id");

                entity.HasOne(d => d.CaseType)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.CaseTypeId)
                    .HasConstraintName("FK__cases__case_type__5DCAEF64");

                entity.HasOne(d => d.Court)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.CourtId)
                    .HasConstraintName("FK__cases__court_id__5CD6CB2B");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__cases__created_b__5EBF139D");

                entity.HasOne(d => d.FilingForAttorney)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.FilingForAttorneyId)
                    .HasConstraintName("FK__cases__filing_fo__60A75C0F");

                entity.HasOne(d => d.UserPaymentAcc)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.UserPaymentAccId)
                    .HasConstraintName("FK__cases__user_paym__5FB337D6");
            });

            modelBuilder.Entity<CaseDocument>(entity =>
            {
                entity.ToTable("case_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaseId).HasColumnName("case_id");

                entity.Property(e => e.CourtDocId).HasColumnName("court_doc_id");

                entity.Property(e => e.CourtReservationNo).HasColumnName("court_reservation_no");

                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("file_name");

                entity.Property(e => e.FiledBy).HasColumnName("filed_by");

                entity.Property(e => e.NameExtension)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("name_extension");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Security).HasColumnName("security");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CaseDocuments)
                    .HasForeignKey(d => d.CaseId)
                    .HasConstraintName("FK__case_docu__case___6383C8BA");

                entity.HasOne(d => d.CourtDoc)
                    .WithMany(p => p.CaseDocuments)
                    .HasForeignKey(d => d.CourtDocId)
                    .HasConstraintName("FK__case_docu__court__6477ECF3");

                entity.HasOne(d => d.FiledByNavigation)
                    .WithMany(p => p.CaseDocuments)
                    .HasForeignKey(d => d.FiledBy)
                    .HasConstraintName("FK__case_docu__filed__656C112C");
            });

            modelBuilder.Entity<CaseParty>(entity =>
            {
                entity.ToTable("case_parties");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_2");

                entity.Property(e => e.AddressUnknown).HasColumnName("address_unknown");

                entity.Property(e => e.CaseId).HasColumnName("case_id");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("company_name");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FeeExemption).HasColumnName("fee_exemption");

                entity.Property(e => e.FilingParty).HasColumnName("filing_party");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.InternationalAddress).HasColumnName("international_address");

                entity.Property(e => e.Interpreter)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("interpreter");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_no");

                entity.Property(e => e.RepresentingAttorneyId).HasColumnName("representing_attorney_id");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.SaveToAddressBook).HasColumnName("save_to_address_book");

                entity.Property(e => e.State)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.ZipCode).HasColumnName("zip_code");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CaseParties)
                    .HasForeignKey(d => d.CaseId)
                    .HasConstraintName("FK__case_part__case___68487DD7");

                entity.HasOne(d => d.RepresentingAttorney)
                    .WithMany(p => p.CaseParties)
                    .HasForeignKey(d => d.RepresentingAttorneyId)
                    .HasConstraintName("FK__case_part__repre__693CA210");
            });

            modelBuilder.Entity<CaseServiceContact>(entity =>
            {
                entity.ToTable("case_service_contacts");

                entity.Property(e => e.Id).HasColumnName("id");

                //entity.Property(e => e.Address1)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("address_1");

                //entity.Property(e => e.Address2)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("address_2");

                //entity.Property(e => e.AdminCopyEmail)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("admin_copy_email");

                entity.Property(e => e.CaseId).HasColumnName("case_id");

                //entity.Property(e => e.City)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("city");

                entity.Property(e => e.EServe).HasColumnName("e_serve");

                //entity.Property(e => e.Email)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("email");

                //entity.Property(e => e.FirstName)
                //    .HasMaxLength(50)
                //    .IsUnicode(false)
                //    .HasColumnName("first_name");

                //entity.Property(e => e.IsPublic).HasColumnName("is_public");

                //entity.Property(e => e.LastName)
                //    .HasMaxLength(50)
                //    .IsUnicode(false)
                //    .HasColumnName("last_name");

                //entity.Property(e => e.MiddleName)
                //    .HasMaxLength(50)
                //    .IsUnicode(false)
                //    .HasColumnName("middle_name");

                //entity.Property(e => e.PhoneNo)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("phone_no");

                entity.Property(e => e.ServiceType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("service_type");

                //entity.Property(e => e.State)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("state");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserAttorneyId).HasColumnName("user_attorney_id");

                entity.Property(e => e.UserServiceContactId).HasColumnName("user_service_contact_id");

                //entity.Property(e => e.ZipCode).HasColumnName("zip_code");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CaseServiceContacts)
                    .HasForeignKey(d => d.CaseId)
                    .HasConstraintName("FK__case_serv__case___6C190EBB");

                entity.HasOne(d => d.UserAttorney)
                    .WithMany(p => p.CaseServiceContacts)
                    .HasForeignKey(d => d.UserAttorneyId)
                    .HasConstraintName("FK__case_serv__user___6D0D32F4");

                entity.HasOne(d => d.UserServiceContact)
                    .WithMany(p => p.CaseServiceContacts)
                    .HasForeignKey(d => d.UserServiceContactId)
                    .HasConstraintName("FK__case_serv__user___6E01572D");
            });

            modelBuilder.Entity<CaseServiceContactsDoc>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("case_service_contacts_docs");

                entity.Property(e => e.CaseDocId).HasColumnName("case_doc_id");

                entity.Property(e => e.OpenedAt)
                    .HasColumnType("date")
                    .HasColumnName("opened_at");

                entity.Property(e => e.ServiceContactId).HasColumnName("service_contact_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.HasOne(d => d.CaseDoc)
                    .WithMany()
                    .HasForeignKey(d => d.CaseDocId)
                    .HasConstraintName("FK__case_serv__case___70DDC3D8");

                entity.HasOne(d => d.ServiceContact)
                    .WithMany()
                    .HasForeignKey(d => d.ServiceContactId)
                    .HasConstraintName("FK__case_serv__servi__6FE99F9F");
            });

            modelBuilder.Entity<CaseType>(entity =>
            {
                entity.ToTable("case_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaseStatus).HasColumnName("case_status");

                entity.Property(e => e.CourtId).HasColumnName("court_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.PrimaryCaseType).HasColumnName("primary_case_type");

                entity.Property(e => e.ProviderFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("provider_fee");

                entity.HasOne(d => d.Court)
                    .WithMany(p => p.CaseTypes)
                    .HasForeignKey(d => d.CourtId)
                    .HasConstraintName("FK__case_type__court__3D5E1FD2");
            });

            modelBuilder.Entity<CaseTypeParty>(entity =>
            {
                entity.ToTable("case_type_parties");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaseTypeId).HasColumnName("case_type_id");

                entity.Property(e => e.DemandedParties).HasColumnName("demanded_parties");

                entity.HasOne(d => d.CaseType)
                    .WithMany(p => p.CaseTypeParties)
                    .HasForeignKey(d => d.CaseTypeId)
                    .HasConstraintName("FK__case_type__case___403A8C7D");
            });

            modelBuilder.Entity<County>(entity =>
            {
                entity.ToTable("county");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Court>(entity =>
            {
                entity.ToTable("court");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountyId).HasColumnName("county_id");

                entity.Property(e => e.HasOptionalServices).HasColumnName("has_optional_services");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.TransactionFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transaction_fee");

                entity.HasOne(d => d.County)
                    .WithMany(p => p.Courts)
                    .HasForeignKey(d => d.CountyId)
                    .HasConstraintName("FK__court__county_id__3A81B327");
            });

            modelBuilder.Entity<CourtDocument>(entity =>
            {
                entity.ToTable("court_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourtId).HasColumnName("court_id");

                entity.Property(e => e.DocFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("doc_fee");

                entity.Property(e => e.DocType).HasColumnName("doc_type");

                entity.HasOne(d => d.Court)
                    .WithMany(p => p.CourtDocuments)
                    .HasForeignKey(d => d.CourtId)
                    .HasConstraintName("FK__court_doc__court__4316F928");
            });

            modelBuilder.Entity<CourtOptionalService>(entity =>
            {
                entity.ToTable("court_optional_services");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourtDocId).HasColumnName("court_doc_id");

                entity.Property(e => e.CourtOpSrvType).HasColumnName("court_op_srv_type");

                entity.Property(e => e.OpSrvFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("op_srv_fee");

                entity.HasOne(d => d.CourtDoc)
                    .WithMany(p => p.CourtOptionalServices)
                    .HasForeignKey(d => d.CourtDocId)
                    .HasConstraintName("FK__court_opt__court__45F365D3");
            });

            modelBuilder.Entity<ServeOnlyCase>(entity =>
            {
                entity.ToTable("serve_only_case");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClientMatterNo).HasColumnName("client_matter_no");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ServeOnlyCases)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__serve_onl__creat__73BA3083");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_2");

                entity.Property(e => e.CcEmails)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("cc_emails");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("created_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.RecieveFilingStatus).HasColumnName("filing_status");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsActivated).HasColumnName("is_activated");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                entity.Property(e => e.Organization)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("organization");

                entity.Property(e => e.PasswordEncrypted)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password_encrypted");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_no");

                entity.Property(e => e.SecurityAnswer)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("security_answer");

                entity.Property(e => e.SecurityQuestion).HasColumnName("security_question");

                entity.Property(e => e.State)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("date")
                    .HasColumnName("updated_at");

                entity.Property(e => e.ZipCode).HasColumnName("zip_code");
            });

            modelBuilder.Entity<UserAttorney>(entity =>
            {
                entity.ToTable("user_attorneys");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_2");

                entity.Property(e => e.BarId).HasColumnName("bar_id");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MakeFirmAdmin).HasColumnName("make_firm_admin");

                entity.Property(e => e.MakeServiceContact).HasColumnName("make_service_contact");

                entity.Property(e => e.MakeServiceContactPublic).HasColumnName("make_service_contact_public");

                entity.Property(e => e.MakeUserLogin).HasColumnName("make_user_login");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_no");

                entity.Property(e => e.RecFilingStatusEmails).HasColumnName("rec_filing_status_emails");

                entity.Property(e => e.State)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ZipCode).HasColumnName("zip_code");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAttorneys)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_atto__user___571DF1D5");
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                //entity.HasNoKey();

                entity.ToTable("user_notifications");

                //entity.Property(e => e.Notification)
                //    .HasColumnType("text")
                //    .HasColumnName("notification");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_noti__user___4E88ABD4");
            });

            modelBuilder.Entity<UserParty>(entity =>
            {
                entity.ToTable("user_parties");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address_2");

                entity.Property(e => e.AddressUnknown).HasColumnName("address_unknown");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.InternationalAddress).HasColumnName("international_address");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_no");

                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.Property(e => e.State)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ZipCode).HasColumnName("zip_code");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserParties)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_part__user___5441852A");
            });

            modelBuilder.Entity<UserPaymentAccount>(entity =>
            {
                entity.ToTable("user_payment_account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountNickname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_nickname");

                entity.Property(e => e.AccountType).HasColumnName("account_type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPaymentAccounts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_paym__user___4CA06362");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("user_preferences");

                entity.Property(e => e.AutoCalFees).HasColumnName("auto_cal_fees");

                entity.Property(e => e.ConvertToTextPdf)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("convert_to_text_pdf");

                entity.Property(e => e.DefaultCaseDesc).HasColumnName("default_case_desc");

                entity.Property(e => e.DefaultCourt).HasColumnName("default_court");

                entity.Property(e => e.DefaultCourtSystem).HasColumnName("default_court_system");

                entity.Property(e => e.DefaultFilingList).HasColumnName("default_filing_list");

                entity.Property(e => e.DefaultPlaintiff).HasColumnName("default_plaintiff");

                entity.Property(e => e.DefaultScreen).HasColumnName("default_screen");

                entity.Property(e => e.DefaultServiceContactSelection).HasColumnName("default_service_contact_selection");

                entity.Property(e => e.DefaultTimzone).HasColumnName("default_timzone");

                entity.Property(e => e.DetailedFilingReceiptAttached).HasColumnName("detailed_filing_receipt_attached");

                entity.Property(e => e.FileStamped).HasColumnName("file_stamped");

                entity.Property(e => e.FilingAccepted).HasColumnName("filing_accepted");

                entity.Property(e => e.FilingReciepted).HasColumnName("filing_reciepted");

                entity.Property(e => e.FilingRefreshRate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("filing_refresh_rate");

                entity.Property(e => e.FilingRejected).HasColumnName("filing_rejected");

                entity.Property(e => e.FilingStatementAttached).HasColumnName("filing_statement_attached");

                entity.Property(e => e.FilingStatusNotif).HasColumnName("filing_status_notif");

                entity.Property(e => e.FilingSubmissionFailed).HasColumnName("filing_submission_failed");

                entity.Property(e => e.FilingSubmitted).HasColumnName("filing_submitted");

                entity.Property(e => e.ServiceUndeliverable).HasColumnName("service_undeliverable");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.DefaultPlaintiffNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.DefaultPlaintiff)
                    .HasConstraintName("FK__user_pref__defau__76969D2E");

                entity.HasOne(d => d.DefaultServiceContactSelectionNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.DefaultServiceContactSelection)
                    .HasConstraintName("FK__user_pref__defau__778AC167");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_pref__user___75A278F5");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                //entity.HasNoKey();

                entity.ToTable("user_roles");

                //entity.Property(e => e.UserAccountType).HasColumnName("user_account_type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_role__user___49C3F6B7");
            });

            modelBuilder.Entity<UserServiceContact>(entity =>
            {
                entity.ToTable("user_service_contacts");

                entity.Property(e => e.Id).HasColumnName("id");

                //entity.Property(e => e.Address1)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("address_1");

                //entity.Property(e => e.Address2)
                //    .HasMaxLength(100)
                //    .IsUnicode(false)
                //    .HasColumnName("address_2");

                entity.Property(e => e.AdminCopyEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("admin_copy_email");

                //entity.Property(e => e.City)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsPublic).HasColumnName("is_public");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                //entity.Property(e => e.PhoneNo)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("phone_no");

                //entity.Property(e => e.State)
                //    .HasMaxLength(15)
                //    .IsUnicode(false)
                //    .HasColumnName("state");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                //entity.Property(e => e.ZipCode).HasColumnName("zip_code");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserServiceContacts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_serv__user___5165187F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
