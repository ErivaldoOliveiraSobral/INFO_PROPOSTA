using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class ParecerCreditoSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public string CRED_FiadoresParecerCredito { get; set; }

        public string CRED_SociosParecerCredito { get; set; }

        public string CRED_MutuoFaseadoParecerCredito { get; set; }

        public Nullable<System.Int32> CRED_NumeroParcelasParecerCredito { get; set; }

        public string CRED_CondicoesParecerCredito { get; set; }
    }
}
