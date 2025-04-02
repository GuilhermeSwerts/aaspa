using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class LogBeneficioResponse
    {
        public string Nome { get; set; }
        public string Acao { get; set; }
        public string DataVinculo { get; set; }
        public string DataRemocaoVinculo { get; set; }
        public string VinculoAtivo { get; set; }
        public int Id { get; set; }
    }
}
