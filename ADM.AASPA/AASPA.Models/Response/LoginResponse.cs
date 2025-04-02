using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class LoginResponse
    {
        public UsuarioResponse Usuario { get; set; }
        public string Token { get; set; }
    }

    public class UsuarioResponse
    {
        public int Usuario_id { get; set; }
        public int Usuario_tipo { get; set; }
        public string Usuario_nome { get; set; }
    }

}
