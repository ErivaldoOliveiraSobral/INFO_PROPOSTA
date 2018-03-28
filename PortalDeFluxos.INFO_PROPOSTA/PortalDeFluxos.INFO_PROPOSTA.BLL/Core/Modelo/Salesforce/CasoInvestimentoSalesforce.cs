using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class CasoInvestimentoSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public Nullable<System.Decimal> INVE_QtdeGasolinaComumAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeGasolinaAditivadaAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeEtanolAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeEtanolAditivadoAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeDieselAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeDieselAditivadoAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeOleoCombustivelAno1 { get; set; }

        public Nullable<System.Decimal> INVE_QtdeOleoCombustivelAditivadoAno1 { get; set; }

        public Nullable<System.Decimal> INVE_AluguelPago { get; set; }

        public Nullable<System.Decimal> INVE_AluguelRecebido { get; set; }

        public Nullable<System.Decimal> INVE_CorrecaoMonetaria { get; set; }

        public Nullable<System.Decimal> INVE_MargemTotal { get; set; }

        public Nullable<System.Decimal> INVE_MargemFaixaCasoInvestimento { get; set; }

        public string INVE_NomeBase { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno1 { get; set; }

        public Nullable<System.Int32> INVE_LubrificantesAno1 { get; set; }

        public string INVE_TrocaOleo { get; set; }

        public string INVE_ModeloGNV { get; set; }

        public Nullable<System.Decimal> INVE_FeeMargemGNV { get; set; }

        public Nullable<System.Decimal> INVE_FundoPerdido { get; set; }

        public Nullable<System.Decimal> INVE_ImagemRVI { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve1 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve2 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve3 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve4 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve5 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve6 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve7 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve8 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve9 { get; set; }

        public Nullable<System.Decimal> INVE_BonificacaoInve10 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno1 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno2 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno3 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno4 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno5 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno6 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno7 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno8 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno9 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno10 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno11 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno12 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno13 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno14 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno15 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno16 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno17 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno18 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno19 { get; set; }

        public Nullable<System.Decimal> INVE_CurvaMaturacaoFuelsAno20 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno2 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno3 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno4 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno5 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno6 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno7 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno8 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno9 { get; set; }

        public Nullable<System.Decimal> INVE_MargemPropostaAno10 { get; set; }

        public Nullable<System.Decimal> INVE_MargemBackup1 { get; set; }

        public Nullable<System.Decimal> INVE_MargemBackup2 { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_RVI { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_RVIAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_Terreno { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_TerrenoAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_Pista { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_PistaAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_Tanques { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_TanquesAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_Bombas { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_BombasAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_Loja { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_LojaAno { get; set; }

        public Nullable<System.Decimal> INVE_CAPEX_EquipamentosLoja { get; set; }

        public Nullable<System.Int32> INVE_CAPEX_EquipamentosLojaAno { get; set; }

        public Nullable<System.Decimal> INVE_Manutencao { get; set; }

        public Nullable<System.Int32> INVE_ManutencaoAno { get; set; }

        public Nullable<System.Decimal> INVE_MeioAmbiente { get; set; }

        public Nullable<System.Int32> INVE_MeioAmbienteAno { get; set; }

        public Nullable<System.Decimal> INVE_OutrasReceitas_Total { get; set; }

        public Nullable<System.Decimal> INVE_OutrasDispesas_Total { get; set; }

        public Nullable<System.Decimal> INVE_OutrasReceitas_NON_TX_Total { get; set; }

        public Nullable<System.Decimal> INVE_OutrasDispesas_NON_TX_Total { get; set; }

        public Nullable<System.Decimal> INVE_FundoPerdidoM3 { get; set; }

        public Nullable<System.Decimal> INVE_FundoPerdidoAno1 { get; set; }

        public Nullable<System.Decimal> INVE_FundoPerdidoAno2 { get; set; }

        public Nullable<System.Decimal> INVE_FundoPerdidoAno3 { get; set; }

        public Nullable<System.Decimal> INVE_RVI_M3 { get; set; }

        public Nullable<System.Int32> INVE_PrazoMogasAno1 { get; set; }

        public Nullable<System.Int32> INVE_PrazoDieselAno1 { get; set; }

        public Nullable<System.Int32> INVE_PrazoOleoCombustivelAno1 { get; set; }

        public Nullable<System.Int32> INVE_PrazoMogasAno2 { get; set; }

        public Nullable<System.Int32> INVE_PrazoDieselAno2 { get; set; }

        public Nullable<System.Int32> INVE_PrazoOleoCombustivelAno2 { get; set; }

        public Nullable<System.Int32> INVE_PrazoMogasAno3 { get; set; }

        public Nullable<System.Int32> INVE_PrazoDieselAno3 { get; set; }

        public Nullable<System.Int32> INVE_PrazoOleoCombustivelAno3 { get; set; }

        public Nullable<System.Decimal> INVE_ATIVOS_ValorVenda { get; set; }

        public Nullable<System.Decimal> INVE_ATIVOS_Book { get; set; }

        public Nullable<System.Decimal> INVE_ATIVOS_ProfitLoss { get; set; }

        public Nullable<System.Decimal> INVE_ValorAvaliacao { get; set; }

        public Nullable<System.Decimal> INVE_ValorVenda { get; set; }

        public Nullable<System.Decimal> INVE_ValorEntrada { get; set; }

        public Nullable<System.Decimal> INVE_ValorParcelado { get; set; }

        public Nullable<System.Int32> INVE_QtdeParcelaVendaAtivo { get; set; }

        public Nullable<System.Decimal> INVE_ValorBook { get; set; }

        public Nullable<System.Decimal> INVE_ValorProfitLoss { get; set; }

        public Nullable<System.Decimal> INVE_Percentual_FP_RegistroHipotecaValor { get; set; }

        public Nullable<System.Decimal> INVE_Perc_FP_FinaObraInstalacaoTanques { get; set; }

        public Nullable<System.Decimal> INVE_Perc_FP_FinalizacaoObra { get; set; }

        public Nullable<System.Decimal> INVE_CustoLitro { get; set; }

        public Nullable<System.Decimal> INVE_FreteLitro { get; set; }

        public string INVE_TratamentoAtivo { get; set; }

        public Nullable<System.Decimal> INVE_VendaAtivo { get; set; }

        public Nullable<System.Int32> INVE_Backup_1 { get; set; }

        public Nullable<System.Int32> INVE_Backup_2 { get; set; }

        public Nullable<System.Int32> INVE_Backup_3 { get; set; }

        public Nullable<System.Int32> INVE_Backup_4 { get; set; }

        public Nullable<System.Decimal> INVE_Backup_5 { get; set; }

        public Nullable<System.Decimal> INVE_Backup_6 { get; set; }

        public Nullable<System.Decimal> INVE_Backup_7 { get; set; }

        public Nullable<System.Decimal> INVE_Backup_8 { get; set; }

        public string INVE_Backup_9 { get; set; }

        public string INVE_Backup_10 { get; set; }

        public Nullable<System.Decimal> INVE_ValorEquivalenteRVI { get; set; }

        public Nullable<System.Int32> INVE_NumeroAnosProjeto { get; set; }

        public Nullable<System.Decimal> INVE_GasolinaComumMedia { get; set; }

        public Nullable<System.Decimal> INVE_GasolinaComumProposta { get; set; }

        public Nullable<System.Decimal> INVE_AjusteMargem { get; set; }

        public Nullable<System.Decimal> INVE_GasolinaVPowerMedia { get; set; }

        public Nullable<System.Decimal> INVE_GasolinaVPowerProposta { get; set; }

        public Nullable<System.Decimal> INVE_EtanolComumMedia { get; set; }

        public Nullable<System.Decimal> INVE_EtanolComumProposta { get; set; }

        public Nullable<System.Decimal> INVE_EtanolVPowerMedia { get; set; }

        public Nullable<System.Decimal> INVE_EtanolVPowerProposta { get; set; }

        public Nullable<System.Decimal> INVE_DieselComumMedia { get; set; }

        public Nullable<System.Decimal> INVE_DieselComumProposta { get; set; }

        public Nullable<System.Decimal> INVE_FormulaDieselMedia { get; set; }

        public Nullable<System.Decimal> INVE_FormulaDieselProposta { get; set; }

        public Nullable<System.Decimal> INVE_TotalMofuelMedia { get; set; }

        public Nullable<System.Decimal> INVE_TotalMofuelProposta { get; set; }

        public Nullable<System.Decimal> INVE_VolumeGNV { get; set; }

        public Nullable<System.Int32> INVE_PrazoMogas { get; set; }

        public Nullable<System.Decimal> INVE_ValorEquivalenteFundoPerdido { get; set; }

        public Nullable<System.Decimal> INVE_CreditoDiesel { get; set; }

        public Nullable<System.Decimal> INVE_OutrasReceitas1 { get; set; }

        public Nullable<System.Decimal> INVE_OutrasReceitas2 { get; set; }

        public Nullable<System.Decimal> INVE_OutrasReceitas3 { get; set; }

        public Nullable<System.Decimal> INVE_OutrasDespesas1 { get; set; }

        public Nullable<System.Decimal> INVE_OutrasDespesas2 { get; set; }

        public Nullable<System.Decimal> INVE_OutrasDespesas3 { get; set; }

        public string INVE_ContratoPadrao { get; set; }

        public string INVE_ClausulasAlteradas { get; set; }

        public string INVE_PontosRelevantes { get; set; }
    }
}
