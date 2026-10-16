using AutoMapper;
using ERPWMS.Application.Common;
using ERPWMS.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Infrastructure;
using ERPWMS.Infrastructure.Repositories;
using ERPWMS.Infrastructure.Repositories.Inventory;
using ERPWMS.Infrastructure.Repositories.Sales;
using ERPWMS.Infrastructure.Repositories.Workflow;
using ERPWMS.Infrastructure.Services;
using ERPWMS.Infrastructure.Services.Audit;
using ERPWMS.Infrastructure.Services.Identity;
using ERPWMS.Infrastructure.Services.Notification;
using ERPWMS.Infrastructure.Services.Reports;
using ERPWMS.Infrastructure.Services.Workflow;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
// === ERPWMS DataProtection (auto-added) ===
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Keys\ERPWMS"))
    .SetApplicationName("ERPWMS");
// === end ===

// ============================================================
// 1. DATABASE
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ============================================================
// 2. AUTHENTICATION (Custom Cookie Auth - SECURE)
// ============================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;

        // Cookie Security Settings
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "ERPWMS.Auth";
        options.Cookie.IsEssential = true;

        // Session validation
        options.Events.OnValidatePrincipal = async context =>
        {
            if (context.Principal?.Identity?.IsAuthenticated == true)
            {
                var lastActivity = context.Properties?.Items["LastActivity"];
                if (!string.IsNullOrEmpty(lastActivity))
                {
                    if (DateTime.TryParse(lastActivity, out var last))
                    {
                        if ((DateTime.UtcNow - last).TotalMinutes > 60)
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }
                }
            }
        };
    });

// ============================================================
// 3. AUTHORIZATION POLICIES
// ============================================================
builder.Services.AddAuthorization(options =>
{
    // Company
    options.AddPolicy("CompanyView", p => p.RequireClaim("Permission", "CompanyView"));
    options.AddPolicy("CompanyCreate", p => p.RequireClaim("Permission", "CompanyCreate"));
    options.AddPolicy("CompanyUpdate", p => p.RequireClaim("Permission", "CompanyUpdate"));
    options.AddPolicy("CompanyDelete", p => p.RequireClaim("Permission", "CompanyDelete"));

    // Branch
    options.AddPolicy("BranchView", p => p.RequireClaim("Permission", "BranchView"));
    options.AddPolicy("BranchCreate", p => p.RequireClaim("Permission", "BranchCreate"));
    options.AddPolicy("BranchUpdate", p => p.RequireClaim("Permission", "BranchUpdate"));
    options.AddPolicy("BranchDelete", p => p.RequireClaim("Permission", "BranchDelete"));
    options.AddPolicy("BranchActivate", p => p.RequireClaim("Permission", "BranchUpdate"));
    options.AddPolicy("BranchDeactivate", p => p.RequireClaim("Permission", "BranchUpdate"));

    // Department
    options.AddPolicy("DepartmentView", p => p.RequireClaim("Permission", "DepartmentView"));
    options.AddPolicy("DepartmentCreate", p => p.RequireClaim("Permission", "DepartmentCreate"));
    options.AddPolicy("DepartmentUpdate", p => p.RequireClaim("Permission", "DepartmentUpdate"));
    options.AddPolicy("DepartmentDelete", p => p.RequireClaim("Permission", "DepartmentDelete"));

    // Designation
    options.AddPolicy("DesignationView", p => p.RequireClaim("Permission", "DesignationView"));
    options.AddPolicy("DesignationCreate", p => p.RequireClaim("Permission", "DesignationCreate"));
    options.AddPolicy("DesignationUpdate", p => p.RequireClaim("Permission", "DesignationUpdate"));
    options.AddPolicy("DesignationDelete", p => p.RequireClaim("Permission", "DesignationDelete"));

    // Warehouse
    options.AddPolicy("WarehouseView", p => p.RequireClaim("Permission", "WarehouseView"));
    options.AddPolicy("WarehouseCreate", p => p.RequireClaim("Permission", "WarehouseCreate"));
    options.AddPolicy("WarehouseUpdate", p => p.RequireClaim("Permission", "WarehouseUpdate"));
    options.AddPolicy("WarehouseDelete", p => p.RequireClaim("Permission", "WarehouseDelete"));

    // Employee
    options.AddPolicy("EmployeeView", p => p.RequireClaim("Permission", "EmployeeView"));
    options.AddPolicy("EmployeeCreate", p => p.RequireClaim("Permission", "EmployeeCreate"));
    options.AddPolicy("EmployeeUpdate", p => p.RequireClaim("Permission", "EmployeeUpdate"));
    options.AddPolicy("EmployeeDelete", p => p.RequireClaim("Permission", "EmployeeDelete"));

    // System Admin
    options.AddPolicy("SystemAdmin.View", p => p.RequireClaim("Permission", "SystemAdmin.View"));
    options.AddPolicy("SystemAdmin.Manage", p => p.RequireClaim("Permission", "SystemAdmin.Manage"));
});

// ============================================================
// 4. MVC + Razor Pages
// ============================================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ============================================================
// 5. HTTP CONTEXT
// ============================================================
builder.Services.AddHttpContextAccessor();

// ============================================================
// 6. MEDIATR (Commands/Queries ke liye)
// ============================================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ERPWMS.Application.Common.ICurrentUserService).Assembly));

// ============================================================
// 7. AUTOMAPPER
// ============================================================
builder.Services.AddAutoMapper(cfg => { }, typeof(ERPWMS.Application.Common.ICurrentUserService).Assembly);

// ============================================================
// 8. COMMON SERVICES
// ============================================================
builder.Services.AddScoped<ERPWMS.Application.Common.ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ERPWMS.Domain.Services.ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<ERPWMS.Application.Common.ILoggingService, LoggingService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<ERPWMS.Application.Common.IValidationService, ValidationService>();
builder.Services.AddScoped<ERPWMS.Application.Common.IPaginationService, PaginationService>();

// ============================================================
// 9. DOMAIN SERVICES (Business Logic)
// ============================================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IEmployeeAuthorizationService, EmployeeAuthorizationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
builder.Services.AddScoped<IInventoryAdjustmentService, InventoryAdjustmentService>();
builder.Services.AddScoped<IInventoryCycleCountService, InventoryCycleCountService>();
builder.Services.AddScoped<IInventoryTransferService, InventoryTransferService>();
builder.Services.AddScoped<IPickingService, PickingService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ISalesReturnService, SalesReturnService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IDispatchService, DispatchService>();
builder.Services.AddScoped<IPurchaseMasterService, PurchaseMasterService>();
builder.Services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
builder.Services.AddScoped<IGrnService, GrnService>();
builder.Services.AddScoped<IAttendanceDomainService, AttendanceDomainService>();
builder.Services.AddScoped<ILeaveDomainService, LeaveDomainService>();
builder.Services.AddScoped<IPayrollDomainService, PayrollDomainService>();
builder.Services.AddScoped<IEmployeeDomainService, EmployeeDomainService>();
builder.Services.AddScoped<IDepartmentDomainService, DepartmentDomainService>();
builder.Services.AddScoped<IDesignationDomainService, DesignationDomainService>();
builder.Services.AddScoped<IBranchDomainService, BranchDomainService>();
builder.Services.AddScoped<ICompanyDomainService, CompanyDomainService>();
builder.Services.AddScoped<IWarehouseDomainService, WarehouseDomainService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();
builder.Services.AddScoped<IWorkflowDefinitionService, WorkflowDefinitionService>();
builder.Services.AddScoped<IWorkflowInstanceService, WorkflowInstanceService>();
builder.Services.AddScoped<IWorkflowStageService, WorkflowStageService>();
builder.Services.AddScoped<IWorkflowTransitionService, WorkflowTransitionService>();
builder.Services.AddScoped<IWorkflowAssignmentService, WorkflowAssignmentService>();
builder.Services.AddScoped<IWorkflowActionService, WorkflowActionService>();
builder.Services.AddScoped<IWorkflowConditionService, WorkflowConditionService>();
builder.Services.AddScoped<IWorkflowDeadlineService, WorkflowDeadlineService>();
builder.Services.AddScoped<IWorkflowDashboardService, WorkflowDashboardService>();
builder.Services.AddScoped<IWorkflowInstanceLogService, WorkflowInstanceLogService>();

// ============================================================
// 10. BUSINESS RULES
// ============================================================
builder.Services.AddScoped<IBankAccountBusinessRules, BankAccountBusinessRules>();
builder.Services.AddScoped<IChartOfAccountBusinessRules, ChartOfAccountBusinessRules>();
builder.Services.AddScoped<ITaxSettingBusinessRules, TaxSettingBusinessRules>();

// ============================================================
// 11. REPOSITORIES (Domain.Interfaces)
// ============================================================
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeSalaryRepository, EmployeeSalaryRepository>();
builder.Services.AddScoped<IEmployeeBankAccountRepository, EmployeeBankAccountRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
builder.Services.AddScoped<ITaxSettingRepository, TaxSettingRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IPayrollSettingRepository, PayrollSettingRepository>();
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<IWeeklyOffRepository, WeeklyOffRepository>();
builder.Services.AddScoped<IAttendanceProcessor, ERPWMS.Infrastructure.Services.AttendanceProcessor>();
builder.Services.AddScoped<ILatePolicyEngine, ERPWMS.Infrastructure.Services.LatePolicyEngine>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.IAbsenceCalculator, ERPWMS.Infrastructure.Services.AbsenceCalculator>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.IOvertimeCalculator, ERPWMS.Infrastructure.Services.OvertimeCalculator>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.ILoanRecoveryService, ERPWMS.Infrastructure.Services.LoanRecoveryService>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.IPayrollCalculationEngine, ERPWMS.Infrastructure.Services.PayrollCalculationEngine>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.ISalaryStructureService, ERPWMS.Infrastructure.Services.SalaryStructureService>();
builder.Services.AddScoped<ERPWMS.Infrastructure.Services.ILeaveBalanceService, ERPWMS.Infrastructure.Services.LeaveBalanceService>();
builder.Services.AddScoped<IHalfDayCalculator, ERPWMS.Infrastructure.Services.HalfDayCalculator>();
builder.Services.AddScoped<IBatchTrackingRepository, BatchTrackingRepository>();
builder.Services.AddScoped<ISerialNumberRepository, SerialNumberRepository>();
builder.Services.AddScoped<IStockLedgerRepository, StockLedgerRepository>();
builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();
builder.Services.AddScoped<IStockTransferDetailRepository, StockTransferDetailRepository>();
builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
builder.Services.AddScoped<IStockAdjustmentDetailRepository, StockAdjustmentDetailRepository>();
builder.Services.AddScoped<ICycleCountRepository, CycleCountRepository>();
builder.Services.AddScoped<ICycleCountDetailRepository, CycleCountDetailRepository>();
builder.Services.AddScoped<IPurchaseMasterRepository, PurchaseMasterRepository>();
builder.Services.AddScoped<IPurchaseDetailRepository, PurchaseDetailRepository>();
builder.Services.AddScoped<IPurchaseReturnRepository, PurchaseReturnRepository>();
builder.Services.AddScoped<IPurchaseReturnDetailRepository, PurchaseReturnDetailRepository>();
builder.Services.AddScoped<IGrnRepository, GrnRepository>();
builder.Services.AddScoped<IGrndetailRepository, GrndetailRepository>();
builder.Services.AddScoped<ISalesMasterRepository, SalesMasterRepository>();
builder.Services.AddScoped<ISalesDetailRepository, SalesDetailRepository>();
builder.Services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();
builder.Services.AddScoped<ISalesReturnDetailRepository, SalesReturnDetailRepository>();
builder.Services.AddScoped<ISalesOrderStatusRepository, SalesOrderStatusRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
builder.Services.AddScoped<IPickingListRepository, PickingListRepository>();
builder.Services.AddScoped<IPickingDetailRepository, PickingDetailRepository>();
builder.Services.AddScoped<IDispatchMasterRepository, DispatchMasterRepository>();
builder.Services.AddScoped<IDispatchDetailRepository, DispatchDetailRepository>();
builder.Services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
builder.Services.AddScoped<IWorkflowStageRepository, WorkflowStageRepository>();
builder.Services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
builder.Services.AddScoped<IWorkflowTransitionRepository, WorkflowTransitionRepository>();
builder.Services.AddScoped<IWorkflowAssignmentRepository, WorkflowAssignmentRepository>();
builder.Services.AddScoped<IWorkflowConditionRepository, WorkflowConditionRepository>();
builder.Services.AddScoped<IWorkflowDeadlineRepository, WorkflowDeadlineRepository>();
builder.Services.AddScoped<IWorkflowInstanceLogRepository, WorkflowInstanceLogRepository>();

// ============================================================
// 12. BUILD
// ============================================================
var app = builder.Build();

// ============================================================
// 13. MIDDLEWARE PIPELINE
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


