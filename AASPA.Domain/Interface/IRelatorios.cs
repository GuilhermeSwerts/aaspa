using AASPA.Models.Response;
using System;

namespace AASPA.Controllers
{
    public interface IRelatorios
    {
        GerarRelatorioAverbacaoResponse GerarRelatorioAverbacao(string anomes);
        void GerarArquivoRelatorioAverbacao(string anomes);
        BuscarArquivoResponse BuscarArquivoAverbacao(string anomes);
    }
}