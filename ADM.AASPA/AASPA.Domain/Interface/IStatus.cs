using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IStatus
    {
        List<BuscarLogStatusClienteIdResponse> BuscarLogStatusClienteId(int clienteId);
        void AlterarStatusCliente(AlterarStatusClienteRequest request);
        object BuscarStatusById(int statusId);
        List<StatusDb> BuscarTodosStatus();
        void EditarStatus(StatusDb status);
        void InserirNovoStatus(string status_nome);
    }
}
