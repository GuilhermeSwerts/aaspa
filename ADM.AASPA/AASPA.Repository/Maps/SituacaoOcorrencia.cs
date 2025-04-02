using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    [Table("situacao_ocorrencia")]
    public class SituacaoOcorrencia
    {
        [Key]
        [Column("situacao_ocorrencia_id")]
        public int Id { get; set; }
        [Column("situacao_ocorrencia_nome")]
        public string Nome { get; set; }
    }
}
