using AASPA.Repository.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface ILog
    {
        object GetLogAlteracao(int tabelaFk);
        void NovaAlteracao(string titulo, string log, int usuarioId, ETipoLog tipo,int fk);
    }
}
