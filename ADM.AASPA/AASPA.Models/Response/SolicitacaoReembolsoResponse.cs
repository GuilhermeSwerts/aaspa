using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class SolicitacaoReembolsoResponse
    {
        public int IdSolicitacao { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Nb { get; set; }
        public string Telefone { get; set; }
        public string ChavePix { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Digito { get; set; }
        public string Protocolo { get; set; }
        public string Situacao { get; set; }
        public string DtSolicitacao { get; set; }
        public string DtPagamento { get; set; }

        public SolicitacaoReembolsoResponse(SolicitacaoReembolsoDb sol, ElegivelReembolsoDb ele)
        {
            IdSolicitacao = sol.id;
            Nome = sol.nome;
            Cpf = ele.cpf;
            Nb = ele.nb;
            Telefone = sol.telefone;
            ChavePix = sol.chave_pix;
            Banco = sol.banco;
            Agencia = sol.agencia;
            Conta = sol.conta;
            Digito = sol.digito;
            Protocolo = sol.protocolo;
            Situacao = sol.dtpagamento.HasValue ? "PAGAMENTO EFETUADO" : "AGUARDANDO PAGAMENTO";
            DtSolicitacao = sol.dtsolicitacao.ToString("dd/MM/yyyy HH:mm:ss");
            DtPagamento = sol.dtpagamento.HasValue ? sol.dtpagamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
        }

    }
}
