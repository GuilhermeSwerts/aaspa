using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class ElegivelReembolsoDb
    {
        [Key]
        public int id { get; set; }
        public string cpf { get; set; }
        public string nb { get; set; }
    }
}
