using AASPA.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IMotivoContato
    {
        object BuscarMotivosById(int motivoId);
        object BuscarTodosMotivos();
        void EditarMotivo(MotivoContatoRequest motivoContatoRequest);
        void NovoMotivo(string nomeMotivo);
    }
}
