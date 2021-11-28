using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Funcoes
{
    public class Vetores
    {
        public string VetorComoTexto(string[] vetor)
        {
            string retorno = "";

            foreach (var item in vetor)
            {
                retorno = retorno + item + " - ";
            }

            // REMOVE ULTIMOS CARACTERES " - "
            retorno = retorno.Remove(retorno.Length - 3);

            return retorno;
        }
    }
}