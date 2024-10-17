using AASPA.Domain.Interface;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using AASPA.Repository.Response;
using Dapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AASPA.Domain.Service
{
    public class RemessaService : IRemessa
    {
        private readonly IHistoricoContatoOcorrencia _historicoContato;
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;
        private readonly IStatus _statusService;
        private readonly ICliente _clienteService;

        public RemessaService(MysqlContexto mysql, IHostEnvironment env, IStatus statusService, ICliente clienteService, IHistoricoContatoOcorrencia historicoContato)
        {
            _mysql = mysql;
            _env = env;
            _statusService = statusService;
            _clienteService = clienteService;
            _historicoContato = historicoContato;
        }

        public void VincularRemessaCliente(int clienteId, int remessaId)
        {
            var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == clienteId);
            cliente.cliente_remessa_id = remessaId;
            _mysql.SaveChanges();
        }

        public void AtualizarRemessaCliente(List<int> clienteIds, int remessaId)
        {
            using (var connection = new MySqlConnection(_mysql.Database.GetConnectionString()))
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("UPDATE clientes SET cliente_remessa_id = @RemessaId WHERE cliente_id IN @Ids");

                var parameters = new DynamicParameters();
                parameters.Add("@RemessaId", remessaId);
                parameters.Add("@Ids", clienteIds);

                connection.Execute(sqlBuilder.ToString(), parameters);
            }
        }

        public RetornoRemessaResponse GerarRemessa(int mes, int ano, DateTime dateInit, DateTime dateEnd)
        {
            try
            {
                var RetornoVinculado = new object();
                var Remessaexist = RemessaExiste(mes, ano);

                if (Remessaexist != null)
                {
                    RetornoVinculado = VerificarRetornoVinculadoRemessa(Remessaexist.remessa_id);
                    if (RetornoVinculado != null) { throw new Exception($"Remessa já vinculada a um retorno! Não será possível gerar outra remessa com a competência {ano}{mes.ToString().PadLeft(2, '0')}."); }
                    InativarRemessa(Remessaexist);
                    RemoverVinculoClientesRemessa(Remessaexist);
                }

                string nomeArquivo = $"D.SUB.GER.176.{ano}{mes.ToString().PadLeft(2, '0')}";

                var clientes = RecuperarClientesAtivosExcluidos(dateInit, dateEnd)
                    ?? throw new Exception("Não existe nenhum cliente para ser gerado remessa no período informado!");

                var idRegistro = SalvarDadosRemessa(clientes, mes, ano, nomeArquivo, dateInit, dateEnd);

                AtualizaStatusClienteGeradoRemessa(clientes);

                return new RetornoRemessaResponse
                {
                    remessa_id = idRegistro,
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void AtualizaStatusClienteGeradoRemessa(List<ClientesAtivosExcluidosResponse> clientes)
        {
            var logStatus = clientes.Select(x => new LogStatusDb
            {
                log_status_antigo_id = x.LogStatusNovoId,
                log_status_cliente_id = x.ClienteId,
                log_status_novo_id = x.LogStatusNovoId == (int)EStatus.AtivoAguardandoAverbacao ? (int)EStatus.Ativo : (int)EStatus.Deletado,
                log_status_dt_cadastro = DateTime.Now
            }).ToList();

            int batchSize = 10000;
            int maxBatchSize = clientes.Count;
            var registros = new List<LogStatusDb>();

            foreach (var log in logStatus)
            {
                registros.Add(log);

                if (registros.Count >= batchSize || registros.Count == clientes.Count || maxBatchSize == registros.Count)
                {
                    using (var connection = new MySqlConnection(_mysql.Database.GetConnectionString()))
                    {
                        var sqlBuilder = new StringBuilder();
                        sqlBuilder.Append("INSERT INTO log_status (log_status_antigo_id, log_status_cliente_id, log_status_novo_id, log_status_dt_cadastro) VALUES ");

                        var parameters = new DynamicParameters();
                        int counter = 0;

                        foreach (var registro in logStatus)
                        {
                            sqlBuilder.Append($"(@AntigoStatus{counter}, @ClienteId{counter}, @NovoStatus{counter}, now()),");

                            parameters.Add($"@AntigoStatus{counter}", registro.log_status_antigo_id);
                            parameters.Add($"@ClienteId{counter}", registro.log_status_cliente_id);
                            parameters.Add($"@NovoStatus{counter}", registro.log_status_novo_id);

                            counter++;
                        }

                        sqlBuilder.Length--;
                        sqlBuilder.Append(";");

                        connection.Execute(sqlBuilder.ToString(), parameters);
                    }
                    maxBatchSize -= batchSize;
                    registros.Clear();
                }
            }
        }

        private byte[] GetByteRemessa(int idRegistro)
        {
            var linhaArquivo = new StringBuilder();
            linhaArquivo.AppendLine("0AASPA            11                        ".PadRight(45));
            var clientes = _mysql.registro_remessa.Where(x => x.remessa_id == idRegistro).ToList();
            foreach (var cliente in clientes)
            {
                linhaArquivo.AppendLine($"1{cliente.registro_numero_beneficio}{cliente.registro_codigo_operacao}000{cliente.registro_decimo_terceiro}{cliente.registro_valor_percentual_desconto.ToString().PadLeft(5, '0')}".PadRight(45));
            }
            linhaArquivo.AppendLine($"9{clientes.Count.ToString().PadLeft(6, '0')}".PadRight(45));

            return Encoding.Latin1.GetBytes(linhaArquivo.ToString());
        }

        private void RemoverVinculoClientesRemessa(RemessaDb remessaexist)
        {
            var clientes = _mysql.clientes.Where(x => x.cliente_remessa_id == remessaexist.remessa_id).ToList();

            foreach (var cliente in clientes)
            {
                cliente.cliente_remessa_id = 0;

                _mysql.clientes.Update(cliente);
            }
            _mysql.SaveChanges();
        }

        private List<ClienteDb> RecuperarClientesAtivosExcluidosLegados(DateTime dateEnd, List<ClienteDb> clientes)
        {
            var result = (from c in _mysql.clientes
                          join l in
                              (from l1 in _mysql.log_status
                               join l2 in
                                   (from ls in _mysql.log_status
                                    group ls by ls.log_status_cliente_id into g
                                    select new
                                    {
                                        log_status_cliente_id = g.Key,
                                        max_date = g.Max(x => x.log_status_dt_cadastro)
                                    })
                               on new { l1.log_status_cliente_id, l1.log_status_dt_cadastro }
                               equals new { l2.log_status_cliente_id, log_status_dt_cadastro = l2.max_date }
                               select l1)
                          on c.cliente_id equals l.log_status_cliente_id
                          where c.cliente_remessa_id == 0 && new List<int> { (int)EStatus.AtivoAguardandoAverbacao, (int)EStatus.ExcluidoAguardandoEnvio }.Contains(l.log_status_novo_id)
                          select c).ToList();

            result = result
                .Where(x => x.cliente_DataAverbacao < dateEnd.AddDays(1)).ToList();


            foreach (var item in result)
            {
                if (!clientes.Any(x => x.cliente_id == item.cliente_id))
                {
                    clientes.Add(item);
                }
            }

            return clientes;
        }

        public int SalvarDadosRemessa(List<ClientesAtivosExcluidosResponse> clientes, int mes, int ano, string nomeArquivo, DateTime dateInit, DateTime dateEnd)
        {
            var remessa = new RemessaDb
            {
                remessa_ano_mes = $"{ano}{mes.ToString().PadLeft(2, '0')}",
                remessa_data_criacao = DateTime.Now,
                nome_arquivo_remessa = nomeArquivo,
                remessa_periodo_de = dateInit,
                remessa_periodo_ate = dateEnd,
                remessa_status = true
            };

            _mysql.remessa.Add(remessa);
            _mysql.SaveChanges();
            int idRemessa = remessa.remessa_id;

            int batchSize = 10000;
            int maxBatchSize = clientes.Count;
            var registros = new List<RegistroRemessaDb>();

            foreach (var clienteDb in clientes)
            {
                registros.Add(new RegistroRemessaDb
                {
                    registro_numero_beneficio = clienteDb.ClienteMatriculaBeneficio.Length > 10 ? clienteDb.ClienteMatriculaBeneficio.Substring(0, 10) : clienteDb.ClienteMatriculaBeneficio.PadLeft(10, '0'),
                    registro_codigo_operacao = clienteDb.ClienteSituacao ? 1 : 5,
                    registro_decimo_terceiro = 0,
                    registro_valor_percentual_desconto = 500,
                    remessa_id = idRemessa
                });

                if (registros.Count >= batchSize || registros.Count == clientes.Count || maxBatchSize == registros.Count)
                {
                    InserirDaosRemessa(registros);
                    AtualizarRemessaCliente(clientes.Select(x => x.ClienteId).ToList(), idRemessa);
                    maxBatchSize -= batchSize;
                    registros.Clear();
                }
            }

            return idRemessa;
        }

        private void InserirDaosRemessa(List<RegistroRemessaDb> registros)
        {
            using (var connection = new MySqlConnection(_mysql.Database.GetConnectionString()))
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("INSERT INTO registro_remessa (registro_numero_beneficio, registro_codigo_operacao, registro_decimo_terceiro, registro_valor_percentual_desconto, remessa_id) VALUES ");

                var parameters = new DynamicParameters();
                int counter = 0;

                foreach (var registro in registros)
                {
                    sqlBuilder.Append($"(@NumeroBeneficio{counter}, @CodigoOperacao{counter}, @DecimoTerceiro{counter}, @ValorDesconto{counter}, @RemessaId{counter}),");

                    parameters.Add($"@NumeroBeneficio{counter}", registro.registro_numero_beneficio);
                    parameters.Add($"@CodigoOperacao{counter}", registro.registro_codigo_operacao);
                    parameters.Add($"@DecimoTerceiro{counter}", registro.registro_decimo_terceiro);
                    parameters.Add($"@ValorDesconto{counter}", registro.registro_valor_percentual_desconto);
                    parameters.Add($"@RemessaId{counter}", registro.remessa_id);

                    counter++;
                }

                sqlBuilder.Length--;
                sqlBuilder.Append(";");

                connection.Execute(sqlBuilder.ToString(), parameters);
            }
        }

        private object VerificarRetornoVinculadoRemessa(int remessa_id)
        {
            return _mysql.retornos_remessa.FirstOrDefault(x => x.Remessa_Id == remessa_id);
        }

        private void InativarRemessa(RemessaDb remessa)
        {
            var atualizarremessa = _mysql.remessa.FirstOrDefault(x => x.remessa_id == remessa.remessa_id);
            atualizarremessa.remessa_status = false;
            _mysql.remessa.Update(atualizarremessa);
            _mysql.SaveChanges();

        }

        public string GerarArquivoRemessa(int idRegistro, int mes, int ano, string nomeArquivo)
        {
            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Remessa", nomeArquivo);
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            List<string> ValorLinha = new List<string>();
            using (StreamWriter writer = new StreamWriter(caminhoArquivoSaida))
            {
                ValorLinha.Add("0AASPA            11                        ".PadRight(45));

                var clientes = _mysql.registro_remessa.Where(x => x.remessa_id == idRegistro).ToList();

                foreach (var cliente in clientes)
                {
                    ValorLinha.Add($"1{cliente.registro_numero_beneficio}{cliente.registro_codigo_operacao}000{cliente.registro_decimo_terceiro}{cliente.registro_valor_percentual_desconto.ToString().PadLeft(5, '0')}".PadRight(45));
                }
                ValorLinha.Add($"9{clientes.Count.ToString().PadLeft(6, '0')}".PadRight(45));

                foreach (var linha in ValorLinha)
                {
                    writer.WriteLine(linha);
                }
            }
            return caminhoArquivoSaida;
        }
        public RemessaDb RemessaExiste(int mes, int ano)
        {
            RemessaDb remessa = _mysql.remessa.FirstOrDefault(x => x.remessa_ano_mes == $"{ano}{mes.ToString().PadLeft(2, '0')}" && x.remessa_status == true);
            return remessa == null ? null : remessa;
        }
        public List<BuscarTodasRemessas> BuscarTodasRemessas(int? ano, int? mes)
        {
            var listaTodasRemessas = new List<BuscarTodasRemessas>();

            var filtro = ano.HasValue && mes.HasValue
                ? $"{ano}{mes.ToString().PadLeft(2, '0')}"
                : ano.HasValue && !mes.HasValue
                    ? $"{ano}"
                    : !ano.HasValue && mes.HasValue
                        ? $"{mes.ToString().PadLeft(2, '0')}"
                        : null;

            var retornos = _mysql.remessa
                .Where(
                    x => x.remessa_id > 0 &&
                    (string.IsNullOrEmpty(filtro) || x.remessa_ano_mes.Contains(filtro))
                ).ToList();

            foreach (var buscar in retornos)
            {
                int mesDaRemessa = int.Parse(buscar.remessa_ano_mes.Substring(4, 2));
                int anoDaRemessa = int.Parse(buscar.remessa_ano_mes.Substring(0, 4));
                var buscarTodasRemessas = new BuscarTodasRemessas()
                {
                    RemessaId = buscar.remessa_id,
                    Mes = CultureInfo.CreateSpecificCulture("pt-BR").DateTimeFormat.GetMonthName(mesDaRemessa).ToUpper(),
                    Ano = anoDaRemessa,
                    DataCriacao = buscar.remessa_data_criacao.ToString(),
                    Periodo = "De " + buscar.remessa_periodo_de.ToString("dd/MM/yyyy") + " até " + buscar.remessa_periodo_ate.ToString("dd/MM/yyyy"),
                    remessa_status = buscar.remessa_status,
                };

                listaTodasRemessas.Add(buscarTodasRemessas);
            }
            return listaTodasRemessas;
        }

        public BuscarArquivoResponse BuscarArquivo(int remessaId)
        {
            var remessaDb = _mysql.remessa.FirstOrDefault(x => x.remessa_id == remessaId)
                ?? throw new Exception($"Remessa não encontrada. id: {remessaId}");

            return new BuscarArquivoResponse
            {
                NomeArquivo = remessaDb.nome_arquivo_remessa,
                Bytes = GetByteRemessa(remessaDb.remessa_id)
            };
        }

        public async Task<string> LerRetornoRepasse(IFormFile file, int usuarioLogadoId)
        {
            RetornoFinanceiroDb retorno_financeiro = new RetornoFinanceiroDb();
            string content;
            ///var repasse = file.FileName.Substring(17, 2);
            var anomes = file.FileName.Substring(18, 6);

            var ano = file.FileName.Substring(18, 4);
            var mes = file.FileName.Substring(22, 2);
            mes = (int.Parse(mes) - 1).ToString().PadLeft(2, '0');

            anomes = $"{ano}{mes}";

            if (file == null || file.Length == 0)
            {
                throw new Exception("Nenhum arquivo foi enviado.");
            }
            else if (!file.FileName.Contains($"D.SUB.GER.177."))
            {
                throw new Exception("Arquivo com nome fora do formato!");
            }
            try
            {

                var remessa = _mysql.remessa.FirstOrDefault(x => x.remessa_ano_mes == anomes);
                var retorno = _mysql.retornos_remessa.FirstOrDefault(x => x.AnoMes == anomes);
                var repasse = _mysql.retorno_financeiro.FirstOrDefault(x => x.ano_mes == anomes);
                if (remessa == null)
                {
                    throw new Exception("Não existe nenhuma remessa para o retorno financeiro importado!");
                }
                else if (retorno == null)
                {
                    throw new Exception("Não existe nenhum retorno para o retorno financeiro importado!");
                }
                if (repasse != null)
                {
                    throw new Exception("Já existe um arquivo de repasse financeiro importado para o mês/ano competente!");
                }

                using var tran = _mysql.Database.BeginTransaction();
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        content = await reader.ReadToEndAsync();

                        var linhas = content.Split('\n');
                        List<RegistroRetornoFinanceiroDb> processados = new();
                        foreach (var line in linhas)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                if (int.Parse(line.Substring(0, 1)) == 0)
                                {
                                    if (line.Substring(16, 10).Trim() != "AASPA")
                                    {
                                        throw new Exception("Arquivo não pertence a AASPA");
                                    }

                                    retorno_financeiro = new RetornoFinanceiroDb()
                                    {
                                        repasse = line.Substring(1, 7),
                                        competencia_Repasse = int.Parse(line.Substring(9, 6)),
                                        ano_mes = anomes,
                                        data_importacao = DateTime.Now,
                                        nome_arquivo = file.FileName,
                                        remessa_id = remessa.remessa_id,
                                        retorno_id = retorno.Retorno_Id,
                                    };
                                    _mysql.retorno_financeiro.Add(retorno_financeiro);
                                    _mysql.SaveChanges();
                                }
                                else if (int.Parse(line.Substring(0, 1)) == 1)
                                {
                                    DateTime date;
                                    var nb = line.Substring(1, 10);
                                    var cd = int.Parse(line.Substring(11, 6));
                                    var ep = int.Parse(line.Substring(17, 2));
                                    var uf = line.Substring(19, 2);
                                    var dc = decimal.Parse(line.Substring(21, 5));
                                    var rId = retorno_financeiro.retorno_financeiro_id;

                                    var registro_Financeiro = new RegistroRetornoFinanceiroDb()
                                    {
                                        numero_beneficio = nb,
                                        competencia_desconto = cd,
                                        especie = ep,
                                        uf = uf,
                                        desconto = dc,
                                        retorno_financeiro_id = rId,
                                    };

                                    _mysql.registro_retorno_financeiro.Add(registro_Financeiro);
                                    processados.Add(registro_Financeiro);
                                    _mysql.SaveChanges();
                                }
                            }
                        }
                        tran.Commit();

                        AdicionarHistoricoPagamento(processados, usuarioLogadoId);


                        return anomes;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return "";
        }

        private void AdicionarHistoricoPagamento(List<RegistroRetornoFinanceiroDb> processados, int usuarioLogadoId)
        {
            try
            {
                foreach (var repasse in processados)
                {
                    var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_matriculaBeneficio.PadLeft(10, '0') == repasse.numero_beneficio.PadLeft(10, '0'));
                    if (cliente != null)
                    {
                        var desconto = repasse.desconto.HasValue? repasse.desconto.Value.ToString("C", new System.Globalization.CultureInfo("pt-BR")) : "R$ 00,00";

                        _historicoContato.NovoContatoOcorrencia(new HistoricoContatosOcorrenciaRequest
                        {
                            HistoricoContatosOcorrenciaOrigemId = (int)EOrigem.ARQUIVO_REPASSE_FINANCEIRO,
                            HistoricoContatosOcorrenciaDtOcorrencia = DateTime.Now,
                            HistoricoContatosOcorrenciaClienteId = cliente.cliente_id,
                            HistoricoContatosOcorrenciaMotivoContatoId = (int)EMotivo.ARQUIVO_INSS,
                            HistoricoContatosOcorrenciaSituacaoOcorrencia = "EM PROCESSAMENTO",

                            HistoricoContatosOcorrenciaDescricao = $"Desconto do valor de {desconto}",
                            HistoricoContatosOcorrenciaAgencia = "",
                            HistoricoContatosOcorrenciaPix = "",
                            HistoricoContatosOcorrenciaBanco = "",
                            HistoricoContatosOcorrenciaConta = "",
                            HistoricoContatosOcorrenciaDigito = "",
                            HistoricoContatosOcorrenciaTelefone = "",
                            HistoricoContatosOcorrenciaTipoConta = "",
                            HistoricoContatosOcorrenciaTipoChavePix = "",
                            HistoricoContatosOcorrenciaAnexos = null,
                            HistoricoContatosOcorrenciaId = 0
                        }, usuarioLogadoId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> LerRetorno(IFormFile file)
        {
            string content;
            List<string> clientesparainativar = new List<string>();
            List<string> ClienteparaAtivar = new List<string>();
            List<string> ClienteparaExcluir = new List<string>();
            var repasse = file.FileName.Substring(14, 3);
            var anomes = file.FileName.Substring(14, 6);
            if (file == null || file.Length == 0)
            {
                throw new Exception("Nenhum arquivo foi enviado.");
            }
            else if (!file.FileName.Contains($"D.SUB.GER.177."))
            {
                throw new Exception("Arquivo com nome fora do formato!");
            }

            try
            {
                using var tran = _mysql.Database.BeginTransaction();

                var remessa = _mysql.remessa.FirstOrDefault(x => x.remessa_ano_mes == anomes && x.remessa_status == true);
                if (remessa == null)
                {
                    throw new Exception("Não existe nenhuma remessa para o retorno importado!");
                }
                var retornoDb = _mysql.retornos_remessa.FirstAsync(x => x.AnoMes == anomes);

                if (retornoDb != null)
                {
                    throw new Exception("Já existe um arquivo de retorno importado para o mês/ano competente!");
                }

                RetornoRemessaDb retorno = new()
                {
                    Data_Importacao = DateTime.Now,
                    AnoMes = file.FileName.Substring(14, 6),
                    Nome_Arquivo_Retorno = file.FileName,
                    Remessa_Id = remessa.remessa_id
                };

                _mysql.retornos_remessa.Add(retorno);
                _mysql.SaveChanges();

                var idRetorno = retorno.Retorno_Id;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        content = await reader.ReadToEndAsync();

                        var linhas = content.Split('\n');

                        foreach (var line in linhas)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                if (int.Parse(line.Substring(0, 1)) == 0)
                                {
                                    if (line.Substring(1, 10).Trim() != "AASPA")
                                    {
                                        throw new Exception("Arquivo não pertence a AASPA");
                                    }
                                }
                                else if (int.Parse(line.Substring(0, 1)) == 1)
                                {
                                    if (int.Parse(line.Substring(12, 1)) == 2)
                                    {
                                        clientesparainativar.Add(line);
                                    }
                                    else if (int.Parse(line.Substring(11, 1)) != (int)EStatus.ExcluidoAguardandoEnvio && int.Parse(line.Substring(12, 1)) == 1)
                                    {
                                        ClienteparaAtivar.Add(line);
                                    }
                                    else if (int.Parse(line.Substring(11, 1)) == (int)EStatus.ExcluidoAguardandoEnvio && int.Parse(line.Substring(12, 1)) == 1)
                                    {
                                        ClienteparaExcluir.Add(line);
                                    }
                                    DateTime date;
                                    var registroretorno = new RegistroRetornoRemessaDb()
                                    {
                                        Numero_Beneficio = line.Substring(1, 10),
                                        Codigo_Operacao = int.Parse(line.Substring(11, 1)),
                                        Codigo_Resultado = int.Parse(line.Substring(12, 1)),
                                        Motivo_Rejeicao = int.Parse(line.Substring(13, 3)),
                                        Valor_Desconto = decimal.Parse(line.Substring(16, 5)),
                                        Data_Inicio_Desconto = line.Substring(21, 8) == "00000000" ? DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture), "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.ParseExact(line.Substring(21, 8), "yyyyMMdd", CultureInfo.InvariantCulture),
                                        Codigo_Especie_Beneficio = int.Parse(line.Substring(29, 2)),
                                        Retorno_Remessa_Id = idRetorno
                                    };
                                    _mysql.registros_retorno_remessa.Add(registroretorno);
                                    _mysql.SaveChanges();
                                }
                            }
                        }
                        tran.Commit();
                        InativarClienteRejeitado(clientesparainativar);
                        AtivarClienteRemessaEnviada(ClienteparaAtivar);
                        ExcluirClientesRemessa(ClienteparaExcluir);
                        return anomes;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ExcluirClientesRemessa(List<string> clienteparaExcluir)
        {
            foreach (string line in clienteparaExcluir)
            {

                var matricula = line.Substring(1, 10);

                var query = (from c in _mysql.clientes
                             join l in _mysql.log_status on c.cliente_id equals l.log_status_cliente_id
                             select new { c, l }).AsEnumerable()
                             .Where(ti => ti.c.cliente_matriculaBeneficio == matricula)
                             .Select(ti => ti.l)
                             .FirstOrDefault();

                if (query != null)
                {
                    AlterarStatusClienteRequest novostatus = new AlterarStatusClienteRequest()
                    {
                        cliente_id = query.log_status_cliente_id,
                        status_id_antigo = query.log_status_novo_id,
                        status_id_novo = (int)EStatus.Deletado
                    };

                    _statusService.AlterarStatusCliente(novostatus);
                }
            }
        }

        private void AtivarClienteRemessaEnviada(List<string> lines)
        {
            foreach (string line in lines)
            {
                var matricula = line.Substring(1, 10);

                var query = (from c in _mysql.clientes
                             join l in _mysql.log_status on c.cliente_id equals l.log_status_cliente_id
                             select new { c, l }).AsEnumerable()
                             .Where(ti => ti.c.cliente_matriculaBeneficio == matricula)
                             .Select(ti => ti.l)
                             .FirstOrDefault();

                if (query != null)
                {
                    AlterarStatusClienteRequest novostatus = new AlterarStatusClienteRequest()
                    {
                        cliente_id = query.log_status_cliente_id,
                        status_id_antigo = query.log_status_novo_id,
                        status_id_novo = (int)EStatus.Ativo
                    };

                    _statusService.AlterarStatusCliente(novostatus);
                }
            }
        }

        private void InativarClienteRejeitado(List<string> lines)
        {

            foreach (string line in lines)
            {
                var matricula = line.Substring(1, 10);

                var query = (from c in _mysql.clientes
                             join l in _mysql.log_status on c.cliente_id equals l.log_status_cliente_id
                             select new { c, l }).AsEnumerable()
                             .Where(ti => ti.c.cliente_matriculaBeneficio == matricula)
                             .Select(ti => ti.l)
                             .FirstOrDefault();

                if (query != null)
                {
                    AlterarStatusClienteRequest novostatus = new AlterarStatusClienteRequest()
                    {
                        cliente_id = query.log_status_cliente_id,
                        status_id_antigo = query.log_status_novo_id,
                        status_id_novo = (int)EStatus.Inativo,
                    };

                    _statusService.AlterarStatusCliente(novostatus);

                    var cliente = _mysql.clientes.Where(x => x.cliente_matriculaBeneficio == matricula).FirstOrDefault();
                    cliente.cliente_StatusIntegral = 14;
                    _mysql.SaveChanges();
                }
            }
        }
        bool IsValidClienteMatricula(string line, string clienteMatriculaBeneficio)
        {
            if (string.IsNullOrEmpty(line) || line.Length < 11)
            {
                return false;
            }

            try
            {
                string substring = line.Substring(1, 10);
                int parsedValue = int.Parse(substring);
                return clienteMatriculaBeneficio == parsedValue.ToString();
            }
            catch
            {
                return false;
            }
        }
        public List<BuscarArquivosResponse> BuscarRetorno(int mes, int ano)
        {
            try
            {
                try
                {
                    var res = (from ret in _mysql.retornos_remessa
                               join rem in _mysql.remessa on ret.Remessa_Id equals rem.remessa_id
                               select new BuscarArquivosResponse
                               {
                                   DataImportacao = ret.Data_Importacao.Value.ToString("dd/MM/yyyy hh:mm:ss"),
                                   NomeRemessaCompetente = rem.nome_arquivo_remessa,
                                   NomeRetornoCompetente = ret.Nome_Arquivo_Retorno,
                                   RetornoId = ret.Retorno_Id
                               });

                    return res.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ClientesAtivosExcluidosResponse> RecuperarClientesAtivosExcluidos(DateTime dateInit, DateTime dateEnd)
        {
            using (var connection = new MySqlConnection(_mysql.Database.GetConnectionString()))
            {
                string sqlQuery = @"select 
	                     cli.cliente_id as ClienteId ,
	                     max(ls.log_status_dt_cadastro) as LogStatusDtCadastro ,
	                     cli.cliente_cpf as ClienteCpf,
	                     cli.cliente_situacao as ClienteSituacao,
	                     cli.cliente_matriculaBeneficio as ClienteMatriculaBeneficio,
                         ls.log_status_novo_id as LogStatusNovoId
                    from clientes cli
                    join log_status ls on cli.cliente_id = ls.log_status_cliente_id
                where 
	                (cli.cliente_remessa_id is null or cli.cliente_remessa_id = 0) and 
	                (ls.log_status_novo_id = 1 or ls.log_status_novo_id = 5) and 
	                (@DtInit is null or cli.cliente_dataAverbacao >= @DtInit) and
	                (@DtEnd is null or cli.cliente_dataAverbacao < @DtEnd)
                group by cli.cliente_id,ls.log_status_novo_id";

                var clientes = connection.Query<ClientesAtivosExcluidosResponse>(sqlQuery, new { DtInit = dateInit, DtEnd = dateEnd.AddDays(1) }).ToList();

                if (clientes.Count == 0) return null;

                return clientes;
            }
        }

        public object GetBuscarRepasse(int? mes, int? ano)
        {
            var listaTodasRemessas = new List<BuscarTodasRemessas>();

            var filtro = ano.HasValue && mes.HasValue
                ? $"{ano}{mes.ToString().PadLeft(2, '0')}"
                : ano.HasValue && !mes.HasValue
                    ? $"{ano}"
                    : !ano.HasValue && mes.HasValue
                        ? $"{mes.ToString().PadLeft(2, '0')}"
                        : null;

            var remessa = _mysql.remessa
                .Where(
                    x => x.remessa_id > 0 &&
                    (string.IsNullOrEmpty(filtro) || x.remessa_ano_mes.Contains(filtro) && x.remessa_status == true)
                ).FirstOrDefault();

            if (remessa == null)
            {
                return new
                {
                    remessa = new RemessaDb { },
                    retorno = new RetornoRemessaDb { },
                    repasses = new RetornoFinanceiroDb { },
                    dadosRepasse = new List<RegistroRetornoFinanceiroDb>()
                };
            }

            var retorno = _mysql.retornos_remessa
                .Where(
                    x => x.Retorno_Id > 0 &&
                    (string.IsNullOrEmpty(filtro) || x.Remessa_Id == remessa.remessa_id)
                ).FirstOrDefault();

            if (retorno == null)
            {
                return new
                {
                    remessa,
                    retorno = new RetornoRemessaDb { },
                    repasses = new RetornoFinanceiroDb { },
                    dadosRepasse = new List<RegistroRetornoFinanceiroDb>()
                };
            }

            var repasses = _mysql.retorno_financeiro
                .Where(
                    x => x.remessa_id > 0 &&
                    (string.IsNullOrEmpty(filtro) || x.retorno_id == retorno.Retorno_Id)
                ).FirstOrDefault();

            if (repasses == null)
            {
                return new
                {
                    remessa,
                    retorno,
                    repasses = new RetornoFinanceiroDb { },
                    dadosRepasse = new List<RegistroRetornoFinanceiroDb>()
                };
            }

            var dadosRepasse = _mysql.registro_retorno_financeiro
                .Where(x => x.retorno_financeiro_id == repasses.retorno_financeiro_id)
                .ToList();

            return new
            {
                remessa,
                retorno,
                repasses,
                dadosRepasse
            };
        }

        public (List<ClienteDb> Clientes, int QtdPaginas, int TotalClientes) BuscarClientesElegivel(ConsultaParametros request)
        {
            request.QtdPorPagina ??= 10;
            request.StatusRemessa ??= 0;
            request.StatusIntegraall ??= 0;

            var (Clientes, _) = GetClientesByFiltro(request);
            var (_, TotalClientes) = GetClientesByFiltro(request, true);

            var copyRequest = request;
            copyRequest.PaginaAtual = null;

            int totalPaginas = (TotalClientes / request.QtdPorPagina) ?? 1;

            return (Clientes, totalPaginas, TotalClientes);
        }

        private (List<ClienteDb> Clientes, int TotalClientes) GetClientesByFiltro(ConsultaParametros request, bool isCount = false, bool isDownload = false)
        {
            int pageSize = request.QtdPorPagina ?? 10;
            int currentPage = request.PaginaAtual ?? 1;
            int offset = (currentPage - 1) * pageSize;

            using (MySqlConnection connection = new(_mysql.Database.GetConnectionString()))
            {
                string colunas = isCount ? "count(*) as Qtd" : "cli.*";
                string query = $@"SELECT {colunas}
                    from clientes cli
                    join log_status ls on cli.cliente_id = ls.log_status_cliente_id
                where 
	                (cli.cliente_remessa_id is null or cli.cliente_remessa_id = 0) and 
	                (ls.log_status_novo_id = 1 or ls.log_status_novo_id = 5)";

                if (request.DateInit.HasValue)
                    query += (" AND (@DtInit is null or cli.cliente_dataAverbacao >= @DtInit)");
                if (request.DateEnd.HasValue)
                    query += (" AND (@DtEnd is null or cli.cliente_dataAverbacao < @DtEnd)");

                query += " group by cli.cliente_id";

                if (request.PaginaAtual != null && !isCount)
                {
                    query += $" LIMIT {pageSize} OFFSET {offset}";
                }

                var param = new
                {
                    DtInit = request.DateInit ?? (DateTime?)null,
                    DateEnd = request.DateEnd ?? (DateTime?)null
                };

                if (isCount)
                {
                    var clientes = connection.Query<QuantidadeResponse>(query, param).ToList();
                    var soma = clientes.Sum(x => x.Qtd);

                    return (new List<ClienteDb>(), soma);
                }
                else
                {
                    var clientes = connection.Query<ClienteDb>(query, param).ToList();
                    return (clientes, clientes.Count);
                }

            }

        }
    }
}
