using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RegistroRetornoFinanceiroDb
    {
        [Key]
        public int id { get; set; }
        public int retorno_financeiro_id { get; set; }
        public string? numero_beneficio { get; set; }
        public int? competencia_desconto { get; set; }
        public int? especie { get; set; }
        public string uf { get; set; }
        public decimal? desconto { get; set; }
    }
}
