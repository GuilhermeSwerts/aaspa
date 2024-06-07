using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarTodasRemessas
    {
        public int remessa_id { get; set; }
        public string mes { get; set; }
        public int ano { get; set; }
        public DateTime Data_Criacao { get; set; }
    }
}
