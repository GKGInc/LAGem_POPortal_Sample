using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class GeneralDetailData
    {
        public int Id { get; set; }

        public int PODetailId { get; set; }
        public string PONumber { get; set; }        //PO Number
        public int POHeaderId { get; set; }
        public int POLineNo { get; set; }

        public int ProductId { get; set; }
        public string ProductNo { get; set; }       //ITEM CATALOG #

        public int BusinessPartnerId_Vendor { get; set; }
        public string VendorName { get; set; }       //Jewelry Factory
        public int BusinessPartnerId_Customer { get; set; }
        public string CustomerName { get; set; }     //Customer
        public string CountryCode { get; set; }     //Country of origin

        public decimal QtyOrdered { get; set; }     //Units
        public DateTime? PODate { get; set; }        //PO Order Date 
        public DateTime? EndDate { get; set; }       //Factory Cancel Date
        public DateTime? StartDate { get; set; }     //Customer Ship date

        public string SONumber { get; set; }        //SO Number
        public int SOLineNo { get; set; }
        public int SOSubLineNo { get; set; }
        public decimal SOLineNoExt { get; set; }        
        public int SOSubLineTypeId { get; set; }
        public string SOSubLineType { get; set; }

    }
}
