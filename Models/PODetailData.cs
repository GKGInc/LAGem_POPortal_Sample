using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class PODetailData
    {
        public int Id { get; set; }

        public string PONumber { get; set; }        //PO Number
        public int POHeaderId { get; set; }
        public int PODetailId { get; set; }
        public int POLineNo { get; set; }

        public int ProductId { get; set; }
        public string ProductNo { get; set; }       //ITEM CATALOG #
        public string ProductName { get; set; }

        public string VendorName { get; set; }       //Jewelry Factory
        public string CountryCode { get; set; }     //Country of origin
        public string CustomerName { get; set; }     //Customer

        public decimal QtyOrdered { get; set; }     //Units

        public DateTime? SODate { get; set; }        //PO Order Date 
        public DateTime? PODate { get; set; }        //PO Order Date 
        public DateTime? EndDate { get; set; }       //Factory Cancel Date
        public DateTime? StartDate { get; set; }     //Customer Ship date

        public string SONumber { get; set; }        //SO Number
        public int SOHeaderId { get; set; }
        public int SODetailId { get; set; }
        public int SOLineNo { get; set; }
        public int SOSubLineNo { get; set; }
        public decimal SOLineNoExt { get; set; }

        public decimal DisplaySequence { get; set; }        
        public int LineDisplaySequence { get; set; }
        public int ProductDisplaySequence { get; set; }

        public int SOSubLineTypeId { get; set; }
        public string SOSubLineType { get; set; }   //ProductTypeName
        public string ProductTypeGroup { get; set; }
        public string LineTypeName { get; set; }
        public string LineTypeGroup { get; set; }
        public string ProgramName { get; set; }

        public decimal SOQty { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int POQty { get; set; }

        public string ForProductNo { get; set; }

        public DateTime? ShipmentDate { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShipToETA { get; set; }
        public int ShipmentQty { get; set; }

        public string SOStatus { get; set; }
        public string QCStatus { get; set; }
        public DateTime? QCStatusDate { get; set; }
        public string QCComments { get; set; }        

    }
}
