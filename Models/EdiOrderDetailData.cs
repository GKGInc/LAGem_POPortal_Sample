using Azure;
using DevExpress.DataAccess.DataFederation;
using DevExpress.Map.Native;
using DevExpress.Office;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace LAGem_POPortal.Models
{
    public class EdiOrderDetailData //
    {
        public int Id { get; set; }

        public int TradingPartnerId { get; set; }       // EdiOrderSummaryVw
        public string TradingPartnerCode { get; set; }  // EdiOrderSummaryVw
        public string TradingPartnerName { get; set; }  // EdiOrderSummaryVw
        public string PONumber { get; set; }            // EdiOrderSummaryVw
        public DateTime PODate { get; set; }

        public string POStatus { get; set; }
        public int ProductId { get; set; }
        public string ProductNo { get; set; }

        public decimal SOQty { get; set; }          // EdiOrderSummaryVw.Ord_Qty
        public decimal Price { get; set; }
        public decimal ExtPrice { get; set; }       // EdiOrderSummaryVw.ExtPrice

        public string TransactionType { get; set; }

        public DateTime? ShipDate { get; set; }     // EdiOrderSummaryVw.StartDate
        public DateTime? CancelDate { get; set; }   // EdiOrderSummaryVw   
        public DateTime? ShipWindow { get; set; }     // 

        public int ShipYear { get; set; }
        public int ShipMonth { get; set; }
        public int ShipWeek { get; set; }
        public DateTime? MondayOfTheWeek { get; set; }

        public int ItemsCount { get; set; } = 0;       // EdiOrderSummaryVw.Items

        public int EdiHdrId { get; set; }
        public int EdiTrnId { get; set; }

        public int SoHeaderId { get; set; }
        public int SoDetailId { get; set; }

        public int CustTPId { get; set; }
        public string EDIPOType { get; set; }
        public string EDITradingPartnerName { get; set; }

        public bool Archived { get; set; }
        public bool IsGroupPO { get; set; }
        public string GroupPO { get; set; }

    }
}
