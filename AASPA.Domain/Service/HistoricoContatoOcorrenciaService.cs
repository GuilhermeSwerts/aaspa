using AASPA.Domain.Interface;
using AASPA.Domain.Util;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    historico_contatos_ocorrencia_tipo_conta = historicoContatos.HistoricoContatosOcorrenciaTipoConta != null ? historicoContatos.HistoricoContatosOcorrenciaTipoConta.Replace("null", "")  : ""
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
            var clientes = (from cli in _mysql.clientes
                            join hist in _mysql.historico_contatos_ocorrencia on cli.cliente_id equals hist.historico_contatos_ocorrencia_cliente_id
                            select new
                            {
                                Cliente = cli,
                                Historico = hist
                            })
                .ToList() // Transfere os dados para memória
                .GroupBy(x => x.Cliente)
                .Select(x => new
                {
                    Cliente = x.Key,
                    UltimoHistorico = x
                        .OrderByDescending(h => h.Historico.historico_contatos_ocorrencia_dt_cadastro) // Ordena por data de cadastro
                        .FirstOrDefault().Historico // Pega o último histórico
                })
                .ToList();


            var clientesData = (from data in clientes
                                join mot in _mysql.motivo_contato on data.UltimoHistorico.historico_contatos_ocorrencia_motivo_contato_id equals mot.motivo_contato_id
                                join ori in _mysql.origem on data.UltimoHistorico.historico_contatos_ocorrencia_origem_id equals ori.origem_id
                                join vin in _mysql.vinculo_cliente_captador on data.Cliente.cliente_id equals vin.vinculo_cliente_id
                                join cap in _mysql.captadores on vin.vinculo_captador_id equals cap.captador_id
                                where
                                    (string.IsNullOrEmpty(request.Cpf) || data.Cliente.cliente_cpf == request.Cpf.Replace(".","").Replace("-","").Replace(" ","")) &&
                                    (string.IsNullOrEmpty(request.Beneficio) || data.Cliente.cliente_matriculaBeneficio == request.Beneficio) &&
                                    (!request.DataInitAtendimento.HasValue || data.UltimoHistorico.historico_contatos_ocorrencia_dt_ocorrencia >= request.DataInitAtendimento.Value) &&
                                    (!request.DataEndAtendimento.HasValue || data.UltimoHistorico.historico_contatos_ocorrencia_dt_ocorrencia < request.DataEndAtendimento.Value.AddDays(1)) &&
                                    ((request.SituacoesOcorrencias == null || request.SituacoesOcorrencias.Count == 0) ||  request.SituacoesOcorrencias.Contains(data.UltimoHistorico.historico_contatos_ocorrencia_situacao_ocorrencia))
                                select new BuscarClienteByIdResponse
                                {
                                    Cliente = data.Cliente,
                                    Historico = data.UltimoHistorico,
                                    Motivo = mot,
                                    Origem = ori,
                                    Captador = cap,
                                }).Distinct().ToList();

            var builder = new StringBuilder();

            builder.AppendLine("#;CPF;NOME;CEP;LOGRADOURO;BAIRRO;LOCALIDADE;UF;NUMERO;COMPLEMENTO;DATANASC;DATACADASTRO;NRDOCTO;EMPREGADOR;MATRICULABENEFICIO;NOMEMAE;NOMEPAI;TELEFONEFIXO;TELEFONECELULAR;POSSUIWHATSAPP;FUNCAOAASPA;EMAIL;SITUACAO;ESTADO_CIVIL;SEXO;REMESSA_ID;CAPTADOR_NOME;CAPTADOR_CPF_OU_CNPJ;CAPTADOR_DESCRICAO;DATA_AVERBACAO;STATUS_INTEGRAALL;MOTIVO_ATENDIMENTO;ORIGEM_ATENDIMENTO;SITUACAO_ATENDIMENTO;DATA_ATENDIMENTO;DESCRICAO_ATENDIMENTO;DADOS_BANCARIOS_BANCO;DADOS_BANCARIOS_AGENCIA;DADOS_BANCARIOS_CONTA;DADOS_BANCARIOS_DIGITO;DADOS_BANCARIOS_CHAVE_PIX");
            
            for (int i = 0; i < clientesData.Count; i++)
            {
                var cliente = clientesData[i];
                var dtAverbacao = cliente.Cliente.cliente_DataAverbacao.HasValue ? cliente.Cliente.cliente_DataAverbacao.Value.ToString("dd/MM/yyyy hh:mm:ss:") : "";
                var motivoNome = cliente.Motivo != null ? cliente.Motivo.motivo_contato_nome : ";;";
                var origem = (cliente.Origem != null && !string.IsNullOrEmpty(cliente.Origem.origem_nome)) ? cliente.Origem.origem_nome : ";;";
                var remessa = cliente.Cliente.cliente_remessa_id ?? 0;
                var historico = cliente.Historico != null ? $"{cliente.Historico.historico_contatos_ocorrencia_situacao_ocorrencia};{cliente.Historico.historico_contatos_ocorrencia_dt_ocorrencia:dd/MM/yyyy hh:mm:ss};{cliente.Historico.historico_contatos_ocorrencia_descricao};{cliente.Historico.historico_contatos_ocorrencia_banco};{cliente.Historico.historico_contatos_ocorrencia_agencia};{cliente.Historico.historico_contatos_ocorrencia_conta};{cliente.Historico.historico_contatos_ocorrencia_digito};{cliente.Historico.historico_contatos_ocorrencia_chave_pix}" : ";;;;;;";
                builder.AppendLine(
                    $"{cliente.Cliente.cliente_id};" +
                    $"{cliente.Cliente.cliente_cpf};" +
                    $"{cliente.Cliente.cliente_nome};" +
                    $"{cliente.Cliente.cliente_cep};" +
                    $"{cliente.Cliente.cliente_logradouro};" +
                    $"{cliente.Cliente.cliente_bairro};" +
                    $"{cliente.Cliente.cliente_localidade};" +
                    $"{cliente.Cliente.cliente_uf};" +
                    $"{cliente.Cliente.cliente_numero.Replace(";", "")};" +
                    $"{cliente.Cliente.cliente_complemento};" +
                    $"{cliente.Cliente.cliente_dataNasc.ToString("dd/MM/yyyy")};" +
                    $"{cliente.Cliente.cliente_dataCadastro};" +
                    $"{cliente.Cliente.cliente_nrDocto};" +
                    $"{cliente.Cliente.cliente_empregador};" +
                    $"{cliente.Cliente.cliente_matriculaBeneficio};" +
                    $"{cliente.Cliente.cliente_nomeMae};" +
                    $"{cliente.Cliente.cliente_nomePai};" +
                    $"{cliente.Cliente.cliente_telefoneFixo};" +
                    $"{cliente.Cliente.cliente_telefoneCelular};" +
                    $"{(cliente.Cliente.cliente_possuiWhatsapp ? "Sim" : "Não")};" +
                    $"{cliente.Cliente.cliente_funcaoAASPA};" +
                    $"{cliente.Cliente.cliente_email};" +
                    $"{(cliente.Cliente.cliente_situacao ? "Ativo" : "Inativo")};" +
                    $"{(cliente.Cliente.cliente_estado_civil switch { 1 => "Solteiro", 2 => "Casado", 3 => "Viúvo", 4 => "Separado judicialmente", 5 => "União estável", 6 => "Outros", _ => "Não especificado" })};" +
                    $"{(cliente.Cliente.cliente_sexo == 1? "Masculino" : cliente.Cliente.cliente_sexo == 2? "Feminino" : "Outros")};" +
                    $"{remessa};" +
                    $"{cliente.Captador.captador_nome};" +
                    $"{cliente.Captador.captador_cpf_cnpj};" +
                    $"{cliente.Captador.captador_descricao.Replace("\r\n", "").Replace("\n", "").Replace("\r", "")};" +
                    $"{dtAverbacao};" +
                    $"{cliente.Cliente.cliente_StatusIntegral};" +
                    $"{motivoNome};" +
                    $"{origem};" +
                    $"{historico.Replace("\r\n", "").Replace("\n", "").Replace("\r", "")}");
            }

            byte[] fileBytes = Encoding.Latin1.GetBytes(builder.ToString());
            return fileBytes;
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
