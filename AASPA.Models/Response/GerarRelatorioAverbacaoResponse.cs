using AASPA.Models.Model.RelatorioAverbacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class GerarRelatoriResponse
    {
        //public List<RelatorioAverbacaoResponse> Relatorio { get; set; }
        //public ResumoAverbacaoResponse Resumo { get; set; }
        //public List<MotivoNaoAverbacaoResponse> MotivosNaoAverbada { get; set; }
        //public string Base64 { get; set; }
        //public Detalhes Detalhes { get; set; }
        //public int TaxaNaoAverbado { get; set; }
        public List<RelatorioAverbacaoResponse> Relatorio { get; set; }
        public int QtdRegistro { get; set; }
        public int QtdAverbados { get; set; }
        public int QtdExcluidos { get; set; }
        public int QtdAutomatico { get; set; }
        public double TaxaAverbacao { get; set; }
        public double TaxaNaoAverbacao { get; set; }
        public double TaxaErroAuto { get; set; }
        public List<MotivoNaoAverbacaoResponse> MotivoNaoAverbada { get; set; }
        public List<MotivoNaoAverbacaoResponse> MotivoAutomatico { get; set; }
        public int QtdNaoAverbados { get; set; }
        public List<MotivoNaoAverbacaoResponse> MotivoNaoExclusao { get; set; }
        public double TaxaNaoExcluido { get; set; }
        public int QtdNaoExcluidos { get; set; }
    }
}
