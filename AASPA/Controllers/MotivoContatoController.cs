using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class MotivoContatoController : PrivateController
    {
        private readonly IMotivoContato _motivoContato;

        public MotivoContatoController(IMotivoContato motivoContato)
        {
            _motivoContato = motivoContato;
        }

        [HttpGet]
        [Route("/BuscarTodosMotivos")]
        public ActionResult BuscarTodosMotivos()
        {
            try
            {
                var motivos = _motivoContato.BuscarTodosMotivos();
                return Ok(motivos);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarMotivosId/{motivoId}")]
        public ActionResult BuscarTodosMotivos([FromRoute] int motivoId)
        {
            try
            {
                var motivo = _motivoContato.BuscarMotivosById(motivoId);
                return Ok(motivo);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/NovoMotivo")]
        public ActionResult NovoMotivo([FromForm] string nomeMotivo)
        {
            try
            {
                _motivoContato.NovoMotivo(nomeMotivo);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/EditarMotivo")]
        public ActionResult EditarMotivo([FromForm] MotivoContatoRequest motivoContatoRequest)
        {
            try
            {
                _motivoContato.EditarMotivo(motivoContatoRequest);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
