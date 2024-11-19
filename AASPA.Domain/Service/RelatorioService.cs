using AASPA.Controllers;
using AASPA.Domain.Interface;
using AASPA.Models.Model.RelatorioAverbacao;
using AASPA.Models.Model.RelatorioRepasse;
using AASPA.Models.Response;
using AASPA.Repository;
using AASPA.Repository.Maps;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Path = System.IO.Path;

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

        public List<RelatorioAverbacaoResponse> BuscarClientesRelatorioRetorno(string anomes)
        {
            var codRetornos = _mysql.codigo_retorno.ToList();
            var retorno = _mysql.retornos_remessa.FirstOrDefault(x => x.AnoMes == anomes)
                   ?? throw new Exception("Não existe retorno para mês/ano competente");
            var clientes = (
                                from ret in _mysql.registros_retorno_remessa
                                join cli in _mysql.clientes
                                    on ret.Numero_Beneficio equals cli.cliente_matriculaBeneficio into cliGroup
                                from cli in cliGroup.DefaultIfEmpty()
                                where ret.Retorno_Remessa_Id == retorno.Retorno_Id
                                select new
                                {
                                    Cliente = cli ?? new ClienteDb
                                    {
                                        cliente_matriculaBeneficio = ret.Numero_Beneficio,
                                        cliente_cpf = "-",
                                        cliente_nome = "CLIENTE NAO ENCONTRADO NA BASE",
                                    },
                                    Registro = ret
                                }).ToList();

            return clientes.Select(x => new RelatorioAverbacaoResponse
            {
                Status =
                      x.Registro.Codigo_Resultado == 1 && x.Registro.Codigo_Resultado == 1 ? "Incluído(a)"
                    : x.Registro.Codigo_Operacao == 5 && x.Registro.Codigo_Resultado == 1 ? "Excluído(a)"
                    : x.Registro.Codigo_Operacao == 7 && x.Registro.Codigo_Resultado == 0 ? "Erro Automático"
                    : "Erro",
                RemessaId = x.Registro.Retorno_Remessa_Id,
                ClienteCpf = x.Cliente.cliente_cpf,
                ClienteNome = x.Cliente.cliente_nome,
                CodExterno = x.Cliente.cliente_matriculaBeneficio.PadLeft(10, '0'),
                CodigoOperacao = x.Registro.Codigo_Operacao,
                CodigoResultado = x.Registro.Codigo_Resultado,
                DataInicioDesconto = x.Registro.Data_Inicio_Desconto,
                ValorDesconto = FormatarValorDescontado(x.Registro.Valor_Desconto).ToString("C", new CultureInfo("pt-BR")),
                DescricaoErro = codRetornos
                .FirstOrDefault(c => c.CodigoErro == x.Registro.Motivo_Rejeicao.ToString().PadLeft(3, '0') && c.CodigoOperacao == x.Registro.Codigo_Operacao)
                != null ? codRetornos
                .FirstOrDefault(c => c.CodigoErro == x.Registro.Motivo_Rejeicao.ToString().PadLeft(3, '0') && c.CodigoOperacao == x.Registro.Codigo_Operacao)
                .DescricaoErro : $"Codigo de erro {x.Registro.Motivo_Rejeicao.ToString().PadLeft(3, '0')} ou Codigo da operação {x.Registro.Codigo_Operacao} nao encontrados",
                CodOperacao = x.Registro.Codigo_Operacao,
                CodResultado = x.Registro.Codigo_Resultado,
                CodErro = x.Registro.Motivo_Rejeicao
            }).Distinct().ToList();
        }

        public GerarRelatoriResponse GerarRelatorioRetorno(string anomes, int captadorId)
        {
            try
            {
                var retorno = _mysql.retornos_remessa.FirstOrDefault(x => x.AnoMes == anomes)
                    ?? throw new Exception("Não existe nenhum retorno para Ano/Mês competente");

                var relatorio = BuscarClientesRelatorioRetorno(anomes);

                int qtdRegistro = _mysql.registros_retorno_remessa.Count(x => x.Retorno_Remessa_Id == retorno.Retorno_Id);
                int qtdAverbados = relatorio.Count(x => x.CodOperacao == 1 && x.CodResultado == 1);
                int qtdNaoExcluidos = relatorio.Count(x => x.CodOperacao == 5 && x.CodResultado == 2);
                int qtdExcluidos = relatorio.Count(x => x.CodOperacao == 5 && x.CodResultado == 1);
                int qtdAutomatico = relatorio.Count(x => x.CodOperacao == 7 && x.CodResultado == 0);
                int qtdNaoAverbados = relatorio.Count(x => x.CodOperacao == 1 && x.CodResultado == 2);

                var motivoNaoAverbada = relatorio.Where(x => x.CodigoOperacao == 1 && x.CodigoResultado == 2)
                    .GroupBy(x => new
                    {
                        x.CodErro,
                        x.DescricaoErro
                    }).Select(x => new MotivoNaoAverbacaoResponse
                    {
                        TotalPorCodigoErro = x.Count(),
                        CodigoErro = x.Key.CodErro.ToString().PadLeft(3, '0'),
                        DescricaoErro = x.Key.DescricaoErro
                    })
                    .ToList();

                var motivoNaoExclusao = relatorio.Where(x => x.CodigoOperacao == 5 && x.CodigoResultado == 2)
                    .GroupBy(x => new
                    {
                        x.CodErro,
                        x.DescricaoErro
                    }).Select(x => new MotivoNaoAverbacaoResponse
                    {
                        TotalPorCodigoErro = x.Count(),
                        CodigoErro = x.Key.CodErro.ToString().PadLeft(3, '0'),
                        DescricaoErro = x.Key.DescricaoErro
                    })
                    .ToList();

                var motivoAutomatico = relatorio.Where(x => x.CodigoOperacao == 7 && x.CodigoResultado == 0)
                    .GroupBy(x => new
                    {
                        x.CodErro,
                        x.DescricaoErro
                    }).Select(x => new MotivoNaoAverbacaoResponse
                    {
                        TotalPorCodigoErro = x.Count(),
                        CodigoErro = x.Key.CodErro.ToString().PadLeft(3, '0'),
                        DescricaoErro = x.Key.DescricaoErro
                    })
                    .ToList();


                var taxaaverbacao = 0.0;
                if (qtdAverbados != 0 && qtdRegistro != 0)
                    taxaaverbacao = Math.Round((double)qtdAverbados * 100 / qtdRegistro, 2);

                var taxaNaoAverbacao = 0.0;
                foreach (var item in motivoNaoAverbada)
                {
                    item.TotalPorcentagem = Math.Round((double)item.TotalPorCodigoErro * 100 / qtdRegistro, 2);
                    taxaNaoAverbacao += item.TotalPorcentagem;
                }

                var taxaErroAuto = 0.0;
                foreach (var item in motivoAutomatico)
                {
                    item.TotalPorcentagem = Math.Round((double)item.TotalPorCodigoErro * 100 / qtdRegistro, 2);
                    taxaErroAuto += item.TotalPorcentagem;
                }

                var taxaNaoExcluido = 0.0;
                foreach (var item in motivoNaoExclusao)
                {
                    item.TotalPorcentagem = Math.Round((double)item.TotalPorCodigoErro * 100 / qtdRegistro, 2);
                    taxaNaoExcluido += item.TotalPorcentagem;
                }

                return new GerarRelatoriResponse
                {
                    Relatorio = relatorio,
                    TaxaAverbacao = taxaaverbacao,
                    TaxaNaoAverbacao = taxaNaoAverbacao,
                    TaxaErroAuto = taxaErroAuto,
                    TaxaNaoExcluido = taxaNaoExcluido,
                    QtdNaoAverbados = qtdNaoAverbados,
                    QtdRegistro = qtdRegistro,
                    QtdAverbados = qtdAverbados,
                    QtdExcluidos = qtdExcluidos,
                    QtdAutomatico = qtdAutomatico,
                    QtdNaoExcluidos = qtdNaoExcluidos,
                    MotivoNaoAverbada = motivoNaoAverbada,
                    MotivoAutomatico = motivoAutomatico,
                    MotivoNaoExclusao = motivoNaoExclusao,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GerarArquivoRelatorioAverbacao(string anomes, int captadorId)
        {
            //var captador = captadorId > 0 ? _mysql.captadores.First(x => x.captador_id == captadorId) : new() { captador_nome = "TODOS" };

            //string diretorioBase = _env.ContentRootPath;
            //string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelAverbacao.{anomes}.xlsx");
            //if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            //if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }
            //var dados = GerarRelatorioRetorno(anomes, captadorId);

            //if (File.Exists(caminhoArquivoSaida))
            //    File.Delete(caminhoArquivoSaida);

            //using (var workbook = new XLWorkbook())
            //{
            //    var worksheet = workbook.Worksheets.Add("Relatório Averbacao");

            //    int lastRow = 15 + dados.Relatorio.Count;

            //    var title = worksheet.Range("A1:G4");
            //    title.Merge();
            //    title.Value = "EXTRATO DE RETORNO DATAPREV";
            //    title.Style.Font.Bold = true;
            //    title.Style.Font.FontSize = 16;
            //    title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //    title.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //    string caminhoImagem = Path.Combine(diretorioBase, "Imagens", "logo.png");
            //    if (Directory.GetFiles(Path.Combine(_env.ContentRootPath, "Imagens")).Any(file => Path.GetFileName(file).Contains($"logo.png")))
            //    {
            //        var imagem = worksheet.AddPicture(caminhoImagem ?? "")
            //                    .MoveTo(worksheet.Cell("G1"))
            //                    .WithSize((int)(8.16 * 28.3465), (int)(2.83 * 28.3465));
            //    }

            //    worksheet.Range("A5:G5").Merge();
            //    worksheet.Cell("A5").Value = "Resumo de Produção";
            //    worksheet.Cell("A5").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
            //    worksheet.Cell("A5").Style.Font.Bold = true;
            //    worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    var rangeA6C13 = worksheet.Range("A6:C13");
            //    rangeA6C13.Style.Border.LeftBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.RightBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.TopBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.BottomBorder = XLBorderStyleValues.None;

            //    worksheet.Cell("A6").Value = "COMPETENCIA:";
            //    worksheet.Cell("B6").Value = long.TryParse(dados.Detalhes.Competencia, out var detalhes) ? detalhes : 0;
            //    worksheet.Cell("A7").Value = "CORRETORA:";
            //    worksheet.Cell("B7").Value = captador.captador_nome;
            //    worksheet.Cell("A8").Value = "Remessa:";
            //    worksheet.Cell("B8").Value = dados.Resumo.TotalRemessa;
            //    worksheet.Cell("A9").Value = "Averbados:";
            //    worksheet.Cell("B9").Value = dados.Detalhes.Averbados;
            //    worksheet.Cell("A10").Value = "Taxa de Averbação:";
            //    worksheet.Cell("B10").Value = $"{dados.Detalhes.TaxaAverbacao}%";

            //    var rangeD6G13 = worksheet.Range("D6:G13");
            //    rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
            //    var rangeF6G13 = worksheet.Range("F6:G13");
            //    rangeF6G13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    worksheet.Cell("D6").Value = "Motivos não averbados";
            //    worksheet.Cell("D7").Value = "002 - Espécie incompatível";
            //    worksheet.Cell("D8").Value = "004 - NB inexistente no cadastro";
            //    worksheet.Cell("D9").Value = "005 - Benefício não ativo";
            //    worksheet.Cell("D10").Value = "006 - Valor ultrapassa MR do titular";
            //    worksheet.Cell("D11").Value = "008 - Já existe desc. p/ outra entidade";
            //    worksheet.Cell("D12").Value = "012 - Benefício bloqueado para desconto";
            //    worksheet.Cell("D13").Value = "Total Não averbado";

            //    worksheet.Cell("F7").Value = 0;
            //    worksheet.Cell("G7").Value = $"{0}%";
            //    worksheet.Cell("F8").Value = 0;
            //    worksheet.Cell("G8").Value = $"{0}%";
            //    worksheet.Cell("F9").Value = 0;
            //    worksheet.Cell("G9").Value = $"{0}%";
            //    worksheet.Cell("F10").Value = 0;
            //    worksheet.Cell("G10").Value = $"{0}%";
            //    worksheet.Cell("F11").Value = 0;
            //    worksheet.Cell("G11").Value = $"{0}%";
            //    worksheet.Cell("F12").Value = 0;
            //    worksheet.Cell("G12").Value = $"{0}%";
            //    worksheet.Cell("F13").Value = 0;
            //    worksheet.Cell("G13").Value = $"{0}%";

            //    if (dados.MotivosNaoAverbada != null && dados.MotivosNaoAverbada.Count > 0)
            //    {
            //        foreach (var item in dados.MotivosNaoAverbada)
            //        {
            //            worksheet.Cell("F6").Value = "Total não averbados";
            //            worksheet.Cell("G6").Value = "%";
            //            worksheet.Cell("F7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
            //            worksheet.Cell("G7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? $"{item.TotalPorcentagem}&" : $"{0}%";
            //            worksheet.Cell("F8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
            //            worksheet.Cell("G8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("F9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("G9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("G10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("F10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("G11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}";
            //            worksheet.Cell("F11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("F12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorCodigoErro}%" : $"{0}";
            //            worksheet.Cell("G12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //        }
            //    }
            //    worksheet.Cell("F13").Value = dados.MotivosNaoAverbada.Count;
            //    worksheet.Cell("G13").Value = $"{dados.TaxaNaoAverbado}%";

            //    worksheet.Cell("A14").Value = "Detalhe de Produção";
            //    worksheet.Range("A14:G14").Merge();
            //    worksheet.Cell("A14").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
            //    worksheet.Cell("A14").Style.Font.Bold = true;
            //    worksheet.Cell("A14").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    worksheet.Cell("A15").Value = "Cod Externo";
            //    worksheet.Cell("B15").Value = "CPF";
            //    worksheet.Cell("C15").Value = "Nome";
            //    worksheet.Cell("D15").Value = "Data Adesão";
            //    worksheet.Cell("E15").Value = "Taxa Associativa";
            //    worksheet.Cell("F15").Value = "Status";
            //    worksheet.Cell("G15").Value = "Motivo";

            //    int row = 16;
            //    foreach (var item in dados.Relatorio)
            //    {
            //        worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
            //        worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber) ? cpfNumber : item.ClienteCpf;
            //        worksheet.Cell(row, 3).Value = item.ClienteNome;
            //        worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
            //        worksheet.Cell(row, 5).Value = decimal.TryParse(item.ValorDesconto.ToString("C"), out decimal valordesconto) ? valordesconto : item.ValorDesconto;
            //        worksheet.Cell(row, 6).Value = item.CodigoResultado == 1 ? "Averbado" : "Não Averbado";
            //        worksheet.Cell(row, 7).Value = item.DescricaoErro;
            //        row++;
            //    }

            //    var range = worksheet.Range("A15:G" + row);
            //    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    var outlineRange = worksheet.Range("A1:G" + row);
            //    outlineRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            //    outlineRange.Style.Border.OutsideBorderColor = XLColor.Blue;

            //    worksheet.Columns().AdjustToContents();
            //    worksheet.Column("A").Width = 18;
            //    worksheet.Column("B").Width = 15;
            //    worksheet.Column("C").Width = 40;
            //    worksheet.Column("D").Width = 18;
            //    worksheet.Column("E").Width = 18;
            //    worksheet.Column("F").Width = 18;
            //    worksheet.Column("G").Width = 38;

            //    worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            //    worksheet.PageSetup.PrintAreas.Add("A1:G" + row);
            //    worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

            //    workbook.SaveAs(caminhoArquivoSaida);
            //}
        }

        public void GerarArquivoRelatorioRetorno(GerarRelatoriResponse dados, string anomes)
        {
            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelRetorno.{anomes}.xlsx");
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }

            if (File.Exists(caminhoArquivoSaida))
                File.Delete(caminhoArquivoSaida);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório Retorno");

                int lastRow = 15 + dados.Relatorio.Count;

                var title = worksheet.Range("A1:H4");
                title.Merge();
                title.Value = "EXTRATO DE RETORNO DATAPREV";
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

                worksheet.Range("A5:H5").Merge();
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
                worksheet.Cell("B6").Value = $"{anomes.Substring(4, 2)}/{anomes.Substring(0, 4)}";

                worksheet.Cell("A7").Value = "Total Retornos:";
                worksheet.Cell("B7").Value = dados.QtdRegistro;

                worksheet.Cell("A8").Value = "Total Averbados";
                worksheet.Cell("B8").Value = dados.QtdAverbados;

                worksheet.Cell("A9").Value = "Total Excluido:";
                worksheet.Cell("B9").Value = dados.QtdExcluidos;

                worksheet.Cell("A10").Value = "Total Erro Automático:";
                worksheet.Cell("B10").Value = $"{dados.QtdAutomatico}%";

                // TEM QUE FAZER QUANDO TIVER UM ERRO FAZER O MAPEAMENTO
                int rowMotivoAuto = 11;
                foreach (var erroAutomatico in dados.MotivoAutomatico)
                {
                    worksheet.Cell($"A{rowMotivoAuto}").Value = $"({erroAutomatico.CodigoErro} - {erroAutomatico.DescricaoErro}):";
                    worksheet.Cell($"B{rowMotivoAuto}").Value = $"{erroAutomatico.TotalPorCodigoErro} ({erroAutomatico.TotalPorcentagem}%)";
                    rowMotivoAuto++;
                }

                worksheet.Cell($"A17").Value = "Taxa de Averbação:";
                worksheet.Cell($"B17").Value = $"{dados.TaxaAverbacao}%";
                // TEM QUE REVISAR ISSO

                var rangeD6G13 = worksheet.Range("D6:G13");
                rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
                var rangeF6G13 = worksheet.Range("F6:G13");
                rangeF6G13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell($"D6").Value = "Total Não averbados:";
                worksheet.Cell($"E6").Value = dados.QtdNaoAverbados;

                for (int i = 7; i < dados.MotivoNaoAverbada.Count + 7; i++)
                {
                    var naoAverbada = dados.MotivoNaoAverbada[i - 7];

                    worksheet.Cell($"D{i}").Value = $"({naoAverbada.CodigoErro} - {naoAverbada.DescricaoErro}):";
                    worksheet.Cell($"E{i}").Value = $"{naoAverbada.TotalPorCodigoErro} ({naoAverbada.TotalPorcentagem}%)";
                }

                worksheet.Cell("D17").Value = "Taxa Não averbado:";
                worksheet.Cell("E17").Value = $"{dados.TaxaNaoAverbacao}%";

                worksheet.Cell($"G6").Value = "Total Não excluídos:";
                worksheet.Cell($"H6").Value = dados.QtdNaoExcluidos;

                for (int i = 7; i < dados.MotivoNaoExclusao.Count + 7; i++)
                {
                    var naoExclusao = dados.MotivoNaoExclusao[i - 7];

                    worksheet.Cell($"G{i}").Value = $"({naoExclusao.CodigoErro} - {naoExclusao.DescricaoErro}):";
                    worksheet.Cell($"H{i}").Value = $"{naoExclusao.TotalPorCodigoErro} ({naoExclusao.TotalPorcentagem}%)";
                }

                worksheet.Cell($"G17").Value = "Taxa Não Exclusão:";
                worksheet.Cell($"H17").Value = $"{dados.TaxaNaoExcluido}%";

                worksheet.Cell("A18").Value = "Detalhe de Produção";
                worksheet.Range("A18:H18").Merge();
                worksheet.Cell("A18").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A18").Style.Font.Bold = true;
                worksheet.Cell("A18").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A19").Value = "Cod Externo";
                worksheet.Cell("B19").Value = "CPF";
                worksheet.Cell("C19").Value = "Nome";
                worksheet.Cell("D19").Value = "Data Adesão";
                worksheet.Cell("E19").Value = "Taxa Associativa";
                worksheet.Cell("F19").Value = "Status";
                worksheet.Cell("G19").Value = "Motivo";

                int row = 20;
                foreach (var item in dados.Relatorio)
                {
                    worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
                    worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber) ? cpfNumber : item.ClienteCpf;
                    worksheet.Cell(row, 3).Value = item.ClienteNome;
                    worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
                    worksheet.Cell(row, 5).Value = item.ValorDesconto;
                    worksheet.Cell(row, 6).Value = item.CodigoResultado == 1 ? "Averbado" : "Não Averbado";
                    worksheet.Cell(row, 7).Value = item.DescricaoErro;
                    row++;
                }

                var range = worksheet.Range("A15:H" + row);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var outlineRange = worksheet.Range("A1:H" + row);
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
                worksheet.PageSetup.PrintAreas.Add("A1:I" + row);
                worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

                workbook.SaveAs(caminhoArquivoSaida);
            }
        }

        public void GerarArquivoRelatorioRepasse(GerarRelatorioRepasseResponse dados, string anomes)
        {
            string diretorioBase = _env.ContentRootPath;
            string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelRepasse.{anomes}.xlsx");
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }

            if (File.Exists(caminhoArquivoSaida))
                File.Delete(caminhoArquivoSaida);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório Retorno");

                int lastRow = 15 + dados.Relatorio.Count;

                var title = worksheet.Range("A1:H4");
                title.Merge();
                title.Value = "EXTRATO DE REPASSE INSS";
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

                worksheet.Range("A5:H5").Merge();
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
                worksheet.Cell("B6").Value = $"{dados.Cabecalho.Competencia}";

                worksheet.Cell("A7").Value = "MÊS REPASSE:";
                worksheet.Cell("B7").Value = dados.Cabecalho.MesRepasse;

                worksheet.Cell("A8").Value = "Total Descontos";
                worksheet.Cell("B8").Value = dados.Cabecalho.TotalDescontos;

                worksheet.Cell("A9").Value = "Valor Total Repasse:";
                worksheet.Cell("B9").Value = dados.Cabecalho.ValorTotalRepasse;

                var rangeD6G13 = worksheet.Range("D6:G13");
                rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
                rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
                var rangeF6G13 = worksheet.Range("F6:G13");
                rangeF6G13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A12").Value = "Detalhe de Produção";
                worksheet.Range("A12:H12").Merge();
                worksheet.Cell("A12").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
                worksheet.Cell("A12").Style.Font.Bold = true;
                worksheet.Cell("A12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A13").Value = "N. Beneficio";
                worksheet.Cell("B13").Value = "CPF";
                worksheet.Cell("C13").Value = "Nome";
                worksheet.Cell("D13").Value = "Data Adesão";
                worksheet.Cell("E13").Value = "Taxa Associativa";
                worksheet.Cell("F13").Value = "Parcela";

                int row = 14;
                foreach (var item in dados.Relatorio)
                {
                    worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
                    worksheet.Cell(row, 2).Value = long.TryParse(item.Cpf, out long cpfNumber) ? cpfNumber : item.Cpf;
                    worksheet.Cell(row, 3).Value = item.Nome;
                    worksheet.Cell(row, 4).Value = item.DataAdesao;
                    worksheet.Cell(row, 5).Value = item.TaxaAssociativa;
                    worksheet.Cell(row, 6).Value = item.Parcela;
                    row++;
                }

                var range = worksheet.Range("A14:H" + row);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var outlineRange = worksheet.Range("A1:H" + row);
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
                worksheet.PageSetup.PrintAreas.Add("A1:I" + row);
                worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

                workbook.SaveAs(caminhoArquivoSaida);
            }
        }

        public BuscarArquivoResponse BuscarArquivoRelatorio(string anomes, int tiporel)
        {
            if (tiporel == 1)
            {
                var relatorio = GerarRelatorioRetorno(anomes, 0);

                GerarArquivoRelatorioRetorno(relatorio, anomes);
            }
            else
            {
                var relatorio = (GerarRelatorioRepasseResponse)GerarRelatorioRepasse(anomes, 0);

                GerarArquivoRelatorioRepasse(relatorio, anomes);
            }

            if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Remessa")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Remessa"))); }
            string diretorioBase = Path.Combine(_env.ContentRootPath, "Relatorio");
            var path = string.Empty;

            string[] todosLogs = Directory.GetFiles(diretorioBase);
            if (tiporel == 1)
            {
                path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"RelRetorno.{anomes}.xlsx"));
            }
            else
            {
                path = todosLogs.FirstOrDefault(arquivo => Path.GetFileName(arquivo).Contains($"RelRepasse.{anomes}.xlsx"));
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
            //var captador = captadorId > 0 ? _mysql.captadores.First(x => x.captador_id == captadorId) : new() { captador_nome = "TODOS" };

            //string diretorioBase = _env.ContentRootPath;
            //string caminhoArquivoSaida = Path.Combine(diretorioBase, "Relatorio", $"RelCarteira.{anomes}.xlsx");
            //if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Relatorio"))); }
            //if (!Directory.Exists(Path.Combine(string.Join(_env.ContentRootPath, "Imagens")))) { Directory.CreateDirectory(Path.Combine(string.Join(_env.ContentRootPath, "Imagens"))); }
            //var dados = GerarRelatorioRetorno(anomes, captadorId);

            //if (File.Exists(caminhoArquivoSaida))
            //    File.Delete(caminhoArquivoSaida);

            //using (var workbook = new XLWorkbook())
            //{
            //    var worksheet = workbook.Worksheets.Add("Relatório Carteira");

            //    int lastRow = 15 + dados.Relatorio.Count;

            //    var title = worksheet.Range("A1:I4");
            //    title.Merge();
            //    title.Value = "EXTRATO DE REPASSE DATAPREV";
            //    title.Style.Font.Bold = true;
            //    title.Style.Font.FontSize = 16;
            //    title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //    title.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //    string caminhoImagem = Path.Combine(diretorioBase, "Imagens", "logo.png");
            //    if (Directory.GetFiles(Path.Combine(_env.ContentRootPath, "Imagens")).Any(file => Path.GetFileName(file).Contains($"logo.png")))
            //    {
            //        var imagem = worksheet.AddPicture(caminhoImagem ?? "")
            //                    .MoveTo(worksheet.Cell("G1"))
            //                    .WithSize((int)(8.16 * 28.3465), (int)(2.83 * 28.3465));
            //    }

            //    worksheet.Range("A5:I5").Merge();
            //    worksheet.Cell("A5").Value = "Resumo de Produção";
            //    worksheet.Cell("A5").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
            //    worksheet.Cell("A5").Style.Font.Bold = true;
            //    worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    var rangeA6C13 = worksheet.Range("A6:C13");
            //    rangeA6C13.Style.Border.LeftBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.RightBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.TopBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Border.BottomBorder = XLBorderStyleValues.None;
            //    var rangeAC = worksheet.Range("C13:C13");
            //    rangeAC.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    worksheet.Cell("A6").Value = "COMPETENCIA:";
            //    worksheet.Cell("B6").Value = long.TryParse(dados.Detalhes.Competencia, out var detalhes) ? detalhes : 0;
            //    worksheet.Cell("A7").Value = "CORRETORA:";
            //    worksheet.Cell("B7").Value = captador.captador_nome;
            //    worksheet.Cell("A9").Value = "Carteira:";
            //    worksheet.Cell("B9").Value = "Qtde total";
            //    worksheet.Cell("C9").Value = dados.Relatorio.Count;
            //    worksheet.Cell("B10").Value = "Cancelados";
            //    worksheet.Cell("C10").Value = dados.Relatorio.Count(x => x.Status == "Excluido");
            //    worksheet.Cell("B11").Value = "Inadimplentes";
            //    worksheet.Cell("C11").Value = dados.Relatorio.Count(x => x.Status == "Sem desconto");
            //    worksheet.Cell("B12").Value = "Em dia";
            //    worksheet.Cell("C12").Value = dados.Relatorio.Count(x => x.Status == "Pago");

            //    var rangeD6G13 = worksheet.Range("D6:I13");
            //    rangeD6G13.Style.Border.LeftBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.RightBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.TopBorder = XLBorderStyleValues.None;
            //    rangeD6G13.Style.Border.BottomBorder = XLBorderStyleValues.None;
            //    rangeA6C13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //    rangeA6C13.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //    worksheet.Cell("D6").Value = "Motivos não averbados";
            //    worksheet.Cell("D7").Value = "002 - Espécie incompatível";
            //    worksheet.Cell("D8").Value = "004 - NB inexistente no cadastro";
            //    worksheet.Cell("D9").Value = "005 - Benefício não ativo";
            //    worksheet.Cell("D10").Value = "006 - Valor ultrapassa MR do titular";
            //    worksheet.Cell("D11").Value = "008 - Já existe desc. p/ outra entidade";
            //    worksheet.Cell("D12").Value = "012 - Benefício bloqueado para desconto";
            //    worksheet.Cell("D13").Value = "Total Não averbado";

            //    worksheet.Cell("G7").Value = 0;
            //    worksheet.Cell("H7").Value = $"{0}%";
            //    worksheet.Cell("G7").Value = 0;
            //    worksheet.Cell("H8").Value = $"{0}%";
            //    worksheet.Cell("G9").Value = 0;
            //    worksheet.Cell("H9").Value = $"{0}%";
            //    worksheet.Cell("G10").Value = 0;
            //    worksheet.Cell("H10").Value = $"{0}%";
            //    worksheet.Cell("G11").Value = 0;
            //    worksheet.Cell("H11").Value = $"{0}%";
            //    worksheet.Cell("G12").Value = 0;
            //    worksheet.Cell("GH12").Value = $"{0}%";
            //    worksheet.Cell("G13").Value = 0;
            //    worksheet.Cell("H13").Value = $"{0}%";

            //    if (dados.MotivosNaoAverbada != null && dados.MotivosNaoAverbada.Count > 0)
            //    {
            //        foreach (var item in dados.MotivosNaoAverbada)
            //        {
            //            worksheet.Cell("G6").Value = "Total não averbados";
            //            worksheet.Cell("H6").Value = "%";
            //            worksheet.Cell("G7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
            //            worksheet.Cell("H7").Value = item.CodigoErro == "2".PadLeft(3, '0') ? $"{item.TotalPorcentagem}&" : $"{0}%";
            //            worksheet.Cell("G8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0; ;
            //            worksheet.Cell("H8").Value = item.CodigoErro == "4".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("G9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("H9").Value = item.CodigoErro == "5".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("G10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //            worksheet.Cell("H10").Value = item.CodigoErro == "6".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("G11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}";
            //            worksheet.Cell("H11").Value = item.CodigoErro == "8".PadLeft(3, '0') ? item.TotalPorCodigoErro : 0;
            //            worksheet.Cell("G12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorCodigoErro}%" : $"{0}";
            //            worksheet.Cell("H12").Value = item.CodigoErro == "12".PadLeft(3, '0') ? $"{item.TotalPorcentagem}%" : $"{0}%";
            //        }
            //    }
            //    worksheet.Cell("F13").Value = dados.MotivosNaoAverbada.Count;
            //    worksheet.Cell("G13").Value = $"{dados.TaxaNaoAverbado}%";

            //    worksheet.Cell("A14").Value = "Detalhe de Produção";
            //    worksheet.Range("A14:I14").Merge();
            //    worksheet.Cell("A14").Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247);
            //    worksheet.Cell("A14").Style.Font.Bold = true;
            //    worksheet.Cell("A14").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    worksheet.Cell("A15").Value = "NB/Matrícula";
            //    worksheet.Cell("B15").Value = "CPF";
            //    worksheet.Cell("C15").Value = "Nome";
            //    worksheet.Cell("D15").Value = "Data Adesão";
            //    worksheet.Cell("E15").Value = "Taxa Associativa";
            //    worksheet.Cell("F15").Value = "Parcela Atual";
            //    worksheet.Cell("G15").Value = "Data Pagameto";
            //    worksheet.Cell("H15").Value = "Status";
            //    worksheet.Cell("I15").Value = "Motivo";

            //    int row = 16;
            //    foreach (var item in dados.Relatorio)
            //    {
            //        worksheet.Cell(row, 1).Value = long.TryParse(item.CodExterno, out long codexterno) ? codexterno : item.CodExterno;
            //        worksheet.Cell(row, 2).Value = long.TryParse(item.ClienteCpf, out long cpfNumber) ? cpfNumber : item.ClienteCpf;
            //        worksheet.Cell(row, 3).Value = item.ClienteNome;
            //        worksheet.Cell(row, 4).Value = item.DataInicioDesconto;
            //        worksheet.Cell(row, 5).Value = decimal.TryParse(item.ValorDesconto.ToString("C"), out decimal valordesconto) ? valordesconto : item.ValorDesconto;
            //        worksheet.Cell(row, 6).Value = item.QuantidadeParcelas;
            //        worksheet.Cell(row, 7).Value = item.DataPagamento;
            //        worksheet.Cell(row, 8).Value = item.Status;
            //        worksheet.Cell(row, 9).Value = item.DescricaoErro;
            //        row++;
            //    }

            //    var range = worksheet.Range("A15:I" + row);
            //    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //    var outlineRange = worksheet.Range("A1:I" + row);
            //    outlineRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            //    outlineRange.Style.Border.OutsideBorderColor = XLColor.Blue;

            //    worksheet.Columns().AdjustToContents();
            //    worksheet.Column("A").Width = 18;
            //    worksheet.Column("B").Width = 15;
            //    worksheet.Column("C").Width = 40;
            //    worksheet.Column("D").Width = 15;
            //    worksheet.Column("E").Width = 15;
            //    worksheet.Column("F").Width = 15;
            //    worksheet.Column("G").Width = 15;
            //    worksheet.Column("H").Width = 15;
            //    worksheet.Column("I").Width = 25;

            //    worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            //    worksheet.PageSetup.PrintAreas.Add("A1:I" + row);
            //    worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

            //    workbook.SaveAs(caminhoArquivoSaida);
            //}
        }

        public object GerarRelatorioRepasse(string anomes, int captadorId)
        {
            try
            {
                var repasse = _mysql.retorno_financeiro.FirstOrDefault(x => x.ano_mes == anomes)
                    ?? throw new Exception("Não existe repasse para mês/ano competente");

                var clientes = from ret in _mysql.registro_retorno_financeiro
                               join cli in _mysql.clientes
                                   on ret.numero_beneficio equals cli.cliente_matriculaBeneficio into cliGroup
                               from cli in cliGroup.DefaultIfEmpty()
                               join rr in _mysql.registros_retorno_remessa
                                   on repasse.retorno_id equals rr.Id into retornoGroup
                               from rr in retornoGroup.DefaultIfEmpty()
                               where ret.retorno_financeiro_id == repasse.retorno_financeiro_id
                               select new
                               {
                                   Cliente = cli ?? new ClienteDb
                                   {
                                       cliente_matriculaBeneficio = ret.numero_beneficio,
                                       cliente_cpf = "-",
                                       cliente_nome = "CLIENTE NAO ENCONTRADO NA BASE",
                                   },
                                   Registro = ret,
                                   Retorno = rr
                               };

                var relatorio = clientes.ToList().Select(x => new RelatorioRepasse
                {
                    CodExterno = x.Cliente.cliente_matriculaBeneficio,
                    Cpf = x.Cliente.cliente_cpf,
                    Nome = x.Cliente.cliente_nome,
                    DataAdesao = x.Retorno?.Data_Inicio_Desconto,
                    Valor = FormatarValorDescontado(x.Registro.desconto.Value),
                    TaxaAssociativa = FormatarValorDescontado(x.Registro.desconto.Value).ToString("C", new CultureInfo("pt-BR")),
                    Parcela = x.Registro.parcela
                }).ToList();

                var mesRepasse = repasse.nome_arquivo.Replace("D.SUB.GER.177.REP.", "");

                DateTime data = DateTime.ParseExact(mesRepasse, "yyyyMM", CultureInfo.InvariantCulture);

                string dataFormatada = data.ToString("MMM/yy", new CultureInfo("pt-BR"));

                var cabecalho = new CabecalhoRepasse
                {
                    Competencia = $"{repasse.ano_mes.Substring(4, 2)}/{repasse.ano_mes.Substring(0, 4)}",
                    MesRepasse = dataFormatada,
                    TotalDescontos = relatorio.Count(),
                    ValorTotalRepasse = relatorio.Sum(x => x.Valor).ToString("C", new CultureInfo("pt-BR"))
                };

                return new GerarRelatorioRepasseResponse
                {
                    Relatorio = relatorio,
                    Cabecalho = cabecalho
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private decimal FormatarValorDescontado(decimal desconto)
        {
            if (desconto > 9)
            {

                var desc = desconto.ToString().Replace(".", "").Replace(",", "");

                var integerPart = desc.Length > 2 ? desc.Substring(0, 2) : desc.PadLeft(2, '0');
                var fractionalPart = desc.Length > 4 ? desc.Substring(2, 2) : "00";

                var valorFormatado = $"{integerPart}.{fractionalPart}";

                decimal valorConvertido = decimal.Parse(valorFormatado, CultureInfo.InvariantCulture);

                return valorConvertido;
            }
            else
            {
                var desc = desconto.ToString().Replace(".", "").Replace(",", "");

                var integerPart = desc.Substring(0,1);
                var fractionalPart = desc.Substring(1, 2);

                var valorFormatado = $"{integerPart}.{fractionalPart}";

                decimal valorConvertido = decimal.Parse(valorFormatado, CultureInfo.InvariantCulture);

                return valorConvertido;

            }
        }

    }
}
