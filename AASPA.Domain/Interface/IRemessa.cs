using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IRemessa
    {
        int GerarRemessa(int mes, int ano);
        bool RemessaExiste(int mes, int ano);
    }
}
