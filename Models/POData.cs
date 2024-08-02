using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class POData
    {
        public int Id { get; set; }

        public string? PONumber { get; set; }        //PO Number
        //public string? ProductNo { get; set; }       //ITEM CATALOG #
        public string? VendorName { get; set; }       //Jewelry Factory
        public string? CountryCode { get; set; }     //Country of origin
        public string? CustomerName { get; set; }     //Customer
        //public decimal? QtyOrdered { get; set; }     //Units
        public DateTime? PODate { get; set; }        //PO Order Date 
        public DateTime? EndDate { get; set; }       //Factory Cancel Date
        public DateTime? StartDate { get; set; }     //Customer Ship date
        public string? SONumber { get; set; }        //SO Number
    }
}
