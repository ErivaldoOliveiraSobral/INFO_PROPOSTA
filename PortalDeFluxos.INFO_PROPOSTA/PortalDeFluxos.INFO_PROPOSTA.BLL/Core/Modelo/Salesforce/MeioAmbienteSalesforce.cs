using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class MeioAmbienteSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public string AMBI_ComentariosSSMA { get; set; }

        public string AMBI_TermoCooperacaoSSMA { get; set; }

        public Nullable<System.Decimal> AMBI_ValorRemediadoSSMA { get; set; }

        public string AMBI_PropostaCooperacaoSSMA { get; set; }

        public string AMBI_MutuoFaseadoSSMA { get; set; }

        public string AMBI_FormaPagamento { get; set; }
    }
}
