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
            " INSERT INTO \"SapUtility\".fornitori_sc_log (codice_fornitore, data_check, esito_check, note) " +
            "                                      VALUES(:pcodice_fornitore, current_date,:pesito_check, :pnote)";


        public static string SelectFornitoriSAP = " " +
            " select * " + 
            "  from ( " + 
            "        SELECT a.CFISC, a.piva, a.CODICE_FORNITORE , a.NOME_FORNITORE , a.NOME_BANCA , a.iban, c.data_check, coalesce(c.esito_check,'--') as esito_check, c.note " + 
            "          FROM \"SapUtility\".fornitori_sap   a " + 
            "          left join (select CODICE_FORNITORE, max(id) as max_id from \"SapUtility\".fornitori_sc_log group by CODICE_FORNITORE) b on (a.CODICE_FORNITORE = b.CODICE_FORNITORE ) " + 
            "          left join \"SapUtility\".fornitori_sc_log c on (a.CODICE_FORNITORE = c.CODICE_FORNITORE and c.id = b.max_id ) " +
            "         WHERE 1=1 {where_condition}  " +
            "       ) s " +
            " order by nome_fornitore asc ";
    }
}