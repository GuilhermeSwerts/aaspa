using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class UsuarioDb
    {
        [Key]
        public int usuario_id { get; set; }
        public int usuario_tipo { get; set; }
        public string usuario_nome { get; set; }
        public string usuario_senha { get; set; }
        public string usuario_username { get; set; }
        public bool usuario_status { get; set; } = true;
        public DateTime usuario_dt_cadastro { get; set; }
    }
}
