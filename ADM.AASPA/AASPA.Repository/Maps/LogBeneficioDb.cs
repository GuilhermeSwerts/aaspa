using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class LogBeneficioDb
    {

        [Key]
        public int log_beneficios_id { get; set; }
        public int log_beneficios_beneficio_id { get; set; }
        public int log_beneficios_acao_id { get; set; }
        public DateTime log_beneficios_dt_cadastro { get; set; } = DateTime.Now;
        public DateTime? log_beneficios_dt_removido { get; set; }
        public int log_beneficios_cliente_id { get; set; }
        public bool log_beneficios_ativo { get; set; } = true;

    }

    public enum AcaoLogBeneficio
    {
        Adicionado = 1,
        Removido = 2
    }

}
