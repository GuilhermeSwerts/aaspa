using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarRemessasResponse
    {
        public int RemessaId { get; set; }
        public int Mês { get; set; }
        public int Ano { get; set; }
        public string DataCriacao { get; set; }
    }
}
