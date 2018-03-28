using Iteris.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using Iteris;
using System.Xml;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace PortalDeFluxos.Core.Test
{
//    public enum TipoPropostaTeste
//    {
//        EmbandeiramentoRNIP = 1,
//        DesExitLocRNIP = 2,
//        RenovacaoContratoRNIP = 3
//    }

//    public class PropostaTeste
//    {
//        public const string queryCaixaIncremental = "<Where><Eq><FieldRef Name='IdItemProposta' /><Value Type='Lookup'>{0}</Value></Eq></Where>";

//        #region [Propriedades]

//        public int IndexProposta { get; set; }
//        public string NomeProposta { get; set; }
//        public int IdListItem { get; set; }
//        public string GuidLista { get; set; }
//        public string urlListItem { get; set; }
//        public TipoProposta TipoProposta { get; set; }
//        public int VolumeGasolina { get; set; }
//        public int VolumeEtanol { get; set; }
//        public int VolumeDies { get; set; }
//        public int VolumeTotal { get; set; }
//        public int RebateUnitario { get; set; }
//        public int FPUnitario { get; set; }
//        public int RVIUnitario { get; set; }
//        public int TotalUnitario { get; set; }
//        public int FPTotal { get; set; }
//        public int FPTotalBase { get; set; }
//        public int RVITotal { get; set; }
//        public int RVITotalBase { get; set; }
//        public int ConcessoesTotal { get; set; }
//        public int ConcessoesTotalBase { get; set; }
//        public int HeadlineSize { get; set; }
//        public int VPLInvestimento { get; set; }
//        public int VPLBase { get; set; }
//        public int VPLDiferencial { get; set; }
//        public int VPLSensibilidade { get; set; }
//        public float TIRInvestimento { get; set; }
//        public float TIRBase { get; set; }
//        public float TIRDiferencial { get; set; }
//        public float TIRSensibilidade { get; set; }
//        public float ComprometMargemInvestimento { get; set; }
//        public int ComprometMargemSensibilidade { get; set; }
//        public float TempoRetornoInvestimento { get; set; }
//        public float TempoRetornoBase { get; set; }
//        public float TempoRetornoSensibilidade { get; set; }
//        public string DescricaoSensibilidade { get; set; }
//        public int AnosProjeto { get; set; }
//        public int AnosProjetoBase { get; set; }
//        public int TotalMofuel { get; set; }
//        public int TotalMofuelBase { get; set; }
//        public float[] BonificacaoAno { get; set; }
//        public float[] CurvaMaturacaoAno { get; set; }
//        public float TotalBonificacao { get; set; }
//        public float[] BonificacaoAnoBase { get; set; }
//        public float[] CurvaMaturacaoAnoBase { get; set; }
//        public float TotalBonificacaoBase { get; set; }
//        public int VolumeTotalBase { get; set; }
//        public float[] FluxoCaixaIncrementalDiferencial { get; set; }
//        public float[] FluxoCaixaIncrementalInvestimento { get; set; }
//        public float[] FluxoCaixaIncrementalBase { get; set; }
//        public float Capex { get; set; }
//        public bool BaixaDeAtivos { get; set; }
//        public string SubTipoProposta { get; set; }
//        public bool ContratoPadrao { get; set; }
//        public bool FOB { get; set; }
//        public bool CIF { get; set; }
//        public int[] IBM { get; set; }
//        public bool CriarIBM { get; set; }
//        public float MargemFaixa { get; set; }
//        public float MargemFaixaBase { get; set; }
//        public float MargemAjustada { get; set; }
//        public float MargemAjustadaBase { get; set; }
//        public float AjusteMargem { get; set; }
//        public float AjusteMargemBase { get; set; }
//        public int MargemAgregadaInvestimento { get; set; }
//        public int MargemAgregadaBase { get; set; }
//        public int[] NumFichaCadastral { get; set; }
//        public string CNPJ { get; set; }
//        public bool isRBA { get; set; }


//        #endregion

//        public PropostaTeste()
//        {

//        }
//    }

     
//    [TestClass]
//    public class TesteEntidadeDBTeste
//    {
//        String xmlProposta = @"<?xml version=""1.0"" encoding=""UTF-8""?>
//<ArrayOfProposta xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
//   <Proposta>
//      <IndexProposta>0</IndexProposta>
//      <NomeProposta>[RNIP] REN - AUTO POSTO REDE SENNA LTDA - 1016713 - 1857</NomeProposta>
//      <IdListItem>1857</IdListItem>
//      <GuidLista>2d84fc2a-071c-4653-9074-bb61218acf18</GuidLista>
//      <urlListItem>Lists/RenovacaoContratoRNIP/1857_.000</urlListItem>
//      <TipoProposta>RenovacaoContratoRNIP</TipoProposta>
//      <VolumeGasolina>9492000</VolumeGasolina>
//      <VolumeEtanol>7182000</VolumeEtanol>
//      <VolumeDies>798000</VolumeDies>
//      <VolumeTotal>17472000</VolumeTotal>
//      <RebateUnitario>0</RebateUnitario>
//      <FPUnitario>0</FPUnitario>
//      <RVIUnitario>0</RVIUnitario>
//      <TotalUnitario>0</TotalUnitario>
//      <FPTotal>0</FPTotal>
//      <FPTotalBase>0</FPTotalBase>
//      <RVITotal>0</RVITotal>
//      <RVITotalBase>0</RVITotalBase>
//      <ConcessoesTotal>646464</ConcessoesTotal>
//      <ConcessoesTotalBase>52542</ConcessoesTotalBase>
//      <HeadlineSize>0</HeadlineSize>
//      <VPLInvestimento>998</VPLInvestimento>
//      <VPLBase>183</VPLBase>
//      <VPLDiferencial>815</VPLDiferencial>
//      <VPLSensibilidade>0</VPLSensibilidade>
//      <TIRInvestimento>0</TIRInvestimento>
//      <TIRBase>0</TIRBase>
//      <TIRDiferencial>0</TIRDiferencial>
//      <TIRSensibilidade>0</TIRSensibilidade>
//      <ComprometMargemInvestimento>26.4</ComprometMargemInvestimento>
//      <ComprometMargemSensibilidade>0</ComprometMargemSensibilidade>
//      <TempoRetornoInvestimento>1</TempoRetornoInvestimento>
//      <TempoRetornoBase>0</TempoRetornoBase>
//      <TempoRetornoSensibilidade>0</TempoRetornoSensibilidade>
//      <AnosProjeto>4</AnosProjeto>
//      <AnosProjetoBase>1</AnosProjetoBase>
//      <TotalMofuel>416</TotalMofuel>
//      <TotalMofuelBase>417</TotalMofuelBase>
//      <BonificacaoAno>
//         <float>0</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//      </BonificacaoAno>
//      <CurvaMaturacaoAno>
//         <float>0</float>
//         <float>100</float>
//         <float>100</float>
//         <float>100</float>
//         <float>50</float>
//      </CurvaMaturacaoAno>
//      <TotalBonificacao>646.46405</TotalBonificacao>
//      <BonificacaoAnoBase>
//         <float>0</float>
//         <float>0.025</float>
//      </BonificacaoAnoBase>
//      <CurvaMaturacaoAnoBase>
//         <float>0</float>
//         <float>42</float>
//      </CurvaMaturacaoAnoBase>
//      <TotalBonificacaoBase>52.542</TotalBonificacaoBase>
//      <VolumeTotalBase>2101680</VolumeTotalBase>
//      <FluxoCaixaIncrementalDiferencial>
//         <float>5.2</float>
//         <float>242.1</float>
//         <float>434.4</float>
//         <float>197.4</float>
//         <float>92</float>
//         <float>-4.5</float>
//      </FluxoCaixaIncrementalDiferencial>
//      <FluxoCaixaIncrementalInvestimento>
//         <float>9</float>
//         <float>439.6</float>
//         <float>430.6</float>
//         <float>197.4</float>
//         <float>92</float>
//         <float>-4.5</float>
//      </FluxoCaixaIncrementalInvestimento>
//      <FluxoCaixaIncrementalBase>
//         <float>3.8</float>
//         <float>197.5</float>
//         <float>-3.8</float>
//      </FluxoCaixaIncrementalBase>
//      <Capex>0</Capex>
//      <BaixaDeAtivos>false</BaixaDeAtivos>
//      <ContratoPadrao>false</ContratoPadrao>
//      <FOB>false</FOB>
//      <CIF>false</CIF>
//      <IBM>
//         <int>1016713</int>
//      </IBM>
//      <CriarIBM>false</CriarIBM>
//      <MargemFaixa>0.111</MargemFaixa>
//      <MargemFaixaBase>0.111</MargemFaixaBase>
//      <MargemAjustada>0.1804</MargemAjustada>
//      <MargemAjustadaBase>0.1804</MargemAjustadaBase>
//      <AjusteMargem>0.0694</AjusteMargem>
//      <AjusteMargemBase>0.0694</AjusteMargemBase>
//      <MargemAgregadaInvestimento>2632280</MargemAgregadaInvestimento>
//      <MargemAgregadaBase>379143</MargemAgregadaBase>
//      <isRBA>false</isRBA>
//   </Proposta>
//   <Proposta>
//      <IndexProposta>1</IndexProposta>
//      <NomeProposta>[RNIP] REN - VALENTE AUTO POSTO E SERVICOS LTDA - 1019775 - 1858</NomeProposta>
//      <IdListItem>1858</IdListItem>
//      <GuidLista>2d84fc2a-071c-4653-9074-bb61218acf18</GuidLista>
//      <urlListItem>Lists/RenovacaoContratoRNIP/1858_.000</urlListItem>
//      <TipoProposta>RenovacaoContratoRNIP</TipoProposta>
//      <VolumeGasolina>6510000</VolumeGasolina>
//      <VolumeEtanol>4158000</VolumeEtanol>
//      <VolumeDies>798000</VolumeDies>
//      <VolumeTotal>11466000</VolumeTotal>
//      <RebateUnitario>0</RebateUnitario>
//      <FPUnitario>0</FPUnitario>
//      <RVIUnitario>0</RVIUnitario>
//      <TotalUnitario>0</TotalUnitario>
//      <FPTotal>0</FPTotal>
//      <FPTotalBase>0</FPTotalBase>
//      <RVITotal>0</RVITotal>
//      <RVITotalBase>0</RVITotalBase>
//      <ConcessoesTotal>424242</ConcessoesTotal>
//      <ConcessoesTotalBase>34650</ConcessoesTotalBase>
//      <HeadlineSize>1</HeadlineSize>
//      <VPLInvestimento>600</VPLInvestimento>
//      <VPLBase>110</VPLBase>
//      <VPLDiferencial>490</VPLDiferencial>
//      <VPLSensibilidade>0</VPLSensibilidade>
//      <TIRInvestimento>0</TIRInvestimento>
//      <TIRBase>0</TIRBase>
//      <TIRDiferencial>0</TIRDiferencial>
//      <TIRSensibilidade>0</TIRSensibilidade>
//      <ComprometMargemInvestimento>26.9</ComprometMargemInvestimento>
//      <ComprometMargemSensibilidade>0</ComprometMargemSensibilidade>
//      <TempoRetornoInvestimento>1</TempoRetornoInvestimento>
//      <TempoRetornoBase>0</TempoRetornoBase>
//      <TempoRetornoSensibilidade>0</TempoRetornoSensibilidade>
//      <AnosProjeto>4</AnosProjeto>
//      <AnosProjetoBase>1</AnosProjetoBase>
//      <TotalMofuel>273</TotalMofuel>
//      <TotalMofuelBase>275</TotalMofuelBase>
//      <BonificacaoAno>
//         <float>0</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//      </BonificacaoAno>
//      <CurvaMaturacaoAno>
//         <float>0</float>
//         <float>100</float>
//         <float>100</float>
//         <float>100</float>
//         <float>50</float>
//      </CurvaMaturacaoAno>
//      <TotalBonificacao>424.241974</TotalBonificacao>
//      <BonificacaoAnoBase>
//         <float>0</float>
//         <float>0.025</float>
//      </BonificacaoAnoBase>
//      <CurvaMaturacaoAnoBase>
//         <float>0</float>
//         <float>42</float>
//      </CurvaMaturacaoAnoBase>
//      <TotalBonificacaoBase>34.65</TotalBonificacaoBase>
//      <VolumeTotalBase>1386000</VolumeTotalBase>
//      <FluxoCaixaIncrementalDiferencial>
//         <float>1.3</float>
//         <float>141.3</float>
//         <float>259.4</float>
//         <float>122.4</float>
//         <float>59.5</float>
//         <float>-1.1</float>
//      </FluxoCaixaIncrementalDiferencial>
//      <FluxoCaixaIncrementalInvestimento>
//         <float>2.3</float>
//         <float>260.7</float>
//         <float>258.5</float>
//         <float>122.4</float>
//         <float>59.5</float>
//         <float>-1.1</float>
//      </FluxoCaixaIncrementalInvestimento>
//      <FluxoCaixaIncrementalBase>
//         <float>1</float>
//         <float>119.5</float>
//         <float>-1</float>
//      </FluxoCaixaIncrementalBase>
//      <Capex>0</Capex>
//      <BaixaDeAtivos>false</BaixaDeAtivos>
//      <ContratoPadrao>false</ContratoPadrao>
//      <FOB>false</FOB>
//      <CIF>false</CIF>
//      <IBM>
//         <int>1019775</int>
//      </IBM>
//      <CriarIBM>false</CriarIBM>
//      <MargemFaixa>0.111</MargemFaixa>
//      <MargemFaixaBase>0.111</MargemFaixaBase>
//      <MargemAjustada>0.1734</MargemAjustada>
//      <MargemAjustadaBase>0.1734</MargemAjustadaBase>
//      <AjusteMargem>0.0624</AjusteMargem>
//      <AjusteMargemBase>0.0624</AjusteMargemBase>
//      <MargemAgregadaInvestimento>1681570</MargemAgregadaInvestimento>
//      <MargemAgregadaBase>240332</MargemAgregadaBase>
//      <isRBA>false</isRBA>
//   </Proposta>
//   <Proposta>
//      <IndexProposta>2</IndexProposta>
//      <NomeProposta>[RNIP] REN - AUTO POSTO G5 LTDA EPP - 1014519 - 1859</NomeProposta>
//      <IdListItem>1859</IdListItem>
//      <GuidLista>2d84fc2a-071c-4653-9074-bb61218acf18</GuidLista>
//      <urlListItem>Lists/RenovacaoContratoRNIP/1859_.000</urlListItem>
//      <TipoProposta>RenovacaoContratoRNIP</TipoProposta>
//      <VolumeGasolina>6510000</VolumeGasolina>
//      <VolumeEtanol>4242000</VolumeEtanol>
//      <VolumeDies>504000</VolumeDies>
//      <VolumeTotal>11256000</VolumeTotal>
//      <RebateUnitario>0</RebateUnitario>
//      <FPUnitario>0</FPUnitario>
//      <RVIUnitario>0</RVIUnitario>
//      <TotalUnitario>0</TotalUnitario>
//      <FPTotal>0</FPTotal>
//      <FPTotalBase>0</FPTotalBase>
//      <RVITotal>0</RVITotal>
//      <RVITotalBase>0</RVITotalBase>
//      <ConcessoesTotal>416472</ConcessoesTotal>
//      <ConcessoesTotalBase>33894</ConcessoesTotalBase>
//      <HeadlineSize>0</HeadlineSize>
//      <VPLInvestimento>587</VPLInvestimento>
//      <VPLBase>109</VPLBase>
//      <VPLDiferencial>478</VPLDiferencial>
//      <VPLSensibilidade>0</VPLSensibilidade>
//      <TIRInvestimento>0</TIRInvestimento>
//      <TIRBase>0</TIRBase>
//      <TIRDiferencial>0</TIRDiferencial>
//      <TIRSensibilidade>0</TIRSensibilidade>
//      <ComprometMargemInvestimento>27</ComprometMargemInvestimento>
//      <ComprometMargemSensibilidade>0</ComprometMargemSensibilidade>
//      <TempoRetornoInvestimento>1</TempoRetornoInvestimento>
//      <TempoRetornoBase>0</TempoRetornoBase>
//      <TempoRetornoSensibilidade>0</TempoRetornoSensibilidade>
//      <AnosProjeto>4</AnosProjeto>
//      <AnosProjetoBase>1</AnosProjetoBase>
//      <TotalMofuel>268</TotalMofuel>
//      <TotalMofuelBase>269</TotalMofuelBase>
//      <BonificacaoAno>
//         <float>0</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//         <float>0.037</float>
//      </BonificacaoAno>
//      <CurvaMaturacaoAno>
//         <float>0</float>
//         <float>100</float>
//         <float>100</float>
//         <float>100</float>
//         <float>50</float>
//      </CurvaMaturacaoAno>
//      <TotalBonificacao>416.472015</TotalBonificacao>
//      <BonificacaoAnoBase>
//         <float>0</float>
//         <float>0.025</float>
//      </BonificacaoAnoBase>
//      <CurvaMaturacaoAnoBase>
//         <float>0</float>
//         <float>42</float>
//      </CurvaMaturacaoAnoBase>
//      <TotalBonificacaoBase>33.8939972</TotalBonificacaoBase>
//      <VolumeTotalBase>1355760</VolumeTotalBase>
//      <FluxoCaixaIncrementalDiferencial>
//         <float>1.9</float>
//         <float>140.1</float>
//         <float>255.9</float>
//         <float>116.2</float>
//         <float>55.7</float>
//         <float>-1.6</float>
//      </FluxoCaixaIncrementalDiferencial>
//      <FluxoCaixaIncrementalInvestimento>
//         <float>3.2</float>
//         <float>257.7</float>
//         <float>254.5</float>
//         <float>116.2</float>
//         <float>55.7</float>
//         <float>-1.6</float>
//      </FluxoCaixaIncrementalInvestimento>
//      <FluxoCaixaIncrementalBase>
//         <float>1.3</float>
//         <float>117.6</float>
//         <float>-1.3</float>
//      </FluxoCaixaIncrementalBase>
//      <Capex>0</Capex>
//      <BaixaDeAtivos>false</BaixaDeAtivos>
//      <ContratoPadrao>false</ContratoPadrao>
//      <FOB>false</FOB>
//      <CIF>false</CIF>
//      <IBM>
//         <int>1014519</int>
//      </IBM>
//      <CriarIBM>false</CriarIBM>
//      <MargemFaixa>0.11</MargemFaixa>
//      <MargemFaixaBase>0.11</MargemFaixaBase>
//      <MargemAjustada>0.1744</MargemAjustada>
//      <MargemAjustadaBase>0.1744</MargemAjustadaBase>
//      <AjusteMargem>0.0644</AjusteMargem>
//      <AjusteMargemBase>0.0644</AjusteMargemBase>
//      <MargemAgregadaInvestimento>1652380</MargemAgregadaInvestimento>
//      <MargemAgregadaBase>236444</MargemAgregadaBase>
//      <isRBA>false</isRBA>
//   </Proposta>
//</ArrayOfProposta>";

//        [TestMethod]
//        public void TesteCombo()
//        {
//            List<Proposta> listaPropostas = XmlDeserializeProposta(xmlProposta);

//            for (int i = listaPropostas.Count - 1; i > 0; i--)
//            {
//                Proposta proposta = listaPropostas[i];
//                if (proposta.TipoProposta != TipoProposta.EmbandeiramentoRNIP)
//                {
//                    listaPropostas.RemoveAt(i);
//                }
//                else if (!String.IsNullOrEmpty(proposta.CNPJ) && proposta.CNPJ != "00.000.000/0000-00")
//                {
//                    listaPropostas.RemoveAt(i);
//                }
//            }
//        }

//        public static List<Proposta> XmlDeserializeProposta(string xml)
//        {
//            List<Proposta> listProposta = new List<Proposta>();

//            byte[] byteArray = Encoding.UTF8.GetBytes(xml);
//            MemoryStream memStream2 = new MemoryStream(byteArray);
//            XmlSerializer serial = new XmlSerializer(typeof(List<Proposta>));
//            memStream2.Flush();
//            memStream2.Seek(0, SeekOrigin.Begin);

//            listProposta = new List<Proposta>();
//            listProposta = (List<Proposta>)serial.Deserialize(XmlReader.Create(memStream2));

//            return listProposta;

//        }
    //}
}
