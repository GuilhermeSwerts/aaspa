using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class NaoAssociadosNovoAtendimentoRequest
    {
        public string? Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public int Origem { get; set; }
        public DateTime DataHora { get; set; }
        public int Motivo { get; set; }
        public int Situacao { get; set; }
        public string Telefone { get; set; }
        public string Descricao { get; set; }
    }
}
