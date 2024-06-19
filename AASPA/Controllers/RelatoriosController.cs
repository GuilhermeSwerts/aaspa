using AASPA.Domain.Interface;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly IRelatorios _relatorios;

        public RelatoriosController(IRelatorios relatorios)
        {
            _relatorios = relatorios;
        }

        [HttpGet]
        [Route("/RelatorioAverbacao")]
        public ActionResult RelatorioAvebacao([FromQuery]int mes, int ano)
        {
            try
            {
                var response = _relatorios.GerarRelatorioAverbacao($"{ano}{mes.ToString().PadLeft(2,'0')}");
                //byte[] conteudoBytes = System.IO.File.ReadAllBytes(response.Base64);
                //response.Base64 = Convert.ToBase64String(conteudoBytes);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
