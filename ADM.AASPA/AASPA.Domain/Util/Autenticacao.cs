using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Microsoft.IdentityModel.Tokens;

namespace AASPA.Domain.Util
{
    public class Autenticacao
    {
        public static class Settings
        {
            public static string Secret = "kljsdkkdlo4454GG00155sajuklmbkdl";
        }

        public static string GenerateToken(UsuarioDb usuario, MysqlContexto _mySQLContext)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            List<Claim> claimsIdentity = new()
            {
                    new Claim(ClaimTypes.Name, usuario.usuario_nome),
                    new Claim(ClaimTypes.UserData,usuario.usuario_id.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
