using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RetornoRemessaDb
    {
        [Key]
        public int Retorno_Id { get; set; }
        public string AnoMes { get; set; }
        public string Nome_Arquivo_Retorno { get; set; }
        public int? Remessa_Id { get; set; }
        public DateTime? Data_Importacao { get; set; }
    }
}
