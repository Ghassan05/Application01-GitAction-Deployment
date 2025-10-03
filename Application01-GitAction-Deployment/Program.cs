using System.Data.Common;
using Application01_GitAction_Deployment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application01_GitAction_Deployment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // EF Core
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Apply EF migrations with detailed logging
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    DbConnection conn = db.Database.GetDbConnection();
                    conn.Open();

                    logger.LogInformation("SQL connected. DataSource={DataSource}, Database={Database}, ServerVersion={ServerVersion}",
                        conn.DataSource, conn.Database, conn.ServerVersion);

                    logger.LogInformation("Applying EF Core migrations...");
                    db.Database.Migrate();
                    logger.LogInformation("EF Core migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    var st = ex.StackTrace ?? string.Empty;
                    logger.LogError(ex, "EF migration failed at startup. Error={Message}\n{StackTrace}", msg, st);
                }
            }

            app.Run();
        }
    }
}
