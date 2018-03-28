using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class PropostaInvestimentoVarejoSalesforce
    {
		public string UrlProposta { get; set; }

        public int NumeroPropostaSalesForce { get; set; }

        public string OrigemProposta { get; set; }

        public int NumeroFluxo { get; set; }

        public string TipoProposta { get; set; }

        public int SiteCode { get; set; }

        public Nullable<System.Int32> IBM { get; set; }

        public string StatusFluxoSP { get; set; }

        public string StatusPropostaSalesforce { get; set; }

        public string IDProposta { get; set; }

        public string INFO_CriadoPor { get; set; }

        public Nullable<System.DateTime> INFO_DataCriacao { get; set; }

        public Nullable<System.Int32> INFO_MesesProjeto { get; set; }

        public string INFO_RVI_Nivel { get; set; }

        public Nullable<System.DateTime> INFO_DataFutura { get; set; }

        public string INFO_VersaoSAE { get; set; }

        public string INFO_NomeRede { get; set; }

        public Nullable<System.Int32> INFO_Qtde_IBMs { get; set; }

        public string INFO_PerfilPosto { get; set; }

        public string INFO_ImovelProprioPosto { get; set; }

        public Nullable<System.DateTime> INFO_VencimentoLocacao { get; set; }

        public string INFO_NomeFaixa { get; set; }

        public string INFO_Cidade { get; set; }

        public string INFO_UF { get; set; }

        public string INFO_OfertaIntegrada { get; set; }

        public string INFO_PlataformaProposta { get; set; }

        public Nullable<System.DateTime> INFO_DataEnvioAprovacao { get; set; }

        public Nullable<System.DateTime> INFO_DataAprovacaoFinancas { get; set; }

        public Nullable<System.DateTime> INFO_DataAprovacaoContratos { get; set; }

        public Nullable<System.DateTime> INFO_DataAprovacaoGT { get; set; }

        public Nullable<System.DateTime> INFO_DataContratoRecebimento { get; set; }

        public Nullable<System.DateTime> INFO_DataReprovacao { get; set; }

        public Nullable<System.DateTime> INFO_DataExpiracao { get; set; }

        public string INFO_RazaoSocial { get; set; }

        public string INFO_CidadeConveniencia { get; set; }

        public string INFO_UFConveniencia { get; set; }

        public string INFO_OrigemD1 { get; set; }

        public string INFO_GerenteTerritorio { get; set; }

        public string INFO_Gerencia { get; set; }

        public string INFO_DiretorVendas { get; set; }

        public string INFO_CDR { get; set; }

        public string INFO_GDR { get; set; }

        public string INFO_PlataformaAtual { get; set; }

        public string INFO_PlanoRenovacaoAnoSafra { get; set; }

        public string INFO_Frete { get; set; }

        public string INFO_RedeExpress { get; set; }

        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> LastDeletedDate { get; set; }
    }
}
