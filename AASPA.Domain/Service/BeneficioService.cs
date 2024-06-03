using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class BeneficioService : IBeneficio
    {
        private readonly MysqlContexto _mysql;

        public BeneficioService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object BuscarBeneficioId(int beneficioId)
        {
            return _mysql.beneficios.FirstOrDefault(x => x.beneficio_id == beneficioId)
                ?? throw new Exception("Beneficio do id: {beneficioId} não encontrado");
        }

        public object BuscarLogBeneficiosClienteId(int clienteId, DateTime? dtInicio = null, DateTime? dtFim = null)
        {
            var logs = _mysql.log_beneficios.Where(x => 
            x.log_beneficios_cliente_id == clienteId
             && (dtInicio == null || x.log_beneficios_dt_cadastro >= DateTime.Parse(DateTime.Parse(dtInicio.ToString()).ToString("dd/MM/yyyy 00:00:00")))
             && (dtFim == null || x.log_beneficios_dt_cadastro <= DateTime.Parse(DateTime.Parse(dtFim.ToString()).ToString("dd/MM/yyyy 23:59:59")))
            ).ToList();

            var logsRemovidos = _mysql.log_beneficios.Where(x =>
            x.log_beneficios_cliente_id == clienteId
             && (dtInicio == null || x.log_beneficios_dt_removido >= DateTime.Parse(DateTime.Parse(dtInicio.ToString()).ToString("dd/MM/yyyy 00:00:00")))
             && (dtFim == null || x.log_beneficios_dt_removido <= DateTime.Parse(DateTime.Parse(dtFim.ToString()).ToString("dd/MM/yyyy 23:59:59")))
            ).ToList();

            logs.AddRange(logsRemovidos);
            List<LogBeneficioResponse> response = new();

            foreach (var log in logs)
            {
                var beneficio = _mysql.beneficios.FirstOrDefault(x => 
                x.beneficio_id == log.log_beneficios_beneficio_id);
                var logResponse = new LogBeneficioResponse
                {
                    Nome = beneficio.beneficio_nome_beneficio,
                    Acao = log.log_beneficios_acao_id == 1 ? "ADICIONADO" : "REMOVIDO",
                    DataVinculo = log.log_beneficios_dt_cadastro.ToString("dd/MM/yyy HH:mm:ss"),
                    DataRemocaoVinculo = log.log_beneficios_dt_removido != null
                    ? DateTime.Parse(log.log_beneficios_dt_removido.ToString()).ToString("dd/MM/yyy HH:mm:ss")
                    : "-",
                    VinculoAtivo = log.log_beneficios_ativo ? "SIM" : "NÃO"
                };
                if (!response.Any(x=> x == logResponse))
                    response.Add(logResponse);
            }

            return response;
        }

        public object BuscarTodosBeneficios()
        {
            return _mysql.beneficios.Where(x => x.beneficio_id > 0).ToList();
        }

        public void EditarBeneficio(BeneficioRequest request)
        {
            var beneficio = _mysql.beneficios.FirstOrDefault(x => x.beneficio_id == request.BeneficioId) ??
                throw new Exception("Beneficio não cadastrado");
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var valor = decimal.Parse(request.ValorAPagarAoFornecedor.Replace(".", ","));

                beneficio.beneficio_cod_beneficio = int.Parse(request.CodBeneficio);
                beneficio.beneficio_descricao_beneficios = request.DescricaoBeneficios;
                beneficio.beneficio_fornecedor_beneficio = request.FornecedorBeneficio;
                beneficio.beneficio_nome_beneficio = request.NomeBeneficio;
                beneficio.beneficio_valor_a_pagar_ao_fornecedor = valor;

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void NovoBeneficio(BeneficioRequest request)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                if (_mysql.beneficios.Any(x => x.beneficio_nome_beneficio.ToUpper() == request.NomeBeneficio.ToUpper()))
                    throw new Exception("Beneficio já cadastrado");

                var valor = decimal.Parse(request.ValorAPagarAoFornecedor.Replace(".", ","));
                _mysql.beneficios.Add(new Repository.Maps.BeneficioDb
                {
                    beneficio_cod_beneficio = int.Parse(request.CodBeneficio),
                    beneficio_descricao_beneficios = request.DescricaoBeneficios,
                    beneficio_dt_beneficio = DateTime.Now,
                    beneficio_fornecedor_beneficio = request.FornecedorBeneficio,
                    beneficio_nome_beneficio = request.NomeBeneficio,
                    beneficio_valor_a_pagar_ao_fornecedor = valor
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

        public void VincularBeneficios(int clienteId, List<int> beneficiosIds)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var beneficiosOld = _mysql.log_beneficios
                                        .Where(x => x.log_beneficios_cliente_id == clienteId && x.log_beneficios_dt_removido == null)
                                        .Select(x => x.log_beneficios_beneficio_id)
                                        .ToList();
                var beneficiosNew = "";

                var BeneficiosRemovidosIds = beneficiosOld.Except(beneficiosIds).ToList();
                var BeneficiosNovosInds = beneficiosIds.Except(beneficiosOld).ToList();

                foreach (var beneficioId in BeneficiosRemovidosIds)
                {
                    var log = _mysql.log_beneficios
                        .FirstOrDefault(x =>
                            x.log_beneficios_beneficio_id == beneficioId
                            && x.log_beneficios_cliente_id == clienteId
                            && x.log_beneficios_ativo);

                    if (log != null)
                    {
                        log.log_beneficios_ativo = false;
                        log.log_beneficios_acao_id = (int)AcaoLogBeneficio.Removido;
                        log.log_beneficios_dt_removido = DateTime.Now;
                        _mysql.SaveChanges();
                    }
                }

                foreach (var beneficioId in BeneficiosNovosInds)
                {
                    _mysql.log_beneficios.Add(new LogBeneficioDb
                    {
                        log_beneficios_acao_id = (int)AcaoLogBeneficio.Adicionado,
                        log_beneficios_ativo = true,
                        log_beneficios_beneficio_id = beneficioId,
                        log_beneficios_cliente_id = clienteId,
                        log_beneficios_dt_cadastro = DateTime.Now
                    });
                    _mysql.SaveChanges();
                }

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
