using AASPA.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IRemessa
    {
        List<BuscarTodasRemessas> BuscarTodasRemessas(int? ano, int? mes);
        RetornoRemessaResponse GerarRemessa(int mes, int ano);
        bool RemessaExiste(int mes, int ano);
        string GerarArquivoRemessa(int idRegistro, int mes, int ano, string nomeArquivo);
        BuscarArquivoResponse BuscarArquivo(int remessaId);
        Task<string> LerRetorno(IFormFile file);
        Task<string> LerRetornoRepasse(IFormFile file);
        BuscarRetornoResponse BuscarRetorno(int mes, int ano);
    }
}
