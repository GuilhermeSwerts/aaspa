using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    [Table("log_arquivo_sindicato")]
    public class LogArquivoSindicato
    {
        [Key]
        [Column("log_arquivo_sindicato_id")]
        public int Id { get; set; }
        
        [Column("log_arquivo_sindicato_nome_arquivo")]
        public string NomeArquivo { get; set; }

        [Column("log_arquivo_sindicato_dt_importacao")]
        public DateTime DtImportacao { get; set; }
        
        [Column("log_arquivo_sindicato_importado")]
        public bool Importado { get; set; } = false;
        
        [Column("log_arquivo_sindicato_erro")]
        public string Erro { get; set; } = string.Empty;

        [Column("log_arquivo_sindicato_gerou_erro")]
        public bool GerouErro { get; set; } = false;

        [Column("log_arquivo_sindicato_dt_cadastro")]
        public DateTime DtCadastro { get; set; } = DateTime.Now;
        
    }
}
