using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IReembolso
    {
        List<SolicitacaoReembolsoResponse> Get(DateTime? dtInicio, DateTime? dtFim);
        byte[] DownloadRelatorio(DateTime? dtInicio, DateTime? dtFim);
        void InformaPagamento(int idSolicitacao);
    }
}
