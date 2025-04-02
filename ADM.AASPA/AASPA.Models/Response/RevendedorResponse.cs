using AASPA.Repository.Maps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class VinculoCaptadoRevendedor
    {
        public CaptadorDb captador { get; set; }
        public int RevendedorId { get; set; }
    }
    public class RevendedorResponse
    {
        [JsonProperty("qtdMaxima")]
        public int QtdMaxima { get; set; }

        [JsonProperty("paginaAtual")]
        public int PaginaAtual { get; set; }

        [JsonProperty("qtdPaginas")]
        public int QtdPaginas { get; set; }

        [JsonProperty("qtdResultado")]
        public int QtdResultado { get; set; }

        [JsonProperty("qtdTotal")]
        public int QtdTotal { get; set; }

        [JsonProperty("resultado")]
        public List<DadosRevendedorResponse> Resultado { get; set; }
    }

    public class DadosRevendedorResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("tipoPessoa")]
        public string TipoPessoa { get; set; }

        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonProperty("inscricaoEstadual")]
        public string InscricaoEstadual { get; set; }

        [JsonProperty("inscricaoMunicipal")]
        public string InscricaoMunicipal { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("sedeId")]
        public int? SedeId { get; set; }

        [JsonProperty("indicadorId")]
        public int? IndicadorId { get; set; }

        [JsonProperty("situacaoRevendedorId")]
        public int SituacaoRevendedorId { get; set; }

        [JsonProperty("sexo")]
        public string Sexo { get; set; }

        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        [JsonProperty("responsavel")]
        public string Responsavel { get; set; }

        [JsonProperty("sede")]
        public string Sede { get; set; }

        [JsonProperty("indicador")]
        public string Indicador { get; set; }

        [JsonProperty("situacaoRevendedor")]
        public SituacaoRevendedor SituacaoRevendedor { get; set; }

        [JsonProperty("conta")]
        public string Conta { get; set; }

        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        [JsonProperty("contato")]
        public Contato Contato { get; set; }

        [JsonProperty("produtos")]
        public string Produtos { get; set; }

        [JsonProperty("revenda")]
        public int Revenda { get; set; }

        [JsonProperty("revendedor")]
        public string Revendedor { get; set; }

        [JsonProperty("estrategias")]
        public string Estrategias { get; set; }

        [JsonProperty("situacao")]
        public int Situacao { get; set; }

        [JsonProperty("vendedorUsuarioId")]
        public int VendedorUsuarioId { get; set; }

        [JsonProperty("tipoCadastro")]
        public string TipoCadastro { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("usuarioAutomatico")]
        public string UsuarioAutomatico { get; set; }
    }

    public class SituacaoRevendedor
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }

    public class Contato
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("telefone")]
        public string Telefone { get; set; }
    }
}
