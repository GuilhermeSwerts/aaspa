using AASPA.Repository.Maps;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace AASPA.Controllers
{
    [Authorize]
    public class PrivateController : PrivateController
    {
        public UsuarioDb Usuario => GetUser();

        public int UsuarioLogadoId => GetUsuarioLogadoId();

        [HttpGet]
        public UsuarioDb GetUser()
        {
            if (User != null)
            {
                if (User.Claims.Count() == 0) { return null; }

                UsuarioDb usuario = new();

                var userdata = User.Claims.First(x => x.Type.Contains("userdata"));

                //var role = User.Claims.First(x => x.Type.Contains("role"));

                if (userdata != null)
                {
                    usuario.usuario_id = int.Parse(userdata.Value);
                    //usuario.usuario_tipo = int.Parse(role.Value);
                    return usuario;
                }
            }
            return null;
        }

        [HttpGet]
        public int GetUsuarioLogadoId()
        {
            var usuario = GetUser();
            
            return usuario != null ? usuario.usuario_id : 0;
        }
    }
}
