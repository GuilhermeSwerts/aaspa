using AASPA.Models.Response;
using System;

namespace AASPA.Controllers
{
    public interface IRelatorios
    {
        GerarRelatorioAverbacaoResponse GerarRelatorioAverbacao(string anomes);
    }
}