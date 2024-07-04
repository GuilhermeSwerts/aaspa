using AASPA.Domain.Interface;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
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
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;
        private readonly IStatus _statusService;
        private readonly ICliente _clienteService;

        public RemessaService(MysqlContexto mysql, IHostEnvironment env, IStatus statusService, ICliente clienteService)
        {
            _mysql = mysql;
            _env = env;
            _statusService = statusService;
            _clienteService = clienteService;
        }

        public void VincularRemessaCliente(int clienteId, int remessaId)
        {
            var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == clienteId);
            cliente.cliente_remessa_id = remessaId;
            _mysql.SaveChanges();
        }

        public RetornoRemessaResponse GerarRemessa(int mes, int ano, DateTime dateInit, DateTime dateEnd)
        {
            try
            {
                string nomeArquivo = $"D.SUB.GER.176.{ano}{mes.ToString().PadLeft(2, '0')}";

                var clientes = RecuperarClientesAtivosExcluidos(dateInit, dateEnd);

                clientes = RecuperarClientesAtivosExcluidosLegados(dateEnd, clientes);

                var idRegistro = SalvarDadosRemessa(clientes, mes, ano, nomeArquivo, dateInit, dateEnd);

                string caminho = GerarArquivoRemessa(idRegistro, mes, ano, nomeArquivo);

                foreach (var cliente in clientes)
                {
                    var clienteData = _clienteService.BuscarClienteID(cliente.cliente_id);

                    var statusNovo = clienteData.StatusAtual.status_id == (int)EStatus.AtivoAguardandoAverbacao ?
                        (int)EStatus.Ativo : (int)EStatus.Deletado;

                    VincularRemessaCliente(cliente.cliente_id, idRegistro);
                }

                return new RetornoRemessaResponse
                {
                    caminho = caminho,
                    remessa_id = idRegistro,
                };
            }
            catch (Exception)
            {

                throw;
            }
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
                .Where(x => x.cliente_dataCadastro < dateEnd.AddDays(1)).ToList();


            foreach (var item in result)
            {
                if (!clientes.Any(x => x.cliente_id == item.cliente_id))
                {
                    clientes.Add(item);
                }
            }

            return clientes;
        }

        public int SalvarDadosRemessa(List<ClienteDb> clientes, int mes, int ano, string nomeArquivo, DateTime dateInit, DateTime dateEnd)
        {
            var RetornoVinculado = new object();
            var RemessaDeletar = RemessaExiste(mes, ano);

            if (RemessaDeletar != null)
            {
                RetornoVinculado = VerificarRetornoVinculadoRemessa(RemessaDeletar.remessa_id);
                if (RetornoVinculado != null){  throw new Exception("Remessa já vinculada a um retorno! Não será possível gerar outra remessa.");}
                InativarRemessa(RemessaDeletar);
            }

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

            foreach (var clienteDb in clientes)
            {
                _mysql.registro_remessa.Add(new RegistroRemessaDb
                {
                    registro_numero_beneficio = clienteDb.cliente_matriculaBeneficio.Length > 10 ? clienteDb.cliente_matriculaBeneficio.Substring(0, 10) : clienteDb.cliente_matriculaBeneficio.PadLeft(10, '0'),
                    registro_codigo_operacao = clienteDb.cliente_situacao ? 1 : 5,
                    registro_decimo_terceiro = 0,
                    registro_valor_percentual_desconto = 500,
                    remessa_id = idRemessa
                });
            }

            _mysql.SaveChanges();

            return idRemessa;
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

            var anoMes = remessaDb.remessa_ano_mes.Replace("-", "");

            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Remessa");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);

            path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"D.SUB.GER.176.{anoMes}"));

            if (!File.Exists(path)) throw new Exception("Arquivo não encontrado");

            return new BuscarArquivoResponse
            {
                NomeArquivo = $"D.SUB.GER.176.{anoMes}",
                Base64 = path
            };
        }
        public async Task<string> LerRetornoRepasse(IFormFile file)
        {
            RetornoFinanceiroDb retorno_financeiro = new RetornoFinanceiroDb();
            string content;
            ///var repasse = file.FileName.Substring(17, 2);
            var anomes = file.FileName.Substring(18, 6);

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
                if (remessa == null)
                {
                    throw new Exception("Não existe nenhuma remessa para o retorno financeiro importado!");
                }
                else if (retorno == null)
                {
                    throw new Exception("Não existe nenhum retorno para o retorno financeiro importado!");
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
                                    _mysql.SaveChanges();
                                }
                            }
                        }
                        tran.Commit();
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

                RetornoRemessaDb retorno;
                retorno = new RetornoRemessaDb()
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
                        status_id_novo = (int)EStatus.Inativo
                    };

                    _statusService.AlterarStatusCliente(novostatus);
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
        public BuscarRetornoResponse BuscarRetorno(int mes, int ano)
        {
            try
            {
                var anomes = (ano + mes.ToString().PadLeft(2, '0')).ToString();
                var retorno = _mysql.retornos_remessa.FirstOrDefault(x => x.AnoMes == anomes);


                if (retorno != null)
                {
                    var remessa = _mysql.remessa.FirstOrDefault(x => x.remessa_id == retorno.Remessa_Id);
                    var registroretorno = _mysql.registros_retorno_remessa.Where(x => x.Retorno_Remessa_Id == retorno.Retorno_Id).ToList();
                    List<RetornoRemessaDb> ret = new List<RetornoRemessaDb>();


                    return new BuscarRetornoResponse
                    {
                        DataImportacao = retorno.Data_Importacao,
                        IdRetorno = retorno.Retorno_Id,
                        IdRemessa = remessa.remessa_id,
                        NomeArquivoRemessa = remessa.nome_arquivo_remessa,
                        DataHoraGeracaoRemessa = remessa.remessa_data_criacao,
                        NomeArquivoRetorno = retorno.Nome_Arquivo_Retorno,
                        Retornos = registroretorno
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ClienteDb> RecuperarClientesAtivosExcluidos(DateTime dateInit, DateTime dateEnd)
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
                          where (c.cliente_remessa_id == 0 || c.cliente_remessa_id == null) && new List<int> { (int)EStatus.AtivoAguardandoAverbacao, (int)EStatus.ExcluidoAguardandoEnvio }.Contains(l.log_status_novo_id)
                          select c).ToList();

            result = result
                .Where(x => x.cliente_dataCadastro >= dateInit && x.cliente_dataCadastro < dateEnd.AddDays(1)).ToList();

            return result;
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
    }
}
