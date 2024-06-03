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
    public class MotivoContatoService : IMotivoContato
    {
        private readonly MysqlContexto _mysql;
        public MotivoContatoService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object BuscarMotivosById(int motivoId)
        {
            return _mysql.motivo_contato.FirstOrDefault(x => x.motivo_contato_id == motivoId)??
                throw new Exception("Motivo do contato não encontrado.");
        }

        public object BuscarTodosMotivos()
        {
            return _mysql.motivo_contato.Where(x => x.motivo_contato_id > 0).ToList();
        }

        public void EditarMotivo(MotivoContatoRequest motivoContatoRequest)
        {
            var motivo = _mysql.motivo_contato.FirstOrDefault(x => x.motivo_contato_id == motivoContatoRequest.MotivoId)
            ?? throw new Exception("Motivo do contato não encontrado.");

            motivo.motivo_contato_nome = motivoContatoRequest.NomeMotivo.ToUpper();
            _mysql.SaveChanges();
        }

        public void NovoMotivo(string nomeMotivo)
        {
            if (_mysql.motivo_contato.Any(x => x.motivo_contato_nome.ToUpper() == nomeMotivo.ToUpper()))
                throw new Exception("Motivo do contato já cadastrado.");

            _mysql.motivo_contato.Add(new Repository.Maps.MotivoContatoDb
            {
                motivo_contato_nome = nomeMotivo.ToUpper(),
            });

            _mysql.SaveChanges();
        }
    }
}
