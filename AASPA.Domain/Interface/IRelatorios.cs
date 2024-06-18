using System;

namespace AASPA.Controllers
{
    public interface IRelatorios
    {
        object GerarRelatorioAverbacao(DateTime inicio, DateTime fim);
    }
}