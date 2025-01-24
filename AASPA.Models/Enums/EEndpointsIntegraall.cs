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
        [Description("PropostaStatus/AtualizarStatusParaAverbado")]
        AtualizaStatusAverbado,
        [Description("PropostaStatus/AtualizarStatusParaCancelado")]
        AtualizaStatusCancelado,
        [Description("PropostaStatus/AtualizarStatusParaAtivoPago")]
        AtualizaStatusPago,
    }
}
