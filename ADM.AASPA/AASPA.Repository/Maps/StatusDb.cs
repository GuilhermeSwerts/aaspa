using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class StatusDb
    {
        [Key]
        public int status_id { get; set; }
        public string status_nome { get; set; }
    }
}
