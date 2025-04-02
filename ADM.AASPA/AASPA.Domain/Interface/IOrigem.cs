using AASPA.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IOrigem
    {
        object BuscarOrigemById(int motivoId);
        object BuscarTodasOrigem();
        void EditarOrigem(OrigemRequest origemRequest);
        void NovaOrigem(string nomeOrigem);
    }
}
