using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.SAE
{
    [Serializable]
    public class ResultadoEconomicoSAE
    {
        public string HeadLineSize { get; set; }

        public string CompromentimentoMargem { get; set; }

        public string InvestAlt1TIR { get; set; }

        public string InvestAlt1VPL { get; set; }

        public string InvestAlt1Retorno { get; set; }

        public string DoNothingTIR { get; set; }

        public string DoNothingVPL { get; set; }

        public string InvestDoNothingTIR { get; set; }

        public string InvestDoNothingVPL { get; set; }

        public string NivelAprovacaoRequerido { get; set; }

        public string MargemProposta { get; set; }

        public string ValorEquivalenteFundoPerdido { get; set; }

        public string ValorEquivalenteRVI { get; set; }

        public string ValorEquivalenteFundoPerdidoBase { get; set; }

        public string ValorEquivalenteRVIBase { get; set; }

        public string MargemFaixa { get; set; }

        public string MargemFaixaBase { get; set; }

        public string BookProfitLoss { get; set; }

        public string IrLucroVendaAtivo { get; set; }

        public FluxoCaixaIncrementalSAE InfoFluxoCaixaIncremental { get; set; }

        public string PeriodoIsencaoFee { get; set; }

        public string FeeSobreFaturamentoReal { get; set; }

        public string InicioVigenciaFeeMinimo { get; set; }

        public string ECON_MargemFuelsCasoInvestimento { get; set; }
    }
}
