using AASPA.Domain.Interface;
using AASPA.Domain.Util;
using AASPA.Models.Enums;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class HistoricoContatoOcorrenciaService : IHistoricoContatoOcorrencia
    {
        private readonly MysqlContexto _mysql;
        private readonly ILog _log;

        public HistoricoContatoOcorrenciaService(MysqlContexto mysql, ILog log)
        {
            _mysql = mysql;
            _log = log;
        }

        public object BuscarTodosContatoOcorrencia(int clienteId)
        {
            var resultado = (from hit in _mysql.historico_contatos_ocorrencia
                             join ori in _mysql.origem on hit.historico_contatos_ocorrencia_origem_id equals ori.origem_id
                             join mot in _mysql.motivo_contato on hit.historico_contatos_ocorrencia_motivo_contato_id equals mot.motivo_contato_id
                             join usu in _mysql.usuarios on hit.historico_contatos_ocorrencia_usuario_fk equals usu.usuario_id into usuario
                             from usu in usuario.DefaultIfEmpty()
                             where hit.historico_contatos_ocorrencia_cliente_id == clienteId && hit.historico_contatos_ocorrencia_ativo
                             select new HistoricoContatoOcorrenciaResponse
                             {
                                 DataHoraOcorrencia = hit.historico_contatos_ocorrencia_dt_ocorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                                 MotivoDoContato = mot.motivo_contato_nome,
                                 Origem = ori.origem_nome,
                                 SituacaoOcorrencia = hit.historico_contatos_ocorrencia_situacao_ocorrencia,
                                 DescricaoDaOcorrência = hit.historico_contatos_ocorrencia_descricao,
                                 Id = hit.historico_contatos_ocorrencia_id,
                                 Agencia = hit.historico_contatos_ocorrencia_agencia == null ? "-" : hit.historico_contatos_ocorrencia_agencia,
                                 Banco = hit.historico_contatos_ocorrencia_banco == null ? "-" : hit.historico_contatos_ocorrencia_banco,
                                 Conta = hit.historico_contatos_ocorrencia_conta == null ? "-" : hit.historico_contatos_ocorrencia_conta,
                                 Digito = hit.historico_contatos_ocorrencia_digito == null ? "-" : hit.historico_contatos_ocorrencia_digito,
                                 Pix = hit.historico_contatos_ocorrencia_chave_pix == null ? "-" : hit.historico_contatos_ocorrencia_chave_pix,
                                 Usuario = usu != null ? usu.usuario_nome : "",
                             }).ToList().OrderByDescending(x => x.DataHoraOcorrencia);

            foreach (var item in resultado)
            {
                item.UltimoUsuario = BuscarNomeUltimoUsuario(item.Id);
            }

            return resultado;
        }

        private string BuscarNomeUltimoUsuario(int hstId)
        {
            var usu = _mysql.log_alteracao.Where(x => x.log_id_tabela_fk == hstId).ToList();

            if (usu.Count == 0) return "";

            var ultimo = usu.OrderByDescending(x => x.log_id).FirstOrDefault();

            if (ultimo == null) return "";

            return _mysql.usuarios.FirstOrDefault(x => x.usuario_id == ultimo.log_usuario_fk).usuario_nome;
        }

        public object BuscarContatoOcorrenciaById(int historicoContatosOcorrenciaId)
        {
            var hst = _mysql.historico_contatos_ocorrencia
                .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatosOcorrenciaId && x.historico_contatos_ocorrencia_ativo)
                ?? throw new Exception($"Contato Historico do id: {historicoContatosOcorrenciaId} não encontrado");

            var anexos = _mysql.anexos.Where(x => x.anexo_historico_contato_fk == hst.historico_contatos_ocorrencia_id).ToList();

            return new
            {
                hst.historico_contatos_ocorrencia_id,
                hst.historico_contatos_ocorrencia_origem_id,
                hst.historico_contatos_ocorrencia_cliente_id,
                hst.historico_contatos_ocorrencia_motivo_contato_id,
                hst.historico_contatos_ocorrencia_dt_ocorrencia,
                hst.historico_contatos_ocorrencia_descricao,
                hst.historico_contatos_ocorrencia_situacao_ocorrencia,
                hst.historico_contatos_ocorrencia_dt_cadastro,
                hst.historico_contatos_ocorrencia_banco,
                hst.historico_contatos_ocorrencia_agencia,
                hst.historico_contatos_ocorrencia_conta,
                hst.historico_contatos_ocorrencia_digito,
                hst.historico_contatos_ocorrencia_chave_pix,
                hst.historico_contatos_ocorrencia_tipo_chave_pix,
                hst.historico_contatos_ocorrencia_telefone,
                hst.historico_contatos_ocorrencia_tipo_conta,
                hst.historico_contatos_ocorrencia_valor_reembolso,
                hst.historico_contatos_ocorrencia_valor_parcela,
                hst.historico_contatos_ocorrencia_valor_parcela_2,
                anexos
            };
        }

        public void DeletarContatoOcorrencia(int historicoContatosOcorrenciaId, int usuarioId)
        {
            var contatoOcorrencia = _mysql.historico_contatos_ocorrencia
                .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatosOcorrenciaId)
                ?? throw new Exception($"Contato Historico do id: {historicoContatosOcorrenciaId} não encontrado");

            _log.NovaAlteracao("Exclusão Do Atendimento", "", usuarioId, ETipoLog.Atendimento, contatoOcorrencia.historico_contatos_ocorrencia_id);
            contatoOcorrencia.historico_contatos_ocorrencia_ativo = false;

            _mysql.SaveChanges();
        }

        public void EditarContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos, int usuarioLogadoId)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var contatoOcorrencia = _mysql.historico_contatos_ocorrencia
                    .FirstOrDefault(x => x.historico_contatos_ocorrencia_id == historicoContatos.HistoricoContatosOcorrenciaId)
                    ?? throw new Exception($"Contato Historico do id: {historicoContatos.HistoricoContatosOcorrenciaId} não encontrado");

                var old = JsonConvert.DeserializeObject<HistoricoContatosOcorrenciaDb>(JsonConvert.SerializeObject(contatoOcorrencia));

                contatoOcorrencia.historico_contatos_ocorrencia_descricao = historicoContatos.HistoricoContatosOcorrenciaDescricao;
                contatoOcorrencia.historico_contatos_ocorrencia_dt_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaDtOcorrencia;
                contatoOcorrencia.historico_contatos_ocorrencia_motivo_contato_id = historicoContatos.HistoricoContatosOcorrenciaMotivoContatoId;
                contatoOcorrencia.historico_contatos_ocorrencia_origem_id = historicoContatos.HistoricoContatosOcorrenciaOrigemId;
                contatoOcorrencia.historico_contatos_ocorrencia_situacao_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaSituacaoOcorrencia.ToUpper();
                contatoOcorrencia.historico_contatos_ocorrencia_banco = historicoContatos.HistoricoContatosOcorrenciaBanco != null ? historicoContatos.HistoricoContatosOcorrenciaBanco.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_agencia = historicoContatos.HistoricoContatosOcorrenciaAgencia != null ? historicoContatos.HistoricoContatosOcorrenciaAgencia.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_conta = historicoContatos.HistoricoContatosOcorrenciaConta != null ? historicoContatos.HistoricoContatosOcorrenciaConta.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_digito = historicoContatos.HistoricoContatosOcorrenciaDigito != null ? historicoContatos.HistoricoContatosOcorrenciaDigito.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_chave_pix = historicoContatos.HistoricoContatosOcorrenciaPix != null ? historicoContatos.HistoricoContatosOcorrenciaPix.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_tipo_chave_pix = historicoContatos.HistoricoContatosOcorrenciaTipoChavePix != null ? historicoContatos.HistoricoContatosOcorrenciaTipoChavePix.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_telefone = historicoContatos.HistoricoContatosOcorrenciaTelefone != null ? historicoContatos.HistoricoContatosOcorrenciaTelefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_tipo_conta = historicoContatos.HistoricoContatosOcorrenciaTipoConta != null ? historicoContatos.HistoricoContatosOcorrenciaTipoConta.Replace("null", "") : "";
                contatoOcorrencia.historico_contatos_ocorrencia_valor_reembolso = historicoContatos?.HistoricoContatosOcorrenciaValorReembolso != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorReembolso, CultureInfo.InvariantCulture) : 0m;
                contatoOcorrencia.historico_contatos_ocorrencia_valor_parcela = historicoContatos?.HistoricoContatosOcorrenciaValorParcela != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorParcela, CultureInfo.InvariantCulture) : 0m;
                contatoOcorrencia.historico_contatos_ocorrencia_valor_parcela_2 = historicoContatos?.HistoricoContatosOcorrenciaValorParcela2 != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorParcela2, CultureInfo.InvariantCulture) : 0m;

                var anexos = _mysql.anexos.Where(x => x.anexo_historico_contato_fk == contatoOcorrencia.historico_contatos_ocorrencia_id).ToList();
                _mysql.anexos.RemoveRange(anexos);
                var novosAnexos = new List<AnexosDb>();

                if (historicoContatos.HistoricoContatosOcorrenciaAnexos != null)
                {

                    foreach (var item in historicoContatos.HistoricoContatosOcorrenciaAnexos)
                    {
                        novosAnexos.Add(new AnexosDb
                        {
                            anexo_anexo = ConverterArquivoParaBase64(item),
                            anexo_historico_contato_fk = contatoOcorrencia.historico_contatos_ocorrencia_id,
                            anexo_nome = item.FileName
                        });
                    }
                    _mysql.anexos.AddRange(novosAnexos);
                    _mysql.SaveChanges();
                }

                if (anexos.Count != novosAnexos.Count)
                {
                    bool adicionouArquivo = anexos.Count < novosAnexos.Count;
                    if (adicionouArquivo)
                    {
                        var nomes = anexos.Select(x => x.anexo_nome).ToList();
                        var lgAnexo = novosAnexos.Where(x => !nomes.Contains(x.anexo_nome)).Select(x => x.anexo_nome).ToList();
                        _log.NovaAlteracao("Adicionado(s) Novo(s) Anexo(s)", $"Anexos Adicionados:\n {string.Join("\n ", lgAnexo)}", usuarioLogadoId, ETipoLog.Atendimento, contatoOcorrencia.historico_contatos_ocorrencia_id);
                    }
                    else
                    {
                        var nomes = novosAnexos.Select(x => x.anexo_nome).ToList();
                        var lgAnexo = anexos.Where(x => !nomes.Contains(x.anexo_nome)).Select(x => x.anexo_nome).ToList();
                        _log.NovaAlteracao("Removeu Anexo(s)", $"Anexos Removidos:\n {string.Join("\n ", lgAnexo)}", usuarioLogadoId, ETipoLog.Atendimento, contatoOcorrencia.historico_contatos_ocorrencia_id);

                    }
                }

                var (CamposAlterados, Log) = Comparador.CompararObjetos(old, contatoOcorrencia);

                if (CamposAlterados.Count > 0 && Log.Count > 0)
                    _log.NovaAlteracao($"Alteração No Atendimento", string.Join("\n", Log), usuarioLogadoId, ETipoLog.Atendimento, contatoOcorrencia.historico_contatos_ocorrencia_id);

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
        public string ConverterArquivoParaBase64(IFormFile arquivo)
        {
            using (var memoryStream = new MemoryStream())
            {
                arquivo.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
        public void NovoContatoOcorrencia(HistoricoContatosOcorrenciaRequest historicoContatos, int usuarioId)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var hst = new Repository.Maps.HistoricoContatosOcorrenciaDb
                {
                    historico_contatos_ocorrencia_cliente_id = historicoContatos.HistoricoContatosOcorrenciaClienteId,
                    historico_contatos_ocorrencia_descricao = historicoContatos.HistoricoContatosOcorrenciaDescricao,
                    historico_contatos_ocorrencia_dt_cadastro = DateTime.Now,
                    historico_contatos_ocorrencia_dt_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaDtOcorrencia,
                    historico_contatos_ocorrencia_motivo_contato_id = historicoContatos.HistoricoContatosOcorrenciaMotivoContatoId,
                    historico_contatos_ocorrencia_origem_id = historicoContatos.HistoricoContatosOcorrenciaOrigemId,
                    historico_contatos_ocorrencia_situacao_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaSituacaoOcorrencia.ToUpper(),
                    historico_contatos_ocorrencia_banco = historicoContatos.HistoricoContatosOcorrenciaBanco != null ? historicoContatos.HistoricoContatosOcorrenciaBanco.Replace("null", "") : "",
                    historico_contatos_ocorrencia_agencia = historicoContatos.HistoricoContatosOcorrenciaAgencia != null ? historicoContatos.HistoricoContatosOcorrenciaAgencia.Replace("null", "") : "",
                    historico_contatos_ocorrencia_conta = historicoContatos.HistoricoContatosOcorrenciaConta != null ? historicoContatos.HistoricoContatosOcorrenciaConta.Replace("null", "") : "",
                    historico_contatos_ocorrencia_digito = historicoContatos.HistoricoContatosOcorrenciaDigito != null ? historicoContatos.HistoricoContatosOcorrenciaDigito.Replace("null", "") : "",
                    historico_contatos_ocorrencia_chave_pix = historicoContatos.HistoricoContatosOcorrenciaPix != null ? historicoContatos.HistoricoContatosOcorrenciaPix.Replace("null", "") : "",
                    historico_contatos_ocorrencia_tipo_chave_pix = historicoContatos.HistoricoContatosOcorrenciaTipoChavePix != null ? historicoContatos.HistoricoContatosOcorrenciaTipoChavePix.Replace("null", "") : "",
                    historico_contatos_ocorrencia_telefone = historicoContatos.HistoricoContatosOcorrenciaTelefone != null ? historicoContatos.HistoricoContatosOcorrenciaTelefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "",
                    historico_contatos_ocorrencia_usuario_fk = usuarioId,
                    historico_contatos_ocorrencia_tipo_conta = historicoContatos.HistoricoContatosOcorrenciaTipoConta != null ? historicoContatos.HistoricoContatosOcorrenciaTipoConta.Replace("null", "") : "",
                    historico_contatos_ocorrencia_valor_reembolso = historicoContatos?.HistoricoContatosOcorrenciaValorReembolso != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorReembolso, CultureInfo.InvariantCulture) : 0m,
                    historico_contatos_ocorrencia_valor_parcela = historicoContatos?.HistoricoContatosOcorrenciaValorParcela != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorParcela, CultureInfo.InvariantCulture) : 0m,
                    historico_contatos_ocorrencia_valor_parcela_2 = historicoContatos?.HistoricoContatosOcorrenciaValorParcela2 != null ? decimal.Parse(historicoContatos.HistoricoContatosOcorrenciaValorParcela2, CultureInfo.InvariantCulture) : 0m
                };

                _mysql.historico_contatos_ocorrencia.Add(hst);
                _mysql.SaveChanges();

                if (historicoContatos.HistoricoContatosOcorrenciaAnexos != null)
                {
                    foreach (var item in historicoContatos.HistoricoContatosOcorrenciaAnexos)
                    {
                        _mysql.anexos.Add(new AnexosDb
                        {
                            anexo_anexo = ConverterArquivoParaBase64(item),
                            anexo_historico_contato_fk = hst.historico_contatos_ocorrencia_id,
                            anexo_nome = item.FileName
                        });
                        _mysql.SaveChanges();
                    }
                }

                _log.NovaAlteracao("Novo Atendimento", "-", usuarioId, ETipoLog.Atendimento, hst.historico_contatos_ocorrencia_id);

                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
        public (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) BuscarTodosClientes(ConsultaParametros request)
        {
            request.StatusCliente = request.StatusCliente ?? 0;
            request.StatusRemessa = request.StatusRemessa ?? 0;
            request.StatusIntegraall = request.StatusIntegraall ?? 0;

            var clientes = (from cli in _mysql.clientes
                            join vin in _mysql.vinculo_cliente_captador on cli.cliente_id equals vin.vinculo_cliente_id
                            join cpt in _mysql.captadores on vin.vinculo_captador_id equals cpt.captador_id
                            join his in _mysql.historico_contatos_ocorrencia on cli.cliente_id equals his.historico_contatos_ocorrencia_cliente_id into hisGroup
                            from his in hisGroup.DefaultIfEmpty()
                            join ori in _mysql.origem on his == null ? (int?)null : his.historico_contatos_ocorrencia_origem_id equals ori.origem_id into oriGroup
                            from ori in oriGroup.DefaultIfEmpty()
                            join mot in _mysql.motivo_contato on his == null ? (int?)null : his.historico_contatos_ocorrencia_motivo_contato_id equals mot.motivo_contato_id into motGroup
                            from mot in motGroup.DefaultIfEmpty()
                            where
                                   (request.DateInit == null || cli.cliente_dataCadastro >= request.DateInit)
                                && (request.DateEnd == null || cli.cliente_dataCadastro < request.DateEnd.Value.AddDays(1))
                                && (request.DateInitAverbacao == null || cli.cliente_DataAverbacao >= request.DateInitAverbacao)
                                && (request.DateEndAverbacao == null || cli.cliente_DataAverbacao < request.DateEndAverbacao.Value.AddDays(1))
                                && (string.IsNullOrEmpty(request.Nome) || cli.cliente_nome.ToUpper().Contains(request.Nome))
                                && (string.IsNullOrEmpty(request.Cpf) || cli.cliente_cpf.ToUpper().Contains(request.Cpf.Replace(".", "").Replace("-", "")))
                                && (string.IsNullOrEmpty(request.Beneficio) || cli.cliente_matriculaBeneficio.Contains(request.Beneficio))
                            select new BuscarClienteByIdResponse
                            {
                                Captador = cpt,
                                Cliente = cli,
                                Historico = his,
                                Origem = ori,
                                Motivo = mot
                            }).ToList()
                            .Distinct()
                            .ToList()
                            .OrderByDescending(x => x.Cliente.cliente_dataCadastro).ToList();

            if (request.CadastroExterno == 1)
            {
                clientes = clientes.Where(x => x.Cliente.clientes_cadastro_externo).ToList();
            }

            if (request.CadastroExterno == 2)
            {
                clientes = clientes.Where(x => !x.Cliente.clientes_cadastro_externo).ToList();
            }

            if (request.StatusCliente == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao).ToList();
            }

            if (request.StatusCliente == 3)
            {
                clientes = clientes.Where(x => !x.Cliente.cliente_situacao).ToList();
            }

            if (request.StatusRemessa == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_remessa_id != null && x.Cliente.cliente_remessa_id > 0).ToList();
            }

            if (request.StatusRemessa == 2)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_remessa_id == null || x.Cliente.cliente_remessa_id == 0).ToList();
            }

            if (request.StatusIntegraall > 0)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_StatusIntegral == request.StatusIntegraall).ToList();
            }

            if (request.SituacaoOcorrencia != "TODOS" && request.SituacaoOcorrencia != null)
            {
                clientes = clientes
                    .Where(x => x.Historico != null &&
                                x.Historico.historico_contatos_ocorrencia_situacao_ocorrencia != null &&
                                x.Historico.historico_contatos_ocorrencia_situacao_ocorrencia == request.SituacaoOcorrencia)
                    .ToList();
            }

            if (request.DataInitAtendimento != null)
            {
                var dataInitAtendimento = request.DataInitAtendimento.Value.Date;
                clientes = clientes.Where(x => x?.Historico?.historico_contatos_ocorrencia_dt_ocorrencia.Date >= dataInitAtendimento).ToList();
            }

            if (request.DataEndAtendimento != null)
            {
                var dataEndAtendimento = request.DataEndAtendimento.Value.Date;
                clientes = clientes.Where(x => x?.Historico?.historico_contatos_ocorrencia_dt_ocorrencia.Date <= dataEndAtendimento).ToList();
            }

            foreach (var cliente in clientes)
            {
                var ben = _mysql.log_beneficios.Where(x => x.log_beneficios_cliente_id == cliente.Cliente.cliente_id && x.log_beneficios_ativo).ToList();
                if (ben.Count == 0)
                {
                    cliente.Beneficios = new List<BeneficioDb>
                    {
                        new() {
                            beneficio_nome_beneficio = "NENHUM BENEFICIO ATIVO",
                            beneficio_id = 0
                        }
                    };
                }
                else
                {
                    var ids = ben.Select(x => x.log_beneficios_beneficio_id).ToList();
                    cliente.Beneficios = _mysql.beneficios.Where(x => ids.Contains(x.beneficio_id)).ToList();
                }

                var ultimoStatus = _mysql.log_status
                .Where(l => l.log_status_cliente_id == cliente.Cliente.cliente_id)
                .Max(l => l.log_status_id);
                if (ultimoStatus > 0)
                {
                    var idStatus = _mysql.log_status.FirstOrDefault(x => x.log_status_id == ultimoStatus).log_status_novo_id;

                    cliente.StatusAtual = _mysql.status.FirstOrDefault(x => x.status_id == idStatus);
                }
            }

            if (request.StatusCliente == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao && x.StatusAtual != null && (x.StatusAtual.status_id == (int)EStatus.Ativo || x.StatusAtual.status_id == (int)EStatus.AtivoAguardandoAverbacao)).ToList();
            }

            if (request.StatusCliente == 2)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao && x.StatusAtual != null && x.StatusAtual.status_id == (int)EStatus.Inativo).ToList();
            }

            var todosClientes = clientes.ToList().Distinct().ToList();
            int totalClientes = todosClientes.Count();
            if (request.PaginaAtual == null)
                return (todosClientes.OrderByDescending(x => x.Cliente.cliente_dataCadastro).ToList(), 0, totalClientes);

            return CalcularPagina(todosClientes, request.PaginaAtual, totalClientes);
        }
        public byte[] DownloadContatoFiltro(ConsultaParametros request)
        {
            var clientesData = (from cli in _mysql.clientes
                                join hist in _mysql.historico_contatos_ocorrencia
                                    on cli.cliente_id equals hist.historico_contatos_ocorrencia_cliente_id
                                join mot in _mysql.motivo_contato
                                    on hist.historico_contatos_ocorrencia_motivo_contato_id equals mot.motivo_contato_id
                                join ori in _mysql.origem
                                    on hist.historico_contatos_ocorrencia_origem_id equals ori.origem_id
                                join vin in _mysql.vinculo_cliente_captador
                                    on cli.cliente_id equals vin.vinculo_cliente_id
                                join cap in _mysql.captadores
                                    on vin.vinculo_captador_id equals cap.captador_id
                                join ultimo_usu in _mysql.usuarios
                                    on hist.historico_contatos_ocorrencia_usuario_fk equals ultimo_usu.usuario_id
                                where (string.IsNullOrEmpty(request.Cpf) || cli.cliente_cpf == request.Cpf.PadLeft(11, '0').Replace(".", "").Replace("-", "").Replace(" ", "")) &&
                                      (string.IsNullOrEmpty(request.Beneficio) || cli.cliente_matriculaBeneficio == request.Beneficio) &&
                                      (!request.DataInitAtendimento.HasValue || hist.historico_contatos_ocorrencia_dt_ocorrencia >= request.DataInitAtendimento.Value) &&
                                      (!request.DataEndAtendimento.HasValue || hist.historico_contatos_ocorrencia_dt_ocorrencia < request.DataEndAtendimento.Value.AddDays(1)) &&
                                      (request.SituacoesOcorrencias == null || !request.SituacoesOcorrencias.Any() ||
                                      request.SituacoesOcorrencias.Contains(hist.historico_contatos_ocorrencia_situacao_ocorrencia))
                                select new
                                {
                                    Cliente = cli,
                                    Historico = hist,
                                    Motivo = mot,
                                    Origem = ori,
                                    Captador = cap,
                                    Usuario = ultimo_usu,
                                    UsuarioCriador = (from criador in _mysql.usuarios
                                                      join histo in _mysql.historico_contatos_ocorrencia
                                                            on criador.usuario_id equals histo.historico_contatos_ocorrencia_usuario_fk
                                                      where histo.historico_contatos_ocorrencia_cliente_id == cli.cliente_id
                                                      orderby histo.historico_contatos_ocorrencia_dt_ocorrencia
                                                      select criador).FirstOrDefault()
                                }).ToList()
                               .Select(x => new
                               {
                                   ClienteId = x.Cliente.cliente_id.ToString(),
                                   Cpf = string.IsNullOrEmpty(x.Cliente.cliente_cpf) ? ";" : x.Cliente.cliente_cpf,
                                   Nome = string.IsNullOrEmpty(x.Cliente.cliente_nome) ? ";" : x.Cliente.cliente_nome,
                                   Logradouro = string.IsNullOrEmpty(x.Cliente.cliente_logradouro) ? ";" : x.Cliente.cliente_logradouro,
                                   Uf = string.IsNullOrEmpty(x.Cliente.cliente_uf) ? ";" : x.Cliente.cliente_uf,
                                   Numero = string.IsNullOrEmpty(x.Cliente.cliente_numero) ? ";" : x.Cliente.cliente_numero,
                                   Complemento = string.IsNullOrEmpty(x.Cliente.cliente_complemento) ? ";" : x.Cliente.cliente_complemento,
                                   Data_Nascimento = x.Cliente.cliente_dataNasc.ToString("dd/MM/yyyy"),
                                   Numero_Documento = string.IsNullOrEmpty(x.Cliente.cliente_nrDocto) ? ";" : x.Cliente.cliente_nrDocto,
                                   Empregador = string.IsNullOrEmpty(x.Cliente.cliente_empregador) ? ";" : x.Cliente.cliente_empregador,
                                   Matricula_Benefico = string.IsNullOrEmpty(x.Cliente.cliente_matriculaBeneficio) ? ";" : x.Cliente.cliente_matriculaBeneficio,
                                   Nome_Mae = string.IsNullOrEmpty(x.Cliente.cliente_nomeMae) ? ";" : x.Cliente.cliente_nomeMae,
                                   Nome_Pai = string.IsNullOrEmpty(x.Cliente.cliente_nomePai) ? ";" : x.Cliente.cliente_nomePai,
                                   Telefone_Fixo = string.IsNullOrEmpty(x.Cliente.cliente_telefoneFixo) ? ";" : x.Cliente.cliente_telefoneFixo,
                                   Telefone_Celular = string.IsNullOrEmpty(x.Cliente.cliente_telefoneCelular) ? ";" : x.Cliente.cliente_telefoneCelular,
                                   Possui_Whatsapp = x.Cliente.cliente_possuiWhatsapp ? "Sim" : "Não",
                                   Funcao_Aaspa = string.IsNullOrEmpty(x.Cliente.cliente_funcaoAASPA) ? ";" : x.Cliente.cliente_funcaoAASPA,
                                   Email = string.IsNullOrEmpty(x.Cliente.cliente_email) ? ";" : x.Cliente.cliente_email,
                                   Situacao = x.Cliente.cliente_situacao ? "Ativo" : "Inativo",
                                   Estado_Civil = x.Cliente.cliente_estado_civil.ToString(),
                                   Sexo = x.Cliente.cliente_estado_civil switch
                                   {
                                       1 => "Solteiro",
                                       2 => "Casado",
                                       3 => "Viúvo",
                                       4 => "Separado judicialmente",
                                       5 => "União estável",
                                       6 => "Outros",
                                       _ => "Não especificado"
                                   },
                                   RemessaId = x.Cliente.cliente_remessa_id.ToString(),
                                   Captador_Nome = string.IsNullOrEmpty(x.Captador.captador_nome) ? ";" : x.Captador.captador_nome,
                                   Captador_CPF_ou_CNPJ = string.IsNullOrEmpty(x.Captador.captador_cpf_cnpj) ? ";" : x.Captador.captador_cpf_cnpj,
                                   Captador_Descricao = string.IsNullOrEmpty(x.Captador.captador_descricao) ? ";" : x.Captador.captador_descricao,
                                   Data_Averbacao = x.Cliente.cliente_DataAverbacao.HasValue ? x.Cliente.cliente_DataAverbacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : ";",
                                   Status_Integraall = string.IsNullOrEmpty(x.Cliente.cliente_StatusIntegral.ToString()) ? ";" : x.Cliente.cliente_StatusIntegral.ToString(),
                                   Motivo_Atendimento = string.IsNullOrEmpty(x.Motivo?.motivo_contato_nome) ? ";" : x.Motivo.motivo_contato_nome,
                                   Origem_Atendimento = string.IsNullOrEmpty(x.Origem?.origem_nome) ? ";" : x.Origem.origem_nome,
                                   Situacao_Atendimento = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_situacao_ocorrencia) ? ";" : x.Historico.historico_contatos_ocorrencia_situacao_ocorrencia,
                                   Usuario_Responsavel_Pelo_Registro = string.IsNullOrEmpty(x.UsuarioCriador.usuario_nome) ? ";" : x.UsuarioCriador.usuario_nome,
                                   Usuario_Responsavel_Pela_Ultima_Alteracao = string.IsNullOrEmpty(x.Usuario.usuario_nome) ? ";" : x.Usuario.usuario_nome,
                                   Data_Atendimento = x.Historico != null ? x.Historico.historico_contatos_ocorrencia_dt_ocorrencia.ToString("dd/MM/yyyy HH:mm:ss") : ";",
                                   Descricao_Atendimento = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_descricao) ? ";" : x.Historico.historico_contatos_ocorrencia_descricao.Replace("\r"," ").Replace("\n", " "),
                                   Dados_Bancarios_Banco = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_banco) ? ";" : x.Historico.historico_contatos_ocorrencia_banco,
                                   Dados_Bancarios_Agencia = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_agencia) ? ";" : x.Historico.historico_contatos_ocorrencia_agencia,
                                   Dados_Bancarios_Conta = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_conta) ? ";" : x.Historico.historico_contatos_ocorrencia_conta,
                                   Dados_Bancarios_Digito = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_digito) ? ";" : x.Historico.historico_contatos_ocorrencia_digito,
                                   Dados_Bancarios_Chave_Tipo_Chave_Pix = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_tipo_chave_pix) ? ";" : x.Historico.historico_contatos_ocorrencia_tipo_chave_pix,
                                   Dados_Bancarios_Chave_Pix = string.IsNullOrEmpty(x.Historico?.historico_contatos_ocorrencia_chave_pix) ? ";" : x.Historico.historico_contatos_ocorrencia_chave_pix,
                                   Valor_Parcela = x.Historico?.historico_contatos_ocorrencia_valor_parcela.ToString("C", new System.Globalization.CultureInfo("pt-BR")) ?? ";",
                                   Valor_Parcela_2 = x.Historico?.historico_contatos_ocorrencia_valor_parcela_2.ToString("C", new System.Globalization.CultureInfo("pt-BR")) ?? ";",
                                   Valor_Total_Reembolso = x.Historico?.historico_contatos_ocorrencia_valor_reembolso.ToString("C", new System.Globalization.CultureInfo("pt-BR")) ?? ";"
                               }).ToList();

            var dados = clientesData; // Substitua pelo seu código real

            var csv = new StringBuilder();

            // Cabeçalho
            var header = string.Join(";", dados.First().GetType().GetProperties().Select(p => p.Name));
            csv.AppendLine(header);

            // Linhas de dados
            foreach (var item in dados)
            {
                var values = item.GetType().GetProperties().Select(p =>
                {
                    var value = p.GetValue(item)?.ToString()?.Replace(";", "-").Replace("\r", " ")?.Replace("\n", " ") ?? "";
                    return value;
                });

                csv.AppendLine(string.Join(";", values));
            }
            byte[] fileBytes = Encoding.Latin1.GetBytes(csv.ToString());
            return fileBytes;

            //File.WriteAllText("clientes.csv", csv.ToString(), Encoding.UTF8);


            //var builder = new StringBuilder();

            //var cabecalho = string.Join(";", clientesData.First().GetType().GetProperties().Select(p => p.Name));
            //builder.Append(cabecalho);
            //foreach (var item in clientesData)
            //{
            //    var properties = item.GetType().GetProperties();
            //    var valores = properties.Select(p => p.GetValue(item)?.ToString() ?? ";");
            //    builder.AppendLine(string.Join(";", valores));
            //}
            //byte[] fileBytes = Encoding.Latin1.GetBytes(builder.ToString());
            //return fileBytes;
        }
        private (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) CalcularPagina(List<BuscarClienteByIdResponse> todosClientes, int? paginaAtual, int totalClientes)
        {
            int qtdPorPagina = 5;
            int pagina = paginaAtual ?? 1;

            int indiceInicial = (pagina - 1) * qtdPorPagina;

            var qtd = (todosClientes.Count + qtdPorPagina - 1) / qtdPorPagina; ;

            var qtdPaginas = Math.Ceiling(Convert.ToDecimal(qtd));

            qtdPaginas = qtdPaginas > 0 ? qtdPaginas : 1;

            return (todosClientes.Skip(indiceInicial).Take(qtdPorPagina).ToList().OrderByDescending(x => x.Cliente.cliente_dataCadastro).ToList(), Convert.ToInt32(qtdPaginas), totalClientes);
        }
    }
}
