using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class BeneficioDb
    {
        [Key]
        public int beneficio_id { get; set; }
        public int beneficio_cod_beneficio { get; set; }
        public string beneficio_nome_beneficio { get; set; }
        public string beneficio_fornecedor_beneficio { get; set; }
        public DateTime beneficio_dt_beneficio { get; set; }
        public string beneficio_descricao_beneficios { get; set; }
        public decimal beneficio_valor_a_pagar_ao_fornecedor { get; set; }
    }
}
