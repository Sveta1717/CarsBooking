using CarBooking.Data;
using CarBooking.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHashService, Md5HashService>();

// ��������� Data Context
builder.Services.AddDbContext<DataContext>(options =>
options.UseMySql(
    builder.Configuration.GetConnectionString("PlanetDb"),
    new MySqlServerVersion(new Version(8, 0, 23)),
    serverOptions =>
         serverOptions
         .MigrationsHistoryTable(
             tableName: HistoryRepository.DefaultTableName,
             schema: "CarBooking")
         .SchemaBehavior(MySqlSchemaBehavior.Translate,
         (schema, table) => $"{schema}_{table}")
    ));

#region https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

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

app.UseAuthorization();

app.UseSession();
app.UseSessionAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
