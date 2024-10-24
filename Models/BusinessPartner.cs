using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class BusinessPartner
    {
        public int BusinessPartnerId { get; set; }
        public string? BusinessPartnerType { get; set; }
        public string? BusinessPartnerCode { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? DBAName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public int? RowStatus { get; set; }
    }
}
