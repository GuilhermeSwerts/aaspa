using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class AlterarStatusClienteRequest
    {
        public int status_id_antigo { get; set; }
        public int status_id_novo { get; set; }
        public int cliente_id { get; set; }
    }
}
