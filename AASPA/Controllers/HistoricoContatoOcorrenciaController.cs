using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class HistoricoContatoOcorrenciaController : PrivateController
    {
        private readonly IHistoricoContatoOcorrencia _historicoContato;
        private readonly ICliente _cliente;

        public HistoricoContatoOcorrenciaController(IHistoricoContatoOcorrencia historicoContato, ICliente cliente)
        {
            this._historicoContato = historicoContato;
            this._cliente = cliente;
        }

        [HttpGet]
        [Route("BuscarContatoOcorrenciaById/{contatoOcorrenciaId}")]
        public ActionResult BuscarContatoOcorrenciaById([FromRoute] int contatoOcorrenciaId)
        {
            try
            {
                var historico = _historicoContato.BuscarContatoOcorrenciaById(contatoOcorrenciaId);
                return Ok(historico);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeletarContatoOcorrencia/{contatoOcorrenciaId}")]
        public ActionResult DeletarContatoOcorrencia([FromRoute] int contatoOcorrenciaId)
        {
            try
            {
                _historicoContato.DeletarContatoOcorrencia(contatoOcorrenciaId,UsuarioLogadoId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("BuscarTodosContatoOcorrencia/{clienteId}")]
        public ActionResult BuscarTodosContatoOcorrencia([FromRoute] int clienteId)
        {
            try
            {
                var historico = _historicoContato.BuscarTodosContatoOcorrencia(clienteId);
                return Ok(historico);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EditarContatoOcorrencia")]
        public ActionResult EditarContatoOcorrencia([FromForm] HistoricoContatosOcorrenciaRequest historicoContatos)
        {
            try
            {
                _historicoContato.EditarContatoOcorrencia(historicoContatos,UsuarioLogadoId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("NovoContatoOcorrencia")]
        public ActionResult NovoContatoOcorrencia([FromForm] HistoricoContatosOcorrenciaRequest historicoContatos)
        {
            try
            {
                _historicoContato.NovoContatoOcorrencia(historicoContatos, UsuarioLogadoId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/DownloadContatoFiltro")]
        public IActionResult DownloadContatoFiltro([FromQuery] ConsultaParametros request)
        {
            try
            {
                request.PaginaAtual = null;

                byte[] base64 = _historicoContato.DownloadContatoFiltro(request);
                return File(base64, "application/csv;charset=utf-8", "FiltroClientes.csv");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
