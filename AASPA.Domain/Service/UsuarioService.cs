using AASPA.Domain.Interface;
using AASPA.Domain.Util;
using AASPA.Models.Response;
using AASPA.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class UsuarioService : IUsuario
    {
        private readonly MysqlContexto _mysql;

        public UsuarioService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public LoginResponse Login(string usuario, string senha)
        {
            try
            {
                var usuarioDb = _mysql.usuarios.FirstOrDefault(x => x.usuario_username.ToUpper() == usuario.ToUpper())
                        ?? throw new Exception("Usuário não existe!");

                if (Cripto.Decrypt(usuarioDb.usuario_senha) != senha)
                    throw new Exception("Senha incorreta!");

                var token = Autenticacao.GenerateToken(usuarioDb, _mysql);

                Console.WriteLine($"Token gerado no login: {token.ToString()}");

                return new LoginResponse
                {
                    Token = token,
                    Usuario = new UsuarioResponse
                    {
                        Usuario_id = usuarioDb.usuario_id,
                        Usuario_nome = usuarioDb.usuario_nome,
                        Usuario_tipo = usuarioDb.usuario_tipo
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao fazer login: {ex.Message}");
            }
        }
    }
}
