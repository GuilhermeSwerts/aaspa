using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class ClienteIntegraallResponse
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string Cpf { get; set; }
        public string EstadoCivil { get; set; }
        public string Sexo { get; set; }
        public string DocIdentidade { get; set; }
        public string? DocIdentidadeUf { get; set; }
        public string? DocIdentidadeOrgEmissor { get; set; }
        public DateTime? DocIdentidadeDataEmissao { get; set; }
        public string NomeMae { get; set; }
        public string NomePai { get; set; }
        public string EmailPessoal { get; set; }
        public string TelefonePessoal { get; set; }
        public string EmailCorporativo { get; set; }
        public string TelefoneCorporativo { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Complemento { get; set; }
        public string EndNumero { get; set; }
        public int ProdutoId { get; set; }
        public int RevendedorId { get; set; }
        public int StatusId { get; set; }
        public int VendedorUsuarioId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int CodEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Matricula { get; set; }
        public string Identificadores { get; set; }
        public string LinkKompletoCliente { get; set; }
        public string LinkKompletoVendedor { get; set; }
        public string NomeRevendedor { get; set; }
        public string NomeProduto { get; set; }
        public string NomeVendedor { get; set; }
        public string NomeStatus { get; set; }
        public decimal ValorProduto { get; set; }
        public string TipoPagamentoNome { get; set; }
        public int TipoPagamentoId { get; set; }
        public string PropostaAnexos { get; set; }
        public string PropostaAnexosHistorico { get; set; }
        public string Token { get; set; }
        public string LinkKompleto { get; set; }
        public string PropostaHistoricos { get; set; }
        public DateTime? dataSolicitacaoAtivacao { get; set; }
    }
}
