using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetRo__3214EC070D172892", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BrandCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LogoURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Brands__DAD4F3BECF2D5F8B", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__19093A2BA0C0A00A", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Companie__2D971C4CDB2EC2E0", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NTN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    CreditDays = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PaymentTerms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64B8B3F7C3A9", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "DashboardWidgets",
                columns: table => new
                {
                    WidgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WidgetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WidgetConfig = table.Column<string>(type: "json", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dashboar__ADFD307220315B63", x => x.WidgetId);
                });

            migrationBuilder.CreateTable(
                name: "DataBackupLogs",
                columns: table => new
                {
                    BackupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackupDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    BackupType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FileSizeMB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DataBack__EB9069E2E94B0AA7", x => x.BackupId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__C8EE204333154D6F", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    ErrorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TraceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExceptionType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequestURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ErrorLog__358565CA10610678", x => x.ErrorId);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModuleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Modules__2B7477871FADDB09", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__F87ADD07A879CF2B", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Password__3214EC0774178481", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__C8EE20438EEABA77", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "ReportDefinitions",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QueryDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parameters = table.Column<string>(type: "json", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportDe__D5BD48E5CFE81B71", x => x.ReportId);
                });

            migrationBuilder.CreateTable(
                name: "SalaryComponents",
                columns: table => new
                {
                    ComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComponentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalaryCo__D79CF02EF3553438", x => x.ComponentId);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalesOrd__C8EE20438742B90B", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledReports",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ScheduleTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ScheduleDays = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Recipients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastRunDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    NextRunDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Active"),
                    Parameters = table.Column<string>(type: "json", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__9C8A5B694FF8F3F0", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    GracePeriodMinutes = table.Column<int>(type: "int", nullable: true, defaultValue: 15),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shifts__C0A838E162D06154", x => x.ShiftId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NTN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankAccountNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankBranch = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    PaymentTerms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__4BE666948B5DD509", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SystemSe__54372AFDA48CF2CE", x => x.SettingId);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsBaseUnit = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,4)", nullable: true, defaultValue: 1.0m),
                    BaseUnitId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Units__44F5EC9529FF48A4", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_Units_BaseUnit",
                        column: x => x.BaseUnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowActions",
                columns: table => new
                {
                    ActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__FFE3F4B90FFB8BCA", x => x.ActionId);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModuleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__A61E4D70F9F4B247", x => x.WorkflowDefId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetRo__3214EC07031A00E6", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SubCateg__26BE5BF9580776CC", x => x.SubCategoryId);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    BankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BankAcco__4FC8E741BFB9B295", x => x.BankAccountId);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Branches__A1682FA593F86D01", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branches_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentAccountId = table.Column<int>(type: "int", nullable: true),
                    AccountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChartOfA__349DA586099C5D6F", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_Parent",
                        column: x => x.ParentAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "TaxSettings",
                columns: table => new
                {
                    TaxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaxSetti__711BE08C43C8CE5A", x => x.TaxId);
                    table.ForeignKey(
                        name: "FK_TaxSettings_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerContacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__5C6625BBD49455AD", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_CustomerContacts_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerFeedback",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__6A4BEDF6997C44DB", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_CustomerFeedback_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "ModulePermissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ModulePe__EFA6FB0F79896DFA", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_ModulePermissions_AspNetRoles",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModulePermissions_Modules",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId");
                });

            migrationBuilder.CreateTable(
                name: "SupplierContacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__5C6625BBA8857DE8", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_SupplierContacts_Suppliers",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStages",
                columns: table => new
                {
                    StageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    IsStart = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsEnd = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    SLADeadlineHours = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__03EB7AF8472D07C1", x => x.StageId);
                    table.ForeignKey(
                        name: "FK_WorkflowStages_WorkflowDefinitions",
                        column: x => x.WorkflowDefId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "WorkflowDefId");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    MinStockLevel = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    MaxStockLevel = table.Column<int>(type: "int", nullable: true, defaultValue: 999999),
                    ReorderLevel = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsSerialized = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsBatchManaged = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__B40CC6ED37103C80", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Brands",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId");
                    table.ForeignKey(
                        name: "FK_Products_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_Products_SubCategories",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "SubCategoryId");
                    table.ForeignKey(
                        name: "FK_Products_Units",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "BankReconciliation",
                columns: table => new
                {
                    ReconciliationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    StatementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StatementBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SystemBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Difference = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    ReconciliationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BankReco__096DC860E6FC8211", x => x.ReconciliationId);
                    table.ForeignKey(
                        name: "FK_BankReconciliation_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId");
                });

            migrationBuilder.CreateTable(
                name: "BankTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BankTran__55433A4BD3D50720", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_BankTransactions_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId");
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__B2079BCDC850A6FA", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_Departments_Branches",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId");
                    table.ForeignKey(
                        name: "FK_Departments_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Warehous__2608AFD907140E66", x => x.WarehouseId);
                    table.ForeignKey(
                        name: "FK_Warehouses_Branches",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId");
                });

            migrationBuilder.CreateTable(
                name: "AccountLedger",
                columns: table => new
                {
                    LedgerEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    VoucherType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccountL__ACE98A5A36F5EB46", x => x.LedgerEntryId);
                    table.ForeignKey(
                        name: "FK_AccountLedger_ChartOfAccounts",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowConditions",
                columns: table => new
                {
                    ConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false),
                    StageFromId = table.Column<int>(type: "int", nullable: false),
                    StageToId = table.Column<int>(type: "int", nullable: false),
                    ConditionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionOnFail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Reject"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__37F5C0EF65CB2C47", x => x.ConditionId);
                    table.ForeignKey(
                        name: "FK_WorkflowConditions_StageFrom",
                        column: x => x.StageFromId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowConditions_StageTo",
                        column: x => x.StageToId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowConditions_WorkflowDefinitions",
                        column: x => x.WorkflowDefId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "WorkflowDefId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    WorkflowInstId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentStageId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "InProgress"),
                    StartedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CompletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__AB2080AE427D3005", x => x.WorkflowInstId);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_CurrentStage",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_WorkflowDefinitions",
                        column: x => x.WorkflowDefId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "WorkflowDefId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    TransitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false),
                    StageFromId = table.Column<int>(type: "int", nullable: false),
                    StageToId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__54F048475DD5F971", x => x.TransitionId);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_StageFrom",
                        column: x => x.StageFromId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_StageTo",
                        column: x => x.StageToId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowActions",
                        column: x => x.ActionId,
                        principalTable: "WorkflowActions",
                        principalColumn: "ActionId");
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowDefinitions",
                        column: x => x.WorkflowDefId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "WorkflowDefId");
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                columns: table => new
                {
                    AttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AttributeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductA__C189298AE4957945", x => x.AttributeId);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductI__7516F4EC37B02ED2", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    DesignationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RankLevel = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Designat__BABD603E441E7F05", x => x.DesignationId);
                    table.ForeignKey(
                        name: "FK_Designations_Departments",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId");
                });

            migrationBuilder.CreateTable(
                name: "InventoryValuation",
                columns: table => new
                {
                    ValuationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ValuationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__CF17CF52CC9964ED", x => x.ValuationId);
                    table.ForeignKey(
                        name: "FK_InventoryValuation_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_InventoryValuation_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    LastLoginDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC07DAF34399", x => x.Id);
                    table.UniqueConstraint("AK_Users_UserName", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_Users_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZoneCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Zones__6016679592546CE2", x => x.ZoneId);
                    table.ForeignKey(
                        name: "FK_Zones_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseMasters",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__6B0A6BDECAD3925C", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_PurchaseMasters_Suppliers",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                    table.ForeignKey(
                        name: "FK_PurchaseMasters_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_PurchaseMasters_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_PurchaseMasters_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "SalesMasters",
                columns: table => new
                {
                    SalesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    SalesOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalesMas__C952FB128EB0F677", x => x.SalesId);
                    table.ForeignKey(
                        name: "FK_SalesMasters_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_SalesMasters_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_SalesMasters_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_SalesMasters_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "SupplierQuotations",
                columns: table => new
                {
                    QuotationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    QuotationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuotationDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__E19752B3C6AF96E3", x => x.QuotationId);
                    table.ForeignKey(
                        name: "FK_SupplierQuotations_Suppliers",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                    table.ForeignKey(
                        name: "FK_SupplierQuotations_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DesignationId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Cnic = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TerminationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShiftId = table.Column<int>(type: "int", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DOB1 = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__7AD04FF1EEF6AF6A", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_Shifts",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                    table.ForeignKey(
                        name: "FK_Employees_Branches",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId");
                    table.ForeignKey(
                        name: "FK_Employees_Departments",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId");
                    table.ForeignKey(
                        name: "FK_Employees_Designations",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "DesignationId");
                    table.ForeignKey(
                        name: "FK_Employees_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetUs__3214EC07D40D00E1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetUs__2B2C5B526918598C", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetUs__AF2760ADD4854641", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AspNetUs__8CC49841F90B3A6C", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__A17F23B8FBC043B9", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Modules",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerComplaints",
                columns: table => new
                {
                    ComplaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ComplaintType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Open"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    EscalatedTo = table.Column<int>(type: "int", nullable: true),
                    ResolvedBy = table.Column<int>(type: "int", nullable: true),
                    ResolutionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ResolutionRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__740D89AF4BDD5761", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_EscalatedTo",
                        column: x => x.EscalatedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_ResolvedBy",
                        column: x => x.ResolvedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerInquiries",
                columns: table => new
                {
                    InquiryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    InquiryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Open"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    ResolvedBy = table.Column<int>(type: "int", nullable: true),
                    ResolvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__05E6E7EFDA90D1EE", x => x.InquiryId);
                    table.ForeignKey(
                        name: "FK_CustomerInquiries_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerInquiries_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_CustomerInquiries_ResolvedBy",
                        column: x => x.ResolvedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerInquiries_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_CustomerInquiries_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "JournalVouchers",
                columns: table => new
                {
                    VoucherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JournalV__3AEE79C11B230391", x => x.VoucherId);
                    table.ForeignKey(
                        name: "FK_JournalVouchers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_JournalVouchers_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_JournalVouchers_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "LoginHistory",
                columns: table => new
                {
                    LoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    LogoutTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoginHis__4DDA283820AEB128", x => x.LoginId);
                    table.ForeignKey(
                        name: "FK_LoginHistory_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ReadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceModule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32CD4241CE", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationTemplates",
                        column: x => x.TemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "TemplateId");
                    table.ForeignKey(
                        name: "FK_Notifications_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductPricingHistory",
                columns: table => new
                {
                    PriceHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductP__A927CB2B5C28FDFA", x => x.PriceHistoryId);
                    table.ForeignKey(
                        name: "FK_ProductPricingHistory_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductPricingHistory_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromWarehouseId = table.Column<int>(type: "int", nullable: false),
                    ToWarehouseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TransferNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockTra__95490171B1DA619E", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_StockTransfersProducts",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_StockTransfers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_StockTransfers_FromWarehouse",
                        column: x => x.FromWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_StockTransfers_ReceivedBy",
                        column: x => x.ReceivedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_StockTransfers_ToWarehouse",
                        column: x => x.ToWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_StockTransfers_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_StockTransfers_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    LastActivity = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    LogoutTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserSess__C9F492700F177F1E", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CanApprove = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanReject = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanHold = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanSendBack = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__32499E5705FAAAC7", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_Role",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_WorkflowDefinitions",
                        column: x => x.WorkflowDefId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "WorkflowDefId");
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_WorkflowStages",
                        column: x => x.StageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDeadlines",
                columns: table => new
                {
                    DeadlineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EscalatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    EscalatedTo = table.Column<int>(type: "int", nullable: true),
                    IsMet = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__CAF057F70ABD090D", x => x.DeadlineId);
                    table.ForeignKey(
                        name: "FK_WorkflowDeadlines_EscalatedTo",
                        column: x => x.EscalatedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowDeadlines_StageID",
                        column: x => x.StageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowDeadlines_WorkflowInstances",
                        column: x => x.WorkflowInstId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstId = table.Column<int>(type: "int", nullable: false),
                    StageFromId = table.Column<int>(type: "int", nullable: true),
                    StageToId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__5E5499A8F3F33C26", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceLog_s_WorkflowActions",
                        column: x => x.ActionId,
                        principalTable: "WorkflowActions",
                        principalColumn: "ActionId");
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceLogs_StageFrom",
                        column: x => x.StageFromId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceLogs_StageTo",
                        column: x => x.StageToId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceLogs_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceLogs_WorkflowInstances",
                        column: x => x.WorkflowInstId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ReadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Email"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__20CF2E3229514DB3", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_WorkflowNotifications_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowNotifications_WorkflowInstances",
                        column: x => x.WorkflowInstId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "Racks",
                columns: table => new
                {
                    RackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    RackName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RackCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Racks__0363D9486B4EFF42", x => x.RackId);
                    table.ForeignKey(
                        name: "FK_Racks_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                });

            migrationBuilder.CreateTable(
                name: "GRN",
                columns: table => new
                {
                    GrnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    GrnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TotalReceivedQty = table.Column<int>(type: "int", nullable: false),
                    TotalAcceptedQty = table.Column<int>(type: "int", nullable: false),
                    TotalRejectedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GRN__BC0E8C62AE01BC15", x => x.GrnId);
                    table.ForeignKey(
                        name: "FK_GRN_PurchaseMasters",
                        column: x => x.PurchaseId,
                        principalTable: "PurchaseMasters",
                        principalColumn: "PurchaseId");
                    table.ForeignKey(
                        name: "FK_GRN_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_GRN_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_GRN_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "PaymentVouchers",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    PaymentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentV__9B556A580A50AB1E", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId");
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_PurchaseMasters",
                        column: x => x.PurchaseId,
                        principalTable: "PurchaseMasters",
                        principalColumn: "PurchaseId");
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_Suppliers",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_PaymentVouchers_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReceivedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__135C314D5FEF6E3E", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_PurchaseDetails_PurchaseMasters",
                        column: x => x.PurchaseId,
                        principalTable: "PurchaseMasters",
                        principalColumn: "PurchaseId");
                });

            migrationBuilder.CreateTable(
                name: "DispatchMasters",
                columns: table => new
                {
                    DispatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    DispatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DispatchDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CarrierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrackingNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    DispatchedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dispatch__434DBD75E0D2D673", x => x.DispatchId);
                    table.ForeignKey(
                        name: "FK_DispatchMasters_DispatchedBy",
                        column: x => x.DispatchedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_DispatchMasters_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_DispatchMasters_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_DispatchMasters_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_DispatchMasters_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    BalanceDue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Unpaid"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoices__D796AAD552D409B9", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_Invoices_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_Invoices_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "PickingLists",
                columns: table => new
                {
                    PickingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    PickingNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PickingDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Created"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    PickedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PickingL__A93D74E0792E55BE", x => x.PickingId);
                    table.ForeignKey(
                        name: "FK_PickingLists_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PickingLists_PickedBy",
                        column: x => x.PickedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_PickingLists_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_PickingLists_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_PickingLists_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_PickingLists_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_PickingLists_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptVouchers",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    ReceiptNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReceiptV__CC08C400B9906120", x => x.ReceiptId);
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId");
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_ReceiptVouchers_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "SalesDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AllocatedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PickedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    ShippedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SalesDetailId = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalesDet__135C314D4A34B2F0", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_SalesDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_SalesDetails_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAttendance",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckInTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OvertimeHours = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShiftId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__8B69263C774A3B52", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendance_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendance_Shifts",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBankAccounts",
                columns: table => new
                {
                    BankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__4FC8E7417B00B282", x => x.BankAccountId);
                    table.ForeignKey(
                        name: "FK_EmployeeBankAccounts_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeave",
                columns: table => new
                {
                    LeaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__796DB979EB15456E", x => x.LeaveId);
                    table.ForeignKey(
                        name: "FK_EmployeeLeave_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_EmployeeLeave_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeeLeave_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaves",
                columns: table => new
                {
                    LeaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__796DB959CCB174FD", x => x.LeaveId);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_EmployeeLeaves_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    SalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UtilityAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherAllowances = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalAllowances = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProfessionalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__4BE20457674B57E1", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_EmpSalaries_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalary",
                columns: table => new
                {
                    SalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    UtilityAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    OtherAllowances = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TotalAllowances = table.Column<decimal>(type: "decimal(21,2)", nullable: true, computedColumnSql: "((([HouseRent]+[UtilityAllowance])+[MedicalAllowance])+[OtherAllowances])", stored: true),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    ProfessionalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    TotalDeductions = table.Column<decimal>(type: "decimal(20,2)", nullable: true, computedColumnSql: "(([IncomeTax]+[ProfessionalTax])+[OtherDeductions])", stored: true),
                    NetSalary = table.Column<decimal>(type: "decimal(23,2)", nullable: true, computedColumnSql: "(([BasicSalary]+((([HouseRent]+[UtilityAllowance])+[MedicalAllowance])+[OtherAllowances]))-(([IncomeTax]+[ProfessionalTax])+[OtherDeductions]))", stored: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__4BE204B7225312D7", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_EmployeeSalary_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "JournalVoucherDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JournalV__135C316D2542D4F8", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_JVD_Accounts",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_JVD_Vouchers",
                        column: x => x.VoucherId,
                        principalTable: "JournalVouchers",
                        principalColumn: "VoucherId");
                });

            migrationBuilder.CreateTable(
                name: "Bins",
                columns: table => new
                {
                    BinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RackId = table.Column<int>(type: "int", nullable: false),
                    BinName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BinCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bins__4BFF5A4EE5B1B3A3", x => x.BinId);
                    table.ForeignKey(
                        name: "FK_Bins_Racks",
                        column: x => x.RackId,
                        principalTable: "Racks",
                        principalColumn: "RackId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReturns",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    GrnId = table.Column<int>(type: "int", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__F445E988FA473F9E", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_PurchaseReturns_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_PurchaseReturns_GRN",
                        column: x => x.GrnId,
                        principalTable: "GRN",
                        principalColumn: "GrnId");
                    table.ForeignKey(
                        name: "FK_PurchaseReturns_PurchaseMasters",
                        column: x => x.PurchaseId,
                        principalTable: "PurchaseMasters",
                        principalColumn: "PurchaseId");
                    table.ForeignKey(
                        name: "FK_PurchaseReturns_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_PurchaseReturns_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "PaymentVoucherDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentV__135C316D29F85606", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_PVD_Accounts",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_PVD_Payments",
                        column: x => x.PaymentId,
                        principalTable: "PaymentVouchers",
                        principalColumn: "PaymentId");
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChallans",
                columns: table => new
                {
                    ChallanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    DispatchId = table.Column<int>(type: "int", nullable: false),
                    ChallanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChallanDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Delivery__BFB043C452E881D1", x => x.ChallanId);
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_DispatchMasters",
                        column: x => x.DispatchId,
                        principalTable: "DispatchMasters",
                        principalColumn: "DispatchId");
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "SalesReturns",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalesRet__F445E98848DF03F5", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_SalesReturns_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_SalesReturns_Invoices",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_SalesReturns_SalesMasters",
                        column: x => x.SalesId,
                        principalTable: "SalesMasters",
                        principalColumn: "SalesId");
                    table.ForeignKey(
                        name: "FK_SalesReturns_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_SalesReturns_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptVoucherDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReceiptV__135C316DAEC0FC63", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_RVD_Accounts",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_RVD_Receipts",
                        column: x => x.ReceiptId,
                        principalTable: "ReceiptVouchers",
                        principalColumn: "ReceiptId");
                });

            migrationBuilder.CreateTable(
                name: "DispatchDetails",
                columns: table => new
                {
                    DispatchDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchId = table.Column<int>(type: "int", nullable: false),
                    SalesDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dispatch__8B84B60099F1B6E4", x => x.DispatchDetailId);
                    table.ForeignKey(
                        name: "FK_DispatchDetails_DispatchMasters",
                        column: x => x.DispatchId,
                        principalTable: "DispatchMasters",
                        principalColumn: "DispatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DispatchDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_DispatchDetails_SalesDetails",
                        column: x => x.SalesDetailId,
                        principalTable: "SalesDetails",
                        principalColumn: "DetailId");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    InvoiceDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    SalesDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvoiceD__1F1578F1D344532E", x => x.InvoiceDetailId);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_SalesDetails",
                        column: x => x.SalesDetailId,
                        principalTable: "SalesDetails",
                        principalColumn: "DetailId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeePayroll",
                columns: table => new
                {
                    PayrollId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    MonthYear = table.Column<DateOnly>(type: "date", nullable: false),
                    GrossPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Draft"),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__99DFC69249C6CFC9", x => x.PayrollId);
                    table.ForeignKey(
                        name: "FK_EmployeePayroll_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_EmployeePayroll_EmployeeSalary",
                        column: x => x.SalaryId,
                        principalTable: "EmployeeSalary",
                        principalColumn: "SalaryId");
                    table.ForeignKey(
                        name: "FK_EmployeePayroll_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeePayroll_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                });

            migrationBuilder.CreateTable(
                name: "CycleCounts",
                columns: table => new
                {
                    CycleCountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SystemQty = table.Column<int>(type: "int", nullable: false),
                    ActualQty = table.Column<int>(type: "int", nullable: false),
                    Variance = table.Column<int>(type: "int", nullable: false),
                    CountedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CountedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    VerifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CycleCou__A4189B51045FD997", x => x.CycleCountId);
                    table.ForeignKey(
                        name: "FK_CycleCounts_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_CycleCounts_CountedBy",
                        column: x => x.CountedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_CycleCounts_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_CycleCounts_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_CycleCounts_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "GRNDetails",
                columns: table => new
                {
                    GRNDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrnId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    ReceivedQty = table.Column<int>(type: "int", nullable: false),
                    AcceptedQty = table.Column<int>(type: "int", nullable: false),
                    RejectedQty = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SerialNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MfgDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GRNDetai__B7FEA6A1203C8712", x => x.GRNDetailId);
                    table.ForeignKey(
                        name: "FK_GRNDetails_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_GRNDetails_GRN",
                        column: x => x.GrnId,
                        principalTable: "GRN",
                        principalColumn: "GrnId");
                    table.ForeignKey(
                        name: "FK_GRNDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_GRNDetails_PurchaseDetails",
                        column: x => x.PurchaseDetailId,
                        principalTable: "PurchaseDetails",
                        principalColumn: "DetailId");
                });

            migrationBuilder.CreateTable(
                name: "PickingDetails",
                columns: table => new
                {
                    PickingDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingId = table.Column<int>(type: "int", nullable: false),
                    SalesDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    PickedQty = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PickingD__8189D109015FDA2D", x => x.PickingDetailId);
                    table.ForeignKey(
                        name: "FK_PickingDetails_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_PickingDetails_PickingLists",
                        column: x => x.PickingId,
                        principalTable: "PickingLists",
                        principalColumn: "PickingId");
                    table.ForeignKey(
                        name: "FK_PickingDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_PickingDetails_SalesDetails",
                        column: x => x.SalesDetailId,
                        principalTable: "SalesDetails",
                        principalColumn: "DetailId");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    AdjustmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    AdjustmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PreviousQty = table.Column<int>(type: "int", nullable: false),
                    NewQty = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: true),
                    CurrentStageId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockAdj__E60DB8B3F69F412D", x => x.AdjustmentId);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_WorkflowInstances",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowInstId");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_WorkflowStages",
                        column: x => x.CurrentStageId,
                        principalTable: "WorkflowStages",
                        principalColumn: "StageId");
                });

            migrationBuilder.CreateTable(
                name: "StockLedgers",
                columns: table => new
                {
                    LedgerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferenceDocNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceDocType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockLed__AE70E0AF173C007C", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_StockLedger_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_StockLedger_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_StockLedger_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "StockTransferDetails",
                columns: table => new
                {
                    TransferDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FromBinId = table.Column<int>(type: "int", nullable: false),
                    ToBinId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockTra__F9BF690FAF5CF4DB", x => x.TransferDetailId);
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_FromBin",
                        column: x => x.FromBinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_ToBin",
                        column: x => x.ToBinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_Transfers",
                        column: x => x.TransferId,
                        principalTable: "StockTransfers",
                        principalColumn: "TransferId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReturnDetails",
                columns: table => new
                {
                    ReturnDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__8B89C9EAC9883A03", x => x.ReturnDetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseReturnDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_PurchaseReturnDetails_PurchaseDetails",
                        column: x => x.PurchaseDetailId,
                        principalTable: "PurchaseDetails",
                        principalColumn: "DetailId");
                    table.ForeignKey(
                        name: "FK_PurchaseReturnDetails_Returns",
                        column: x => x.ReturnId,
                        principalTable: "PurchaseReturns",
                        principalColumn: "ReturnId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnDetails",
                columns: table => new
                {
                    ReturnDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    InvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalesRet__8B89C9EA579A657C", x => x.ReturnDetailId);
                    table.ForeignKey(
                        name: "FK_SalesReturnDetails_InvoiceDetails",
                        column: x => x.InvoiceDetailId,
                        principalTable: "InvoiceDetails",
                        principalColumn: "InvoiceDetailId");
                    table.ForeignKey(
                        name: "FK_SalesReturnDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_SalesReturnDetails_Returns",
                        column: x => x.ReturnId,
                        principalTable: "SalesReturns",
                        principalColumn: "ReturnId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePayrollDetails",
                columns: table => new
                {
                    PayrollDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__010127A91EFF93FC", x => x.PayrollDetailId);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollDetails_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeePayrollDetails_Payroll",
                        column: x => x.PayrollId,
                        principalTable: "EmployeePayroll",
                        principalColumn: "PayrollId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollDetails_SalaryComponents",
                        column: x => x.ComponentId,
                        principalTable: "SalaryComponents",
                        principalColumn: "ComponentId");
                });

            migrationBuilder.CreateTable(
                name: "CycleCountDetails",
                columns: table => new
                {
                    CountDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    SystemQty = table.Column<int>(type: "int", nullable: false),
                    ActualQty = table.Column<int>(type: "int", nullable: false),
                    Variance = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CycleCou__8C9890BB37DFFB17", x => x.CountDetailId);
                    table.ForeignKey(
                        name: "FK_CycleCountDetails_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_CycleCountDetails_Counts",
                        column: x => x.CountId,
                        principalTable: "CycleCounts",
                        principalColumn: "CycleCountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CycleCountDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "BatchTracking",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    GRNDetailId = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RemainingQty = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BatchTra__5D55CE388A1B93D6", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_BatchTracking_GRNDetails",
                        column: x => x.GRNDetailId,
                        principalTable: "GRNDetails",
                        principalColumn: "GRNDetailId");
                    table.ForeignKey(
                        name: "FK_BatchTracking_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "SerialNumbers",
                columns: table => new
                {
                    SerialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    GRNDetailId = table.Column<int>(type: "int", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "IN"),
                    SalesDetailId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    OutDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SerialNu__5E5B3EC49E9A4FF2", x => x.SerialId);
                    table.ForeignKey(
                        name: "FK_SerialNumbers_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_SerialNumbers_GRNDetails",
                        column: x => x.GRNDetailId,
                        principalTable: "GRNDetails",
                        principalColumn: "GRNDetailId");
                    table.ForeignKey(
                        name: "FK_SerialNumbers_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_SerialNumbers_SalesDetails",
                        column: x => x.SalesDetailId,
                        principalTable: "SalesDetails",
                        principalColumn: "DetailId");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentDetails",
                columns: table => new
                {
                    AdjustmentDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjustmentId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BinId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PreviousQty = table.Column<int>(type: "int", nullable: false),
                    NewQty = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockAdj__A4CB61D8BF9810D0", x => x.AdjustmentDetailId);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_Adjustments",
                        column: x => x.AdjustmentId,
                        principalTable: "StockAdjustments",
                        principalColumn: "AdjustmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountLedger_AccountID",
                table: "AccountLedger",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLedger_EntryDate",
                table: "AccountLedger",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLedger_VoucherID",
                table: "AccountLedger",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLedger_VoucherType",
                table: "AccountLedger",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__AspNetRo__737584F669A3CA15",
                table: "AspNetRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__AspNetRo__A93C97B917A1600B",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AspNetRoles_Name",
                table: "AspNetRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ModuleId",
                table: "AuditLogs",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserID",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CompanyId",
                table: "BankAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UQ__BankAcco__8235CCBCD1DC99C8",
                table: "BankAccounts",
                column: "Iban",
                unique: true,
                filter: "[Iban] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BankReconciliation_BankAccountID",
                table: "BankReconciliation",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BankAccountId",
                table: "BankTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTracking_BatchNo",
                table: "BatchTracking",
                column: "BatchNo");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTracking_ExpiryDate",
                table: "BatchTracking",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTracking_GRNDetailID",
                table: "BatchTracking",
                column: "GRNDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTracking_IsActive",
                table: "BatchTracking",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTracking_ProductID",
                table: "BatchTracking",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Bins_IsOccupied",
                table: "Bins",
                column: "IsOccupied");

            migrationBuilder.CreateIndex(
                name: "IX_Bins_RackID",
                table: "Bins",
                column: "RackId");

            migrationBuilder.CreateIndex(
                name: "UQ__Bins__634342456C4CE7E7",
                table: "Bins",
                column: "BinCode",
                unique: true,
                filter: "[BinCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyID",
                table: "Branches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_IsActive",
                table: "Branches",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_IsDeleted",
                table: "Branches",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "UQ__Branches__1C61B88890C8036C",
                table: "Branches",
                column: "BranchCode",
                unique: true,
                filter: "[BranchCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Branches_BranchCode",
                table: "Branches",
                column: "BranchCode",
                unique: true,
                filter: "[BranchCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Branches_BranchName",
                table: "Branches",
                column: "BranchName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Brands__44292CC7E2D45ED3",
                table: "Brands",
                column: "BrandCode",
                unique: true,
                filter: "[BrandCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Brands_BrandName",
                table: "Brands",
                column: "BrandName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Categori__371BA955AE2B3953",
                table: "Categories",
                column: "CategoryCode",
                unique: true,
                filter: "[CategoryCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_AccountType",
                table: "ChartOfAccounts",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_CompanyId",
                table: "ChartOfAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_ParentAccountID",
                table: "ChartOfAccounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "UQ__ChartOfA__38D0C56AF1D9515C",
                table: "ChartOfAccounts",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsActive",
                table: "Companies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsDeleted",
                table: "Companies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "UQ__Companie__6EF5E042AE5386DC",
                table: "Companies",
                column: "RegistrationNo",
                unique: true,
                filter: "[RegistrationNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Companies_CompanyName",
                table: "Companies",
                column: "CompanyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_AssignedTo",
                table: "CustomerComplaints",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_CurrentStageId",
                table: "CustomerComplaints",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_CustomerId",
                table: "CustomerComplaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_EscalatedTo",
                table: "CustomerComplaints",
                column: "EscalatedTo");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_ResolvedBy",
                table: "CustomerComplaints",
                column: "ResolvedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_WorkflowInstanceId",
                table: "CustomerComplaints",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_CustomerID",
                table: "CustomerContacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFeedback_CustomerId",
                table: "CustomerFeedback",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInquiries_AssignedTo",
                table: "CustomerInquiries",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInquiries_CurrentStageId",
                table: "CustomerInquiries",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInquiries_CustomerId",
                table: "CustomerInquiries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInquiries_ResolvedBy",
                table: "CustomerInquiries",
                column: "ResolvedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInquiries_WorkflowInstanceId",
                table: "CustomerInquiries",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive",
                table: "Customers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Code_F",
                table: "Customers",
                column: "CustomerCode",
                unique: true,
                filter: "([CustomerCode] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_CustomerCode_Filtered",
                table: "Customers",
                column: "CustomerCode",
                unique: true,
                filter: "([CustomerCode] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_CustomerName_Filtered",
                table: "Customers",
                column: "CustomerName",
                unique: true,
                filter: "([CustomerName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Email_F",
                table: "Customers",
                column: "Email",
                unique: true,
                filter: "([Email] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Email_Filtered",
                table: "Customers",
                column: "Email",
                unique: true,
                filter: "([Email] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Name_F",
                table: "Customers",
                column: "CustomerName",
                unique: true,
                filter: "([CustomerName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Phone_F",
                table: "Customers",
                column: "Phone",
                unique: true,
                filter: "([Phone] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_Phone_Filtered",
                table: "Customers",
                column: "Phone",
                unique: true,
                filter: "([Phone] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_TaxId_F",
                table: "Customers",
                column: "TaxId",
                unique: true,
                filter: "([TaxId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_TaxId_Filtered",
                table: "Customers",
                column: "TaxId",
                unique: true,
                filter: "([TaxId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountDetails_BinId",
                table: "CycleCountDetails",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountDetails_CountID",
                table: "CycleCountDetails",
                column: "CountId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountDetails_ProductID",
                table: "CycleCountDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_BinID",
                table: "CycleCounts",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_CountedBy",
                table: "CycleCounts",
                column: "CountedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_ProductID",
                table: "CycleCounts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_Status",
                table: "CycleCounts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_VerifiedBy",
                table: "CycleCounts",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCounts_WarehouseID",
                table: "CycleCounts",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_CurrentStageId",
                table: "DeliveryChallans",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_DispatchID",
                table: "DeliveryChallans",
                column: "DispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_SalesID",
                table: "DeliveryChallans",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_Status",
                table: "DeliveryChallans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_WorkflowInstanceID",
                table: "DeliveryChallans",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Delivery__BFB7BB8A6BBC174E",
                table: "DeliveryChallans",
                column: "ChallanNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BranchID",
                table: "Departments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyID",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UQ__Departme__6EA8896DB2A460DB",
                table: "Departments",
                column: "DepartmentCode",
                unique: true,
                filter: "[DepartmentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true,
                filter: "[DepartmentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Departments_DepartmentName",
                table: "Departments",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_DepartmentID",
                table: "Designations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UX_Designations_DesignationName",
                table: "Designations",
                column: "DesignationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchDetails_DispatchID",
                table: "DispatchDetails",
                column: "DispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchDetails_ProductID",
                table: "DispatchDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchDetails_SalesDetailId",
                table: "DispatchDetails",
                column: "SalesDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_CurrentStageId",
                table: "DispatchMasters",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_DispatchedBy",
                table: "DispatchMasters",
                column: "DispatchedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_SalesID",
                table: "DispatchMasters",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_Status",
                table: "DispatchMasters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_WarehouseID",
                table: "DispatchMasters",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchMasters_WorkflowInstanceID",
                table: "DispatchMasters",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Dispatch__434D95C77358C77D",
                table: "DispatchMasters",
                column: "DispatchNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendance_AttendanceDate",
                table: "EmployeeAttendance",
                column: "AttendanceDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendance_EmployeeID",
                table: "EmployeeAttendance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendance_ShiftId",
                table: "EmployeeAttendance",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_EmployeeID",
                table: "EmployeeBankAccounts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeave_ApprovedBy",
                table: "EmployeeLeave",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeave_EmployeeID",
                table: "EmployeeLeave",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeave_Status",
                table: "EmployeeLeave",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeave_WorkflowInstanceID",
                table: "EmployeeLeave",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_ApprovedBy",
                table: "EmployeeLeaves",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaves_EmployeeId",
                table: "EmployeeLeaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayroll_ApprovedBy",
                table: "EmployeePayroll",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayroll_EmployeeID",
                table: "EmployeePayroll",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayroll_SalaryID",
                table: "EmployeePayroll",
                column: "SalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayroll_Status",
                table: "EmployeePayroll",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayroll_WorkflowInstanceID",
                table: "EmployeePayroll",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollDetails_ComponentID",
                table: "EmployeePayrollDetails",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollDetails_EmployeeID",
                table: "EmployeePayrollDetails",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollDetails_PayrollID",
                table: "EmployeePayrollDetails",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchID",
                table: "Employees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentID",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationID",
                table: "Employees",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsActive",
                table: "Employees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ShiftId",
                table: "Employees",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserID",
                table: "Employees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WarehouseID",
                table: "Employees",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__1F642548965B7004",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Employees_UserId_Filtered",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "([UserId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId",
                table: "EmployeeSalaries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalary_EmployeeID",
                table: "EmployeeSalary",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalary_IsActive",
                table: "EmployeeSalary",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__05E7698AFFD78AC8",
                table: "EmployeeStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRN_CurrentStageID",
                table: "GRN",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_PurchaseID",
                table: "GRN",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_Status",
                table: "GRN",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_WarehouseID",
                table: "GRN",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_WorkflowInstanceID",
                table: "GRN",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__GRN__BC0E85A839D807E0",
                table: "GRN",
                column: "GrnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRNDetails_BinID",
                table: "GRNDetails",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNDetails_GRNID",
                table: "GRNDetails",
                column: "GrnId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNDetails_ProductID",
                table: "GRNDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNDetails_PurchaseDetailID",
                table: "GRNDetails",
                column: "PurchaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuation_ProductID",
                table: "InventoryValuation",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuation_ValuationDate",
                table: "InventoryValuation",
                column: "ValuationDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuation_WarehouseID",
                table: "InventoryValuation",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceID",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_ProductID",
                table: "InvoiceDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_SalesDetailId",
                table: "InvoiceDetails",
                column: "SalesDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CurrentStageId",
                table: "Invoices",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DueDate",
                table: "Invoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SalesID",
                table: "Invoices",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                table: "Invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_WorkflowInstanceID",
                table: "Invoices",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Invoices__D796B227918B33BD",
                table: "Invoices",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalVoucherDetails_AccountId",
                table: "JournalVoucherDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVoucherDetails_VoucherId",
                table: "JournalVoucherDetails",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_ApprovedBy",
                table: "JournalVouchers",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_CurrentStageId",
                table: "JournalVouchers",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_WorkflowInstanceId",
                table: "JournalVouchers",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__JournalV__3AD31D6F6B59EB14",
                table: "JournalVouchers",
                column: "VoucherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistory_UserId",
                table: "LoginHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulePermissions_ModuleId",
                table: "ModulePermissions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulePermissions_RoleId",
                table: "ModulePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Modules__EB27D4330AAF47C5",
                table: "Modules",
                column: "ModuleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TemplateId",
                table: "Notifications",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__Notifica__0FDB5081EADFB857",
                table: "NotificationTemplates",
                column: "TemplateCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_NotifTemplates_TemplateCode",
                table: "NotificationTemplates",
                column: "TemplateCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_AccountId",
                table: "PaymentVoucherDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_PaymentId",
                table: "PaymentVoucherDetails",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_ApprovedBy",
                table: "PaymentVouchers",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_BankAccountId",
                table: "PaymentVouchers",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_CurrentStageId",
                table: "PaymentVouchers",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_PurchaseId",
                table: "PaymentVouchers",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_SupplierId",
                table: "PaymentVouchers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVouchers_WorkflowInstanceId",
                table: "PaymentVouchers",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__PaymentV__9B5572663630B3C8",
                table: "PaymentVouchers",
                column: "PaymentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingDetails_BinID",
                table: "PickingDetails",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingDetails_PickingID",
                table: "PickingDetails",
                column: "PickingId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingDetails_ProductID",
                table: "PickingDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingDetails_SalesDetailID",
                table: "PickingDetails",
                column: "SalesDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingDetails_Status",
                table: "PickingDetails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_AssignedTo",
                table: "PickingLists",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_CurrentStageId",
                table: "PickingLists",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_PickedBy",
                table: "PickingLists",
                column: "PickedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_SalesID",
                table: "PickingLists",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_Status",
                table: "PickingLists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_VerifiedBy",
                table: "PickingLists",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_WarehouseID",
                table: "PickingLists",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_WorkflowInstanceID",
                table: "PickingLists",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__PickingL__A93357416FB2D8D4",
                table: "PickingLists",
                column: "PickingNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_ProductID",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_IsPrimary",
                table: "ProductImages",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductID",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricingHistory_ChangedBy",
                table: "ProductPricingHistory",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricingHistory_ProductID",
                table: "ProductPricingHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandID",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubCategoryID",
                table: "Products",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitID",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "UQ__Products__177800D3CB972393",
                table: "Products",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Products__CA1ECF0DABE3AF88",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_ProductID",
                table: "PurchaseDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_PurchaseID",
                table: "PurchaseDetails",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_CurrentStageID",
                table: "PurchaseMasters",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_OrderDate",
                table: "PurchaseMasters",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_Status",
                table: "PurchaseMasters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_SupplierID",
                table: "PurchaseMasters",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_WarehouseID",
                table: "PurchaseMasters",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMasters_WorkflowInstanceID",
                table: "PurchaseMasters",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Purchase__03153EF2963BFC6E",
                table: "PurchaseMasters",
                column: "PurchaseOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Purchase__05E7698AB9C93CE4",
                table: "PurchaseOrderStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnDetails_ProductID",
                table: "PurchaseReturnDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnDetails_PurchaseDetailId",
                table: "PurchaseReturnDetails",
                column: "PurchaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnDetails_ReturnID",
                table: "PurchaseReturnDetails",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_ApprovedBy",
                table: "PurchaseReturns",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_CurrentStageId",
                table: "PurchaseReturns",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_GRNID",
                table: "PurchaseReturns",
                column: "GrnId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_PurchaseID",
                table: "PurchaseReturns",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_Status",
                table: "PurchaseReturns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_WorkflowInstanceID",
                table: "PurchaseReturns",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Purchase__F445F15AAA28D1D0",
                table: "PurchaseReturns",
                column: "ReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Racks_ZoneID",
                table: "Racks",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "UQ__Racks__BAFE22FEC7250078",
                table: "Racks",
                column: "RackCode",
                unique: true,
                filter: "[RackCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_AccountId",
                table: "ReceiptVoucherDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_ReceiptId",
                table: "ReceiptVoucherDetails",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_ApprovedBy",
                table: "ReceiptVouchers",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_BankAccountId",
                table: "ReceiptVouchers",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_CurrentStageId",
                table: "ReceiptVouchers",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_CustomerId",
                table: "ReceiptVouchers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_SalesId",
                table: "ReceiptVouchers",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVouchers_WorkflowInstanceId",
                table: "ReceiptVouchers",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__ReceiptV__CC0B72A6C6E8DD7F",
                table: "ReceiptVouchers",
                column: "ReceiptNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__ReportDe__0EDCD614F13BC563",
                table: "ReportDefinitions",
                column: "ReportCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SalaryCo__898CFBF8EF4D5AFB",
                table: "SalaryComponents",
                column: "ComponentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesDetails_ProductID",
                table: "SalesDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDetails_SalesID",
                table: "SalesDetails",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_CurrentStageID",
                table: "SalesMasters",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_CustomerID",
                table: "SalesMasters",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_OrderDate",
                table: "SalesMasters",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_Status",
                table: "SalesMasters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_WarehouseID",
                table: "SalesMasters",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMasters_WorkflowInstanceID",
                table: "SalesMasters",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__SalesMas__B1403B8C781F1970",
                table: "SalesMasters",
                column: "SalesOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SalesOrd__05E7698A5CDCDD0B",
                table: "SalesOrderStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnDetails_InvoiceDetailId",
                table: "SalesReturnDetails",
                column: "InvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnDetails_ProductID",
                table: "SalesReturnDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnDetails_ReturnID",
                table: "SalesReturnDetails",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_ApprovedBy",
                table: "SalesReturns",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_CurrentStageId",
                table: "SalesReturns",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_InvoiceID",
                table: "SalesReturns",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_SalesID",
                table: "SalesReturns",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_Status",
                table: "SalesReturns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_WorkflowInstanceID",
                table: "SalesReturns",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__SalesRet__F445F15A263681E9",
                table: "SalesReturns",
                column: "ReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_CustomerId",
                table: "SerialNumbers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_GRNDetailID",
                table: "SerialNumbers",
                column: "GRNDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_ProductID",
                table: "SerialNumbers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_SalesDetailId",
                table: "SerialNumbers",
                column: "SalesDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_SerialNo",
                table: "SerialNumbers",
                column: "SerialNo");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNumbers_Status",
                table: "SerialNumbers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UQ__SerialNu__5E5A535EF9FBE859",
                table: "SerialNumbers",
                column: "SerialNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_AdjustmentID",
                table: "StockAdjustmentDetails",
                column: "AdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_BinId",
                table: "StockAdjustmentDetails",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_ProductID",
                table: "StockAdjustmentDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ApprovedBy",
                table: "StockAdjustments",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_BinID",
                table: "StockAdjustments",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_CurrentStageId",
                table: "StockAdjustments",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ProductID",
                table: "StockAdjustments",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_Status",
                table: "StockAdjustments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_WarehouseID",
                table: "StockAdjustments",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_WorkflowInstanceID",
                table: "StockAdjustments",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_BinID",
                table: "StockLedgers",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_ProductID",
                table: "StockLedgers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_ReferenceDocNo",
                table: "StockLedgers",
                column: "ReferenceDocNo");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_TransactionDate",
                table: "StockLedgers",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_TransactionType",
                table: "StockLedgers",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_WarehouseID",
                table: "StockLedgers",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_FromBinId",
                table: "StockTransferDetails",
                column: "FromBinId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_ProductID",
                table: "StockTransferDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_ToBinId",
                table: "StockTransferDetails",
                column: "ToBinId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_TransferID",
                table: "StockTransferDetails",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApprovedBy",
                table: "StockTransfers",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_CurrentStageId",
                table: "StockTransfers",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_FromWarehouseID",
                table: "StockTransfers",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ProductID",
                table: "StockTransfers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ReceivedBy",
                table: "StockTransfers",
                column: "ReceivedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_Status",
                table: "StockTransfers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToWarehouseID",
                table: "StockTransfers",
                column: "ToWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_WorkflowInstanceID",
                table: "StockTransfers",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__StockTra__9548BE62668F5E78",
                table: "StockTransfers",
                column: "TransferNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "UQ__SubCateg__E7EB0784FC45A639",
                table: "SubCategories",
                column: "SubCategoryCode",
                unique: true,
                filter: "[SubCategoryCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierContacts_SupplierID",
                table: "SupplierContacts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_Status",
                table: "SupplierQuotations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_SupplierID",
                table: "SupplierQuotations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_WorkflowInstanceID",
                table: "SupplierQuotations",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "UQ__Supplier__E192718177FA5213",
                table: "SupplierQuotations",
                column: "QuotationNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_IsActive",
                table: "Suppliers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_Suppliers_Code_F",
                table: "Suppliers",
                column: "SupplierCode",
                unique: true,
                filter: "([SupplierCode] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Suppliers_Email_F",
                table: "Suppliers",
                column: "Email",
                unique: true,
                filter: "([Email] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Suppliers_Name_F",
                table: "Suppliers",
                column: "SupplierName",
                unique: true,
                filter: "([SupplierName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Suppliers_Phone_F",
                table: "Suppliers",
                column: "Phone",
                unique: true,
                filter: "([Phone] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Suppliers_TaxId_F",
                table: "Suppliers",
                column: "TaxId",
                unique: true,
                filter: "([TaxId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UQ__SystemSe__01E719AD6DB88761",
                table: "SystemSettings",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SystemSettings_SettingKey",
                table: "SystemSettings",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxSettings_CompanyId",
                table: "TaxSettings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UQ__TaxSetti__12945A28B67D0A53",
                table: "TaxSettings",
                column: "TaxCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_BaseUnitId",
                table: "Units",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "UQ__Units__0665E6D9293372BF",
                table: "Units",
                column: "UnitCode",
                unique: true,
                filter: "[UnitCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Units_UnitCode",
                table: "Units",
                column: "UnitCode",
                unique: true,
                filter: "[UnitCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Units_UnitName",
                table: "Units",
                column: "UnitName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_WarehouseId",
                table: "Users",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__1F6425486D9C599E",
                table: "Users",
                column: "EmployeeCode",
                unique: true,
                filter: "[EmployeeCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105345D012C50",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__C9F28456DAEB93E2",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__UserSess__46BDD124A81ADCA2",
                table: "UserSessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_BranchID",
                table: "Warehouses",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_IsActive",
                table: "Warehouses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ__Warehous__1686A056BF7CD1D5",
                table: "Warehouses",
                column: "WarehouseCode",
                unique: true,
                filter: "[WarehouseCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Warehouses_WarehouseCode",
                table: "Warehouses",
                column: "WarehouseCode",
                unique: true,
                filter: "[WarehouseCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Warehouses_WarehouseName",
                table: "Warehouses",
                column: "WarehouseName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_RoleID",
                table: "WorkflowAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_StageID",
                table: "WorkflowAssignments",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_UserID",
                table: "WorkflowAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_WorkflowDefID",
                table: "WorkflowAssignments",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowConditions_StageFromID",
                table: "WorkflowConditions",
                column: "StageFromId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowConditions_StageToID",
                table: "WorkflowConditions",
                column: "StageToId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowConditions_WorkflowDefID",
                table: "WorkflowConditions",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDeadlines_DueDate",
                table: "WorkflowDeadlines",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDeadlines_EscalatedTo",
                table: "WorkflowDeadlines",
                column: "EscalatedTo");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDeadlines_StageID",
                table: "WorkflowDeadlines",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDeadlines_WorkflowInstID",
                table: "WorkflowDeadlines",
                column: "WorkflowInstId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_IsActive",
                table: "WorkflowDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_ModuleType",
                table: "WorkflowDefinitions",
                column: "ModuleType");

            migrationBuilder.CreateIndex(
                name: "UQ_WorkflowDefinition_ModuleType",
                table: "WorkflowDefinitions",
                column: "ModuleType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_WFDefs_WorkflowName",
                table: "WorkflowDefinitions",
                column: "WorkflowName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_ActionDate",
                table: "WorkflowInstanceLogs",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_ActionId",
                table: "WorkflowInstanceLogs",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_StageFromId",
                table: "WorkflowInstanceLogs",
                column: "StageFromId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_StageToId",
                table: "WorkflowInstanceLogs",
                column: "StageToId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_UserID",
                table: "WorkflowInstanceLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceLogs_WorkflowInstID",
                table: "WorkflowInstanceLogs",
                column: "WorkflowInstId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_CurrentStageID",
                table: "WorkflowInstances",
                column: "CurrentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_RecordID",
                table: "WorkflowInstances",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_RecordType",
                table: "WorkflowInstances",
                column: "RecordType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status",
                table: "WorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowDefID",
                table: "WorkflowInstances",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNotifications_IsRead",
                table: "WorkflowNotifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNotifications_UserID",
                table: "WorkflowNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowNotifications_WorkflowInstID",
                table: "WorkflowNotifications",
                column: "WorkflowInstId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStages_StageOrder",
                table: "WorkflowStages",
                column: "StageOrder");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStages_WorkflowDefID",
                table: "WorkflowStages",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "UX_WFStages_StageCode_DefId",
                table: "WorkflowStages",
                columns: new[] { "WorkflowDefId", "StageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_ActionID",
                table: "WorkflowTransitions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_StageFromID",
                table: "WorkflowTransitions",
                column: "StageFromId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_StageToID",
                table: "WorkflowTransitions",
                column: "StageToId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowDefID",
                table: "WorkflowTransitions",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_WarehouseID",
                table: "Zones",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "UQ__Zones__005DD0BC712843BA",
                table: "Zones",
                column: "ZoneCode",
                unique: true,
                filter: "[ZoneCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLedger");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BankReconciliation");

            migrationBuilder.DropTable(
                name: "BankTransactions");

            migrationBuilder.DropTable(
                name: "BatchTracking");

            migrationBuilder.DropTable(
                name: "CustomerComplaints");

            migrationBuilder.DropTable(
                name: "CustomerContacts");

            migrationBuilder.DropTable(
                name: "CustomerFeedback");

            migrationBuilder.DropTable(
                name: "CustomerInquiries");

            migrationBuilder.DropTable(
                name: "CycleCountDetails");

            migrationBuilder.DropTable(
                name: "DashboardWidgets");

            migrationBuilder.DropTable(
                name: "DataBackupLogs");

            migrationBuilder.DropTable(
                name: "DeliveryChallans");

            migrationBuilder.DropTable(
                name: "DispatchDetails");

            migrationBuilder.DropTable(
                name: "EmployeeAttendance");

            migrationBuilder.DropTable(
                name: "EmployeeBankAccounts");

            migrationBuilder.DropTable(
                name: "EmployeeLeave");

            migrationBuilder.DropTable(
                name: "EmployeeLeaves");

            migrationBuilder.DropTable(
                name: "EmployeePayrollDetails");

            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "EmployeeStatus");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "InventoryValuation");

            migrationBuilder.DropTable(
                name: "JournalVoucherDetails");

            migrationBuilder.DropTable(
                name: "LoginHistory");

            migrationBuilder.DropTable(
                name: "ModulePermissions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PaymentVoucherDetails");

            migrationBuilder.DropTable(
                name: "PickingDetails");

            migrationBuilder.DropTable(
                name: "ProductAttributes");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductPricingHistory");

            migrationBuilder.DropTable(
                name: "PurchaseOrderStatus");

            migrationBuilder.DropTable(
                name: "PurchaseReturnDetails");

            migrationBuilder.DropTable(
                name: "ReceiptVoucherDetails");

            migrationBuilder.DropTable(
                name: "ReportDefinitions");

            migrationBuilder.DropTable(
                name: "SalesOrderStatus");

            migrationBuilder.DropTable(
                name: "SalesReturnDetails");

            migrationBuilder.DropTable(
                name: "ScheduledReports");

            migrationBuilder.DropTable(
                name: "SerialNumbers");

            migrationBuilder.DropTable(
                name: "StockAdjustmentDetails");

            migrationBuilder.DropTable(
                name: "StockLedgers");

            migrationBuilder.DropTable(
                name: "StockTransferDetails");

            migrationBuilder.DropTable(
                name: "SupplierContacts");

            migrationBuilder.DropTable(
                name: "SupplierQuotations");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TaxSettings");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WorkflowAssignments");

            migrationBuilder.DropTable(
                name: "WorkflowConditions");

            migrationBuilder.DropTable(
                name: "WorkflowDeadlines");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceLogs");

            migrationBuilder.DropTable(
                name: "WorkflowNotifications");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "CycleCounts");

            migrationBuilder.DropTable(
                name: "DispatchMasters");

            migrationBuilder.DropTable(
                name: "EmployeePayroll");

            migrationBuilder.DropTable(
                name: "SalaryComponents");

            migrationBuilder.DropTable(
                name: "JournalVouchers");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "PaymentVouchers");

            migrationBuilder.DropTable(
                name: "PickingLists");

            migrationBuilder.DropTable(
                name: "PurchaseReturns");

            migrationBuilder.DropTable(
                name: "ChartOfAccounts");

            migrationBuilder.DropTable(
                name: "ReceiptVouchers");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "SalesReturns");

            migrationBuilder.DropTable(
                name: "GRNDetails");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "WorkflowActions");

            migrationBuilder.DropTable(
                name: "EmployeeSalary");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "SalesDetails");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "GRN");

            migrationBuilder.DropTable(
                name: "PurchaseDetails");

            migrationBuilder.DropTable(
                name: "Bins");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "SalesMasters");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "PurchaseMasters");

            migrationBuilder.DropTable(
                name: "Racks");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "WorkflowStages");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
