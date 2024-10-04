using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        [Route("/PreVisualizar")]
        public ActionResult GetPreVisualizar([FromQuery] ConsultaParametros request)
        {
            try
            {
                var (Clientes, QtdPaginas, TotalClientes) = _remessa.BuscarClientesElegivel(request);
                return Ok(new
                {
                    Clientes,
                    QtdPaginas,
                    TotalClientes
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


            [HttpGet]
        [Route("/GerarRemessa")]
        public IActionResult GerarRemessa([FromQuery] int mes, [FromQuery] int ano, [FromQuery] DateTime dateInit, [FromQuery] DateTime dateEnd)
        {
            try
            {
                RetornoRemessaResponse retorno = _remessa.GerarRemessa(mes, ano, dateInit, dateEnd);

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
                Response.Headers.Add("X-File-Name", response.NomeArquivo);
                return File(response.Bytes, "application/csv;charset=utf-8", response.NomeArquivo);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarRepasse")]
        public ActionResult GetBuscarRepasse([FromQuery] int? mes,int? ano)
        {
            try
            {
                var repasses = _remessa.GetBuscarRepasse(mes,ano);
                return Ok(repasses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("LerRetornoRemessa")]
        public async Task<ActionResult> LerRetornoRemessa(IFormFile file)
        {
            try
            {
                var retorno = "";
                if (file.FileName.Contains("REP"))
                {
                     retorno = await _remessa.LerRetornoRepasse(file);
                }
                else
                {
                    retorno = await _remessa.LerRetorno(file);
                }
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        [Route("BuscarRetorno")]
        public ActionResult BuscarRetorno([FromQuery] int mes, [FromQuery] int ano)
        {
            try
            {
                BuscarRetornoResponse retorno = _remessa.BuscarRetorno(mes, ano);
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
