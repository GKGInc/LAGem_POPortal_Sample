using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class POOpenVendor
    {
        public int Id { get; set; }

        //public int BusinessPartnerId { get; set; }
        public int VendorId { get; set; }
        //public string? BusinessPartnerName { get; set; }
        public string? VendorName { get; set; }
        public int OpenPOs { get; set; }
    }
}
