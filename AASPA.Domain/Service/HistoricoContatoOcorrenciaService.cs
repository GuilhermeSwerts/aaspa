using AASPA.Domain.Interface;
using AASPA.Models.Enum;
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
                contatoOcorrencia.historico_contatos_ocorrencia_banco = historicoContatos.HistoricoContatosOcorrenciaBanco;
                contatoOcorrencia.historico_contatos_ocorrencia_agencia = historicoContatos.HistoricoContatosOcorrenciaAgencia;
                contatoOcorrencia.historico_contatos_ocorrencia_conta = historicoContatos.HistoricoContatosOcorrenciaConta;

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
                    historico_contatos_ocorrencia_situacao_ocorrencia = historicoContatos.HistoricoContatosOcorrenciaSituacaoOcorrencia.ToUpper(),
                    historico_contatos_ocorrencia_banco = historicoContatos.HistoricoContatosOcorrenciaBanco,
                    historico_contatos_ocorrencia_agencia = historicoContatos.HistoricoContatosOcorrenciaAgencia,
                    historico_contatos_ocorrencia_conta = historicoContatos.HistoricoContatosOcorrenciaConta
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
        public byte[] DownloadContatoFiltro((List<BuscarClienteByIdResponse> Clientes, int QtdPaginas, int TotalClientes) clientesData)
        {
            string texto = "#;NOME;CEP;LOGRADOURO;BAIRRO;LOCALIDADE;UF;NUMERO;COMPLEMENTO;DATANASC;DATACADASTRO;NRDOCTO;EMPREGADOR;MATRICULABENEFICIO;NOMEMAE;NOMEPAI;TELEFONEFIXO;TELEFONECELULAR;POSSUIWHATSAPP;FUNCAOAASPA;EMAIL;SITUACAO;ESTADO_CIVIL;SEXO;REMESSA_ID;CAPTADOR_NOME;CAPTADOR_CPF_OU_CNPJ;CAPTADOR_DESCRICAO;DATA_AVERBACAO;STATUS_INTEGRAALL;MOTIVO_CONTATO;SITUACAO_OCORRENCIA;DATA_OCORRENCIA\n";
            for (int i = 0; i < clientesData.Clientes.Count; i++)
            {
                var cliente = clientesData.Clientes[i];
                texto += $"{cliente.Cliente.cliente_id};{cliente.Cliente.cliente_cpf};{cliente.Cliente.cliente_nome};{cliente.Cliente.cliente_cep};{cliente.Cliente.cliente_logradouro};{cliente.Cliente.cliente_bairro};{cliente.Cliente.cliente_localidade};{cliente.Cliente.cliente_uf};{cliente.Cliente.cliente_numero.Replace(";", "")};{cliente.Cliente.cliente_complemento};{cliente.Cliente.cliente_dataNasc};{cliente.Cliente.cliente_dataCadastro};{cliente.Cliente.cliente_nrDocto};{cliente.Cliente.cliente_empregador};{cliente.Cliente.cliente_matriculaBeneficio};{cliente.Cliente.cliente_nomeMae};{cliente.Cliente.cliente_nomePai};{cliente.Cliente.cliente_telefoneFixo};{cliente.Cliente.cliente_telefoneCelular};{cliente.Cliente.cliente_possuiWhatsapp};{cliente.Cliente.cliente_funcaoAASPA};{cliente.Cliente.cliente_email};{cliente.Cliente.cliente_situacao};{cliente.Cliente.cliente_estado_civil};{cliente.Cliente.cliente_sexo};{cliente.Cliente.cliente_remessa_id};{cliente.Captador.captador_nome};{cliente.Captador.captador_cpf_cnpj};{cliente.Captador.captador_descricao};{cliente.Cliente.cliente_DataAverbacao.Value.ToString("dd/MM/yyyy hh:mm:ss:")};{cliente.Cliente.cliente_StatusIntegral};{cliente.Motivo.motivo_contato_nome};{cliente.Historico.historico_contatos_ocorrencia_situacao_ocorrencia};{cliente.Historico.historico_contatos_ocorrencia_dt_ocorrencia}";
                texto += "\n";
            }

            byte[] fileBytes = Encoding.Latin1.GetBytes(texto);
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
