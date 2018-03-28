using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    public class PropostaInvestimentoSalesforce
    {
        public PropostaInvestimentoVarejoSalesforce ItemInvestimentoVarejo { get; set; }

        public ComentariosSalesforce ItemComentarios { get; set; }

        public ContratosSalesforce ItemContrato { get; set; }

        public CasoInvestimentoSalesforce ItemCasoInvestimento { get; set; }

        public CasoBaseSalesforce ItemCasoBase { get; set; }

        public AtivosSalesforce ItemAtivos { get; set; }

        public ConvenienciaSalesforce ItemConveniencia { get; set; }

        public MeioAmbienteSalesforce ItemMeioAmbiente { get; set; }

        public ParecerCreditoSalesforce ItemParecerCredito { get; set; }

        public ResultadosEconomicosSalesforce ItemResultadosEconomicos { get; set; }
    }
}
