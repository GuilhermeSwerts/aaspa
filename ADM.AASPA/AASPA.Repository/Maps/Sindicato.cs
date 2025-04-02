using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    [Table("sindicatos")]
    public class Sindicato
    {
        [Key]
        [Column("sindicato_id")]
        public int Id { get; set; }

        [Column("sindicato_nb")]
        public string Nb { get; set; }

        [Column("sindicato_competencia")]
        public string Competencia { get; set; }

        [Column("sindicato_cs_sindicato")]
        public string CdSindicato { get; set; }

        [Column("log_arquivo_sindicato_id")]
        public int LogArquivoId { get; set; }
    }
}
