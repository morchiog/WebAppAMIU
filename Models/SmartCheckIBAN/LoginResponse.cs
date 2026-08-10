using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAMIU.Models.SmartCheckIBAN
{
    public class LoginResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }
}