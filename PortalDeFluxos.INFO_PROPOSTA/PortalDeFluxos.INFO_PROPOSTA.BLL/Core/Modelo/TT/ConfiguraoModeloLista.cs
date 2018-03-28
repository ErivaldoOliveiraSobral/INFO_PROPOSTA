using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo.T4
{
    public class ConfiguraoModeloLista
    {
        public String UrlSite { get; set; }

        public List<ModeloListaSP> ModelosListaSP { get; set; }
    }

    public class ModeloListaSP
    {
        /// <summary>
        /// Display name da lista  automatico
        /// </summary>
        public String NomeLista { get; set; }

        public Boolean ListaProposta { get; set; }

        /// <summary>
        /// Campos que não reinicia o fluxo automaticamente
        /// </summary>
        public List<String> NaoReiniciar { get; set; }
    }
}
