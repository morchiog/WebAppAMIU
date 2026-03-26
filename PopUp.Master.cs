 
using System;
using System.Web;
using System.Web.UI;

namespace WebAppAMIU
{
    public partial class PopUpMaster : MasterPage
    {
        #region Variables

   

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //lblAppTitle.Text = "Liste TARI";

            //if (Session[GestListeTARI.Constants.SessionVariables.POSTGRE_DB_CONN] != null)
            //{
            //    pgDbConn = (PostgreSQLConnector)Session[GestListeTARI.Constants.SessionVariables.POSTGRE_DB_CONN];
            //}
            //else
            //{
            //    pgDbConn = new PostgreSQLConnector("AMIU_WEB_UTILITIES");
            //    Session[GestListeTARI.Constants.SessionVariables.POSTGRE_DB_CONN] = pgDbConn;
            //}

            //loggedUser = HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper();

            ////abilito i vari menu/funzionalità in base al ruolo
            //Ruolo currentRole = Utilities.GeneralUtils.GetRole(pgDbConn, loggedUser);
            
        }
    }
}