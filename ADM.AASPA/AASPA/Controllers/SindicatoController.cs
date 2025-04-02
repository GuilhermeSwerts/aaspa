using AASPA.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class SindicatoController : Controller
    {
        private readonly MysqlContexto _contexto;
        private IConfiguration _configuration;
        public SindicatoController(MysqlContexto contexto, IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }
        public static List<string> GerarCompetencias(string inicio, string fim)
        {
            var competencias = new List<string>();
            var dataAtual = DateTime.ParseExact(inicio, "yyyyMM", CultureInfo.InvariantCulture);
            var dataFim = DateTime.ParseExact(fim, "yyyyMM", CultureInfo.InvariantCulture);

            while (dataAtual <= dataFim)
            {
                competencias.Add(dataAtual.ToString("yyyyMM"));
                dataAtual = dataAtual.AddMonths(1);
            }
            return competencias;
        }

        public static int BuscarDiffAnos(string inicio, string fim)
        {
            var dataAtual = DateTime.ParseExact(inicio, "yyyyMM", CultureInfo.InvariantCulture);
            var dataFim = DateTime.ParseExact(fim, "yyyyMM", CultureInfo.InvariantCulture);

            int anos = dataFim.Year - dataAtual.Year;

            if (dataFim.Month < dataAtual.Month)
            {
                anos--;
            }

            return anos;
        }

        public static string AdicionarUmAno(string competencia, string competenciaFinal)
        {
            var data = DateTime.ParseExact(competencia, "yyyyMM", CultureInfo.InvariantCulture);

            if (data.ToString("yyyy") == competenciaFinal.Substring(0, 4))
            {
                return competencia;
            }


            return data.AddYears(1).ToString("yyyyMM");
        }

        [HttpGet]
        [Route("/Sindicato/Buscar/{nb}")]
        public async Task<IActionResult> BuscarSindicato([FromRoute] string nb)
        {
            var competenciaFinal = $"{DateTime.Now:yyyy}{DateTime.Now:MM}";
            var sindicatos = _contexto.Sindicatos.Where(x => x.Nb.PadLeft(10, '0') == nb.PadLeft(10, '0')).ToList();
            var competencias = GerarCompetencias("202301", competenciaFinal);

            var resultado = new List<object>();

            var anos = BuscarDiffAnos("202301", competenciaFinal);
            int count = 0;
            foreach (var competencia in competencias)
            {
                count++;

                if (count > 12) continue;

                var linha = new List<object>();
                var competenciaAtual = competencia;
                for (int i = 0; i < anos + 1; i++)
                {
                    var sindicato = sindicatos.FirstOrDefault(s => s.Nb.PadLeft(10, '0') == nb.PadLeft(10, '0') && s.Competencia == competenciaAtual);
                    var obj = new { Competencia = competenciaAtual, CdSindicato = sindicato?.CdSindicato ?? "-" };

                    linha.Add(obj);
                    competenciaAtual = AdicionarUmAno(competenciaAtual, competenciaFinal);
                }
                resultado.Add(linha);
            }
            return Ok(resultado);
        }

        [HttpGet]
        [Route("/Sindicato/Buscar/Arquivos")]
        public async Task<IActionResult> BuscarArquivos()
        {
            try
            {
                var result = _contexto.LogArquivoSindicato.ToList().OrderByDescending(x => x.DtCadastro).ToList().Select(x => new
                {
                    NomeArquivo = x.NomeArquivo,
                    DataCadastro = x.DtCadastro.ToString("dd/MM/yyyy hh:mm:ss"),
                    DataImportacao = x.DtImportacao.ToString("dd/MM/yyyy hh:mm:ss"),
                    TempoImportacao = (x.DtImportacao - x.DtCadastro).ToString(@"mm\:ss"),
                    ArquivoImportado = x.Importado ? "Sim" : "Não",
                    ArquivoComErro = x.GerouErro ? "Sim" : "Não",
                    Erro = x.Erro
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/Sindicato/Importar")]
        public async Task<IActionResult> ImportarArquivoSindicato([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            string diretorio = _configuration["PASTA_SINDICATOS"];

            string tempFilePath = Path.Combine(diretorio, file.FileName);

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var logArquivo = new Repository.Maps.LogArquivoSindicato
            {
                NomeArquivo = file.FileName,
                DtCadastro = DateTime.Now,
            };

            _contexto.LogArquivoSindicato.Add(logArquivo);

            try
            {
                await _contexto.SaveChangesAsync();

                string consoleAppPath = _configuration["CAMINHO_WORKER"].ToString();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = consoleAppPath,
                    Arguments = $"\"{tempFilePath}\" {logArquivo.Id}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();
                    System.IO.File.Delete(tempFilePath);
                    logArquivo.DtImportacao = DateTime.Now;
                    if (!string.IsNullOrEmpty(error))
                    {
                        logArquivo.Erro = error;
                        logArquivo.GerouErro = true;
                        logArquivo.Importado = false;
                        await _contexto.SaveChangesAsync();
                        return BadRequest($"Erro ao executar o console: {error}");
                    }

                    logArquivo.Erro = "";
                    logArquivo.GerouErro = false;
                    logArquivo.Importado = true;
                    await _contexto.SaveChangesAsync();

                    return Ok($"Arquivo importado e processo executado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                logArquivo.Erro = ex.Message;
                logArquivo.GerouErro = true;
                logArquivo.Importado = false;
                await _contexto.SaveChangesAsync();
                return BadRequest(ex.Message);
            }
        }
    }
}