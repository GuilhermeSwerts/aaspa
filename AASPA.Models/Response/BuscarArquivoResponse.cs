﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.Response
{
    public class BuscarArquivoResponse
    {
        public string NomeArquivo { get; set; }
        public byte[] Bytes { get; set; }
    }
}
