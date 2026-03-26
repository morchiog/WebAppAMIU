using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebAppAMIU.DbConnector;

namespace WebAppAMIU.SapTrasporti
{
    public partial class ListaSapTrasporti : System.Web.UI.Page
    {
        public enum codList
        {
            id = 0,
            data_richiesta,
            utente,
            trasporto,
            ticket,
            nota,
            azioni,
            allegati
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InizializzaFiltri();
            }
        }

        protected void InizializzaFiltri()
        {
            // il btn x prendere in carico è a disp. sei soli op_bonifica
            var userName = User.Identity.Name;
            var currYear = System.DateTime.Now.Year;
            var dt = System.DateTime.Now.AddDays(-60);
            data_richiesta_a.Value = currYear.ToString() + "-12-31"; // arrivo fino a fine anno corrente
            data_richiesta_da.Value = dt.ToString("yyyy-MM-dd");

            FillTable();
        }

        protected DataTable RetrieveRows(bool soloValide = false)
        {
            string whereCond = "";
            string query = DBAccess.DbWebUtilities.SelectListaSapTrasporti;

            if (data_richiesta_da.Value != "") { whereCond += " and data_ins >= '" + data_richiesta_da.Value + "'"; }
            if (data_richiesta_a.Value != "") { whereCond += " and data_ins <= '" + data_richiesta_a.Value + "'"; }


            if (txtRicerca.Value != "") { whereCond += " and upper(coalesce(num_trasp,'_') || '_' || coalesce(ticket,'_') || '_' || coalesce(nota,'_')) like upper('%" + txtRicerca.Value + "%')"; }

            query = query.Replace("{where_condition}", whereCond);

            var dt = PostgreSQLConnector.ExecuteReader("AMIU_WEB_UTILITIES", query);

            return dt;
        }


        protected void FillTable()
        {
            var dataTable = RetrieveRows();

            if (dataTable.Rows.Count < 1)//se la tabella è vuota viene mostrato un messaggio di informazione
            {
                no_data_lbl.Attributes.CssStyle.Add("display", "block");
            }
            else
            {
                no_data_lbl.Attributes.CssStyle.Add("display", "none");
            }

            data_gridview.DataSource = dataTable; //matching della gridview con la datatable
            data_gridview.DataBind();
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("ListaSapTrasporti.aspx");
        }

        protected void BtnCerca_Click(object sender, EventArgs e)
        {
            FillTable();
        }



        public NpgsqlParameter[] GetParameters()
        {
    
            var pnota = new NpgsqlParameter("pnota", NpgsqlDbType.Varchar) { Value = txtNote.Text };
            var pnum_trasp = new NpgsqlParameter("pnum_trasp", NpgsqlDbType.Varchar) { Value = txtNumTrasp.Text };
            var pticket = new NpgsqlParameter("pticket", NpgsqlDbType.Varchar) { Value = txtTicket.Text };

            var putente_ins = new NpgsqlParameter("putente_ins", NpgsqlDbType.Varchar) { Value = Common.GetUsername(this.Page) };
            var pdata_ins = new NpgsqlParameter("pdata_ins", NpgsqlDbType.Date) { Value = System.DateTime.Now };
            var pid = new NpgsqlParameter("pid", NpgsqlDbType.Integer) { Value = int.Parse(hdId.Value) };


            NpgsqlParameter[] parameters = { pid, putente_ins, pdata_ins, pnota, pnum_trasp, pticket };

            return parameters;
        }


        protected void btnHiddenSave_Click(object sender, EventArgs e)
        {
            var qry = DBAccess.DbWebUtilities.UpdateSapTrasporti;
            if (hdMode.Value == "I") { qry = DBAccess.DbWebUtilities.InsertSapTrasporti; }

            PostgreSQLConnector.ExecuteNonQuery("AMIU_WEB_UTILITIES", qry, GetParameters());

            FillTable();
        }

        protected void Data_gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "download")
            {
                // il command argument è una roba tipo: 
                //fld{2024_08agosto},nome{P313-2024 BERNABOVI.pdf},#{1}},est{pdf}},t{0}
                var arrCA = e.CommandArgument.ToString().Split(',');
                var cartella = arrCA[0].Replace("fld{", "").Replace("}", "");
                var fileName = arrCA[1].Replace("nome{", "").Replace("}", "");
                var estensione = arrCA[3].Replace("est{", "").Replace("}", "");
                //var tipoFile = arrCA[4].Replace("t{", "").Replace("}", "");

                var fullFileName = ConfigurationManager.AppSettings["CartellaAllegati"] + "\\" + cartella + "\\" + fileName;

                FileInfo objFile = new System.IO.FileInfo(fullFileName);
                if (!objFile.Exists) { throw new InvalidOperationException("Documento " + fileName + " non è stato trovato sul repository server!!"); }

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

        }


        protected void Data_gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // To check condition on integer value
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                // gestione apertura paginetta allegati
                var userName = User.Identity.Name;

                var imgManAll = (ImageButton)e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("imgManAllegati");

                var lblId = (Label)e.Row.Cells[Convert.ToInt32(codList.id)].FindControl("lblId");

                imgManAll.OnClientClick = "OpenManAllegati('" + lblId.Text + "','si'); return false;";

                //controllo :  se esiste planimetria ne permetto lo scaricamento
                var txtLitAll = (System.Web.UI.WebControls.TextBox)e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("txtListaImg");
                e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("imgBtnDwldPl").Visible = false;
                if (!string.IsNullOrEmpty(txtLitAll.Text))
                {
                    if (txtLitAll.Text.IndexOf("t{0}") >= 0)
                    { e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("imgBtnDwldPl").Visible = true; }

                    var arrLst = txtLitAll.Text.Split(';');
                    var imgBtn = (ImageButton)e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("imgManAllegati");
                    var lblMAll = (System.Web.UI.WebControls.Label)e.Row.Cells[Convert.ToInt32(codList.allegati)].FindControl("lblNumAllegati");
                    if (imgBtn != null)
                    {
                        if (arrLst.Length == 0)
                        { imgBtn.ToolTip = "Non sono presenti allegati!"; }

                        lblMAll.Text = arrLst.Length.ToString();

                        if (arrLst.Length == 1)
                        {
                            imgBtn.ToolTip = "E' presente un allegato!";
                        }
                        if (arrLst.Length > 1)
                        {
                            imgBtn.ToolTip = "Sono presenti " + arrLst.Length.ToString() + " allegati!";
                        }
                    }
                }
            }

            //var operatore = e.Row.Cells[Convert.ToInt32(codList.utente)].Text;

            //var IsGestoreOrIsOperatore = Common.IsOperatore(operatore, this.Page);
            //if (!IsGestoreOrIsOperatore)
            //{
            //    ((LinkButton)e.Row.Cells[Convert.ToInt32(codList.azioni)].FindControl("btnEdit")).Text = "Visualizza";
            //    ((LinkButton)e.Row.Cells[Convert.ToInt32(codList.azioni)].FindControl("btnEdit")).ToolTip = "Modifica Permessa solo a Operatore!";
            //}
        }

    }
}