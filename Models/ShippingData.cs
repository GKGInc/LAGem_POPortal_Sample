using LAGem_POPortal.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LAGem_POPortal.Models
{
    public class ShippingData
    {
        public int Id { get; set; }

        public int ShipmentHeaderId { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string InvoiceNo { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime ShipToETA { get; set; }
        //public string BusinessPartnerName { get; set; }
        public string PONumber { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public int OrderQty { get; set; }
        public int ShipmentQty { get; set; }
        public int ShipmentDetailId { get; set; }
        public int PODetailId { get; set; }
        public int ProductId { get; set; }


        public int POHeaderId { get; set; }
        public int SOHeaderId { get; set; }
        public int SODetailId { get; set; }


        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        
        public DateTime SODate { get; set; }
        public DateTime PODate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string SONumber { get; set; }


        public string ForProductNo { get; set; }
        public int LastShipmentQty { get; set; }

        public bool isNew { get; set; }
        public bool isUpdate { get; set; }
        public bool isDirty { get; set; }

    }
}