using AASPA.Models.Response;
using System;

namespace AASPA.Controllers
{
    public interface IRelatorios
    {
        GerarRelatorioAverbacaoResponse GerarRelatorioAverbacao(string anomes, int captadorId);
        void GerarArquivoRelatorioAverbacao(string anomes, int captadorId);
        BuscarArquivoResponse BuscarArquivoRelatorio(string anomes, int tiporel);
        void GerarArquivoRelatorioCarteiras(string anomes, int captadorId);
    }
}