using AASPA.Domain.Interface;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class ReembolsoService : IReembolso
    {
        private readonly MysqlContexto contexto;

        public ReembolsoService(MysqlContexto contexto)
        {
            this.contexto = contexto;
        }
        public List<SolicitacaoReembolsoResponse> Get(DateTime? dtInicio, DateTime? dtFim)
        {
            return (from sol in contexto.solicitacaoreembolso
                    join ele in contexto.elegivelreembolso
                    on sol.elegivelreembolso_fk equals ele.id
                    where
                        (dtInicio == null || sol.dtsolicitacao >= dtInicio.Value) &&
                        (dtFim == null || sol.dtsolicitacao < dtFim.Value.AddDays(1))
                    select new SolicitacaoReembolsoResponse(sol,ele)).ToList();
        }

        public void InformaPagamento(int idSolicitacao)
        {
            var solicitacao = contexto.solicitacaoreembolso.FirstOrDefault(x => x.id == idSolicitacao)
                ?? throw new Exception("Solicitação não encontrada");
            solicitacao.dtpagamento = DateTime.Now;
            contexto.SaveChanges();
        }

        public byte[] DownloadRelatorio(DateTime? dtInicio, DateTime? dtFim)
        {
            var data = Get(dtInicio, dtFim);

            string csv = "IdSolicitacao;Nome;Cpf;Nb;Telefone;ChavePix;Banco;Agencia;Conta;Protocolo;Situacao;DtSolicitacao;DtPagamento\n";

            foreach (var item in data)
            {
                csv += string.Join(";",item.IdSolicitacao, item.Nome, item.Cpf, item.Nb, item.Telefone, item.ChavePix, item.Banco, item.Agencia, item.Conta, item.Protocolo, item.Situacao, item.DtSolicitacao, item.DtPagamento);
                csv += "\n";
            }

            byte[] fileBytes = Encoding.Latin1.GetBytes(csv);
            return fileBytes;
        }
    }
}
