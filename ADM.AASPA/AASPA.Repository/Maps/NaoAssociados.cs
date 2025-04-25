using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    [Table("nao_associados")]
    public class NaoAssociados
    {
        [Key]
        public int nao_associado_id { get; set; }
        public string nome_nao_associados { get; set; }
        public string cpf_nao_associados { get; set; }
        public int origem_id { get; set; }
        public int motivo_contato_id { get; set; }
        public int situacao_ocorrencia_id { get; set; }
        public string telefone { get; set; }
        public DateTime nao_associado_dt_cadastro { get; set; }
        public DateTime data_ocorrencia { get; set; }
        public string descricao_nao_associado { get; set; }

    }
}
