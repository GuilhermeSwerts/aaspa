using AASPA.Domain.Interface;
using AASPA.Domain.Util;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using DocumentFormat.OpenXml.Office2010.Excel;
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

        public void EditarUsuario(UsuarioRequest data)
        {
            var usuario = _mysql.usuarios.FirstOrDefault(x => x.usuario_id == data.Id)
               ?? throw new Exception("Usuário não encontrado");

            usuario.usuario_nome = data.Nome;
            usuario.usuario_tipo = data.tipo;
            usuario.usuario_username = data.Usuario;

            _mysql.SaveChanges();
        }

        public void ExcluirUsuario(int usuarioId)
        {
            var usuario = _mysql.usuarios.FirstOrDefault(x => x.usuario_id == usuarioId)
               ?? throw new Exception("Usuário não encontrado");
            usuario.usuario_status = false;
            _mysql.SaveChanges();
        }

        public object GetAll(UsuarioDb usuarioDb)
        {
            var usuariosMaster = _mysql.usuarios.FirstOrDefault(x => x.usuario_id == usuarioDb.usuario_id && x.usuario_tipo == 1)
                ?? throw new Exception("Você não tem permissão para efetuar essa ação!");

            return _mysql.usuarios.Where(x => x.usuario_id != usuarioDb.usuario_id).ToList().Where(x=> x.usuario_status).Select(x => new
            {
                UsuarioId = x.usuario_id,
                Nome = x.usuario_nome,
                Usuario = x.usuario_username,
                TipoUsuario = x.usuario_tipo,
                DataCadastro = x.usuario_dt_cadastro
            }).ToList();
        }

        public LoginResponse Login(string usuario, string senha)
        {
            try
            {
                var usuarioDb = _mysql.usuarios.FirstOrDefault(x => x.usuario_username.ToUpper() == usuario.ToUpper() && x.usuario_status)
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

        public void NovoUsuario(UsuarioRequest data)
        {
            var usuario = _mysql.usuarios.FirstOrDefault(x => x.usuario_username.ToUpper() == data.Usuario.ToUpper());

            if (usuario != null) throw new Exception("Nome de usuário já em uso.");

            _mysql.usuarios.Add(new UsuarioDb
            {
                usuario_dt_cadastro = DateTime.Now,
                usuario_nome = data.Nome,
                usuario_senha = Cripto.Encrypt("P@drao123"),
                usuario_tipo = data.tipo,
                usuario_username = data.Usuario
            });

            _mysql.SaveChanges();
        }

        public void ResetaSenhaUsuario(int id)
        {
            var usuario = _mysql.usuarios.FirstOrDefault(x => x.usuario_id == id)
            ?? throw new Exception("Usuário não encontrado");

            usuario.usuario_senha = Cripto.Encrypt("P@drao123");

            _mysql.SaveChanges();
        }

        public void TrocaSenha(string senhaAtual, string senhaNova, UsuarioDb usuarioDb)
        {
            var usuario = _mysql.usuarios.FirstOrDefault(x => x.usuario_id == usuarioDb.usuario_id)
            ?? throw new Exception("Usuário não encontrado");

            if (Cripto.Decrypt(usuario.usuario_senha) != senhaAtual)
                throw new Exception("Senha atual incorreta.");

            usuario.usuario_senha = Cripto.Encrypt(senhaNova);
            _mysql.SaveChanges();
        }
    }
}
