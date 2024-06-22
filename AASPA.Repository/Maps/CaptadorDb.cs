using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class CaptadorDb
    {

        [Key]
        public int captador_id { get; set; }
        public string captador_cpf_cnpj { get; set; }
        public string captador_nome { get; set; }
        public string? captador_descricao { get; set; }
        public bool captador_e_cnpj { get; set; }
        public bool captador_situacao { get; set; }
        
    }
}
