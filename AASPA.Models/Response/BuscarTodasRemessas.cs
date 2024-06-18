using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarTodasRemessas
    {
        public int RemessaId { get; set; }
        public string Mes { get; set; }
        public int Ano { get; set; }
        public string DataCriacao { get; set; }
        public string Periodo { get; set; }
    }
}
