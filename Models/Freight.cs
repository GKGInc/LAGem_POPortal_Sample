using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace MasterDetail.Models
{
    public class Freight // [SOTRAN]
    {
        public string PONumber { get; set; }
        public string ShipMethod { get; set; }
        public string InvoiceNo { get; set; }
        public string Tracking { get; set; }
        public string UnitsShipped { get; set; }
        public DateTime DateShipped { get; set; }
        public DateTime ETA_LA { get; set; }
        public int Id { get; set; }
        
    }
}
