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
        [Route("/RelatorioRepasse")]
        public ActionResult RelatorioRepasse([FromQuery] int mes, int ano, int captadorId)
        {
            try
            {
                var response = _relatorios.GerarRelatorioRepasse($"{ano}{mes.ToString().PadLeft(2, '0')}", captadorId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet]
        [Route("/RelatorioRetorno")]  
        public ActionResult RelatorioRetorno([FromQuery] int mes, int ano, int captadorId)
        {
            try
            {
                var response = _relatorios.GerarRelatorioRetorno($"{ano}{mes.ToString().PadLeft(2, '0')}", captadorId);

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
        public ActionResult RelatorioCarteiras([FromQuery] int mes, int ano, int captadorId)
        {
            try
            {
                var response = _relatorios.GerarRelatorioRetorno($"{ano}{mes.ToString().PadLeft(2, '0')}", captadorId);
                _relatorios.GerarArquivoRelatorioCarteiras($"{ano}{mes.ToString().PadLeft(2, '0')}", captadorId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
