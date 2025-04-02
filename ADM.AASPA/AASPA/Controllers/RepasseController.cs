using AASPA.Domain.Interface;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AASPA.Controllers
{
    public class RepasseController : PrivateController
    {
        private readonly IRepasse _repasse;

        public RepasseController(IRepasse repasse)
        {
            _repasse = repasse;
        }

        [HttpGet]
        [Route("/BuscarRepasses")]
        public ActionResult BuscarRepasses([FromQuery] int? mes, [FromQuery] int? ano)
        {
            try
            {
                List<BuscarArquivosResponse> repasses = _repasse.BuscarTodosRepasses(ano, mes);
                return Ok(repasses);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
