using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace WebAppAMIU.DBAccess
{
    public class DbSmartCheck
    {
        public static string InsertFornitoreSmartCheckLog = " " +
            " INSERT INTO \"SapUtility\".fornitori_sc_log (codice_fornitore, tipo_banca_partner, data_check, esito_check, note) " +
            "                                      VALUES(:pcodice_fornitore, :ptipo_banca_partner, current_date,:pesito_check, :pnote)";


        public static string SelectFornitoriSAPBck = " " +
            " select * " + 
            "  from ( " + 
            "        SELECT a.CFISC, a.piva, a.CODICE_FORNITORE , a.NOME_FORNITORE , a.NOME_BANCA , a.iban, c.data_check, coalesce(c.esito_check,'--') as esito_check, c.note " + 
            "          FROM \"SapUtility\".fornitori_sap   a " + 
            "          left join (select CODICE_FORNITORE, max(id) as max_id from \"SapUtility\".fornitori_sc_log group by CODICE_FORNITORE) b on (a.CODICE_FORNITORE = b.CODICE_FORNITORE ) " + 
            "          left join \"SapUtility\".fornitori_sc_log c on (a.CODICE_FORNITORE = c.CODICE_FORNITORE and c.id = b.max_id ) " +
            "         WHERE 1=1 {where_condition}  " +
            "       ) s " +
            " order by nome_fornitore asc ";

        public static string SelectFornitoriSAP = " " +
            " select v.tipo_banca_partner,  v.CFISC, v.piva, v.CODICE_FORNITORE , v.NOME_FORNITORE , v.NOME_BANCA , v.iban, c.id as last1_id , c.data_check as last1_data_check, c.esito_check as last1_esito_check, c.note as last1_note, d.id as last2_id, d.data_check as last2_data_check, d.esito_check as last2_esito_check, d.note as last2_note " + 
            "   from \"SapUtility\".fornitori_sap v " + 
            "   left join ( " + 
            "              select *  " + 
            "                from(   " +
            "                     SELECT  a.tipo_banca_partner, a.CFISC, a.piva, a.CODICE_FORNITORE , a.NOME_FORNITORE , a.NOME_BANCA , a.iban, b.id, row_number() over (partition by b.CODICE_FORNITORE, b.tipo_banca_partner  order by b.id desc) as row_number_aux " + 
            "                       FROM \"SapUtility\".fornitori_sap   a " +
            "                       left join \"SapUtility\".fornitori_sc_log b on (a.CODICE_FORNITORE = b.CODICE_FORNITORE and a.tipo_banca_partner = b.tipo_banca_partner ) " + 
            "                    ) x " + 
            "               where  coalesce(x.row_number_aux,1) in (1)  " +
            "             ) z on (v.codice_fornitore = z.codice_fornitore and v.tipo_banca_partner = z.tipo_banca_partner) " + 
            "   left join (     " + 
            "              select * " + 
            "                from( " +
            "                     SELECT  a.tipo_banca_partner, a.CFISC, a.piva, a.CODICE_FORNITORE , a.NOME_FORNITORE , a.NOME_BANCA , a.iban, b.id, row_number() over (partition by b.CODICE_FORNITORE, b.tipo_banca_partner  order by b.id desc) as row_number_aux " + 
            "                       FROM \"SapUtility\".fornitori_sap   a  " +
            "                       left join \"SapUtility\".fornitori_sc_log b on (a.CODICE_FORNITORE = b.CODICE_FORNITORE and a.tipo_banca_partner = b.tipo_banca_partner ) " + 
            "                    ) x " + 
            "               where x.row_number_aux in (2) " +
            "             ) w on (v.codice_fornitore = w.codice_fornitore and v.tipo_banca_partner = w.tipo_banca_partner) " +
            "  left join \"SapUtility\".fornitori_sc_log c on (v.CODICE_FORNITORE = z.CODICE_FORNITORE and v.tipo_banca_partner = z.tipo_banca_partner and z.id=c.id ) " +
            "  left join \"SapUtility\".fornitori_sc_log d on (v.CODICE_FORNITORE = w.CODICE_FORNITORE and v.tipo_banca_partner = w.tipo_banca_partner and  w.id=d.id  ) " +
            "  WHERE 1=1 {where_condition} " +
            "  order by  v.nome_fornitore asc , v.tipo_banca_partner desc  ";


    }
}