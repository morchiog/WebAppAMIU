using Npgsql;
using System;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using System.Web;
using WebAppAMIU.DbConnector;


namespace WebAppAMIU.SapTrasporti
{

    public partial class GestioneAllegatiST : System.Web.UI.Page
    {
        public enum colListManAllegati
        {
            idOgg = 0,
            percorso,
            nome,
            tipoDescr,
            id,
            bottoni,
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //if (!(Common.IsOperatore(hdOperatore.Value, this.Page)))
            //{
            //    divLoadDoc.Visible = false;
            //}

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Session["REFRESH_AFTER_POPUP"] = "true";
            if (!IsPostBack)
            {
                hdId.Value = Request.Params["id"];
                hdOpeAdminSiNo.Value = Request.Params["opeAdmin"];

                InizializzaFiltri();
            }
        }

        protected void EventTable(object sender, EventArgs e)
        {
            FillTable();
        }

   

        protected System.Data.DataTable RetrieveRows() //bool soloValide = false)
        {
           
            var pid_trasp = new NpgsqlParameter("pid_trasp", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(hdId.Value) };

            string query = DBAccess.DbWebUtilities.SelectSTFile;            

            var dataTable = PostgreSQLConnector.ExecuteReader("AMIU_WEB_UTILITIES", query, pid_trasp);

            return dataTable;
        }

        protected void FillTable()
        {

            var dataTable = RetrieveRows();

            if (dataTable.Rows.Count < 1)//se la tabella è vuota viene mostrato un messaggio di informazione
            {
                lblHelpDescr.Text = "Carica primo Documento!";
            }

            data_gridview.DataSource = dataTable; //matching della gridview con la datatable
            data_gridview.DataBind();
        }

        //inizializza la tabella del filtro anno
        protected void InizializzaFiltri()
        {
            FillTable();
        }

        protected void ReloadPage(object sender, EventArgs e)
        {

        }

        #region gestione gridview

        protected void Update_table(object sender, GridViewUpdateEventArgs e) //funzione che abilita la modifica e rimanda alla pagina inerente al tipo di riduzione
        {
            FillTable();
        }

        protected void Data_gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cancella")
            {
                var pid = new NpgsqlParameter("pid", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(e.CommandArgument.ToString()) };

                PostgreSQLConnector.ExecuteNonQuery("AMIU_WEB_UTILITIES", DBAccess.DbWebUtilities.DeleteSTFile, pid);
            }
            if (e.CommandName == "download")
            {
                try
                {
                    // il command argument è una roba tipo: percorso;nome;estensione;tipo
                    var arrCA = e.CommandArgument.ToString().Split(';');
                    var cartella = arrCA[0];
                    var fileName = arrCA[1];
                    var estensione = arrCA[2];
                    var tipoFile = arrCA[3];

                    var fullFileName = ConfigurationManager.AppSettings["CartellaAllegati"] + "\\" + cartella + "\\" + fileName;

                    FileInfo objFile = new System.IO.FileInfo(fullFileName);

                    if (!objFile.Exists) { throw new InvalidOperationException("Documento Non trovato (potrebbe esser stato spostato o cancellato a mano?)"); }

                    if (estensione.ToLower() == "pdf") { Response.ContentType = "application/pdf"; }
                    if (estensione.ToLower() == "jpg" || estensione.ToLower() == "jpeg") { Response.ContentType = "image/jpeg"; }
                    if (estensione.ToLower() == "gif") { Response.ContentType = "image/gif"; }
                    if (estensione.ToLower() == "png") { Response.ContentType = "image/png"; }
                    if (estensione.ToLower() == "docx") { Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; }
                    if (estensione.ToLower() == "xlsx") { Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }

                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.AddHeader("Content-Length", objFile.Length.ToString());
                    Response.TransmitFile(fullFileName);
                    Response.End();
                }
                catch (InvalidOperationException ee)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "error", "<script language=\"javascript\">AlertError('" + ee.Message + "')</script>");
                }
                catch (Exception)
                {
                    throw;
                }
            }

            FillTable();
        }

        // se la data_cons_tecn è scritta, la planimetria si può cancellare?
        // se la dataRestituzione è scritta, la planimetria si può cancellare?

        protected void Data_gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// To check condition on integer value
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (!( Common.IsOperatore(hdOperatore.Value, this.Page)))
                //{
                //    var imgDel = (ImageButton)e.Row.Cells[Convert.ToInt32(colListManAllegati.bottoni)].FindControl("imgDelete");
                //    imgDel.Enabled = false;
                //    imgDel.ToolTip = "la cancellazione è risevata a operatore del Allineamento!";
                //    imgDel.ImageUrl = "~/Images/delete_doc_disabled.png";
                //}
            }
        }
        #endregion

        protected void BtnFileUpl_Click(object sender, EventArgs e)
        {
            try
            {
                var ext = "";

                if (!UplDoc.HasFile) { throw new InvalidOperationException("Selezionare un file!!!"); }

               

                var posPoint = UplDoc.FileName.LastIndexOf(".");
                if (posPoint > 0)
                {
                    ext = UplDoc.FileName.Substring(posPoint + 1);
                }
                var nomeDir = System.DateTime.Now.Year.ToString() + "_" + System.DateTime.Now.Month.ToString().PadLeft(2, '0') + System.DateTime.Now.ToString("MMMM");

                DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["CartellaAllegati"]);
                if (!dir.Exists) { throw new Exception("Cartella degli allegati agli allineamenti Anagrafici non trovata!"); }

                dir = new DirectoryInfo(ConfigurationManager.AppSettings["CartellaAllegati"] + "\\" + nomeDir);
                if (!dir.Exists) { dir.Create(); }

                // salvare FS su filesystem
                var fullFileName = ConfigurationManager.AppSettings["CartellaAllegati"] + "\\" + nomeDir + "\\" + UplDoc.FileName;

                var pid_trasp = new NpgsqlParameter("pid_trasp", NpgsqlTypes.NpgsqlDbType.Integer) { Value = int.Parse(hdId.Value) };
                var ppercorso_file = new NpgsqlParameter("ppercorso_file", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = nomeDir };
                var pestensione = new NpgsqlParameter("pestensione", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ext };
                var ptipo = new NpgsqlParameter("ptipo", NpgsqlTypes.NpgsqlDbType.Integer) { Value = 0 };
                var pnome_file = new NpgsqlParameter("pnome_file", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = UplDoc.FileName };

                UplDoc.SaveAs(fullFileName);

                PostgreSQLConnector.ExecuteNonQuery("AMIU_WEB_UTILITIES", DBAccess.DbWebUtilities.InsertSTFile, ptipo, pestensione, pid_trasp,pnome_file,ppercorso_file);               

                FillTable();
                ReloadPage(sender, e);
            }
            catch (InvalidOperationException ee)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "error", "<script language=\"javascript\">AlertError('" + ee.Message + "')</script>");
            }
            catch (Exception)
            { throw; }
        }
    }
}