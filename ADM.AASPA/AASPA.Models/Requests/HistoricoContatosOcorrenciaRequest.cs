﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Requests
{
    public class HistoricoContatosOcorrenciaRequest
    {
        public int HistoricoContatosOcorrenciaId { get; set; } = 0;
        public int HistoricoContatosOcorrenciaOrigemId { get; set; }
        public int HistoricoContatosOcorrenciaClienteId { get; set; }
        public int HistoricoContatosOcorrenciaMotivoContatoId { get; set; }
        public DateTime HistoricoContatosOcorrenciaDtOcorrencia { get; set; }
        public string HistoricoContatosOcorrenciaDescricao { get; set; }
        public string HistoricoContatosOcorrenciaSituacaoOcorrencia { get; set; }
        public string? HistoricoContatosOcorrenciaBanco { get; set; }
        public string? HistoricoContatosOcorrenciaAgencia { get; set; }
        public string? HistoricoContatosOcorrenciaTipoConta { get; set; }
        public string? HistoricoContatosOcorrenciaConta { get; set; }
        public string? HistoricoContatosOcorrenciaDigito { get; set; }
        public string? HistoricoContatosOcorrenciaPix { get; set; }
        public string? HistoricoContatosOcorrenciaTipoChavePix { get; set; }
        public string? HistoricoContatosOcorrenciaTelefone { get; set; }
        public List<IFormFile> HistoricoContatosOcorrenciaAnexos { get; set; }
        public string HistoricoContatosOcorrenciaValorReembolso { get; set; }
        public string HistoricoContatosOcorrenciaValorParcela { get; set; }
        public string HistoricoContatosOcorrenciaValorParcela2 { get; set; }
    }
}
