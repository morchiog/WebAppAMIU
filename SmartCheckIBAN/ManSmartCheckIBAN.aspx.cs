using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WebAppAMIU.DbConnector;
using WebAppAMIU.Models.SmartCheckIBAN;


/*
  visto che non esisite un package compatibile col framework 4.6.2 ( dovrei installare un driver disponibile sul isto SAP colo con ACCOUNT SAP .. ma non ce l'ho io..)
  per ora uso blocchetto che trasferisce la  QLIK.V_FORNITORI_CHECKIBAN e scrive su PAGOPA.fornitori_SAP
  il blocchetto è su Morchiog/SAP e si chiama "dati_fornitori".. sarà da mettere in un JOB e da richiamare ogni x minuti.. o mezzora o ora etc etc 
 */

namespace WebAppAMIU.SmartCheckIBAN
{
    public partial class ManSmartCheckIBAN : System.Web.UI.Page
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
            FillTable();
        }

        protected void InsertEsitoCheck(SmartCheckResponse scr)
        {
            var pcodice_fornitore = new NpgsqlParameter("pcodice_fornitore", NpgsqlDbType.Varchar) { Value = txtCodForn.Text };
            var pesito_check = new NpgsqlParameter("pesito_check", NpgsqlDbType.Varchar);
            var pnote = new NpgsqlParameter("pnote", NpgsqlDbType.Varchar);

            if (scr.success)
            {
                pesito_check.Value = "OK";
                pnote.Value = DBNull.Value;
            }
            // se è success ,, ma qualcosa non va...
            if (scr.payload.isAllowed)
            {
                pesito_check.Value = "OK";
                pnote.Value = DBNull.Value;
            }
            else if (!scr.payload.isAllowed)
            {
                pesito_check.Value = "KO";
                pnote.Value = scr.payload.additionalInfo;
            }
            else if (scr.errors != null)
            {
                pesito_check.Value = "KO";
                pnote.Value = scr.errors[0].description;
            }
            else 
            {
                pesito_check.Value = "KO";
                pnote.Value = "Errore...";
            }

                PostgreSQLConnector.ExecuteNonQuery("PagoPa", DBAccess.DbSmartCheck.InsertFornitoreSmartCheckLog, pcodice_fornitore, pesito_check, pnote);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var certPath = Server.MapPath("") + "\\" + "CERT_64936.p12";

            var options = new RestClientOptions("https://external-api.intesasanpaolo.com");

            options.ClientCertificates = new X509CertificateCollection
                {
                    new X509Certificate2(certPath, "pulDvRghdx")
                };


            var client = new RestClient(options);

            var request = new RestRequest("/auth/oauth/v2/token", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Basic SDlvenVFVlBrbTJwd1RYUUE4ZmdhdzRraTIzdm05OVBXN05RM1JBMnhRMzhQOUl2OlRuU2VtVzNhV1NNWUo1YXg3dWNmaGFJZ0p5NzhGRTBqTjVzNkNGaTRiSGlKSXpkY0hHN0JxdzI4cTdVVnJqV3k=");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "oob");
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful) { txtAuxRes.Text = response.ErrorException.ToString(); }
            else { txtAuxRes.Text = response.Content; }

            var data = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.LoginResponse>(response.Content);

            txtAuxRes.Text += data.access_token;


            var request1 = new RestRequest("/twa/live/v1/tools/iban/chargeabilityMulti?fiscalCode=" + lblCfisc.Text + "&iban=" + lblIban.Text + "&vatNumber=" + lblPIVA.Text, Method.Get);
            request1.AddHeader("Accept", "*/*");
            request1.AddHeader("Authorization", "Bearer " + data.access_token);
            RestResponse response1 = client.Execute(request1);

            txtAuxRes.Text += response1.Content;
            var resSCI = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.SmartCheckResponse>(response1.Content);

            txtAuxRes.Text += " Risultato:" + resSCI.success.ToString();

            InsertEsitoCheck(resSCI);

            FillTable();
        }





        protected void btnSearch_Click(object sender, EventArgs e)
        {
            FillTable();
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

        protected DataTable RetrieveRows()
        {
            string whereCond = "";
            string query = DBAccess.DbSmartCheck.SelectFornitoriSAP;
            System.Data.DataTable dataTable = new System.Data.DataTable();

            var stElaSelected = false;
            var condStEla = " and (1=0 ";
            foreach (System.Web.UI.WebControls.ListItem li in chkStatoVerif.Items)
            {
                if (li.Selected)
                {
                    stElaSelected = true;
                    condStEla += " or coalesce(c.esito_check,'--') = '" + li.Value + "'";
                }
            }
            condStEla += " )";

            if (stElaSelected)
            {
                whereCond += condStEla;
            }

            if (txtRicerca.Value != "") { whereCond += " and upper(coalesce(CFISC,'_') || '_' || coalesce(piva,'_') || '_' || coalesce(iban,'_')|| '_' || coalesce(a.CODICE_FORNITORE,'_') || '_' || coalesce(NOME_FORNITORE,'_') || '_' || coalesce(NOME_BANCA,'_') || coalesce(note,'_')) like upper('%" + txtRicerca.Value + "%')"; }
 
            query = query.Replace("{where_condition}", whereCond);

            dataTable = PostgreSQLConnector.ExecuteReader("PagoPa", query);

            return dataTable;
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManSmartCheckIBAN");
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {

        }

        protected void data_gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void data_gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
    }
}