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

        public string OrderNo { get; set; }
        public string CustNo { get; set; }
        public string SourceKey { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string ShipList { get; set; }
        public decimal OrderTotal { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public int ItemQty { get; set; }
        public DateTime ItemShipDate { get; set; }
        public string ItemState { get; set; }
        public int Year { get; set; }
        public int M { get; set; }
        public string Month { get; set; }
        public int Day { get; set; }
        public string DisplayDate { get; set; }
    }
}