using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IRemessa
    {
        List<BuscarTodasRemessas> BuscarTodasRemessas();
        RetornoRemessa GerarRemessa(int mes, int ano);
        bool RemessaExiste(int mes, int ano);
        string GerarArquivoRemessa(int idRegistro, int mes, int ano);
    }
}
