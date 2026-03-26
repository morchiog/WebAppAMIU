using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebAppAMIU.DbConnector;

namespace WebAppAMIU.LogConsEcos
{
    public partial class LogConservazione : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InizializzaFiltri();
            }
        }

        protected void InizializzaFiltri()
        {
            //carico ddl reg
            var dtddlreg = MOracleHelp.ExecuteReaderEcos(null, DBAccess.DbOraEcos.SelectRegistri);

            if (dtddlreg != null && dtddlreg.Rows != null)
            {
                var dr = dtddlreg.NewRow();
                dr["txt4ddl"] = "";
                dr["ident_registro"] = "";
                dtddlreg.Rows.InsertAt(dr, 0);
                dtddlreg.AcceptChanges();
            }

            ddlReg.DataSource = dtddlreg;
            ddlReg.DataTextField = "txt4ddl";
            ddlReg.DataValueField = "ident_registro";
            ddlReg.DataBind();


            // il btn x prendere in carico è a disp. sei soli op_bonifica
            var userName = User.Identity.Name;
            var currYear = System.DateTime.Now.Year;
            var dt = System.DateTime.Now.AddDays(-60);
            data_richiesta_a.Value = currYear.ToString() + "-12-31";
            data_richiesta_da.Value = dt.ToString("yyyy-MM-dd");

            FillTable();
        }

        protected DataTable RetrieveRows(bool soloValide = false)
        {
            string whereCond = "";
            string query = DBAccess.DbWebUtilities.SelectLogConservazione;

            if (data_richiesta_da.Value != "") { whereCond += " and data_ins >= '" + data_richiesta_da.Value + "'"; }
            if (data_richiesta_a.Value != "") { whereCond += " and data_ins <= '" + data_richiesta_a.Value + "'"; }

            if (!string.IsNullOrEmpty(ddlReg.SelectedValue))
            {
                whereCond += " and nomef like '%" + ddlReg.SelectedValue.Trim() + "%'";
            }

            if (txtRicerca.Value != "") { whereCond += " and upper(coalesce(nomef,'_') || '_' || coalesce(anno,'-1') || '_' || coalesce(progda,'-1') || '_' || coalesce(proga,'-1')) like upper('%" + txtRicerca.Value + "%')"; }

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
                lblRighe.Text = "0";
            }
            else
            {
                no_data_lbl.Attributes.CssStyle.Add("display", "none");
                lblRighe.Text = dataTable.Rows.Count.ToString();
            }

            data_gridview.DataSource = dataTable; //matching della gridview con la datatable
            data_gridview.DataBind();
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("LogConservazione.aspx");
        }

        protected void BtnCerca_Click(object sender, EventArgs e)
        {
            FillTable();
        }
    }
}