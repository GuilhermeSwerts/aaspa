using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class LogCancelamentoService : ILogCancelamento
    {
        private readonly MysqlContexto _context;
        public LogCancelamentoService(MysqlContexto context)
        {
            _context = context;
        }
        public void Logger(AlterarStatusClientesIntegraallRequest request, string? destino, int? status, string? erro)
        {
            var criarlog = new LogCancelamentoClienteDb
            {
                id_usuario = request.usuario.Id,
                nome_usuario = request.usuario.Nome,
                data_cancelamento = DateTime.Now,
                motivo_cancelamento = request.motivocancelamento,
                tipo_cancelamento = request.cancelamento,
                id_cliente = request.clienteid,
                destino_cancelamento = destino,
                status_cancelamento = status,
                erro_cancelamento = erro
            };
            _context.log_cancelamento_cliente.Add(criarlog);
            _context.SaveChanges();
        }
    }
}
