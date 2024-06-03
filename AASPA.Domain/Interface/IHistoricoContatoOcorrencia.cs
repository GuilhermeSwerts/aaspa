using AASPA.Models.Requests;
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
    }
}
