using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarArquivosResponse
    {
        public int RetornoId { get; set; }
        public int RepasseId { get; set; }
        public string NomeRemessaCompetente { get; set; }
        public string NomeRetornoCompetente { get; set; }
        public string NomeRepasseCompetente { get; set; }
        public string DataImportacao { get; set; }
    }
}
