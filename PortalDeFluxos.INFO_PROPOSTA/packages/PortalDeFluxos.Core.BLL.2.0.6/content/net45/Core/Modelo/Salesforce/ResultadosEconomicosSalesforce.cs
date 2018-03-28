using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class ResultadosEconomicosSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public Nullable<System.Decimal> ECON_HeadLineSize { get; set; }

        public string ECON_NivelAprovacaoRequerido { get; set; }

        public Nullable<System.Decimal> ECON_TIRCasoBase { get; set; }

        public Nullable<System.Decimal> ECON_TIRDiferencial { get; set; }

        public Nullable<System.Decimal> ECON_TIRCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_TIRSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_VPLCasoBase { get; set; }

        public Nullable<System.Decimal> ECON_VPLDiferencial { get; set; }

        public Nullable<System.Decimal> ECON_VPLCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_VPLSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_TempoRetornoAnosCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_TempoRetornoAnosSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_MargemFuelsCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_ComprometimentoMargemSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_VIR { get; set; }

        public Nullable<System.Decimal> ECON_TempoRetornoAnos { get; set; }

        public Nullable<System.Decimal> ECON_ValorPagoProposta { get; set; }

        public Nullable<System.Decimal> ECON_VIRCasoBase { get; set; }

        public Nullable<System.Decimal> ECON_TempoRetornoCasoBaseAnos { get; set; }

        public Nullable<System.Decimal> ECON_ValorPagoPropostaCasoBase { get; set; }

        public Nullable<System.Decimal> ECON_ComprometimentoMargemTotal { get; set; }

        public Nullable<System.Decimal> ECON_VIRCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_ValorPagoPropostaCasoInvestimento { get; set; }

        public Nullable<System.Decimal> ECON_ComprometimentoMargemBKP { get; set; }

        public Nullable<System.Decimal> ECON_VIRSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_ValorPagoPropostaSensibilidade1 { get; set; }

        public Nullable<System.Decimal> ECON_VIRSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_VPLSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_TIRSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_TempoRetornoAnosSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_ValorPagoPropostaSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_ComprometimentoMargemSensibilidade2 { get; set; }

        public Nullable<System.Decimal> ECON_Bonificacao1 { get; set; }

        public Nullable<System.Int32> ECON_NumeroAnosProjeto { get; set; }

        public Nullable<System.Decimal> ECON_TotalMofuelResultadosEconomicos { get; set; }

        public Nullable<System.Decimal> ECON_MargemProposta { get; set; }

        public Nullable<System.Decimal> ECON_FundoPerdido { get; set; }

        public Nullable<System.Decimal> ECON_ValorEquivalenteFundoPerdido { get; set; }

        public Nullable<System.Decimal> ECON_ImagemRVI { get; set; }

        public Nullable<System.Decimal> ECON_ValorEquivalenteRVI { get; set; }

        public Nullable<System.Decimal> ECON_CreditoMogas { get; set; }

        public Nullable<System.Decimal> ECON_CreditoDiesel { get; set; }
    }
}
