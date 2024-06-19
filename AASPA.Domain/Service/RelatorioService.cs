using AASPA.Controllers;
using AASPA.Models.Model.RelatorioAverbacao;
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
                                      join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                      where rr.AnoMes == anomes
                                      select new RelatorioAverbacaoResponse
                                      {
                                          CodExterno = c.cliente_matriculaBeneficio,
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
                                        join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                        where rr.AnoMes == anomes && r.Codigo_Resultado == 2
                                        select r).Count();

                var totalAverbada = (from c in _mysql.clientes
                                     join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                     join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                     join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                     where rr.AnoMes == anomes && r.Codigo_Resultado == 1
                                     select r).Count();

                var motivoNaoAverbada = (from c in _mysql.clientes
                                         join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                         join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                         join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                         where rr.AnoMes == anomes && r.Codigo_Resultado == 2
                                         group cr by new { cr.CodigoErro, cr.DescricaoErro } into g
                                         select new MotivoNaoAverbacaoResponse
                                         {
                                             TotalPorCodigoErro = g.Count(),
                                             DescricaoErro = g.Key.DescricaoErro
                                         }).ToList();

                var numeroRemessa = (from c in _mysql.clientes
                                     join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                     join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                     join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                     where rr.AnoMes == anomes
                                     select r.Retorno_Remessa_Id).FirstOrDefault();

                var detalhes = new Detalhes
                {
                    Competencia = $"{anomes.Substring(0, 4)}{anomes.Substring(4, 2)}",
                    Averbados = totalAverbada,
                    Corretora = "Confia",
                    Remessa = numeroRemessa,
                    TaxaAverbacao = (totalAverbada * 100) / (totalNaoAverbada + totalAverbada)
                };

                foreach (var item in motivoNaoAverbada)
                {
                    item.TotalPorcentagem = (item.TotalPorCodigoErro * 100) / (totalNaoAverbada + totalAverbada);
                }

                var resumoAverbacao = new ResumoAverbacaoResponse
                {
                    TotalRemessa = totalRemessa,
                    TotalNaoAverbada = totalNaoAverbada
                };

                var resultado = new GerarRelatorioAverbacaoResponse
                {
                    Detalhes = detalhes,
                    TaxaNaoAverbado = (resumoAverbacao.TotalNaoAverbada * 100) / (totalNaoAverbada + totalAverbada),
                    Relatorio = corporelatorio,
                    Resumo = resumoAverbacao,
                    MotivosNaoAverbada = motivoNaoAverbada
                };

                return resultado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
