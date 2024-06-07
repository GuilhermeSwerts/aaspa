using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RegistroRemessaDb
    {
        [Key]
        public int registro_Id { get; set; } 
        public string registro_numero_beneficio { get; set; }
        public int registro_codigo_operacao { get; set; }
        public int registro_decimo_terceiro { get; set; } = 0;
        public int registro_valor_percentual_desconto { get; set; } = 0;
        public int remessa_id { get; set; }
    }
}
