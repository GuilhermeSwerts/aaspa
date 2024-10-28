using AASPA.Models.Requests;
using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IIntegracaoKompleto
    {
        Task<CancelarPropostaKompletoResponse> CancelarPropostaAsync(AlterarStatusClientesIntegraallRequest request);
    }
}
