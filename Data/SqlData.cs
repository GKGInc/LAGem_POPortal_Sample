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
        // ----------------------------------------------------------------------------------

        #region Public Functions

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

        public async Task<List<POData>> GetPOData()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY poh.[PODate]) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  --,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
        public async Task<List<POData>> GetPOData(string sono)
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY poh.[PODate]) AS [Id]
      ,ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
  WHERE SoNumber= 'I-10896' 
	AND [SOLineNo] = 1
  ORDER BY [SOLineNo],[SOSubLineNo]";

            string fullQuery = string.Format(query, sono, lineno);
            return await GetSqlData<PODetailData>(fullQuery);
        }

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

        public async Task<List<FreightData>> GetFreightData(int poDetailId)
        {
            string query = @"
SELECT sh.[ShipmentHeaderId] AS [Id]

      ,sh.[ShipMethodId]
	  ,ISNULL(shipping.[LookupDescripiton], '') AS [ShipMethod]

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

        public async Task<List<PODetailData>> GetSOPOData()
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
      ,ISNULL([DisplaySequence], 0) AS [DisplaySequence]
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
  --FROM [PIMS].[dbo].[SOPOvw]
  FROM [PIMS].[dbo].[SOPOvw-JorgeDiagram]
  ORDER BY [SONumber], [SOLineNo],ISNULL([DisplaySequence],0),[SOSubLineNo] ";

            return await GetSqlData<PODetailData>(query);
        }


        public async Task<List<POData>> GetSOData()
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY soh.[SODate]) AS [Id]
      ,ISNULL(soh.[SONumber], '') AS [SONumber]  	
      ,ISNULL(soh.[SOHeaderId], 0) AS [SOHeaderId]
      ,ISNULL(soh.[StartDate], '1900-01-01') AS [StartDate]	
	  ,ISNULL(soh.[BusinessPartnerId], 0) AS [BusinessPartnerId_Customer] 
	  ,ISNULL(customer.[BusinessPartnerName], '') AS [CustomerName] 
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
	  ,ISNULL(shipping.[LookupDescripiton], '') AS [ShipMethod]

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
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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

        #region Old Functions

        public async Task<List<POData>> GetPOData_old()
        {
            string query = "";

            query = @"
SELECT ISNULL(poh.[POHeaderId], 0) AS [POHeaderId]
      ,ISNULL(poh.[PONumber], '') AS [PONumber]
	  ,ISNULL(prod.[ProductNo], '') AS [ProductNo]
	  ,ISNULL(vendor.[BusinessPartnerName], '') AS [VendorName]
	  ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]
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
            //     ,ISNULL(country.[LookupDescripiton], '') AS [CountryCode]

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
	  ,ISNULL(shipping.[LookupDescripiton], '') AS [ShipMethod]

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
