using AASPA.Domain.Interface;
using AASPA.Models.Response;
using AASPA.Repository;
using NuGet.Packaging.Signing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class RepasseService : IRepasse
    {
        private readonly MysqlContexto _mysql;
        public RepasseService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public List<BuscarArquivosResponse> BuscarTodosRepasses(int? ano, int? mes)
        {
            try
            {
                var respasses = (from rep in _mysql.retorno_financeiro
                                 join ret in _mysql.retornos_remessa on rep.retorno_id equals ret.Retorno_Id
                                 join rem in _mysql.remessa on rep.remessa_id equals rem.remessa_id
                                 select new BuscarArquivosResponse
                                 {
                                     DataImportacao = rep.data_importacao.ToString("dd/MM/yyyy hh:mm:ss"),
                                     NomeRemessaCompetente = rem.nome_arquivo_remessa,
                                     NomeRetornoCompetente = ret.Nome_Arquivo_Retorno,
                                     NomeRepasseCompetente = rep.nome_arquivo,
                                     RepasseId = rep.retorno_financeiro_id
                                 });

                return respasses.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
