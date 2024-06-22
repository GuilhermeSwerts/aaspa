using AASPA.Domain.Interface;
using AASPA.Models.Enum;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class StatusService : IStatus
    {
        private readonly MysqlContexto _mysql;

        public StatusService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public List<BuscarLogStatusClienteIdResponse> BuscarLogStatusClienteId(int clienteId, DateTime? dtInicio = null, DateTime? dtFim = null)
        {
            var logs = _mysql.log_status
     .Where(x => x.log_status_id > 0
         && (dtInicio == null || x.log_status_dt_cadastro >= dtInicio)
         && (dtFim == null || x.log_status_dt_cadastro <= dtFim)
     ).ToList();

            List<BuscarLogStatusClienteIdResponse> response = new();
            foreach (var log in logs)
            {
                var de = _mysql.status.First(x => x.status_id == log.log_status_antigo_id);
                var para = _mysql.status.First(x => x.status_id == log.log_status_novo_id);

                response.Add(new BuscarLogStatusClienteIdResponse
                {
                    Data = log.log_status_dt_cadastro.ToString("dd/MM/yyyy HH:mm:ss"),
                    De = de.status_nome,
                    Para = log.log_status_antigo_id == log.log_status_novo_id ? "-" :para.status_nome
                });
            }

            return response;
        }

        public void AlterarStatusCliente(AlterarStatusClienteRequest request)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                if(request.status_id_novo == (int)EStatus.Deletado || request.status_id_novo == (int)EStatus.ExcluidoAguardandoEnvio)
                {
                    var cliente = _mysql.clientes.FirstOrDefault(x=> x.cliente_id == request.cliente_id);
                    cliente.cliente_situacao = false;
                    _mysql.SaveChanges();
                }

                _mysql.log_status.Add(new LogStatusDb
                {
                    log_status_antigo_id = request.status_id_antigo,
                    log_status_dt_cadastro = DateTime.Now,
                    log_status_cliente_id = request.cliente_id,
                    log_status_novo_id = request.status_id_novo
                });

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public object BuscarStatusById(int statusId)
        {
            return _mysql.status.FirstOrDefault(x => x.status_id == statusId)
                 ?? throw new Exception("Status do id: {statusId} não encontrado.");
        }

        public List<StatusDb> BuscarTodosStatus()
        {
            return _mysql.status.Where(x => x.status_id > 0).ToList();
        }

        public void EditarStatus(StatusDb status)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var statusDb = _mysql.status.FirstOrDefault(x => x.status_id == status.status_id)
                ?? throw new Exception("Status do id: {statusId} não encontrado.");

                statusDb.status_id = status.status_id;
                statusDb.status_nome = status.status_nome;

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void InserirNovoStatus(string status_nome)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                _mysql.status.Add(new StatusDb
                {
                    status_nome = status_nome,
                });
                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
