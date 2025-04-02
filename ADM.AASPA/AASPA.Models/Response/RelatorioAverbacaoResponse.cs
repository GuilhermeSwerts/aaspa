using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class RelatorioAverbacaoResponse
    {
        public string Token { get; set; }
        public string CodExterno { get; set; }
        public string ClienteCpf { get; set; }
        public string ClienteNome { get; set; }
        public DateTime DataInicioDesconto { get; set; }
        public string ValorDesconto { get; set; }
        public int CodigoResultado { get; set; }
        public string DescricaoErro { get; set; }
        public string Status { get; set; }
        public int QuantidadeParcelas { get; set; }
        public DateTime? DataPagamento { get; set; }
        public int CodigoOperacao { get; set; }
        public int? RemessaId { get; set; }
        public int? CodOperacao { get; set; }
        public int? CodResultado { get; set; }
        public int CodErro { get; set; }
    }
}
