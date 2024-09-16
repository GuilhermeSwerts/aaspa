using AASPA.Models.Requests;
using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IHistoricoContatoOcorrencia
    {
        object BuscarContatoOcorrenciaById(int contatoOcorrenciaId);
        object BuscarTodosContatoOcorrencia(int clienteId);
        void DeletarContatoOcorrencia(int contatoOcorrenciaId);
        void EditarContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos);
        void NovoContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos);
        (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) BuscarTodosClientes(ConsultaParametros request);
        byte[] DownloadContatoFiltro((List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) clientes);
    }
}
