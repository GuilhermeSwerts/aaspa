using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class AlterarStatusClientesIntegraallRequest
    {
        public int clienteid { get; set; }
        public int cancelamento { get; set; }
        public string motivocancelamento { get; set; }
        public int status_id_antigo { get; set; }
        public int status_id_novo { get; set; }
    }
}
