using AASPA.Domain.Interface;
using AASPA.Repository;
using AASPA.Repository.Maps;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class LogService : ILog
    {
        private readonly MysqlContexto _mysql;

        public LogService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object GetLogAlteracao(int tabelaFk)
        {
            var resultado = (from log in _mysql.log_alteracao
                             join usu in _mysql.usuarios on log.log_usuario_fk equals usu.usuario_id
                             where log.log_id_tabela_fk == tabelaFk
                             select new
                             {
                                 Titulo = log.log_titulo,
                                 Log = log.log_log,
                                 DtCadastro = log.log_dt_cadastro,
                                 Usuario = usu.usuario_nome,
                             });

            return resultado.OrderByDescending(c=> c.DtCadastro).ToList();
        }

        public void NovaAlteracao(string titulo, string log, int usuarioId, ETipoLog tipo, int fk)
        {
            _mysql.log_alteracao.Add(new LogAlteracaoDb
            {
                log_titulo = titulo,
                log_log = log,
                log_dt_cadastro = DateTime.Now,
                log_usuario_fk = usuarioId,
                log_tipo = tipo,
                log_id_tabela_fk = fk
            });
            _mysql.SaveChanges();
        }
    }
}
