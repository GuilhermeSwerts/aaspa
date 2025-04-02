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
    public class PagamentoService : IPagamento
    {
        private readonly MysqlContexto _mysql;

        public PagamentoService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public List<HistoricoPagamentoResponse> BuscarHistoricoPagamentoClienteId(int clienteId)
        {
            return _mysql.pagamentos
                .Where(x => x.pagamento_cliente_id == clienteId)
                .Select(x => new HistoricoPagamentoResponse
                {
                    DtPagamento = x.pagamento_dt_pagamento.HasValue ? x.pagamento_dt_pagamento.Value.ToString("dd/MM/yyyy") : null,
                    Dt_Cadastro = x.pagamento_dt_cadastro.ToString("dd/MM/yyyy HH:mm:ss"),
                    PagamentoId = x.pagamento_id,
                    ValorPago = x.pagamento_valor_pago.ToString("C", new System.Globalization.CultureInfo("pt-BR")),
                    Parcela = x.pagamento_parcela,
                    CompetenciaPagamento = x.pagamento_competencia_pagamento,
                    CompetenciaRepasse   = x.pagamento_competencia_repasse
                }).ToList();
        }

        public HistoricoPagamentoResponse BuscarPagamentoById(int pagamentoId)
        {
            var pagamento = _mysql.pagamentos.FirstOrDefault(x => x.pagamento_id == pagamentoId)
                ?? throw new Exception("Pagamento não encontrado.");
            return new HistoricoPagamentoResponse
            {
                DtPagamento = pagamento.pagamento_dt_pagamento.HasValue ? pagamento.pagamento_dt_pagamento.Value.ToString("dd/MM/yyyy") : null,
                PagamentoId = pagamento.pagamento_id,
                ValorPago = pagamento.pagamento_valor_pago.ToString(),
                Parcela = pagamento.pagamento_parcela,
                CompetenciaPagamento = pagamento.pagamento_competencia_pagamento,
                CompetenciaRepasse = pagamento.pagamento_competencia_repasse
            };
        }

        public void EditarPagamento(PagamentoDb pagamentoDb)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var pagamento = _mysql.pagamentos.FirstOrDefault(x => x.pagamento_id == pagamentoDb.pagamento_id);

                pagamento.pagamento_valor_pago = pagamentoDb.pagamento_valor_pago;
                pagamento.pagamento_dt_pagamento = pagamentoDb.pagamento_dt_pagamento;
                pagamento.pagamento_cliente_id = pagamentoDb.pagamento_cliente_id;

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw;
            }
        }

        public void ExcluirPagamento(int pagamentoId)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var pagamento = _mysql.pagamentos.FirstOrDefault(x => x.pagamento_id == pagamentoId);
                _mysql.pagamentos.Remove(pagamento);
                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void NovoPagamento(PagamentoDb pagamento)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                _mysql.pagamentos.Add(pagamento);
                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
