using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        public async Task<ActionResult> Get([FromQuery] int? statusCliente, int? statusRemessa, DateTime? dateInit, DateTime? dateEnd, int? paginaAtual)
        {
            try
            {
                 var clientes = await _service.GetClientesIntegraall("2024/06/01");
                _service.SalvarNovoCliente(clientes);
                var (Clientes, QtdPaginas, TotalClientes) = _service.BuscarTodosClientes(statusCliente, statusRemessa, dateInit, dateEnd, paginaAtual);
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
        [Route("/GetClientesIntegraall")]
        public ActionResult GetClientesIntegraall([FromQuery] string DataCadastroInicio)
        {
            try
            {
                _service.GetClientesIntegraall(DataCadastroInicio);

                return Ok();
            }
            catch (Exception ex)
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
                _service.NovoCliente(request, false);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/DownloadClienteFiltro")]
        public IActionResult DownloadFiltro([FromQuery] int? statusCliente, int? statusRemessa, DateTime? dateInit, DateTime? dateEnd)
        {
            try
            {
                var clientes = _service.BuscarTodosClientes(statusCliente, statusRemessa, dateInit, dateEnd,null);
                byte[] base64 = _service.DownloadFiltro(clientes);
                return File(base64, "application/csv;charset=utf-8", "FiltroClientes.csv");
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
