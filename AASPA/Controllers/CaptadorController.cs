using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class CaptadorController : PrivateController
    {
        private readonly ICaptador _captadorService;

        public CaptadorController(ICaptador captadorService)
        {
            _captadorService = captadorService;
        }

        [HttpPost]
        [Route("/NovoCaptador")]
        public ActionResult NovoCaptador([FromForm] NovoCaptador captador)
        {
            try
            {
                _captadorService.NovoCaptador(captador);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/EditarCaptador")]
        public ActionResult EditarCaptador([FromForm] NovoCaptador captador)
        {
            try
            {
                _captadorService.EditarCaptador(captador);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarCaptadores")]
        public ActionResult BuscarCaptadores()
        {
            try
            {
                var captadores = _captadorService.BuscarCaptadores();

                return Ok(captadores);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("/ExcluirCaptador/{captadorId}")]
        public ActionResult ExcluirCaptador([FromRoute] int captadorId)
        {
            try
            {
                _captadorService.DeletarCaptador(captadorId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
