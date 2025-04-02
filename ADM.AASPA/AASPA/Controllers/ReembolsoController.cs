using AASPA.Domain.Interface;
using AASPA.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AASPA.Controllers
{
    public class ReembolsoController : Controller
    {
        private readonly IReembolso _service;

        public ReembolsoController(IReembolso service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("SolicitacaoReembolso")]
        public IActionResult Get([FromQuery] DateTime? dtInicio, DateTime? dtFim)
        {
            try
            {
                var data = _service.Get(dtInicio, dtFim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SolicitacaoReembolso/{idSolicitacao}")]
        public IActionResult Post([FromRoute] int idSolicitacao)
        {
            try
            {
                _service.InformaPagamento(idSolicitacao);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("DownloadSolicitacaoReembolso")]
        public IActionResult Post([FromQuery] DateTime? dtInicio, DateTime? dtFim)
        {
            try
            {
                var bytes = _service.DownloadRelatorio(dtInicio, dtFim);
                var date = DateTime.Now;
                return File(bytes, "application/csv;charset=utf-8", $"relatorio_solicitacao_reembolso_{date.Day}{date.Month}{date.Year}{date.Hour}{date.Minute}{date.Second}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
