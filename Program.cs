using DevExpress.Data.Browsing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DevExpress.XtraCharts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

using LAGem_POPortal.Code;
using LAGem_POPortal.Authentication;
using LAGem_POPortal.Data;
using LAGem_POPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// -- =========================================================================

//// Logging: using Microsoft.AspNetCore.Identity custom
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 5;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireNonAlphanumeric = false;
//    options.SignIn.RequireConfirmedEmail = false;
//})
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<DataContext>();

// Logging: using custom
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
});

// For CustomAuthenticationStateProvider process
builder.Services.AddAuthenticationCore();

// -- =========================================================================

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
//builder.Services.AddDbContextFactory<NorthwindContext>((sp, options) =>
//{
//    var env = sp.GetRequiredService<IWebHostEnvironment>();
//    var dbPath = Path.Combine(env.ContentRootPath, "Northwind.mdf");
//    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=" + dbPath);
//});

builder.Services.AddDbContextFactory<DbContext>((sp, options) =>
{
    var sqlDefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(sqlDefaultConnection);
});

//var sqlDefaultConnection = Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(sqlDefaultConnection));
//var sqlLAGemConnection = Configuration.GetConnectionString("LAGemConnection");
//builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(sqlLAGemConnection));

// -- =========================================================================

//// For CustomAuthenticationStateProvider process
//builder.Services.AddAuthenticationCore();

// For CustomAuthenticationStateProvider process
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddSingleton<UserAccountService>();

//// Logging: using Microsoft.AspNetCore.Identity custom
//builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

// Logging: using custom
builder.Services.AddSingleton<CustomAuthService>();

// -- =========================================================================

// initialize XPO DAL
var store = XpoDefault.GetConnectionProvider(ConnectionHelper.ConnectionString, AutoCreateOption.SchemaAlreadyExists);
//var store = XpoDefault.GetConnectionProvider(sqlConnection, AutoCreateOption.SchemaAlreadyExists);

// Initialize the XPO dictionary.
DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
dict.GetDataStoreSchema(ConnectionHelper.GetPersistentTypes());

DevExpress.Xpo.XpoDefault.DataLayer = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);
DevExpress.Xpo.XpoDefault.Session = null;

// -- =========================================================================

//builder.Services.AddSingleton<GlobalData>();
builder.Services.AddScoped<GlobalData>();

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
//app.UseSession(); // from https://github.com/DevExpress-Examples/DashboardDifferentUserDataAspNetCore/blob/23.2.2%2B/CS/Startup.cs
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// -- =========================================================================