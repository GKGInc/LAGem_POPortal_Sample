using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class Lookup
    {
        public int Id { get; set; }             // [LookupDetailId]

        public int LookupValue { get; set; }    
        public string LookupText { get; set; }  // [LookupDescription]
        public string LookupCode { get; set; }  // [LookupCode]

    }
}
