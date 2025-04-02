using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class NovoPagamentoRequest
    {
        public int PagamentoId { get; set; } = 0;
        public int ClienteId { get; set; }
        public string ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
