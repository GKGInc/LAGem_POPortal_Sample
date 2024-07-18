//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using DevExpress.DashboardAspNetCore;
using MasterDetail.Data;
using MasterDetail.Models;

namespace MasterDetail.Code
{
    public static class DashboardUtils
    {
        public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider)
        {
            DashboardConfigurator configurator = new DashboardConfigurator();
            configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
            configurator.SetDashboardStorage(dashboardFileStorage);

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            //// Registers an SQL data source.
            //DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            //sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            //SelectQuery query = SelectQueryFluentBuilder
            //    .AddTable("Categories").SelectAllColumnsFromTable()
            //    .Join("Products", "CategoryID").SelectAllColumnsFromTable()
            //    .Build("Products_Categories");
            //sqlDataSource.Queries.Add(query);
            //dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            //// Registers an SQL data source.
            //DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "DefaultConnection");
            //sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            //dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            //// Registers an Object data source.
            //DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            //objDataSource.DataId = "Object Data Source Data Id";
            //dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

            //// Registers an Excel data source.
            //DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            //excelDataSource.ConnectionName = "Excel Data Source Connection Name";
            //excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
            //dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

            //// Create a sample JSON data source
            //DashboardJsonDataSource jsonDataSourceUrl = new DashboardJsonDataSource("JSON Data Source (URL)");
            //jsonDataSourceUrl.ConnectionName = "jsonCustomers";
            //jsonDataSourceUrl.RootElement = "Customers";
            //dataSourceStorage.RegisterDataSource("jsonDataSourceUrl", jsonDataSourceUrl.SaveToXml());


            // Registers an SQL data source.
            DashboardObjectDataSource objDataSource1 = new DashboardObjectDataSource("DepartmentQueueProcessesView_all", typeof(DepartmentQueueProcessesView));
            dataSourceStorage.RegisterDataSource("ediAttendantDataObjectDataSource", objDataSource1.SaveToXml());
            DashboardObjectDataSource objDataSource2 = new DashboardObjectDataSource("DepartmentQueueProcessesView_thisyear", typeof(DepartmentQueueProcessesView));
            dataSourceStorage.RegisterDataSource("ediAttendantDataObjectDataSource", objDataSource2.SaveToXml());

            configurator.SetDataSourceStorage(dataSourceStorage);

            configurator.CustomParameters += Configurator_CustomParameters;

            //configurator.DataLoading += (s, e) => {
            //    if (e.DataId == "Object Data Source Data Id")
            //    {
            //        e.Data = Invoices.CreateData(); // temp data
            //    }
            //    if (e.DataId == "IVAN")
            //    {
            //        e.Data = Invoices.CreateSQLData(); // temp data
            //    }
            //};
            configurator.DataLoading += Configurator_DataLoading;

            configurator.ConfigureDataConnection += (s, e) =>
            {
                //if (e.ConnectionName == "Excel Data Source Connection Name")
                //{
                //    ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
                //    excelParameters.FileName = fileProvider.GetFileInfo("Data/Sales.xlsx").PhysicalPath;
                //}

                Configurator_ConfigureDataConnection(s, e, fileProvider);
            };

            configurator.CustomFilterExpression += Configurator_CustomFilterExpression;
            configurator.ValidateCustomSqlQuery += Configurator_ValidateCustomSqlQuery;

            return configurator;
        }

        //public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider, IHttpContextAccessor contextAccessor)
        //{
        //    string promoCode = contextAccessor.HttpContext.Request.Headers["PromoCode"];

        //    DashboardConfigurator configurator = new DashboardConfigurator();
        //    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

        //    DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
        //    configurator.SetDashboardStorage(dashboardFileStorage);

        //    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

        //    // Registers an SQL data source.
        //    DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("ObjectDataSource", typeof(EdiAttendantData));
        //    dataSourceStorage.RegisterDataSource("objectDataSource", objDataSource.SaveToXml());

        //    configurator.SetDataSourceStorage(dataSourceStorage);

        //    configurator.DataLoading += (s, e) =>
        //    {
        //        SqlData sqlData = new SqlData();
        //        List<EdiAttendantData> data = sqlData.GetEdiAttendantData(promoCode).Result;
        //        e.Data = data;
        //    };

        //    return configurator;
        //}

        private static void Configurator_ValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryWebEventArgs e)
        {

        }

        private static void Configurator_CustomFilterExpression(object sender, CustomFilterExpressionWebEventArgs e)
        {
            // nothing
        }

        private static void Configurator_CustomParameters(object sender, CustomParametersWebEventArgs e)
        {
            //if (e.DashboardId == "dashboard2")
            //{

            //}

            //var param = e.Parameters.FirstOrDefault(p => p.Name == "dashboard2");
            //if (param != null)
            //{
            //    param.Value = "2800";
            //}

            //if (e.Parameters.Count > 0)
            //{

            //}
            //else
            //{
            //    e.Parameters.Add(new DashboardParameter("promoCode", typeof(string), "2800"));
            //}
        }

        private async static void Configurator_DataLoading(object sender, DataLoadingWebEventArgs e)
        {
            //if (e.DashboardId == "IVAN" || e.DataId == "IVAN")
            //{
            //    e.Data = Invoices.CreateData(); // temp data
            //}
            //if (e.DataId == "Object Data Source Data Id")
            //{
            //    e.Data = Invoices.CreateData(); // temp data
            //}

            //string user = contextAccessor.HttpContext.Request.Headers["User"];
            //string promoCode = contextAccessor.HttpContext.Request.Headers["PromoCode"];

            //string user = "";
            //string promoCode = "2800";

            SqlData sqlData = new SqlData();
            //e.Data = await sqlData.GetDashboardData(e.DashboardId, user, promoCode, e.DataSourceName, e.DataSourceComponentName);
            e.Data = await sqlData.GetDashboardData(e.DashboardId);
        }
        private static void Configurator_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e, IFileProvider fileProvider)
        {
            //if (e.ConnectionName == "Excel Data Source Connection Name")
            //{
            //    ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
            //    excelParameters.FileName = fileProvider.GetFileInfo("Data/Sales.xlsx").PhysicalPath;
            //}
            //if (e.ConnectionName == "jsonCustomers")
            //{
            //    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
            //    jsonParams.JsonSource = new UriJsonSource(
            //    new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"));
            //    e.ConnectionParameters = jsonParams;
            //}
        }

    }
}