using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAMIU.Models.SmartCheckIBAN
{
    public class ChargeabilityResponse
    {
        public string additionalInfo { get; set; }
        public string ibanType { get; set; }
        public bool isAllowed { get; set; }
    }
}