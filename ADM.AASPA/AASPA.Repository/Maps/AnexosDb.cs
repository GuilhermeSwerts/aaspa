using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class AnexosDb
    {
        [Key]
        public int anexo_id { get; set; }
        public int? anexo_historico_contato_fk { get; set; }
        public string anexo_anexo { get; set; }
        public string anexo_nome { get; set; }
    }
}
