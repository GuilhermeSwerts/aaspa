using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RemessaDb
    {
        [Key]
        public int remessa_id { get; set; }
        public string remessa_ano_mes { get; set; }
        public string nome_arquivo_remessa { get; set; }
        public DateTime remessa_data_criacao { get; set; }
        public DateTime remessa_periodo_de { get; set; }
        public DateTime remessa_periodo_ate { get; set; }
    }
}
