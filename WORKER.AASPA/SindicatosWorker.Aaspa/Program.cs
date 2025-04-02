using SindicatosWorker.Aaspa.Repository.DataBase;
using SindicatosWorker.Aaspa.Repository.Mdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SindicatosWorker.Aaspa
{
    internal class Program
    {
        public class SindicatosModel
        {
            public string Nb { get; set; }
            public string Data { get; set; }
            public string CsSindicato { get; set; }
        }

        static async Task Main(string[] args)
        {
            var tran = await DataBase.BeginTransactionAsync();
            string file = args[0] ?? throw new Exception("Caminho do arquivo não especificado");
            string id = args[1] ?? throw new Exception("Id do log do arquivo não especificado"); ;
            try
            {
                var context = new MdbContext(file);
                var fileInfo = new FileInfo(file);
                var tableName = fileInfo.Name.Replace(Path.GetExtension(fileInfo.Name), "");
                var competencia = tableName.Split('_')[1];
                tableName = context.Tables().Where(x => x.Contains("sindicatos")).First();
                var script = $"SELECT nb, [dt-compet-averb] AS Data, [cs-sindicato-averbacao] AS CsSindicato FROM {tableName}";
                var result = context.Query<SindicatosModel>(script);

                int maxSize = 30000;
                int size = (result.Count / maxSize) + 1;


                for (int i = 0; i < size; i++)
                {
                    var pageData = result.Skip(i * maxSize).Take(maxSize).ToList();

                    StringBuilder sqlBuilder = new StringBuilder();
                    foreach (var data in pageData)
                    {
                        sqlBuilder.Append($"('{data.Nb}','{data.CsSindicato}','{data.Data}',{id}),");
                    }

                    sqlBuilder.Length--;
                    sqlBuilder.Append(";");

                    var values = sqlBuilder.ToString();

                    string insert = $@"INSERT IGNORE INTO sindicatos
                    (sindicato_nb, sindicato_cs_sindicato,sindicato_competencia,log_arquivo_sindicato_id)
                    VALUES {values}";

                    await DataBase.ExecuteAsync(insert, tran);
                }

                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}
