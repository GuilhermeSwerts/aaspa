using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository.Maps;
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
        private readonly IIntegracaoKompleto _kompleto;

        public ClienteController(ICliente service, IIntegracaoKompleto kompleto)
        {
            _service = service;
            _kompleto = kompleto;
        }

        #region [HTTPGET]

        [HttpPost]
        [Route("/Cliente/CancelarCliente")]
        public ActionResult CancelarCliente([FromBody] string cpf)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf))
                    throw new Exception("Cpf não informado");

                cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

                _service.CancelarClienteByCpf(cpf);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

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
                return Get(request);
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
                request.PaginaAtual = null;
                request.BuscarStatus = false;
                if (request.DateInitAverbacao.HasValue && !request.DateInit.HasValue)
                    request.DateInit = request.DateInitAverbacao.Value;
                if (request.DateEndAverbacao.HasValue && !request.DateEnd.HasValue)
                    request.DateEnd = request.DateEndAverbacao.Value;

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
        [HttpPost("/CancelarClienteIntegraall")]
        public async Task<ActionResult> CancelarClienteIntegraall([FromBody] AlterarStatusClientesIntegraallRequest request)
        {
            try
            {
                string tokenIntegraall = await _service.GerarToken();
                await _service.CancelarClienteIntegraall(request, tokenIntegraall);
                var retorno = await _kompleto.CancelarPropostaAsync(request);
                if (retorno.Ok) { await _service.CancelarCliente(request); }
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir cliente: {ex.Message}");
            }
        }
        #endregion
    }
}
