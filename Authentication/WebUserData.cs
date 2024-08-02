using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAGem_POPortal.Authentication
{
    public class WebUserData
    {
        public int Oid { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserStatus { get; set; } // Role
        public string PromoCode { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string LastLogin { get; set; }
    }
}
