using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class ClienteRequest
    {
        public NovoCliente Cliente { get; set; }
        public NovoCaptador Captador { get; set; }
    }

    public class NovoCliente
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string Uf { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; } = string.Empty;
        public DateTime DataNasc { get; set; }
        public DateTime DataCad { get; set; } = DateTime.Now;
        public string NrDocto { get; set; } = string.Empty;
        public string Empregador { get; set; } = string.Empty;
        public string MatriculaBeneficio { get; set; }
        public string NomeMae { get; set; }
        public string NomePai { get; set; } = string.Empty;
        public string TelefoneFixo { get; set; } = string.Empty;
        public string TelefoneCelular { get; set; }
        public bool PossuiWhatsapp { get; set; }
        public string FuncaoAASPA { get; set; }
        public string Email { get; set; } = string.Empty;
        public int EstadoCivil { get; set; }
        public int? Sexo { get; set; }
        public DateTime? DataAverbacao { get; set; }
        public int StatusIntegral { get; set; }
    }

    public class NovoCaptador 
    {
        public int? CaptadorId { get; set; }
        public string CpfOuCnpj { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }

}
