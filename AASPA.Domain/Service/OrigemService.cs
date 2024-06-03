using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class OrigemService : IOrigem
    {
        private readonly MysqlContexto _mysql;
        public OrigemService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object BuscarOrigemById(int origemId)
        {
            return _mysql.origem.FirstOrDefault(x => x.origem_id == origemId) ??
            throw new Exception("Origem não encontrado.");
        }

        public object BuscarTodasOrigem()
        {
            return _mysql.origem.Where(x => x.origem_id > 0).ToList();
        }

        public void EditarOrigem(OrigemRequest origemRequest)
        {
            var motivo = _mysql.origem.FirstOrDefault(x => x.origem_id == origemRequest.OrigemId)
           ?? throw new Exception("Origem não encontrado.");

            motivo.origem_nome = origemRequest.NomeOrigem.ToUpper();
            _mysql.SaveChanges();
        }

        public void NovaOrigem(string nomeOrigem)
        {
            if (_mysql.origem.Any(x => x.origem_nome.ToUpper() == nomeOrigem.ToUpper()))
                throw new Exception("Origem já cadastrado.");

            _mysql.origem.Add(new Repository.Maps.OrigemDb
            {
                origem_nome = nomeOrigem.ToUpper(),
            });

            _mysql.SaveChanges();
        }
    }
}
