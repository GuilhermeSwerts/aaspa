using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AASPA.Controllers
{
    public class StatusController : PrivateController
    {
        private readonly IStatus _service;

        public StatusController(IStatus service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("/TodosStatus")]
        public ActionResult Get()
        {
            try
            {
                var status = _service.BuscarTodosStatus();
                return Ok(status);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/StatusId/{statusId}")]
        public ActionResult GetById([FromRoute] int statusId)
        {
            try
            {
                var status = _service.BuscarStatusById(statusId);
                return Ok(status);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        [Route("/InserirStatus")]
        public ActionResult Post([FromForm] string status_nome)
        {
            try
            {
                _service.InserirNovoStatus(status_nome);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        [Route("/EditarStatus")]
        public ActionResult Post([FromForm] StatusDb status)
        {
            try
            {
                _service.EditarStatus(status);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost()]
        [Route("/AlterarStatusCliente")]
        public ActionResult AlterarStatusCliente([FromForm] AlterarStatusClienteRequest request)
        {
            try
            {
                _service.AlterarStatusCliente(request);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [Route("/BuscarLogStatusClienteId/{clienteId}")]
        public ActionResult BuscarLogStatusClienteId([FromRoute]int clienteId, [FromQuery] DateTime? dtInicio = null, DateTime? dtFim = null)
        {
            try
            {

                if (dtFim.HasValue)
                {
                    var v = dtFim.Value.ToString().Replace(" 00:00:00", " 23:59:59");
                    dtFim = DateTime.Parse(v);
                }

                var logs = _service.BuscarLogStatusClienteId(clienteId, dtInicio, dtFim);
                return Ok(logs);    
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
