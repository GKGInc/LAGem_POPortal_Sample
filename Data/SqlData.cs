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

        #region Main Query Functions

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

        public async Task<List<PODetailData>> GetSOPOData() // Overview
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

  --FROM [PIMS].[dbo].[SOPOvw]
  FROM [PIMS].[dbo].[SOPOvw-JorgeDiagram]
  ORDER BY [SONumber], [SOLineNo],ISNULL([LineDisplaySequence],0),ISNULL([ProductDisplaySequence],0),[SOSubLineNo],[POLineNo] ";

            return await GetSqlData<PODetailData>(query);
        }
        
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
      ,sDetail.[OrderQty]
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
      ,ISNULL(shipment.[ShipmentHeaderId], 0) AS [ShipmentHeaderId]
      ,ISNULL(shipment.[ShipmentDate], '1900-01-01') AS [ShipmentDate]
      ,ISNULL(shipment.[InvoiceNo], '') AS [InvoiceNo]
      ,ISNULL(shipment.[TrackingNumber], '') AS [TrackingNumber]
      ,ISNULL(shipment.[ShipToETA], '1900-01-01') AS [ShipToETA]
      ,ISNULL(shipment.[BusinessPartnerName], '') AS [BusinessPartnerName]
      --,shipment.[PONumber]
      --,shipment.[ProductNo]
      --,shipment.[ProductName]
      --,shipment.[OrderQty]
      ,ISNULL(shipment.[ShipmentQty], 0) AS [ShipmentQty]
      ,ISNULL(shipment.[ShipmentDetailId], 0) AS [ShipmentDetailId]
      --,shipment.[PODetailId]
      ,ISNULL(shipment.[ProductId], 0) AS [ProductId]
	  ,ISNULL(sod.[SODetailId], 0) AS [SODetailId]
	  ,ISNULL(sod.[ForSoDetailId], 0) AS [ForSoDetailId]
	  ,ISNULL(forsod.ProductId, 0) AS [ForProductId]
      ,ISNULL(forprod.[ProductNo], '') AS [ForProductNo]
      --,ISNULL(forprod.[ProductName], '') AS [ForProductName]
      ,ISNULL(shipment.[ShipmentQty], 0) AS [LastShipmentQty]
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


        public async Task<List<CustomerSoPoData>> GetCustomerSoData(bool populatePOListString = true) // CustomerSoPoData
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [CustomerName]) AS [Id]
      ,[CustomerName]
      ,[CustomerPO]
      ,[SONumber]
      ,[StartDate]
      ,[EndDate]
      ,[SOHeaderId]
      ,[SOShipYear]
      ,[SOShipMonth]
      ,[SOShipWeek]
      ,[SOQty]
      ,[SORetail]
      ,ISNULL([ShipmentQty], 0) AS [ShipmentQty]
      ,ISNULL([PONumbers], '') AS [PONumber]
  FROM [PIMS].[dbo].[CustomerSoSummaryVw]";

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

            return await GetSqlData<CustomerSoPoData>(query);
        }

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

        public async Task<List<CustomerSoPoData>> GetCustomerSoPoDetailData(int soHeaderId) 
        {
            string fullQuery = "";
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY [CustomerName],[SONumber],[SOLineNo]) AS [Id]
      ,[CustomerName]
      ,[CustomerPO]
      ,[SONumber]
      ,[ProgramName]
      ,[SOHeaderId]
      ,[SODetailId]
      ,[SODate]
      ,[StartDate]
      ,[EndDate]
      ,[SOLineNo]
      ,[ProductNo]
      ,[ProductName]
      ,[SOQty]
      ,[Cost]
      ,[Price]
      ,[VendorPO]
      ,[VendorName]
      ,[POQty]
      ,ISNULL([ShipmentDate], '1900-01-01') AS [ShipmentDate]
      ,ISNULL([TrackingNumber], '') AS [TrackingNumber]
      ,ISNULL([ShipToETA], '1900-01-01') AS [ShipToETA]
      ,ISNULL([ShipmentQty], 0) AS [ShipmentQty]
      ,[SOShipYear]
      ,[SOShipMonth]
      ,[SOShipWeek]
  FROM [PIMS].[dbo].[CustomerSoPoDetailVw]";

            if (soHeaderId > 0)
            {
                query += @"
WHERE [SOHeaderId] = {0}";

                fullQuery = string.Format(query, soHeaderId) + @"
ORDER BY [SONumber], [SOLineNo]";
            }
            else
            {
                fullQuery = query + @"
ORDER BY [SONumber], [SOLineNo]";
            }

            return await GetSqlData<CustomerSoPoData>(fullQuery);
        }


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
      ,ediView.[NoOfItems]	AS [ItemsCount]
      ,ediView.[Ord_qty]	AS [SOQty]
      ,ediView.[ExtPrice]	AS [ExtPrice]
  FROM [PIMS].[edi].[EdiOrderSummaryVw] ediView
	LEFT JOIN [dbo].[BusinessPartner] bizPartner
		ON bizPartner.EDITradPartId = ediView.[Tradpartid]
  ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]";
            return await GetSqlData<EdiOrderDetailData>(query);
        }

        public async Task<List<EdiOrderDetailData>> GetEdiOrderDetailViewData(string poNo) // [EdiOrderDetailvw]
        {
            string query = @"
SELECT ROW_NUMBER() OVER (ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum],ediView.[Item]) AS [Id]
      ,ediView.[Tradpartid]	AS [TradingPartnerId]
      ,ISNULL(bizPartner.BusinessPartnerCode, '') AS [TradingPartnerCode]
      ,ISNULL(bizPartner.BusinessPartnerName, '') AS [TradingPartnerName]
      ,ediView.[Ponum]		AS [PONumber]
      ,ediView.[Podte]		AS [PODate]
      ,ediView.[PoStat]		AS [POStatus]
	  ,ISNULL(prod.[ProductId],0)   AS [ProductId]
      ,ediView.[Item]		AS [ProductNo]
      ,ediView.[Ord_qty]	AS [SOQty]
      ,ediView.[Price]		AS [Price]
      ,ediView.[ExtPrice]	AS [ExtPrice]
      ,ediView.[Tran_type]	AS [TransactionType]
      ,ediView.[StartDate]	AS [ShipDate]
      ,ediView.[CancelDate]	AS [CancelDate]
      ,ediView.[ShipYear]	AS [ShipYear]
      ,ediView.[ShipMonth]	AS [ShipMonth]
      ,ediView.[ShipWeek]	AS [ShipWeek]
  FROM [PIMS].[edi].[EdiOrderDetailvw] ediView
	LEFT JOIN [dbo].[BusinessPartner] bizPartner
		ON bizPartner.EDITradPartId = ediView.[Tradpartid]
	LEFT JOIN [PIMS].[dbo].[Product] prod
		ON prod.[ProductNo] = ediView.[Item]
  WHERE ediView.[Ponum] = '{0}'
  ORDER BY ediView.[ShipYear] DESC, ediView.[ShipMonth] ASC, ediView.[ShipWeek] ASC, ediView.[Ponum]";

            string fullQuery = string.Format(query, poNo);
            return await GetSqlData<EdiOrderDetailData>(fullQuery);
        }

        #endregion

        // ----------------------------------------------------------------------------------

        #region More Query Functions

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


        public async Task<List<Lookup>> GetLookupList(string lookup)
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
            return await GetSqlData<Lookup>(fullQuery);
        }


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


        public async Task<List<SODetailMaterial>> GetSODetailMaterialData(int soHeaderId = 0) // 
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
                query += @"
	WHERE [SOHeaderId] = {0}";

                fullQuery = string.Format(query, soHeaderId) + @"
)x
ORDER BY [SONumber],[SOLineNo],[SOSubLineNo]";
            }
            else
            {
                fullQuery = query + @"
)x
ORDER BY [SONumber],[SOLineNo],[SOSubLineNo]";
            }

            return await GetSqlData<SODetailMaterial>(fullQuery);
        }

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
