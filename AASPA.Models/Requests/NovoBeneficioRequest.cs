using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class BeneficioRequest
    {
        public int BeneficioId { get; set; }
        public string CodBeneficio { get; set; }
        public string NomeBeneficio { get; set; }
        public string FornecedorBeneficio { get; set; }
        public string ValorAPagarAoFornecedor { get; set; }
        public string DescricaoBeneficios { get; set; }
    }
}
