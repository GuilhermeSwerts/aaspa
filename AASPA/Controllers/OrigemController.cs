using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class OrigemController : PrivateController
    {
        private readonly IOrigem _origem;

        public OrigemController(IOrigem origem)
        {
            _origem = origem;
        }

        [HttpGet]
        [Route("/BuscarTodasOrigem")]
        public ActionResult BuscarTodasOrigem()
        {
            try
            {
                var origens = _origem.BuscarTodasOrigem();
                return Ok(origens);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarOrigemId/{origemId}")]
        public ActionResult BuscarOrigemId([FromRoute] int origemId)
        {
            try
            {
                var origem = _origem.BuscarOrigemById(origemId);
                return Ok(origem);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/NovaOrigem")]
        public ActionResult NovaOrigem([FromForm] string nomeOrigem)
        {
            try
            {
                _origem.NovaOrigem(nomeOrigem);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/EditarOrigem")]
        public ActionResult EditarOrigem([FromForm] OrigemRequest origemRequest)
        {
            try
            {
                _origem.EditarOrigem(origemRequest);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
