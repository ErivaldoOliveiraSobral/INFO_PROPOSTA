using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class AtivosSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualBombas { get; set; }

        public string ATIV_AcaoBombas { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoBombas { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualTanques { get; set; }

        public string ATIV_AcaoTanques { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoTanques { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualRVI { get; set; }

        public string ATIV_AcaoRVI { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoRVI { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualCivil { get; set; }

        public string ATIV_AcaoCivil { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoCivil { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualCTO { get; set; }

        public string ATIV_AcaoCTO { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoCTO { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualTerreno { get; set; }

        public string ATIV_AcaoTerreno { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoTerreno { get; set; }

        public Nullable<System.Decimal> ATIV_ValorResidualTotal { get; set; }

        public Nullable<System.Decimal> ATIV_ValorNegociadoTotal { get; set; }
    }
}
