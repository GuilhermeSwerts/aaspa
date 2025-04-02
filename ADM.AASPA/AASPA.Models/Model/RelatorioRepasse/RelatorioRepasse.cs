using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Model.RelatorioRepasse
{
    public class RelatorioRepasse
    {
        public string CodExterno { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime? DataAdesao { get; set; }
        public decimal Valor { get; set; }
        public string TaxaAssociativa { get; set; }
        public int Parcela { get; set; }
    }
}
