using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Office.Utils;
using LAGem_POPortal.Authentication;
using LAGem_POPortal.Code;
using LAGem_POPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LAGem_POPortal.Data
{
    public class SqlData
    {
        public async Task<List<T>> GetSqlData<T>(string query)
        {
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(query);
            }
            catch (Exception ex)
            {
                return new List<T>();
            }

            if (dt == null)
            {
                return new List<T>();
            }

            List<T> data = new List<T>();
            data = await converter.GetObjectListFromDataTable<T>(dt, new Dictionary<string, string>());

            return data;
        }

        // ----------------------------------------------------------------------------------

        #region Main Query Functions

        //"Overview"
        public async Task<List<PODetailData>> GetSOPOData() // Overview: Using SOPOvw-JorgeDiagram
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY [SOLineNo],[SOSubLineNo]) AS [Id]
      ,[SONumber]
      ,[SODate]
      ,[CustomerName]	
      ,[CustomerPO]
      ,[StartDate]
      ,[EndDate]
      ,[SOLineNo]
      ,[SOSubLineNo]
	  ,CAST(ISNULL([SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]
      ,ISNULL([LineDisplaySequence], 0) AS [LineDisplaySequence]
      ,ISNULL([ProductDisplaySequence], 0) AS [ProductDisplaySequence]
	  ,CAST(ISNULL([LineDisplaySequence], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([ProductDisplaySequence], 0) AS VARCHAR(4))  AS [DisplaySequence]
      ,ISNULL([ProgramName], '') AS [ProgramName]
      ,ISNULL([ProductTypeName], '') AS [SOSubLineType]
      ,ISNULL([ProductNo], '') AS [ProductNo]
      ,ISNULL([ProductName], '') AS [ProductName]
      ,ISNULL([SOQty], 0) AS [SOQty]
      ,ISNULL([Cost], 0.000000) AS [Cost]
      ,ISNULL([Price], 0.000000) AS [Price]
      ,ISNULL([VendorPO], '') AS [PONumber]
      ,ISNULL([PODate], '1900-01-01') AS [PODate]
      ,ISNULL([VendorName], '') AS [VendorName]
      ,ISNULL([POLineNo], 0) AS [POLineNo]
      ,ISNULL([POSubLineNo], 0) AS [POSubLineNo]
	  ,CAST(ISNULL([POLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([POSubLineNo], 0) AS VARCHAR(4))  AS [POLineNoExt]
      ,ISNULL([POQty], 0) AS [POQty]
      ,ISNULL([POHeaderId], 0) AS [POHeaderId]
      ,ISNULL([PODetailId], 0) AS [PODetailId]
      ,ISNULL([ForProductNo], '') AS [ForProductNo]

      ,ISNULL([ShipmentDate], '1900-01-01') AS [ShipmentDate]
      ,ISNULL([TrackingNumber], '') AS [TrackingNumber]
      ,ISNULL([ShipToETA], '1900-01-01') AS [ShipToETA]
      ,ISNULL([ShipmentQty], 0) AS [ShipmentQty]
      ,ISNULL([SOStatus],'NEW') AS [SOStatus]
      ,ISNULL([ProductTypeGroup],'') AS [ProductTypeGroup]
      ,ISNULL([SOSubLineTypeId], 0) AS [SOSubLineTypeId]
      ,ISNULL([LineTypeName],'') AS [LineTypeName]
      ,ISNULL([LineTypeGroup],'') AS [LineTypeGroup]

      ,ISNULL([QCStatus],'') AS [QCStatus]
      ,ISNULL([QCStatusDate], '1900-01-01') AS [QCStatusDate]
      ,ISNULL([QCComments],'') AS [QCComments]

  FROM [PIMS].[dbo].[SOPOvw-JorgeDiagram]
  ORDER BY [SONumber], [SOLineNo],ISNULL([LineDisplaySequence],0),ISNULL([ProductDisplaySequence],0),[SOSubLineNo],[POLineNo] ";

            return await GetSqlData<PODetailData>(query);
        }
        public async Task<List<PODetailData>> GetSOPODataLite() // Overview: Using SOPOvw
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY [SONumber], [SOLineNo]
	,ISNULL([ProductDisplaySequence],0)
	,[SOSubLineNo],[POLineNo]) AS [Id]
      ,[SONumber]
      ,[SODate]
      ,[CustomerName]	
      ,[CustomerPO]
      ,[StartDate]
      ,[EndDate]
      ,[SOLineNo]
      ,[SOSubLineNo]
	  ,CAST(ISNULL([SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]

      ,ISNULL([ProductDisplaySequence], 0) AS [ProductDisplaySequence]
	  
      ,ISNULL([ProgramName], '') AS [ProgramName]
      ,ISNULL([ProductTypeName], '') AS [SOSubLineType]
      ,ISNULL([ProductNo], '') AS [ProductNo]
      ,ISNULL([ProductName], '') AS [ProductName]

      ,ISNULL([SOQty], 0) AS [SOQty]
      ,ISNULL([Cost], 0.000000) AS [Cost]
      ,ISNULL([Price], 0.000000) AS [Price]
      ,ISNULL([VendorPO], '') AS [PONumber]
      ,ISNULL([PODate], '1900-01-01') AS [PODate]
      ,ISNULL([VendorName], '') AS [VendorName]
      ,ISNULL([POLineNo], 0) AS [POLineNo]
      ,ISNULL([POSubLineNo], 0) AS [POSubLineNo]
	  ,CAST(ISNULL([POLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([POSubLineNo], 0) AS VARCHAR(4))  AS [POLineNoExt]
      ,ISNULL([POQty], 0) AS [POQty]
      ,ISNULL([POHeaderId], 0) AS [POHeaderId]
      ,ISNULL([PODetailId], 0) AS [PODetailId]
      ,ISNULL([ForProductNo], '') AS [ForProductNo]

FROM [PIMS].[dbo].[SOPOvw]
ORDER BY [SONumber], [SOLineNo]
	,ISNULL([ProductDisplaySequence],0)
	,[SOSubLineNo],[POLineNo]  ";

            return await GetSqlData<PODetailData>(query);
        }
        //"Overview"
        public async Task<List<POData>> GetPOData()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY poh.[PODate]) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  --,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]
	  --,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  
  FROM [PIMS].[dbo].[POHeader] poh
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.SoHeaderId = soh.[SOHeaderId]
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON vendor.CountryCodeId = country.LookupDetailId
  ORDER BY poh.[PODate]
 ";
            return await GetSqlData<POData>(query);
        }
        //"Overview"
        public async Task<List<POData>> GetPOData(string sono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY poh.[PODate]) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  
  FROM [PIMS].[dbo].[POHeader] poh
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.SoHeaderId = soh.[SOHeaderId]
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON vendor.CountryCodeId = country.LookupDetailId
  WHERE soh.[sONumber] = '{0}'
  ORDER BY poh.[PODate]
 ";
            string fullQuery = string.Format(query, sono);
            return await GetSqlData<POData>(fullQuery);
        }
        //"Overview"
        public async Task<List<PODetailData>> GetPODetailData(string pono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY pod.[POLineNo], sod.[SOLineNo],sod.[SOSubLineTypeId]) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  
      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]
      ,ISNULL(pod.[POSubLineTypeId], 0) AS [POSubLineTypeId]
      ,ISNULL(pod.[ProductId], 0) AS [ProductId]

	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [BusinessPartnerId] 
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  

	  ,ISNULL(sod.[SOLineNo], 0) AS [SOLineNo]
      ,ISNULL(sod.[SOSubLineNo], 0) AS [SOSubLineNo]
      ,ISNULL(sod.[SOSubLineTypeId], 0) AS [SOSubLineTypeId]
      ,CASE WHEN sod.[SOSubLineTypeId] = 1 THEN 'Factory' 
		WHEN sod.[SOSubLineTypeId] = 2 THEN 'Packaging' 
		ELSE '' END AS [SOSubLineType]

  FROM [PIMS].[dbo].[PODetail] pod
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.[SOHeaderId] = poh.SoHeaderId		
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sod.[SODetailId] = pod.[SODetailId]	
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  WHERE poh.[PONumber] = '{0}'
  ORDER BY pod.[POLineNo],sod.[SOLineNo],sod.[SOSubLineTypeId] ";

            string fullQuery = string.Format(query, pono);
            return await GetSqlData<PODetailData>(fullQuery);
        }
        //"Overview"
        public async Task<List<PODetailData>> GetBOMData(string sono, int lineno)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [SOLineNo],[SOSubLineNo]) AS [Id]
      ,[SONumber]
      ,[SODate]
      ,[CustomerName]	
      ,[CustomerPO]
      ,[StartDate]
      ,[EndDate]
      ,[SOLineNo]
      ,[SOSubLineNo]
	  ,CAST(ISNULL([SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL([SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]
      ,[ProgramName]
      ,[ProductTypeName] AS [SOSubLineType]
      ,[ProductNo]
      ,[ProductName]
      ,[SOQty]
      ,[Cost]
      ,[Price]
      ,ISNULL([VendorPO], '') AS [VendorPO]
      ,ISNULL([PODate], '1900-01-01') AS [PODate]
      ,ISNULL([VendorName], '') AS [VendorName]
      ,ISNULL([POLineNo], 0) AS [POLineNo]
      ,ISNULL([POQty], 0) AS [POQty]
      ,ISNULL([POHeaderId], 0) AS [POHeaderId]
      ,ISNULL([PODetailId], 0) AS [PODetailId]
  FROM [PIMS].[dbo].[SOPOvw]
  WHERE SoNumber= '{0}' 
	AND [SOLineNo] = {1}
  ORDER BY [SOLineNo],[SOSubLineNo]";

            string fullQuery = string.Format(query, sono, lineno);
            return await GetSqlData<PODetailData>(fullQuery);
        }
        //"Overview" --NOT WORKING
        public async Task<List<CommentsData>> GetCommentsData(int poDetailId)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY qc.[CreatedOn]) AS [Id]
      ,qc.[QualityControlId]
      ,qc.[QualityControlTypeId]
      ,qc.[ProductId]
      ,qc.[PODetailID]
      ,qc.[QualityControlStatus]

	  ,0 AS [CommentaryId]
      ,0 AS [CommentaryTypeId]
      ,'' AS [Comments]

      ,'' AS [SamplesApproval]
      ,'' AS [DisneyStatus]
      ,'' AS [ImageApproval]

      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
  FROM [PIMS].[dbo].[QualityControl] qc
	--LEFT JOIN [PIMS].[dbo].[Commentary] com
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON qc.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]   
  WHERE pod.[PODetailId] = {0} ";

            string fullQuery = string.Format(query, poDetailId);
            return await GetSqlData<CommentsData>(fullQuery);
        }
        //"Overview"
        public async Task<List<FreightData>> GetFreightData(int poDetailId)
        {
            string query = @"
SELECT sh.[ShipmentHeaderId] AS [Id]

      ,sh.[ShipMethodId]
	  ,ISNULL(shipping.[LookupDescription], '') AS [ShipMethod]

      ,sh.[InvoiceNo]	  
      ,sd.[ShipmentQty]
      ,sh.[TrackingNumber] AS [Tracking]
      ,sh.[ShipmentDate]
      ,sh.[ShipToETA]

  FROM [PIMS].[dbo].[ShipmentHeader] sh
	LEFT JOIN [PIMS].[dbo].[ShipmentDetails] sd
		ON sh.[ShipmentHeaderId] = sd.ShipmentHeaderId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] shipping
		ON sh.[ShipMethodId] = shipping.LookupDetailId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sd.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]  
  WHERE pod.[PODetailId] = {0}
  ORDER BY [ShipToETA] ASC ";

            string fullQuery = string.Format(query, poDetailId);
            return await GetSqlData<FreightData>(fullQuery);
        }
        //"Overview"
        public async Task<List<CostData>> GetCostData(int poDetailId)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ISNULL(pod.[POLineNo], 0) ) AS [Id]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]

      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]

	  ,ISNULL(prod.[ProductId], 0) AS [ProductId] 
	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  	  
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
	  ,ISNULL(pod.[Cost], 0.0) AS [FirstCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [JewelryCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [PackagingCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [TotalCost]
	  ,ISNULL(sod.[Price], 0.0) AS [Price]
	  ,ISNULL(sod.[Price], 0.0) * ISNULL(pod.[OrderQty], 0) AS [SellAmount]

	  ,ISNULL(hts.[HTSCode], '') AS [HTSCode]
	  ,ISNULL(hts.[HTSAmount], 0.0) AS [DutyPercent]
	  ,ISNULL(hts.[HTSAmount], 0.0) * ISNULL(pod.[OrderQty], 0) * ISNULL(pod.[Cost], 0) AS [DutyAmount]
	  
	  ,0.0 AS [LaborFreight]
	  ,0.0 AS [LaborAmount]
	  ,0.0 AS [DisneyRoyalty]
	  ,0.0 AS [TotalCostLanded]
	  ,0.0 AS [COGPercent]

  FROM [PIMS].[dbo].[PODetail] pod
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.[SOHeaderId] = poh.SoHeaderId		
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sod.[SODetailId] = pod.[SODetailId]	
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[ProductAttributes] attribute
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[ProductHTS] prodhts
		ON prodhts.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[HTS] hts
		ON hts.HTSId = prodhts.HTSID
  WHERE pod.[PODetailId] = {0}
  ORDER BY ISNULL(pod.[POLineNo], 0)";

            string fullQuery = string.Format(query, poDetailId);
            return await GetSqlData<CostData>(fullQuery);
        }

        //"EDI Orders"
        public async Task<List<EdiOrderDetailData>> GetEdiOrderSummaryViewData() // [EdiOrderSummaryVw]
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]) AS [Id]
      ,ediView.[Tradpartid]	AS [TradingPartnerId]
      ,ISNULL(bizPartner.BusinessPartnerCode, '') AS [TradingPartnerCode]
      ,ISNULL(bizPartner.BusinessPartnerName, '') AS [TradingPartnerName]
      ,ediView.[Ponum]		AS [PONumber]
      ,ediView.[StartDate]	AS [ShipDate]
      ,ediView.[CancelDate]	AS [CancelDate]
      ,ediView.[ShipYear]	AS [ShipYear]
      ,ediView.[ShipMonth]	AS [ShipMonth]
      ,ediView.[ShipWeek]	AS [ShipWeek]
	  ,CAST(DATEADD(wk, DATEDIFF(wk,0,ediView.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
      ,ediView.[NoOfItems]	AS [ItemsCount]
      ,ediView.[Ord_qty]	AS [SOQty]
      ,ediView.[ExtPrice]	AS [ExtPrice]
  FROM [PIMS].[edi].[EdiOrderSummaryVw] ediView
	LEFT JOIN [dbo].[BusinessPartner] bizPartner
		ON bizPartner.EDITradPartId = ediView.[Tradpartid]
  ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]";
            return await GetSqlData<EdiOrderDetailData>(query);
        }
        public async Task<List<EdiOrderDetailData>> SearchEdiOrderSummaryViewData(string poNo) // [EdiOrderSummaryVw]
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]) AS [Id]
      ,ediView.[Tradpartid]	AS [TradingPartnerId]
      ,ISNULL(bizPartner.BusinessPartnerCode, '') AS [TradingPartnerCode]
      ,ISNULL(bizPartner.BusinessPartnerName, '') AS [TradingPartnerName]
      ,ediView.[Ponum]		AS [PONumber]
      ,ediView.[StartDate]	AS [ShipDate]
      ,ediView.[CancelDate]	AS [CancelDate]
      ,ediView.[ShipYear]	AS [ShipYear]
      ,ediView.[ShipMonth]	AS [ShipMonth]
      ,ediView.[ShipWeek]	AS [ShipWeek]
	  ,CAST(DATEADD(wk, DATEDIFF(wk,0,ediView.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
      ,ediView.[NoOfItems]	AS [ItemsCount]
      ,ediView.[Ord_qty]	AS [SOQty]
      ,ediView.[ExtPrice]	AS [ExtPrice]
  FROM [PIMS].[edi].[EdiOrderSummaryVw] ediView
	LEFT JOIN [dbo].[BusinessPartner] bizPartner
		ON bizPartner.EDITradPartId = ediView.[Tradpartid]
  WHERE ediView.[Ponum] LIKE '%{0}%'
  ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]";

            string fullQuery = string.Format(query, poNo);
            return await GetSqlData<EdiOrderDetailData>(fullQuery);
        }
        //"EDI Orders", "Orders"
        public async Task<List<EdiOrderDetailData>> GetEdiOrderDetailViewData(string poNo) // [EdiOrderDetailvw]
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum],ediView.[Item]) AS [Id]
    ,ediView.[Tradpartid]	AS [TradingPartnerId]
    ,ISNULL(bizPartner.BusinessPartnerCode, ISNULL(bizPartner2.BusinessPartnerCode, '')) AS [TradingPartnerCode]
    ,ISNULL(bizPartner.BusinessPartnerName, ISNULL(bizPartner2.BusinessPartnerName, '')) AS [TradingPartnerName]
    ,ediView.[Ponum]		AS [PONumber]
    ,ediView.[Podte]		AS [PODate]
    ,ediView.[PoStat]		AS [POStatus]
	,ISNULL(prod.[ProductId],0)   AS [ProductId]
    ,ediView.[Item]			AS [ProductNo]
    ,ediView.[Ord_qty]		AS [SOQty]
    ,ediView.[Price]		AS [Price]
    ,ediView.[ExtPrice]		AS [ExtPrice]
    ,ediView.[Tran_type]	AS [TransactionType]
    ,ediView.[StartDate]	AS [ShipDate]
    ,ediView.[CancelDate]	AS [CancelDate]
    ,ediView.[ShipYear]		AS [ShipYear]
    ,ediView.[ShipMonth]	AS [ShipMonth]
    ,ediView.[ShipWeek]		AS [ShipWeek]
	,CAST(DATEADD(wk, DATEDIFF(wk,0,ediView.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]	
	,ediView.[Edihdrid]		AS [EdiHdrId]
	,ediView.[Editrnid]		AS [EdiTrnId]
	,ediView.[CustTPID]		AS [CustTPId]
	,ediView.[Potype]		AS [EDIPOType]
	,ediView.[TP_Name]		AS [EDITradingPartnerName]
	,ISNULL(ediView.[SoHeaderId], 0)	AS [SoHeaderId]
	,ISNULL(ediView.[SoDetailId], 0)	AS [SoDetailId]
	,ISNULL(ediView.[ProductId], 0)		AS [EDIProductId]
FROM [PIMS].[edi].[EdiOrderDetailvw] ediView
    LEFT JOIN [dbo].[BusinessPartner] bizPartner
	    ON bizPartner.EDITradPartId = ediView.[Tradpartid]
    LEFT JOIN [dbo].[BusinessPartner] bizPartner2
	    ON bizPartner2.BusinessPartnerName = ediView.[TP_Name]
    LEFT JOIN [PIMS].[dbo].[Product] prod
	    ON prod.[ProductNo] = ediView.[Item]
WHERE ediView.[Ponum] = '{0}'
ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]";

            string fullQuery = string.Format(query, poNo);
            return await GetSqlData<EdiOrderDetailData>(fullQuery);
        }

        //"Orders"
        public async Task<List<SoEdiData>> GetSoEdiData() //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY main.[SOShipYear] DESC
    ,main.[SOShipMonth] DESC
    ,main.[SOShipWeek] DESC
	,main.[CustomerName] ASC
	,main.[SONumber] DESC) AS [Id]

	,[IsLinked]
	,ISNULL([Edihdrid], 0) AS [Edihdrid]
	,[CustomerName]
	,[CustomerPO]
	,ISNULL([RefPonum], '') AS [RefPonum]
	,[SONumber]

	,CASE WHEN [IsLinked] = 0 THEN [SOShipYear]  ELSE [EDIShipYear] END AS [ShipYear]
    ,CASE WHEN [IsLinked] = 0 THEN [SOShipMonth] ELSE [EDIShipMonth] END AS [ShipMonth]
    ,CASE WHEN [IsLinked] = 0 THEN [SOShipWeek]  ELSE [EDIShipWeek] END AS [ShipWeek]
	,CASE WHEN [IsLinked] = 0 
		THEN CAST(DATEADD(wk, DATEDIFF(wk,0,[SO_StartDate]), 0) AS DATE)
		ELSE CAST(DATEADD(wk, DATEDIFF(wk,0,ISNULL([EDI_StartDate], '1900-01-01')), 0) AS DATE) END AS [MondayOfTheWeek]
		
	,CASE WHEN [IsLinked] = 0 
		THEN ISNULL([SO_StartDate], '1900-01-01')
		ELSE ISNULL([EDI_StartDate], '1900-01-01') END AS [StartDate]
	,CASE WHEN [IsLinked] = 0 
		THEN ISNULL([SO_EndDate], '1900-01-01')
		ELSE ISNULL([EDI_EndDate], '1900-01-01') END AS [EndDate]
	,CASE WHEN [IsLinked] = 0 
		THEN ISNULL([SOQty], 0)
		ELSE ISNULL([Ord_Qty], 0) END AS [OrderQty]

    ,[SOHeaderId]
	--,ISNULL([SOQty], 0)			AS [SOQty]
	--,ISNULL([SORetail], 0.0)	AS [SORetail]
	--,ISNULL([ShipmentQty], 0)	AS [SOShipmentQty]
	--,ISNULL([NoOfItems], 0)		AS [EDINumberOfItems]
	--,ISNULL([Ord_Qty], 0)		AS [EDIOrderQty]
	--,ISNULL([ExtPrice], 0.0)	AS [EDIExtPrice]
	--,ISNULL([Tradpartid], 0)	AS [EDITradingPartnerId]
	--,ISNULL([Podte], '1900-01-01')	AS [EDIPODate]
	--,ISNULL([Comments], '')		AS [EDIComments]
	,CASE WHEN [IsLinked] = 1 AND ([EDI_StartDate] IS NULL OR [EDI_EndDate] IS NULL) THEN 1 ELSE 0 END AS [EDIDateIssue]
FROM (
SELECT DISTINCT 
	ISNULL(main.[CustomerName], '')	 AS [CustomerName]
    ,ISNULL(ediH.Ponum,main.[CustomerPO]) AS [CustomerPO]
	,CASE WHEN ediH.Ponum IS NULL THEN 0 ELSE 1 END AS [IsLinked]
    ,ediH.[Ponum]
	,ediH.[RefPonum]
    ,main.[SONumber]
    ,main.[StartDate]	AS [SO_StartDate]
    ,main.[EndDate]		AS [SO_EndDate]
    ,main.[SOHeaderId]
    ,main.[SOShipYear]
    ,main.[SOShipMonth]
    ,main.[SOShipWeek]
	,CAST(DATEADD(wk, DATEDIFF(wk,0,main.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
    ,main.[SOQty]
    ,main.[SORetail]
    ,ISNULL(main.[ShipmentQty], 0) AS [ShipmentQty]
	,ediH.[Edihdrid]
	,ediO.[Tradpartid]
    ,ediO.[Podte]
    ,ediO.[Comments]
    ,CASE WHEN ediO.[StartDate]= '1900-01-01' THEN NULL ELSE ediO.[StartDate] END AS [EDI_StartDate]
    ,CASE WHEN ediO.[CancelDate] = '1900-01-01' THEN NULL ELSE ediO.[CancelDate] END AS [EDI_EndDate]
    ,ediO.[ShipYear]  AS [EDIShipYear]
    ,ediO.[ShipMonth] AS [EDIShipMonth]
    ,ediO.[ShipWeek]  AS [EDIShipWeek]
    ,ediO.[NoOfItems]
    ,ediO.[Ord_Qty]
    ,ediO.[ExtPrice]
FROM [PIMS].[dbo].[CustomerSoSummaryVw] main -- SELECT * FROM [PIMS].[dbo].[CustomerSoSummaryVw] 
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sod.[SOHeaderId] = main.[SOHeaderId]
	LEFT JOIN [PIMS].[edi].[EdiHdr]	ediH
		ON main.SOHeaderId = ediH.SoHeaderId
	LEFT JOIN [PIMS].[edi].[EdiTrn]	ediD
		ON ediD.[Edihdrid] = ediH.[Edihdrid] 
			AND ediD.[SoDetailId] = sod.[SoDetailId]
	LEFT JOIN [PIMS].[edi].[EdiOrderSummaryVw] ediO
		ON ediO.[Ponum] = ediH.[Ponum] AND ediO.[SoHeaderId] = main.[SoHeaderId] 
) main
ORDER BY [SOShipYear] DESC
    ,[SOShipMonth] DESC
    ,[SOShipWeek] DESC
	,[CustomerName] ASC
	,[SONumber] DESC
	,[EDI_EndDate] DESC";

            return await GetSqlData<SoEdiData>(query);
        }
        //"Orders"
        public async Task<List<SoEdiData>> GetSoEdiDetailData(int ediId) //
        {
            string query = @"
SELECT edi.[Edihdrid]
    ,edi.[Editrnid]
	,edi.[Ponum]    AS [CustomerPO]
    ,edi.[Item]		AS [ItemNo]
    ,edi.[Ord_qty]	AS [OrderQty]
    ,edi.[Price]
	,edi.[TP_Name]	AS [CustomerName]
    ,ISNULL(edi.[StartDate], '1900-01-01')	AS [StartDate]
    ,ISNULL(edi.[CancelDate], '1900-01-01')	AS [EndDate]
    ,ISNULL(edi.[SoDetailId], 0)	AS [SoDetailId]
    ,ISNULL(edi.[ProductId], 0)		AS [ProductId]
	,ISNULL(soh.[SONumber], '')		AS [SONumber]
	,ISNULL(sod.[ForSoDetailId], 0) AS [ForSoDetailId]
	,ISNULL(sod.[OrderQty], 0)		AS [SOOrderQty]
	,ISNULL(sod.[Cost], 0.0)		AS [Cost]
	,ISNULL(prod.[SKU], '')			AS [SKU]
	,ISNULL(prod.[ProductName], '') AS [Description]
	--,prod.[ProductFullDescription] AS [Description]
FROM [PIMS].[edi].[EdiOrderDetailvw] edi
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON edi.[SoDetailId] = sod.[SODetailId]
			AND edi.[ProductId] = sod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.[SOHeaderId] = sod.[SOHeaderId]
	--LEFT JOIN [PIMS].[dbo].[PODetail] pod
	--	ON sod.[SODetailId] = pod.[SODetailId]
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON prod.[ProductId] = sod.[ProductId]
WHERE edi.[Edihdrid] = {0}";

            string fullQuery = string.Format(query, ediId);

            return await GetSqlData<SoEdiData>(fullQuery);
        }

        //"I2 Orders", "Orders"
        public async Task<List<CustomerSoPoData>> GetCustomerSoData() // CustomerSoPoData
        {
            string query = @"
            SELECT ROW_NUMBER() OVER (ORDER BY main.[SOShipYear] DESC
                ,main.[SOShipMonth] DESC
                ,main.[SOShipWeek] DESC
            	,main.[CustomerName] ASC
            	,main.[SONumber] DESC) AS [Id]
                ,ISNULL(main.[CustomerName], '')	 AS [CustomerName]
                ,ISNULL(edi.Ponum,main.[CustomerPO]) AS [CustomerPO]
                ,main.[SONumber]
                ,main.[StartDate]
                ,main.[EndDate]
                ,main.[SOHeaderId]
                ,main.[SOShipYear]
                ,main.[SOShipMonth]
                ,main.[SOShipWeek]
            	,CAST(DATEADD(wk, DATEDIFF(wk,0,[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
                ,main.[SOQty]
                ,main.[SORetail]
                ,ISNULL(main.[ShipmentQty], 0) AS [ShipmentQty]
            	,ISNULL(det.POList, '') AS [PONumber]
            FROM [PIMS].[dbo].[CustomerSoSummaryVw] main
            	LEFT JOIN [PIMS].[edi].[EdiHdr]	edi
            		ON main.SOHeaderId = edi.SoHeaderId
            LEFT JOIN (SELECT top_query.[SOShipYear] AS [ListSOShipYear],top_query.[SOShipWeek] AS [ListSOShipWeek], top_query.[SONumber],
            				(SELECT STUFF([list],1,1,'') AS stuff_list
            					FROM (SELECT ',' + CAST(vw.[VendorPO] AS VARCHAR(255)) AS [text()]
            							FROM [PIMS].[dbo].[CustomerSoPoDetailVw] vw 
            							LEFT JOIN [PIMS].[dbo].[SODetailMaterial] som
            								ON vw.[SODetailMaterialId] = som.[SODetailMaterialId]
            							WHERE vw.[SOShipYear] = top_query.[SOShipYear] 
            								AND vw.[SOShipWeek] = top_query.[SOShipWeek] 
            								AND som.[SoSubLineType] <> 'Packaging'
            								AND vw.[SONumber] = top_query.[SONumber] 
            							FOR XML PATH('')
            							) sub_query([list])
            					) AS POList
            			FROM [PIMS].[dbo].[CustomerSoPoSummaryVw] top_query
            			GROUP BY top_query.[SOShipYear], top_query.[SOShipWeek], top_query.[SONumber]) det
            	ON main.[SOShipYear] = det.[ListSOShipYear] 
            		AND main.[SOShipWeek] = det.[ListSOShipWeek]
            		AND main.[SONumber] = det.[SONumber]
            ORDER BY main.[SOShipYear] DESC
                ,main.[SOShipMonth] DESC
                ,main.[SOShipWeek] DESC
            	,main.[CustomerName] ASC
            	,main.[SONumber] DESC";
                       
            return await GetSqlData<CustomerSoPoData>(query);
        }
        public async Task<List<CustomerSoPoData>> GetCustomerSoData_old(bool populatePOListString = true) // CustomerSoPoData
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY so.[SOShipYear] DESC
    ,so.[SOShipMonth] DESC
    ,so.[SOShipWeek] DESC
	,so.[CustomerName] ASC
	,so.[SONumber] DESC) AS [Id]
    ,ISNULL(so.[CustomerName], '')	AS [CustomerName]
    ,ISNULL(edi.Ponum,so.[CustomerPO]) AS [CustomerPO]
    ,so.[SONumber]
    ,so.[StartDate]
    ,so.[EndDate]
    ,so.[SOHeaderId]
    ,so.[SOShipYear]
    ,so.[SOShipMonth]
    ,so.[SOShipWeek]
	,CAST(DATEADD(wk, DATEDIFF(wk,0,so.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
    ,so.[SOQty]
    ,so.[SORetail]
    ,ISNULL(so.[ShipmentQty], 0) AS [ShipmentQty]
    ,ISNULL(so.[PONumbers], '') AS [PONumber]
FROM [PIMS].[dbo].[CustomerSoSummaryVw] so
	LEFT JOIN [PIMS].[edi].[EdiHdr]	edi
		ON so.SOHeaderId = edi.SoHeaderId
ORDER BY so.[SOShipYear] DESC
    ,so.[SOShipMonth] DESC
    ,so.[SOShipWeek] DESC
	,so.[CustomerName] ASC
	,so.[SONumber] DESC";

            if (populatePOListString)
            {
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY main.[SOShipYear] DESC
    ,main.[SOShipMonth] DESC
    ,main.[SOShipWeek] DESC
	,main.[CustomerName] ASC
	,main.[SONumber] DESC) AS [Id]
    ,ISNULL(main.[CustomerName], '')	 AS [CustomerName]
    ,ISNULL(edi.Ponum,main.[CustomerPO]) AS [CustomerPO]
    ,main.[SONumber]
    ,main.[StartDate]
    ,main.[EndDate]
    ,main.[SOHeaderId]
    ,main.[SOShipYear]
    ,main.[SOShipMonth]
    ,main.[SOShipWeek]
	,CAST(DATEADD(wk, DATEDIFF(wk,0,[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
    ,main.[SOQty]
    ,main.[SORetail]
    ,ISNULL(main.[ShipmentQty], 0) AS [ShipmentQty]
	,ISNULL(det.POList, '') AS [PONumber]
FROM [PIMS].[dbo].[CustomerSoSummaryVw] main
	LEFT JOIN [PIMS].[edi].[EdiHdr]	edi
		ON main.SOHeaderId = edi.SoHeaderId
LEFT JOIN (SELECT top_query.[SOShipYear] AS [ListSOShipYear],top_query.[SOShipWeek] AS [ListSOShipWeek], 
          (SELECT STUFF([list],1,1,'') AS stuff_list
         FROM (SELECT ',' + CAST([VendorPO] AS VARCHAR(255)) AS [text()]
                  FROM [PIMS].[dbo].[CustomerSoPoSummaryVw] sub
                  WHERE sub.[SOShipYear] = top_query.[SOShipYear] AND sub.[SOShipWeek] = top_query.[SOShipWeek] --sub.column1 = top_query.column1
                  FOR XML PATH('')
                  ) sub_query([list])
              ) AS POList
FROM  [PIMS].[dbo].[CustomerSoPoSummaryVw] top_query
GROUP BY top_query.[SOShipYear],top_query.[SOShipWeek]) det
	ON main.[SOShipYear] = det.[ListSOShipYear] AND main.[SOShipWeek] = det.[ListSOShipWeek]
ORDER BY main.[SOShipYear] DESC
    ,main.[SOShipMonth] DESC
    ,main.[SOShipWeek] DESC
	,main.[CustomerName] ASC
	,main.[SONumber] DESC";
            }

            return await GetSqlData<CustomerSoPoData>(query);
        }
        //"I2 Orders"
        public async Task<List<CustomerSoPoData>> GetCustomerSoPoSummaryData(int soShipYear, int soShipWeek, bool populatePOListString = true)
        {
            string fullQuery = "";
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [CustomerName],[SONumber]) AS [Id]
      ,[CustomerName]
      ,[CustomerPO]
      ,[SONumber]
      ,[VendorPO]
      ,[StartDate]
      ,ISNULL([TrackingNumber], '') AS [TrackingNumber]
      ,[EndDate]
      ,[SOHeaderId]
      ,[SOShipYear]
      ,[SOShipMonth]
      ,[SOShipWeek]
	  ,CAST(DATEADD(wk, DATEDIFF(wk,0,[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
      ,[SOQty]
      ,[SORetail]
      ,ISNULL([ShipmentQty], 0) AS [ShipmentQty]
  FROM [PIMS].[dbo].[CustomerSoPoSummaryVw]";

            if (populatePOListString)
            {
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY main.[CustomerName]) AS [Id]
      ,main.[CustomerName]
      ,main.[CustomerPO]
      ,main.[SONumber]
      ,main.[StartDate]
      ,main.[EndDate]
      ,main.[SOHeaderId]
      ,main.[SOShipYear]
      ,main.[SOShipMonth]
      ,main.[SOShipWeek]
      ,main.[SOQty]
      ,main.[SORetail]
      ,ISNULL(main.[ShipmentQty], 0) AS [ShipmentQty]
	  ,ISNULL(det.POList, '') AS [PONumber]
FROM [PIMS].[dbo].[CustomerSoSummaryVw] main
LEFT JOIN (SELECT top_query.[SOShipYear] AS [ListSOShipYear],top_query.[SOShipWeek] AS [ListSOShipWeek], 
          (SELECT STUFF([list],1,1,'') AS stuff_list
         FROM (SELECT ',' + CAST([VendorPO] AS VARCHAR(255)) AS [text()]
                  FROM [PIMS].[dbo].[CustomerSoPoSummaryVw] sub
                  WHERE sub.[SOShipYear] = top_query.[SOShipYear] AND sub.[SOShipWeek] = top_query.[SOShipWeek] --sub.column1 = top_query.column1
                  FOR XML PATH('')
                  ) sub_query([list])
              ) AS POList
FROM  [PIMS].[dbo].[CustomerSoPoSummaryVw] top_query
GROUP BY top_query.[SOShipYear],top_query.[SOShipWeek]) det
	ON main.[SOShipYear] = det.[ListSOShipYear] AND main.[SOShipWeek] = det.[ListSOShipWeek]";
            }

            if (soShipYear > 0 && soShipWeek > 0)
            {
                query += @"
WHERE [soShipYear] = {0} AND [soShipWeek] = {1}";

                fullQuery = string.Format(query, soShipYear, soShipWeek) + @"
ORDER BY [soShipYear], [soShipWeek]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY [soShipYear], [soShipWeek]";
            }

            return await GetSqlData<CustomerSoPoData>(fullQuery);
        }
        //"I2 Orders", "Orders"
        public async Task<List<CustomerSoPoData>> GetCustomerSoPoDetailData(int soHeaderId, bool includePackaging = true)
        {
            string fullQuery = "";
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY vw.[CustomerName],vw.[SONumber],vw.[SOLineNo]) AS [Id]
      ,ISNULL(vw.[CustomerName], '')		AS [CustomerName]
      ,ISNULL(edi.[Ponum],vw.[CustomerPO])	AS [CustomerPO]
      ,vw.[SONumber]
      ,ISNULL(vw.[ProgramName], '')			AS [ProgramName]
      ,vw.[SOHeaderId]
      ,vw.[SODetailId]
      ,vw.[SODate]
      ,vw.[StartDate]
      ,vw.[EndDate]
      ,vw.[SOLineNo]
      ,vw.[ProductId]
      ,vw.[ProductNo]
      ,vw.[ProductName]
      ,vw.[SOQty]
      ,vw.[Cost]
      ,vw.[Price]
      ,vw.[VendorPO]
      ,vw.[VendorName]
      ,vw.[POQty]
      ,ISNULL(vw.[ShipmentDate], '1900-01-01') AS [ShipmentDate]
      ,ISNULL(vw.[TrackingNumber], '') AS [TrackingNumber]
      ,ISNULL(vw.[ShipToETA], '1900-01-01') AS [ShipToETA]
      ,ISNULL(vw.[ShipmentQty], 0) AS [ShipmentQty]
      ,vw.[SOShipYear]
      ,vw.[SOShipMonth]
      ,vw.[SOShipWeek]
	  ,CAST(DATEADD(wk, DATEDIFF(wk,0,vw.[StartDate]), 0) AS DATE) AS [MondayOfTheWeek]
	  ,som.[SoSubLineTypeId]
	  ,som.[SoSubLineType]
	  ,ISNULL(edidetail.[Edihdrid], 0)	AS [EdiHdrId]
	  ,ISNULL(edidetail.[Editrnid], 0)	AS [EdiTrnId]
	  ,ISNULL(edi.[Ponum], '')			AS [EDIPONumber]
	  ,ISNULL(edi.[RefPoNum], '')		AS [EDIRefPONumber]
FROM [PIMS].[dbo].[CustomerSoPoDetailVw] vw 
	LEFT JOIN [PIMS].[dbo].[SODetailMaterial] som
		ON vw.[SODetailMaterialId] = som.[SODetailMaterialId]
	LEFT JOIN edi.[EdiTrn] edidetail
		ON edidetail.[SoDetailId] = vw.SODetailId AND edidetail.[ProductId] = vw.[ProductId]
	LEFT JOIN [edi].[EdiHdr] edi
		ON edi.[Edihdrid] = edidetail.[Edihdrid]";

            fullQuery = query;
            if (soHeaderId > 0 || !includePackaging)
            {
                if (soHeaderId > 0)
                {
                    fullQuery += string.Format(@"
WHERE vw.[SOHeaderId] = {0}", soHeaderId);
                    if (!includePackaging) fullQuery += @"
    AND som.[SoSubLineType] <> 'Packaging' ";
                }
                else
                {
                    fullQuery += @"WHERE som.[SoSubLineType] <> 'Packaging'"; 
                }
            }

            fullQuery = fullQuery + @"
ORDER BY vw.[SONumber], vw.[SOLineNo]";

            return await GetSqlData<CustomerSoPoData>(fullQuery);
        }

        //"Shipments"
        public async Task<List<ShippingData>> GetShippingDetailData() // Shipping
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [ShipmentHeaderId],[ShipmentDetailId]) AS [Id]
      ,sDetail.[ShipmentHeaderId]
      ,sDetail.[ShipmentDate]
      ,sDetail.[InvoiceNo]
      ,sDetail.[TrackingNumber]
      ,sDetail.[ShipToETA]
      ,sDetail.[BusinessPartnerName] AS [VendorName]	  
      ,sDetail.[PONumber]
      ,sDetail.[ProductNo]
      ,sDetail.[ProductName]
      --,sDetail.[OrderQty]
	  ,sDetail.[Qty] AS	[OrderQty]
      ,sDetail.[ShipmentQty]
      ,sDetail.[ShipmentDetailId]
      ,sDetail.[PODetailId]
      ,sDetail.[ProductId]
	  ,poh.POHeaderId
	  ,soh.SOHeaderId
	  	  
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [VendorId] 
	  --,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]	  
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [CustomerId] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

      ,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  

  FROM [PIMS].[dbo].[ShipmentDetailvw] sDetail 
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON pod.PODetailId = sDetail.[PODetailId]
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.SOHeaderId = poh.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId";

            return await GetSqlData<ShippingData>(query);
        }
        //"Shipments"
        public async Task<List<POOpenVendor>> GetPOOpenVendorData() // [POOpenDetailVw]
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY [BusinessPartnerName]) AS [Id]
    ,[BusinessPartnerId] AS [VendorId]
    ,[BusinessPartnerName] AS [VendorName]
    ,[OpenPOs]
FROM [PIMS].[dbo].[POOpenBusinessPartnerVw]
ORDER BY [BusinessPartnerName]";
            return await GetSqlData<POOpenVendor>(query);
        }
        //"Shipments"
        public async Task<List<ShippingData>> GetOpenPOShipmentData(string poNo = "") 
        {
            string fullQuery = "";
            string query = @"
--POOpenDetail
SELECT ROW_NUMBER() OVER (ORDER BY openpo.[PONumber], ISNULL(shipment.[ShipmentDetailId], 0), openpo.[ProductNo]) AS [Id]
      ,openpo.[BusinessPartnerId] AS [VendorId]
      ,openpo.[BusinessPartnerName] AS [VendorName]
      ,ISNULL(openpo.[ProgramName], '') AS [ProgramName]
      ,openpo.[POHeaderId]
      ,openpo.[PONumber]	--
      ,openpo.[PODetailId]	--
      ,openpo.[ProductNo]	--
      ,openpo.[ProductName]	--
      ,openpo.[OrderQty]	--
--ShippingData
      ,ISNULL(shipment.[ShipmentHeaderId], 0)           AS [ShipmentHeaderId]
      ,ISNULL(shipment.[ShipmentDate], '1900-01-01')    AS [ShipmentDate]
      ,ISNULL(shipment.[InvoiceNo], '')                 AS [InvoiceNo]
      ,ISNULL(shipment.[TrackingNumber], '')            AS [TrackingNumber]
      ,ISNULL(shipment.[ShipToETA], '1900-01-01')       AS [ShipToETA]
      ,ISNULL(shipment.[BusinessPartnerName], '')       AS [BusinessPartnerName]
      --,shipment.[PONumber]
      --,shipment.[ProductNo]
      --,shipment.[ProductName]
      --,shipment.[OrderQty]
      ,ISNULL(shipment.[ShipmentQty], 0)    AS [ShipmentQty]
      ,ISNULL(shipment.[ShipmentDetailId], 0) AS [ShipmentDetailId]
      --,shipment.[PODetailId]
      ,ISNULL(shipment.[ProductId], 0)      AS [ProductId]
	  ,ISNULL(sod.[SODetailId], 0)          AS [SODetailId]
	  ,ISNULL(sod.[ForSoDetailId], 0)       AS [ForSoDetailId]
	  ,ISNULL(forsod.ProductId, 0)          AS [ForProductId]
      ,ISNULL(forprod.[ProductNo], '')      AS [ForProductNo]
      --,ISNULL(forprod.[ProductName], '')  AS [ForProductName]
      ,ISNULL(shipment.[ShipmentQty], 0)    AS [LastShipmentQty]
FROM [PIMS].[dbo].[POOpenDetailVw] openpo
	LEFT JOIN [PIMS].[dbo].[ShipmentDetailVw] shipment
		ON openpo.[PODetailId] = shipment.[PODetailId]
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON openpo.[PODetailId] = pod.[PODetailId]
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON pod.[SODetailId] = sod.[SODetailId]
	LEFT JOIN [PIMS].[dbo].[SODetail] forsod
		ON sod.[SODetailId] = forsod.[SODetailId]
	LEFT JOIN [PIMS].[dbo].[Product] forprod 
		 ON forsod.ProductId = forprod.ProductId ";

            if (!string.IsNullOrWhiteSpace(poNo))
            {
                query += @"
WHERE openpo.[PONumber] = '{0}'";

                fullQuery = string.Format(query, poNo) + @"
ORDER BY openpo.[PONumber], ISNULL(shipment.[ShipmentDetailId], 0), openpo.[ProductNo]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY openpo.[PONumber], ISNULL(shipment.[ShipmentDetailId], 0), openpo.[ProductNo]";
            }

            return await GetSqlData<ShippingData>(fullQuery);
        }

        public async Task<List<POOpenDetail>> GetPOOpenDetailData(int vendorId = 0) // POOpenDetailvw
        {
            string fullQuery = "";
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [BusinessPartnerId] ASC, [PONumber], [ProductNo]) AS [Id]
    ,[BusinessPartnerId] AS [VendorId]
    ,ISNULL([BusinessPartnerName], '') AS [VendorName]
    ,ISNULL([ProgramName], '') AS [ProgramName]
    ,ISNULL([POHeaderId], 0) AS [POHeaderId]
    ,ISNULL([PONumber], '') AS [PONumber]
    ,ISNULL([PODetailId], 0) AS [PODetailId]
    ,ISNULL([ProductNo], '') AS [ProductNo]
    ,ISNULL([ProductName], '') AS [ProductName]
    ,ISNULL([OrderQty], 0) AS [OrderQty]
FROM [PIMS].[dbo].[POOpenDetailVw]";

            if (vendorId > 0)
            {
                query += @"
WHERE [BusinessPartnerId] = {0}";

                fullQuery = string.Format(query, vendorId) + @"
ORDER BY [PONumber], [ProductNo]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY [PONumber], [ProductNo]";
            }

            return await GetSqlData<POOpenDetail>(fullQuery);
        }

        //"Tasks Maint"
        public async Task<List<Tasks>> GetTasksList()
        {
            string query = @"SELECT [TaskId]
      ,ISNULL([TaskName], '')			AS [TaskName]
      ,ISNULL([TaskDescription], '')	AS [TaskDescription]
      ,ISNULL([TaskType], '')			AS [TaskType]

      ,ISNULL([TaskSequence], 0)		AS [TaskSequence]
      ,ISNULL([Required], 0)			AS [Required]
      ,ISNULL([Qty], 0)					AS [Qty]
      ,ISNULL([AssignedTo], '')			AS [AssignedTo]

      ,ISNULL([LegacySystemId], 0)		AS [LegacySystemId]

  FROM [PIMS].[dbo].[Task]
  WHERE [DeletedOn] IS NULL
  ORDER BY CASE WHEN [TaskSequence] = 0 THEN 1000 ELSE [TaskSequence] END
	--[TaskSequence]
	,[TaskName]";

            return await GetSqlData<Tasks>(query);
        }
        //"Tasks Maint"
        public async Task<List<BusinessPartnerTask>> GetAllBusinessPartnerTaskList()
        {
            string query = @"SELECT [BusinessPartnerTaskId]
      ,ISNULL([BusinessPartnerId], 0)   AS [BusinessPartnerId]

      ,ISNULL([TaskId], 0)              AS [TaskId]
      ,ISNULL([TaskName], '')			AS [TaskName]
      ,ISNULL([TaskDescription], '')	AS [TaskDescription]
      ,ISNULL([TaskType], '')			AS [TaskType]

      ,ISNULL([TaskSequence], 0)		AS [TaskSequence]
      ,ISNULL([Required], 0)			AS [Required]
      ,ISNULL([Qty], 0)					AS [Qty]
      ,ISNULL([AssignedTo], '')			AS [AssignedTo]

      ,ISNULL([LegacySystemId], 0) AS [LegacySystemId]

  FROM [PIMS].[dbo].[BusinessPartnerTask]
  ORDER BY CASE WHEN [TaskSequence] = 0 THEN 1000 ELSE [TaskSequence] END
	--[TaskSequence]
	,[TaskName]";

            return await GetSqlData<BusinessPartnerTask>(query);
        }
        //"Tasks Maint"
        public async Task<List<BusinessPartner>> GetCustomerBusinessPartnerList(int businessPartnerId = -1)
        {
            string query = @"SELECT [BusinessPartnerId]
      ,[BusinessPartnerType]
      ,LTRIM(RTRIM(ISNULL([BusinessPartnerCode],'')))	AS [BusinessPartnerCode]
      ,LTRIM(RTRIM(ISNULL([BusinessPartnerName],'')))	AS [BusinessPartnerName]
      ,LTRIM(RTRIM(ISNULL([DBAName],'')))				AS [DBAName]
      ,ISNULL([Address1],'')	AS [Address1]
      ,ISNULL([Address2],'')	AS [Address2]
      ,ISNULL([Address3],'')	AS [Address3]
      ,ISNULL([Address4],'')	AS [Address4]
      ,ISNULL([City],'')		AS [City]
      ,ISNULL([State],'')		AS [State]
      ,ISNULL([Zip],'')			AS [Zip]
      ,ISNULL([RowStatus],1)	AS [RowStatus]
FROM [PIMS].[dbo].[BusinessPartner]
WHERE [BusinessPartnerType] IS NOT NULL AND [DeletedOn] IS NULL
    AND UPPER([BusinessPartnerType]) = 'C'
	AND [BusinessPartnerName] <> '' AND [BusinessPartnerName] <> '---'";

            string fullQuery = "";
            if (businessPartnerId >= 0)
            {
                query += @"
	AND [BusinessPartnerId] = {0}";

                fullQuery = string.Format(query, businessPartnerId) + @"
ORDER BY [BusinessPartnerName]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY [BusinessPartnerName]";
            }

            return await GetSqlData<BusinessPartner>(fullQuery);
        }
        //"Tasks Maint" / "Tasks & Notes"
        public async Task<List<BusinessPartnerTask>> GetBusinessPartnerTaskList(int businessPartnerId)
        {
            string query = @"SELECT [BusinessPartnerTaskId]
      ,ISNULL([BusinessPartnerId], 0) AS [BusinessPartnerId]

      ,ISNULL([TaskId], 0) AS [TaskId]
      ,ISNULL([TaskName], '') AS [TaskName]
      ,ISNULL([TaskDescription], '') AS [TaskDescription]
      ,ISNULL([TaskType], '') AS [TaskType]

      ,ISNULL([TaskSequence], 0) AS [TaskSequence]
      ,ISNULL([Required], 0) AS [Required]
      ,ISNULL([Qty], 0) AS [Qty]
      ,ISNULL([AssignedTo], '') AS [AssignedTo]

      ,ISNULL([LegacySystemId], 0) AS [LegacySystemId]

  FROM [PIMS].[dbo].[BusinessPartnerTask]
  WHERE [BusinessPartnerId] = {0}
  --WHERE [DeletedOn] IS NULL
  ORDER BY [TaskSequence],[TaskId]";

            string fullQuery = string.Format(query, businessPartnerId);
            return await GetSqlData<BusinessPartnerTask>(fullQuery);
        }

        //"Product Tests"
        public async Task<List<ProductTest>> GetAllProductTests_old() //
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY result.[ProductNo]) AS [Id], *
FROM ( SELECT DISTINCT
      ISNULL(test.[ProductTestId], 0)	AS [ProductTestId]
      ,prod.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]
      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM (SELECT DISTINCT [ProductId]
  FROM [PIMS].[dbo].[SODetailMaterialVw]
  WHERE [SOSubLineNo] = 0) soitems
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON soitems.[ProductId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[ProductTest] test
		ON test.[ProductId] = prod.[ProductId]
) result
ORDER BY result.[ProductNo]
";
            return await GetSqlData<ProductTest>(fullQuery);
        }
        public async Task<List<ProductTest>> GetAllProductTests() //
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY result.[ProductNo]) AS [Id], *
FROM ( SELECT DISTINCT
      ISNULL(test.[ProductTestId], 0)	AS [ProductTestId]
      ,prod.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]

      ,ISNULL(test.[PODetailId], 0)		AS [PODetailId]
      ,ISNULL(test.[PONumber], '')		AS [PONumber]
      ,ISNULL(test.[Supplier], '')		AS [Supplier]

      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM [PIMS].[dbo].[ProductTest] test
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON test.[ProductId] = prod.[ProductId]
	LEFT JOIN (SELECT DISTINCT [ProductId]
  FROM [PIMS].[dbo].[SODetailMaterialVw]
  WHERE [SOSubLineNo] = 0) soitems
		ON test.[ProductId] = soitems.[ProductId]
) result
ORDER BY result.[ProductNo]
";
            return await GetSqlData<ProductTest>(fullQuery);
        }
        //"Product Tests"
        public async Task<List<ProductTest>> GetProductTests(int productId = 0) //
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY result.[ProductNo]) AS [Id], *
FROM ( 
SELECT test.[ProductTestId]
    ,test.[ProductId]
    ,ISNULL(test.[Qty], 0)			AS [Qty]
    ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
    ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
    ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')		AS [ReceivedDate]
    ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
    ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
    ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
    ,ISNULL(test.[FailedBy], '')	AS [FailedBy]
    ,ISNULL(test.[Comments], '')	AS [Comments]
    ,ISNULL(test.[Attachment], '')	AS [Attachment]
	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]

	,ISNULL(prod.[ProductNo], '')	AS [ProductNo]
	,ISNULL(prod.[ProductName], '')	AS [ProductName]
	,ISNULL(test.[PODetailId], 0)	AS [PODetailId]
	,ISNULL(test.[Amount], 0)		AS [Amount]
	,ISNULL(test.[InvoiceNo], '')	AS [InvoiceNo]	
	,ISNULL(test.[InvoiceDate], '1900-01-01 00:00:00')		AS [InvoiceDate]
	,ISNULL(test.[Supplier], '')	AS [Supplier]
	,ISNULL(test.[PONumber], '')	AS [PONumber]
	,ISNULL(test.[TestNumber], '')	AS [TestNumber]
	,ISNULL(test.[TestDocId], 0)	AS [TestDocId]
	,ISNULL(test.[TestDocCreated], '1900-01-01 00:00:00')	AS [TestDocCreated]
	,ISNULL(test.[TestType], '')	AS [TestType]
	,ISNULL(test.[Requested2], '1900-01-01 00:00:00')		AS [Requested2]
	,ISNULL(test.[QuoteReceived], '1900-01-01 00:00:00')	AS [QuoteReceived]
	,ISNULL(test.[QuoteApproved], '1900-01-01 00:00:00')	AS [QuoteApproved]
	,ISNULL(test.[SupplierAmount], 0)	AS [SupplierAmount]
	,ISNULL(test.[TestGroup], '')		AS [TestGroup]

	,ISNULL(poh.POHeaderId, 0)			AS [POHeaderId] 
	,ISNULL(soh.SOHeaderId, 0)			AS [SOHeaderId] 
	,ISNULL(poh.[BusinessPartnerId], 0) AS [VendorId] 
	--,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]	  
	,ISNULL(soh.[BusinessPartnerId], 0) AS [CustomerId] 
	,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

	,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]
	,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
	,ISNULL(poh.[EndDate], '1900-01-01')    AS [EndDate]
	,ISNULL(soh.[StartDate], '1900-01-01')  AS [StartDate]	  
	,ISNULL(soh.[SONumber], '')			AS [SONumber]  

FROM [PIMS].[dbo].[ProductTest] test
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON test.[ProductId] = prod.[ProductId]
		
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON pod.PODetailId = test.[PODetailId]
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.SOHeaderId = poh.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId

)result
";
            if (productId != 0)
            {
                fullQuery += @"
WHERE [ProductId] = " + productId;
            }

            return await GetSqlData<ProductTest>(fullQuery);
        }
        public async Task<List<ProductTest>> GetProductTests(int soHeaderId, List<int> productIdList) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY result.[ProductNo]) AS [Id], *
FROM ( 
SELECT test.[ProductTestId]
    ,test.[ProductId]
    ,ISNULL(test.[Qty], 0)			AS [Qty]
    ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
    ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
    ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')		AS [ReceivedDate]
    ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
    ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
    ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
    ,ISNULL(test.[FailedBy], '')	AS [FailedBy]
    ,ISNULL(test.[Comments], '')	AS [Comments]
    ,ISNULL(test.[Attachment], '')	AS [Attachment]
	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]

	,ISNULL(prod.[ProductNo], '')	AS [ProductNo]
	,ISNULL(prod.[ProductName], '')	AS [ProductName]
	,ISNULL(test.[PODetailId], 0)	AS [PODetailId]
	,ISNULL(test.[Amount], 0)		AS [Amount]
	,ISNULL(test.[InvoiceNo], '')	AS [InvoiceNo]	
	,ISNULL(test.[InvoiceDate], '1900-01-01 00:00:00')		AS [InvoiceDate]
	,ISNULL(test.[Supplier], '')	AS [Supplier]
	,ISNULL(test.[PONumber], '')	AS [PONumber]
	,ISNULL(test.[TestNumber], '')	AS [TestNumber]
	,ISNULL(test.[TestDocId], 0)	AS [TestDocId]
	,ISNULL(test.[TestDocCreated], '1900-01-01 00:00:00')	AS [TestDocCreated]
	,ISNULL(test.[TestType], '')	AS [TestType]
	,ISNULL(test.[Requested2], '1900-01-01 00:00:00')		AS [Requested2]
	,ISNULL(test.[QuoteReceived], '1900-01-01 00:00:00')	AS [QuoteReceived]
	,ISNULL(test.[QuoteApproved], '1900-01-01 00:00:00')	AS [QuoteApproved]
	,ISNULL(test.[SupplierAmount], 0)	AS [SupplierAmount]
	,ISNULL(test.[TestGroup], '')		AS [TestGroup]

	,ISNULL(poh.POHeaderId, 0)			AS [POHeaderId] 
	,ISNULL(soh.SOHeaderId, 0)			AS [SOHeaderId] 
	,ISNULL(poh.[BusinessPartnerId], 0) AS [VendorId] 
	--,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]	  
	,ISNULL(soh.[BusinessPartnerId], 0) AS [CustomerId] 
	,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

	,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]
	,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
	,ISNULL(poh.[EndDate], '1900-01-01')    AS [EndDate]
	,ISNULL(soh.[StartDate], '1900-01-01')  AS [StartDate]	  
	,ISNULL(soh.[SONumber], '')			AS [SONumber]  

FROM [PIMS].[dbo].[ProductTest] test
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON test.[ProductId] = prod.[ProductId]
		
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON pod.PODetailId = test.[PODetailId]
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.SOHeaderId = poh.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId

)result
--WHERE [SOHeaderId] = {0} --AND [ProductId] IN ({1})
";
            string fullQuery = "";
            //fullQuery = string.Format(query, soHeaderId, string.Join(",", productIdList));
            fullQuery = query + @"
WHERE [SoHeaderId] = " + soHeaderId;

            return await GetSqlData<ProductTest>(fullQuery);
        }

        //"Product Tasks"
        public async Task<List<TaskQueue>> GetAllProductQueueTasks(int soHeaderId = -1) 
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY 
	result.[ProductNo]) AS [Id]
	, *
FROM (
SELECT DISTINCT 0 AS [TaskQueueId]
      ,ISNULL(tq.[SoHeaderId], 0)			AS [SoHeaderId]
	  ,ISNULL(header.[SONumber], '')		AS [SONumber]
	  ,ISNULL(header.[SODate], '1900-01-01 00:00:00.000') AS [SODate]
	  ,ISNULL(header.[SOStatusId], 0)		AS [SOStatusId]
	  ,ISNULL(header.[SalesProgramId], 0)	AS [SalesProgramId]

      ,ISNULL(tq.[ProductId], 0)	AS [ProductId]
	  ,ISNULL(prod.[ProductNo], '')	AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')		AS [ProductName]
	  ,ISNULL(ptype.[ProductTypeName], '')	AS [ProductTypeName]
	  ,ISNULL(prod.[StyleNo], '')	AS [StyleNo]
	  ,ISNULL(prod.[SKU], '')		AS [SKU]

	  ,ISNULL(cust.[BusinessPartnerCode], '')	AS [CustomerCode]
	  ,ISNULL(cust.[BusinessPartnerName], '')	AS [CustomerName]

	  ,ISNULL(prodTasks.[TasksCount],headerTasks.[TasksCount]) AS [TasksCount]
FROM [PIMS].[dbo].[TaskQueue] tq
LEFT JOIN [PIMS].[dbo].[Product] prod
	ON tq.[ProductId] = prod.[ProductId]
LEFT JOIN [PIMS].[dbo].[Task] task
	ON tq.[TaskId] = task.[TaskId]
LEFT JOIN [PIMS].[dbo].[ProductType] ptype
	ON ptype.[ProductTypeId] = prod.[ProductTypeId]
LEFT JOIN [PIMS].[dbo].[SOHeader] header
	ON header.SOHeaderId = tq.[SoHeaderId]
LEFT JOIN [PIMS].[dbo].[BusinessPartner] AS cust
	ON cust.BusinessPartnerId = header.BusinessPartnerId
LEFT JOIN (SELECT [ProductId], COUNT(*) AS [TasksCount] FROM [TaskQueue] GROUP BY [ProductId]) prodTasks
	ON tq.[ProductId] = prodTasks.[ProductId]
LEFT JOIN (SELECT [SoHeaderId], COUNT(*) AS [TasksCount] FROM [TaskQueue] WHERE ISNULL([ProductId],'') = '' GROUP BY [SoHeaderId]) headerTasks
	ON tq.[SoHeaderId] = headerTasks.[SoHeaderId]
WHERE tq.[DeletedOn] IS NULL
)result";
            if (soHeaderId >= 0)
            {
                fullQuery += @"
WHERE [SoHeaderId] = " + soHeaderId;
            }

            return await GetSqlData<TaskQueue>(fullQuery);
        }
        //"Product Tasks" 
        public async Task<List<TaskQueue>> GetProductQueueTasks(int productId = -1, int soHeaderId = -1) 
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY 
	result.[ProductNo]
	,result.[TaskSequence]) AS [Id]
	, *
FROM (
SELECT tq.[TaskQueueId]
      ,ISNULL(tq.[SoHeaderId], 0)			AS [SoHeaderId]
	  ,ISNULL(header.[SONumber], '')		AS [SONumber]
	  ,ISNULL(header.[SODate], '1900-01-01 00:00:00.000') AS [SODate]
	  ,ISNULL(header.[SOStatusId], 0)		AS [SOStatusId]
	  ,ISNULL(header.[SalesProgramId], 0)	AS [SalesProgramId]

      ,ISNULL(tq.[ProductId], 0)	AS [ProductId]
	  ,ISNULL(prod.[ProductNo], '')	AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')		AS [ProductName]
	  ,ISNULL(ptype.[ProductTypeName], '')	AS [ProductTypeName]
	  ,ISNULL(prod.[StyleNo], '')	AS [StyleNo]
	  ,ISNULL(prod.[SKU], '')		AS [SKU]

      ,ISNULL(tq.[TaskId], 0)		AS [TaskId]
	  ,ISNULL(task.[TaskName], '')	AS [TaskName]
      ,ISNULL(tq.[TaskDescription], '')		AS [TaskDescription]
      ,ISNULL(tq.[Required], 0)		AS [Required]
      ,ISNULL(tq.[Qty], 0)			AS [Qty]
      ,ISNULL(tq.[AssignedTo], '')	AS [AssignedTo]
	  ,ISNULL(task.[TaskType], '')	AS [TaskType]
	  ,ISNULL(task.[TaskSequence], 0)	AS [TaskSequence]
	  
	  ,ISNULL(cust.[BusinessPartnerCode], '')	AS [CustomerCode]
	  ,ISNULL(cust.[BusinessPartnerName], '')	AS [CustomerName]

      ,ISNULL(tq.[TaskStatusId], 0)		AS [TaskStatusId]
      ,ISNULL(sts.[LookupDescription], '')		AS [TaskStatusName]
      ,ISNULL(tq.[TaskCompleted], 0)	AS [TaskCompleted]
      ,ISNULL(tq.[TaskNote], '')		AS [TaskNote]
      ,ISNULL(tq.[SODetailMaterialId], 0)	AS [SODetailMaterialId]
      ,ISNULL(tq.[SODetailId], 0)		AS [SODetailId]
      ,ISNULL(tq.[SOLineNo], 0)			AS [SOLineNo]
      ,ISNULL(tq.[SOSubLineNo], 0)		AS [SOSubLineNo]
      ,ISNULL(tq.[SoSubLineTypeId], 0)	AS [SoSubLineTypeId]
      ,ISNULL(tq.[SoSubLineType], '')	AS [SoSubLineType]
      ,ISNULL(tq.[LegacySystemId], 0)	AS [LegacySystemId]
      ,ISNULL(tq.[CreatedOn], '1900-01-01 00:00:00.000') AS [CreatedOn]
FROM [PIMS].[dbo].[TaskQueue] tq
LEFT JOIN [PIMS].[dbo].[Product] prod
	ON tq.[ProductId] = prod.[ProductId]
LEFT JOIN [PIMS].[dbo].[Task] task
	ON tq.[TaskId] = task.[TaskId]
LEFT JOIN [PIMS].[dbo].[ProductType] ptype
	ON ptype.[ProductTypeId] = prod.[ProductTypeId]
LEFT JOIN [PIMS].[dbo].[SOHeader] header
	ON header.SOHeaderId = tq.[SoHeaderId]
LEFT JOIN [PIMS].[dbo].[BusinessPartner] AS cust
	ON cust.BusinessPartnerId = header.BusinessPartnerId
LEFT JOIN [PIMS].[dbo].[LookupsVw] sts
	ON sts.[LookupName] = 'QC STATUS' AND tq.[TaskStatusId] = sts.[LookupDetailId]
WHERE tq.[DeletedOn] IS NULL
)result";

            if (productId >= 0)
            {
                fullQuery += @"
WHERE [ProductId] = " + productId;
            }
            else
            {
                if (soHeaderId >= 0)
                {
                    fullQuery += @"
WHERE [SoHeaderId] = " + soHeaderId;
                }
            }

            return await GetSqlData<TaskQueue>(fullQuery);
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Unused Screen Query Functions

        //"Product Tasks" -- not used
        public async Task<List<ProductMaterialTask>> GetAllProductMaterialTasks() //
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY 
	--result.[ProductNo], 
	result.[OrigTaskSequence]) AS [Id]
	, *
FROM (
SELECT DISTINCT
      prodtask.[ProductTaskId]
      --,prodtask.[ProductId]
      ,prodtask.[MaterialId]	AS [ItemId]
      ,prodtask.[Qty]
      ,prodtask.[TaskId]
      ,prodtask.[TaskName]
      ,prodtask.[TaskDescription]
      ,prodtask.[TaskStatusId]
      ,prodtask.[LegacySystemId]
	  ,CASE WHEN prodtask.[ProductId] = prodtask.[MaterialId] THEN 'Product' ELSE 'Material' END AS [LineTypeName]
      --,prodtask.[ProductNo]
      --,prodtask.[ProductName]
      ,prodtask.[MaterialNo]	AS [ItemNo]
      ,prodtask.[MaterialName]	AS [ItemName]
      ,prodtask.[OrigTaskName]
      ,prodtask.[OrigTaskDescription]
      ,prodtask.[OrigTaskType]
      ,prodtask.[OrigTaskAssignedTo]
      ,prodtask.[OrigTaskRequired]
      ,prodtask.[OrigTaskSequence]
FROM (	
	SELECT DISTINCT [MaterialId] --[ProductId]
	  FROM [PIMS].[dbo].[SODetailMaterialVw]
	  --WHERE [SOSubLineNo] = 0
	  ) soitems
	LEFT JOIN [PIMS].[dbo].[Product] prod
		--ON soitems.[ProductId] = prod.[ProductId]
		ON soitems.[MaterialId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
		--ON prodtask.[ProductId] = prod.[ProductId]
		ON prodtask.[MaterialId] = prod.[ProductId]
WHERE prodtask.[TaskName] <> ''
) result
ORDER BY result.[ItemNo],
      result.[ItemName],
	  result.[OrigTaskSequence]";

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }
        //"Product Tasks" -- not used
        public async Task<List<ProductMaterialTask>> GetProductMaterialTasksSummary() //
        {
            string fullQuery = @"
SELECT ROW_NUMBER() OVER (ORDER BY 
	result.[ItemNo]) AS [Id]
	,result.*
	,COUNT(prodtask.ProductTaskId) AS [TasksCount]
FROM (
SELECT DISTINCT
      prodtask.[MaterialId]	AS [ItemId]
      ,prodtask.[MaterialNo]	AS [ItemNo]
      ,prodtask.[MaterialName]	AS [ItemName]
	  ,CASE WHEN prodtask.[ProductId] = prodtask.[MaterialId] THEN 'Product' ELSE 'Material' END AS [LineTypeName]
FROM (	
	SELECT DISTINCT [MaterialId] 
	  FROM [PIMS].[dbo].[SODetailMaterialVw]
	  ) soitems
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON soitems.[MaterialId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
		ON prodtask.[MaterialId] = prod.[ProductId]
WHERE prodtask.[TaskName] <> ''
) result
	LEFT JOIN [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
		ON prodtask.[MaterialId] = result.[ItemId] AND prodtask.[TaskName] <> ''
GROUP BY result.[ItemId]
      ,result.[ItemNo]
      ,result.[ItemName]
	  ,result.[LineTypeName]
ORDER BY result.[ItemNo]
      ,result.[ItemName]";

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }

        //"Tasks & Notes"
        public async Task<List<SODetailMaterial>> GetSODetailMaterialDataExt(int soHeaderId, bool mainLineItemOnly = true, bool showDuty = false) // 
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]) AS [Id]
	,matView.*
	,COUNT(sodetask.[SODetailTaskId]) AS TasksCount
FROM (
	SELECT DISTINCT 
		[SODetailMaterialId]
		,[SODetailId]
		,[SoLineNo]
		,[SoSubLineNo]
		,[SoSubLineTypeId]
		,[ProductId]
		,[ProductNo]
		,[ProductName]
		,[MaterialId]
		,[MaterialNo]
		,[MaterialName]
		,[MaterialStatusId]
		,[MaterialNote]
		,[SOHeaderId]
		,[LineTypeName]
		,[LineTypeGroup]
		,[ForSoDetailId]
		,[SalesProgramId]
		,[ProgramName]
		,[OrderQty]
		,[ReceivedQty]
		,[UOMId]
		,[Cost]
		,[Price]
		,[SONumber]
		,[BusinessPartnerId_Customer]
		,[CustomerName]
	FROM [PIMS].[dbo].[SODetailMaterialVw] 
	WHERE [SOHeaderId] = {0}
)matView
	LEFT JOIN [PIMS].[dbo].[SODetailTask] sodetask
		ON matView.[SODetailId] = sodetask.[SODetailId]
			AND matView.[SOLineNo] = sodetask.[SOLineNo]
			AND matView.[MaterialId] = sodetask.[MaterialId]";

            if (mainLineItemOnly || !showDuty)
            {
                //WHERE matView.[SoSubLineNo] = 0
                //	AND matView.[MaterialId] <> 2180 -- DUTY
                query += @"
WHERE ";
                if (mainLineItemOnly)
                    query += @" matView.[SoSubLineNo] = 0";

                if (!showDuty)
                {
                    if (mainLineItemOnly)
                        query += @"
    AND ";
                    query += @" matView.[MaterialId] <> 2180";
                }
            }

            query += @"
GROUP BY
	matView.[SODetailMaterialId]
	,matView.[SODetailId]
	,matView.[SoLineNo]
	,matView.[SoSubLineNo]
	,matView.[SoSubLineTypeId]
	,matView.[ProductId]
	,matView.[ProductNo]
	,matView.[ProductName]
	,matView.[MaterialId]
	,matView.[MaterialNo]
	,matView.[MaterialName]
	,matView.[MaterialStatusId]
	,matView.[MaterialNote]
	,matView.[SOHeaderId]
	,matView.[LineTypeName]
	,matView.[LineTypeGroup]
	,matView.[ForSoDetailId]
	,matView.[SalesProgramId]
	,matView.[ProgramName]
	,matView.[OrderQty]
	,matView.[ReceivedQty]
	,matView.[UOMId]
	,matView.[Cost]
	,matView.[Price]
	,matView.[SONumber]
	,matView.[BusinessPartnerId_Customer]
	,matView.[CustomerName]
ORDER BY matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]";

            string fullQuery = string.Format(query, soHeaderId);

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }
        //"Tasks & Notes"
        public async Task<List<SOData>> GetSOData()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY soh.[SODate]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]	  
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName] 
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]	  	  
	  ,ISNULL(SalesProgramId, 0) AS [SalesProgramId]
	  --,ISNULL(ProgramName, '') AS [ProgramName]
	  ,ISNULL(PONumber, '') AS [PONumber]
	  ,ISNULL(TotalCost, 0.0) AS [TotalCost]
	  ,ISNULL(TotalPrice , 0.0) AS [TotalPrice]
      ,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]	
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	
      ,ISNULL(soh.[EndDate], '1900-01-01') AS [EndDate]	
      ,ISNULL(soh.[PostedDate], '1900-01-01') AS [PostedDate]		  
	  ,ISNULL(PostedToERP, 0) AS [PostedToERP]
  FROM [PIMS].[dbo].[SOHeader] soh
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  ORDER BY soh.[SODate]
 ";
            return await GetSqlData<SOData>(query);
        }
        //"Tasks & Notes"
        public async Task<List<SODetailMaterial>> GetSODetailMaterialData(int soHeaderId = 0, bool showDuty = true) // 
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [SONumber],[SOLineNo],[SOSubLineNo]) AS [Id], *
FROM (
SELECT DISTINCT 
	[SODetailId]
	,[SoLineNo]
	,[SoSubLineNo]
	,[SoSubLineTypeId]
	--,[SoSubLineType]
	,[ProductId]
	,[ProductNo]
	,[ProductName]
	,[MaterialId]
	,[MaterialNo]
	,[MaterialName]
	--,[Qty]
	,[MaterialStatusId]
	,[MaterialNote]
	,[SOHeaderId]
	,[LineTypeName]
	,[LineTypeGroup]
	,[ForSoDetailId]
	,[SalesProgramId]
	,[ProgramName]
	,[OrderQty]
	,[ReceivedQty]
	,[UOMId]
	,[Cost]
	,[Price]
	,[SONumber]
	,[BusinessPartnerId_Customer]
	,[CustomerName]
FROM [PIMS].[dbo].[SODetailMaterialVw]";

            string fullQuery = "";
            if (soHeaderId >= 0)
            {
                if (showDuty)
                    query += @"
	WHERE [SOHeaderId] = {0}";
                else
                    query += @"
	WHERE [SOHeaderId] = {0}
        AND [MaterialId] <> 2180";

                fullQuery = string.Format(query, soHeaderId) + @"
)x
ORDER BY [SONumber],[SOLineNo],[SOSubLineNo]";
            }
            else
            {
                if (!showDuty)
                    query += @"
	WHERE [MaterialId] <> 2180";

                fullQuery = query + @"
)x
ORDER BY [SONumber],[SOLineNo],[SOSubLineNo]";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }
        //"Tasks & Notes"
        public async Task<List<SODetailMaterial>> GetSODetailProductAndMaterialSummaryData(int soHeaderId = 0, bool showDuty = true) // 
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [LineTypeName] DESC, [MaterialNo] ASC) AS [Id], *
FROM (
SELECT 
	DISTINCT 
	list.[LineType] AS [LineTypeName]
	,list.[MaterialId]
	,list.[MaterialNo]
	,list.[MaterialName]
	,list.[MaterialStatusId]
	,list.[MaterialNote]
	,list.[SOHeaderId]
	,list.[SalesProgramId]
	,list.[ProgramName]
	,list.[SONumber]
	,list.[BusinessPartnerId_Customer]
	,list.[CustomerName]
	, ISNULL(CASE WHEN [LineType] = 'Product' THEN prodTasks.[TasksCount] WHEN [LineType] = 'Material' THEN  matTasks.[TasksCount] ELSE '' END, 0) AS [TasksCount]
FROM [PIMS].[dbo].[SODetailMaterialVw] list
LEFT JOIN (SELECT [ProductId], COUNT(*) [TasksCount] FROM [ProductMaterialTaskVw] WHERE [TaskName] <> '' GROUP BY [ProductId]) prodTasks
	ON list.[LineType] = 'Product' AND list.[MaterialId] = prodTasks.[ProductId]
LEFT JOIN (SELECT [MaterialId], COUNT(*) [TasksCount] FROM [ProductMaterialTaskVw] WHERE [TaskName] <> '' GROUP BY [MaterialId]) matTasks
	ON list.[LineType] = 'Material' AND list.[MaterialId] = matTasks.[MaterialId]";

            string fullQuery = "";
            if (soHeaderId >= 0)
            {
                if (showDuty)
                    query += @"
	WHERE list.[SOHeaderId] = {0}";
                else
                    query += @"
	WHERE list.[SOHeaderId] = {0}
        AND list.[MaterialId] <> 2180";

                fullQuery = string.Format(query, soHeaderId) + @"
)x
ORDER BY [LineTypeName] DESC, [MaterialNo] ASC";
            }
            else
            {
                if (!showDuty)
                    query += @"
	WHERE list.[MaterialId] <> 2180";

                fullQuery = query + @"
)x
ORDER BY [LineTypeName] DESC, [MaterialNo] ASC";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }
        //"Tasks & Notes"
        public async Task<List<ProductMaterialTask>> GetMaterialTasks(int soHeaderId) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]) AS [Id]
      ,prodtask.[ProductTaskId]
      ,0 AS [ProductId]
      ,prodtask.[MaterialId]
      ,prodtask.[Qty]
      ,prodtask.[TaskId]
      ,prodtask.[TaskName]
      ,prodtask.[TaskDescription]
      ,prodtask.[TaskStatusId]
      ,prodtask.[LegacySystemId]
      ,'' AS [ProductNo]
      ,'' AS [ProductName]
      ,prodtask.[MaterialNo]
      ,prodtask.[MaterialName]
      ,prodtask.[OrigTaskName]
      ,prodtask.[OrigTaskDescription]
      ,prodtask.[OrigTaskType]
      ,prodtask.[OrigTaskAssignedTo]
      ,prodtask.[OrigTaskRequired]
      ,prodtask.[OrigTaskSequence]
FROM [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
WHERE prodtask.[MaterialId] IN (SELECT DISTINCT [MaterialId]
FROM [PIMS].[dbo].[SODetailMaterialVw]
	WHERE [SOHeaderId] = {0}) 
ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]  ";

            string fullQuery = "";
            fullQuery = string.Format(query, soHeaderId);

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }

        public async Task<List<SODetailMaterial>> GetSODetailMaterialSummaryData(int soHeaderId = 0, bool showDuty = true) // 
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [MaterialNo]) AS [Id], *
FROM (
SELECT DISTINCT 
	[MaterialId]
	,[MaterialNo]
	,[MaterialName]
	,[MaterialStatusId]
	,[MaterialNote]
	,[SOHeaderId]
	,[LineTypeName]
	,[LineTypeGroup]
	,[SalesProgramId]
	,[ProgramName]
	,[SONumber]
	,[BusinessPartnerId_Customer]
	,[CustomerName]
FROM [PIMS].[dbo].[SODetailMaterialVw]";

            string fullQuery = "";
            if (soHeaderId >= 0)
            {
                if (showDuty)
                    query += @"
	WHERE [SOHeaderId] = {0}";
                else
                    query += @"
	WHERE [SOHeaderId] = {0}
        AND [MaterialId] <> 2180";

                fullQuery = string.Format(query, soHeaderId) + @"
)x
ORDER BY [MaterialNo]";
            }
            else
            {
                if (!showDuty)
                    query += @"
	WHERE [MaterialId] <> 2180";

                fullQuery = query + @"
)x
ORDER BY [MaterialNo]";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }

        public async Task<List<SODetailMaterial>> GetSODetailMaterialTasksData(int soHeaderId = 0) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]) AS [Id]
	  ,matView.*
	  ,ISNULL(sodetask.[SODetailTaskId], 0) AS [SODetailTaskId] 
	  ,ISNULL(sodetask.[TaskId], 0) AS [TaskId] 
      ,ISNULL(sodetask.[Task], '') AS [Task] 
      ,ISNULL(sodetask.[TaskStatusId], 0) AS [TaskStatusId] 
      ,ISNULL(sodetask.[TaskNote], '') AS [TaskNote]  

      ,ISNULL(biztask.[TaskSequence], 0)		AS [TaskSequence]
      ,ISNULL(biztask.[Required], 0)			AS [Required]
      ,ISNULL(biztask.[Qty], 0)					AS [TaskQty]
      ,ISNULL(biztask.[AssignedTo], '')			AS [AssignedTo]

FROM [PIMS].[dbo].[SODetailMaterialVw] matView
	LEFT JOIN [PIMS].[dbo].[SODetailTask] sodetask
		ON matView.[SODetailId] = sodetask.[SODetailId]
			AND matView.[SOLineNo] = sodetask.[SOLineNo]
			AND matView.[SODetailMaterialId] = sodetask.[SODetailMaterialId]
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sodetask.[SODetailId] = sod.[SODetailId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.[SOHeaderId] = soh.[SOHeaderId]
	LEFT JOIN [PIMS].[dbo].[BusinessPartnerTask] biztask
		ON soh.[BusinessPartnerId] = biztask.[BusinessPartnerId] AND sodetask.[TaskId] = biztask.[TaskId] ";

            string fullQuery = "";
            if (soHeaderId >= 0)
            {
                query += @"
	WHERE matView.[SOHeaderId] = {0}";

                fullQuery = string.Format(query, soHeaderId) + @"
ORDER BY matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY  CASE WHEN ISNULL(biztask.[TaskSequence], 0) = 0 THEN 1000 ELSE [TaskSequence] END
    matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }

        public async Task<List<SODetailMaterial>> GetSODetailMaterialTasksData(int soHeaderId, int soDetailId, int materialId) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY matView.[SONumber],matView.[SOLineNo],matView.[SOSubLineNo]) AS [Id]
	  ,matView.*
	  ,ISNULL(sodetask.[SODetailTaskId], 0) AS [SODetailTaskId] 
	  ,ISNULL(sodetask.[TaskId], 0) AS [TaskId] 
      ,ISNULL(sodetask.[Task], '') AS [Task] 
      ,ISNULL(sodetask.[TaskStatusId], 0) AS [TaskStatusId] 
      ,ISNULL(sodetask.[TaskNote], '') AS [TaskNote]  
	  
      ,ISNULL(biztask.[TaskSequence], 0)		AS [TaskSequence]
      ,ISNULL(biztask.[Required], 0)			AS [Required]
      ,ISNULL(biztask.[Qty], 0)					AS [TaskQty]
      ,ISNULL(biztask.[AssignedTo], '')			AS [AssignedTo]

FROM [PIMS].[dbo].[SODetailMaterialVw] matView
	LEFT JOIN [PIMS].[dbo].[SODetailTask] sodetask
		ON matView.[SODetailId] = sodetask.[SODetailId]
			AND matView.[SOLineNo] = sodetask.[SOLineNo]
			AND matView.[SODetailMaterialId] = sodetask.[SODetailMaterialId]			
	--LEFT JOIN [PIMS].[dbo].[SODetail] sod
	--	ON sodetask.[SODetailId] = sod.[SODetailId]
	--LEFT JOIN [PIMS].[dbo].[SOHeader] soh
	--	ON sod.[SOHeaderId] = soh.[SOHeaderId]
	--LEFT JOIN [PIMS].[dbo].[BusinessPartnerTask] biztask
	--	ON soh.[BusinessPartnerId] = biztask.[BusinessPartnerId] AND sodetask.[TaskId] = biztask.[TaskId] 
	LEFT JOIN [PIMS].[dbo].[BusinessPartnerTask] biztask
		ON matView.[BusinessPartnerId_Customer] = biztask.[BusinessPartnerId] AND sodetask.[TaskId] = biztask.[TaskId] 

WHERE matView.[SOHeaderId] = {0}
	AND matView.[SODetailId] = {1}
	AND matView.[MaterialId] = {2}
    AND [SODetailTaskId] > 0
ORDER BY  CASE WHEN ISNULL(biztask.[TaskSequence], 0) = 0 THEN 1000 ELSE [TaskSequence] END
	,matView.[SONumber]
	,matView.[SOLineNo]
	,matView.[SOSubLineNo]
";

            string fullQuery = string.Format(query, soHeaderId, soDetailId, materialId);
            return await GetSqlData<SODetailMaterial>(fullQuery);
        }

        public async Task<List<SODetailMaterial>> GetSODetailMaterialAndTestData(int soHeaderId = 0, bool showDuty = true) // 
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY soDetail.[SONumber],soDetail.[SOLineNo],soDetail.[SOSubLineNo]) AS [Id]
	, soDetail.*
	, ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
	, ISNULL(test.[RequestedBy], '')						AS [RequestedBy]
	, ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]

	, CASE WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN test.[PassedDate]	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN test.[FailedDate]	
		ELSE '1900-01-01 00:00:00' END AS [ProcessedDate]
	, CASE WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN test.[PassedBy]	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN test.[FailedBy]	
		ELSE '' END AS [ProcessedBy]
	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestResult]
FROM (
SELECT DISTINCT 
	[SODetailId]
	,[SoLineNo]
	,[SoSubLineNo]
	,[SoSubLineTypeId]
	--,[SoSubLineType]
	,[ProductId]
	,[ProductNo]
	,[ProductName]
	,[MaterialId]
	,[MaterialNo]
	,[MaterialName]
	--,[Qty]
	,[MaterialStatusId]
	,[MaterialNote]
	,[SOHeaderId]
	,[LineTypeName]
	,[LineTypeGroup]
	,[ForSoDetailId]
	,[SalesProgramId]
	,[ProgramName]
	,[OrderQty]
	,[ReceivedQty]
	,[UOMId]
	,[Cost]
	,[Price]
	,[SONumber]
	,[BusinessPartnerId_Customer]
	,[CustomerName]
FROM [PIMS].[dbo].[SODetailMaterialVw]";

            string fullQuery = "";
            if (soHeaderId >= 0)
            {
                if (showDuty)
                    query += @"
	WHERE [SOHeaderId] = {0}";
                else
                    query += @"
	WHERE [SOHeaderId] = {0}
        AND [MaterialId] <> 2180";

                fullQuery = string.Format(query, soHeaderId) + @"
)soDetail
	LEFT JOIN [PIMS].[dbo].[ProductTest] test
		ON soDetail.[ProductId] = test.[ProductId]
ORDER BY soDetail.[SONumber],soDetail.[SOLineNo],soDetail.[SOSubLineNo]";
            }
            else
            {
                if (!showDuty)
                    query += @"
	WHERE [MaterialId] <> 2180";

                fullQuery = query + @"
)soDetail
	LEFT JOIN [PIMS].[dbo].[ProductTest] test
		ON soDetail.[ProductId] = test.[ProductId]
ORDER BY soDetail.[SONumber],soDetail.[SOLineNo],soDetail.[SOSubLineNo]";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }

        public async Task<List<ProductTest>> GetProductTests(List<int> productIdList, bool getEmptyTests = false) //
        {
            string query = "";

            if (getEmptyTests)
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prod.[ProductNo]) AS [Id]
      ,ISNULL(test.[ProductTestId], 0)	AS [ProductTestId]
      ,prod.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]
      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM [PIMS].[dbo].[Product] prod
	LEFT JOIN [PIMS].[dbo].[ProductTest] test
		ON test.[ProductId] = prod.[ProductId]
WHERE test.[ProductId] IN ({0}) 
ORDER BY prod.[ProductNo]
";
            else
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prod.[ProductNo]) AS [Id]
      ,test.[ProductTestId]
      ,test.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]
      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM [PIMS].[dbo].[ProductTest] test
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON test.[ProductId] = prod.[ProductId]
WHERE test.[ProductId] IN ({0}) 
ORDER BY prod.[ProductNo]
";
            string fullQuery = "";
            fullQuery = string.Format(query, string.Join(",", productIdList));

            return await GetSqlData<ProductTest>(fullQuery);
        }
        public async Task<List<ProductTest>> GetProductTests(int soHeaderId, bool getEmptyTests = false) //
        {
            string query = "";

            if (getEmptyTests)
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prod.[ProductNo]) AS [Id]
      ,ISNULL(test.[ProductTestId], 0)	AS [ProductTestId]
      ,prod.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]
      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM [PIMS].[dbo].[Product] prod
	LEFT JOIN [PIMS].[dbo].[ProductTest] test
		ON test.[ProductId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SODetail] sodetail
		ON test.[ProductId] = sodetail.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soheader
		ON sodetail.[SOHeaderId] = soheader.[SOHeaderId]
WHERE soheader.[SOHeaderId] = {0}
ORDER BY sodetail.SOLineNo, prod.[ProductNo]
";
            else
                query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prod.[ProductNo]) AS [Id]
      ,ISNULL(test.[ProductTestId], 0)	AS [ProductTestId]
      ,prod.[ProductId]
	  ,ISNULL(prod.[ProductNo], '')		AS [ProductNo]
	  ,ISNULL(prod.[ProductName], '')	AS [ProductName]
      ,ISNULL(test.[Qty], 0)			AS [Qty]
      ,ISNULL(test.[RequestedDate], '1900-01-01 00:00:00')	AS [RequestedDate]
      ,ISNULL(test.[RequestedBy], '')	AS [RequestedBy]
      ,ISNULL(test.[ReceivedDate], '1900-01-01 00:00:00')	AS [ReceivedDate]
      ,ISNULL(test.[PassedDate], '1900-01-01 00:00:00')		AS [PassedDate]
      ,ISNULL(test.[PassedBy], '')							AS [PassedBy]
      ,ISNULL(test.[FailedDate], '1900-01-01 00:00:00')		AS [FailedDate]
      ,ISNULL(test.[FailedBy], '')		AS [FailedBy]
      ,ISNULL(test.[Comments], '')		AS [Comments]
      ,ISNULL(test.[Attachment], '')	AS [Attachment]	  	  
      --,soheader.[SOHeaderId]
	  --,sodetail.[SODetailId]
	  	, CASE 
		WHEN test.[RequestedDate] IS NULL OR test.[RequestedDate] = '1900-01-01 00:00:00' THEN 'Un-Tested'	
		WHEN test.[RequestedDate] IS NOT NULL AND test.[RequestedDate] > '1900-01-01 00:00:00' 
			AND (test.[PassedDate] IS NULL OR test.[PassedDate] = '1900-01-01 00:00:00')
			AND (test.[FailedDate] IS NULL OR test.[FailedDate] = '1900-01-01 00:00:00')
			THEN 'Pending'	
		WHEN test.[PassedDate] IS NOT NULL AND test.[PassedDate] > '1900-01-01 00:00:00' THEN 'Passed'	
		WHEN test.[FailedDate] IS NOT NULL AND test.[FailedDate] > '1900-01-01 00:00:00' THEN 'Failed'
		ELSE '' END AS [TestStatus]
FROM [PIMS].[dbo].[ProductTest] test
	LEFT JOIN [PIMS].[dbo].[Product] prod	
		ON test.[ProductId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SODetail] sodetail
		ON test.[ProductId] = sodetail.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soheader
		ON sodetail.[SOHeaderId] = soheader.[SOHeaderId]
WHERE soheader.[SOHeaderId] = {0}
ORDER BY sodetail.SOLineNo, prod.[ProductNo]
";
            string fullQuery = "";
            fullQuery = string.Format(query, soHeaderId);

            return await GetSqlData<ProductTest>(fullQuery);
        }

        public async Task<List<ProductMaterialTask>> GetProductMaterialTasks(List<int> productIdList) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]) AS [Id]
      ,prodtask.[ProductTaskId]
      ,prodtask.[ProductId]
      ,prodtask.[MaterialId]
      ,prodtask.[Qty]
      ,prodtask.[TaskId]
      ,prodtask.[TaskName]
      ,prodtask.[TaskDescription]
      ,prodtask.[TaskStatusId]
      ,prodtask.[LegacySystemId]
      ,prodtask.[ProductNo]
      ,prodtask.[ProductName]
      ,prodtask.[MaterialNo]
      ,prodtask.[MaterialName]
      ,prodtask.[OrigTaskName]
      ,prodtask.[OrigTaskDescription]
      ,prodtask.[OrigTaskType]
      ,prodtask.[OrigTaskAssignedTo]
      ,prodtask.[OrigTaskRequired]
      ,prodtask.[OrigTaskSequence]
FROM [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
WHERE prodtask.[ProductId] IN ({0}) 
ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]";

            string fullQuery = "";
            fullQuery = string.Format(query, string.Join(",", productIdList));

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }

        public async Task<List<ProductMaterialTask>> GetMaterialTasks(List<int> materialIdList) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]) AS [Id]
      ,prodtask.[ProductTaskId]
      ,0 AS [ProductId]
      ,prodtask.[MaterialId]
      ,prodtask.[Qty]
      ,prodtask.[TaskId]
      ,prodtask.[TaskName]
      ,prodtask.[TaskDescription]
      ,prodtask.[TaskStatusId]
      ,prodtask.[LegacySystemId]
      ,'' AS [ProductNo]
      ,'' AS [ProductName]
      ,prodtask.[MaterialNo]
      ,prodtask.[MaterialName]
      ,prodtask.[OrigTaskName]
      ,prodtask.[OrigTaskDescription]
      ,prodtask.[OrigTaskType]
      ,prodtask.[OrigTaskAssignedTo]
      ,prodtask.[OrigTaskRequired]
      ,prodtask.[OrigTaskSequence]
FROM [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
WHERE prodtask.[MaterialId] IN ({0}) 
ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence] ";

            string fullQuery = "";
            fullQuery = string.Format(query, string.Join(",", materialIdList));

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }

        public async Task<List<ProductMaterialTask>> GetProductMaterialTasks(int soHeaderId) //
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY prodtask.[ProductNo], prodtask.[OrigTaskSequence]) AS [Id]
      ,prodtask.[ProductTaskId]
      ,prodtask.[ProductId]
      ,prodtask.[MaterialId]
      ,prodtask.[Qty]
      ,prodtask.[TaskId]
      ,prodtask.[TaskName]
      ,prodtask.[TaskDescription]
      ,prodtask.[TaskStatusId]
      ,prodtask.[LegacySystemId]
	  ,CASE WHEN prodtask.[ProductId] = prodtask.[MaterialId] THEN 'Product' ELSE 'Material' END AS [LineTypeName]
      ,prodtask.[ProductNo]
      ,prodtask.[ProductName]
      ,prodtask.[MaterialNo]
      ,prodtask.[MaterialName]
      ,prodtask.[OrigTaskName]
      ,prodtask.[OrigTaskDescription]
      ,prodtask.[OrigTaskType]
      ,prodtask.[OrigTaskAssignedTo]
      ,prodtask.[OrigTaskRequired]
      ,prodtask.[OrigTaskSequence]
FROM [PIMS].[dbo].[ProductMaterialTaskVw] prodtask
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON prodtask.[ProductId] = prod.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SODetail] sodetail
		ON prodtask.[ProductId] = sodetail.[ProductId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soheader
		ON sodetail.[SOHeaderId] = soheader.[SOHeaderId]
WHERE soheader.[SOHeaderId] = {0}
	AND prodtask.[TaskName] <> ''
ORDER BY prodtask.[ProductNo]
      ,prodtask.[MaterialNo]
	  ,prodtask.[OrigTaskSequence] ";

            string fullQuery = "";
            fullQuery = string.Format(query, soHeaderId);

            return await GetSqlData<ProductMaterialTask>(fullQuery);
        }

        public async Task<List<POData>> GetSODataLite()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY soh.[SODate]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName] 
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
  FROM [PIMS].[dbo].[SOHeader] soh
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  ORDER BY soh.[SODate]
 ";
            return await GetSqlData<POData>(query);
        }

        public async Task<List<PODetailData>> GetSODetailData(string sono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]	  
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
	  
	  ,ISNULL(sod.[SOLineNo], 0) AS [SOLineNo]
      ,ISNULL(sod.[SOSubLineNo], 0) AS [SOSubLineNo]
      ,ISNULL(sod.[SOSubLineTypeId], 0) AS [SOSubLineTypeId]
      ,CASE WHEN sod.[SOSubLineTypeId] = 1 THEN 'Factory' 
		WHEN sod.[SOSubLineTypeId] = 2 THEN 'Packaging' 
		ELSE '' END AS [SOSubLineType]
		
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]	  
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]

	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Vendor] 
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh	
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId

	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  WHERE soh.[SONumber] = '{0}'
  ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId]   ";

            string fullQuery = string.Format(query, sono);
            return await GetSqlData<PODetailData>(fullQuery);
        }

        public async Task<List<BOMData>> GetBOMData(string pono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ISNULL(pt.[DisplaySequence], 0)) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]

      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]  

      ,ISNULL(sp.[ProgramName], '') AS [ProgramName]
      ,prod.ProductId
      ,LTRIM(ISNULL(prod.ProductNo, '') + ' ' + ISNULL(prod.ProductName, '')) AS [Product]
	  ,ISNULL(prod.[ProductTypeId], 0) AS [ProductTypeId]
	  ,ISNULL(pt.[ProductTypeName], '') AS [ProductTypeName]
	  ,ISNULL(pt.[DisplaySequence], 0) AS [DisplaySequence]
	  ,ISNULL(pt.[POSuffix], '') AS [POSuffix]

      ,'' AS [BoxStyle]           
      ,'' AS [Accessories]         
      ,'' AS [TicketEdiStyle]       
      ,'' AS [TicketInfo]           
      ,'' AS [TicketTypeDestination]
      ,'' AS [TicketSource]    
      ,'' AS [TicketProofApproval]  
      ,'' AS [BoxPONo]              
      ,'' AS [CardsManufacturer]
	  
  FROM [PIMS].[dbo].[PODetail] pod
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.[SOHeaderId] = poh.SoHeaderId		
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sod.[SODetailId] = pod.[SODetailId]	
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[ProductType] pt
		ON prod.[ProductTypeId] = pt.[ProductTypeId]
  WHERE poh.[PONumber] = '{0}'
  ORDER BY ISNULL(pt.[DisplaySequence], 0) ";

            string fullQuery = string.Format(query, pono);
            return await GetSqlData<BOMData>(fullQuery);
        }

        public async Task<List<CommentsData>> GetCommentsData(string pono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY qc.[CreatedOn]) AS [Id]
      ,qc.[QualityControlId]
      ,qc.[QualityControlTypeId]
      ,qc.[ProductId]
      ,qc.[PODetailID]
      ,qc.[QualityControlStatus]

	  ,0 AS [CommentaryId]
      ,0 AS [CommentaryTypeId]
      ,'' AS [Comments]

      ,'' AS [SamplesApproval]
      ,'' AS [DisneyStatus]
      ,'' AS [ImageApproval]

      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
  FROM [PIMS].[dbo].[QualityControl] qc
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON qc.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]   
  WHERE poh.[PONumber] = '{0}' ";

            string fullQuery = string.Format(query, pono);
            return await GetSqlData<CommentsData>(fullQuery);
        }

        public async Task<List<FreightData>> GetFreightData(string pono)
        {
            string query = @"
SELECT sh.[ShipmentHeaderId] AS [Id]

      ,sh.[ShipMethodId]
	  ,ISNULL(shipping.[LookupDescription], '') AS [ShipMethod]

      ,sh.[InvoiceNo]	  
      ,sd.[ShipmentQty]
      ,sh.[TrackingNumber] AS [Tracking]
      ,sh.[ShipmentDate]
      ,sh.[ShipToETA]

  FROM [PIMS].[dbo].[ShipmentHeader] sh
	LEFT JOIN [PIMS].[dbo].[ShipmentDetails] sd
		ON sh.[ShipmentHeaderId] = sd.ShipmentHeaderId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] shipping
		ON sh.[ShipMethodId] = shipping.LookupDetailId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sd.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]  
  WHERE poh.[PONumber] = '{0}'
  ORDER BY [ShipToETA] ASC ";


            string fullQuery = string.Format(query, pono);
            return await GetSqlData<FreightData>(fullQuery);
        }

        public async Task<List<CostData>> GetCostData(string pono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ISNULL(pod.[POLineNo], 0) ) AS [Id]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]

      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]

	  ,ISNULL(prod.[ProductId], 0) AS [ProductId] 
	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  	  
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
	  ,ISNULL(pod.[Cost], 0.0) AS [FirstCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [JewelryCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [PackagingCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [TotalCost]
	  ,ISNULL(sod.[Price], 0.0) AS [Price]
	  ,ISNULL(sod.[Price], 0.0) * ISNULL(pod.[OrderQty], 0) AS [SellAmount]

	  ,ISNULL(hts.[HTSCode], '') AS [HTSCode]
	  ,ISNULL(hts.[HTSAmount], 0.0) AS [DutyPercent]
	  ,ISNULL(hts.[HTSAmount], 0.0) * ISNULL(pod.[OrderQty], 0) * ISNULL(pod.[Cost], 0) AS [DutyAmount]
	  
	  ,0.0 AS [LaborFreight]
	  ,0.0 AS [LaborAmount]
	  ,0.0 AS [DisneyRoyalty]
	  ,0.0 AS [TotalCostLanded]
	  ,0.0 AS [COGPercent]

  FROM [PIMS].[dbo].[PODetail] pod
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[POHeaderId] = pod.[POHeaderId]
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON soh.[SOHeaderId] = poh.SoHeaderId		
	LEFT JOIN [PIMS].[dbo].[SODetail] sod
		ON sod.[SODetailId] = pod.[SODetailId]	
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[ProductAttributes] attribute
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[ProductHTS] prodhts
		ON prodhts.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[HTS] hts
		ON hts.HTSId = prodhts.HTSID
  WHERE poh.[PONumber] = '{0}'
  ORDER BY ISNULL(pod.[POLineNo], 0)";

            string fullQuery = string.Format(query, pono);
            return await GetSqlData<CostData>(fullQuery);
        }
        
        #endregion

        // ----------------------------------------------------------------------------------

        #region Shared Query Functions

        public async Task<List<LAGem_POPortal.Models.Lookup>> GetLookupList(string lookup)
        {
            string query = @"
SELECT ld.[LookupDetailId]		AS [Id]
      ,ld.[LookupDescription]	AS [LookupText]
      ,ld.[LookupCode]			AS [LookupCode]
FROM [PIMS].[dbo].[LookupHeader] lh
LEFT JOIN [PIMS].[dbo].[LookupDetail] ld
	ON ld.[LookupHeaderId] = lh.[LookupHeaderId]
WHERE lh.[LookupName] = '{0}'
ORDER BY lh.[LookupName], ld.[LookupDescription]";

            string fullQuery = string.Format(query, lookup);
            return await GetSqlData<LAGem_POPortal.Models.Lookup>(fullQuery);
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region DepartmentQueue Functions

        public async Task<object> GetDashboardData(string dashboardId)
        {
            //List<DepartmentQueueProcessesView> data = GetDepartmentQueueProcessesViewData().Result;
            //return data;

            DataTableToObjectConverter converter = new DataTableToObjectConverter();
            DataTable dt = new DataTable();
            string whereStatement = "";

            switch (dashboardId)
            {
                //case "HomePageDashboard":
                case "PivotDashboard":
                case "DashboardEdit":
                case "DepartmentQueueProcessesView_all":
                    whereStatement = "";
                    break;

                case "DepartmentQueueProcessesView_thisyear":
                    whereStatement = @"
  WHERE YEAR([EndTime]) = YEAR(GETDATE())";
                    break;

                case "HomePageDashboard":
                case "SOPOGeneralDetailData":
                    List<GeneralDetailData> list = await GetGeneralDetailData();
                    return list;
                    break;
                //return await GetGenaralDetailData();
                //break;

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

        #region Dashboard Query Functions

        public async Task<List<GeneralDetailData>> GetGeneralDetailData()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]	  
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
	  
	  ,ISNULL(sod.[SOLineNo], 0) AS [SOLineNo]
      ,ISNULL(sod.[SOSubLineNo], 0) AS [SOSubLineNo]
      ,ISNULL(sod.[SOSubLineTypeId], 0) AS [SOSubLineTypeId] 
	  ,CAST(ISNULL(sod.[SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL(sod.[SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]
      ,CASE WHEN sod.[SOSubLineTypeId] = 1 THEN 'Factory' 
		WHEN sod.[SOSubLineTypeId] = 2 THEN 'Packaging' 
		ELSE '' END AS [SOSubLineType]
		
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]	  
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]

      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]
      ,ISNULL(pod.[POSubLineTypeId], 0) AS [POSubLineTypeId]
      ,ISNULL(pod.[ProductId], 0) AS [ProductId]

	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Vendor] 
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh	
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
		
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sod.[SODetailId] = pod.[SODetailId]

	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId] 
	,pod.[POLineNo],pod.[POSubLineTypeId]   ";

            return await GetSqlData<GeneralDetailData>(query);
        }
        public async Task<List<GeneralDetailData>> GetGeneralDetailData_BySono(string sono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]	  
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
	  
	  ,ISNULL(sod.[SOLineNo], 0) AS [SOLineNo]
      ,ISNULL(sod.[SOSubLineNo], 0) AS [SOSubLineNo]
      ,ISNULL(sod.[SOSubLineTypeId], 0) AS [SOSubLineTypeId] 
	  ,CAST(ISNULL(sod.[SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL(sod.[SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]
      ,CASE WHEN sod.[SOSubLineTypeId] = 1 THEN 'Factory' 
		WHEN sod.[SOSubLineTypeId] = 2 THEN 'Packaging' 
		ELSE '' END AS [SOSubLineType]
		
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]	  
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]

      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]
      ,ISNULL(pod.[POSubLineTypeId], 0) AS [POSubLineTypeId]
      ,ISNULL(pod.[ProductId], 0) AS [ProductId]

	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Vendor] 
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh	
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
		
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sod.[SODetailId] = pod.[SODetailId]

	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  WHERE soh.[SONumber] = '{0}'
  ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId] 
	,pod.[POLineNo],pod.[POSubLineTypeId]   ";

            string fullQuery = string.Format(query, sono);
            return await GetSqlData<GeneralDetailData>(fullQuery);
        }
        public async Task<List<GeneralDetailData>> GetGeneralDetailData_ByPono(string pono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]	  
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
	  
	  ,ISNULL(sod.[SOLineNo], 0) AS [SOLineNo]
      ,ISNULL(sod.[SOSubLineNo], 0) AS [SOSubLineNo]
      ,ISNULL(sod.[SOSubLineTypeId], 0) AS [SOSubLineTypeId] 
	  ,CAST(ISNULL(sod.[SOLineNo], 0) AS VARCHAR(4)) + '.' + CAST(ISNULL(sod.[SOSubLineNo], 0) AS VARCHAR(4))  AS [SOLineNoExt]
      ,CASE WHEN sod.[SOSubLineTypeId] = 1 THEN 'Factory' 
		WHEN sod.[SOSubLineTypeId] = 2 THEN 'Packaging' 
		ELSE '' END AS [SOSubLineType]
		
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]	  
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]

      ,ISNULL(pod.[PODetailId], 0) AS [PODetailId]
      ,ISNULL(pod.[POLineNo], 0) AS [POLineNo]
      ,ISNULL(pod.[POSubLineNo], 0) AS [POSubLineNo]
      ,ISNULL(pod.[POSubLineTypeId], 0) AS [POSubLineTypeId]
      ,ISNULL(pod.[ProductId], 0) AS [ProductId]

	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(poh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Vendor] 
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh	
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
		
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sod.[SODetailId] = pod.[SODetailId]

	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON soh.[BusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON poh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON customer.CountryCodeId = country.LookupDetailId
  WHERE poh.[PONumber] = '{0}'
  ORDER BY sod.[SOLineNo],sod.[SOSubLineTypeId] 
	,pod.[POLineNo],pod.[POSubLineTypeId]   ";

            string fullQuery = string.Format(query, pono);
            return await GetSqlData<GeneralDetailData>(fullQuery);
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region Old/Unused Functions

        public async Task<List<POData>> GetPOData_old()
        {
            string query = "";

            query = @"
SELECT ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescription], '') AS [CountryCode]
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
      ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	  
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON soh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON poh.[ShipToBusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON prod.ProductId = pod.ProductId
			AND sp.SalesProgramId = pod.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON vendor.CountryCodeId = country.LookupDetailId
  ORDER BY poh.[PODate]
 ";
            //query = @"SELECT TOP 1000 poh.[POHeaderId]
            //     ,ISNULL(poh.[PONumber], '') AS [PONumber]
            //     ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]
            //     ,ISNULL(poh.[EndDate], '1900-01-01') AS [EndDate]
            //     ,ISNULL(poh.[PostedToERP], 0) AS [PO_PostedToERP]

            //     ,ISNULL(prod.[ProductId], 0) AS [ProductId] 
            //     ,ISNULL(prod.[ProductNo], '') AS [ProductNo]

            //     ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
            //     ,ISNULL(soh.[SONumber], '') AS [SONumber]
            //     ,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]
            //     ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]
            //     ,ISNULL(soh.[PostedToERP], 0) AS [SO_PostedToERP]

            //     ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId]
            //     ,ISNULL(vendor.[BusinessPartnerCode], '') AS [BusinessPartnerCode_Vendor]
            //     ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
            //     ,ISNULL(vendor.[CountryCodeId], 0) AS [CountryCodeId]
            //     ,ISNULL(country.[LookupDescription], '') AS [CountryCode]

            //     ,ISNULL(poh.[ShipToBusinessPartnerId], 0) AS [ShipToBusinessPartnerId]
            //     ,ISNULL(customer.[BusinessPartnerCode], '') AS [BusinessPartnerCode_Customer]
            //     ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]

            //     ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]

            // FROM [PIMS].[dbo].[POHeader] poh
            //LEFT JOIN [PIMS].[dbo].[SOHeader] soh
            //	ON poh.SoHeaderId = soh.[SOHeaderId]
            //LEFT JOIN [PIMS].[dbo].[SODetail] sod
            //	ON soh.[SOHeaderId] = sod.SoHeaderId
            //LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
            //	ON soh.[BusinessPartnerId] = vendor.BusinessPartnerId
            //LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
            //	ON poh.[ShipToBusinessPartnerId] = customer.BusinessPartnerId
            //LEFT JOIN [PIMS].[dbo].[Product] prod
            //	ON sod.ProductId = prod.ProductId
            //LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
            //	ON sod.SalesProgramId = sp.SalesProgramId
            //LEFT JOIN [PIMS].[dbo].[PODetail] pod
            //	ON prod.ProductId = pod.ProductId
            //		AND sp.SalesProgramId = pod.SalesProgramId
            //LEFT JOIN [PIMS].[dbo].[LookupDetail] country
            //	ON vendor.CountryCodeId = country.LookupDetailId
            // ORDER BY poh.[PODate]
            //";

            return await GetSqlData<POData>(query);

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
                return new List<POData>();
            }

            if (dt == null)
            {
                return new List<POData>();
            }

            List<POData> data = new List<POData>();
            data = await converter.GetObjectListFromDataTable<POData>(dt, new Dictionary<string, string>());

            //foreach (DataRow dr in dt.Rows)
            //{
            //    data.Add(new POData()
            //    {
            //        PONumber = dr["PONumber"].ToString(),
            //        ProductNo = dr["ProductNo"].ToString(),
            //        VendorName = dr["VendorName"].ToString(),
            //        CountryCode = dr["CountryCode"].ToString(),
            //        CustomerName = dr["CustomerName"].ToString(),
            //        QtyOrdered = int.Parse(dr["QtyOrdered"].ToString()),
            //        PODate = DateTime.Parse(dr["PODate"].ToString()),
            //        EndDate = DateTime.Parse(dr["EndDate"].ToString()),
            //        StartDate = DateTime.Parse(dr["StartDate"].ToString()),
            //        SONumber = dr["SONumber"].ToString()
            //    });
            //}

            return data;
        }

        public async Task<List<BOMData>> GetBOMData_old(string pono)
        {
            string query = @"
SELECT ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
      ,ISNULL(poh.[PODate], '1900-01-01') AS [PODate]

      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(soh.[SODate], '1900-01-01') AS [SODate]  

      ,ISNULL(sp.[ProgramName], '') AS [ProgramName]
      ,prod.ProductId
      ,LTRIM(ISNULL(prod.ProductNo, '') + ' ' + ISNULL(prod.ProductName, '')) AS [Product]

      ,'' AS [BoxStyle]           
      ,'' AS [Accessories]         
      ,'' AS [TicketEdiStyle]       
      ,'' AS [TicketInfo]           
      ,'' AS [TicketTypeDestination]
      ,'' AS [TicketSource]    
      ,'' AS [TicketProofApproval]  
      ,'' AS [BoxPONo]              
      ,'' AS [CardsManufacturer]

	  ,ISNULL(customer.[BusinessPartnerCode], '') AS [BusinessPartnerCode_Customer]
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName]
	  
	  ,ISNULL(vendor.[BusinessPartnerCode], '') AS [BusinessPartnerCode_Vendor]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  
  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON soh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON poh.[ShipToBusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON prod.ProductId = pod.ProductId
			AND sp.SalesProgramId = pod.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON vendor.CountryCodeId = country.LookupDetailId
  WHERE poh.[PONumber] = '{0}'
  ORDER BY poh.[PODate] ";
            string fullQuery = string.Format(query, pono);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return new List<BOMData>();
            }

            if (dt == null)
            {
                return new List<BOMData>();
            }

            List<BOMData> data = new List<BOMData>();
            data = await converter.GetObjectListFromDataTable<BOMData>(dt, new Dictionary<string, string>());

            //foreach (DataRow dr in dt.Rows)
            //{
            //    data.Add(new ProgramData()
            //    {
            //        PONumber = dr["PONumber"].ToString(),
            //        ProgramName = dr["ProgramName"].ToString(),
            //        Product = dr["Product"].ToString(),
            //        CustomerName = dr["CustomerName"].ToString(),
            //        VendorName = dr["VendorName"].ToString(),

            //        SODetailId = int.Parse(dr["SODetailId"].ToString()),
            //        OrderQty = int.Parse(dr["OrderQty"].ToString()),
            //        Cost = decimal.Parse(dr["Cost"].ToString()),
            //        Price = decimal.Parse(dr["Price"].ToString())
            //    });
            //}

            return data;
        }

        public async Task<List<CommentsData>> GetCommentsData_old(string pono)
        {
            string query = @"

SELECT qc.[QualityControlId]
      ,qc.[QualityControlTypeId]
      ,qc.[ProductId]
      ,qc.[PODetailID]
      ,qc.[QualityControlStatus]

	  ,0 AS [CommentaryId]
      ,0 AS [CommentaryTypeId]
      ,'' AS [Comments]

      ,'' AS [SamplesApproval]
      ,'' AS [DisneyStatus]
      ,'' AS [ImageApproval]

      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
  FROM [PIMS].[dbo].[QualityControl] qc
	--LEFT JOIN [PIMS].[dbo].[Commentary] com
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON qc.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]   
  WHERE poh.[PONumber] = '{0}' ";

            return await GetSqlData<CommentsData>(query);
        }

        public async Task<List<FreightData>> GetFreightData_old(string pono)
        {
            string query = @"
SELECT sh.[ShipmentHeaderId] AS [Id]

      ,sh.[ShipMethodId]
	  ,ISNULL(shipping.[LookupDescription], '') AS [ShipMethod]

      ,sh.[InvoiceNo]	  
      ,sd.[ShipmentQty]
      ,sh.[TrackingNumber] AS [Tracking]
      ,sh.[ShipmentDate]
      ,sh.[ShipToETA]

  FROM [PIMS].[dbo].[ShipmentHeader] sh
	LEFT JOIN [PIMS].[dbo].[ShipmentDetails] sd
		ON sh.[ShipmentHeaderId] = sd.ShipmentHeaderId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] shipping
		ON sh.[ShipMethodId] = shipping.LookupDetailId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON sd.[PODetailId] = pod.[PODetailId] 
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON pod.[POHeaderId] = poh.[POHeaderId] 
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON poh.[SOHeaderId] = soh.[SOHeaderId]  
  WHERE poh.[PONumber] = '{0}'
  ORDER BY [ShipToETA] ASC ";
            string fullQuery = string.Format(query, pono);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return new List<FreightData>();
            }

            if (dt == null)
            {
                return new List<FreightData>();
            }

            List<FreightData> data = new List<FreightData>();
            data = await converter.GetObjectListFromDataTable<FreightData>(dt, new Dictionary<string, string>());

            //foreach (DataRow dr in dt.Rows)
            //{
            //    data.Add(new FreightData()
            //    {
            //        PONumber = dr["PONumber"].ToString(),
            //        ShipMethod = dr["ShipMethod"].ToString(),
            //        InvoiceNo = dr["InvoiceNo"].ToString(),
            //        Tracking = dr["Tracking"].ToString(),
            //        ShipmentQty = dr["ShipmentQty"].ToString(),
            //        ShipmentDate = DateTime.Parse(dr["ShipmentDate"].ToString()),
            //        ShipToETA = DateTime.Parse(dr["ShipToETA"].ToString()),
            //        Id = int.Parse(dr["Id"].ToString())
            //    });
            //}

            return data;
        }

        public async Task<List<CostData>> GetCostData_old(string pono)
        {
            string query = @"
SELECT ISNULL(sod.[SODetailId], 0) AS [SODetailId]
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]

	  ,ISNULL(prod.[ProductId], 0) AS [ProductId] 
	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  	  
	  ,ISNULL(pod.[OrderQty], 0) AS [QtyOrdered]
	  ,ISNULL(pod.[Cost], 0.0) AS [FirstCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [JewelryCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [PackagingCost]
	  ,ISNULL(pod.[OrderQty], 0.0) * ISNULL(pod.[Cost], 0) AS [TotalCost]
	  ,ISNULL(sod.[Price], 0.0) AS [Price]
	  ,ISNULL(sod.[Price], 0.0) * ISNULL(pod.[OrderQty], 0) AS [SellAmount]

	  ,ISNULL(hts.[HTSCode], '') AS [HTSCode]
	  ,ISNULL(hts.[HTSAmount], 0.0) AS [DutyPercent]
	  ,ISNULL(hts.[HTSAmount], 0.0) * ISNULL(pod.[OrderQty], 0) * ISNULL(pod.[Cost], 0) AS [DutyAmount]
	  
	  ,0.0 AS [LaborFreight]
	  ,0.0 AS [LaborAmount]
	  ,0.0 AS [DisneyRoyalty]
	  ,0.0 AS [TotalCostLanded]
	  ,0.0 AS [COGPercent]

  FROM [PIMS].[dbo].[SODetail] sod
	LEFT JOIN [PIMS].[dbo].[SOHeader] soh
		ON sod.SoHeaderId = soh.[SOHeaderId]   
	LEFT JOIN [PIMS].[dbo].[POHeader] poh
		ON poh.[SOHeaderId] = sod.SoHeaderId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] vendor
		ON soh.[BusinessPartnerId] = vendor.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[BusinessPartner] customer
		ON poh.[ShipToBusinessPartnerId] = customer.BusinessPartnerId
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[ProductAttributes] attribute
		ON sod.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[SalesProgram] sp
		ON sod.SalesProgramId = sp.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[PODetail] pod
		ON prod.ProductId = pod.ProductId
			AND sp.SalesProgramId = pod.SalesProgramId
	LEFT JOIN [PIMS].[dbo].[LookupDetail] country
		ON vendor.CountryCodeId = country.LookupDetailId 
	LEFT JOIN [PIMS].[dbo].[ProductHTS] prodhts
		ON prodhts.ProductId = prod.ProductId
	LEFT JOIN [PIMS].[dbo].[HTS] hts
		ON hts.HTSId = prodhts.HTSID
  WHERE poh.[PONumber] = '{0}'
  ORDER BY poh.[PODate] ";
            string fullQuery = string.Format(query, pono);
            DataTableToObjectConverter converter = new DataTableToObjectConverter();

            DataTable dt = null;
            try
            {
                dt = await converter.GetDataTableFromQuery(fullQuery);
            }
            catch (Exception ex)
            {
                //LogError("GetSqlData", ex, ex.Message);
                return new List<CostData>();
            }

            if (dt == null)
            {
                return new List<CostData>();
            }

            List<CostData> data = new List<CostData>();
            data = await converter.GetObjectListFromDataTable<CostData>(dt, new Dictionary<string, string>());

            return data;
        }

        #endregion

        // ----------------------------------------------------------------------------------
    }
}
