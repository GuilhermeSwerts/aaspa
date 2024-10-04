using AASPA.Domain.CustonException;
using AASPA.Domain.Interface;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Newtonsoft.Json;
using NuGet.Packaging;
using Paket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class ClienteService : ICliente
    {
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;
        private static readonly HttpClient _httpClient = new HttpClient();
        private string login = "AASPA";
        private string senha = "l@znNL,Lkc9x";
        private string captcha = "XD5V";
        private string token = "LWGb8VjYsZZkmJfA9JK9tQ==:E1huR9Q8It+WFpAES+pLsA==:0urZEQiqBcMNEGchHF8Elg==";

        public ClienteService(MysqlContexto mysql, IHostEnvironment env)
        {
            _mysql = mysql;
            _env = env;
        }

        public BuscarClienteByIdResponse BuscarClienteID(int clienteId)
        {
            try
            {
                var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == clienteId)
                    ?? throw new Exception("Cliente não encontrado.");

                var vinculo = _mysql.vinculo_cliente_captador.FirstOrDefault(x => x.vinculo_cliente_id == clienteId)
                    ?? throw new Exception("Vinculo de Cliente X Captador não encontrado.");

                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_id == vinculo.vinculo_captador_id)
                    ?? throw new Exception("Captador não encontrado.");

                var benIds = _mysql.log_beneficios.Where(x => x.log_beneficios_cliente_id == clienteId && x.log_beneficios_ativo).ToList().Select(x => x.log_beneficios_beneficio_id).ToList();
                var beneficios = _mysql.beneficios.Where(x => benIds.Contains(x.beneficio_id)).ToList();

                var ultimoStatus = _mysql.log_status
                    .Where(l => l.log_status_cliente_id == cliente.cliente_id)
                    .Max(l => l.log_status_id);
                StatusDb statusDb;

                var idStatus = _mysql.log_status.FirstOrDefault(x => x.log_status_id == ultimoStatus).log_status_novo_id;

                statusDb = _mysql.status.FirstOrDefault(x => x.status_id == idStatus);

                return new BuscarClienteByIdResponse
                {
                    Captador = captador,
                    Cliente = cliente,
                    Beneficios = beneficios,
                    StatusAtual = statusDb
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AtualizaCliente(ClienteRequest novoCliente)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                string cpf = novoCliente.Cliente.Cpf.Replace(".", "").Replace("-", "");
                var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == novoCliente.Cliente.Id) ??
                    throw new Exception($"Cliente com id: ${novoCliente.Cliente.Id} não cadastrado.");

                cliente.cliente_dataCadastro = novoCliente.Cliente.DataCad;
                cliente.cliente_cpf = cpf;
                cliente.cliente_nome = novoCliente.Cliente.Nome ?? "";
                cliente.cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", "");
                cliente.cliente_logradouro = novoCliente.Cliente.Logradouro ?? "";
                cliente.cliente_bairro = novoCliente.Cliente.Bairro ?? "";
                cliente.cliente_localidade = novoCliente.Cliente.Localidade ?? "";
                cliente.cliente_uf = novoCliente.Cliente.Uf ?? "";
                cliente.cliente_numero = novoCliente.Cliente.Numero ?? "";
                cliente.cliente_complemento = novoCliente.Cliente.Complemento ?? "";
                cliente.cliente_dataNasc = novoCliente.Cliente.DataNasc;
                cliente.cliente_nrDocto = novoCliente.Cliente.NrDocto ?? "";
                cliente.cliente_empregador = novoCliente.Cliente.Empregador ?? "";
                cliente.cliente_matriculaBeneficio = novoCliente.Cliente.MatriculaBeneficio ?? "";
                cliente.cliente_nomeMae = novoCliente.Cliente.NomeMae ?? "";
                cliente.cliente_nomePai = novoCliente.Cliente.NomePai ?? "";
                cliente.cliente_telefoneFixo = novoCliente.Cliente.TelefoneFixo != null ? novoCliente.Cliente.TelefoneFixo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                cliente.cliente_telefoneCelular = novoCliente.Cliente.TelefoneCelular != null ? novoCliente.Cliente.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                cliente.cliente_possuiWhatsapp = novoCliente.Cliente.PossuiWhatsapp;
                cliente.cliente_funcaoAASPA = novoCliente.Cliente.FuncaoAASPA ?? "";
                cliente.cliente_email = novoCliente.Cliente.Email ?? "";
                cliente.cliente_estado_civil = novoCliente.Cliente.EstadoCivil;
                cliente.cliente_sexo = novoCliente.Cliente.Sexo ?? 0;

                _mysql.SaveChanges();

                string captadorCpfCnpj = novoCliente.Captador.CpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                var vinculo = _mysql.vinculo_cliente_captador.FirstOrDefault(x => x.vinculo_cliente_id == novoCliente.Cliente.Id);
                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_id == vinculo.vinculo_captador_id);
                captador.captador_cpf_cnpj = captadorCpfCnpj;
                captador.captador_nome = novoCliente.Captador.Nome;
                captador.captador_descricao = novoCliente.Captador.Descricao ?? "";
                captador.captador_e_cnpj = captadorCpfCnpj.Length > 11;

                _mysql.SaveChanges();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Houve um erro ao cadastrar um novo cliente.");
            }
        }

        public void NovoCliente(ClienteRequest novoCliente, bool isList = false, bool cadastroExterno = false)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var cliente = new ClienteDb();
                string cpf = novoCliente.Cliente.Cpf.Replace(".", "").Replace("-", "").PadLeft(11, '0');
                var clienteCadastrado = _mysql.clientes.FirstOrDefault(x => x.cliente_cpf == cpf);
                if (clienteCadastrado != null)
                {
                    if (!isList)
                    {
                        throw new ClienteException($"Cliente com o cpf: {cpf} já cadastrado.");
                    }
                    else
                    {
                        clienteCadastrado.cliente_nome = novoCliente.Cliente.Nome ?? "";
                        clienteCadastrado.cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", "");
                        clienteCadastrado.cliente_logradouro = novoCliente.Cliente.Logradouro ?? "";
                        clienteCadastrado.cliente_bairro = novoCliente.Cliente.Bairro ?? "";
                        clienteCadastrado.cliente_localidade = novoCliente.Cliente.Localidade ?? "";
                        clienteCadastrado.cliente_uf = novoCliente.Cliente.Uf ?? "";
                        clienteCadastrado.cliente_numero = novoCliente.Cliente.Numero ?? "";
                        clienteCadastrado.cliente_complemento = novoCliente.Cliente.Complemento ?? "";
                        clienteCadastrado.cliente_dataNasc = novoCliente.Cliente.DataNasc;
                        clienteCadastrado.cliente_dataCadastro = novoCliente.Cliente.DataCad;
                        clienteCadastrado.cliente_nrDocto = novoCliente.Cliente.NrDocto ?? string.Empty;
                        clienteCadastrado.cliente_empregador = novoCliente.Cliente.Empregador ?? "";
                        clienteCadastrado.cliente_matriculaBeneficio = novoCliente.Cliente.MatriculaBeneficio ?? "";
                        clienteCadastrado.cliente_nomeMae = novoCliente.Cliente.NomeMae ?? "";
                        clienteCadastrado.cliente_nomePai = novoCliente.Cliente.NomePai ?? "";
                        clienteCadastrado.cliente_telefoneFixo = novoCliente.Cliente.TelefoneFixo != null ? novoCliente.Cliente.TelefoneFixo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : null;
                        clienteCadastrado.cliente_telefoneCelular = novoCliente.Cliente.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        clienteCadastrado.cliente_possuiWhatsapp = novoCliente.Cliente.PossuiWhatsapp;
                        clienteCadastrado.cliente_funcaoAASPA = novoCliente.Cliente.FuncaoAASPA ?? "";
                        clienteCadastrado.cliente_estado_civil = novoCliente.Cliente.EstadoCivil;
                        clienteCadastrado.cliente_sexo = novoCliente.Cliente.Sexo;
                        clienteCadastrado.clientes_cadastro_externo = cadastroExterno;
                        clienteCadastrado.cliente_DataAverbacao = novoCliente.Cliente.DataAverbacao;
                        clienteCadastrado.cliente_StatusIntegral = novoCliente.Cliente.StatusIntegral;

                        _mysql.clientes.Update(clienteCadastrado);
                        _mysql.SaveChanges();
                    }
                }
                else
                {
                    cliente = new ClienteDb
                    {
                        cliente_cpf = cpf.PadLeft(11, '0'),
                        cliente_nome = novoCliente.Cliente.Nome ?? "",
                        cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", ""),
                        cliente_logradouro = novoCliente.Cliente.Logradouro ?? "",
                        cliente_bairro = novoCliente.Cliente.Bairro ?? "",
                        cliente_localidade = novoCliente.Cliente.Localidade ?? "",
                        cliente_uf = novoCliente.Cliente.Uf ?? "",
                        cliente_numero = novoCliente.Cliente.Numero ?? "",
                        cliente_complemento = novoCliente.Cliente.Complemento ?? "",
                        cliente_dataNasc = novoCliente.Cliente.DataNasc,
                        cliente_dataCadastro = novoCliente.Cliente.DataCad,
                        cliente_nrDocto = novoCliente.Cliente.NrDocto ?? string.Empty,
                        cliente_empregador = novoCliente.Cliente.Empregador ?? "",
                        cliente_matriculaBeneficio = novoCliente.Cliente.MatriculaBeneficio ?? "",
                        cliente_nomeMae = novoCliente.Cliente.NomeMae ?? "",
                        cliente_nomePai = novoCliente.Cliente.NomePai ?? "",
                        cliente_telefoneFixo = novoCliente.Cliente.TelefoneFixo != null ? novoCliente.Cliente.TelefoneFixo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : null,
                        cliente_telefoneCelular = novoCliente.Cliente.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                        cliente_possuiWhatsapp = novoCliente.Cliente.PossuiWhatsapp,
                        cliente_funcaoAASPA = novoCliente.Cliente.FuncaoAASPA ?? "",
                        cliente_email = novoCliente.Cliente.Email ?? "",
                        cliente_situacao = true,
                        cliente_estado_civil = novoCliente.Cliente.EstadoCivil,
                        cliente_sexo = novoCliente.Cliente.Sexo,
                        clientes_cadastro_externo = cadastroExterno,
                        cliente_DataAverbacao = novoCliente.Cliente.DataAverbacao,
                        cliente_StatusIntegral = novoCliente.Cliente.StatusIntegral
                    };

                    if (novoCliente.Cliente.DataCad != default(DateTime))
                    {
                        cliente.cliente_dataCadastro = novoCliente.Cliente.DataCad;
                    }

                    _mysql.clientes.Add(cliente);
                    _mysql.SaveChanges();

                    _mysql.log_status.Add(new LogStatusDb
                    {
                        log_status_antigo_id = 1,
                        log_status_novo_id = novoCliente.Cliente.StatusIntegral == 0 ? 1 : novoCliente.Cliente.StatusIntegral == 11 ? 1 : 4,
                        log_status_cliente_id = cliente.cliente_id,
                        log_status_dt_cadastro = DateTime.Now
                    });
                    _mysql.SaveChanges();

                    string captadorCpfCnpj = novoCliente.Captador.CpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                    var captador = _mysql.captadores.FirstOrDefault(x => x.captador_cpf_cnpj == captadorCpfCnpj);
                    if (captador != null)
                    {
                        VincularCaptadorCliente(captador, cliente);
                    }
                    else
                    {
                        captador = new CaptadorDb
                        {
                            captador_cpf_cnpj = captadorCpfCnpj,
                            captador_nome = novoCliente.Captador.Nome,
                            captador_descricao = novoCliente.Captador.Descricao ?? "",
                            captador_e_cnpj = captadorCpfCnpj.Length > 11,
                            captador_situacao = true
                        };
                        _mysql.captadores.Add(captador);
                        _mysql.SaveChanges();
                        VincularCaptadorCliente(captador, cliente);
                    }
                }

                tran.Commit();
            }
            catch (ClienteException ce)
            {
                throw new ClienteException(ce.Message);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Houve um erro ao cadastrar um novo cliente.");
            }

        }

        private void VincularCaptadorCliente(CaptadorDb captador, ClienteDb cliente)
        {
            try
            {
                _mysql.vinculo_cliente_captador.Add(new VinculoClienteCaptadorDb
                {
                    vinculo_captador_id = captador.captador_id,
                    vinculo_cliente_id = cliente.cliente_id,
                });
                _mysql.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) BuscarTodosClientes(ConsultaParametros request)
        {
            request.StatusCliente = request.StatusCliente ?? 0;
            request.QtdPorPagina = request.QtdPorPagina ?? 10;
            request.StatusRemessa = request.StatusRemessa ?? 0;
            request.StatusIntegraall = request.StatusIntegraall ?? 0;

            var (Clientes, _) = GetClientesByFiltro(request);
            request.PaginaAtual = null;
            var (_, TotalClientes) = GetClientesByFiltro(request, true);

            var copyRequest = request;
            copyRequest.PaginaAtual = null;

            int totalPaginas = (TotalClientes / request.QtdPorPagina) ?? 1;

            return (Clientes, totalPaginas, TotalClientes);
        }

        private (List<BuscarClienteByIdResponse> Clientes, int TotalClientes) GetClientesByFiltro(ConsultaParametros request, bool isCount = false, bool isDownload = false)
        {
            int pageSize = request.QtdPorPagina ?? 10;
            int currentPage = request.PaginaAtual ?? 1;
            int offset = (currentPage - 1) * pageSize;

            List<BuscarClienteByIdResponse> clientes = new();
            int qtd = 0;
            using (MySqlConnection connection = new(_mysql.Database.GetConnectionString()))
            {
                List<string> filtros = new List<string>();
                var colunas = @"
cli.cliente_id
,cli.cliente_cpf
,cli.cliente_nome
,cli.cliente_cep
,cli.cliente_logradouro
,cli.cliente_bairro
,cli.cliente_localidade
,cli.cliente_uf
,cli.cliente_numero
,cli.cliente_complemento
,cli.cliente_dataNasc
,cli.cliente_dataCadastro
,cli.cliente_nrDocto
,cli.cliente_empregador
,cli.cliente_matriculaBeneficio
,cli.cliente_nomeMae
,cli.cliente_nomePai
,cli.cliente_telefoneFixo
,cli.cliente_telefoneCelular
,cli.cliente_possuiWhatsapp
,cli.cliente_funcaoAASPA
,cli.cliente_email
,cli.cliente_situacao
,cli.cliente_estado_civil
,cli.cliente_sexo
,cli.cliente_remessa_id
,cli.clientes_cadastro_externo
,cli.cliente_StatusIntegral
,cpt.captador_id
,cpt.captador_cpf_cnpj
,cpt.captador_nome
,cpt.captador_descricao
,cpt.captador_e_cnpj
,cpt.captador_situacao
";

                string coluns = isCount ? "count(*) as QTD" : colunas;
                string query = @$"
                          SELECT {coluns}
                FROM clientes cli
                JOIN vinculo_cliente_captador vin ON cli.cliente_id = vinculo_cliente_id
                JOIN captadores cpt ON vin.vinculo_captador_id = cpt.captador_id
                LEFT JOIN historico_contatos_ocorrencia his ON cli.cliente_id = his.historico_contatos_ocorrencia_cliente_id";

                if (request.DateInit.HasValue)
                {
                    filtros.Add("cli.cliente_dataCadastro >= @DateInit");
                }

                if (request.DateEnd.HasValue)
                {
                    filtros.Add("cli.cliente_dataCadastro < @DateEnd");
                }

                if (!string.IsNullOrEmpty(request.Nome))
                {
                    filtros.Add("cli.cliente_nome LIKE CONCAT('%', @Nome, '%')");
                }

                if (!string.IsNullOrEmpty(request.Cpf))
                {
                    filtros.Add("cli.cliente_cpf LIKE CONCAT('%', @Cpf, '%')");
                }

                if (!string.IsNullOrEmpty(request.Beneficio))
                {
                    filtros.Add("cli.cliente_matriculaBeneficio LIKE CONCAT('%', @Nb, '%')");
                }

                if (request.CadastroExterno > 0)
                {
                    filtros.Add("cli.clientes_cadastro_externo = @CadastroExterno");
                }

                if (request.StatusCliente > 0)
                {
                    filtros.Add("cli.cliente_situacao = @StatusCliente");
                }

                if (request.StatusRemessa > 0)
                {
                    filtros.Add("cli.cliente_remessa_id = @StatusRemessa");
                }

                if (request.StatusIntegraall > 0)
                {
                    filtros.Add("cli.cliente_StatusIntegral = @StatusIntegraall");
                }
                if (request.SituacaoOcorrencia != null)
                {
                    filtros.Add("his.historico_contatos_ocorrencia_situacao_ocorrencia = @SituacaoOcorrencia");
                }
                if (request.DataInitAtendimento.HasValue)
                {
                    filtros.Add("his.historico_contatos_ocorrencia_dt_ocorrencia >= @DataInitAtendimento");
                }
                if (request.DataEndAtendimento.HasValue)
                {
                    filtros.Add("his.historico_contatos_ocorrencia_dt_ocorrencia <= @DataEndAtendimento");
                }

                if (filtros.Count > 0)
                {
                    query += " WHERE";
                    bool first = true;
                    foreach (var item in filtros)
                    {
                        query += first ? $" {item}" : $" AND {item}";
                        first = false;
                    }
                }

                query +=" ORDER BY cli.cliente_dataCadastro DESC";

                if (request.PaginaAtual != null)
                {
                    query += " LIMIT @PageSize OFFSET @Offset";
                }


                MySqlCommand cmd = new MySqlCommand(query, connection);

                if (request.DateInit.HasValue)
                    cmd.Parameters.AddWithValue("@DateInit", request.DateInit);
                if (request.DateEnd.HasValue)
                    cmd.Parameters.AddWithValue("@DateEnd", request.DateEnd.Value.AddDays(1));
                if (!string.IsNullOrEmpty(request.Nome))
                    cmd.Parameters.AddWithValue("@Nome", request.Nome);
                if (!string.IsNullOrEmpty(request.Cpf))
                    cmd.Parameters.AddWithValue("@Cpf", RemoveZerosEsquerda(request.Cpf.Replace(".", "").Replace("-", "")));
                if (request.CadastroExterno > 0)
                    cmd.Parameters.AddWithValue("@CadastroExterno", request.CadastroExterno);
                if (request.CadastroExterno > 0)
                    cmd.Parameters.AddWithValue("@StatusCliente", request.StatusCliente);
                if (request.StatusRemessa > 0)
                    cmd.Parameters.AddWithValue("@StatusRemessa", request.StatusRemessa);
                if (request.StatusIntegraall > 0)
                    cmd.Parameters.AddWithValue("@StatusIntegraall", request.StatusIntegraall);
                if (!string.IsNullOrEmpty(request.Beneficio))
                    cmd.Parameters.AddWithValue("@Nb", request.Beneficio.Replace(".", "").Replace("-", ""));
                if(!string.IsNullOrEmpty(request.SituacaoOcorrencia))
                    cmd.Parameters.AddWithValue("@SituacaoOcorrencia", request.SituacaoOcorrencia);
                if (request.DataInitAtendimento.HasValue)
                    cmd.Parameters.AddWithValue("@DataInitAtendimento", request.DataInitAtendimento);
                if (request.DataEndAtendimento.HasValue)
                    cmd.Parameters.AddWithValue("@DataEndAtendimento", request.DataEndAtendimento.Value.AddDays(1));
                if (request.PaginaAtual != null)
                {
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                }

                connection.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (isCount)
                        {
                            qtd = reader.GetInt32("QTD");
                        }
                        else
                        {
                            clientes.Add(new BuscarClienteByIdResponse
                            {
                                Cliente = new ClienteDb
                                {
                                    cliente_id = reader.GetInt32("cliente_id"),
                                    cliente_cpf = reader.IsDBNull("cliente_cpf") ? null : reader.GetString("cliente_cpf").PadLeft(11,'0'),
                                    cliente_nome = reader.IsDBNull("cliente_nome") ? null : reader.GetString("cliente_nome"),
                                    cliente_cep = reader.IsDBNull("cliente_cep") ? null : reader.GetString("cliente_cep"),
                                    cliente_logradouro = reader.IsDBNull("cliente_logradouro") ? null : reader.GetString("cliente_logradouro"),
                                    cliente_bairro = reader.IsDBNull("cliente_bairro") ? null : reader.GetString("cliente_bairro"),
                                    cliente_localidade = reader.IsDBNull("cliente_localidade") ? null : reader.GetString("cliente_localidade"),
                                    cliente_uf = reader.IsDBNull("cliente_uf") ? null : reader.GetString("cliente_uf"),
                                    cliente_numero = reader.IsDBNull("cliente_numero") ? null : reader.GetString("cliente_numero"),
                                    cliente_complemento = reader.IsDBNull("cliente_complemento") ? null : reader.GetString("cliente_complemento"),
                                    cliente_dataNasc = reader.GetDateTime("cliente_dataNasc"),
                                    cliente_dataCadastro = reader.GetDateTime("cliente_dataCadastro"),
                                    cliente_nrDocto = reader.IsDBNull("cliente_nrDocto") ? null : reader.GetString("cliente_nrDocto"),
                                    cliente_empregador = reader.IsDBNull("cliente_empregador") ? null : reader.GetString("cliente_empregador"),
                                    cliente_matriculaBeneficio = reader.IsDBNull("cliente_matriculaBeneficio") ? null : reader.GetString("cliente_matriculaBeneficio").PadLeft(10, '0'),
                                    cliente_nomeMae = reader.IsDBNull("cliente_nomeMae") ? null : reader.GetString("cliente_nomeMae"),
                                    cliente_nomePai = reader.IsDBNull("cliente_nomePai") ? null : reader.GetString("cliente_nomePai"),
                                    cliente_telefoneFixo = reader.IsDBNull("cliente_telefoneFixo") ? null : reader.GetString("cliente_telefoneFixo"),
                                    cliente_telefoneCelular = reader.IsDBNull("cliente_telefoneCelular") ? null : reader.GetString("cliente_telefoneCelular"),
                                    cliente_possuiWhatsapp = reader.IsDBNull("cliente_possuiWhatsapp") ? false : reader.GetBoolean("cliente_possuiWhatsapp"),
                                    cliente_funcaoAASPA = reader.IsDBNull("cliente_funcaoAASPA") ? null : reader.GetString("cliente_funcaoAASPA"),
                                    cliente_email = reader.IsDBNull("cliente_email") ? null : reader.GetString("cliente_email"),
                                    cliente_situacao = reader.IsDBNull("cliente_situacao") ? false : reader.GetBoolean("cliente_situacao"),
                                    cliente_estado_civil = reader.GetInt32("cliente_estado_civil"),
                                    cliente_sexo = reader.IsDBNull("cliente_sexo") ? (int?)null : reader.GetInt32("cliente_sexo"),
                                    cliente_remessa_id = reader.IsDBNull("cliente_remessa_id") ? (int?)null : reader.GetInt32("cliente_remessa_id"),
                                    clientes_cadastro_externo = reader.IsDBNull("clientes_cadastro_externo") ? false : reader.GetBoolean("clientes_cadastro_externo"),
                                    cliente_StatusIntegral = reader.IsDBNull("cliente_StatusIntegral") ? (int?)null : reader.GetInt32("cliente_StatusIntegral")
                                },
                                Captador = new CaptadorDb
                                {
                                    captador_id = reader.GetInt32("captador_id"),
                                    captador_cpf_cnpj = reader.IsDBNull("captador_cpf_cnpj") ? null : reader.GetString("captador_cpf_cnpj"),
                                    captador_nome = reader.IsDBNull("captador_nome") ? null : reader.GetString("captador_nome"),
                                    captador_descricao = reader.IsDBNull("captador_descricao") ? null : reader.GetString("captador_descricao"),
                                    captador_e_cnpj = reader.IsDBNull("captador_e_cnpj") ? false : reader.GetBoolean("captador_e_cnpj"),
                                    captador_situacao = reader.IsDBNull("captador_situacao") ? false : reader.GetBoolean("captador_situacao")
                                },
                                StatusAtual = BuscaStatusAtual(reader.GetInt32("cliente_id"), isDownload)
                            });
                        }
                    }
                }
            }

            if (isCount)
            {
                return (clientes, qtd);
            }
            else
            {
                return (clientes, qtd);
            }

        }

        private string RemoveZerosEsquerda(string cpf)
        {
            string numerosCpf = Regex.Replace(cpf, @"\D", "");
            numerosCpf = numerosCpf.TrimStart('0');
            return Regex.Replace(numerosCpf, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1$2$3$4");
        }
        public StatusDb BuscaStatusAtual(int clienteId, bool isDownload)
        {
            if (isDownload) return new StatusDb();

            var lgstatus = _mysql.log_status
                                 .Where(x => x.log_status_cliente_id == clienteId)
                                 .OrderByDescending(x => x.log_status_dt_cadastro);

            var ultimoStatus = lgstatus.FirstOrDefault() ?? new LogStatusDb();

            var statusAtual = _mysql.status.FirstOrDefault(x => x.status_id == ultimoStatus.log_status_novo_id);

            return statusAtual ?? new StatusDb();
        }

        public byte[] DownloadFiltro(ConsultaParametros request)
        {
            List<BuscarClienteByIdResponse> clientesData = new List<BuscarClienteByIdResponse>();

            var client = BuscarTodosClientes(request);

            clientesData.AddRange(client.Clientes);

            var ultimoCliente = new BuscarClienteByIdResponse();
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine("#;CPF;NOME;CEP;LOGRADOURO;BAIRRO;LOCALIDADE;UF;NUMERO;COMPLEMENTO;DATANASC;DATACADASTRO;NRDOCTO;EMPREGADOR;MATRICULABENEFICIO;NOMEMAE;NOMEPAI;TELEFONEFIXO;TELEFONECELULAR;POSSUIWHATSAPP;FUNCAOAASPA;EMAIL;SITUACAO;ESTADO_CIVIL;SEXO;REMESSA_ID;CAPTADOR_NOME;CAPTADOR_CPF_OU_CNPJ;CAPTADOR_DESCRICAO;DATA_AVERBACAO;STATUS_INTEGRAALL");
                for (int i = 0; i < clientesData.Count; i++)
                {
                    var cliente = clientesData[i];
                    ultimoCliente = cliente;
                    var dataAve = cliente.Cliente.cliente_DataAverbacao.HasValue
                        ? cliente.Cliente.cliente_DataAverbacao.Value.ToString("dd/MM/yyyy hh:mm:ss")
                        : "";

                    var empregador = string.IsNullOrEmpty(cliente.Cliente.cliente_empregador) ? "" : cliente.Cliente.cliente_empregador;
                    var fixo = string.IsNullOrEmpty(cliente.Cliente.cliente_telefoneFixo) ? "" : cliente.Cliente.cliente_telefoneFixo;
                    var funcaoAaspa = string.IsNullOrEmpty(cliente.Cliente.cliente_funcaoAASPA) ? "" : cliente.Cliente.cliente_funcaoAASPA;

                    builder.AppendLine($"{cliente.Cliente.cliente_id};" +
                                 $"{cliente.Cliente.cliente_cpf ?? ""};" +
                                 $"{cliente.Cliente.cliente_nome ?? ""};" +
                                 $"{cliente.Cliente.cliente_cep ?? ""};" +
                                 $"{cliente.Cliente.cliente_logradouro ?? ""};" +
                                 $"{cliente.Cliente.cliente_bairro ?? ""};" +
                                 $"{cliente.Cliente.cliente_localidade ?? ""};" +
                                 $"{cliente.Cliente.cliente_uf ?? ""};" +
                                 $"{cliente.Cliente.cliente_numero?.Replace(";", "") ?? ""};" +
                                 $"{cliente.Cliente.cliente_complemento ?? ""};" +
                                 $"{cliente.Cliente.cliente_dataNasc.ToString("dd/MM/yyyy") ?? ""};" +
                                 $"{cliente.Cliente.cliente_dataCadastro.ToString("dd/MM/yyyy") ?? ""};" +
                                 $"{cliente.Cliente.cliente_nrDocto ?? ""};" +
                                 $"{empregador};" +
                                 $"{cliente.Cliente.cliente_matriculaBeneficio ?? ""};" +
                                 $"{cliente.Cliente.cliente_nomeMae ?? ""};" +
                                 $"{cliente.Cliente.cliente_nomePai ?? ""};" +
                                 $"{fixo};" +
                                 $"{cliente.Cliente.cliente_telefoneCelular ?? ""};" +
                                 $"{cliente.Cliente.cliente_possuiWhatsapp};" +
                                 $"{funcaoAaspa};" +
                                 $"{cliente.Cliente.cliente_email ?? ""};" +
                                 $"{cliente.Cliente.cliente_situacao};" +
                                 $"{cliente.Cliente.cliente_estado_civil};" +
                                 $"{cliente.Cliente.cliente_sexo};" +
                                 $"{cliente.Cliente.cliente_remessa_id ?? 0};" +
                                 $"{cliente.Captador.captador_nome ?? ""};" +
                                 $"{cliente.Captador.captador_cpf_cnpj ?? ""};" +
                                 $"{cliente.Captador.captador_descricao ?? ""};" +
                                 $"{dataAve};" +
                                 $"{StatusDescription(cliente.Cliente.cliente_StatusIntegral ?? 0)}");
                }

                byte[] fileBytes = Encoding.Latin1.GetBytes(builder.ToString());
                return fileBytes;
            }
            catch (Exception)
            {
                var err = $"Erro no cliente: ID: {ultimoCliente.Cliente.cliente_id}, Nome: {ultimoCliente.Cliente.cliente_nome}";
                throw new Exception(err);
            }
        }
        public string StatusDescription(int StatusIntegral)
        {
            return StatusIntegral switch
            {
                11 => "Aguardando Averbação",
                12 => "Enviado para Averbação",
                13 => "Cancelado",
                14 => "Cancelado/Não averbado",
                15 => "Averbado",
                16 => "Ativo/Pago",
                _ => ""
            };
        }
        public async Task<List<ClienteRequest>> GetClientesIntegraall(string DataCadastroInicio, string DataCadastroFim)
        {
            var clientesIntegral = new List<ClienteRequest>();
            try
            {
                using (var client = new HttpClient() { Timeout = TimeSpan.FromMinutes(2) })
                {
                    string token = await GerarToken();

                    var captadores = await GetCaptador(token);
                    List<int> statusId = new List<int> { 11, 12, 15 };

                    foreach (var id in statusId)
                    {
                        string requestUri = $"https://integraall.com/api/Pessoa/ListarPessoasPorFiltro?StatusId={id}&DataCadastroInicio={DataCadastroInicio}&DataCadastroFim={DataCadastroFim}";
                        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
                        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        var response = await client.SendAsync(requestMessage);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var data = JsonConvert.DeserializeObject<List<ClienteIntegraallResponse>>(responseBody);

                            foreach (var item in data)
                            {
                                var cliente = new NovoCliente()
                                {
                                    Id = item.Id,
                                    Nome = item.NomeCliente,
                                    Cpf = item.Cpf,
                                    EstadoCivil = ConverterTextoParaInt(item.EstadoCivil),
                                    Sexo = item.Sexo.ToLower() == "masculino" ? 1 : 2,
                                    NrDocto = item.DocIdentidade,
                                    NomeMae = item.NomeMae,
                                    NomePai = item.NomePai,
                                    Email = item.EmailPessoal,
                                    TelefoneCelular = item.TelefonePessoal,
                                    TelefoneFixo = item.TelefoneCorporativo,
                                    Logradouro = item.Logradouro,
                                    Bairro = item.Bairro,
                                    Cep = item.Cep,
                                    Localidade = item.Cidade,
                                    Uf = item.Uf,
                                    Complemento = item.Complemento,
                                    Numero = item.EndNumero,
                                    DataCad = item.DataCadastro,
                                    DataNasc = item.DataNascimento,
                                    MatriculaBeneficio = item.Matricula,
                                    DataAverbacao = item.dataSolicitacaoAtivacao,
                                    StatusIntegral = item.StatusId
                                };

                                var clientes = new ClienteRequest()
                                {
                                    Cliente = cliente,
                                    Captador = ObterCaptador(item.RevendedorId, captadores)
                                };
                                clientesIntegral.Add(clientes);
                            }
                        }
                    }
                }
                return clientesIntegral;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private NovoCaptador ObterCaptador(int revendedorId, List<VinculoCaptadoRevendedor> captadores)
        {
            var vinculo = captadores.FirstOrDefault(x => x.RevendedorId == revendedorId);

            if (vinculo != null)
            {
                return new NovoCaptador()
                {
                    CpfOuCnpj = vinculo.captador.captador_cpf_cnpj,
                    Descricao = vinculo.captador.captador_descricao,
                    Nome = vinculo.captador.captador_nome
                };
            }
            else
            {
                return new NovoCaptador();
            }
        }

        public async Task<List<VinculoCaptadoRevendedor>> GetCaptador(string token)
        {
            VinculoCaptadoRevendedor revendedor = new VinculoCaptadoRevendedor();
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestUricaptador = "https://integraall.com/api/Revendedor/ListarRevendedores";
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUricaptador);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                requestMessage.Content = new StringContent("", Encoding.UTF8, "application/json");

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

                var response = await _httpClient.SendAsync(requestMessage, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<RevendedorResponse>(responseString);

                    var cap = new List<VinculoCaptadoRevendedor>();
                    foreach (var c in data.Resultado)
                    {
                        var captador = new CaptadorDb()
                        {
                            captador_cpf_cnpj = c.CpfCnpj,
                            captador_nome = c.Nome,
                        };
                        revendedor = new VinculoCaptadoRevendedor()
                        {
                            captador = captador,
                            RevendedorId = c.Id,
                        };
                        cap.Add(revendedor);
                    }
                    return cap;
                }
                else
                {
                    throw new Exception($"Erro na requisição: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter captadores: {ex.Message}");
            }
        }
        public void SalvarNovoCliente(List<ClienteRequest> clientes)
        {
            foreach (var cli in clientes)
            {
                NovoCliente(cli, true, true);
            }
        }
        public async Task<string> GerarToken()
        {
            try
            {
                var requestUriLogin = "https://hml.integraall.com/api/Login/validar";
                var loginRequest = new
                {
                    login,
                    senha,
                    captcha,
                    token
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUriLogin, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseObject.token;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar gerar token.", ex);
            }
        }

        public int ConverterTextoParaInt(string estadoCivilTexto)
        {
            switch (estadoCivilTexto.ToLower()) // Converter para minúsculas para comparar sem diferenciação de maiúsculas/minúsculas
            {
                case "solteiro":
                    return 1;
                case "casado":
                    return 2;
                case "viúvo":
                    return 3;
                case "viuvo":
                    return 3;
                case "separado judicialmente":
                    return 4;
                case "união estável":
                    return 5;
                case "uniao estavel":
                    return 5;
                case "outros":
                    return 6;
                default:
                    throw new ArgumentException("Estado civil não reconhecido.");
            }
        }

        public async Task ExcluirCliente(ClienteRequest request)
        {
            var cliente = _mysql.clientes.Where(x => x.cliente_cpf == request.Cliente.Cpf).FirstOrDefault();

            if (cliente != null)
            {
                var logs = _mysql.log_status.Where(l => l.log_status_cliente_id == cliente.cliente_id);
                if (logs.Any())
                {
                    _mysql.log_status.RemoveRange(logs);
                }

                var beneficio = _mysql.log_beneficios.Where(b => b.log_beneficios_cliente_id == cliente.cliente_id);
                if (beneficio.Any())
                {
                    _mysql.log_beneficios.RemoveRange(beneficio);
                }

                _mysql.Remove(cliente);
                _mysql.SaveChanges();                
            }
        }

        public async Task<string> InativarClienteIntegraall(ClienteRequest request, string motivocancelamento, string tokenIntegraall)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenIntegraall);
                var url = "https://hml.integraall.com/api/Proposta/CancelarPorCpfMatricula";

                var data = new
                {
                    cpf = request.Cliente.Cpf,
                    matricula = request.Cliente.MatriculaBeneficio,
                    motivoCancelamento = motivocancelamento
                };
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                Console.WriteLine(response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    ExcluirCliente(request);
                    throw new Exception("Cliente excluido com sucesso!");
                }
                else if(response.Content.ReadAsStringAsync().Result.Contains("Proposta não encontrada!"))
                {
                    throw new Exception("Proposta não encontrada!");
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
