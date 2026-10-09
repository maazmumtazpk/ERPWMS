using Microsoft.EntityFrameworkCore;
using ERPWMS.Database.Models;

namespace ERPWMS.Database.Models;

public partial class AppDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // PayrollRun → PayrollPeriod
        modelBuilder.Entity<PayrollRun>()
            .HasOne(r => r.Period).WithMany()
            .HasForeignKey(r => r.PeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        // PayrollRegister → PayrollRun
        modelBuilder.Entity<PayrollRegister>()
            .HasOne(r => r.Run).WithMany(x => x.Registers)
            .HasForeignKey(r => r.RunId)
            .OnDelete(DeleteBehavior.Restrict);

        // PayrollRegister → Employee
        modelBuilder.Entity<PayrollRegister>()
            .HasOne(r => r.Employee).WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payslip → PayrollRegister
        modelBuilder.Entity<Payslip>()
            .HasOne(p => p.Register).WithMany()
            .HasForeignKey(p => p.RegisterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payslip → Employee
        modelBuilder.Entity<Payslip>()
            .HasOne(p => p.Employee).WithMany()
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // PayrollApprovalHistory → PayrollRun
        modelBuilder.Entity<PayrollApprovalHistory>()
            .HasOne(h => h.Run).WithMany()
            .HasForeignKey(h => h.RunId)
            .OnDelete(DeleteBehavior.Restrict);

        // Incident → Employee
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Employee).WithMany()
            .HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // DisciplinaryAction → Incident
        modelBuilder.Entity<DisciplinaryAction>()
            .HasOne(a => a.Incident).WithMany()
            .HasForeignKey(a => a.IncidentId)
            .OnDelete(DeleteBehavior.Restrict);

        // DisciplinaryAction → Employee
        modelBuilder.Entity<DisciplinaryAction>()
            .HasOne(a => a.Employee).WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // PayrollPayment → Employee
        modelBuilder.Entity<PayrollPayment>()
            .HasOne(p => p.Employee).WithMany()
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
