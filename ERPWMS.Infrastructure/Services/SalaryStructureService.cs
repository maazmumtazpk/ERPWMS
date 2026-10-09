using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public interface ISalaryStructureService
    {
        Task<List<SalaryStructure>> GetAllAsync();
        Task<SalaryStructure?> GetByIdAsync(int id);
        Task<int> CreateAsync(SalaryStructure s, int userId);
        Task<bool> UpdateAsync(SalaryStructure s, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

    public class SalaryStructureService : ISalaryStructureService
    {
        private readonly AppDbContext _context;
        public SalaryStructureService(AppDbContext context) { _context = context; }

        public async Task<List<SalaryStructure>> GetAllAsync()
            => await _context.SalaryStructures.Where(s => s.IsDeleted != true).OrderBy(s => s.StructureName).ToListAsync();

        public async Task<SalaryStructure?> GetByIdAsync(int id)
            => await _context.SalaryStructures.Include(s => s.Components).FirstOrDefaultAsync(s => s.StructureId == id);

        public async Task<int> CreateAsync(SalaryStructure s, int userId)
        {
            s.CreatedBy = userId.ToString();
            s.CreatedDate = DateTime.Now;
            s.IsDeleted = false;
            _context.SalaryStructures.Add(s);
            await _context.SaveChangesAsync();
            return s.StructureId;
        }

        public async Task<bool> UpdateAsync(SalaryStructure s, int userId)
        {
            var e = await _context.SalaryStructures.FindAsync(s.StructureId);
            if (e == null) return false;
            e.StructureCode = s.StructureCode;
            e.StructureName = s.StructureName;
            e.Grade = s.Grade;
            e.Description = s.Description;
            e.MinSalary = s.MinSalary;
            e.MaxSalary = s.MaxSalary;
            e.IsActive = s.IsActive;
            e.UpdatedBy = userId.ToString();
            e.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var e = await _context.SalaryStructures.FindAsync(id);
            if (e == null) return false;
            e.IsDeleted = true; e.IsActive = false;
            e.UpdatedBy = userId.ToString(); e.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
