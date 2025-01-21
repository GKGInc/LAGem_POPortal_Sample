using DevExpress.Drawing.Internal.Fonts.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;


namespace LAGem_POPortal.Models
{
    public class SoEdiData 
    {
        public int Id { get; set; }

        // Header Data

        public int IsLinked { get; set; }

        public string CustomerName { get; set; }    // Header/Detail
        public string CustomerPO { get; set; }      // Header/Detail
        public string RefPonum { get; set; }

        public string SONumber { get; set; }    // Header/Detail

        public int ShipYear { get; set; }
        public int ShipMonth { get; set; }
        public int ShipWeek { get; set; }
        public DateTime? MondayOfTheWeek { get; set; }

        public DateTime? StartDate { get; set; }     //Customer Ship date       // Header/Detail
        public DateTime? EndDate { get; set; }       //Factory Cancel Date      // Header/Detail

        public int OrderQty { get; set; }   // Header/Detail
        public int SOHeaderId { get; set; }
        public int Edihdrid { get; set; }   // Header/Detail

        // Detail Data
        public int Editrnid { get; set; }   
        public int SoDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string ItemNo { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }


    }
}
