using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class VinculoClienteCaptadorDb
    {
        [Key]
        public int vinculo_cliente_captador_id { get; set; }
        public int vinculo_cliente_id { get; set; }
        public int vinculo_captador_id { get; set; }
    }
}
