using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAMIU.Models.SmartCheckIBAN
{
    public class SmartCheckResponse
    {
        public bool success { get; set; }
        public ChargeabilityResponse payload { get; set; }
		public List<ISPError> errors { get; set; }
    }
}