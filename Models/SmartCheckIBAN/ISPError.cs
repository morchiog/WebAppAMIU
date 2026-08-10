using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAMIU.Models.SmartCheckIBAN
{
    public class ISPError
    {
        public string code { get; set; }
        public string description { get; set; }
		public string type { get; set; }
    }
}