using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace MasterDetail.Models
{
    public class POData
    {
        public string PONumber { get; set; }
        public string SONumber { get; set; }
        public string ItemCatalog { get; set; }
        public decimal Units { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime FactoryCancel { get; set; }
    }
}
