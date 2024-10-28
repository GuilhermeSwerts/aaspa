using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class CancelarPropostaKompletoRequest
    {
        public string token { get; set; }
        public int origem { get; set; } = 1;
        public string cpfSolicitante { get; set; }
        public string motivoCancelamento { get; set; }
    }
}
