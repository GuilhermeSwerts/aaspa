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
        (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas,int TotalClientes) BuscarTodosClientes(int? statusCliente, int? statusRemessa, DateTime? dateInit, DateTime? dateEnd, int? paginaAtual);
        byte[] DownloadFiltro((List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) clientes);
        Task<List<ClienteRequest>> GetClientesIntegraall(string datacadastro, string dataCadastroFim);
        void SalvarNovoCliente(List<ClienteRequest> clientes);
    }
}
