using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Enums
{
    public enum EEndpointsIntegraall
    {
        [Description("api/PropostaStatus/AtualizarStatusParaAverbado")]
        AtualizaStatusAverbado,
        [Description("api/PropostaStatus/AtualizarStatusParaCancelado")]
        AtualizaStatusCancelado,
        [Description("api/PropostaStatus/AtualizarStatusParaAtivoPago")]
        AtualizaStatusPago,
    }
}
