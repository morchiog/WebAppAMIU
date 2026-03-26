using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace WebAppAMIU.DBAccess
{
    public class DbWebUtilities
    {
        public static string SelectLogConservazione = " " +
            " SELECT short_filename, tipo, nomef, anno, progda, proga, data_ins " +
            "   FROM \"AMIUWebUtilities\".ecos_log_conservazione " +
            "  WHERE 1=1 {where_condition} order by 3,4,5 ";


        // trasporti SAP FILE
        public static string SelectSTFile = " " +
            " SELECT id, id_trasp, percorso_file, estensione, tipo, nome_file, '' as tipodescr " +
            "   FROM \"AMIUWebUtilities\".trasporti_sap_file " +
            "  WHERE id_trasp = :pid_trasp ";

        public static string DeleteSTFile = " " +
            " DELETE FROM \"AMIUWebUtilities\".trasporti_sap_file WHERE id=:pid";

        public static string InsertSTFile = " " +
            "  INSERT INTO \"AMIUWebUtilities\".trasporti_sap_file (id_trasp, percorso_file, estensione, tipo, nome_file) " +
            "                                      VALUES(:pid_trasp, :ppercorso_file, :pestensione, :ptipo, :pnome_file); ";
       
        // trasporti SAP  testata
        public static string SelectListaSapTrasporti = " " +
            " SELECT a.id, data_ins, utente_ins, num_trasp, ticket, nota " +
            "        , STRING_AGG ('fld{'||b.percorso_file || '},nome{'|| b.nome_file || '},#{'||b.id||'}'|| '},est{'||b.estensione||'}' || '},t{'||b.tipo||'}', ';' order by b.tipo) as lista_allegati" +
            "   FROM \"AMIUWebUtilities\".trasporti_sap a " +
            "   LEFT JOIN \"AMIUWebUtilities\".trasporti_sap_file b on (b.id_trasp = a.id) " +
            "  WHERE 1=1 {where_condition} group by a.id, data_ins, utente_ins, num_trasp, ticket, nota order by a.id desc";

        public static string InsertSapTrasporti = " " +
            " INSERT INTO \"AMIUWebUtilities\".trasporti_sap ( data_ins, utente_ins, num_trasp, ticket, nota) " +
            "                              VALUES( :pdata_ins, :putente_ins, :pnum_trasp, :pticket, :pnota)";

        public static string UpdateSapTrasporti = " " +
            " UPDATE \"AMIUWebUtilities\".trasporti_sap " +
            "    SET num_trasp = :pnum_trasp " +
            "      , ticket    = :pticket    " +
            "      , nota      = :pnota      " +
            "  WHERE id= :pid";


    }
}