using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RegistroRetornoRemessaDb
    {
        public int Id { get; set; }
        public int NumeroBeneficio { get; set; }
        public int CodigoOperacao { get; set; }
        public int CodigoResultado { get; set; }
        public int MotivoRejeicao { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime DataInicioDesconto { get; set; }
        public int CodigoEspecieBeneficio { get; set; }
        public int? RetornoRemessaId { get; set; }
    }
}
