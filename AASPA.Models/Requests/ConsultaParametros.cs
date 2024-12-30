using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class ConsultaParametros
    {
        public bool BuscarStatus { get; set; } = true;
        public int? StatusCliente { get; set; }
        public int? StatusRemessa { get; set; }
        public DateTime? DateInit { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? PaginaAtual { get; set; }
        public int? QtdPorPagina { get; set; } = null;
        public int? StatusIntegraall { get; set; } = 0;
        public int CadastroExterno { get; set; } = 0;
        public string Nome { get; set; } = "";
        public string Captador { get; set; } = "";
        public string Cpf { get; set; } = "";
        public DateTime? DateInitAverbacao { get; set; } = null;
        public DateTime? DateEndAverbacao { get; set; } = null;
        public string Beneficio { get; set; } = null;
        public string SituacaoOcorrencia { get; set; }
        public List<string> SituacoesOcorrencias { get; set; }
        public DateTime? DataInitAtendimento { get; set; }
        public DateTime? DataEndAtendimento { get; set; }
        public List<int>? ListaStatus { get; set; }  
    }
}
