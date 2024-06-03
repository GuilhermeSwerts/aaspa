using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AASPA.Controllers
{
    public class BeneficioController : PrivateController
    {
        private readonly IBeneficio _service;

        public BeneficioController(IBeneficio service)
        {
            _service = service;
        }

        [HttpGet()]
        [Route("/BuscarLogBeneficios/{clienteId}")]
        public ActionResult BuscarLogBeneficiosClienteId([FromRoute] int clienteId, [FromQuery] DateTime? dtInicio = null, DateTime? dtFim = null)
        {
            try
            {
                var logs = _service.BuscarLogBeneficiosClienteId(clienteId, dtInicio, dtFim);
                return Ok(logs);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/VincularBeneficios/{clienteId}")]
        public ActionResult VincularBeneficios([FromRoute] int clienteId, [FromForm] List<int> beneficiosIds)
        {
            try
            {
                _service.VincularBeneficios(clienteId, beneficiosIds);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarTodosBeneficios")]
        public ActionResult GetAll()
        {
            try
            {
                var beneficios = _service.BuscarTodosBeneficios();
                return Ok(beneficios);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/NovoBeneficio")]
        public ActionResult NovoBeneficio([FromForm] BeneficioRequest request)
        {
            try
            {
                _service.NovoBeneficio(request);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarBeneficioId/{beneficioId}")]
        public ActionResult GetById(int beneficioId)
        {
            try
            {
                var beneficio = _service.BuscarBeneficioId(beneficioId);
                return Ok(beneficio);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/EditarBeneficio")]
        public ActionResult EditarBeneficio([FromForm] BeneficioRequest request)
        {
            try
            {
                _service.EditarBeneficio(request);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
