using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class HistoricoPagamentoResponse
    {
        public decimal PagamentoId { get; set; }
        public decimal ValorPago { get; set; }
        public string DtPagamento { get; set; }
        public string Dt_Cadastro { get; set; }
    }
}
