using AASPA.Domain.Interface;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class LogController : PrivateController
    {
        private readonly ILog _logService;

        public LogController(ILog logService)
        {
            _logService = logService;
        }

        [HttpGet]
        [Route("/Log/Alteracao")]
        public IActionResult GetLogAlteracao([FromQuery] int tabelaFk)
        {
            try
            {
                var logs = _logService.GetLogAlteracao(tabelaFk);

                return Ok(logs);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
