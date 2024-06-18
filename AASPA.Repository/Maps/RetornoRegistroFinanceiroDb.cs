using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RetornoRegistroFinanceiroDb
    {
        [Key]
        public int Id { get; set; }
        public int RetornoFinanceiroId { get; set; }
        public int? NumeroBeneficio { get; set; }
        public int? CompetenciaDesconto { get; set; }
        public int? Especie { get; set; }
        public int? UF { get; set; }
        public decimal? Desconto { get; set; }
    }
}
