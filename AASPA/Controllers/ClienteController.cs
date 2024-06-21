using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AASPA.Host.Controllers
{
    public class ClienteController : PrivateController
    {
        private readonly ICliente _service;

        public ClienteController(ICliente service)
        {
            _service = service;
        }

        #region [HTTPGET]

        [HttpGet]
        [Route("/BuscarClienteID/{clienteId}")]
        public ActionResult GetById([FromRoute] int clienteId)
        {
            try
            {
                BuscarClienteByIdResponse cliente = _service.BuscarClienteID(clienteId);
                return Ok(cliente);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarTodosClientes")]
        public ActionResult Get([FromQuery] int? statusCliente, int? statusRemessa)
        {
            try
            {
                List<BuscarClienteByIdResponse> clientes = _service.BuscarTodosClientes(statusCliente, statusRemessa);
                return Ok(clientes);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region [HTTPPOST]


        [HttpPost]
        [Route("/NovoCliente")]
        public ActionResult Post([FromForm] ClienteRequest request)
        {
            try
            {
                _service.NovoCliente(request);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("/EditarCliente")]
        public ActionResult EditarCliente([FromForm] ClienteRequest request)
        {
            try
            {
               _service.AtualizaCliente(request);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
