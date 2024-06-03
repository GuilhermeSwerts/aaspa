using AASPA.Repository.Maps;
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
    }
}
