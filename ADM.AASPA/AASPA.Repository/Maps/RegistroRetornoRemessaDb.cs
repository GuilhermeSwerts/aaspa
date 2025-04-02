using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class RegistroRetornoRemessaDb
    {
        [Key]
        public int Id { get; set; }
        public string Numero_Beneficio { get; set; }
        public int Codigo_Operacao { get; set; }
        public int Codigo_Resultado { get; set; }
        public int Motivo_Rejeicao { get; set; }
        public decimal Valor_Desconto { get; set; }
        public DateTime Data_Inicio_Desconto { get; set; }
        public int Codigo_Especie_Beneficio { get; set; }
        public int? Retorno_Remessa_Id { get; set; }
    }
}
