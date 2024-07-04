using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Model.RelatorioAverbacao;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

        public GerarRelatoriResponse GerarRelatorioAverbacao(string anomes, int captadorId)
        {
            try
            {
                var captador = _mysql.captadores.First(x => x.captador_id == captadorId);

                var corporelatorio = (from c in _mysql.clientes
                             join vin in _mysql.vinculo_cliente_captador on c.cliente_id equals vin.vinculo_cliente_id
                             join r in _mysql.registros_retorno_remessa on c.cliente_matriculaBeneficio equals r.Numero_Beneficio
                             join rr in _mysql.retornos_remessa on r.Retorno_Remessa_Id equals rr.Retorno_Id
                             join cr in _mysql.codigo_retorno on
                                        new { CodigoErro = r.Motivo_Rejeicao.ToString().PadLeft(3, '0'), CodigoOperacao = r.Codigo_Operacao }
                                 equals new { CodigoErro = cr.CodigoErro, CodigoOperacao = cr.CodigoOperacao }
                             join rrf in _mysql.registro_retorno_financeiro on c.cliente_matriculaBeneficio equals rrf.numero_beneficio into rrfGroup
                             from rrf in rrfGroup.DefaultIfEmpty()
                             join p in _mysql.pagamentos on c.cliente_id equals p.pagamento_cliente_id into pGroup
                             from p in pGroup.DefaultIfEmpty()
                             where rr.AnoMes == anomes && vin.vinculo_captador_id == captadorId
                             group new { c, r, cr, p, rrf } by new
                             {
                                 c.cliente_remessa_id,
                                 c.cliente_matriculaBeneficio,
                                 c.cliente_cpf,
                                 c.cliente_nome,
                                 r.Data_Inicio_Desconto,
                                 r.Valor_Desconto,
                                 r.Codigo_Resultado,
                                 cr.DescricaoErro,
                                 rrf.id,
                                 r.Codigo_Operacao
                             } into g
                             orderby g.Count(x => x.p != null) descending,
                                     g.Max(x => x.p.pagamento_dt_pagamento)
                             select new RelatorioAverbacaoResponse
                             {
                                 RemessaId = g.Key.cliente_remessa_id,
                                 CodExterno = g.Key.cliente_matriculaBeneficio,
                                 ClienteCpf = g.Key.cliente_cpf,
                                 ClienteNome = g.Key.cliente_nome,
                                 DataInicioDesconto = g.Key.Data_Inicio_Desconto,
                                 ValorDesconto = g.Key.Valor_Desconto,
                                 CodigoResultado = g.Key.Codigo_Resultado,
                                 CodigoOperacao = g.Key.Codigo_Operacao,
                                 DescricaoErro = g.Key.DescricaoErro,
                                 QuantidadeParcelas = g.Count(x => x.p != null),
                                 DataPagamento = g.Max(x => x.p.pagamento_dt_pagamento),
                                 Status = g.Key.Codigo_Operacao == 5 && g.Key.Codigo_Resultado == 1 ? "Excluido"
                                          : g.Key.id != null && g.Key.Codigo_Resultado == 1 ? "Pago"
                                          : g.Key.id == null && g.Key.Codigo_Resultado > 1 ? "Sem desconto"
                                          : "Erro automático"
                             }
                            ).Distinct().ToList();

                var totalRemessa = corporelatorio.Count;
                var totalNaoAverbada = corporelatorio.Count(x => x.CodigoResultado == 2 || x.CodigoResultado == 0);
                var totalAverbada = corporelatorio.Count(x => x.CodigoResultado == 1);

                var motivoNaoAverbada = (from c in corporelatorio
                                         join r in _mysql.registros_retorno_remessa on c.CodExterno equals r.Numero_Beneficio
                                         join cr in _mysql.codigo_retorno
                                          on new { CodigoErro = r.Motivo_Rejeicao.ToString().PadLeft(3, '0'), CodigoOperacao = r.Codigo_Operacao }
                                          equals new { CodigoErro = cr.CodigoErro, CodigoOperacao = cr.CodigoOperacao }
                                         where r.Codigo_Resultado == 2
                                         group cr by new { cr.CodigoErro, cr.DescricaoErro } into g
                                         select new MotivoNaoAverbacaoResponse
                                         {
                                             TotalPorCodigoErro = g.Count(),
                                             CodigoErro = g.Key.CodigoErro,
                                             DescricaoErro = g.Key.DescricaoErro
                                         }).ToList();

                var numeroRemessa = corporelatorio.Count > 0 ? corporelatorio.FirstOrDefault().RemessaId : 0;

                var taxaaverbacao = 0;
                if (totalAverbada != 0 && totalRemessa != 0)
                {
                    taxaaverbacao = (totalAverbada * 100) / totalRemessa;
                }

                var detalhes = new Detalhes
                {
                    Competencia = $"{anomes.Substring(0, 4)}{anomes.Substring(4, 2)}",
                    Averbados = totalAverbada,
                    Corretora = captador.captador_nome,
                    Remessa = numeroRemessa,
                    TaxaAverbacao = taxaaverbacao,
                };

                var resumoAverbacao = new ResumoAverbacaoResponse
                {
                    TotalRemessa = totalRemessa,
                    TotalNaoAverbada = totalNaoAverbada
                };

                var taxanaoaverbacao = 0;
                if (totalNaoAverbada != 0 && totalRemessa != 0)
                {
                    taxanaoaverbacao = (resumoAverbacao.TotalNaoAverbada * 100) / totalRemessa;
                }

                foreach (var item in motivoNaoAverbada)
                {
                    if (resumoAverbacao.TotalNaoAverbada != 0)
                    {
                        item.TotalPorcentagem = (item.TotalPorCodigoErro * 100) / resumoAverbacao.TotalNaoAverbada;
                    }
                }

                var resultado = new GerarRelatoriResponse
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
        public void GerarArquivoRelatorioAverbacao(string anomes, int captadorId)
        {
            var captador = _mysql.captadores.First(x => x.captador_id == captadorId);

            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelAverbacao.{anomes}.xlsx");
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }
            var dados = GerarRelatorioAverbacao(anomes, captadorId);

            if (File.Exists(caminhoArquivoSaida))
                File.Delete(caminhoArquivoSaida);

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

                string caminhoImagem = Path.Combine(diretorioBase, "Imagens", "logo.png");
                if (Directory.GetFiles(Path.Combine(_env.ContentRootPath, "Imagens")).Any(file => Path.GetFileName(file).Contains($"logo.png")))
                {
                    var imagem = worksheet.AddPicture(caminhoImagem ?? "")
                                .MoveTo(worksheet.Cell("G1"))
                                .WithSize((int)(8.16 * 28.3465), (int)(2.83 * 28.3465));
                }

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
                worksheet.Cell("B6").Value = long.TryParse(dados.Detalhes.Competencia, out var detalhes) ? detalhes : 0;
                worksheet.Cell("A7").Value = "CORRETORA:";
                worksheet.Cell("B7").Value = captador.captador_nome;
                worksheet.Cell("A8").Value = "Remessa:";
                worksheet.Cell("B8").Value = dados.Resumo.TotalRemessa;
                worksheet.Cell("A9").Value = "Averbados:";
                worksheet.Cell("B9").Value = dados.Detalhes.Averbados;
                worksheet.Cell("A10").Value = "Taxa de Averbação:";
                worksheet.Cell("B10").Value = $"{dados.Detalhes.TaxaAverbacao}%";

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
                        worksheet.Cell("G10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("F10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}";
                        worksheet.Cell("F11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("F12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorCodigoErro}%" : $"{0}";
                        worksheet.Cell("G12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                    }
                }
                worksheet.Cell("F13").Value = dados.MotivosNaoAverbada.Count;
                worksheet.Cell("G13").Value = $"{dados.TaxaNaoAverbado}%";

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
                    worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
                    worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber) ? cpfNumber : item.ClienteCpf;
                    worksheet.Cell(row, 3).Value = item.ClienteNome;
                    worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
                    worksheet.Cell(row, 5).Value = decimal.TryParse(item.ValorDesconto.ToString("C"), out decimal valordesconto) ? valordesconto : item.ValorDesconto;
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
        public BuscarArquivoResponse BuscarArquivoRelatorio(string anomes, int tiporel)
        {
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Relatorio");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);
            if (tiporel == 1)
            {
                path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"RelAverbacao.{anomes}.xlsx"));
            }
            else
            {
                path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"RelCarteira.{anomes}.xlsx"));
            }

            if (!File.Exists(path)) throw new Exception("Arquivo não encontrado");

            return new BuscarArquivoResponse
            {
                NomeArquivo = $"RelAverbacao.{anomes}.xlsx",
                Base64 = path
            };

        }
        public void GerarArquivoRelatorioCarteiras(string anomes, int captadorId)
        {
            var captador = _mysql.captadores.First(x => x.captador_id == captadorId);

            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelCarteira.{anomes}.xlsx");
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }
            var dados = GerarRelatorioAverbacao(anomes, captadorId);

            if (File.Exists(caminhoArquivoSaida))
                File.Delete(caminhoArquivoSaida);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório Carteira");

                int lastRow = 15 + dados.Relatorio.Count;

                var title = worksheet.Range("A1:I4");
                title.Merge();
                title.Value = "EXTRATO DE REPASSE DATA PREV";
                title.Style.Font.Bold = true;
                title.Style.Font.FontSize = 16;
                title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                title.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                string caminhoImagem = Path.Combine(diretorioBase, "Imagens", "logo.png");
                if (Directory.GetFiles(Path.Combine(_env.ContentRootPath, "Imagens")).Any(file => Path.GetFileName(file).Contains($"logo.png")))
                {
                    var imagem = worksheet.AddPicture(caminhoImagem ?? "")
                                .MoveTo(worksheet.Cell("G1"))
                                .WithSize((int)(8.16 * 28.3465), (int)(2.83 * 28.3465));
                }

                worksheet.Range("A5:I5").Merge();
                worksheet.Cell("A5").Value = "Resumo de Produção";
                worksheet.Cell("A5").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A5").Style.Font.Bold = true;
                worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var rangeA6C13 = worksheet.Range("A6:C13");
                rangeA6C13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Border.BottomBorder = XLBorderStyleValues.None;
                var rangeAC = worksheet.Range("C13:C13");
                rangeAC.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A6").Value = "COMPETENCIA:";
                worksheet.Cell("B6").Value = long.TryParse(dados.Detalhes.Competencia, out var detalhes) ? detalhes : 0;
                worksheet.Cell("A7").Value = "CORRETORA:";
                worksheet.Cell("B7").Value = captador.captador_nome;
                worksheet.Cell("A9").Value = "Carteira:";
                worksheet.Cell("B9").Value = "Qtde total";
                worksheet.Cell("C9").Value = dados.Relatorio.Count;
                worksheet.Cell("B10").Value = "Cancelados";
                worksheet.Cell("C10").Value = dados.Relatorio.Count(x=> x.Status == "Excluido");
                worksheet.Cell("B11").Value = "Inadimplentes";
                worksheet.Cell("C11").Value = dados.Relatorio.Count(x => x.Status == "Sem desconto");
                worksheet.Cell("B12").Value = "Em dia";
                worksheet.Cell("C12").Value = dados.Relatorio.Count(x => x.Status == "Pago");

                var rangeD6G13 = worksheet.Range("D6:I13");
                rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
                rangeA6C13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeA6C13.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Cell("D6").Value = "Motivos não averbados";
                worksheet.Cell("D7").Value = "002 - Espécie incompatível";
                worksheet.Cell("D8").Value = "004 - NB inexistente no cadastro";
                worksheet.Cell("D9").Value = "005 - Benefício não ativo";
                worksheet.Cell("D10").Value = "006 - Valor ultrapassa MR do titular";
                worksheet.Cell("D11").Value = "008 - Já existe desc. p/ outra entidade";
                worksheet.Cell("D12").Value = "012 - Benefício bloqueado para desconto";
                worksheet.Cell("D13").Value = "Total Não averbado";

                worksheet.Cell("G7").Value = 0;
                worksheet.Cell("H7").Value = $"{0}%";
                worksheet.Cell("G7").Value = 0;
                worksheet.Cell("H8").Value = $"{0}%";
                worksheet.Cell("G9").Value = 0;
                worksheet.Cell("H9").Value = $"{0}%";
                worksheet.Cell("G10").Value = 0;
                worksheet.Cell("H10").Value = $"{0}%";
                worksheet.Cell("G11").Value = 0;
                worksheet.Cell("H11").Value = $"{0}%";
                worksheet.Cell("G12").Value = 0;
                worksheet.Cell("GH12").Value = $"{0}%";
                worksheet.Cell("G13").Value = 0;
                worksheet.Cell("H13").Value = $"{0}%";

                if (dados.MotivosNaoAverbada != null && dados.MotivosNaoAverbada.Count > 0)
                {
                    foreach (var item in dados.MotivosNaoAverbada)
                    {
                        worksheet.Cell("G6").Value = "Total não averbados";
                        worksheet.Cell("H6").Value = "%";
                        worksheet.Cell("G7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
                        worksheet.Cell("H7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? $"{item.TotalPorcentagem}&" : $"{0}%";
                        worksheet.Cell("G8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
                        worksheet.Cell("H8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("G9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("H9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("G10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                        worksheet.Cell("H10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}";
                        worksheet.Cell("H11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
                        worksheet.Cell("G12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorCodigoErro}%" : $"{0}";
                        worksheet.Cell("H12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
                    }
                }
                worksheet.Cell("F13").Value = dados.MotivosNaoAverbada.Count;
                worksheet.Cell("G13").Value = $"{dados.TaxaNaoAverbado}%";

                worksheet.Cell("A14").Value = "Detalhe de Produção";
                worksheet.Range("A14:I14").Merge();
                worksheet.Cell("A14").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A14").Style.Font.Bold = true;
                worksheet.Cell("A14").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A15").Value = "NB/Matrícula";
                worksheet.Cell("B15").Value = "CPF";
                worksheet.Cell("C15").Value = "Nome";
                worksheet.Cell("D15").Value = "Data Adesão";
                worksheet.Cell("E15").Value = "Taxa Associativa";
                worksheet.Cell("F15").Value = "Parcela Atual";
                worksheet.Cell("G15").Value = "Data Pagameto";
                worksheet.Cell("H15").Value = "Status";
                worksheet.Cell("I15").Value = "Motivo";

                int row = 16;
                foreach (var item in dados.Relatorio)
                {
                    worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
                    worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber) ? cpfNumber : item.ClienteCpf;
                    worksheet.Cell(row, 3).Value = item.ClienteNome;
                    worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
                    worksheet.Cell(row, 5).Value = decimal.TryParse(item.ValorDesconto.ToString("C"), out decimal valordesconto) ? valordesconto : item.ValorDesconto;
                    worksheet.Cell(row, 6).Value = item.QuantidadeParcelas;
                    worksheet.Cell(row, 7).Value = item.DataPagamento;
                    worksheet.Cell(row, 8).Value = item.Status;
                    worksheet.Cell(row, 9).Value = item.DescricaoErro;
                    row++;
                }

                var range = worksheet.Range("A15:I" + row);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var outlineRange = worksheet.Range("A1:I" + row);
                outlineRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                outlineRange.Style.Border.OutsideBorderColor = XLColor.Blue;

                worksheet.Columns().AdjustToContents();
                worksheet.Column("A").Width = 18;
                worksheet.Column("B").Width = 15;
                worksheet.Column("C").Width = 40;
                worksheet.Column("D").Width = 15;
                worksheet.Column("E").Width = 15;
                worksheet.Column("F").Width = 15;
                worksheet.Column("G").Width = 15;
                worksheet.Column("H").Width = 15;
                worksheet.Column("I").Width = 25;

                worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                worksheet.PageSetup.PrintAreas.Add("A1:I" + row);
                worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

                workbook.SaveAs(caminhoArquivoSaida);
            }
        }
    }
}
