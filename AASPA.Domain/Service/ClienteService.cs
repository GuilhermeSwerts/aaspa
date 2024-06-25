using AASPA.Domain.CustonException;
using AASPA.Domain.Interface;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AASPA.Domain.Service
{
    public class ClienteService : ICliente
    {
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;

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
            catch (Exception)
            {
                throw;
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

        public void NovoCliente(ClienteRequest novoCliente)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                string cpf = novoCliente.Cliente.Cpf.Replace(".", "").Replace("-", "");
                if (_mysql.clientes.Any(x => x.cliente_cpf == cpf))
                    throw new ClienteException($"Cliente com o cpf: {cpf} já cadastrado.");

                var cliente = new ClienteDb
                {
                    cliente_cpf = cpf,
                    cliente_nome = novoCliente.Cliente.Nome ?? "",
                    cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", ""),
                    cliente_logradouro = novoCliente.Cliente.Logradouro ?? "",
                    cliente_bairro = novoCliente.Cliente.Bairro ?? "",
                    cliente_localidade = novoCliente.Cliente.Localidade ?? "",
                    cliente_uf = novoCliente.Cliente.Uf ?? "",
                    cliente_numero = novoCliente.Cliente.Numero ?? "",
                    cliente_complemento = novoCliente.Cliente.Complemento ?? "",
                    cliente_dataNasc = novoCliente.Cliente.DataNasc,
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
                    cliente_sexo = novoCliente.Cliente.Sexo
                };
                _mysql.clientes.Add(cliente);
                _mysql.SaveChanges();

                _mysql.log_status.Add(new LogStatusDb
                {
                    log_status_antigo_id = 1,
                    log_status_novo_id = 1,
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
                    };
                    _mysql.captadores.Add(captador);
                    _mysql.SaveChanges();
                    VincularCaptadorCliente(captador, cliente);
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

        public (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) BuscarTodosClientes(int? statusCliente, int? statusRemessa, DateTime? dateInit, DateTime? dateEnd, int? paginaAtual)
        {
            statusCliente = statusCliente ?? 0;
            statusRemessa = statusRemessa ?? 0;

            var clientes = (from cli in _mysql.clientes
                            join vin in _mysql.vinculo_cliente_captador on cli.cliente_id equals vin.vinculo_cliente_id
                            join cpt in _mysql.captadores on vin.vinculo_captador_id equals cpt.captador_id
                            where
                                   (dateInit == null || cli.cliente_dataCadastro >= dateInit)
                                && (dateEnd == null || cli.cliente_dataCadastro < dateEnd.Value.AddDays(1))
                            select new BuscarClienteByIdResponse
                            {
                                Captador = cpt,
                                Cliente = cli,
                            }).ToList().Distinct().ToList();

            if (statusCliente == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao).ToList();
            }

            if (statusCliente == 3)
            {
                clientes = clientes.Where(x => !x.Cliente.cliente_situacao).ToList();
            }

            if (statusRemessa == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_remessa_id != null && x.Cliente.cliente_remessa_id > 0).ToList();
            }

            if (statusRemessa == 2)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_remessa_id == null || x.Cliente.cliente_remessa_id == 0).ToList();
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

            if (statusCliente == 1)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao && x.StatusAtual != null && (x.StatusAtual.status_id == (int)EStatus.Ativo || x.StatusAtual.status_id == (int)EStatus.AtivoAguardandoAverbacao)).ToList();
            }

            if (statusCliente == 2)
            {
                clientes = clientes.Where(x => x.Cliente.cliente_situacao && x.StatusAtual != null && x.StatusAtual.status_id == (int)EStatus.Inativo).ToList();
            }

            var todosClientes = clientes.ToList().Distinct().ToList().OrderBy(x => x.Cliente.cliente_dataCadastro).ToList();
            int totalClientes = todosClientes.Count();
            if (paginaAtual == null)
                return (todosClientes, 0, totalClientes);

            return CalcularPagina(todosClientes, paginaAtual, totalClientes);
        }

        private (List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) CalcularPagina(List<BuscarClienteByIdResponse> todosClientes, int? paginaAtual, int totalClientes)
        {
            int qtdPorPagina = 5;
            int pagina = paginaAtual ?? 1;

            int indiceInicial = (pagina - 1) * qtdPorPagina;

            var qtd = (todosClientes.Count + qtdPorPagina - 1) / qtdPorPagina; ;

            var qtdPaginas = Math.Ceiling(Convert.ToDecimal(qtd));

            qtdPaginas = qtdPaginas > 0 ? qtdPaginas : 1;

            return (todosClientes.Skip(indiceInicial).Take(qtdPorPagina).ToList(), Convert.ToInt32(qtdPaginas), totalClientes);
        }

        public byte[] DownloadFiltro((List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) clientesData)
        {
            string texto = "#;CPF;NOME;CEP;LOGRADOURO;BAIRRO;LOCALIDADE;UF;NUMERO;COMPLEMENTO;DATANASC;DATACADASTRO;NRDOCTO;EMPREGADOR;MATRICULABENEFICIO;NOMEMAE;NOMEPAI;TELEFONEFIXO;TELEFONECELULAR;POSSUIWHATSAPP;FUNCAOAASPA;EMAIL;SITUACAO;ESTADO_CIVIL;SEXO;REMESSA_ID\n";
            for (int i = 0; i < clientesData.Clientes.Count; i++)
            {
                var cliente = clientesData.Clientes[i];
                texto +=  $"{cliente.Cliente.cliente_id};{cliente.Cliente.cliente_cpf};{cliente.Cliente.cliente_nome};{cliente.Cliente.cliente_cep};{cliente.Cliente.cliente_logradouro};{cliente.Cliente.cliente_bairro};{cliente.Cliente.cliente_localidade};{cliente.Cliente.cliente_uf};{cliente.Cliente.cliente_numero};{cliente.Cliente.cliente_complemento};{cliente.Cliente.cliente_dataNasc};{cliente.Cliente.cliente_dataCadastro};{cliente.Cliente.cliente_nrDocto};{cliente.Cliente.cliente_empregador};{cliente.Cliente.cliente_matriculaBeneficio};{cliente.Cliente.cliente_nomeMae};{cliente.Cliente.cliente_nomePai};{cliente.Cliente.cliente_telefoneFixo};{cliente.Cliente.cliente_telefoneCelular};{cliente.Cliente.cliente_possuiWhatsapp};{cliente.Cliente.cliente_funcaoAASPA};{cliente.Cliente.cliente_email};{cliente.Cliente.cliente_situacao};{cliente.Cliente.cliente_estado_civil};{cliente.Cliente.cliente_sexo};{cliente.Cliente.cliente_remessa_id}";
                texto += "\n";
            }

            byte[] fileBytes = Encoding.Latin1.GetBytes(texto);
            return fileBytes;
        }
    }
}
