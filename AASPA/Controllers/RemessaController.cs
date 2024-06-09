using AASPA.Domain.Interface;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class RemessaController : Controller
    {
        private readonly IRemessa _remessa;
        public RemessaController(IRemessa remessa)
        {
            _remessa = remessa;
        }

        [HttpGet]
        [Route("/GerarRemessa")]
        public IActionResult GerarRemessa([FromQuery] int mes, [FromQuery] int ano)
        {
            try
            {
                if (_remessa.RemessaExiste(mes, ano))
                {
                    return BadRequest("Já existe remessa criada para o mês e ano informado!");
                }
                RetornoRemessa retorno = _remessa.GerarRemessa(mes, ano);
                return Ok(retorno.remessa_id);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao gerar a remessa. {ex.Message}");
            }
        }
        [HttpGet]
        [Route("/BuscarRemessas")]
        public ActionResult BuscarTodosClientes([FromQuery] int? mes, [FromQuery] int? ano)
        {
            try
            {
                List<BuscarTodasRemessas> remessas = _remessa.BuscarTodasRemessas(ano, mes);
                return Ok(remessas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("/DownloadRemessa/{remessaId}")]
        public ActionResult DownloadRemessa(int remessaId)
        {
            try
            {
                var response = _remessa.BuscarArquivo(remessaId);
                byte[] conteudoBytes = System.IO.File.ReadAllBytes(response.Base64);
                response.Base64 = Convert.ToBase64String(conteudoBytes);
                return Ok(response);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
