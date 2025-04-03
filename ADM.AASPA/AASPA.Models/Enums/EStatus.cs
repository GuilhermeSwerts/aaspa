using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Enums
{
    public enum EStatus
    {
        AtivoAguardandoAverbacao= 1,
        Inativo= 2,
        Deletado= 3,
        Ativo= 4,
        ExcluidoAguardandoEnvio= 5,
        CanceladoAPedidoDoCliente= 6,
        CanceladoNaoAverbado= 7,
        Cancelado= 8
    }
}
