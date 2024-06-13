using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarRetornoResponse
    {
        public int IdRetorno { get; set; }
        public DateTime? DataImportacao { get; set; }
        public string AnoMes { get; set; }
        public List<RegistroRetornoRemessaDb> Retornos { get; set; }
    }
}
