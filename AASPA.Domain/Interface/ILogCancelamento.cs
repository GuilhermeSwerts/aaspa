using AASPA.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface ILogCancelamento
    {
        void Logger(AlterarStatusClientesIntegraallRequest request, string? destino, int? status, string? erro);
    }
}
