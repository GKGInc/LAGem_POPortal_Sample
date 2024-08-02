using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAGem_POPortal.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string PromoCode { get; set; }
        public int Oid { get; set; }
    }
}
