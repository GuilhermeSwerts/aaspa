using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class LogStatusDb
    {
        [Key]
        public int log_status_id { get; set; }
        public int log_status_antigo_id { get; set; }
        public int log_status_novo_id { get; set; }
        public int log_status_cliente_id { get; set; }
        public DateTime log_status_dt_cadastro { get; set; }
    }
}
