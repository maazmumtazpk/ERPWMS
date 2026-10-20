using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ERPWMS.Database.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountLedger> AccountLedgers { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    public virtual DbSet<BankReconciliation> BankReconciliations { get; set; }

    public virtual DbSet<BankTransaction> BankTransactions { get; set; }

    public virtual DbSet<BatchTracking> BatchTrackings { get; set; }

    public virtual DbSet<Bin> Bins { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerComplaint> CustomerComplaints { get; set; }

    public virtual DbSet<CustomerContact> CustomerContacts { get; set; }

    public virtual DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }

    public virtual DbSet<CustomerInquiry> CustomerInquiries { get; set; }

    public virtual DbSet<CycleCount> CycleCounts { get; set; }

    public virtual DbSet<CycleCountDetail> CycleCountDetails { get; set; }

    public virtual DbSet<DashboardWidget> DashboardWidgets { get; set; }

    public virtual DbSet<DataBackupLog> DataBackupLogs { get; set; }

    public virtual DbSet<DeliveryChallan> DeliveryChallans { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<DispatchDetail> DispatchDetails { get; set; }

    public virtual DbSet<DispatchMaster> DispatchMasters { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }

    public virtual DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; set; }

    public virtual DbSet<EmployeeLeaf> EmployeeLeaves1 { get; set; }

    public virtual DbSet<EmployeeLeave> EmployeeLeaves { get; set; }

    public virtual DbSet<EmployeePayroll> EmployeePayrolls { get; set; }

    public virtual DbSet<EmployeePayrollDetail> EmployeePayrollDetails { get; set; }

    public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }

    public virtual DbSet<EmployeeSalary1> EmployeeSalaries1 { get; set; }

    public virtual DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Grn> Grns { get; set; }

    public virtual DbSet<Grndetail> Grndetails { get; set; }

    public virtual DbSet<InventoryValuation> InventoryValuations { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<JournalVoucher> JournalVouchers { get; set; }

    public virtual DbSet<JournalVoucherDetail> JournalVoucherDetails { get; set; }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModulePermission> ModulePermissions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PaymentVoucher> PaymentVouchers { get; set; }

    public virtual DbSet<PaymentVoucherDetail> PaymentVoucherDetails { get; set; }

    public virtual DbSet<PickingDetail> PickingDetails { get; set; }

    public virtual DbSet<PickingList> PickingLists { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductPricingHistory> ProductPricingHistories { get; set; }

    public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }

    public virtual DbSet<PurchaseMaster> PurchaseMasters { get; set; }

    public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatuses { get; set; }

    public virtual DbSet<PurchaseReturn> PurchaseReturns { get; set; }

    public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }

    public virtual DbSet<Rack> Racks { get; set; }

    public virtual DbSet<ReceiptVoucher> ReceiptVouchers { get; set; }

    public virtual DbSet<ReceiptVoucherDetail> ReceiptVoucherDetails { get; set; }

    public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }

    public virtual DbSet<SalaryComponent> SalaryComponents { get; set; }

    public virtual DbSet<DisciplinaryAction> DisciplinaryActions { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<PayrollApprovalHistory> PayrollApprovalHistories { get; set; }

    public virtual DbSet<Payslip> Payslips { get; set; }

    public virtual DbSet<PayrollRegister> PayrollRegisters { get; set; }

    public virtual DbSet<PayrollRun> PayrollRuns { get; set; }

    public virtual DbSet<PayrollPayment> PayrollPayments { get; set; }

    public virtual DbSet<PayrollPeriod> PayrollPeriods { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<Bonus> Bonuses { get; set; }

    public virtual DbSet<Advance> Advances { get; set; }

    public virtual DbSet<LoanRecovery> LoanRecoveries { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<SalaryHistory> SalaryHistories { get; set; }

    public virtual DbSet<SalaryStructureComponent> SalaryStructureComponents { get; set; }

    public virtual DbSet<SalaryStructure> SalaryStructures { get; set; }

    public virtual DbSet<SalaryHead> SalaryHeads { get; set; }

    public virtual DbSet<SalesDetail> SalesDetails { get; set; }

    public virtual DbSet<SalesMaster> SalesMasters { get; set; }

    public virtual DbSet<SalesOrderStatus> SalesOrderStatuses { get; set; }

    public virtual DbSet<SalesReturn> SalesReturns { get; set; }

    public virtual DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }

    public virtual DbSet<ScheduledReport> ScheduledReports { get; set; }

    public virtual DbSet<SerialNumber> SerialNumbers { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<PayrollSetting> PayrollSettings { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<WeeklyOff> WeeklyOffs { get; set; }

    public virtual DbSet<AttendanceCorrectionRequest> AttendanceCorrectionRequests { get; set; }

    public virtual DbSet<CompOff> CompOffs { get; set; }

    public virtual DbSet<Overtime> Overtimes { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveBalance> LeaveBalances { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<AbsenceRecord> AbsenceRecords { get; set; }

    public virtual DbSet<MissingPunchRequest> MissingPunchRequests { get; set; }

    public virtual DbSet<ShiftAssignment> ShiftAssignments { get; set; }

    public virtual DbSet<ShiftType> ShiftTypes { get; set; }

    public virtual DbSet<StockAdjustment> StockAdjustments { get; set; }

    public virtual DbSet<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }

    public virtual DbSet<StockLedger> StockLedgers { get; set; }

    public virtual DbSet<StockTransfer> StockTransfers { get; set; }

    public virtual DbSet<StockTransferDetail> StockTransferDetails { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierContact> SupplierContacts { get; set; }

    public virtual DbSet<SupplierQuotation> SupplierQuotations { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<TaxSetting> TaxSettings { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WorkflowAction> WorkflowActions { get; set; }

    public virtual DbSet<WorkflowAssignment> WorkflowAssignments { get; set; }

    public virtual DbSet<WorkflowCondition> WorkflowConditions { get; set; }

    public virtual DbSet<WorkflowDeadline> WorkflowDeadlines { get; set; }

    public virtual DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }

    public virtual DbSet<WorkflowInstance> WorkflowInstances { get; set; }

    public virtual DbSet<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; }

    public virtual DbSet<WorkflowNotification> WorkflowNotifications { get; set; }

    public virtual DbSet<WorkflowStage> WorkflowStages { get; set; }

    public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountLedger>(entity =>
        {
            entity.HasKey(e => e.LedgerEntryId).HasName("PK__AccountL__ACE98A5A36F5EB46");

            entity.ToTable("AccountLedger");

            entity.HasIndex(e => e.AccountId, "IX_AccountLedger_AccountID");

            entity.HasIndex(e => e.EntryDate, "IX_AccountLedger_EntryDate");

            entity.HasIndex(e => e.VoucherId, "IX_AccountLedger_VoucherID");

            entity.HasIndex(e => e.VoucherId, "IX_AccountLedger_VoucherType");

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EntryDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.VoucherType).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.AccountLedgers)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountLedger_ChartOfAccounts");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC070D172892");

            entity.HasIndex(e => e.Name, "UQ__AspNetRo__737584F669A3CA15").IsUnique();

            entity.HasIndex(e => e.NormalizedName, "UQ__AspNetRo__A93C97B917A1600B").IsUnique();

            entity.HasIndex(e => e.Name, "UX_AspNetRoles_Name").IsUnique();

            entity.Property(e => e.ConcurrencyStamp).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07031A00E6");

            entity.Property(e => e.ClaimType).HasMaxLength(200);
            entity.Property(e => e.ClaimValue).HasMaxLength(500);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles");
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC07D40D00E1");

            entity.Property(e => e.ClaimType).HasMaxLength(200);
            entity.Property(e => e.ClaimValue).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserClaims_Users");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__AspNetUs__2B2C5B526918598C");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.ProviderDisplayName).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserLogins_Users");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__AspNetUs__8CC49841F90B3A6C");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserTokens_Users");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B8FBC043B9");

            entity.HasIndex(e => e.Timestamp, "IX_AuditLogs_Timestamp");

            entity.HasIndex(e => e.UserId, "IX_AuditLogs_UserID");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.IPAddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.RecordType).HasMaxLength(50);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.Module).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_AuditLogs_Modules");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLogs_Users");
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.BankAccountId).HasName("PK__BankAcco__4FC8E741BFB9B295");

            entity.HasIndex(e => e.Iban, "UQ__BankAcco__8235CCBCD1DC99C8").IsUnique();

            entity.Property(e => e.AccountNo).HasMaxLength(50);
            entity.Property(e => e.AccountTitle).HasMaxLength(200);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(200);
            entity.Property(e => e.BranchCode).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Iban).HasMaxLength(34);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.BankAccounts)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BankAccounts_Companies");
        });

        modelBuilder.Entity<BankReconciliation>(entity =>
        {
            entity.HasKey(e => e.ReconciliationId).HasName("PK__BankReco__096DC860E6FC8211");

            entity.ToTable("BankReconciliation");

            entity.HasIndex(e => e.BankAccountId, "IX_BankReconciliation_BankAccountID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Difference).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReconciliationDate).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.StatementBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SystemBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.BankReconciliations)
                .HasForeignKey(d => d.BankAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BankReconciliation_BankAccounts");
        });

        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__BankTran__55433A4BD3D50720");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.BankTransactions)
                .HasForeignKey(d => d.BankAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BankTransactions_BankAccounts");
        });

        modelBuilder.Entity<BatchTracking>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__BatchTra__5D55CE388A1B93D6");

            entity.ToTable("BatchTracking");

            entity.HasIndex(e => e.BatchNo, "IX_BatchTracking_BatchNo");

            entity.HasIndex(e => e.ExpiryDate, "IX_BatchTracking_ExpiryDate");

            entity.HasIndex(e => e.GrndetailId, "IX_BatchTracking_GRNDetailID");

            entity.HasIndex(e => e.IsActive, "IX_BatchTracking_IsActive");

            entity.HasIndex(e => e.ProductId, "IX_BatchTracking_ProductID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GrndetailId).HasColumnName("GRNDetailId");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Grndetail).WithMany(p => p.BatchTrackings)
                .HasForeignKey(d => d.GrndetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BatchTracking_GRNDetails");

            entity.HasOne(d => d.Product).WithMany(p => p.BatchTrackings)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BatchTracking_Products");
        });

        modelBuilder.Entity<Bin>(entity =>
        {
            entity.HasKey(e => e.BinId).HasName("PK__Bins__4BFF5A4EE5B1B3A3");

            entity.HasIndex(e => e.IsOccupied, "IX_Bins_IsOccupied");

            entity.HasIndex(e => e.RackId, "IX_Bins_RackID");

            entity.HasIndex(e => e.BinCode, "UQ__Bins__634342456C4CE7E7").IsUnique();

            entity.Property(e => e.BinCode).HasMaxLength(20);
            entity.Property(e => e.BinName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsOccupied).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Rack).WithMany(p => p.Bins)
                .HasForeignKey(d => d.RackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bins_Racks");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branches__A1682FA593F86D01");

            entity.HasIndex(e => e.CompanyId, "IX_Branches_CompanyID");

            entity.HasIndex(e => e.IsActive, "IX_Branches_IsActive");

            entity.HasIndex(e => e.IsDeleted, "IX_Branches_IsDeleted");

            entity.HasIndex(e => e.BranchCode, "UQ__Branches__1C61B88890C8036C").IsUnique();

            entity.HasIndex(e => e.BranchCode, "UX_Branches_BranchCode").IsUnique();

            entity.HasIndex(e => e.BranchName, "UX_Branches_BranchName").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BranchCode).HasMaxLength(20);
            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Branches)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Branches_Companies");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F3BECF2D5F8B");

            entity.HasIndex(e => e.BrandCode, "UQ__Brands__44292CC7E2D45ED3").IsUnique();

            entity.HasIndex(e => e.BrandName, "UX_Brands_BrandName").IsUnique();

            entity.Property(e => e.BrandCode).HasMaxLength(50);
            entity.Property(e => e.BrandName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500)
                .HasColumnName("LogoURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BA0C0A00A");

            entity.HasIndex(e => e.CategoryCode, "UQ__Categori__371BA955AE2B3953").IsUnique();

            entity.HasIndex(e => e.CategoryName, "UX_Categories_CategoryName").IsUnique();

            entity.Property(e => e.CategoryCode).HasMaxLength(50);
            entity.Property(e => e.CategoryName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__ChartOfA__349DA586099C5D6F");

            entity.HasIndex(e => e.AccountType, "IX_ChartOfAccounts_AccountType");

            entity.HasIndex(e => e.ParentAccountId, "IX_ChartOfAccounts_ParentAccountID");

            entity.HasIndex(e => e.AccountCode, "UQ__ChartOfA__38D0C56AF1D9515C").IsUnique();

            entity.Property(e => e.AccountCode).HasMaxLength(50);
            entity.Property(e => e.AccountName).HasMaxLength(200);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.ChartOfAccounts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ChartOfAccounts_Company");

            entity.HasOne(d => d.ParentAccount).WithMany(p => p.InverseParentAccount)
                .HasForeignKey(d => d.ParentAccountId)
                .HasConstraintName("FK_ChartOfAccounts_Parent");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971C4CDB2EC2E0");

            entity.HasIndex(e => e.IsActive, "IX_Companies_IsActive");

            entity.HasIndex(e => e.IsDeleted, "IX_Companies_IsDeleted");

            entity.HasIndex(e => e.RegistrationNo, "UQ__Companie__6EF5E042AE5386DC").IsUnique();

            entity.HasIndex(e => e.CompanyName, "UX_Companies_CompanyName").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RegistrationNo).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8B3F7C3A9");

            entity.HasIndex(e => e.IsActive, "IX_Customers_IsActive");

            entity.HasIndex(e => e.CustomerCode, "UX_Customers_Code_F")
                .IsUnique()
                .HasFilter("([CustomerCode] IS NOT NULL)");

            entity.HasIndex(e => e.CustomerCode, "UX_Customers_CustomerCode_Filtered")
                .IsUnique()
                .HasFilter("([CustomerCode] IS NOT NULL)");

            entity.HasIndex(e => e.CustomerName, "UX_Customers_CustomerName_Filtered")
                .IsUnique()
                .HasFilter("([CustomerName] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UX_Customers_Email_F")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UX_Customers_Email_Filtered")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.CustomerName, "UX_Customers_Name_F")
                .IsUnique()
                .HasFilter("([CustomerName] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UX_Customers_Phone_F")
                .IsUnique()
                .HasFilter("([Phone] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UX_Customers_Phone_Filtered")
                .IsUnique()
                .HasFilter("([Phone] IS NOT NULL)");

            entity.HasIndex(e => e.TaxId, "UX_Customers_TaxId_F")
                .IsUnique()
                .HasFilter("([TaxId] IS NOT NULL)");

            entity.HasIndex(e => e.TaxId, "UX_Customers_TaxId_Filtered")
                .IsUnique()
                .HasFilter("([TaxId] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreditDays).HasDefaultValue(0);
            entity.Property(e => e.CreditLimit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomerCode).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(300);
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.NTN)
                .HasMaxLength(50)
                .HasColumnName("NTN");
            entity.Property(e => e.PaymentTerms).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        modelBuilder.Entity<CustomerComplaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Customer__740D89AF4BDD5761");

            entity.Property(e => e.ComplaintType).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResolutionDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.CustomerComplaintAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK_CustomerComplaints_AssignedTo");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.CustomerComplaints)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_CustomerComplaints_WorkflowStages");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerComplaints)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerComplaints_Customers");

            entity.HasOne(d => d.EscalatedToNavigation).WithMany(p => p.CustomerComplaintEscalatedToNavigations)
                .HasForeignKey(d => d.EscalatedTo)
                .HasConstraintName("FK_CustomerComplaints_EscalatedTo");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.CustomerComplaintResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("FK_CustomerComplaints_ResolvedBy");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.CustomerComplaints)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_CustomerComplaints_WorkflowInstances");
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Customer__5C6625BBD49455AD");

            entity.HasIndex(e => e.CustomerId, "IX_CustomerContacts_CustomerID");

            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerContacts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerContacts_Customers");
        });

        modelBuilder.Entity<CustomerFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Customer__6A4BEDF6997C44DB");

            entity.ToTable("CustomerFeedback");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerFeedback_Customers");
        });

        modelBuilder.Entity<CustomerInquiry>(entity =>
        {
            entity.HasKey(e => e.InquiryId).HasName("PK__Customer__05E6E7EFDA90D1EE");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InquiryType).HasMaxLength(50);
            entity.Property(e => e.ResolvedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.CustomerInquiryAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK_CustomerInquiries_AssignedTo");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.CustomerInquiries)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_CustomerInquiries_WorkflowStages");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerInquiries)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerInquiries_Customers");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.CustomerInquiryResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("FK_CustomerInquiries_ResolvedBy");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.CustomerInquiries)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_CustomerInquiries_WorkflowInstances");
        });

        modelBuilder.Entity<CycleCount>(entity =>
        {
            entity.HasKey(e => e.CycleCountId).HasName("PK__CycleCou__A4189B51045FD997");

            entity.HasIndex(e => e.BinId, "IX_CycleCounts_BinID");

            entity.HasIndex(e => e.ProductId, "IX_CycleCounts_ProductID");

            entity.HasIndex(e => e.Status, "IX_CycleCounts_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_CycleCounts_WarehouseID");

            entity.Property(e => e.CountedBy).HasMaxLength(256);
            entity.Property(e => e.CountedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VerifiedBy).HasMaxLength(256);
            entity.Property(e => e.VerifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Bin).WithMany(p => p.CycleCounts)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CycleCounts_Bins");

            entity.HasOne(d => d.CountedByNavigation).WithMany(p => p.CycleCountCountedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.CountedBy)
                .HasConstraintName("FK_CycleCounts_CountedBy");

            entity.HasOne(d => d.Product).WithMany(p => p.CycleCounts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CycleCounts_Products");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.CycleCountVerifiedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("FK_CycleCounts_VerifiedBy");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.CycleCounts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CycleCounts_Warehouses");
        });

        modelBuilder.Entity<CycleCountDetail>(entity =>
        {
            entity.HasKey(e => e.CountDetailId).HasName("PK__CycleCou__8C9890BB37DFFB17");

            entity.HasIndex(e => e.CountId, "IX_CycleCountDetails_CountID");

            entity.HasIndex(e => e.ProductId, "IX_CycleCountDetails_ProductID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Bin).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CycleCountDetails_Bins");

            entity.HasOne(d => d.Count).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.CountId)
                .HasConstraintName("FK_CycleCountDetails_Counts");

            entity.HasOne(d => d.Product).WithMany(p => p.CycleCountDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CycleCountDetails_Products");
        });

        modelBuilder.Entity<DashboardWidget>(entity =>
        {
            entity.HasKey(e => e.WidgetId).HasName("PK__Dashboar__ADFD307220315B63");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WidgetConfig).HasColumnType("json");
            entity.Property(e => e.WidgetType).HasMaxLength(50);
        });

        modelBuilder.Entity<DataBackupLog>(entity =>
        {
            entity.HasKey(e => e.BackupId).HasName("PK__DataBack__EB9069E2E94B0AA7");

            entity.Property(e => e.BackupDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BackupType).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileSizeMb)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("FileSizeMB");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<DeliveryChallan>(entity =>
        {
            entity.HasKey(e => e.ChallanId).HasName("PK__Delivery__BFB043C452E881D1");

            entity.HasIndex(e => e.DispatchId, "IX_DeliveryChallans_DispatchID");

            entity.HasIndex(e => e.SalesId, "IX_DeliveryChallans_SalesID");

            entity.HasIndex(e => e.Status, "IX_DeliveryChallans_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_DeliveryChallans_WorkflowInstanceID");

            entity.HasIndex(e => e.ChallanNo, "UQ__Delivery__BFB7BB8A6BBC174E").IsUnique();

            entity.Property(e => e.ChallanDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ChallanNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReceivedBy).HasMaxLength(200);
            entity.Property(e => e.ReceivedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.DeliveryChallans)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_DeliveryChallans_WorkflowStages");

            entity.HasOne(d => d.Dispatch).WithMany(p => p.DeliveryChallans)
                .HasForeignKey(d => d.DispatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryChallans_DispatchMasters");

            entity.HasOne(d => d.Sales).WithMany(p => p.DeliveryChallans)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryChallans_SalesMasters");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.DeliveryChallans)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_DeliveryChallans_WorkflowInstances");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDC850A6FA");

            entity.HasIndex(e => e.BranchId, "IX_Departments_BranchID");

            entity.HasIndex(e => e.CompanyId, "IX_Departments_CompanyID");

            entity.HasIndex(e => e.DepartmentCode, "UQ__Departme__6EA8896DB2A460DB").IsUnique();

            entity.HasIndex(e => e.DepartmentCode, "UX_Departments_DepartmentCode").IsUnique();

            entity.HasIndex(e => e.DepartmentName, "UX_Departments_DepartmentName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentCode).HasMaxLength(20);
            entity.Property(e => e.DepartmentName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.Departments)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Branches");

            entity.HasOne(d => d.Company).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Companies");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PK__Designat__BABD603E441E7F05");

            entity.HasIndex(e => e.DepartmentId, "IX_Designations_DepartmentID");

            entity.HasIndex(e => e.DesignationName, "UX_Designations_DesignationName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DesignationName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.Designations)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designations_Departments");
        });

        modelBuilder.Entity<DispatchDetail>(entity =>
        {
            entity.HasKey(e => e.DispatchDetailId).HasName("PK__Dispatch__8B84B60099F1B6E4");

            entity.HasIndex(e => e.DispatchId, "IX_DispatchDetails_DispatchID");

            entity.HasIndex(e => e.ProductId, "IX_DispatchDetails_ProductID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Dispatch).WithMany(p => p.DispatchDetails)
                .HasForeignKey(d => d.DispatchId)
                .HasConstraintName("FK_DispatchDetails_DispatchMasters");

            entity.HasOne(d => d.Product).WithMany(p => p.DispatchDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispatchDetails_Products");

            entity.HasOne(d => d.SalesDetail).WithMany(p => p.DispatchDetails)
                .HasForeignKey(d => d.SalesDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispatchDetails_SalesDetails");
        });

        modelBuilder.Entity<DispatchMaster>(entity =>
        {
            entity.HasKey(e => e.DispatchId).HasName("PK__Dispatch__434DBD75E0D2D673");

            entity.HasIndex(e => e.SalesId, "IX_DispatchMasters_SalesID");

            entity.HasIndex(e => e.Status, "IX_DispatchMasters_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_DispatchMasters_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_DispatchMasters_WorkflowInstanceID");

            entity.HasIndex(e => e.DispatchNo, "UQ__Dispatch__434D95C77358C77D").IsUnique();

            entity.Property(e => e.CarrierName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DispatchDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DispatchNo).HasMaxLength(50);
            entity.Property(e => e.DispatchedBy).HasMaxLength(256);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TrackingNo).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.DispatchMasters)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_DispatchMasters_WorkflowStages");

            entity.HasOne(d => d.DispatchedByNavigation).WithMany(p => p.DispatchMasters)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.DispatchedBy)
                .HasConstraintName("FK_DispatchMasters_DispatchedBy");

            entity.HasOne(d => d.Sales).WithMany(p => p.DispatchMasters)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispatchMasters_SalesMasters");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.DispatchMasters)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispatchMasters_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.DispatchMasters)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_DispatchMasters_WorkflowInstances");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1EEF6AF6A");

            entity.HasIndex(e => e.BranchId, "IX_Employees_BranchID");

            entity.HasIndex(e => e.DepartmentId, "IX_Employees_DepartmentID");

            entity.HasIndex(e => e.DesignationId, "IX_Employees_DesignationID");

            entity.HasIndex(e => e.EmployeeCode, "IX_Employees_EmployeeCode");

            entity.HasIndex(e => e.IsActive, "IX_Employees_IsActive");

            entity.HasIndex(e => e.UserId, "IX_Employees_UserID");

            entity.HasIndex(e => e.WarehouseId, "IX_Employees_WarehouseID");

            entity.HasIndex(e => e.UserId, "UQ_Employees_UserId_Filtered")
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F642548965B7004").IsUnique();

            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cnic).HasMaxLength(15);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DOB).HasColumnName("DOB");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FatherName).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Branches");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Departments");

            entity.HasOne(d => d.Designation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Designations");

            entity.HasOne(d => d.Shift).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK_Employee_Shifts");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Employees)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK_Employees_Warehouses");
        });

        modelBuilder.Entity<EmployeeAttendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Employee__8B69263C774A3B52");

            entity.ToTable("EmployeeAttendance");

            entity.HasIndex(e => e.AttendanceDate, "IX_EmployeeAttendance_AttendanceDate");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeAttendance_EmployeeID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OvertimeHours)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Shift).WithMany(p => p.EmployeeAttendances)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK_EmployeeAttendance_Shifts");
        });

        modelBuilder.Entity<EmployeeBankAccount>(entity =>
        {
            entity.HasKey(e => e.BankAccountId).HasName("PK__Employee__4FC8E7417B00B282");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeBankAccounts_EmployeeID");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Iban).HasMaxLength(34);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeBankAccounts)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeBankAccounts_Employees");
        });

        modelBuilder.Entity<EmployeeLeaf>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__Employee__796DB959CCB174FD");

            entity.ToTable("EmployeeLeaves");

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LeaveType).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.EmployeeLeaves)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_EmployeeLeaves_ApprovedBy");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeaves)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeLeaves_Employees");
        });

        modelBuilder.Entity<EmployeeLeave>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__Employee__796DB979EB15456E");

            entity.ToTable("EmployeeLeave");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeLeave_EmployeeID");

            entity.HasIndex(e => e.Status, "IX_EmployeeLeave_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_EmployeeLeave_WorkflowInstanceID");

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveType).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.EmployeeLeavesNavigation)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_EmployeeLeave_ApprovedBy");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeavesNavigation)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeLeave_Employees");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.EmployeeLeaves)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_EmployeeLeave_WorkflowInstances");
        });

        modelBuilder.Entity<EmployeePayroll>(entity =>
        {
            entity.HasKey(e => e.PayrollId).HasName("PK__Employee__99DFC69249C6CFC9");

            entity.ToTable("EmployeePayroll");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeePayroll_EmployeeID");

            entity.HasIndex(e => e.SalaryId, "IX_EmployeePayroll_SalaryID");

            entity.HasIndex(e => e.Status, "IX_EmployeePayroll_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_EmployeePayroll_WorkflowInstanceID");

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GrossPay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetPay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalDeductions)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.EmployeePayrolls)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_EmployeePayroll_ApprovedBy");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePayroll_Employees");

            entity.HasOne(d => d.Salary).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.SalaryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePayroll_EmployeeSalary");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_EmployeePayroll_WorkflowInstances");
        });

        modelBuilder.Entity<EmployeePayrollDetail>(entity =>
        {
            entity.HasKey(e => e.PayrollDetailId).HasName("PK__Employee__010127A91EFF93FC");

            entity.HasIndex(e => e.ComponentId, "IX_EmployeePayrollDetails_ComponentID");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeePayrollDetails_EmployeeID");

            entity.HasIndex(e => e.PayrollId, "IX_EmployeePayrollDetails_PayrollID");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Component).WithMany(p => p.EmployeePayrollDetails)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePayrollDetails_SalaryComponents");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePayrollDetails)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePayrollDetails_Employees");

            entity.HasOne(d => d.Payroll).WithMany(p => p.EmployeePayrollDetails)
                .HasForeignKey(d => d.PayrollId)
                .HasConstraintName("FK_EmployeePayrollDetails_Payroll");
        });

        modelBuilder.Entity<EmployeeSalary>(entity =>
        {
            entity.HasKey(e => e.SalaryId).HasName("PK__Employee__4BE20457674B57E1");

            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HouseRent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MedicalAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherAllowances).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherDeductions).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProfessionalTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAllowances).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDeductions).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UtilityAllowance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSalaries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpSalaries_Employees");
        });

        modelBuilder.Entity<EmployeeSalary1>(entity =>
        {
            entity.HasKey(e => e.SalaryId).HasName("PK__Employee__4BE204B7225312D7");

            entity.ToTable("EmployeeSalary");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeSalary_EmployeeID");

            entity.HasIndex(e => e.IsActive, "IX_EmployeeSalary_IsActive");

            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HouseRent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeTax)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MedicalAllowance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetSalary)
                .HasComputedColumnSql("(([BasicSalary]+((([HouseRent]+[UtilityAllowance])+[MedicalAllowance])+[OtherAllowances]))-(([IncomeTax]+[ProfessionalTax])+[OtherDeductions]))", true)
                .HasColumnType("decimal(23, 2)");
            entity.Property(e => e.OtherAllowances)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherDeductions)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProfessionalTax)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAllowances)
                .HasComputedColumnSql("((([HouseRent]+[UtilityAllowance])+[MedicalAllowance])+[OtherAllowances])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.TotalDeductions)
                .HasComputedColumnSql("(([IncomeTax]+[ProfessionalTax])+[OtherDeductions])", true)
                .HasColumnType("decimal(20, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UtilityAllowance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSalary1s)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeSalary_Employees");
        });

        modelBuilder.Entity<EmployeeStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Employee__C8EE204333154D6F");

            entity.ToTable("EmployeeStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Employee__05E7698AFFD78AC8").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorId).HasName("PK__ErrorLog__358565CA10610678");

            entity.Property(e => e.ErrorDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExceptionType).HasMaxLength(500);
            entity.Property(e => e.IPAddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.RequestMethod).HasMaxLength(20);
            entity.Property(e => e.RequestUrl)
                .HasMaxLength(500)
                .HasColumnName("RequestURL");
            entity.Property(e => e.Source).HasMaxLength(500);
            entity.Property(e => e.TraceId).HasMaxLength(100);
        });

        modelBuilder.Entity<Grn>(entity =>
        {
            entity.HasKey(e => e.GrnId).HasName("PK__GRN__BC0E8C62AE01BC15");

            entity.ToTable("GRN");

            entity.HasIndex(e => e.CurrentStageId, "IX_GRN_CurrentStageID");

            entity.HasIndex(e => e.PurchaseId, "IX_GRN_PurchaseID");

            entity.HasIndex(e => e.Status, "IX_GRN_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_GRN_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_GRN_WorkflowInstanceID");

            entity.HasIndex(e => e.GrnNo, "UQ__GRN__BC0E85A839D807E0").IsUnique();

            entity.Property(e => e.GrnId).HasColumnName("GrnId");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GrnNo)
                .HasMaxLength(50)
                .HasColumnName("GrnNo");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReceiptDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalRejectedQty).HasDefaultValue(0);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.Grns)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_GRN_WorkflowStages");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Grns)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRN_PurchaseMasters");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Grns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRN_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.Grns)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_GRN_WorkflowInstances");
        });

        modelBuilder.Entity<Grndetail>(entity =>
        {
            entity.HasKey(e => e.GrndetailId).HasName("PK__GRNDetai__B7FEA6A1203C8712");

            entity.ToTable("GRNDetails");

            entity.HasIndex(e => e.BinId, "IX_GRNDetails_BinID");

            entity.HasIndex(e => e.GrnId, "IX_GRNDetails_GRNID");

            entity.HasIndex(e => e.ProductId, "IX_GRNDetails_ProductID");

            entity.HasIndex(e => e.PurchaseDetailId, "IX_GRNDetails_PurchaseDetailID");

            entity.Property(e => e.GrndetailId).HasColumnName("GRNDetailId");
            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GrnId).HasColumnName("GrnId");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.RejectedQty).HasDefaultValue(0);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Bin).WithMany(p => p.Grndetails)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRNDetails_Bins");

            entity.HasOne(d => d.Grn).WithMany(p => p.Grndetails)
                .HasForeignKey(d => d.GrnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRNDetails_GRN");

            entity.HasOne(d => d.Product).WithMany(p => p.Grndetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRNDetails_Products");

            entity.HasOne(d => d.PurchaseDetail).WithMany(p => p.Grndetails)
                .HasForeignKey(d => d.PurchaseDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GRNDetails_PurchaseDetails");
        });

        modelBuilder.Entity<InventoryValuation>(entity =>
        {
            entity.HasKey(e => e.ValuationId).HasName("PK__Inventor__CF17CF52CC9964ED");

            entity.ToTable("InventoryValuation");

            entity.HasIndex(e => e.ProductId, "IX_InventoryValuation_ProductID");

            entity.HasIndex(e => e.ValuationDate, "IX_InventoryValuation_ValuationDate");

            entity.HasIndex(e => e.WarehouseId, "IX_InventoryValuation_WarehouseID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Method).HasMaxLength(20);
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryValuations)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryValuation_Products");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryValuations)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryValuation_Warehouses");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD552D409B9");

            entity.HasIndex(e => e.DueDate, "IX_Invoices_DueDate");

            entity.HasIndex(e => e.SalesId, "IX_Invoices_SalesID");

            entity.HasIndex(e => e.Status, "IX_Invoices_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_Invoices_WorkflowInstanceID");

            entity.HasIndex(e => e.InvoiceNo, "UQ__Invoices__D796B227918B33BD").IsUnique();

            entity.Property(e => e.BalanceDue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_Invoices_WorkflowStages");

            entity.HasOne(d => d.Sales).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_SalesMasters");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_Invoices_WorkflowInstances");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F1578F1D344532E");

            entity.HasIndex(e => e.InvoiceId, "IX_InvoiceDetails_InvoiceID");

            entity.HasIndex(e => e.ProductId, "IX_InvoiceDetails_ProductID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountPercent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_InvoiceDetails_Invoices");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceDetails_Products");

            entity.HasOne(d => d.SalesDetail).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.SalesDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceDetails_SalesDetails");
        });

        modelBuilder.Entity<JournalVoucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__JournalV__3AEE79C11B230391");

            entity.HasIndex(e => e.VoucherNo, "UQ__JournalV__3AD31D6F6B59EB14").IsUnique();

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalCredit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDebit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VoucherNo).HasMaxLength(50);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.JournalVouchers)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_JournalVouchers_ApprovedBy");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.JournalVouchers)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_JournalVouchers_WorkflowStages");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.JournalVouchers)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_JournalVouchers_WorkflowInstances");
        });

        modelBuilder.Entity<JournalVoucherDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__JournalV__135C316D2542D4F8");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Narration).HasMaxLength(500);

            entity.HasOne(d => d.Account).WithMany(p => p.JournalVoucherDetails)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JVD_Accounts");

            entity.HasOne(d => d.Voucher).WithMany(p => p.JournalVoucherDetails)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JVD_Vouchers");
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__LoginHis__4DDA283820AEB128");

            entity.ToTable("LoginHistory");

            entity.Property(e => e.DeviceInfo).HasMaxLength(500);
            entity.Property(e => e.IPAddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LogoutTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.LoginHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoginHistory_Users");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B7477871FADDB09");

            entity.HasIndex(e => e.ModuleCode, "UQ__Modules__EB27D4330AAF47C5").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleCode).HasMaxLength(50);
            entity.Property(e => e.ModuleName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__ModulePe__EFA6FB0F79896DFA");

            entity.Property(e => e.CanApprove).HasDefaultValue(false);
            entity.Property(e => e.CanCreate).HasDefaultValue(false);
            entity.Property(e => e.CanDelete).HasDefaultValue(false);
            entity.Property(e => e.CanEdit).HasDefaultValue(false);
            entity.Property(e => e.CanView).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Module).WithMany(p => p.ModulePermissions)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModulePermissions_Modules");

            entity.HasOne(d => d.Role).WithMany(p => p.ModulePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModulePermissions_AspNetRoles");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32CD4241CE");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ReadDate).HasColumnType("datetime");
            entity.Property(e => e.ReferenceModule).HasMaxLength(50);
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.Template).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK_Notifications_NotificationTemplates");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Notifica__F87ADD07A879CF2B");

            entity.HasIndex(e => e.TemplateCode, "UQ__Notifica__0FDB5081EADFB857").IsUnique();

            entity.HasIndex(e => e.TemplateCode, "UX_NotifTemplates_TemplateCode").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.TemplateCode).HasMaxLength(50);
            entity.Property(e => e.TemplateName).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC0774178481");

            entity.HasIndex(e => e.Token, "IX_PasswordResetTokens_Token");

            entity.HasIndex(e => e.UserId, "IX_PasswordResetTokens_UserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentVoucher>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PaymentV__9B556A580A50AB1E");

            entity.HasIndex(e => e.PaymentNo, "UQ__PaymentV__9B5572663630B3C8").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.ChequeNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentNo).HasMaxLength(50);
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.PaymentVouchers)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_PaymentVouchers_ApprovedBy");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.PaymentVouchers)
                .HasForeignKey(d => d.BankAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentVouchers_BankAccounts");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.PaymentVouchers)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_PaymentVouchers_WorkflowStages");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PaymentVouchers)
                .HasForeignKey(d => d.PurchaseId)
                .HasConstraintName("FK_PaymentVouchers_PurchaseMasters");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PaymentVouchers)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_PaymentVouchers_Suppliers");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.PaymentVouchers)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_PaymentVouchers_WorkflowInstances");
        });

        modelBuilder.Entity<PaymentVoucherDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__PaymentV__135C316D29F85606");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Narration).HasMaxLength(500);

            entity.HasOne(d => d.Account).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PVD_Accounts");

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PVD_Payments");
        });

        modelBuilder.Entity<PickingDetail>(entity =>
        {
            entity.HasKey(e => e.PickingDetailId).HasName("PK__PickingD__8189D109015FDA2D");

            entity.HasIndex(e => e.BinId, "IX_PickingDetails_BinID");

            entity.HasIndex(e => e.PickingId, "IX_PickingDetails_PickingID");

            entity.HasIndex(e => e.ProductId, "IX_PickingDetails_ProductID");

            entity.HasIndex(e => e.SalesDetailId, "IX_PickingDetails_SalesDetailID");

            entity.HasIndex(e => e.Status, "IX_PickingDetails_Status");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Bin).WithMany(p => p.PickingDetails)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingDetails_Bins");

            entity.HasOne(d => d.Picking).WithMany(p => p.PickingDetails)
                .HasForeignKey(d => d.PickingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingDetails_PickingLists");

            entity.HasOne(d => d.Product).WithMany(p => p.PickingDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingDetails_Products");

            entity.HasOne(d => d.SalesDetail).WithMany(p => p.PickingDetails)
                .HasForeignKey(d => d.SalesDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingDetails_SalesDetails");
        });

        modelBuilder.Entity<PickingList>(entity =>
        {
            entity.HasKey(e => e.PickingId).HasName("PK__PickingL__A93D74E0792E55BE");

            entity.HasIndex(e => e.AssignedTo, "IX_PickingLists_AssignedTo");

            entity.HasIndex(e => e.SalesId, "IX_PickingLists_SalesID");

            entity.HasIndex(e => e.Status, "IX_PickingLists_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_PickingLists_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_PickingLists_WorkflowInstanceID");

            entity.HasIndex(e => e.PickingNo, "UQ__PickingL__A93357416FB2D8D4").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PickedBy).HasMaxLength(256);
            entity.Property(e => e.PickingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PickingNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Created");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VerifiedBy).HasMaxLength(256);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.PickingListAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK_PickingLists_AssignedTo");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.PickingLists)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_PickingLists_WorkflowStages");

            entity.HasOne(d => d.PickedByNavigation).WithMany(p => p.PickingListPickedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.PickedBy)
                .HasConstraintName("FK_PickingLists_PickedBy");

            entity.HasOne(d => d.Sales).WithMany(p => p.PickingLists)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingLists_SalesMasters");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.PickingListVerifiedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("FK_PickingLists_VerifiedBy");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PickingLists)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingLists_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.PickingLists)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_PickingLists_WorkflowInstances");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED37103C80");

            entity.HasIndex(e => e.BrandId, "IX_Products_BrandID");

            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryID");

            entity.HasIndex(e => e.IsActive, "IX_Products_IsActive");

            entity.HasIndex(e => e.Sku, "IX_Products_SKU");

            entity.HasIndex(e => e.SubCategoryId, "IX_Products_SubCategoryID");

            entity.HasIndex(e => e.UnitId, "IX_Products_UnitID");

            entity.HasIndex(e => e.Barcode, "UQ__Products__177800D3CB972393").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0DABE3AF88").IsUnique();

            entity.HasIndex(e => e.Barcode, "UX_Products_Barcode").IsUnique();

            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBatchManaged).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsSerialized).HasDefaultValue(false);
            entity.Property(e => e.Length).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxStockLevel).HasDefaultValue(999999);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(0);
            entity.Property(e => e.ProductName).HasMaxLength(300);
            entity.Property(e => e.Qrcode)
                .HasMaxLength(200)
                .HasColumnName("QRCode");
            entity.Property(e => e.ReorderLevel).HasDefaultValue(0);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Weight).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Width).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Brands");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_SubCategories");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Units");
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("PK__ProductA__C189298AE4957945");

            entity.HasIndex(e => e.ProductId, "IX_ProductAttributes_ProductID");

            entity.Property(e => e.AttributeName).HasMaxLength(100);
            entity.Property(e => e.AttributeValue).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductAttributes_Products");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ProductI__7516F4EC37B02ED2");

            entity.HasIndex(e => e.IsPrimary, "IX_ProductImages_IsPrimary");

            entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<ProductPricingHistory>(entity =>
        {
            entity.HasKey(e => e.PriceHistoryId).HasName("PK__ProductP__A927CB2B5C28FDFA");

            entity.ToTable("ProductPricingHistory");

            entity.HasIndex(e => e.ProductId, "IX_ProductPricingHistory_ProductID");

            entity.Property(e => e.EffectiveDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NewPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ProductPricingHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPricingHistory_ChangedBy");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPricingHistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPricingHistory_Products");
        });

        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__Purchase__135C314D5FEF6E3E");

            entity.HasIndex(e => e.ProductId, "IX_PurchaseDetails_ProductID");

            entity.HasIndex(e => e.PurchaseId, "IX_PurchaseDetails_PurchaseID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountPercent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReceivedQty).HasDefaultValue(0);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseDetails_Products");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseDetails_PurchaseMasters");
        });

        modelBuilder.Entity<PurchaseMaster>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BDECAD3925C");

            entity.HasIndex(e => e.CurrentStageId, "IX_PurchaseMasters_CurrentStageID");

            entity.HasIndex(e => e.OrderDate, "IX_PurchaseMasters_OrderDate");

            entity.HasIndex(e => e.Status, "IX_PurchaseMasters_Status");

            entity.HasIndex(e => e.SupplierId, "IX_PurchaseMasters_SupplierID");

            entity.HasIndex(e => e.WarehouseId, "IX_PurchaseMasters_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_PurchaseMasters_WorkflowInstanceID");

            entity.HasIndex(e => e.PurchaseOrderNo, "UQ__Purchase__03153EF2963BFC6E").IsUnique();

            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PurchaseOrderNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.PurchaseMasters)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_PurchaseMasters_WorkflowStages");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseMasters)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseMasters_Suppliers");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseMasters)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseMasters_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.PurchaseMasters)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_PurchaseMasters_WorkflowInstances");
        });

        modelBuilder.Entity<PurchaseOrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Purchase__C8EE20438EEABA77");

            entity.ToTable("PurchaseOrderStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Purchase__05E7698AB9C93CE4").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<PurchaseReturn>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__Purchase__F445E988FA473F9E");

            entity.HasIndex(e => e.GrnId, "IX_PurchaseReturns_GRNID");

            entity.HasIndex(e => e.PurchaseId, "IX_PurchaseReturns_PurchaseID");

            entity.HasIndex(e => e.Status, "IX_PurchaseReturns_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_PurchaseReturns_WorkflowInstanceID");

            entity.HasIndex(e => e.ReturnNo, "UQ__Purchase__F445F15AAA28D1D0").IsUnique();

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GrnId).HasColumnName("GrnId");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReturnDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReturnNo).HasMaxLength(50);
            entity.Property(e => e.ReturnReason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.PurchaseReturns)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_PurchaseReturns_ApprovedBy");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_PurchaseReturns_WorkflowStages");

            entity.HasOne(d => d.Grn).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.GrnId)
                .HasConstraintName("FK_PurchaseReturns_GRN");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseReturns_PurchaseMasters");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_PurchaseReturns_WorkflowInstances");
        });

        modelBuilder.Entity<PurchaseReturnDetail>(entity =>
        {
            entity.HasKey(e => e.ReturnDetailId).HasName("PK__Purchase__8B89C9EAC9883A03");

            entity.HasIndex(e => e.ProductId, "IX_PurchaseReturnDetails_ProductID");

            entity.HasIndex(e => e.ReturnId, "IX_PurchaseReturnDetails_ReturnID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseReturnDetails_Products");

            entity.HasOne(d => d.PurchaseDetail).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.PurchaseDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseReturnDetails_PurchaseDetails");

            entity.HasOne(d => d.Return).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.ReturnId)
                .HasConstraintName("FK_PurchaseReturnDetails_Returns");
        });

        modelBuilder.Entity<Rack>(entity =>
        {
            entity.HasKey(e => e.RackId).HasName("PK__Racks__0363D9486B4EFF42");

            entity.HasIndex(e => e.ZoneId, "IX_Racks_ZoneID");

            entity.HasIndex(e => e.RackCode, "UQ__Racks__BAFE22FEC7250078").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.RackCode).HasMaxLength(20);
            entity.Property(e => e.RackName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Zone).WithMany(p => p.Racks)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Racks_Zones");
        });

        modelBuilder.Entity<ReceiptVoucher>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("PK__ReceiptV__CC08C400B9906120");

            entity.HasIndex(e => e.ReceiptNo, "UQ__ReceiptV__CC0B72A6C6E8DD7F").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.ChequeNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ReceiptDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiptNo).HasMaxLength(50);
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.ReceiptVouchers)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_ReceiptVouchers_ApprovedBy");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.ReceiptVouchers)
                .HasForeignKey(d => d.BankAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiptVouchers_BankAccounts");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.ReceiptVouchers)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_ReceiptVouchers_WorkflowStages");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReceiptVouchers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ReceiptVouchers_Customers");

            entity.HasOne(d => d.Sales).WithMany(p => p.ReceiptVouchers)
                .HasForeignKey(d => d.SalesId)
                .HasConstraintName("FK_ReceiptVouchers_SalesMasters");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.ReceiptVouchers)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_ReceiptVouchers_WorkflowInstances");
        });

        modelBuilder.Entity<ReceiptVoucherDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__ReceiptV__135C316DAEC0FC63");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Narration).HasMaxLength(500);

            entity.HasOne(d => d.Account).WithMany(p => p.ReceiptVoucherDetails)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RVD_Accounts");

            entity.HasOne(d => d.Receipt).WithMany(p => p.ReceiptVoucherDetails)
                .HasForeignKey(d => d.ReceiptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RVD_Receipts");
        });

        modelBuilder.Entity<ReportDefinition>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__ReportDe__D5BD48E5CFE81B71");

            entity.HasIndex(e => e.ReportCode, "UQ__ReportDe__0EDCD614F13BC563").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Parameters).HasColumnType("json");
            entity.Property(e => e.ReportCategory).HasMaxLength(100);
            entity.Property(e => e.ReportCode).HasMaxLength(50);
            entity.Property(e => e.ReportName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalaryComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__SalaryCo__D79CF02EF3553438");

            entity.HasIndex(e => e.ComponentCode, "UQ__SalaryCo__898CFBF8EF4D5AFB").IsUnique();

            entity.Property(e => e.ComponentCode).HasMaxLength(50);
            entity.Property(e => e.ComponentName).HasMaxLength(100);
            entity.Property(e => e.ComponentType).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__SalesDet__135C314D4A34B2F0");

            entity.HasIndex(e => e.ProductId, "IX_SalesDetails_ProductID");

            entity.HasIndex(e => e.SalesId, "IX_SalesDetails_SalesID");

            entity.Property(e => e.AllocatedQty).HasDefaultValue(0);
            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountPercent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PickedQty).HasDefaultValue(0);
            entity.Property(e => e.ShippedQty).HasDefaultValue(0);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesDetails_Products");

            entity.HasOne(d => d.Sales).WithMany(p => p.SalesDetails)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesDetails_SalesMasters");
        });

        modelBuilder.Entity<SalesMaster>(entity =>
        {
            entity.HasKey(e => e.SalesId).HasName("PK__SalesMas__C952FB128EB0F677");

            entity.HasIndex(e => e.CurrentStageId, "IX_SalesMasters_CurrentStageID");

            entity.HasIndex(e => e.CustomerId, "IX_SalesMasters_CustomerID");

            entity.HasIndex(e => e.OrderDate, "IX_SalesMasters_OrderDate");

            entity.HasIndex(e => e.Status, "IX_SalesMasters_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_SalesMasters_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_SalesMasters_WorkflowInstanceID");

            entity.HasIndex(e => e.SalesOrderNo, "UQ__SalesMas__B1403B8C781F1970").IsUnique();

            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SalesOrderNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.SalesMasters)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_SalesMasters_WorkflowStages");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesMasters)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesMasters_Customers");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.SalesMasters)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesMasters_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.SalesMasters)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_SalesMasters_WorkflowInstances");
        });

        modelBuilder.Entity<SalesOrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__SalesOrd__C8EE20438742B90B");

            entity.ToTable("SalesOrderStatus");

            entity.HasIndex(e => e.StatusName, "UQ__SalesOrd__05E7698A5CDCDD0B").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<SalesReturn>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__SalesRet__F445E98848DF03F5");

            entity.HasIndex(e => e.InvoiceId, "IX_SalesReturns_InvoiceID");

            entity.HasIndex(e => e.SalesId, "IX_SalesReturns_SalesID");

            entity.HasIndex(e => e.Status, "IX_SalesReturns_Status");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_SalesReturns_WorkflowInstanceID");

            entity.HasIndex(e => e.ReturnNo, "UQ__SalesRet__F445F15A263681E9").IsUnique();

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReturnDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReturnNo).HasMaxLength(50);
            entity.Property(e => e.ReturnReason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.SalesReturns)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_SalesReturns_ApprovedBy");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_SalesReturns_WorkflowStages");

            entity.HasOne(d => d.Invoice).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_SalesReturns_Invoices");

            entity.HasOne(d => d.Sales).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesReturns_SalesMasters");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_SalesReturns_WorkflowInstances");
        });

        modelBuilder.Entity<SalesReturnDetail>(entity =>
        {
            entity.HasKey(e => e.ReturnDetailId).HasName("PK__SalesRet__8B89C9EA579A657C");

            entity.HasIndex(e => e.ProductId, "IX_SalesReturnDetails_ProductID");

            entity.HasIndex(e => e.ReturnId, "IX_SalesReturnDetails_ReturnID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.InvoiceDetail).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.InvoiceDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesReturnDetails_InvoiceDetails");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesReturnDetails_Products");

            entity.HasOne(d => d.Return).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.ReturnId)
                .HasConstraintName("FK_SalesReturnDetails_Returns");
        });

        modelBuilder.Entity<ScheduledReport>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B694FF8F3F0");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Frequency).HasMaxLength(20);
            entity.Property(e => e.LastRunDate).HasColumnType("datetime");
            entity.Property(e => e.NextRunDate).HasColumnType("datetime");
            entity.Property(e => e.Parameters).HasColumnType("json");
            entity.Property(e => e.ScheduleDays).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SerialNumber>(entity =>
        {
            entity.HasKey(e => e.SerialId).HasName("PK__SerialNu__5E5B3EC49E9A4FF2");

            entity.HasIndex(e => e.GrndetailId, "IX_SerialNumbers_GRNDetailID");

            entity.HasIndex(e => e.ProductId, "IX_SerialNumbers_ProductID");

            entity.HasIndex(e => e.SerialNo, "IX_SerialNumbers_SerialNo");

            entity.HasIndex(e => e.Status, "IX_SerialNumbers_Status");

            entity.HasIndex(e => e.SerialNo, "UQ__SerialNu__5E5A535EF9FBE859").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GrndetailId).HasColumnName("GRNDetailId");
            entity.Property(e => e.OutDate).HasColumnType("datetime");
            entity.Property(e => e.SerialNo).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("IN");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.SerialNumbers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_SerialNumbers_Customers");

            entity.HasOne(d => d.Grndetail).WithMany(p => p.SerialNumbersNavigation)
                .HasForeignKey(d => d.GrndetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SerialNumbers_GRNDetails");

            entity.HasOne(d => d.Product).WithMany(p => p.SerialNumbers)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SerialNumbers_Products");

            entity.HasOne(d => d.SalesDetail).WithMany(p => p.SerialNumbers)
                .HasForeignKey(d => d.SalesDetailId)
                .HasConstraintName("FK_SerialNumbers_SalesDetails");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__Shifts__C0A838E162D06154");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.GracePeriodMinutes).HasDefaultValue(15);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ShiftName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.AdjustmentId).HasName("PK__StockAdj__E60DB8B3F69F412D");

            entity.HasIndex(e => e.BinId, "IX_StockAdjustments_BinID");

            entity.HasIndex(e => e.ProductId, "IX_StockAdjustments_ProductID");

            entity.HasIndex(e => e.Status, "IX_StockAdjustments_Status");

            entity.HasIndex(e => e.WarehouseId, "IX_StockAdjustments_WarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_StockAdjustments_WorkflowInstanceID");

            entity.Property(e => e.AdjustmentType).HasMaxLength(50);
            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.StockAdjustments)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_StockAdjustments_ApprovedBy");

            entity.HasOne(d => d.Bin).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockAdjustments_Bins");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_StockAdjustments_WorkflowStages");

            entity.HasOne(d => d.Product).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockAdjustments_Products");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockAdjustments_Warehouses");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_StockAdjustments_WorkflowInstances");
        });

        modelBuilder.Entity<StockAdjustmentDetail>(entity =>
        {
            entity.HasKey(e => e.AdjustmentDetailId).HasName("PK__StockAdj__A4CB61D8BF9810D0");

            entity.HasIndex(e => e.AdjustmentId, "IX_StockAdjustmentDetails_AdjustmentID");

            entity.HasIndex(e => e.ProductId, "IX_StockAdjustmentDetails_ProductID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Adjustment).WithMany(p => p.StockAdjustmentDetails)
                .HasForeignKey(d => d.AdjustmentId)
                .HasConstraintName("FK_StockAdjustmentDetails_Adjustments");

            entity.HasOne(d => d.Bin).WithMany(p => p.StockAdjustmentDetails)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockAdjustmentDetails_Bins");

            entity.HasOne(d => d.Product).WithMany(p => p.StockAdjustmentDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockAdjustmentDetails_Products");
        });

        modelBuilder.Entity<StockLedger>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("PK__StockLed__AE70E0AF173C007C");

            entity.HasIndex(e => e.BinId, "IX_StockLedger_BinID");

            entity.HasIndex(e => e.ProductId, "IX_StockLedger_ProductID");

            entity.HasIndex(e => e.ReferenceDocNo, "IX_StockLedger_ReferenceDocNo");

            entity.HasIndex(e => e.TransactionDate, "IX_StockLedger_TransactionDate");

            entity.HasIndex(e => e.TransactionType, "IX_StockLedger_TransactionType");

            entity.HasIndex(e => e.WarehouseId, "IX_StockLedger_WarehouseID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReferenceDocNo).HasMaxLength(50);
            entity.Property(e => e.ReferenceDocType).HasMaxLength(50);
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(20);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Bin).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockLedger_Bins");

            entity.HasOne(d => d.Product).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockLedger_Products");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockLedgers)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockLedger_Warehouses");
        });

        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasKey(e => e.TransferId).HasName("PK__StockTra__95490171B1DA619E");

            entity.HasIndex(e => e.FromWarehouseId, "IX_StockTransfers_FromWarehouseID");

            entity.HasIndex(e => e.ProductId, "IX_StockTransfers_ProductID");

            entity.HasIndex(e => e.Status, "IX_StockTransfers_Status");

            entity.HasIndex(e => e.ToWarehouseId, "IX_StockTransfers_ToWarehouseID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_StockTransfers_WorkflowInstanceID");

            entity.HasIndex(e => e.TransferNo, "UQ__StockTra__9548BE62668F5E78").IsUnique();

            entity.Property(e => e.ApprovedBy).HasMaxLength(256);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ReceivedBy).HasMaxLength(256);
            entity.Property(e => e.ReceivedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransferDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransferNo).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.StockTransferApprovedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_StockTransfers_ApprovedBy");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.StockTransfers)
                .HasForeignKey(d => d.CurrentStageId)
                .HasConstraintName("FK_StockTransfers_WorkflowStages");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockTransferFromWarehouses)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransfers_FromWarehouse");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransfers)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransfersProducts");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.StockTransferReceivedByNavigations)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.ReceivedBy)
                .HasConstraintName("FK_StockTransfers_ReceivedBy");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockTransferToWarehouses)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransfers_ToWarehouse");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.StockTransfers)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_StockTransfers_WorkflowInstances");
        });

        modelBuilder.Entity<StockTransferDetail>(entity =>
        {
            entity.HasKey(e => e.TransferDetailId).HasName("PK__StockTra__F9BF690FAF5CF4DB");

            entity.HasIndex(e => e.ProductId, "IX_StockTransferDetails_ProductID");

            entity.HasIndex(e => e.TransferId, "IX_StockTransferDetails_TransferID");

            entity.Property(e => e.BatchNo).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.FromBin).WithMany(p => p.StockTransferDetailFromBins)
                .HasForeignKey(d => d.FromBinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransferDetails_FromBin");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransferDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransferDetails_Products");

            entity.HasOne(d => d.ToBin).WithMany(p => p.StockTransferDetailToBins)
                .HasForeignKey(d => d.ToBinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransferDetails_ToBin");

            entity.HasOne(d => d.Transfer).WithMany(p => p.StockTransferDetails)
                .HasForeignKey(d => d.TransferId)
                .HasConstraintName("FK_StockTransferDetails_Transfers");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryId).HasName("PK__SubCateg__26BE5BF9580776CC");

            entity.HasIndex(e => e.SubCategoryCode, "UQ__SubCateg__E7EB0784FC45A639").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.SubCategoryCode).HasMaxLength(50);
            entity.Property(e => e.SubCategoryName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategories_Categories");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666948B5DD509");

            entity.HasIndex(e => e.IsActive, "IX_Suppliers_IsActive");

            entity.HasIndex(e => e.SupplierCode, "UX_Suppliers_Code_F")
                .IsUnique()
                .HasFilter("([SupplierCode] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UX_Suppliers_Email_F")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.SupplierName, "UX_Suppliers_Name_F")
                .IsUnique()
                .HasFilter("([SupplierName] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UX_Suppliers_Phone_F")
                .IsUnique()
                .HasFilter("([Phone] IS NOT NULL)");

            entity.HasIndex(e => e.TaxId, "UX_Suppliers_TaxId_F")
                .IsUnique()
                .HasFilter("([TaxId] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BankAccountNo).HasMaxLength(50);
            entity.Property(e => e.BankBranch).HasMaxLength(200);
            entity.Property(e => e.BankName).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreditLimit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.NTN)
                .HasMaxLength(50)
                .HasColumnName("NTN");
            entity.Property(e => e.PaymentTerms).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.SupplierCode).HasMaxLength(50);
            entity.Property(e => e.SupplierName).HasMaxLength(300);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        modelBuilder.Entity<SupplierContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Supplier__5C6625BBA8857DE8");

            entity.HasIndex(e => e.SupplierId, "IX_SupplierContacts_SupplierID");

            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierContacts)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierContacts_Suppliers");
        });

        modelBuilder.Entity<SupplierQuotation>(entity =>
        {
            entity.HasKey(e => e.QuotationId).HasName("PK__Supplier__E19752B3C6AF96E3");

            entity.HasIndex(e => e.Status, "IX_SupplierQuotations_Status");

            entity.HasIndex(e => e.SupplierId, "IX_SupplierQuotations_SupplierID");

            entity.HasIndex(e => e.WorkflowInstanceId, "IX_SupplierQuotations_WorkflowInstanceID");

            entity.HasIndex(e => e.QuotationNo, "UQ__Supplier__E192718177FA5213").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuotationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuotationNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierQuotations)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierQuotations_Suppliers");

            entity.HasOne(d => d.WorkflowInstance).WithMany(p => p.SupplierQuotations)
                .HasForeignKey(d => d.WorkflowInstanceId)
                .HasConstraintName("FK_SupplierQuotations_WorkflowInstances");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__SystemSe__54372AFDA48CF2CE");

            entity.HasIndex(e => e.SettingKey, "UQ__SystemSe__01E719AD6DB88761").IsUnique();

            entity.HasIndex(e => e.SettingKey, "UX_SystemSettings_SettingKey").IsUnique();

            entity.Property(e => e.DataType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsEncrypted).HasDefaultValue(false);
            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TaxSetting>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("PK__TaxSetti__711BE08C43C8CE5A");

            entity.HasIndex(e => e.TaxCode, "UQ__TaxSetti__12945A28B67D0A53").IsUnique();

            entity.Property(e => e.CompanyId).HasDefaultValue(1);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.TaxSettings)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaxSettings_Companies");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5EC9529FF48A4");

            entity.HasIndex(e => e.UnitCode, "UQ__Units__0665E6D9293372BF").IsUnique();

            entity.HasIndex(e => e.UnitCode, "UX_Units_UnitCode").IsUnique();

            entity.HasIndex(e => e.UnitName, "UX_Units_UnitName").IsUnique();

            entity.Property(e => e.ConversionFactor)
                .HasDefaultValue(1.0m)
                .HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBaseUnit).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UnitCode).HasMaxLength(10);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BaseUnit).WithMany(p => p.InverseBaseUnit)
                .HasForeignKey(d => d.BaseUnitId)
                .HasConstraintName("FK_Units_BaseUnit");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07DAF34399");

            entity.HasIndex(e => e.UserName, "UQ_Users_UserName").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "UQ__Users__1F6425486D9C599E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345D012C50").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456DAEB93E2").IsUnique();

            entity.HasIndex(e => e.Email, "UX_Users_Email").IsUnique();

            entity.HasIndex(e => e.UserName, "UX_Users_UserName").IsUnique();

            entity.Property(e => e.AccessFailedCount).HasDefaultValue(0);
            entity.Property(e => e.ConcurrencyStamp).HasMaxLength(200);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
            entity.Property(e => e.LockoutEnd).HasColumnType("datetime");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PhoneNumberConfirmed).HasDefaultValue(false);
            entity.Property(e => e.SecurityStamp).HasMaxLength(200);
            entity.Property(e => e.TwoFactorEnabled).HasDefaultValue(false);

            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Users)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK_Users_Warehouses");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_AspNetUserRoles_AspNetRoles"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AspNetUserRoles_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760ADD4854641");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__UserSess__C9F492700F177F1E");

            entity.HasIndex(e => e.SessionToken, "UQ__UserSess__46BDD124A81ADCA2").IsUnique();

            entity.Property(e => e.DeviceInfo).HasMaxLength(500);
            entity.Property(e => e.IPAddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastActivity)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LogoutTime).HasColumnType("datetime");
            entity.Property(e => e.SessionToken).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSessions_Users");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFD907140E66");

            entity.HasIndex(e => e.BranchId, "IX_Warehouses_BranchID");

            entity.HasIndex(e => e.IsActive, "IX_Warehouses_IsActive");

            entity.HasIndex(e => e.WarehouseCode, "UQ__Warehous__1686A056BF7CD1D5").IsUnique();

            entity.HasIndex(e => e.WarehouseCode, "UX_Warehouses_WarehouseCode").IsUnique();

            entity.HasIndex(e => e.WarehouseName, "UX_Warehouses_WarehouseName").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseCode).HasMaxLength(20);
            entity.Property(e => e.WarehouseName).HasMaxLength(200);

            entity.HasOne(d => d.Branch).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warehouses_Branches");
        });

        modelBuilder.Entity<WorkflowAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__Workflow__FFE3F4B90FFB8BCA");

            entity.Property(e => e.ActionCode).HasMaxLength(50);
            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFinal).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkflowAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Workflow__32499E5705FAAAC7");

            entity.HasIndex(e => e.RoleId, "IX_WorkflowAssignments_RoleID");

            entity.HasIndex(e => e.StageId, "IX_WorkflowAssignments_StageID");

            entity.HasIndex(e => e.UserId, "IX_WorkflowAssignments_UserID");

            entity.HasIndex(e => e.WorkflowDefId, "IX_WorkflowAssignments_WorkflowDefID");

            entity.Property(e => e.CanApprove).HasDefaultValue(false);
            entity.Property(e => e.CanHold).HasDefaultValue(false);
            entity.Property(e => e.CanReject).HasDefaultValue(false);
            entity.Property(e => e.CanSendBack).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.WorkflowAssignments)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_WorkflowAssignments_Role");

            entity.HasOne(d => d.Stage).WithMany(p => p.WorkflowAssignments)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowAssignments_WorkflowStages");

            entity.HasOne(d => d.User).WithMany(p => p.WorkflowAssignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_WorkflowAssignments_User");

            entity.HasOne(d => d.WorkflowDef).WithMany(p => p.WorkflowAssignments)
                .HasForeignKey(d => d.WorkflowDefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowAssignments_WorkflowDefinitions");
        });

        modelBuilder.Entity<WorkflowCondition>(entity =>
        {
            entity.HasKey(e => e.ConditionId).HasName("PK__Workflow__37F5C0EF65CB2C47");

            entity.HasIndex(e => e.StageFromId, "IX_WorkflowConditions_StageFromID");

            entity.HasIndex(e => e.StageToId, "IX_WorkflowConditions_StageToID");

            entity.HasIndex(e => e.WorkflowDefId, "IX_WorkflowConditions_WorkflowDefID");

            entity.Property(e => e.ActionOnFail)
                .HasMaxLength(50)
                .HasDefaultValue("Reject");
            entity.Property(e => e.ConditionType).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Operator).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.StageFrom).WithMany(p => p.WorkflowConditionStageFroms)
                .HasForeignKey(d => d.StageFromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowConditions_StageFrom");

            entity.HasOne(d => d.StageTo).WithMany(p => p.WorkflowConditionStageTos)
                .HasForeignKey(d => d.StageToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowConditions_StageTo");

            entity.HasOne(d => d.WorkflowDef).WithMany(p => p.WorkflowConditions)
                .HasForeignKey(d => d.WorkflowDefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowConditions_WorkflowDefinitions");
        });

        modelBuilder.Entity<WorkflowDeadline>(entity =>
        {
            entity.HasKey(e => e.DeadlineId).HasName("PK__Workflow__CAF057F70ABD090D");

            entity.HasIndex(e => e.DueDate, "IX_WorkflowDeadlines_DueDate");

            entity.HasIndex(e => e.StageId, "IX_WorkflowDeadlines_StageID");

            entity.HasIndex(e => e.WorkflowInstId, "IX_WorkflowDeadlines_WorkflowInstID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.EscalatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsMet).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.EscalatedToNavigation).WithMany(p => p.WorkflowDeadlines)
                .HasForeignKey(d => d.EscalatedTo)
                .HasConstraintName("FK_WorkflowDeadlines_EscalatedTo");

            entity.HasOne(d => d.Stage).WithMany(p => p.WorkflowDeadlines)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowDeadlines_StageID");

            entity.HasOne(d => d.WorkflowInst).WithMany(p => p.WorkflowDeadlines)
                .HasForeignKey(d => d.WorkflowInstId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowDeadlines_WorkflowInstances");
        });

        modelBuilder.Entity<WorkflowDefinition>(entity =>
        {
            entity.HasKey(e => e.WorkflowDefId).HasName("PK__Workflow__A61E4D70F9F4B247");

            entity.HasIndex(e => e.IsActive, "IX_WorkflowDefinitions_IsActive");

            entity.HasIndex(e => e.ModuleType, "IX_WorkflowDefinitions_ModuleType");

            entity.HasIndex(e => e.ModuleType, "UQ_WorkflowDefinition_ModuleType").IsUnique();

            entity.HasIndex(e => e.WorkflowName, "UX_WFDefs_WorkflowName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModuleType).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkflowName).HasMaxLength(200);
        });

        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.WorkflowInstId).HasName("PK__Workflow__AB2080AE427D3005");

            entity.HasIndex(e => e.CurrentStageId, "IX_WorkflowInstances_CurrentStageID");

            entity.HasIndex(e => e.RecordId, "IX_WorkflowInstances_RecordID");

            entity.HasIndex(e => e.RecordType, "IX_WorkflowInstances_RecordType");

            entity.HasIndex(e => e.Status, "IX_WorkflowInstances_Status");

            entity.HasIndex(e => e.WorkflowDefId, "IX_WorkflowInstances_WorkflowDefID");

            entity.Property(e => e.CompletedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RecordType).HasMaxLength(50);
            entity.Property(e => e.StartedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("InProgress");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CurrentStage).WithMany(p => p.WorkflowInstances)
                .HasForeignKey(d => d.CurrentStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowInstances_CurrentStage");

            entity.HasOne(d => d.WorkflowDef).WithMany(p => p.WorkflowInstances)
                .HasForeignKey(d => d.WorkflowDefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowInstances_WorkflowDefinitions");
        });

        modelBuilder.Entity<WorkflowInstanceLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Workflow__5E5499A8F3F33C26");

            entity.HasIndex(e => e.ActionDate, "IX_WorkflowInstanceLogs_ActionDate");

            entity.HasIndex(e => e.UserId, "IX_WorkflowInstanceLogs_UserID");

            entity.HasIndex(e => e.WorkflowInstId, "IX_WorkflowInstanceLogs_WorkflowInstID");

            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IPAddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.Action).WithMany(p => p.WorkflowInstanceLogs)
                .HasForeignKey(d => d.ActionId)
                .HasConstraintName("FK_WorkflowInstanceLog_s_WorkflowActions");

            entity.HasOne(d => d.StageFrom).WithMany(p => p.WorkflowInstanceLogStageFroms)
                .HasForeignKey(d => d.StageFromId)
                .HasConstraintName("FK_WorkflowInstanceLogs_StageFrom");

            entity.HasOne(d => d.StageTo).WithMany(p => p.WorkflowInstanceLogStageTos)
                .HasForeignKey(d => d.StageToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowInstanceLogs_StageTo");

            entity.HasOne(d => d.User).WithMany(p => p.WorkflowInstanceLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowInstanceLogs_User");

            entity.HasOne(d => d.WorkflowInst).WithMany(p => p.WorkflowInstanceLogs)
                .HasForeignKey(d => d.WorkflowInstId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowInstanceLogs_WorkflowInstances");
        });

        modelBuilder.Entity<WorkflowNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Workflow__20CF2E3229514DB3");

            entity.HasIndex(e => e.IsRead, "IX_WorkflowNotifications_IsRead");

            entity.HasIndex(e => e.UserId, "IX_WorkflowNotifications_UserID");

            entity.HasIndex(e => e.WorkflowInstId, "IX_WorkflowNotifications_WorkflowInstID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.NotificationType)
                .HasMaxLength(50)
                .HasDefaultValue("Email");
            entity.Property(e => e.ReadDate).HasColumnType("datetime");
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.WorkflowNotifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowNotifications_User");

            entity.HasOne(d => d.WorkflowInst).WithMany(p => p.WorkflowNotifications)
                .HasForeignKey(d => d.WorkflowInstId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowNotifications_WorkflowInstances");
        });

        modelBuilder.Entity<WorkflowStage>(entity =>
        {
            entity.HasKey(e => e.StageId).HasName("PK__Workflow__03EB7AF8472D07C1");

            entity.HasIndex(e => e.StageOrder, "IX_WorkflowStages_StageOrder");

            entity.HasIndex(e => e.WorkflowDefId, "IX_WorkflowStages_WorkflowDefID");

            entity.HasIndex(e => new { e.WorkflowDefId, e.StageCode }, "UX_WFStages_StageCode_DefId").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsEnd).HasDefaultValue(false);
            entity.Property(e => e.IsStart).HasDefaultValue(false);
            entity.Property(e => e.SladeadlineHours).HasColumnName("SLADeadlineHours");
            entity.Property(e => e.StageCode).HasMaxLength(50);
            entity.Property(e => e.StageName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.WorkflowDef).WithMany(p => p.WorkflowStages)
                .HasForeignKey(d => d.WorkflowDefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowStages_WorkflowDefinitions");
        });

        modelBuilder.Entity<WorkflowTransition>(entity =>
        {
            entity.HasKey(e => e.TransitionId).HasName("PK__Workflow__54F048475DD5F971");

            entity.HasIndex(e => e.ActionId, "IX_WorkflowTransitions_ActionID");

            entity.HasIndex(e => e.StageFromId, "IX_WorkflowTransitions_StageFromID");

            entity.HasIndex(e => e.StageToId, "IX_WorkflowTransitions_StageToID");

            entity.HasIndex(e => e.WorkflowDefId, "IX_WorkflowTransitions_WorkflowDefID");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsAllowed).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Action).WithMany(p => p.WorkflowTransitions)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowTransitions_WorkflowActions");

            entity.HasOne(d => d.StageFrom).WithMany(p => p.WorkflowTransitionStageFroms)
                .HasForeignKey(d => d.StageFromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowTransitions_StageFrom");

            entity.HasOne(d => d.StageTo).WithMany(p => p.WorkflowTransitionStageTos)
                .HasForeignKey(d => d.StageToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowTransitions_StageTo");

            entity.HasOne(d => d.WorkflowDef).WithMany(p => p.WorkflowTransitions)
                .HasForeignKey(d => d.WorkflowDefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkflowTransitions_WorkflowDefinitions");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__Zones__6016679592546CE2");

            entity.HasIndex(e => e.WarehouseId, "IX_Zones_WarehouseID");

            entity.HasIndex(e => e.ZoneCode, "UQ__Zones__005DD0BC712843BA").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(100);

            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.ZoneCode).HasMaxLength(20);
            entity.Property(e => e.ZoneName).HasMaxLength(100);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Zones)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zones_Warehouses");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}










