using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RemessaDb
    {
        [Key]
        public int remessa_id { get; set; }
        public string remessa_mes_ano { get; set; }
        public DateTime remessa_data_criacao { get; set; }
    }
}
