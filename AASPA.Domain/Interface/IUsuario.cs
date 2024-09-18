using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IUsuario
    {
        void EditarUsuario(UsuarioRequest data);
        void ExcluirUsuario(int usuarioId);
        object GetAll(UsuarioDb usuarioDb);
        LoginResponse Login(string usuario, string senha);
        void ResetaSenhaUsuario(int id);
    }
}
