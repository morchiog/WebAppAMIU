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
                //   InizializzaFiltri();
            }
        }

        protected void InizializzaFiltri()
        {

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

            txtAuxRes.Text +=  " Risultato:" + resSCI.success.ToString();

        }

        protected void grdfornitore_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName=="sel")
            {
                lblCfisc.Text = e.Item.Cells[2].Text;
                lblPIVA.Text = e.Item.Cells[3].Text;
                lblIban.Text = e.Item.Cells[5].Text;
            }
        }

        protected void btnTrigForn_Click(object sender, EventArgs e)
        {
            string strSrc;
            try
            {

                grdfornitore.DataSource = null;
                grdfornitore.DataBind();
                var strSql = new StringBuilder();

                if (txt_fornitore.Text.Trim().ToUpper().Length<= 2) 
                {
                    lblError.Visible = true;
                    lblError.Text = "Inserire più di 2 caratteri...";
                    return;
                }
                lblError.Visible = false;


                strSql.Append(" SELECT  CFISC, piva, CODICE_FORNITORE , NOME_FORNITORE , NOME_BANCA , iban  FROM \"SapUtility\".fornitori_sap ");
                strSql.Append(" WHERE upper(cfisc || piva || nome_fornitore) LIKE '%{0}%' ");

                //strSrc = txt_oggetto.Value.Replace(" ", "%");
                strSrc = txt_fornitore.Text.Trim().ToUpper().Replace(" ", "%");

                var dt = PostgreSQLConnector.ExecuteReader("PagoPa",  strSql.ToString().Replace("{0}", strSrc));
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    grdfornitore.DataSource = new DataView(dt);
                    grdfornitore.DataBind();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

      
    }
}