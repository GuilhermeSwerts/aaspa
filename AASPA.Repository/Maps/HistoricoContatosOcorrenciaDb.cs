﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Repository.Maps
{
    public class HistoricoContatosOcorrenciaDb
    {
        [Key]
        public int historico_contatos_ocorrencia_id { get; set; }
        public int historico_contatos_ocorrencia_origem_id { get; set; }
        public int historico_contatos_ocorrencia_cliente_id { get; set; }
        public int historico_contatos_ocorrencia_motivo_contato_id { get; set; }
        public DateTime historico_contatos_ocorrencia_dt_ocorrencia { get; set; }
        public string historico_contatos_ocorrencia_descricao { get; set; }
        public string historico_contatos_ocorrencia_situacao_ocorrencia { get; set; }
        public DateTime historico_contatos_ocorrencia_dt_cadastro { get; set; } = DateTime.Now;
    }
}
