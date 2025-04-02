using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Util
{

    public class Comparador
    {
        public static (List<string> CamposAlterados, List<string> Log) CompararObjetos<T>(T antigo, T novo)
        {
            var tipo = typeof(T);
            PropertyInfo[] propriedades = tipo.GetProperties();
            
            List<string> camposAlterados = new();
            List<string> log = new();

            foreach (var propriedade in propriedades)
            {
                var valorAntigo = propriedade.GetValue(antigo);
                var valorNovo = propriedade.GetValue(novo);

                if (valorAntigo != null && !valorAntigo.Equals(valorNovo))
                {
                    camposAlterados.Add(propriedade.Name.ToString());
                    log.Add($"{propriedade.Name} de {valorAntigo} para {valorNovo}");
                }
            }

            return (camposAlterados, log);
        }
    }
}
