using MasterDetail.Code;
using MasterDetail.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MasterDetail.Data
{
    public class SqlData
    {
        // ----------------------------------------------------------------------------------

        #region Public Functions

        public async Task<List<POData>> GetPOData()
        {
            string query = @"SELECT TOP (1000) 
		[sono]		AS PONumber
      ,[salesno]	AS SONumber
      ,[partno]		AS ItemCatalog
      ,[sqty]		AS Units
      ,[crea_date]  AS OrderDate
      ,[ship_date]	AS FactoryCancel
  FROM [jade01].[dbo].[SOHEADER]
  ORDER BY 1 DESC ";
            //string fullQuery = string.Format(query, whereStatement);
            string fullQuery = query;
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return null;
            }

            if (dt == null)
            {
                return new List<POData>();
            }

            List<POData> data = new List<POData>();
            foreach (DataRow dr in dt.Rows)
            {
                data.Add(new POData()
                {
                    PONumber = dr["PONumber"].ToString(),
                    SONumber = dr["SONumber"].ToString(),
                    ItemCatalog = dr["ItemCatalog"].ToString(),
                    Units = decimal.Parse(dr["Units"].ToString()),
                    OrderDate = DateTime.Parse(dr["OrderDate"].ToString()),
                    FactoryCancel = DateTime.Parse(dr["FactoryCancel"].ToString())
                });
            }

            return data;
        }

        public async Task<List<Labor>> GetLaborData(string sono)
        {
            string query = @"SELECT [sono] AS PONumber
      --,[opno]
      ,[descrip] AS Program
      ,[loadcenter] AS CardToUse
      ,[laborgrade] AS BoxStyle
      ,[misc01] AS Accessories
      ,[note01] AS TicketProofApproval
	  ,row_number() OVER (ORDER BY [sono], [opno], [timestmp] DESC) AS [Id]
  FROM [jade01].[dbo].[SOROUTE]
  WHERE [sono] = '{0}'
  ORDER BY [opno] ASC ";
            string fullQuery = string.Format(query, sono);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return null;
            }

            if (dt == null)
            {
                return new List<Labor>();
            }

            List<Labor> data = new List<Labor>();
            foreach (DataRow dr in dt.Rows)
            {
                data.Add(new Labor()
                {
                    PONumber = dr["PONumber"].ToString(),
                    Program = dr["Program"].ToString(),
                    CardToUse = dr["CardToUse"].ToString(),
                    BoxStyle = dr["BoxStyle"].ToString(),
                    Accessories = dr["Accessories"].ToString(),
                    TicketProofApproval = dr["TicketProofApproval"].ToString(),
                    Id = int.Parse(dr["Id"].ToString())
                });
            }

            return data;
        }

        public async Task<List<Freight>> GetFreightData(string sono)
        {
            string query = @"SELECT [sono] AS PONumber
      --,[opno]--
      ,[code] AS ShipMethod
      ,[t_time] AS InvoiceNo
      ,[part_grade] AS Tracking
      ,[qty] AS UnitsShipped
      ,[timestmp] AS DateShipped
      --,[lastchg] AS ETA_LA --[timestmp] + 10days
      ,DATEADD(DAY, 10, [timestmp] ) AS ETA_LA --[timestmp] + 10days
	  ,row_number() OVER (ORDER BY [sono], [opno], [timestmp] DESC) AS [Id]
  FROM [jade01].[dbo].[SOTRAN]
  WHERE [sono] = '{0}'
  ORDER BY [opno] ASC ";
            string fullQuery = string.Format(query, sono);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return null;
            }

            if (dt == null)
            {
                return new List<Freight>();
            }

            List<Freight> data = new List<Freight>();
            foreach (DataRow dr in dt.Rows)
            {
                data.Add(new Freight()
                {
                    PONumber = dr["PONumber"].ToString(),
                    ShipMethod = dr["ShipMethod"].ToString(),
                    InvoiceNo = dr["InvoiceNo"].ToString(),
                    Tracking = dr["Tracking"].ToString(),
                    //UnitsShipped = decimal.Parse(dr["UnitsShipped"].ToString()),
                    UnitsShipped = dr["UnitsShipped"].ToString(),
                    DateShipped = DateTime.Parse(dr["DateShipped"].ToString()),
                    ETA_LA = DateTime.Parse(dr["ETA_LA"].ToString()),
                    Id = int.Parse(dr["Id"].ToString())
                });
            }

            return data;
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region DepartmentQueue Functions

  //      public async Task<List<DepartmentQueue>> GetDepartmentQueueData()
  //      {
  //          string whereStatement = "";

  //          string query = @"SELECT TOP (1000) [OID]
  //    ,[sono]
  //    ,[opno]
  //    ,[department]
  //    ,[location]
  //    ,[workcenter]
  //    ,[operator]
  //    ,[starting_qty]
  //    ,[processing_qty]
  //    ,[completed_qty]
  //    ,[status]
  //    ,[scheduling_status]
  //    ,[completed]
  //    ,[completed_processes]
  //    ,[last_queue_process_entry_oid]
  //    ,[previous_department_queue_oid]
  //    ,[sort_order]
  //    ,[created_date]
  //    ,[in_memex]
  //FROM [JAM].[dbo].[DepartmentQueue]
  //  {0}
  //ORDER BY 1 DESC ";
  //          string fullQuery = string.Format(query, whereStatement);
  //          DataTableToObjectConverter converter = new DataTableToObjectConverter();

  //          DataTable dt = null;
  //          try
  //          {
  //              dt = await converter.GetDataTableFromQuery(fullQuery);
  //          }
  //          catch (Exception ex)
  //          {
  //              //LogError("GetSqlData", ex, ex.Message);
  //              return null;
  //          }

  //          if (dt == null)
  //          {
  //              return new List<DepartmentQueue>();
  //          }

  //          List<DepartmentQueue> data = new List<DepartmentQueue>();
  //          foreach (DataRow dr in dt.Rows)
  //          {
  //              data.Add(new DepartmentQueue()
  //              {
  //                  OID = int.Parse(dr["OID"].ToString()),
  //                  sono = dr["sono"].ToString(),
  //                  opno = int.Parse(dr["opno"].ToString()),
  //                  department = dr["department"].ToString(),
  //                  location = dr["location"].ToString(),
  //                  workcenter = dr["workcenter"].ToString(),
  //                  @operator = dr["operator"].ToString(),
  //                  processing_qty = int.Parse(dr["processing_qty"].ToString()),
  //                  completed_qty = int.Parse(dr["completed_qty"].ToString()),
  //                  status = dr["status"].ToString(),
  //                  scheduling_status = dr["scheduling_status"].ToString(),
  //                  completed = bool.Parse(dr["completed"].ToString()),
  //                  completed_processes = dr["completed_processes"].ToString(),
  //                  last_queue_process_entry_oid = int.Parse(dr["last_queue_process_entry_oid"].ToString()),
  //                  previous_department_queue_oid = int.Parse(dr["previous_department_queue_oid"].ToString()),
  //                  sort_order = int.Parse(dr["sort_order"].ToString()),
  //                  created_date = DateTime.Parse(dr["created_date"].ToString()),
  //                  in_memex = bool.Parse(dr["in_memex"].ToString())
  //              });
  //          }

  //          return data;
  //      }

  //      public async Task<List<DepartmentQueueLocations>> GetDepartmentQueueLocationsData(string whereStatement = "WHERE DepartmentQueueOID IN (SELECT TOP (1000) [OID] FROM [JAM].[dbo].[DepartmentQueue] ORDER BY 1 DESC)")
  //      {
  //          string query = @"SELECT [pk]
  //    ,[DepartmentQueueOID]
  //    ,[Location]
  //    ,[WorkCenter]
  //    ,[SchedulingStatus]
  //    ,[StartingQty]
  //    ,[ProcessingQty]
  //    ,[CompletedQty]
  //    ,[IsActive]
  //    ,[LastQueueProcessEntryOid]
  //    ,[EnteredDate]
  //    ,[UpdatedDate]
  //    ,[Status]
  //    ,[Notes]
  //    ,[OperatorId]
  //    ,[AssetType]
  //FROM [JAM].[dbo].[DepartmentQueueLocations]
  //{0}
  //ORDER BY 1 DESC";
  //          string fullQuery = string.Format(query, whereStatement);
  //          DataTableToObjectConverter converter = new DataTableToObjectConverter();

  //          DataTable dt = null;
  //          try
  //          {
  //              dt = await converter.GetDataTableFromQuery(fullQuery);
  //          }
  //          catch (Exception ex)
  //          {
  //              //LogError("GetSqlData", ex, ex.Message);
  //              return null;
  //          }

  //          if (dt == null)
  //          {
  //              return new List<DepartmentQueueLocations>();
  //          }

  //          List<DepartmentQueueLocations> data = new List<DepartmentQueueLocations>();
  //          foreach (DataRow dr in dt.Rows)
  //          {
  //              data.Add(new DepartmentQueueLocations()
  //              {
  //                  pk = int.Parse(dr["pk"].ToString()),
  //                  DepartmentQueueOID = int.Parse(dr["DepartmentQueueOID"].ToString()),
  //                  Location = dr["Location"].ToString(),
  //                  Workcenter = dr["Workcenter"].ToString(),
  //                  SchedulingStatus = dr["SchedulingStatus"].ToString(),
  //                  OperatorId = dr["OperatorId"].ToString(),
  //                  StartingQty = int.Parse(dr["StartingQty"].ToString()),
  //                  ProcessingQty = int.Parse(dr["ProcessingQty"].ToString()),
  //                  CompletedQty = int.Parse(dr["CompletedQty"].ToString()),
  //                  IsActive = bool.Parse(dr["IsActive"].ToString()),
  //                  LastQueueProcessEntryOid = int.Parse(dr["LastQueueProcessEntryOid"].ToString()),
  //                  EnteredDate = DateTime.Parse(dr["EnteredDate"].ToString()),
  //                  UpdatedDate = DateTime.Parse(dr["UpdatedDate"].ToString()),
  //                  Status = dr["Status"].ToString(),
  //                  AssetType = dr["AssetType"].ToString(),
  //                  Notes = dr["Notes"].ToString()
  //              });
  //          }

  //          return data;
  //      }

  //      public async Task<List<DepartmentQueueProcesses>> GetDepartmentQueueProcessesData(string whereStatement = "WHERE [FOID] IN (SELECT TOP (1000) [OID] FROM [JAM].[dbo].[DepartmentQueue] ORDER BY 1 DESC)")
  //      {
  //          string query = @"SELECT [OID]
  //    ,[FOID]
  //    ,[department]
  //    ,[operator]
  //    ,[process]
  //    ,[start_time]
  //    ,[end_time]
  //    ,[qty]
  //    ,[reason]
  //    ,[workcenter]
  //    ,[LOID]
  //    ,[notes]
  //FROM [JAM].[dbo].[DepartmentQueueProcess]
  //{0}
  //ORDER BY 1 DESC";
  //          string fullQuery = string.Format(query, whereStatement);
  //          DataTableToObjectConverter converter = new DataTableToObjectConverter();

  //          DataTable dt = null;
  //          try
  //          {
  //              dt = await converter.GetDataTableFromQuery(fullQuery);
  //          }
  //          catch (Exception ex)
  //          {
  //              //LogError("GetSqlData", ex, ex.Message);
  //              return null;
  //          }

  //          if (dt == null)
  //          {
  //              return new List<DepartmentQueueProcesses>();
  //          }

  //          List<DepartmentQueueProcesses> data = new List<DepartmentQueueProcesses>();
  //          foreach (DataRow dr in dt.Rows)
  //          {
  //              data.Add(new DepartmentQueueProcesses()
  //              {
  //                  OID = int.Parse(dr["OID"].ToString()),
  //                  FOID = int.Parse(dr["FOID"].ToString()),
  //                  LOID = int.Parse(dr["LOID"].ToString()),
  //                  department = dr["department"].ToString(),
  //                  workcenter = dr["workcenter"].ToString(),
  //                  @operator = dr["operator"].ToString(),
  //                  process = dr["process"].ToString(),
  //                  start_time = DateTime.Parse(dr["start_time"].ToString()),
  //                  end_time = DateTime.Parse(dr["end_time"].ToString()),
  //                  qty = int.Parse(dr["qty"].ToString()),
  //                  reason = dr["reason"].ToString(),
  //                  notes = dr["notes"].ToString()
  //              });
  //          }

  //          return data;
  //      }

  //      public async Task<List<DepartmentQueueProcessesView>> GetDepartmentQueueProcessesViewData(string whereStatement = "")
  //      {
  //          string query = @"SELECT [QOID]
  //    ,[POID]
  //    ,[LOID]
  //    ,[DepartmentCode]
  //    ,[DepartmentName]
	 // ,CASE WHEN [DepartmentCode] = 'SNT' THEN CASE WHEN [LocationWorkCenter] = 'false' THEN '' ELSE [LocationWorkCenter] END ELSE [Workcenter] END AS [Workcenter]
  //    ,CASE WHEN [LocationWorkCenter] = 'false' THEN '' ELSE [LocationWorkCenter] END AS [LocationWorkCenter]
  //    ,[OperatorId]
  //    ,[OperatorName]
  //    ,[sono]
  //    ,[opno]
  //    ,[PartNo]
  //    ,[Location]
  //    ,[Process]
  //    ,[ProcessDescrip]
  //    ,[Qty]
  //    ,[Reason]
  //    ,[StartTime]
  //    ,[EndTime]
  //    ,[TimeSpent]
  //    ,[HoursSpent]
  //    ,CAST([MinutesSpent] AS VARCHAR(50)) AS [MinutesSpent]
  //    ,[EndTimeUsed]
  //    ,[ProcessNotes]
  //    ,[UpdatedDate]
  //FROM [JAM].[dbo].[DepartmentQueueProcessesView] {0}";
  //          string fullQuery = string.Format(query, whereStatement);
  //          DataTableToObjectConverter converter = new DataTableToObjectConverter();

  //          DataTable dt = null;
  //          try
  //          {
  //              dt = await converter.GetDataTableFromQuery(fullQuery);
  //          }
  //          catch (Exception ex)
  //          {
  //              //LogError("GetSqlData", ex, ex.Message);
  //              return null;
  //          }

  //          if (dt == null)
  //          {
  //              return new List<DepartmentQueueProcessesView>();
  //          }

  //          List<DepartmentQueueProcessesView> data = new List<DepartmentQueueProcessesView>();
  //          foreach (DataRow dr in dt.Rows)
  //          {
  //              data.Add(new DepartmentQueueProcessesView()
  //              {
  //                  QOID = int.Parse(dr["QOID"].ToString()),
  //                  POID = int.Parse(dr["POID"].ToString()),
  //                  LOID = int.Parse(dr["LOID"].ToString()),
  //                  DepartmentCode = dr["DepartmentCode"].ToString(),
  //                  DepartmentName = dr["DepartmentName"].ToString(),
  //                  Workcenter = dr["Workcenter"].ToString(),
  //                  LocationWorkCenter = dr["LocationWorkCenter"].ToString(),
  //                  OperatorId = dr["OperatorId"].ToString(),
  //                  OperatorName = dr["OperatorName"].ToString(),
  //                  sono = dr["sono"].ToString(),
  //                  opno = int.Parse(dr["opno"].ToString()),
  //                  PartNo = dr["PartNo"].ToString(),
  //                  Location = dr["Location"].ToString(),
  //                  Process = dr["Process"].ToString(),
  //                  ProcessDescrip = dr["ProcessDescrip"].ToString(),
  //                  Qty = int.Parse(dr["Qty"].ToString()),
  //                  Reason = dr["Reason"].ToString(),
  //                  StartTime = DateTime.Parse(dr["StartTime"].ToString()),
  //                  EndTime = DateTime.Parse(dr["EndTime"].ToString()),
  //                  TimeSpent = decimal.Parse(dr["TimeSpent"].ToString()),
  //                  HoursSpent = dr["HoursSpent"].ToString(),
  //                  MinutesSpent = decimal.Parse(dr["MinutesSpent"].ToString()),
  //                  EndTimeUsed = dr["EndTimeUsed"].ToString(),
  //                  ProcessNotes = dr["ProcessNotes"].ToString(),
  //                  UpdatedDate = DateTime.Parse(dr["UpdatedDate"].ToString())
  //              });
  //          }

  //          return data;
  //      }

        public async Task<object> GetDashboardData(string dashboardId)
        {
            //List<DepartmentQueueProcessesView> data = GetDepartmentQueueProcessesViewData().Result;
            //return data;

            DataTableToObjectConverter converter = new DataTableToObjectConverter();
            DataTable dt = new DataTable();
            string whereStatement = "";

            switch (dashboardId)
            {
                case "HomePageDashboard":
                case "PivotDashboard":
                case "DashboardEdit":
                case "DepartmentQueueProcessesView_all":
                    whereStatement = "";
                    break;

                case "DepartmentQueueProcessesView_thisyear":
                    whereStatement = @"
  WHERE YEAR([EndTime]) = YEAR(GETDATE())";
                    break;
                default:
                    whereStatement = "";
                    break;
            }

            List<DepartmentQueueProcessesView> data = GetDepartmentQueueProcessesViewData(whereStatement).Result;
            return data;
        }

        public async Task<List<DepartmentQueueProcessesView>> GetDepartmentQueueProcessesViewData(string whereStatement = "")
        {
            string query = @"SELECT [QOID]
      ,[POID]
      ,[LOID]
      ,[DepartmentCode]
      ,[DepartmentName]
	  ,CASE WHEN [DepartmentCode] = 'SNT' THEN CASE WHEN [LocationWorkCenter] = 'false' THEN '' ELSE [LocationWorkCenter] END ELSE [Workcenter] END AS [Workcenter]
      ,CASE WHEN [LocationWorkCenter] = 'false' THEN '' ELSE [LocationWorkCenter] END AS [LocationWorkCenter]
      ,[OperatorId]
      ,[OperatorName]
      ,[sono]
      ,[opno]
      ,[PartNo]
      ,[Location]
      ,[Process]
      ,[ProcessDescrip]
      ,[Qty]
      ,[Reason]
      ,[StartTime]
      ,[EndTime]
      ,[TimeSpent]
      ,[HoursSpent]
      ,CAST([MinutesSpent] AS VARCHAR(50)) AS [MinutesSpent]
      ,[EndTimeUsed]
      ,[ProcessNotes]
      ,[UpdatedDate]
  FROM [JAM].[dbo].[DepartmentQueueProcessesView] {0}";
            string fullQuery = string.Format(query, whereStatement);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return null;
            }

            if (dt == null)
            {
                return new List<DepartmentQueueProcessesView>();
            }

            List<DepartmentQueueProcessesView> data = new List<DepartmentQueueProcessesView>();
            foreach (DataRow dr in dt.Rows)
            {
                data.Add(new DepartmentQueueProcessesView()
                {
                    QOID = int.Parse(dr["QOID"].ToString()),
                    POID = int.Parse(dr["POID"].ToString()),
                    LOID = int.Parse(dr["LOID"].ToString()),
                    DepartmentCode = dr["DepartmentCode"].ToString(),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    Workcenter = dr["Workcenter"].ToString(),
                    LocationWorkCenter = dr["LocationWorkCenter"].ToString(),
                    OperatorId = dr["OperatorId"].ToString(),
                    OperatorName = dr["OperatorName"].ToString(),
                    sono = dr["sono"].ToString(),
                    opno = int.Parse(dr["opno"].ToString()),
                    PartNo = dr["PartNo"].ToString(),
                    Location = dr["Location"].ToString(),
                    Process = dr["Process"].ToString(),
                    ProcessDescrip = dr["ProcessDescrip"].ToString(),
                    Qty = int.Parse(dr["Qty"].ToString()),
                    Reason = dr["Reason"].ToString(),
                    StartTime = DateTime.Parse(dr["StartTime"].ToString()),
                    EndTime = DateTime.Parse(dr["EndTime"].ToString()),
                    TimeSpent = decimal.Parse(dr["TimeSpent"].ToString()),
                    HoursSpent = dr["HoursSpent"].ToString(),
                    MinutesSpent = decimal.Parse(dr["MinutesSpent"].ToString()),
                    EndTimeUsed = dr["EndTimeUsed"].ToString(),
                    ProcessNotes = dr["ProcessNotes"].ToString(),
                    UpdatedDate = DateTime.Parse(dr["UpdatedDate"].ToString())
                });
            }

            return data;
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Unused EdiAttendantData Functions

        //       public async Task<object> GetDashboardData(string dashboardId, string user, string promoCode, string dataSourceName, string dataSourceComponentName)
        //       {
        //           string query = "";
        //           string fullQuery = "";
        //           DataTableToObjectConverter converter = new DataTableToObjectConverter();
        //           DataTable dt = new DataTable();

        //           switch (dashboardId)
        //           {

        //               case "ProductActivityDashboard":
        //                   List<EdiAttendantData> data = GetEdiAttendantData(promoCode).Result;
        //                   return data;

        //               case "dashboard1":
        //               case "dashboard2":
        //               case "HomePageDashboard":
        //                   List<ShippingData> shippingData = new List<ShippingData>();
        //                   string shippingRange = "FROMBEGINNINGOFYEAR";

        //                   switch (dataSourceName)
        //                   {
        //                       case "ShippingData_FromBeginningofYear":
        //                           shippingRange = "FROMBEGINNINGOFYEAR";
        //                           break;
        //                       case "ShippingData_FromLastThreeMonths":
        //                           shippingRange = "LAST3MONTHS";
        //                           break;
        //                       case "ShippingData_FromMonth":
        //                           shippingRange = "MONTH";
        //                           break;
        //                       case "ShippingData_FromToday":
        //                           shippingRange = "TODAY";
        //                           break;
        //                   }
        //                   shippingData = GetShippingData(promoCode, shippingRange).Result;
        //                   return shippingData;

        //               case "HomePageDashboardTest":
        //                   List<ShippingData> shippingTestData = new List<ShippingData>();

        //                   query = GetQuery("Shipped", promoCode);

        //                   switch (dataSourceName)
        //                   {
        //                       //case "ShippingData_FromBeginningofYear":
        //                       //                           fullQuery = query + @"
        //                       //AND SHIP_DATE >= CAST(YEAR(GETDATE()) AS VARCHAR(4)) + '-01-01'";
        //                       //                           break;
        //                       //case "ShippingData_FromLastThreeMonths":
        //                       //                           fullQuery = query + @"
        //                       //AND SHIP_DATE >= CONVERT(date,DATEADD(month, -3, GETDATE())) ";
        //                       //                           break;
        //                       //case "ShippingDataa_FromToday":
        //                       //                           fullQuery = query + @"
        //                       //AND SHIP_DATE = CONVERT(date,GETDATE()) ";
        //                       //                           break;

        //                       case "ShippingData_FromBeginningofYear":
        //                           fullQuery = query + @"
        //AND SHIP_DATE >= '2022-01-01' AND SHIP_DATE <= '2022-04-15'-- + CAST(DAY(GETDATE()) AS VARCHAR(2)) "; // testing dates
        //                           break;
        //                       case "ShippingData_FromLastThreeMonths":
        //                           fullQuery = query + @"
        //AND SHIP_DATE >= '2022-01-15'-- + CAST(DAY(GETDATE()) AS VARCHAR(2)) "; // testing dates
        //                           break;
        //                       case "ShippingData_FromToday":
        //                           fullQuery = query + @"
        //AND SHIP_DATE = '2022-04-15' --+ CAST(DAY(GETDATE()) AS VARCHAR(2))  "; // testing dates
        //                           break;
        //                   }

        //                   if (!string.IsNullOrWhiteSpace(fullQuery))
        //                   {
        //                       fullQuery += @"
        // ORDER BY CAST(DATEPART(MONTH, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS INT) ASC
        //,CAST(DATEPART(DAY, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS INT) ASC";
        //                       dt = await converter.GetDataTableFromQuery(fullQuery);
        //                       shippingTestData = await converter.GetObjectListFromDataTable<ShippingData>(dt, new Dictionary<string, string>());
        //                       return shippingTestData;
        //                   }
        //                   break;
        //           }
        //           return new List<ShippingData>();
        //       }

        //       public string GetQuery(string queryName, string promoCode, string databasePrefix = "MOM-")
        //       {
        //           string databaseName = databasePrefix + promoCode;
        //           string query = "";

        //           switch (queryName)
        //           {
        //               case "Shipped":
        //                   query = @"SELECT CAST(cms.[ORDERNO] AS VARCHAR(20)) AS OrderNo
        //     ,CAST(cms.[CUSTNUM] AS VARCHAR(20)) AS CustNo
        //     ,cms.[CL_KEY]		AS SourceKey
        //     ,cms.[ODR_DATE]	AS OrderDate
        //     ,cms.[SHIP_DATE]	AS ShipDate
        //     ,cms.[SHIPLIST]	AS ShipList
        //     ,cms.[ORD_TOTAL]	AS OrderTotal
        //     ,CAST(items.[ITEM_ID]	AS VARCHAR(20)) AS ItemId
        //     ,items.[ITEM]			AS ItemCode
        //     ,CAST(items.[QUANTS]	AS INT) AS ItemQty
        //     ,items.[IT_SDATE]		AS ItemShipDate
        //     ,items.[ITEM_STATE]	AS ItemState
        //     ,DATEPART(YEAR, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS [Year]
        //     ,DATEPART(MONTH, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS [M]
        //     ,DATENAME(MONTH, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS [Month]
        //     ,DATENAME(DAY, ISNULL(cms.[SHIP_DATE], '1900-01-01 00:00:00.000')) AS [Day]
        //  ,CONVERT(char(11),cms.[SHIP_DATE],103) AS DisplayDate 
        // FROM [@databaseName].[dbo].[CMS] cms 
        // LEFT JOIN [@databaseName].[dbo].[ITEMS] items
        //ON cms.[ORDERNO] = items.[ORDERNO]
        // WHERE cms.[SHIP_DATE] IS NOT NULL
        //AND items.[ITEM_ID] IS NOT NULL";
        //                   query = query.Replace("@databaseName", databaseName);
        //                   break;
        //           }

        //           return query;
        //       }

        //       public async Task<List<EdiAttendantData>> GetEdiAttendantData(string promoCode)
        //       {
        //           DataTable dt = await GetSqlEdiAttendantDataTable(promoCode);
        //           List<EdiAttendantData> data = new List<EdiAttendantData>();
        //           foreach (DataRow dr in dt.Rows)
        //           {
        //               data.Add(new EdiAttendantData()
        //               {
        //                   Location_Name = dr["Location_Name"].ToString(),
        //                   LocationId = dr["LocationId"].ToString(),
        //                   UOM = dr["UOM"].ToString(),
        //                   QtySold = decimal.Parse(dr["QtySold"].ToString()),
        //                   DateStart = dr["DateStart"].ToString(),
        //                   DateEnd = dr["DateEnd"].ToString(),
        //                   Item = dr["Item"].ToString(),
        //                   Price = int.Parse(dr["Price"].ToString()),
        //                   WeekNo = int.Parse(dr["WeekNo"].ToString()),
        //                   ExtPrice = decimal.Parse(dr["ExtPrice"].ToString()),
        //                   UPC = double.Parse(dr["UPC"].ToString()),
        //               });
        //           }

        //           return data;
        //       }
        //       public async Task<List<ShippingData>> GetShippingData(string promoCode, string range = "FROMBEGINNINGOFYEAR")
        //       {
        //           string query = "EXEC [dbo].[CTCsp_GetShippedData] '{0}', '{1}'";
        //           string fullQuery = string.Format(query, promoCode, range);
        //           DataTableToObjectConverter converter = new DataTableToObjectConverter();
        //           DataTable dt = await converter.GetDataTableFromQuery(fullQuery);
        //           List<ShippingData> data = new List<ShippingData>();
        //           if (dt == null)
        //               return data;

        //           foreach (DataRow dr in dt.Rows)
        //           {
        //               data.Add(new ShippingData()
        //               {
        //                   OrderNo = dr["OrderNo"].ToString(),
        //                   CustNo = dr["CustNo"].ToString(),
        //                   SourceKey = dr["SourceKey"].ToString(),
        //                   OrderDate = DateTime.Parse(dr["OrderDate"].ToString()),
        //                   ShipDate = DateTime.Parse(dr["ShipDate"].ToString()),
        //                   ShipList = dr["ShipList"].ToString(),
        //                   OrderTotal = decimal.Parse(dr["OrderTotal"].ToString()),
        //                   ItemId = dr["ItemId"].ToString(),
        //                   ItemCode = dr["ItemCode"].ToString(),
        //                   ItemQty = int.Parse(dr["ItemQty"].ToString()),
        //                   ItemShipDate = DateTime.Parse(dr["ItemShipDate"].ToString()),
        //                   ItemState = dr["ItemState"].ToString(),
        //                   Year = int.Parse(dr["Year"].ToString()),
        //                   M = int.Parse(dr["M"].ToString()),
        //                   Month = dr["Month"].ToString(),
        //                   Day = int.Parse(dr["Day"].ToString()),
        //                   DisplayDate = dr["DisplayDate"].ToString()
        //               });
        //           }

        //           return data;
        //       }

        //       public async Task<DataTable> GetSqlEdiAttendantDataTable(string promoCode)
        //       {
        //           if (promoCode == "")
        //           {
        //               promoCode = "0000";
        //           }

        //           //            string query = @"
        //           //DECLARE @promoCode		VARCHAR(50) = '{0}'
        //           //DECLARE @ediAttendantId	VARCHAR(50) = NULL
        //           //DECLARE @databaseName	VARCHAR(50) = 'ediAttendant_'
        //           //DECLARE @workQuery		VARCHAR(MAX) = ''	

        //           //SELECT @ediAttendantId = [ediAttendantId]
        //           //  FROM [CTCEnterprise].[dbo].[Company]
        //           //  WHERE [PromoCode] = @promoCode
        //           //SET @databaseName = @databaseName + RTRIM(@ediAttendantId)

        //           //--SELECT @promoCode AS promoCode, @ediAttendantId AS ediAttendantId, @databaseName AS [database]

        //           //SET @workQuery = 'SELECT [Location_Name]
        //           //      ,[Location_id] AS LocationId
        //           //      ,[uom]		AS UOM
        //           //      ,[QtySold]	AS QtySold
        //           //      ,[date_beg]	AS DateStart
        //           //      ,[date_end]	AS DateEnd
        //           //      ,[Item]		AS Item
        //           //      ,[price]		AS Price
        //           //      ,[weekno]		AS WeekNo
        //           //      ,[ExtPrice]	AS ExtPrice
        //           //      ,[upc]		AS UPC
        //           //  FROM [@databaseName].[dbo].[PDDtl_bmy]'

        //           //SET @workQuery = REPLACE(@workQuery, '@databaseName', @databaseName);

        //           //EXEC(@workQuery)
        //           //";
        //           string query = "EXEC CTCsp_GetEdiAttendantData '{0}' ";

        //           string fullQuery = string.Format(query, promoCode);
        //           DataTableToObjectConverter converter = new DataTableToObjectConverter();

        //           DataTable dt = null;
        //           try
        //           {
        //               dt = await converter.GetDataTableFromQuery(fullQuery);
        //           }
        //           catch (Exception ex)
        //           {
        //               //LogError("GetSqlData", ex, ex.Message);
        //           }

        //           return dt;
        //       }
        //       public async Task<DataTable> GetSqlEdiAttendantDataTable(UserAccount user)
        //       {
        //           //            string query = @"
        //           //DECLARE @promoCode		VARCHAR(50) = '{0}'
        //           //DECLARE @ediAttendantId	VARCHAR(50) = NULL
        //           //DECLARE @databaseName	VARCHAR(50) = 'ediAttendant_'
        //           //DECLARE @workQuery		VARCHAR(MAX) = ''	

        //           //SELECT @ediAttendantId = [ediAttendantId]
        //           //  FROM [CTCEnterprise].[dbo].[Company]
        //           //  WHERE [PromoCode] = @promoCode
        //           //SET @databaseName = @databaseName + RTRIM(@ediAttendantId)

        //           //--SELECT @promoCode AS promoCode, @ediAttendantId AS ediAttendantId, @databaseName AS [database]

        //           //SET @workQuery = 'SELECT [Location_Name]
        //           //      ,[Location_id] AS LocationId
        //           //      ,[uom]		AS UOM
        //           //      ,[QtySold]	AS QtySold
        //           //      ,[date_beg]	AS DateStart
        //           //      ,[date_end]	AS DateEnd
        //           //      ,[Item]		AS Item
        //           //      ,[price]		AS Price
        //           //      ,[weekno]		AS WeekNo
        //           //      ,[ExtPrice]	AS ExtPrice
        //           //      ,[upc]		AS UPC
        //           //  FROM [@databaseName].[dbo].[PDDtl_bmy]'

        //           //SET @workQuery = REPLACE(@workQuery, '@databaseName', @databaseName);

        //           //EXEC(@workQuery)
        //           //";
        //           string query = "EXEC CTCsp_GetEdiAttendantData '{0}' ";
        //           string fullQuery = string.Format(query, user.PromoCode);
        //           DataTableToObjectConverter converter = new DataTableToObjectConverter();

        //           DataTable dt = null;
        //           try
        //           {
        //               dt = await converter.GetDataTableFromQuery(fullQuery);
        //           }
        //           catch (Exception ex)
        //           {
        //               //LogError("GetSqlData", ex, ex.Message);
        //           }

        //           return dt;
        //       }

        #endregion

        // ----------------------------------------------------------------------------------
    }
}
