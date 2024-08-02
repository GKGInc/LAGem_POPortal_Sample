using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class FreightData // 
    {
        public int Id { get; set; }

        public string PONumber { get; set; }
        public string ShipMethod { get; set; }
        public string InvoiceNo { get; set; }
        public string Tracking { get; set; }
        public string ShipmentQty { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime ShipToETA { get; set; }
    }
}
