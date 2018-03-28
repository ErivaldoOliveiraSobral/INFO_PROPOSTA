using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class ConfiguracaoEmailAttachment
    {
        public String NomeLista { get; set; }

        public String SiteAnexo { get; set; }

        public Int32 TipoAnexo { get; set; }

        public Int32 TamanhoLimite { get; set; }

        public Dictionary<String, String> Mapeamento { get; set; }
    }
}
