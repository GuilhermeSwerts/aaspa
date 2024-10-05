using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class LogAlteracaoDb
    {
        [Key]   
        public int log_id { get; set; }
        public string log_titulo { get; set; }
        public string log_log { get; set; }
        public int log_usuario_fk { get; set; }
        public ETipoLog log_tipo { get; set; }
        public DateTime log_dt_cadastro { get; set; }
        public int log_id_tabela_fk { get; set; }
    }

    public enum ETipoLog
    {
        [Description("Tabela historico_contatos_ocorrencia")]
        Atendimento = 1    
    }
}
