using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace MasterDetail.Models
{
    public class Labor // [SOROUTE]
    {
        public string PONumber { get; set; }
        public string Program { get; set; }
        public string CardToUse { get; set; }
        public string BoxStyle { get; set; }
        public string Accessories { get; set; }
        public string TicketProofApproval { get; set; }
        public int Id { get; set; }
    }
}
