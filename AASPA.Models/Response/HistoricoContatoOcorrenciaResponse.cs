﻿using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class HistoricoContatoOcorrenciaResponse
    {
        public int Id { get; set; }
        public string Origem { get; set; }
        public string DataHoraOcorrencia { get; set; }
        public string DescricaoDaOcorrência { get; set; }
        public string MotivoDoContato { get; set; }
        public string SituacaoOcorrencia { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Digito { get; set; }
        public string Pix { get; set; }
        public string Usuario { get; set; }
        public string UltimoUsuario { get; set; }
    }
}
