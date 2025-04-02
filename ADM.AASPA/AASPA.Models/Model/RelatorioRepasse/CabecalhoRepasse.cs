using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Model.RelatorioRepasse
{
    public class CabecalhoRepasse
    {
        public string Competencia { get; set; }
        public string MesRepasse { get; set; }
        public int TotalDescontos { get; set; }
        public string ValorTotalRepasse { get; set; }
    }
}
