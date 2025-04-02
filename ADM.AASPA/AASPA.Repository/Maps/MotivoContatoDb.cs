using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class MotivoContatoDb
    {
        [Key]
        public int motivo_contato_id { get; set; }
        public string motivo_contato_nome { get; set; }
    }
}
