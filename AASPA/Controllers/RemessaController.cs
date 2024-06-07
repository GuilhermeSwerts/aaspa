using AASPA.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class RemessaController : Controller
    {
        private readonly IRemessa _remessa;
        public RemessaController(IRemessa remessa)
        {
            _remessa = remessa;
        }

        [HttpGet]
        [Route("/GerarRemessa")]
        public IActionResult GerarRemessa([FromQuery] int mes, [FromQuery] int ano)
        {
            try
            {
                if (_remessa.RemessaExiste(mes, ano))
                {
                    return BadRequest("Já existe remessa criada para o mês e ano informado!");
                }
                int idRegistro = _remessa.GerarRemessa(mes, ano);
                return Ok(idRegistro);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao gerar a remessa. {ex.Message}");
            }
        }
    }
}
