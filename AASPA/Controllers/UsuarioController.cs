using AASPA.Domain.Interface;
using AASPA.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace AASPA.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioService;

        public UsuarioController(IUsuario usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet()]
        [Route("/LoginUsuario")]
        public ActionResult Login([FromQuery] string usuario, string senha)
        {
			try
			{
                LoginResponse res = _usuarioService.Login(usuario, senha);
                return Ok(res);
			}
			catch (System.Exception ex)
			{
                return BadRequest(ex.Message);
			}
        }
    }
}
