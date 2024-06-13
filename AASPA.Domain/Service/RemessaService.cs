using AASPA.Domain.Interface;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AASPA.Domain.Service
{
    public class RemessaService : IRemessa
    {
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;

        public RemessaService(MysqlContexto mysql, IHostEnvironment env)
        {
            _mysql = mysql;
            _env = env;
        }

        public RetornoRemessaResponse GerarRemessa(int mes, int ano)
        {
            try
            {
                var clientes = _mysql.clientes.Where(x => x.cliente_dataCadastro.Month == mes && x.cliente_dataCadastro.Year == ano).ToList();

                var idRegistro = SalvarDadosRemessa(clientes, mes, ano);

                string caminho = GerarArquivoRemessa(idRegistro, mes, ano);

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
        public int SalvarDadosRemessa(List<ClienteDb> clientes, int mes, int ano)
        {
            using var tran = _mysql.Database.BeginTransaction();
            var remessa = new RemessaDb
            {
                remessa_ano_mes = $"{ano}-{mes.ToString().PadLeft(2,'0')}",
                remessa_data_criacao = DateTime.Now
            };

            _mysql.remessa.Add(remessa);
            _mysql.SaveChanges();
            int idRemessa = remessa.remessa_id;

            foreach (var clienteDb in clientes)
            {
                _mysql.registro_remessa.Add(new RegistroRemessaDb
                {
                    registro_numero_beneficio = clienteDb.cliente_matriculaBeneficio,
                    registro_codigo_operacao = clienteDb.cliente_situacao ? 1 : 5,
                    registro_decimo_terceiro = 0,
                    registro_valor_percentual_desconto = 0,
                    remessa_id = idRemessa
                });
            }

            _mysql.SaveChanges();
            tran.Commit();

            return idRemessa;
        }
        public string GerarArquivoRemessa(int idRegistro, int mes, int ano)
        {
            string nomeArquivo = $"D.SUB.GER.176.{ano}{mes.ToString().PadLeft(2, '0')}";
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
                    ValorLinha.Add($"1{cliente.registro_numero_beneficio}{cliente.registro_codigo_operacao}000{cliente.registro_decimo_terceiro}{cliente.registro_valor_percentual_desconto}".PadRight(45));
                }
                ValorLinha.Add($"9{clientes.Count.ToString().PadLeft(6, '0')}".PadRight(45));

                foreach (var linha in ValorLinha)
                {
                    writer.WriteLine(linha);
                }
            }
            return caminhoArquivoSaida;
        }
        public bool RemessaExiste(int mes, int ano)
        {
            return _mysql.remessa.Any(x => x.remessa_ano_mes == $"{ano}-{mes.ToString().PadLeft(2, '0')}");
        }
        public List<BuscarTodasRemessas> BuscarTodasRemessas(int? ano, int? mes)
        {
            var listaTodasRemessas = new List<BuscarTodasRemessas>();

            var filtro = ano.HasValue && mes.HasValue
                ? $"{ano}-{mes.ToString().PadLeft(2, '0')}"
                : ano.HasValue && !mes.HasValue
                    ? $"{ano}-"
                    : !ano.HasValue && mes.HasValue
                        ? $"-{mes.ToString().PadLeft(2, '0')}"
                        : null;

            var retornos = _mysql.remessa
                .Where(
                    x => x.remessa_id > 0 && 
                    (string.IsNullOrEmpty(filtro) || x.remessa_ano_mes.Contains(filtro))
                ).ToList();

            foreach (var buscar in retornos)
            {
                int mesDaRemessa = int.Parse(buscar.remessa_ano_mes.Split('-')[1]);
                int anoDaRemessa = int.Parse(buscar.remessa_ano_mes.Split('-')[0]);
                var buscarTodasRemessas = new BuscarTodasRemessas()
                {
                    RemessaId = buscar.remessa_id,
                    Mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mesDaRemessa).ToUpper(),
                    Ano = anoDaRemessa,
                    DataCriacao = buscar.remessa_data_criacao.ToString()
                };

                listaTodasRemessas.Add(buscarTodasRemessas);
            }
            return listaTodasRemessas;
        }
        public BuscarArquivoResponse BuscarArquivo(int remessaId)
        {
            var remessaDb = _mysql.remessa.FirstOrDefault(x => x.remessa_id == remessaId)
                ?? throw new Exception($"Remessa não encontrada. id: {remessaId}");

            var anoMes = remessaDb.remessa_ano_mes.Replace("-","");

            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Remessa");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);

            path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"D.SUB.GER.176.{anoMes}"));

            if (!File.Exists(path)) throw new Exception("Arquivo não encontrado");

            return new BuscarArquivoResponse
            {
                NomeArquivo= $"D.SUB.GER.176.{anoMes}",
                Base64 = path
            };
        }
        public async Task<string> LerRetorno(IFormFile file)
        {
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

                var remessa_id = _mysql.remessa.FirstOrDefault(x => x.remessa_ano_mes == file.FileName.Substring(15, 6)).remessa_id;
                var retorno = new RetornoRemessaDb()
                {
                    DataImportacao = DateTime.Now,
                    AnoMes = file.FileName,
                    RemessaId = remessa_id
                };
                _mysql.retornosremessa.Add(retorno);
                _mysql.SaveChanges();

                var idRetorno = retorno.RetornoId;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        string content = await reader.ReadToEndAsync();

                        var linhas = content.Split('\n');                    

                        foreach (var line in linhas)
                        {
                            if (int.Parse(line.Substring(0, 1)) == 0)
                            {
                                if(line.Substring(1, 10).Trim() != "AASPA")
                                {
                                    throw new Exception("Arquivo não pertence a AASPA");
                                }
                            }
                            else if (int.Parse(line.Substring(0, 1)) == 1)
                            {
                                DateTime date;
                                var registroretorno = new RegistroRetornoRemessaDb()
                                {
                                    NumeroBeneficio = int.Parse(line.Substring(1, 10)),
                                    CodigoOperacao = int.Parse(line.Substring(11,1)),
                                    CodigoResultado = int.Parse(line.Substring(12,1)),
                                    MotivoRejeicao = int.Parse(line.Substring(13,3)),
                                    ValorDesconto = int.Parse(line.Substring(16,5)),
                                    DataInicioDesconto = DateTime.Parse(line.Substring(21,8)).Date,
                                    CodigoEspecieBeneficio = int.Parse(line.Substring(29, 2)),
                                    RetornoRemessaId = idRetorno
                                };
                                _mysql.registrosretornoremessa.Add(registroretorno);
                                _mysql.SaveChanges();
                            }
                        }
                        tran.Commit();
                        return content;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
