using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarClienteByIdResponse
    {
        public ClienteDb Cliente { get; set; }
        public CaptadorDb Captador { get; set; }
        public List<BeneficioDb> Beneficios { get; set; }
        public StatusDb StatusAtual { get; set; }
        public HistoricoContatosOcorrenciaDb Historico { get; set; }
        public OrigemDb Origem { get; set; }
        public MotivoContatoDb Motivo { get; set; }
        public int QtdPaginas { get; set; }
    }
}
