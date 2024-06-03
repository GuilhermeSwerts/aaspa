using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class HistoricoContatoOcorrenciaController : PrivateController
    {
        private readonly IHistoricoContatoOcorrencia _historicoContato;

        public HistoricoContatoOcorrenciaController(IHistoricoContatoOcorrencia historicoContato)
        {
            this._historicoContato = historicoContato;
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
                _historicoContato.DeletarContatoOcorrencia(contatoOcorrenciaId);
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
                _historicoContato.EditarContatoOcorrencia(historicoContatos);
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
                _historicoContato.NovoContatoOcorrencia(historicoContatos);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
