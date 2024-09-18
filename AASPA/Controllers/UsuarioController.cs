using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AASPA.Controllers
{
    public class UsuarioController : PrivateController
    {
        private readonly IUsuario _usuarioService;

        public UsuarioController(IUsuario usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPut]
        [Route("/ExcluirUsuario/{usuarioId}")]
        public ActionResult delete([FromRoute] int usuarioId)
        {
            try
            {
                _usuarioService.ExcluirUsuario(usuarioId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/TrocaSenha")]
        public ActionResult post([FromQuery] string senhaAtual,string senhaNova)
        {
            try
            {
                _usuarioService.TrocaSenha(senhaAtual, senhaNova,GetUser());
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("/Usuario")]
        public ActionResult post([FromBody] UsuarioRequest data)
        {
            try
            {
                _usuarioService.NovoUsuario(data);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/BuscarTodosUsuarios")]
        public ActionResult Get()
        {
            try
            {
                var usuarios = _usuarioService.GetAll(GetUser());
                return Ok(usuarios);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("/Usuario/AtualizarUsuario")]
        public async Task<IActionResult> Put([FromBody] UsuarioRequest data)
        {
            try
            {
                _usuarioService.EditarUsuario(data);
                return Ok(true);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("/Usuario/ResetaSenha/{id}")]
        public IActionResult ResetaSenhaUsuario([FromRoute] int id)
        {
            try
            {
                _usuarioService.ResetaSenhaUsuario(id);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
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
