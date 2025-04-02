using AASPA.Models.Response;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IPagamento
    {
        List<HistoricoPagamentoResponse> BuscarHistoricoPagamentoClienteId(int clienteId);
        HistoricoPagamentoResponse BuscarPagamentoById(int pagamentoId);
        void EditarPagamento(PagamentoDb pagamentoDb);
        void ExcluirPagamento(int pagamentoId);
        void NovoPagamento(PagamentoDb pagamento);
    }
}
