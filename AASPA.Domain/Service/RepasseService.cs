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
                var respasses = from rep in _mysql.retorno_financeiro
                                join ret in _mysql.retornos_remessa on rep.retorno_id equals ret.Retorno_Id into retGroup
                                from ret in retGroup.DefaultIfEmpty()
                                join rem in _mysql.remessa on rep.remessa_id equals rem.remessa_id into remGroup
                                from rem in remGroup.DefaultIfEmpty()
                                select new BuscarArquivosResponse
                                {
                                    DataImportacao = rep.data_importacao.ToString("dd/MM/yyyy hh:mm:ss"),
                                    NomeRemessaCompetente = rem != null ? rem.nome_arquivo_remessa : null,
                                    NomeRetornoCompetente = ret != null ? ret.Nome_Arquivo_Retorno : null,
                                    NomeRepasseCompetente = rep.nome_arquivo,
                                    RepasseId = rep.retorno_financeiro_id
                                };

                return respasses.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
