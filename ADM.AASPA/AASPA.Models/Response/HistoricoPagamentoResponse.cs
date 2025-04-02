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
        public string ValorPago { get; set; }
        public string DtPagamento { get; set; }
        public string Dt_Cadastro { get; set; }
        public string CompetenciaRepasse { get; set; }
        public string CompetenciaPagamento { get; set; }
        public int Parcela { get; set; }
    }
}
