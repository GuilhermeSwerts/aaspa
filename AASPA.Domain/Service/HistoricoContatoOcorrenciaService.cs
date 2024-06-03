using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class HistoricoContatoOcorrenciaService : IHistoricoContatoOcorrencia
    {
        private readonly MysqlContexto _mysql;

        public HistoricoContatoOcorrenciaService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object BuscarTodosContatoOcorrencia(int clienteId)
        {
            return (from hit in _mysql.historico_contatos_ocorrencia
                    join ori in _mysql.origem on hit.historico_contatos_ocorrencia_origem_id equals ori.origem_id
                    join mot in _mysql.motivo_contato on hit.historico_contatos_ocorrencia_motivo_contato_id equals mot.motivo_contato_id
                    where hit.historico_contatos_ocorrencia_cliente_id == clienteId
                    select new HistoricoContatoOcorrenciaResponse
                    {
                        DataHoraOcorrencia = hit.historico_contatos_ocorrencia_dt_ocorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                        MotivoDoContato = mot.motivo_contato_nome,
                        Origem = ori.origem_nome,
                        SituacaoOcorrencia = hit.historico_contatos_ocorrencia_situacao_ocorrencia,
                        DescricaoDaOcorrência = hit.historico_contatos_ocorrencia_descricao,
                        Id = hit.historico_contatos_ocorrencia_id
                    }).ToList().OrderByDescending(x=> x.DataHoraOcorrencia);
        }

        public object BuscarContatoOcorrenciaById(int historicoContatosOcorrenciaId)
        {
            return _mysql.historico_contatos_ocorrencia
                .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatosOcorrenciaId)
                ?? throw new Exception($"Contato Historico do id: {historicoContatosOcorrenciaId} não encontrado");
        }

        public void DeletarContatoOcorrencia(int historicoContatosOcorrenciaId)
        {
            var contatoOcorrencia = _mysql.historico_contatos_ocorrencia
                .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatosOcorrenciaId)
                ?? throw new Exception($"Contato Historico do id: {historicoContatosOcorrenciaId} não encontrado");

            _mysql.historico_contatos_ocorrencia.Remove(contatoOcorrencia);
            _mysql.SaveChanges();
        }

        public void EditarContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var contatoOcorrencia = _mysql.historico_contatos_ocorrencia
                    .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatos.HistoricoContatosOcorrenciaId)
                    ?? throw new Exception($"Contato Historico do id: {historicoContatos.HistoricoContatosOcorrenciaId} não encontrado");

                contatoOcorrencia.historico_contatos_ocorrencia_descricao = historicoContatos.HistoricoContatosOcorrenciaDescricao;
                contatoOcorrencia.historico_contatos_ocorrencia_dt_cadastro = DateTime.Now;
                contatoOcorrencia.historico_contatos_ocorrencia_dt_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaDtOcorrencia;
                contatoOcorrencia.historico_contatos_ocorrencia_motivo_contato_id = historicoContatos.HistoricoContatosOcorrenciaMotivoContatoId;
                contatoOcorrencia.historico_contatos_ocorrencia_origem_id = historicoContatos.HistoricoContatosOcorrenciaOrigemId;
                contatoOcorrencia.historico_contatos_ocorrencia_situacao_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaSituacaoOcorrencia.ToUpper();

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void NovoContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                _mysql.historico_contatos_ocorrencia.Add(new Repository.Maps.HistoricoContatosOcorrenciaDb
                {
                    historico_contatos_ocorrencia_cliente_id = historicoContatos.HistoricoContatosOcorrenciaClienteId,
                    historico_contatos_ocorrencia_descricao = historicoContatos.HistoricoContatosOcorrenciaDescricao,
                    historico_contatos_ocorrencia_dt_cadastro = DateTime.Now,
                    historico_contatos_ocorrencia_dt_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaDtOcorrencia,
                    historico_contatos_ocorrencia_motivo_contato_id = historicoContatos.HistoricoContatosOcorrenciaMotivoContatoId,
                    historico_contatos_ocorrencia_origem_id = historicoContatos.HistoricoContatosOcorrenciaOrigemId,
                    historico_contatos_ocorrencia_situacao_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaSituacaoOcorrencia.ToUpper()
                });

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
