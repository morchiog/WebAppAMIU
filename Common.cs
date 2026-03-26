using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WebAppAMIU
{
   

    public class Common
    {
        public static string ReplaceForJs(string v)
        {
            if (string.IsNullOrWhiteSpace(v?.Trim())) { return ""; }

            v = v.Replace("'", "");
            v = v.Replace("\"", "");
            v = v.Replace("<", "");
            v = v.Replace(">", ""); 
            v = v.Replace("\r", ""); 
            v = v.Replace("\n", "");

            return v.Trim();
        }

        public static string CleanStr(string s)
        {
            return HttpUtility.HtmlDecode(s).Trim();
        }

       

        public static void ManageVisibility(WebControl ctrl, bool vis)
        {
            if (ctrl != null) { ctrl.Visible = vis; }
        }
        public static void ManageEnable(WebControl ctrl, bool vis, string tooltipIfFalse = "")
        {
            if (ctrl != null) 
            { 
                ctrl.Enabled = vis; 
                if (!string.IsNullOrEmpty(tooltipIfFalse) && !vis)
                {
                    ctrl.ToolTip = tooltipIfFalse;
                }
            }
        }

        public static string GetUsername(System.Web.UI.Page page)
        {
            var userName = page.User.Identity.Name;
            userName = userName.Replace("DSI\\", "");
            return userName.ToUpper();
        }

        public static bool IsGestore(System.Web.UI.Page page, HtmlSelect opeBonif)
        {
            var isGestore = false;
            var userName = Common.GetUsername(page);
            if (!string.IsNullOrEmpty(userName))
            {
                var opeBon = opeBonif.Items.FindByValue(userName.ToUpper());
                if (opeBon != null && opeBon?.Text?.IndexOf("(*)") < 0) { isGestore = true; }
            }
            return isGestore;
        }

        public static bool IsOperatore(string parOperatore, System.Web.UI.Page page)
        {
            var userName = page.User.Identity.Name;
            if (!string.IsNullOrEmpty(parOperatore))
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    userName = userName.Replace("DSI\\", "");
                    if (userName.ToLower() == parOperatore.ToLower()) return true;
                }
            }
            return false;
        }

   
        
    }
}