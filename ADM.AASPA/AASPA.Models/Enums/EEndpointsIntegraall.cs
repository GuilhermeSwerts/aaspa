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
        [Description("api/PropostasStatus/AtualizarStatusParaAverbado")]
        AtualizaStatusAverbado,
        [Description("api/PropostasStatus/AtualizarStatusParaCancelado")]
        AtualizaStatusCancelado,
        [Description("api/PropostasStatus/AtualizarStatusParaAtivoPago")]
        AtualizaStatusPago,
    }
}
