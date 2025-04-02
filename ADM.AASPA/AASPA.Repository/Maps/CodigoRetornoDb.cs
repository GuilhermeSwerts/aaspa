using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class CodigoRetornoDb
    {
        [Key]
        public int Id { get; set; }
        public int CodigoOperacao { get; set; }
        public int CodigoResultado { get; set; }
        public string CodigoErro { get; set; }
        public string DescricaoErro { get; set; }
    }
}
