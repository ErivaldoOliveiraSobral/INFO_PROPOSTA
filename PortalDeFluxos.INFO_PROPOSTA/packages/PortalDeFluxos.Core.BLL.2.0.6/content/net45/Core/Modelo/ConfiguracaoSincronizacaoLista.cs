using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class ConfiguracaoSincronizarLista
    {
        public String NomeTabela { get; set; }

        public String NomeLista { get; set; }

        public Dictionary<String, String> Mapeamento { get; set; }
    }
}
