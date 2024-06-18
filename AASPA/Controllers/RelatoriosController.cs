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
    public class RelatoriosController
    {
        private readonly IRelatorios _relatorios;

        public RelatoriosController(IRelatorios relatorios)
        {
            _relatorios = relatorios;
        }

        [HttpGet]
        [Route("/RelatorioAverbacao")]
        public ActionResult RelatorioAvebacao(DateTime inicio, DateTime fim)
        {
            try
            {
                var response = _relatorios.GerarRelatorioAverbacao(inicio,fim);
                byte[] conteudoBytes = File.ReadAllBytes(response.Base64);
                response.Base64 = Convert.ToBase64String(conteudoBytes);
                return Ok(response);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
