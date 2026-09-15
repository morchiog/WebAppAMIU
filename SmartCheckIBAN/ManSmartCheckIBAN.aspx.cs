using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public enum ColListFornitore
        {
            btn = 0,
            cod_forn,
            nome,
            cfisc,
            piva,
            nomebanca,
            Iban,
            datacheck,
            esitoCheck,
            notecheck,
            datacheck2,
            esitoCheck2,
            notecheck2,
            progr,
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!Common.IsSmartCheckAllowed(this.Page)) { Response.Redirect("../Default"); }

                InizializzaFiltri();
            }
        }

        protected void InizializzaFiltri()
        {
            FillTable();
        }

        protected void InsertEsitoCheck(SmartCheckResponse scr, string sCodForn, string tbp)
        {
            var pcodice_fornitore = new NpgsqlParameter("pcodice_fornitore", NpgsqlDbType.Varchar) { Value = sCodForn };
            var ptipo_banca_partner = new NpgsqlParameter("ptipo_banca_partner", NpgsqlDbType.Varchar) { Value = tbp };
            var pesito_check = new NpgsqlParameter("pesito_check", NpgsqlDbType.Varchar);
            var pnote = new NpgsqlParameter("pnote", NpgsqlDbType.Varchar);

            if (scr.success)
            {
                pesito_check.Value = "OK";
                pnote.Value = DBNull.Value;
            }
            // se è success ,, ma qualcosa non va...
            if (scr.success && scr.payload != null && scr.payload.isAllowed)
            {
                pesito_check.Value = "OK";
                pnote.Value = DBNull.Value;
            }
            else if (scr.payload != null && !scr.payload.isAllowed)
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

            PostgreSQLConnector.ExecuteNonQuery("PagoPa", DBAccess.DbSmartCheck.InsertFornitoreSmartCheckLog, pcodice_fornitore, ptipo_banca_partner, pesito_check, pnote);
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

            if (response.Content == null)
            {
                var sAux = "content Vuoto  " + txtAuxRes.Text;
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('Errore: " + Common.CleanStr(Common.ReplaceForJs(sAux)), true);
                return;
            }

            var data = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.LoginResponse>(response.Content);

            txtAuxRes.Text += data.access_token;

            var res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?fiscalCode=" + lblCfisc.Text + "&iban=" + lblIban.Text + "&vatNumber=" + lblPIVA.Text;

            if (string.IsNullOrEmpty(lblCfisc.Text.Trim())) // caso senza CF
            {
                res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + lblIban.Text + "&vatNumber=" + lblPIVA.Text;
            }
            else if (lblCfisc.Text.Trim().Length == 11) // caso senzA PIVA ma con CFISC che è P.IVA
            {
                res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + lblIban.Text + "&vatNumber=" + lblCfisc.Text.Trim();
            }
            else if (lblCfisc.Text.Trim() == lblPIVA.Text.Trim()) // caso CF uguale a PIVA
            {
                res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + lblIban.Text + "&vatNumber=" + lblPIVA.Text;
            }
            else if (string.IsNullOrEmpty(lblPIVA.Text.Trim()) && lblCfisc.Text.Trim().Length == 11) // caso senzA PIVA ma con CFISC che è P.IVA
            {
                res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + lblIban.Text + "&vatNumber=" + lblCfisc.Text.Trim();
            }
            else if (string.IsNullOrEmpty(lblPIVA.Text.Trim())) // caso senzA PIVA
            {
                res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?fiscalCode=" + lblCfisc.Text + "&iban=" + lblIban.Text;
            }

            var request1 = new RestRequest(res2Call, Method.Get);
            request1.AddHeader("Accept", "*/*");
            request1.AddHeader("Authorization", "Bearer " + data.access_token);
            RestResponse response1 = client.Execute(request1);

            txtAuxRes.Text += response1.Content;
            var resSCI = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.SmartCheckResponse>(response1.Content);

            txtAuxRes.Text += " Risultato:" + resSCI.success.ToString();

            var scr = resSCI;

            // se è success ,, ma qualcosa non va...
            if (scr.success && scr.payload != null && scr.payload.isAllowed)
            {
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('OK')", true);
            }
            else if (scr.payload != null && !scr.payload.isAllowed)
            {
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('KO "  +  Common.CleanStr(Common.ReplaceForJs(scr.payload.additionalInfo)) +  "  ')", true);
            }
            else if (scr.errors != null)
            {
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('KO " + Common.CleanStr(Common.ReplaceForJs(scr.errors[0].description)) + "  ')", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('KO -- ERRORE!  ')", true);
            }
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
                lblRigheSel.Text = "0";
            }
            else
            {
                no_data_lbl.Attributes.CssStyle.Add("display", "none");
                lblRigheSel.Text = dataTable.Rows.Count.ToString();
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

            if (txtRicerca.Value != "") { whereCond += " and upper(coalesce(v.CFISC,'_') || '_' || coalesce(v.piva,'_') || '_' || coalesce(v.iban,'_')|| '_' || coalesce(v.CODICE_FORNITORE,'_') || '_' || coalesce(v.NOME_FORNITORE,'_') || '_' || coalesce(v.NOME_BANCA,'_') || coalesce(c.note,'_')|| coalesce(d.note,'_')) like upper('%" + txtRicerca.Value + "%')"; }

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
            var dt = RetrieveRows();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var row = 2;
            var fi = new FileInfo(Server.MapPath("/SmartCheckIBAN/ExportSCI.xlsx"));


            MemoryStream stream = new MemoryStream();
            if (fi.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(Server.MapPath("/SmartCheckIBAN/ExportSCI.xlsx")))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    foreach (DataRow dr in dt.Rows)
                    {
                        var col = 1;
                        worksheet.Cells[row, col++].Value = dr["codice_fornitore"].ToString();
                        worksheet.Cells[row, col++].Value = dr["NOME_FORNITORE"].ToString();

                        worksheet.Cells[row, col++].Value = dr["CFISC"].ToString();
                        worksheet.Cells[row, col++].Value = dr["PIVA"].ToString();
                        worksheet.Cells[row, col++].Value = dr["NOME_BANCA"].ToString();
                        worksheet.Cells[row, col++].Value = dr["IBAN"].ToString();

                        worksheet.Cells[row, col++].Value = PostgreSQLConnector.ConvDate2String(dr, "last1_data_check", "dd-MM-yyyy");
                        worksheet.Cells[row, col++].Value = dr["last1_esito_check"].ToString();
                        worksheet.Cells[row, col++].Value = dr["last1_note"].ToString();
                        worksheet.Cells[row, col++].Value = PostgreSQLConnector.ConvDate2String(dr, "last2_data_check", "dd-MM-yyyy");
                        worksheet.Cells[row, col++].Value = dr["last2_esito_check"].ToString();
                        worksheet.Cells[row, col++].Value = dr["last2_note"].ToString();

                        row++;
                    }

                    p.SaveAs(stream);
                    stream.Position = 0;

                    Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment;filename=CheckIbanFornitori_{DateTime.Now:yyyyMMddhhmmss}.xlsx");
                    Response.AddHeader("Content-Length", stream.Length.ToString());
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }

        protected void data_gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void data_gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Trim().ToLower() == "verifica")
            {
                var arrArgLst = e.CommandArgument.ToString().Split(';');
                var codForn = arrArgLst[0].Trim();
                var tbp_progr = arrArgLst[1].Trim();
                var codFisc = arrArgLst[2].Trim();
                var pIva = arrArgLst[3].Trim();
                var sIban = arrArgLst[4].Trim();

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

                if (response.Content == null)
                {
                    var sAux = "content Vuoto  " + txtAuxRes.Text;
                    txtAuxRes.Text = sAux;

                    ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('Errore: " + Common.CleanStr(Common.ReplaceForJs(sAux)), true);
                    return;
                }

                var data = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.LoginResponse>(response.Content);

                txtAuxRes.Text += data.access_token;

                var res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?fiscalCode=" + codFisc + "&iban=" + sIban + "&vatNumber=" + pIva;

                if (string.IsNullOrEmpty(codFisc)) // caso senza CF
                {
                    res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + sIban + "&vatNumber=" + pIva;
                }
                else if (codFisc.Length == 11) // caso senzA PIVA ma con CFISC che è P.IVA
                {
                    res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + sIban + "&vatNumber=" + codFisc;
                }
                else if (codFisc == pIva) // caso CF uguale a PIVA
                {
                    res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + sIban + "&vatNumber=" + pIva;
                }
                else if (string.IsNullOrEmpty(pIva) && codFisc.Length == 11) // caso senzA PIVA ma con CFISC che è P.IVA
                {
                    res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?iban=" + sIban + "&vatNumber=" + codFisc;
                }
                else if (string.IsNullOrEmpty(pIva)) // caso senzA PIVA
                {
                    res2Call = "/twa/live/v1/tools/iban/chargeabilityMulti?fiscalCode=" + codFisc + "&iban=" + sIban;
                }

                var request1 = new RestRequest(res2Call, Method.Get);
                request1.AddHeader("Accept", "*/*");
                request1.AddHeader("Authorization", "Bearer " + data.access_token);
                RestResponse response1 = client.Execute(request1);

                txtAuxRes.Text += response1.Content;
                var resSCI = System.Text.Json.JsonSerializer.Deserialize<Models.SmartCheckIBAN.SmartCheckResponse>(response1.Content);

                txtAuxRes.Text += " Risultato:" + resSCI.success.ToString();

                InsertEsitoCheck(resSCI, codForn, tbp_progr);

                FillTable();
            }
        }
    }
}