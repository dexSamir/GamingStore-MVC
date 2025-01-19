using Gaming.Core.Entities;
using Gaming.DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gaming.BL.Extension;

namespace Gaming.MVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

        builder.Services.AddIdentity<User, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = true; 
            opt.Password.RequiredLength = 3;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Lockout.MaxFailedAccessAttempts = 5;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
        }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>(); 

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseUserSeed();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "register",
            pattern: "register",
            defaults: new { Controller = "Account" , Action = "Register" });

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"); 


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

