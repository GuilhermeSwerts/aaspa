using AASPA.Models.Model.RelatorioAverbacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class GerarRelatorioAverbacaoResponse
    {
        public List<RelatorioAverbacaoResponse> Relatorio { get; set; }
        public ResumoAverbacaoResponse Resumo { get; set; }
        public List<MotivoNaoAverbacaoResponse> MotivosNaoAverbada { get; set; }
        public string Base64 { get; set; }
        public Detalhes Detalhes { get; set; }
        public int TaxaNaoAverbado { get; set; }
    }
}
