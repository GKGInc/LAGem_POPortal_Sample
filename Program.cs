using DevExpress.Data.Browsing;
using MasterDetail.Models;
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
using MasterDetail.Code;

var builder = WebApplication.CreateBuilder(args);

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

// initialize XPO DAL
var store = XpoDefault.GetConnectionProvider(ConnectionHelper.ConnectionString, AutoCreateOption.SchemaAlreadyExists);
//var store = XpoDefault.GetConnectionProvider(sqlConnection, AutoCreateOption.SchemaAlreadyExists);

// Initialize the XPO dictionary.
DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
dict.GetDataStoreSchema(ConnectionHelper.GetPersistentTypes());

DevExpress.Xpo.XpoDefault.DataLayer = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);
DevExpress.Xpo.XpoDefault.Session = null;

// -- =========================================================================

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


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();