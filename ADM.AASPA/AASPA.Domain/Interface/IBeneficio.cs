using AASPA.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IBeneficio
    {
        object BuscarBeneficioId(int beneficioId);
        object BuscarLogBeneficiosClienteId(int clienteId, DateTime? dtInicio = null, DateTime? dtFim = null);
        object BuscarTodosBeneficios();
        void EditarBeneficio(BeneficioRequest request);
        void NovoBeneficio(BeneficioRequest request);
        void VincularBeneficios(int clienteId, List<int> beneficiosIds);
    }
}
