using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AASPA.Controllers
{
    public class PagamentoController : PrivateController
    {
        private readonly IPagamento _pagamento;

        public PagamentoController(IPagamento pagamento)
        {
            _pagamento = pagamento;
        }

        [HttpGet]
        [Route("/BuscarHistoricoPagamento/{clienteId}")]
        public ActionResult BuscarHistoricoPagamento([FromRoute] int clienteId)
        {
            try
            {
                var historico = _pagamento.BuscarHistoricoPagamentoClienteId(clienteId);
                return Ok(historico.OrderBy(x=> x.DtPagamento).ToList());
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarPagamentoId/{pagamentoId}")]
        public ActionResult BuscarPagamentoId([FromRoute] int pagamentoId)
        {
            try
            {
                var pagamento = _pagamento.BuscarPagamentoById(pagamentoId);
                return Ok(pagamento);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/NovoPagamento")]
        public ActionResult NovoPagamento([FromForm] NovoPagamentoRequest request)
        {
            try
            {
                _pagamento.NovoPagamento(new Repository.Maps.PagamentoDb
                {
                    pagamento_cliente_id = request.ClienteId,
                    pagamento_dt_cadastro = System.DateTime.Now,
                    pagamento_dt_pagamento = request.DataPagamento,
                    pagamento_valor_pago = decimal.Parse(request.ValorPago.Replace(".", ",").Replace("R$", "").Replace(" ", ""))
                });
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/EditarPagamento")]
        public ActionResult EditarPagamento([FromForm] NovoPagamentoRequest request)
        {
            try
            {
                _pagamento.EditarPagamento(new Repository.Maps.PagamentoDb
                {
                    pagamento_id = request.PagamentoId,
                    pagamento_cliente_id = request.ClienteId,
                    pagamento_dt_cadastro = System.DateTime.Now,
                    pagamento_dt_pagamento = request.DataPagamento,
                    pagamento_valor_pago = decimal.Parse(request.ValorPago.Replace(".", ",").Replace("R$", "").Replace(" ", ""))
                });
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("/ExcluirPagamento/{pagamentoId}")]
        public ActionResult ExcluirPagamento([FromRoute] int pagamentoId)
        {
            try
            {
                _pagamento.ExcluirPagamento(pagamentoId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
