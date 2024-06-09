using AASPA.Domain.Interface;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
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

        public RetornoRemessa GerarRemessa(int mes, int ano)
        {            
            var clientes = _mysql.clientes.Where(x => x.cliente_dataCadastro.Month == mes && x.cliente_dataCadastro.Year == ano).ToList() ;

            var idRegistro = SalvarDadosRemessa(clientes, mes, ano);

            string caminho = GerarArquivoRemessa(idRegistro, mes, ano);

            return new RetornoRemessa 
            {
                caminho = caminho,
                remessa_id = idRegistro,
            };
        }
        public int SalvarDadosRemessa(List<ClienteDb> clientes, int mes, int ano)
        {
            using var tran = _mysql.Database.BeginTransaction();
            var remessa = new RemessaDb
            {
                remessa_mes_ano = $"{ano}-{mes.ToString().PadLeft(2, '0')}",
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
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))){ Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            List<string> ValorLinha = new List<string>();
            using (StreamWriter writer = new StreamWriter(caminhoArquivoSaida))
            {
                ValorLinha.Add("0AASPA            11                        ".PadRight(45));

                var clientes = _mysql.registro_remessa.Where(x => x.remessa_id == idRegistro).ToList();

                foreach(var cliente in clientes)
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
            return _mysql.remessa.Any(x => x.remessa_mes_ano == $"{ano}{mes}");
        }
        public List<BuscarTodasRemessas> BuscarTodasRemessas()
        {
            var listaTodasRemessas = new List<BuscarTodasRemessas>();

            var retornos = _mysql.remessa.ToList();

            foreach(var buscar in retornos)
            {
                int mes = int.Parse(buscar.remessa_mes_ano.Substring(5, 2));
                var buscarTodasRemessas = new BuscarTodasRemessas()
                {
                    remessa_id = buscar.remessa_id,
                    mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes).ToUpper(),
                    ano = int.Parse(buscar.remessa_mes_ano.Substring(0,4), CultureInfo.CurrentCulture.DateTimeFormat),
                    Data_Criacao = buscar.remessa_data_criacao
                };

                listaTodasRemessas.Add(buscarTodasRemessas);
            }
            return listaTodasRemessas;
        }
        public string BuscarArquivo(string anoMes)
        {
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Remessa");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);

            path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"D.SUB.GER.176.{anoMes}"));

            if (!File.Exists(path)) throw new Exception("Arquivo não encontrado");

            return path;
        }
    }
}
