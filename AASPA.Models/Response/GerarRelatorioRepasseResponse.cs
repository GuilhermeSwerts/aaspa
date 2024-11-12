using AASPA.Models.Model.RelatorioRepasse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class GerarRelatorioRepasseResponse
    {
        public List<RelatorioRepasse> Relatorio { get; set; }
        public CabecalhoRepasse Cabecalho { get; set; }
    }
}
