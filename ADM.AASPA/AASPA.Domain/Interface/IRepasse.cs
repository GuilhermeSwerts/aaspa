﻿using AASPA.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Interface
{
    public interface IRepasse
    {
        List<BuscarArquivosResponse> BuscarTodosRepasses(int? ano, int? mes);
    }
}
