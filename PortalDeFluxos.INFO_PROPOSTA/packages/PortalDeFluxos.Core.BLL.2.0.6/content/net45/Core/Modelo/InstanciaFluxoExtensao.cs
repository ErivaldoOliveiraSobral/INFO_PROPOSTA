using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class InstanciaFluxo
    {
        public String HistoricoFluxo { get; set; }

        public Int32? TotalRecordCount { get; set; }

        public String NomeLista { get; set; }

        public String NomeStatusFluxo
        {
            get
            {
                return this.StatusFluxo != null ? NegocioComum.GetTitleFromEnum<StatusFluxo>((Int32)this.StatusFluxo) : "";
            }
        }
    }
}
