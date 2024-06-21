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
        void NovoCliente(ClienteRequest novoCliente);
        List<BuscarClienteByIdResponse> BuscarTodosClientes(int? statusCliente, int? statusRemessa);
    }
}
