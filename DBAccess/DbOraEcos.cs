using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAMIU.DBAccess
{
    public class DbOraEcos
    {
        public static string SelectRegistri = " " +
            "  select distinct  b.des_registro, b.ident_registro,  b.ident_registro || ' - ' || b.des_registro  || ' - ' || d.imp_ragsoc as txt4ddl " +
			"  from bsreregbloc a  " +
			"  join bsrereg     b on (a.id_bsrereg = b.id) " +
			"  left join bsreregass  c on (a.id_bsrereg = c.id_bsrereg) " +
			"  left join bsimp       d on (c.imp_cod = d.imp_cod and c.imp_uni = d.imp_uni) " +
			" where 1=1  	order by 1,2 asc ";
    }
}