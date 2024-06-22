using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Repository;
using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class CaptadorService : ICaptador
    {
        private readonly MysqlContexto _mysql;
        public CaptadorService(MysqlContexto mysql)
        {
            _mysql = mysql;
        }

        public object BuscarCaptadores()
        {
            return _mysql.captadores.Where(x => x.captador_situacao).ToList();
        }

        public void EditarCaptador(NovoCaptador captadorRequest)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_id == captadorRequest.CaptadorId)
                    ?? throw new Exception("Captador não encontrado.");

                string captadorCpfCnpj = captadorRequest.CpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                captador.captador_cpf_cnpj = captadorCpfCnpj;
                captador.captador_nome = captadorRequest.Nome;
                captador.captador_descricao = captadorRequest.Descricao ?? "";
                captador.captador_e_cnpj = captadorCpfCnpj.Length > 11;

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void NovoCaptador(NovoCaptador novoCaptador)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                string captadorCpfCnpj = novoCaptador.CpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_cpf_cnpj == captadorCpfCnpj);
                if (captador != null && captador.captador_situacao)
                    throw new Exception($"Captador do CNPJ {novoCaptador.CpfOuCnpj}, já cadastrado!");

                if (captador != null && !captador.captador_situacao)
                {
                    captador.captador_nome = novoCaptador.Nome;
                    captador.captador_descricao = novoCaptador.Descricao ?? "";
                    captador.captador_situacao = true;
                    _mysql.SaveChanges();
                    tran.Commit();
                }
                else
                {
                    captador = new CaptadorDb
                    {
                        captador_cpf_cnpj = captadorCpfCnpj,
                        captador_nome = novoCaptador.Nome,
                        captador_descricao = novoCaptador.Descricao ?? "",
                        captador_e_cnpj = captadorCpfCnpj.Length > 11,
                        captador_situacao = true,
                    };
                    _mysql.captadores.Add(captador);
                    _mysql.SaveChanges();
                    tran.Commit();
                }
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public void DeletarCaptador(int captadorId)
        {
            using var tran = _mysql.Database.BeginTransaction();
            try
            {
                var captador = _mysql.captadores.FirstOrDefault(x => x.captador_id == captadorId)
                    ?? throw new Exception("Captador não encontrado.");

                captador.captador_situacao = false;

                _mysql.SaveChanges();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

    }
}
