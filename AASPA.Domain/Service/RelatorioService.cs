using AASPA.Controllers;
using AASPA.Models.Model.RelatorioAverbacao;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AASPA.Domain.Service
{
    public class RelatorioService : IRelatorios
    {
        private readonly MysqlContexto _mysql;
        private readonly IHostEnvironment _env;
        public RelatorioService(MysqlContexto mysql, IHostEnvironment env)
        {
            _mysql = mysql;
            _env = env;
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
                                        join cr in _mysql.codigo_retorno on new { Operacao = r.Codigo_Operacao, Resultado = r.Codigo_Resultado } equals new { Operacao = cr.CodigoOperacao, Resultado = cr.CodigoResultado }
                                        where rr.AnoMes == anomes && r.Codigo_Resultado == 2
                                        select r).Count();

                var totalAverbada = (from c in _mysql.clientes
                                     join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                     join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                     join cr in _mysql.codigo_retorno on new { Operacao = r.Codigo_Operacao, Resultado = r.Codigo_Resultado } equals new { Operacao = cr.CodigoOperacao, Resultado = cr.CodigoResultado }
                                     where rr.AnoMes == anomes && r.Codigo_Resultado == 1
                                     select r).Count();

                var motivoNaoAverbada = (from c in _mysql.clientes
                                         join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                         join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                         join cr in _mysql.codigo_retorno on new { Operacao = r.Codigo_Operacao, Resultado = r.Codigo_Resultado } equals new { Operacao = cr.CodigoOperacao, Resultado = cr.CodigoResultado }
                                         where rr.AnoMes == anomes && r.Codigo_Resultado == 2
                                         group cr by new { cr.CodigoErro, cr.DescricaoErro } into g
                                         select new MotivoNaoAverbacaoResponse
                                         {
                                             TotalPorCodigoErro = g.Count(),
                                             CodigoErro = g.Key.CodigoErro,
                                             DescricaoErro = g.Key.DescricaoErro
                                         }).ToList();

                var numeroRemessa = (from c in _mysql.clientes
                                     join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                                     join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                                     join cr in _mysql.codigo_retorno on r.Motivo_Rejeicao.ToString().PadLeft(3, '0') equals cr.CodigoErro
                                     where rr.AnoMes == anomes
                                     select r.Retorno_Remessa_Id).FirstOrDefault();

                var taxaaverbacao = 0;
                if (totalAverbada != 0 && totalNaoAverbada != 0)
                {
                    taxaaverbacao = (totalAverbada * 100) / (totalNaoAverbada + totalAverbada);
                    foreach (var item in motivoNaoAverbada)
                    {
                        item.TotalPorcentagem = (item.TotalPorCodigoErro * 100) / (totalNaoAverbada + totalAverbada);
                    }
                }

                var detalhes = new Detalhes
                {
                    Competencia = $"{anomes.Substring(0, 4)}{anomes.Substring(4, 2)}",
                    Averbados = totalAverbada,
                    Corretora = "Confia",
                    Remessa = numeroRemessa,
                    TaxaAverbacao = taxaaverbacao,
                };
                

                var resumoAverbacao = new ResumoAverbacaoResponse
                {
                    TotalRemessa = totalRemessa,
                    TotalNaoAverbada = totalNaoAverbada
                };

                var taxanaoaverbacao = 0;
                if (totalAverbada != 0 && totalNaoAverbada != 0)
                {
                    taxanaoaverbacao = (resumoAverbacao.TotalNaoAverbada * 100) / (totalNaoAverbada + totalAverbada);
                }

                var resultado = new GerarRelatorioAverbacaoResponse
                {
                    Detalhes = detalhes,
                    TaxaNaoAverbado = taxanaoaverbacao,
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
        public void GerarArquivoRelatorioAverbacao(string anomes)
        {
            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelAverbacao.{anomes}.xlsx");
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            var dados = GerarRelatorioAverbacao(anomes);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório Averbacao");

                int lastRow = 15 + dados.Relatorio.Count;

                var title = worksheet.Range("A1:G4");
                title.Merge();
                title.Value = "EXTRATO DE RETORNO DATA PREV";
                title.Style.Font.Bold = true;
                title.Style.Font.FontSize = 16;
                title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                title.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range("A5:G5").Merge();
                worksheet.Cell("A5").Value = "Resumo de Produção";
                worksheet.Cell("A5").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A5").Style.Font.Bold = true;
                worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var rangeA6C13 = worksheet.Range("A6:C13");
                rangeA6C13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.BottomBorder = XLBorderStyleValues.None;

                worksheet.Cell("A6").Value = "COMPETENCIA:";
                worksheet.Cell("B6").Value = long.TryParse(dados.Detalhes.Competencia, out var detalhes)? detalhes : 0;
                worksheet.Cell("A7").Value = "CORRETORA:";
                worksheet.Cell("B7").Value = "Confia";
                worksheet.Cell("A8").Value = "Remessa:";
                worksheet.Cell("B8").Value = dados.Resumo.TotalRemessa;
                worksheet.Cell("A9").Value = "Averbados:";
                worksheet.Cell("B9").Value = dados.Detalhes.Averbados;
                worksheet.Cell("A10").Value = "Taxa de Averbação:";
                worksheet.Cell("B10").Value = $"{(dados.Detalhes.Averbados * 100) / dados.Resumo.TotalRemessa}%"; 

                var rangeD6G13 = worksheet.Range("D6:G13");
                rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
                var rangeF6G13 = worksheet.Range("F6:G13");
                rangeF6G13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("D6").Value = "Motivos não averbados";
                worksheet.Cell("D7").Value = "002 - Espécie incompatível";
                worksheet.Cell("D8").Value = "004 - NB inexistente no cadastro";
                worksheet.Cell("D9").Value = "005 - Benefício não ativo";
                worksheet.Cell("D10").Value = "006 - Valor ultrapassa MR do titular";
                worksheet.Cell("D11").Value = "008 - Já existe desc. p/ outra entidade";
                worksheet.Cell("D12").Value = "012 - Benefício bloqueado para desconto";
                worksheet.Cell("D13").Value = "Total Não averbado";

                worksheet.Cell("F7").Value = 0;
                worksheet.Cell("G7").Value = $"{0}%";
                worksheet.Cell("F8").Value = 0;
                worksheet.Cell("G8").Value = $"{0}%";
                worksheet.Cell("F9").Value = 0;
                worksheet.Cell("G9").Value = $"{0}%";
                worksheet.Cell("F10").Value = 0;
                worksheet.Cell("G10").Value = $"{0}%";
                worksheet.Cell("F11").Value = 0;
                worksheet.Cell("G11").Value = $"{0}%";
                worksheet.Cell("F12").Value = 0;
                worksheet.Cell("G12").Value = $"{0}%";
                worksheet.Cell("F13").Value = 0;
                worksheet.Cell("G13").Value = $"{0}%";

                if (dados.MotivosNaoAverbada != null && dados.MotivosNaoAverbada.Count > 0)
                {
                    foreach (var item in dados.MotivosNaoAverbada)
                    {
                        worksheet.Cell("F6").Value = "Total não averbados";
                        worksheet.Cell("G6").Value = "%";
                        worksheet.Cell("F7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
                        worksheet.Cell("G7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? $"{item.TotalPorcentagem}&" : $"{0}%";
                        worksheet.Cell("F8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
                        worksheet.Cell("G8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("F9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("F10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("F11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"";
                        worksheet.Cell("F12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorCodigoErro}%" : $"{0}";
                        worksheet.Cell("G12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                    }
                }
                worksheet.Cell("F13").Value = dados.MotivosNaoAverbada.Count;
                worksheet.Cell("G13").Value = $"{(dados.MotivosNaoAverbada.Count * 100) / dados.Resumo.TotalRemessa}%";

                worksheet.Cell("A14").Value = "Detalhe de Produção";
                worksheet.Range("A14:G14").Merge();
                worksheet.Cell("A14").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A14").Style.Font.Bold = true;
                worksheet.Cell("A14").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A15").Value = "Cod Externo";
                worksheet.Cell("B15").Value = "CPF";
                worksheet.Cell("C15").Value = "Nome";
                worksheet.Cell("D15").Value = "Data Adesão";
                worksheet.Cell("E15").Value = "Taxa Associativa";
                worksheet.Cell("F15").Value = "Status";
                worksheet.Cell("G15").Value = "Motivo";

                int row = 16;
                foreach (var item in dados.Relatorio)
                {
                    worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno)? codexterno : item.CodExterno;
                    worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber)? cpfNumber : item.ClienteCpf;
                    worksheet.Cell(row, 3).Value = item.ClienteNome;
                    worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
                    worksheet.Cell(row, 5).Value = decimal.TryParse(item.ValorDesconto.ToString("C"), out decimal valordesconto)? valordesconto : item.ValorDesconto;
                    worksheet.Cell(row, 6).Value = item.CodigoResultado == 1 ? "Averbado" : "Não Averbado";
                    worksheet.Cell(row, 7).Value = item.DescricaoErro;
                    row++;
                }

                var range = worksheet.Range("A15:G" + row);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var outlineRange = worksheet.Range("A1:G" + row);
                outlineRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                outlineRange.Style.Border.OutsideBorderColor = XLColor.Blue;

                worksheet.Columns().AdjustToContents();
                worksheet.Column("A").Width = 18;
                worksheet.Column("B").Width = 15;
                worksheet.Column("C").Width = 40;
                worksheet.Column("D").Width = 18;
                worksheet.Column("E").Width = 18;
                worksheet.Column("F").Width = 18;
                worksheet.Column("G").Width = 38;

                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                worksheet.PageSetup.PrintAreas.Add("A1:G" + row);
                worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

                workbook.SaveAs(caminhoArquivoSaida);
            }
        }
        public BuscarArquivoResponse BuscarArquivoAverbacao(string anomes)
        {
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Relatorio");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);

            path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"RelAverbacao.{anomes}.xlsx"));

            if (!File.Exists(path)) throw new Exception("Arquivo não encontrado");

            return new BuscarArquivoResponse
            {
                NomeArquivo = $"RelAverbacao.{anomes}.xlsx",
                Base64 = path
            };

        }
    }
}
