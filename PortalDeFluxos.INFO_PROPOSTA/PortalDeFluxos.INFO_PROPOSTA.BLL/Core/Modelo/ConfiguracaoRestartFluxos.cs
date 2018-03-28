using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [Serializable]
    public class ConfiguracaoRestartFluxos
    {
        public Int32 NumeroTentativasInicio { get; set; }
        public Int32 TempoMinutosSucesso { get; set; }
    }
}
