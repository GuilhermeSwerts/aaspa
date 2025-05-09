﻿using AASPA.Models.Requests;
using AASPA.Models.Response;
using AASPA.Repository.Maps;
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
        RetornoRemessaResponse GerarRemessa(int mes, int ano, DateTime dateInit, DateTime dateEnd);
        string GerarArquivoRemessa(int idRegistro, int mes, int ano, string nomeArquivo);
        BuscarArquivoResponse BuscarArquivo(int remessaId);
        Task<string> LerRetorno(IFormFile file, int usuarioLogadoId);
        Task<string> LerRetornoRepasse(IFormFile file, int usuarioLogadoId);
        List<BuscarArquivosResponse> BuscarRetorno(int mes, int ano);
        object GetBuscarRepasse(int? mes, int? ano);
        (List<ClienteDb> Clientes, int QtdPaginas, int TotalClientes) BuscarClientesElegivel(ConsultaParametros request);
    }
}
