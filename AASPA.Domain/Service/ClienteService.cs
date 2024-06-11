using AASPA.Domain.Interface;
using AASPA.Models.Requests;
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
    public class ClienteService : ICliente
    {
        private readonly MysqlContexto _mysql;
        public ClienteService(MysqlContexto mysql)
        {
            _mysql = mysql;
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

                cliente.cliente_cpf = cpf;
                cliente.cliente_nome = novoCliente.Cliente.Nome;
                cliente.cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", "");
                cliente.cliente_logradouro = novoCliente.Cliente.Logradouro;
                cliente.cliente_bairro = novoCliente.Cliente.Bairro;
                cliente.cliente_localidade = novoCliente.Cliente.Localidade;
                cliente.cliente_uf = novoCliente.Cliente.Uf;
                cliente.cliente_numero = novoCliente.Cliente.Numero;
                cliente.cliente_complemento = novoCliente.Cliente.Complemento;
                cliente.cliente_dataNasc = novoCliente.Cliente.DataNasc;
                cliente.cliente_nrDocto = novoCliente.Cliente.NrDocto;
                cliente.cliente_empregador = novoCliente.Cliente.Empregador;
                cliente.cliente_matriculaBeneficio = novoCliente.Cliente.MatriculaBeneficio;
                cliente.cliente_nomeMae = novoCliente.Cliente.NomeMae;
                cliente.cliente_nomePai = novoCliente.Cliente.NomePai;
                cliente.cliente_telefoneFixo = novoCliente.Cliente.TelefoneFixo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                cliente.cliente_telefoneCelular = novoCliente.Cliente.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                cliente.cliente_possuiWhatsapp = novoCliente.Cliente.PossuiWhatsapp;
                cliente.cliente_funcaoAASPA = novoCliente.Cliente.FuncaoAASPA;
                cliente.cliente_email = novoCliente.Cliente.Email;

                _mysql.SaveChanges();

                string captadorCpfCnpj = novoCliente.Captador.CpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                var vinculo = _mysql.vinculo_cliente_captador.FirstOrDefault(x => x.vinculo_cliente_id == novoCliente.Cliente.Id);
                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_id == vinculo.vinculo_captador_id);
                captador.captador_cpf_cnpj = captadorCpfCnpj;
                captador.captador_nome = novoCliente.Captador.Nome;
                captador.captador_descricao = novoCliente.Captador.Descricao;
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
                    throw new Exception($"Cliente com o cpf: {cpf} já cadastrado.");

                var cliente = new Repository.Maps.ClienteDb
                {
                    cliente_cpf = cpf,
                    cliente_nome = novoCliente.Cliente.Nome,
                    cliente_cep = novoCliente.Cliente.Cep.Replace(".", "").Replace("-", ""),
                    cliente_logradouro = novoCliente.Cliente.Logradouro,
                    cliente_bairro = novoCliente.Cliente.Bairro,
                    cliente_localidade = novoCliente.Cliente.Localidade,
                    cliente_uf = novoCliente.Cliente.Uf,
                    cliente_numero = novoCliente.Cliente.Numero,
                    cliente_complemento = novoCliente.Cliente.Complemento,
                    cliente_dataNasc = novoCliente.Cliente.DataNasc,
                    cliente_nrDocto = novoCliente.Cliente.NrDocto,
                    cliente_empregador = novoCliente.Cliente.Empregador,
                    cliente_matriculaBeneficio = novoCliente.Cliente.MatriculaBeneficio,
                    cliente_nomeMae = novoCliente.Cliente.NomeMae,
                    cliente_nomePai = novoCliente.Cliente.NomePai,
                    cliente_telefoneFixo = novoCliente.Cliente.TelefoneFixo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                    cliente_telefoneCelular = novoCliente.Cliente.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                    cliente_possuiWhatsapp = novoCliente.Cliente.PossuiWhatsapp,
                    cliente_funcaoAASPA = novoCliente.Cliente.FuncaoAASPA,
                    cliente_email = novoCliente.Cliente.Email
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
                        captador_descricao = novoCliente.Captador.Descricao,
                        captador_e_cnpj = captadorCpfCnpj.Length > 11,
                    };
                    _mysql.captadores.Add(captador);
                    _mysql.SaveChanges();
                    VincularCaptadorCliente(captador, cliente);
                }

                tran.Commit();

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

        public List<BuscarClienteByIdResponse> BuscarTodosClientes()
        {
            var clientes = (from cli in _mysql.clientes
                            join vin in _mysql.vinculo_cliente_captador on cli.cliente_id equals vin.vinculo_cliente_id
                            join cpt in _mysql.captadores on vin.vinculo_captador_id equals cpt.captador_id
                            where cli.cliente_situacao
                            select new BuscarClienteByIdResponse
                            {
                                Captador = cpt,
                                Cliente = cli,
                            }).ToList().Distinct().ToList();

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

            return clientes.ToList().Distinct().ToList();
        }
    }
}
