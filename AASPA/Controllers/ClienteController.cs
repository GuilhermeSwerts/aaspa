using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
        public ActionResult Get([FromQuery] ConsultaParametros request)
        {
            try
            {
                var (Clientes, QtdPaginas, TotalClientes) = _service.BuscarTodosClientes(request);
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
        [Route("/BuscarFiltroClientes")]
        public ActionResult BuscarFiltroClientes([FromQuery] ConsultaParametros request)
        {
            try
            {
                var (Clientes, QtdPaginas, TotalClientes) = _service.BuscarTodosClientes(request);
                return Ok(new
                {
                    Clientes = Clientes.OrderByDescending(x => x.Cliente.cliente_dataCadastro).ToList(),
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
        public async Task<ActionResult> GetClientesIntegraall([FromQuery] string DataCadastroInicio, string DataCadastroFim)
        {
            try
            {
                var clientes = await _service.GetClientesIntegraall(DataCadastroInicio, DataCadastroFim);
                _service.SalvarNovoCliente(clientes);

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
        public IActionResult DownloadFiltro([FromQuery] ConsultaParametros request)
        {
            try
            {
                byte[] base64 = _service.DownloadFiltro(request);
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
