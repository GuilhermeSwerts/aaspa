using AASPA.Controllers;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AASPA.Domain.Service
{
    public class RelatorioService : IRelatorios
    {
        private readonly MysqlContexto _mysql;
        public RelatorioService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }
        public GerarRelatorioAverbacaoResponse GerarRelatorioAverbacao(string anomes)
        {
            try
            {
                var corporelatorio = (from c in _mysql.clientes
                                      join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                      join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                      join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao equals int.Parse(cr.CodigoErro)
                                      where rr.AnoMes == anomes
                                      select new RelatorioAverbacaoResponse
                                      {
                                          ClienteCpf = c.cliente_cpf,
                                          ClienteNome = c.cliente_nome,
                                          DataInicioDesconto = r.Data_Inicio_Desconto,
                                          ValorDesconto = r.Valor_Desconto,
                                          CodigoResultado = r.Codigo_Resultado,
                                          DescricaoErro = cr.DescricaoErro
                                      }).ToList();

                var totalRemessa = (from r in _mysql.retornos_remessa
                                    join rr in _mysql.registros_retorno_remessa on r.Retorno_Id equals rr.Retorno_Remessa_Id
                                    where r.AnoMes == anomes
                                    select r).Count();

                var totalNaoAverbada = (from c in _mysql.clientes
                                        join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                        join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                        join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao equals int.Parse(cr.CodigoErro)
                                        where rr.AnoMes == anomes && r.Codigo_Resultado == 2
                                        select r).Count();

                var motivoNaoAverbadaQuery = from c in _mysql.clientes
                                             join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                             join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                             join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao equals int.Parse(cr.CodigoErro)
                                             where rr.AnoMes == anomes
                                             group cr by new { cr.CodigoErro, cr.DescricaoErro } into g
                                             select new MotivoNaoAverbacaoResponse
                                             {
                                                 TotalPorCodigoErro = g.Count(),
                                                 DescricaoErro = g.Key.DescricaoErro
                                             };

                var motivoNaoAverbada = motivoNaoAverbadaQuery.ToList();

                var resumoAverbacao = new ResumoAverbacaoResponse
                {
                    TotalRemessa = totalRemessa,
                    TotalNaoAverbada = totalNaoAverbada
                };

                var resultado = new GerarRelatorioAverbacaoResponse
                {
                    Relatorio = corporelatorio,
                    Resumo = resumoAverbacao,
                    MotivosNaoAverbada = motivoNaoAverbada
                };

                return resultado;
            }
            catch (Exception)
            {

            }
            return null;
        }

    }
}
