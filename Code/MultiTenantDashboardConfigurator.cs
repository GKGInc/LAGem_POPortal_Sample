using MasterDetail.Data;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.Data.Filtering;
//using DevExpress.DataAccess;
//using DevExpress.DataAccess.ConnectionParameters;
//using DevExpress.DataAccess.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace MasterDetail.Code
{
    public class MultiTenantDashboardConfigurator : DashboardConfigurator
    {
        // ----------------------------------------------------------------------------------

        #region Variables

        private readonly IHttpContextAccessor contextAccessor;
        private IFileProvider fileProvider { get; }
        public IConfiguration Configuration { get; }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Public Functions

        public MultiTenantDashboardConfigurator(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            this.contextAccessor = contextAccessor;
            this.fileProvider = hostingEnvironment.ContentRootFileProvider;
            Configuration = configuration;

            //string user = contextAccessor.HttpContext.Request.Headers["User"];
            //string promoCode = contextAccessor.HttpContext.Request.Headers["PromoCode"];

            //SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath));
            //SetDataSourceStorage(new CustomDataSourceStorage());

            ////"Data/Dashboards"
            ////"\\gkgfp\shares\CustData\CTC\Dashboards"
            string physicalPath = fileProvider.GetFileInfo(@"Data/Dashboards").PhysicalPath; 
            ////if (!Directory.Exists(physicalPath)) // If not developing, get path from appsettings.json "DashboardsLocation"
            ////{
            //    ////physicalPath = @"\\gkgfp\shares\CustData\CTC\Dashboards";
            //    string physicalPath = Configuration.GetConnectionString("DashboardsLocation").ToString(); // works from local
            ////}

            //DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo(@"\\gkgfp\shares\CustData\CTC\Dashboards").PhysicalPath);
            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(physicalPath);
            SetDashboardStorage(dashboardFileStorage);

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "LAGemConnection");
            //DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "DefaultConnection");
            ////sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            sqlDataSource.DataProcessingMode = DataProcessingMode.Server;
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
            //SetDataSourceStorage(dataSourceStorage);

            //DashboardObjectDataSource objDataSource1 = new DashboardObjectDataSource("DepartmentQueueProcessesView_all", typeof(DepartmentQueueProcessesView));
            //dataSourceStorage.RegisterDataSource("ediAttendantDataObjectDataSource", objDataSource1.SaveToXml());
            //DashboardObjectDataSource objDataSource2 = new DashboardObjectDataSource("DepartmentQueueProcessesView_thisyear", typeof(DepartmentQueueProcessesView));
            //dataSourceStorage.RegisterDataSource("ediAttendantDataObjectDataSource", objDataSource2.SaveToXml());

            ConfigureDataConnection += DashboardConfigurator_ConfigureDataConnection;
            DataLoading += DashboardConfigurator_DataLoading;
            CustomParameters += DashboardConfigurator_CustomParameters;
            CustomFilterExpression += DashboardConfigurator_CustomFilterExpression;
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Private Functions

        // Configure user-specific data caching
        private void DashboardConfigurator_CustomParameters(object sender, CustomParametersWebEventArgs e)
        {
        }

        // Conditional data loading for ObjectDataSource
        private async void DashboardConfigurator_DataLoading(object sender, DataLoadingWebEventArgs e)
        {
            SqlData sqlData = new SqlData();
            e.Data = await sqlData.GetDashboardData(e.DashboardId);
        }

        // Conditional data loading for other datasource types
        private void DashboardConfigurator_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e)
        {
            return;
        }

        // Custom data filtering for SqlDataSource
        private void DashboardConfigurator_CustomFilterExpression(object sender, CustomFilterExpressionWebEventArgs e)
        {
        }

        private string GetBaseUrl()
        {
            var request = contextAccessor.HttpContext.Request;
            return UriHelper.BuildAbsolute(request.Scheme, request.Host);
        }

        #endregion

        // ----------------------------------------------------------------------------------

    }
}