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
        public int RetornoId { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime? DataImportacao { get; set; }
    }
}
