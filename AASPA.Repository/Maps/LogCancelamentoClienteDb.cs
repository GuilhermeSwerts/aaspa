using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class LogCancelamentoClienteDb
    {
        [Key]
        public int log_id { get; set; }
        public int id_usuario { get; set; }
        public string nome_usuario { get; set; }
        public DateTime data_cancelamento { get; set; }
        public string motivo_cancelamento { get; set; }
        public int tipo_cancelamento { get; set; }
        public int id_cliente { get; set; }
        public string? destino_cancelamento { get; set; }
        public int? status_cancelamento { get; set; }
        public string? erro_cancelamento { get; set; }
    }
}
