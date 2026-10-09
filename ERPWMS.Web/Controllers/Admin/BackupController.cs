using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Controllers.Admin
{
    [Authorize]
    public class BackupController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        private static string BackupFolder => Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Backups");

        public BackupController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var backups = GetBackupList();
            ViewBag.DatabaseName = GetDatabaseName();
            return View(backups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                if (!Directory.Exists(BackupFolder))
                    Directory.CreateDirectory(BackupFolder);

                var dbName = GetDatabaseName();
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{dbName}_{timestamp}.bak";
                var filePath = Path.Combine(BackupFolder, fileName);

                var sql = "BACKUP DATABASE [" + dbName + "] TO DISK = @path WITH FORMAT, INIT, NAME = @name, SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 600;
                        cmd.Parameters.AddWithValue("@path", filePath);
                        cmd.Parameters.AddWithValue("@name", dbName + " Full Backup");
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                TempData["Success"] = "Backup created: " + fileName;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Backup failed: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Download(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequest("File name required");

                fileName = Path.GetFileName(fileName);
                var fullPath = Path.Combine(BackupFolder, fileName);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound("Backup file not found");

                var bytes = System.IO.File.ReadAllBytes(fullPath);
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Download failed: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    TempData["Error"] = "File name required";
                    return RedirectToAction(nameof(Index));
                }

                fileName = Path.GetFileName(fileName);
                var fullPath = Path.Combine(BackupFolder, fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    TempData["Success"] = "Deleted: " + fileName;
                }
                else
                {
                    TempData["Error"] = "Backup file not found";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete failed: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetStatistics()
        {
            var list = GetBackupList();
            return Json(new
            {
                total = list.Count,
                totalSizeMB = Math.Round(list.Sum(b => b.SizeMB), 2),
                latest = list.FirstOrDefault()?.DateFormatted ?? "-"
            });
        }

        private List<BackupFileInfo> GetBackupList()
        {
            var list = new List<BackupFileInfo>();
            try
            {
                if (!Directory.Exists(BackupFolder)) return list;

                var files = Directory.GetFiles(BackupFolder, "*.bak");
                foreach (var f in files)
                {
                    var fi = new FileInfo(f);
                    list.Add(new BackupFileInfo
                    {
                        FileName = fi.Name,
                        FullPath = fi.FullName,
                        SizeMB = Math.Round(fi.Length / 1024.0 / 1024.0, 2),
                        CreatedDate = fi.CreationTime,
                        DateFormatted = fi.CreationTime.ToString("dd MMM yyyy HH:mm")
                    });
                }
                list = list.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch { }
            return list;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection") ?? "";
        }

        private string GetDatabaseName()
        {
            try
            {
                var cs = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(cs);
                return builder.InitialCatalog ?? "EnterpriseWMS";
            }
            catch
            {
                return _context.Database.GetDbConnection().Database ?? "EnterpriseWMS";
            }
        }
    }

    public class BackupFileInfo
    {
        public string FileName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public double SizeMB { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DateFormatted { get; set; } = "";
    }
}
