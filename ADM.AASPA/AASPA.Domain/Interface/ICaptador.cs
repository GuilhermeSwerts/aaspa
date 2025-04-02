using AASPA.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface ICaptador
    {
        object BuscarCaptadores();
        void EditarCaptador(NovoCaptador captadorRequest);
        void NovoCaptador(NovoCaptador novoCaptador);
        void DeletarCaptador(int captadorId);
    }
}
