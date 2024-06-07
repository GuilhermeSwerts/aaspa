using AASPA.Domain.Interface;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public int GerarRemessa(int mes, int ano)
        {            
            var clientes = _mysql.clientes.Where(x => x.cliente_dataCadastro.Month == mes && x.cliente_dataCadastro.Year == ano).ToList() ;

            var idRegistro = SalvarDadosRemessa(clientes, mes, ano);

            GerarArquivoRemessa(idRegistro, mes, ano);            

            return idRegistro;
        }
        private int SalvarDadosRemessa(List<ClienteDb> clientes, int mes, int ano)
        {
            using var tran = _mysql.Database.BeginTransaction();
            var remessa = new RemessaDb
            {
                remessa_mes_ano = $"{ano}-{mes.ToString().PadLeft(2, '0')}"
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
        private void GerarArquivoRemessa(int idRegistro, int mes, int ano)
        {
            var caminhoArquivoSaida = Path.Combine(string.Join(_env.ContentRootPath, "Remessa"), $"D.SUB.GER.{idRegistro}.{ano}{mes.ToString().PadLeft(2, '0')}");
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
        }
        public bool RemessaExiste(int mes, int ano)
        {
            return _mysql.remessa.Any(x => x.remessa_mes_ano == $"{mes}{ano}");
        }
    }
}
