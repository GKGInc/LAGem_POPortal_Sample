using DevExpress.Drawing.Internal.Fonts.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;


namespace LAGem_POPortal.Models
{
    public class CustomerSoPoData // 
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPO { get; set; }
        public string SONumber { get; set; }
        public string ProgramName { get; set; }

        public int SOHeaderId { get; set; }
        public int SODetailId { get; set; }

        public DateTime? SODate { get; set; }        //PO Order Date 
        public DateTime? StartDate { get; set; }     //Customer Ship date
        public DateTime? EndDate { get; set; }       //Factory Cancel Date

        public int SOLineNo { get; set; }

        public int ProductId { get; set; }
        public string ProductNo { get; set; }       //ITEM CATALOG #
        public string ProductName { get; set; }

        public decimal SOQty { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public decimal SORetail { get; set; }

        public string PONumber { get; set; }

        public string VendorPO { get; set; }
        public string VendorName { get; set; }
        public int POQty { get; set; }

        public DateTime? ShipmentDate { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShipToETA { get; set; }
        public int ShipmentQty { get; set; }
        
        public int SOShipYear { get; set; }
        public int SOShipMonth { get; set; }
        public int SOShipWeek { get; set; }
        public DateTime? MondayOfTheWeek { get; set; }

        public int SoSubLineTypeId { get; set; }
        public string SoSubLineType { get; set; }

        public int EdiHdrId { get; set; }
        public int EdiTrnId { get; set; }
        public string EDIPONumber { get; set; }
        public string EDIRefPONumber { get; set; }

        public string SODetailNotes { get; set; }
        public string PODetailNotes { get; set; }
        public string Notes { get; set; } 

        
        // Link columns
        public bool IsItemLinked { get; set; }
        public string LinkedStatus { get; set; }
        public int LinkedToId { get; set; }
        public string LinkedToName { get; set; }

        public bool Archived { get; set; }
        public bool IsGroupPO { get; set; }
        public string GroupPO { get; set; }
    }
}
