using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class MotivoNaoAverbacaoResponse
    {
        public double TotalPorcentagem { get; set; }
        public string CodigoErro { get; set; }
        public int TotalPorCodigoErro { get; set; }
        public string DescricaoErro { get; set; }
    }
}
