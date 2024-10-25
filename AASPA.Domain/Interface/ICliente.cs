using AASPA.Models.Requests;
using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface ICliente
    {
        BuscarClienteByIdResponse BuscarClienteID(int clienteId);
        void AtualizaCliente(ClienteRequest novoCliente);
        void NovoCliente(ClienteRequest novoCliente, bool isLis, bool cadastroExterno = false);
        (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) BuscarTodosClientes(ConsultaParametros request);
        byte[] DownloadFiltro(ConsultaParametros request);
        Task<List<ClienteRequest>> GetClientesIntegraall(string datacadastro, string dataCadastroFim);
        void SalvarNovoCliente(List<ClienteRequest> clientes);
        Task<string> GerarToken();
        Task CancelarClienteIntegraall(AlterarStatusClientesIntegraallRequest request, string tokenIntegraall);
        Task CancelarCliente(AlterarStatusClientesIntegraallRequest request);

    }
}
