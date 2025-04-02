using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Model.RelatorioAverbacao
{
    public class Detalhes
    {
        public string Competencia { get; set; }
        public string Corretora { get; set; }
        public int? Remessa { get; set; }
        public int Averbados { get; set; }
        public int TaxaAverbacao { get; set; }
    }
}
