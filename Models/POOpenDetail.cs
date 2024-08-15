using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class POOpenDetail
    {
        public int Id { get; set; }

        //public int BusinessPartnerId { get; set; }
        public int VendorId { get; set; }
        //public string? BusinessPartnerName { get; set; }
        public string? VendorName { get; set; }
        public string? ProgramName { get; set; }

        public int POHeaderId { get; set; }
        public string? PONumber { get; set; }
        public int PODetailId { get; set; }

        public string? ProductNo { get; set; }
        public string? ProductName { get; set; }
        public int OrderQty { get; set; }
    }
}
