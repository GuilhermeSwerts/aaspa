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
    }
}
