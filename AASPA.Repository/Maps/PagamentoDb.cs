using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class PagamentoDb
    {
        [Key]
        public int pagamento_id { get; set; }
        public decimal pagamento_valor_pago { get; set; }
        public DateTime pagamento_dt_pagamento { get; set; }
        public DateTime pagamento_dt_cadastro { get; set; } = DateTime.Now;
        public int pagamento_cliente_id { get; set; }

    }
}
