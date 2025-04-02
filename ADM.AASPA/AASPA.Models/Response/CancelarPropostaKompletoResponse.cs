using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class CancelarPropostaKompletoResponse
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public List<string> Errors { get; set; }
    }
}
