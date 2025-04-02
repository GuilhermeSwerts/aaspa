using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Response
{
    public class ClientesAtivosExcluidosResponse
    {
        public int ClienteId { get; set; }
        public DateTime LogStatusDtCadastro { get; set; }
        public string ClienteCpf { get; set; }
        public int LogStatusNovoId { get; set; }
        public bool ClienteSituacao { get; set; }
        public string ClienteMatriculaBeneficio { get; set; }
    }
}
