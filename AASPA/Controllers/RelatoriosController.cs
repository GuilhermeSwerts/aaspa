using AASPA.Domain.Interface;
using AASPA.Models.Response;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly IRelatorios _relatorios;
        private readonly IHostEnvironment _env;

        public RelatoriosController(IRelatorios relatorios, IHostEnvironment env)
        {
            _relatorios = relatorios;
            _env = env;
        }

        [HttpGet]
        [Route("/RelatorioAverbacao")]
        public ActionResult RelatorioAvebacao([FromQuery]int mes, int ano)
        {
            string directoryPath = Path.Combine(_env.ContentRootPath, "Relatorio");
            try
            {
                var response = _relatorios.GerarRelatorioAverbacao($"{ano}{mes.ToString().PadLeft(2, '0')}");
                if (!Directory.GetFiles(directoryPath).Any(file => Path.GetFileName(file).Contains($"RelAverbacao.{$"{ano}{mes.ToString().PadLeft(2, '0')}"}.xlsx")))
                {
                    _relatorios.GerarArquivoRelatorioAverbacao($"{ano}{mes.ToString().PadLeft(2, '0')}");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("/DownloadRelatorio")]
        public ActionResult DownloadRelatorio(int ano, int mes, int tiporel)
        {
            try
            {
                var response = _relatorios.BuscarArquivoRelatorio($"{ano}{mes.ToString().PadLeft(2, '0')}", tiporel);
                byte[] conteudoBytes = System.IO.File.ReadAllBytes(response.Base64);
                response.Base64 = Convert.ToBase64String(conteudoBytes);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("/RelatorioCarteiras")]
        public ActionResult RelatorioCarteiras([FromQuery] int mes, int ano)
        {
            string directoryPath = Path.Combine(_env.ContentRootPath, "Relatorio");
            try
            {
                var response = _relatorios.GerarRelatorioAverbacao($"{ano}{mes.ToString().PadLeft(2, '0')}");
                if (!Directory.GetFiles(directoryPath).Any(file => Path.GetFileName(file).Contains($"RelCarteiras.{$"{ano}{mes.ToString().PadLeft(2, '0')}"}.xlsx")))
                {
                    _relatorios.GerarArquivoRelatorioCarteiras($"{ano}{mes.ToString().PadLeft(2, '0')}");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}
