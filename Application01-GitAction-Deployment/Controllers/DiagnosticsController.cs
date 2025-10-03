using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application01_GitAction_Deployment.Data;

namespace Application01_GitAction_Deployment.Controllers
{
    [Route("diag")]
    public class DiagnosticsController : Controller
    {
        private readonly AppDbContext _db;
        public DiagnosticsController(AppDbContext db) => _db = db; // or any other suitable and meaningful name

        // GET /diag/db
        [HttpGet("db")]
        public async Task<IActionResult> Db()
        {
            try
            {
                var can = await _db.Database.CanConnectAsync();
                var conn = _db.Database.GetDbConnection();
                await conn.OpenAsync();

                // نفذ الهجرة يدويًا هنا أيضاً لضمان التطبيق
                await _db.Database.MigrateAsync();

                // تحقق من وجود جدول التاريخ
                var hasHistory = await _db.Database.ExecuteSqlRawAsync(
                    "IF OBJECT_ID('__EFMigrationsHistory','U') IS NULL SELECT 0 ELSE SELECT 1") > 0;

                // جرّب قراءة عدد الصفوف إن وجد جدول Employees
                int? rows = null;
                try
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT COUNT(*) FROM Employees";
                    var r = await cmd.ExecuteScalarAsync();
                    rows = Convert.ToInt32(r);
                }
                catch { /* الجدول قد لا يكون موجودًا بعد */ }

                return Ok(new
                {
                    Connected = can,
                    DataSource = conn.DataSource,
                    Database = conn.Database,
                    MigrationsApplied = hasHistory,
                    EmployeesRows = rows
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
