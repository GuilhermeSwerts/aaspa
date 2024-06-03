using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class OrigemDb
    {
        [Key]
        public int origem_id { get; set; }
        public string origem_nome { get; set; }
    }
}
