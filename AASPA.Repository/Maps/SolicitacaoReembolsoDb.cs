using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class SolicitacaoReembolsoDb
    {
        [Key]
        public int id { get; set; }
        public int elegivelreembolso_fk { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public string chave_pix { get; set; }
        public string banco { get; set; }
        public string agencia { get; set; }
        public string conta { get; set; }
        public bool ativo { get; set; }
        public string protocolo { get; set; }
        public DateTime dtsolicitacao { get; set; }
        public DateTime? dtpagamento { get; set; }
    }
}
