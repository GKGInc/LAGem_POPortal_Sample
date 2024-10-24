using DevExpress.Drawing.Internal.Fonts.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;


namespace LAGem_POPortal.Models
{
    public class SOData // 
    {
        public int Id { get; set; }

        public int SOHeaderId { get; set; }
        public string SONumber { get; set; }

        public int BusinessPartnerId_Customer { get; set; }        
        public string CustomerName { get; set; }
        public string CountryCode { get; set; }

        public int SalesProgramId { get; set; }
        public string ProgramName { get; set; }
        public string CustomerPO { get; set; }

        public decimal TotalCost { get; set; }
        public decimal TotalPrice { get; set; }
        
        public DateTime? SODate { get; set; }        //PO Order Date 
        public DateTime? StartDate { get; set; }     //Customer Ship date
        public DateTime? EndDate { get; set; }       //Factory Cancel Date

        public bool PostedToERP { get; set; }
        public DateTime? PostedDate { get; set; }
    }
}
