using AASPA.Domain.Interface;
using AASPA.Models.Enums;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using Newtonsoft.Json;
using Paket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class StatusService : IStatus
    {
        private readonly MysqlContexto _mysql;
        private static readonly HttpClient _httpClient = new HttpClient();
        private string login = "AASPA";
        private string senha = "l@znNL,Lkc9x";
        private string captcha = "XD5V";
        private string token = "LWGb8VjYsZZkmJfA9JK9tQ==:E1huR9Q8It+WFpAES+pLsA==:0urZEQiqBcMNEGchHF8Elg==";

        public StatusService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public List<BuscarLogStatusClienteIdResponse> BuscarLogStatusClienteId(int clienteId)
        {
            var logs = (from log in _mysql.log_status
                        join st1 in _mysql.status on log.log_status_novo_id equals st1.status_id
                        join st2 in _mysql.status on log.log_status_antigo_id equals st2.status_id
                        where log.log_status_cliente_id == clienteId
                        select new
                        {
                            Id = log.log_status_antigo_id,
                            Status = st1.status_nome,
                            Data = log.log_status_dt_cadastro
                        })
                        .ToList()
                        .OrderByDescending(c => c.Data).ToList();

            return logs.Select(x => new BuscarLogStatusClienteIdResponse
            {
                Id = x.Id,
                Status = x.Status,
                Data = x.Data.ToString("dd/MM/yyyy HH:mm:ss")
            }).ToList();
        }

        public void AlterarStatusCliente(AlterarStatusClienteRequest request)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                if (request.status_id_novo == (int)EStatus.Deletado || request.status_id_novo == (int)EStatus.ExcluidoAguardandoEnvio)
                {
                    var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == request.cliente_id);
                    cliente.cliente_situacao = false;
                    _mysql.SaveChanges();
                }

                if (request.status_id_novo == (int)EStatus.CanceladoNaoAverbado
                    || request.status_id_novo == (int)EStatus.CanceladoAPedidoDoCliente 
                    || request.status_id_novo == (int)EStatus.Cancelado)
                {
                    var cliente = _mysql.clientes.FirstOrDefault(x => x.cliente_id == request.cliente_id);
                    cliente.cliente_motivocancelamento = request.motivo;
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

        public async Task<string> GerarToken()
        {
            try
            {
                var requestUriLogin = "https://hml.integraall.com/api/Login/validar";
                var loginRequest = new
                {
                    login,
                    senha,
                    captcha,
                    token
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUriLogin, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseObject.token;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar gerar token.", ex);
            }
        }

        public async Task<string> InativarClienteIntegraall(int clienteId, string motivocancelamento)
        {
            try
            {
                var token = await GerarToken();
                var cliente = _mysql.clientes.Where(x => x.cliente_id == clienteId).FirstOrDefault();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var url = "https://hml.integraall.com/api/Proposta/CancelarPorCpfMatricula";

                var data = new
                {
                    cpf = cliente.cliente_cpf,
                    matricula = cliente.cliente_matriculaBeneficio,
                    motivoCancelamento = motivocancelamento
                };
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (responseContent.Contains("Cliente excluido com sucesso!"))
                    {
                        return responseContent;
                    }
                    else
                    {
                        throw new Exception($"Erro ao inativar cliente: {responseContent}");
                    }
                }
                else
                {
                    throw new Exception($"Erro na API: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inativar cliente no Integraall: {ex.Message}", ex);
            }
        }
    }
}
