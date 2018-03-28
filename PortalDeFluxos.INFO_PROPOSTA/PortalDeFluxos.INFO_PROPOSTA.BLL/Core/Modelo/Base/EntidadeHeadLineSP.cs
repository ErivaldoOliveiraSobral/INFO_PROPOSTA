using PortalDeFluxos.Core.BLL.Atributos;
using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class EntidadeHeadlineSP : EntidadeSP
    {
        [InternalName("HeadlineInicial")]
        public Int32? HeadlineSizeInicial { get; set; }

        [InternalName("HeadlineFinal")]
        public Int32? HeadlineSizeFinal { get; set; }

        [InternalName("Aprovadores")]
        public List<ListaSP_RaizenAprovacoesHeadlineSize> Aprovadores { get; set; }

    }
}
