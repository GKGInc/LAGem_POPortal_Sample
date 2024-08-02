using LAGem_POPortal.Code;
using LAGem_POPortal.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LAGem_POPortal.Authentication
{
    public class UserAccountService
    {
        // ----------------------------------------------------------------------------------

        #region Variables

        private List<UserAccount> _users;
        private List<WebUserData> _webUsers;

        #endregion

        // ----------------------------------------------------------------------------------

        #region Public Functions

        public UserAccountService() { GetAllWebUserData(); }

        public UserAccount GetByUserName(string userName)
        {
            if (_webUsers == null)
                GetAllWebUserData();

            WebUserData webUser = _webUsers.FirstOrDefault(x => x.UserName == userName);
            return GetUserAccount(webUser);
        }

        public UserAccount GetByUserName(int oid)
        {
            WebUserData webUser = _webUsers.FirstOrDefault(x => x.Oid == oid);
            return GetUserAccount(webUser);
        }
       
        private UserAccount GetUserAccount(WebUserData webUser)
        {
            if (webUser != null)
            {
                UserAccount user = new UserAccount
                {
                    UserName = webUser.UserName,
                    Password = webUser.UserPassword,
                    Role = (webUser.UserStatus == "ADMIN" || webUser.UserStatus == "ADM") ? "Administrator" : "User",
                    PromoCode = webUser.PromoCode,
                    Oid = webUser.Oid
                };

                //TestGetSqlData(user);
                return user;
            }
            else
                return null;
        }

        public void UpdateLastLoginDate(UserAccount user)
        {
  //          string query = @"UPDATE [CTCEnterprise].[dbo].[WebUsers]
	 // SET [LastLogin] = GETDATE()
  //WHERE [UserName] = '{0}' AND [UserPassword] = '{1}'";
  //          string fullQuery = string.Format(query, user.UserName, user.Password);

  //          try
  //          {
  //              using (var uow = new UnitOfWork())
  //              {
  //                  uow.ExecuteNonQuery(fullQuery);
  //              }
  //          }
  //          catch (Exception ex)
  //          {
  //              //LogError("UpdateLastLoginDate", ex, ex.Message);
  //          }
        }

        public UserAccount GetUserAccountFromClaims(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
        {
            List<System.Security.Claims.Claim> claims = (claimsPrincipal.Identities.ToList()[0] as System.Security.Claims.ClaimsIdentity).Claims.ToList();
            if (claims.Count == 0)
                return null;

            var identity = claimsPrincipal.Identity as System.Security.Claims.ClaimsIdentity;
            var userName = identity.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var password = identity.FindFirst("password")?.Value;
            var userRole = identity.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var promoCode = identity.FindFirst("PromoCode")?.Value;
            var userOid = identity.FindFirst("Oid")?.Value;
            int oid = 0;
            int.TryParse(userOid, out oid);

            UserAccount user = new UserAccount
            {
                UserName = userName,
                Password = password,
                Role = userRole,
                PromoCode = promoCode,
                Oid = oid
            };
            return user;
        }

        public async void GetAllWebUserData()
        {
            string query = @"SELECT [Id] AS [Oid]
      ,[Username] AS [UserName]
      ,[Password] AS [UserPassword]
	  ,'' AS [PromoCode]
      ,UPPER(LEFT([Username],1))+LOWER(SUBSTRING([Username],2,LEN([Username]))) AS [UserFirstName]
      ,'' AS [UserLastName]
      ,'' AS [UserEmail]
      ,[Level] AS [UserStatus]
      ,'1900-01-01 00:00:00.000' AS [LastLogin]
  FROM [Jade01].[dbo].[MobileUsers]";

            try
            {
                DataTableToObjectConverter converter = new DataTableToObjectConverter();
                DataTable dt = await converter.GetDataTableFromQuery(query);

                var json = JsonConvert.SerializeObject(dt);

                List<WebUserData> userData = JsonConvert.DeserializeObject<List<WebUserData>>(json.ToString());
                _webUsers = userData;
            }
            catch (Exception ex)
            {
                string connectionString = ConnectionHelper.ConnectionString;
                if (connectionString == null)
                {
                    connectionString = "Data Source=LAGem;Initial Catalog=LAGem01;User id=sa;Password=sql@GKG;Pooling=false;MultipleActiveResultSets=true"; // Ok                      
                }
                var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);

                DataTable reportTable;
                DevExpress.Xpo.DB.SelectedData selectedData;
                using (var uow = new UnitOfWork(inMemoryDAL))
                {
                    selectedData = await uow.ExecuteQueryWithMetadataAsync(query);

                    var results = selectedData.ResultSet;

                    // build a datatable based on the result information
                    reportTable = DataTableToObjectConverter.BuildTableFromMetaData(results[0]);

                    foreach (var qrow in results[1].Rows)
                    {
                        reportTable.Rows.Add(qrow.Values);
                    }
                }

                var json = JsonConvert.SerializeObject(reportTable);

                List<WebUserData> userData = JsonConvert.DeserializeObject<List<WebUserData>>(json.ToString());

                _webUsers = userData;
            }
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Test Functions

        //        public async void TestGetSqlData(UserAccount user)
        //        {
        //            DataTable dt = await GetSqlData(user);
        //        }

        //        public async Task<DataTable> GetSqlData(UserAccount user)
        //        {
        //            string query = @"
        //DECLARE @promoCode		VARCHAR(50) = '{0}'
        //DECLARE @ediAttendantId	VARCHAR(50) = NULL
        //DECLARE @databaseName	VARCHAR(50) = 'ediAttendant_'
        //DECLARE @workQuery		VARCHAR(MAX) = ''	

        //SELECT @ediAttendantId = [ediAttendantId]
        //  FROM [CTCEnterprise].[dbo].[Company]
        //  WHERE [PromoCode] = @promoCode
        //SET @databaseName = @databaseName + RTRIM(@ediAttendantId)

        //--SELECT @promoCode AS promoCode, @ediAttendantId AS ediAttendantId, @databaseName AS [database]

        //SET @workQuery = 'SELECT [Location_Name]
        //      ,[Location_id] AS LocationId
        //      ,[uom]		AS UOM
        //      ,[QtySold]	AS QtySold
        //      ,[date_beg]	AS DateStart
        //      ,[date_end]	AS DateEnd
        //      ,[Item]		AS Item
        //      ,[price]		AS Price
        //      ,[weekno]		AS WeekNo
        //      ,[ExtPrice]	AS ExtPrice
        //      ,[upc]		AS UPC
        //  FROM [@databaseName].[dbo].[PDDtl_bmy]'

        //SET @workQuery = REPLACE(@workQuery, '@databaseName', @databaseName);

        //EXEC(@workQuery)
        //";
        //            string fullQuery = string.Format(query, user.PromoCode);

        //            DataTable dt = null;
        //            try
        //            {
        //                dt = await GetDataTableFromQuery(fullQuery);
        //            }
        //            catch (Exception ex)
        //            {
        //                //LogError("GetSqlData", ex, ex.Message);
        //            }

        //            return dt;
        //        }

        //        public async Task<DataTable> GetDataTableFromQuery(string query)
        //        {
        //            DataTable reportTable;
        //            DevExpress.Xpo.DB.SelectedData selectedData;

        //            try
        //            {
        //                using (var uow = new UnitOfWork())
        //                {
        //                    selectedData = await uow.ExecuteQueryWithMetadataAsync(query);

        //                    var results = selectedData.ResultSet;

        //                    // build a datatable based on the result information
        //                    reportTable = BuildTableFromMetaData(results[0]);

        //                    foreach (var qrow in results[1].Rows)
        //                    {
        //                        reportTable.Rows.Add(qrow.Values);
        //                    }
        //                }

        //                return reportTable;
        //            }
        //            catch (Exception ex)
        //            {
        //                //LogError("GetDataTableFromQuery", ex, ex.Message);
        //            }

        //            return null;
        //        }

        //        public async Task<DataTable> GetXPOdata(string connectionString, string query)
        //        {
        //            DataTable reportTable;
        //            DevExpress.Xpo.DB.SelectedData selectedData;

        //            try
        //            {
        //                var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);

        //                using (var uow = new UnitOfWork(inMemoryDAL))
        //                {
        //                    string conn = uow.ConnectionString;

        //                    selectedData = await uow.ExecuteQueryWithMetadataAsync(query);

        //                    var results = selectedData.ResultSet;

        //                    // build a datatable based on the result information
        //                    reportTable = BuildTableFromMetaData(results[0]);

        //                    foreach (var qrow in results[1].Rows)
        //                    {
        //                        reportTable.Rows.Add(qrow.Values);
        //                    }
        //                }

        //                return reportTable;
        //            }
        //            catch (Exception ex)
        //            {
        //                //LogError("GetDepartmentRowDataTable", ex, ex.Message);

        //            }

        //            return null;
        //        }

        //        public static DataTable BuildTableFromMetaData(SelectStatementResult columns)
        //        {
        //            var dt = new DataTable();

        //            foreach (var col in columns.Rows)
        //            {
        //                var dotNetType = col.Values[2].ToString();
        //                var dataColumn = new DataColumn(col.Values[0].ToString());
        //                switch (dotNetType)
        //                {
        //                    case "String":
        //                        dataColumn.DataType = typeof(string);
        //                        dataColumn.DefaultValue = "";
        //                        dt.Columns.Add(dataColumn);
        //                        break;
        //                    case "Int32":
        //                    case "Int64":
        //                        dataColumn.DataType = typeof(int);
        //                        dataColumn.DefaultValue = 0;
        //                        dt.Columns.Add(dataColumn);
        //                        break;
        //                    case "Decimal":
        //                        dataColumn.DataType = typeof(decimal);
        //                        dataColumn.DefaultValue = 0m;
        //                        dt.Columns.Add(dataColumn);
        //                        break;
        //                    case "DateTime":
        //                        dataColumn.DataType = typeof(DateTime);
        //                        dataColumn.AllowDBNull = false;
        //                        dataColumn.DefaultValue = new DateTime(1900, 1, 1);
        //                        dt.Columns.Add(dataColumn);
        //                        break;
        //                    case "Boolean":
        //                        dataColumn.DataType = typeof(Boolean);
        //                        dataColumn.AllowDBNull = false;
        //                        dataColumn.DefaultValue = false;
        //                        dt.Columns.Add(dataColumn);
        //                        break;
        //                    default:
        //                        throw new Exception($"Unrecognized column type, '{dotNetType}'.");
        //                }
        //            }

        //            return dt;
        //        }

        #endregion

        // ----------------------------------------------------------------------------------
    }
}
