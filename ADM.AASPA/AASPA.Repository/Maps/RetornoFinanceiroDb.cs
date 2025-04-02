using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RetornoFinanceiroDb
    {
        [Key]
        public int retorno_financeiro_id { get; set; }
        public string repasse { get; set; }
        public int competencia_Repasse { get; set; }
        public int remessa_id { get; set; }
        public int retorno_id { get; set; }
        public string ano_mes { get; set; }
        public DateTime data_importacao { get; set; }
        public string nome_arquivo { get; set; }
    }
}
