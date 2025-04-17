using Microsoft.EntityFrameworkCore;
using PPPK_Enver_Besic.Models;
using PPPK_Enver_Besic.Repositories;
using System;

namespace PPPK_Enver_Besic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("Database");
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString)
                       .UseLazyLoadingProxies();
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();

            // Ako koristite RepositoryFactory, registrirajte i njega:
            builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
