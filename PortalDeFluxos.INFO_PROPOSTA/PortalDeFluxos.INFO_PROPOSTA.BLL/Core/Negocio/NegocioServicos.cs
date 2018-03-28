using Iteris;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.Client.Utilities;
using PortalDeFluxos.Core.BLL.Modelo.SAE;
using System.Reflection;
using PortalDeFluxos.Core.BLL.Modelo.FichaCadastral;
using PortalDeFluxos.Core.BLL.Modelo.Salesforce;
using PortalDeFluxos.Core.BLL.Core.Modelo.Salesforce;
using PortalDeFluxos.Core.BLL.Core.Modelo.A3F;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioServicos
    {
        private const string CONST_ASSEMBLY_SERVICO = "PortalDeFluxos.Core.Servicos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=05ec086ac61b4849";
        private const string CONST_CLASSE_ASSEMBLY_SALESFORCE = "PortalDeFluxos.Core.Servicos.ServicoSalesForce";
        private const string CONST_CLASSE_ASSEMBLY_SAE = "PortalDeFluxos.Core.Servicos.SAE.ServicoSAE";
        private const string CONST_CLASSE_ASSEMBLY_FICHACADASTRAL = "PortalDeFluxos.Core.Servicos.FichaCadastral.ServicoFichaCadastral";
        private const string CONST_CLASSE_ASSEMBLY_SAEB2B = "PortalDeFluxos.Core.Servicos.SAE_B2B.ServicoSAEB2B";
        private const string CONST_CLASSE_ASSEMBLY_SaeComum = "PortalDeFluxos.Core.Servicos.SaeComum.ServicoSaeComum";
		private const string CONST_CLASSE_ASSEMBLY_RBC = "PortalDeFluxos.Core.Servicos.RBC.ServicoRBC";
		private const string CONST_CLASSE_ASSEMBLY_AnexoFacil = "PortalDeFluxos.Core.Servicos.AnexoFacil.ServicoAnexoFacil";
		private const string CONST_CLASSE_ASSEMBLY_A3F = "PortalDeFluxos.Core.Servicos.A3F.ServicoA3F";

		#region [Integração RBC]

		public static ClienteSAE CarregarDadosIntegracaoRBC(int ibm, String urlContexto)
        {
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
			Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_RBC);

            var servicoRBC = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);
			object retornoWS = servicoRBC.GetType().GetMethod("ConsultarCliente").Invoke(servicoRBC, new object[] { ibm.ToString(), String.Empty, urlContexto });

            ClienteSAE cliente = null;

            if (retornoWS != null)
                cliente = Serializacao.DeserializeFromJson<ClienteSAE>(retornoWS.ToString());

            return cliente;
        }

        #endregion

        #region [Ficha Cadastral]

		public static string EnviarDadosFichaCadastral(Int32 idComprador, FichaCadastralFCCD fichaEnviada, EnderecoFCCD endereco, Int32 idProposta = 0)
		{
			Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
			Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_FICHACADASTRAL);

			if (PortalWeb.ContextoWebAtual.Configuracao.AmbienteAtual != Ambiente.PRD)
			{
				new Log().InserirMensagem("EnviarDadosFichaCadastral", "Form", String.Format("Dados - fichaEnviada {0}", idProposta), Serializacao.SerializeToJson(fichaEnviada));
				new Log().InserirMensagem("EnviarDadosFichaCadastral", "Form", String.Format("Dados - endereco {0}", idProposta), Serializacao.SerializeToJson(endereco));
			}

			var servicoFichaCadastral = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

			object retornoWS = servicoFichaCadastral.GetType().GetMethod("AdicionarFichaCadastral").Invoke(servicoFichaCadastral
				, new object[] { Serializacao.SerializeToJson(fichaEnviada), Serializacao.SerializeToJson(endereco), null, null, null });
			string retornoFichas = string.Empty;

			if (retornoWS != null)
			{
				string mensagemWsFicha = retornoWS.ToString();
				

				if (mensagemWsFicha.ToLower().Contains("sucesso"))
				{
					string idFicha = mensagemWsFicha.Replace("Ficha Cadastral ", String.Empty).Replace(" armazenada com sucesso.", String.Empty);
					int numFicha = 0;
					int.TryParse(idFicha, out numFicha);
					retornoFichas = idFicha;
				}
				else
				{
					retornoFichas = "Erro no cadastro do comprador ID: " + idComprador + ", RazaoSocial: " + fichaEnviada.RazaoSocial + ". Erro retornado: " + mensagemWsFicha;
					new Log().InserirMensagem("PortalDeFluxos.Core.Servicos.FichaCadastral.ServicoFichaCadastral", "IntegracaoFichaCadastral", String.Format("EnviarFichasEmbandeiramento - {0}", retornoFichas));
				}
			}

			return retornoFichas;
		}

		public static String RepresentanteVendaGet(String canal_)
		{
			String mensagemWsFicha = String.Empty;
			object retornoWS = null;
			try
			{
				Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
				Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_FICHACADASTRAL);
				var servicoFichaCadastral = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

				retornoWS = servicoFichaCadastral.GetType().GetMethod("RepresentanteVendaGet").Invoke(servicoFichaCadastral
					, new object[] { canal_ });
				mensagemWsFicha = retornoWS.ToString();
			}
			catch (Exception ex)
			{
				mensagemWsFicha = retornoWS != null ? retornoWS.ToString() : "";
				if (String.IsNullOrEmpty(mensagemWsFicha))
					new Log().InserirMensagem("PortalDeFluxos.Core.Servicos.FichaCadastral.ServicoFichaCadastral", "RepresentanteVendaGet", String.Format("RepresentanteVendaGet - {0}", mensagemWsFicha));
				else
					new Log().Inserir("PortalDeFluxos.Core.Servicos.FichaCadastral.ServicoFichaCadastral", "RepresentanteVendaGet", ex);
			}
			return mensagemWsFicha;
		}

        #endregion

        #region [SalesForce]

        public static KeyValuePair<Boolean, String> EnviarDadosSalesForce(PropostaInvestimentoSalesforce propostaInvestimento)
        {
            Boolean retorno = true;
            String message = String.Empty;

            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SALESFORCE);

            if (PortalWeb.ContextoWebAtual.Configuracao.AmbienteAtual != Ambiente.PRD)
            {
				new Log().InserirMensagem("EnviarDadosSalesForce", "Form", String.Format("Dados - propostaInvestimento id:{0}", propostaInvestimento.ItemInvestimentoVarejo.IDProposta), Serializacao.SerializeToJson(propostaInvestimento));
            }

            var servicoSalesForce = Activator.CreateInstance(type, (Int32)TipoWebServices.SalesForce_PropostaInvestimento, PortalWeb.ContextoWebAtual.Url);
            object retornoWS = servicoSalesForce.GetType().GetMethod("EnviarPropostaInvestimento").Invoke(servicoSalesForce, new object[] { Serializacao.SerializeToJson(propostaInvestimento) });

            if (retornoWS != null && retornoWS is String)
            {
				if (PortalWeb.ContextoWebAtual.Configuracao.AmbienteAtual != Ambiente.PRD)
					new Log().InserirMensagem("EnviarDadosSalesForce", "Form", String.Format("Retorno - propostaInvestimento id:{0}", propostaInvestimento.ItemInvestimentoVarejo.IDProposta), retornoWS.ToString());

				PropostaInvestimentoRetornoSalesforce retornoSalesforce = Serializacao.DeserializeFromJson<PropostaInvestimentoRetornoSalesforce>(retornoWS.ToString());
                if (retornoSalesforce.CodigoRetorno <= 0)
                {
                    message = retornoSalesforce.MensagemRetorno;
					new Log().InserirMensagem("NegocioRNIP", "Retorno Salesforce", String.Format("Método:EnviarPropostaInvestimento ID:{0} msg:{1} ", propostaInvestimento.ItemInvestimentoVarejo.IDProposta, retornoSalesforce.MensagemRetorno));
                    retorno = false;
                }
            }
            else
            {
                new Log().InserirMensagem("NegocioRNIP", "Retorno Salesforce", String.Format("Método:EnviarPropostaInvestimento msg:{0}", "retorno nulo."));
                message = "Retorno nulo do Sales Force!";
                retorno = false;
            }

            return new KeyValuePair<bool, string>(retorno, message);
        }

        public static InformacoesSalesforce ObterDadosPorSiteCode(int siteCode, int idTipoProposta, string tipoProposta, Boolean antecipacaoRenovacao)
        {
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SALESFORCE);

            var servicoSalesForce = Activator.CreateInstance(type, (Int32)TipoWebServices.SalesForce_PropostaInvestimento, PortalWeb.ContextoWebAtual.Url);
            object retornoWS = servicoSalesForce.GetType().GetMethod("ObterOportunidadePorSiteCode").Invoke(servicoSalesForce, new object[] { siteCode.ToString(), tipoProposta });

            InformacoesSalesforce retornoSalesforce = null;

            if (retornoWS != null && retornoWS is String)
                retornoSalesforce = Serializacao.DeserializeFromJson<InformacoesSalesforce>(retornoWS.ToString());

            return retornoSalesforce;
        }

        public static Boolean ExisteIbmSitecode(int ibmSitecode_, bool pesquisaPorIBM_)
        {
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SALESFORCE);

            var servicoSalesForce = Activator.CreateInstance(type, (Int32)TipoWebServices.SalesForce_EstruturaComercial, PortalWeb.ContextoWebAtual.Url);
            object retornoWS = servicoSalesForce.GetType().GetMethod("ObterEstruturaComercial").Invoke(servicoSalesForce, new object[] { ibmSitecode_.ToString(), pesquisaPorIBM_, String.Empty, false });

            return retornoWS == null ? false : true;
        }

        public static InformacoesSalesforce ObterDadosPorIBM(int ibm, int idTipoProposta, string tipoProposta, Boolean antecipacaoRenovacao)
        {
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SALESFORCE);

            var servicoSalesForce = Activator.CreateInstance(type, (Int32)TipoWebServices.SalesForce_PropostaInvestimento, PortalWeb.ContextoWebAtual.Url);
            object retornoWS = servicoSalesForce.GetType().GetMethod("ObterOportunidadesPorIbm").Invoke(servicoSalesForce, new object[] { ibm.ToString(), tipoProposta });

            InformacoesSalesforce retornoSalesforce = null;

            if (retornoWS != null && retornoWS is String)
                retornoSalesforce = Serializacao.DeserializeFromJson<InformacoesSalesforce>(retornoWS.ToString());

            return retornoSalesforce;
        }

		public static List<InformacoesIBMSalesForce> ListarVolumePorIBM(List<int> ibms_, DateTime dataInicio_, DateTime dataFim_, char tipoDeRegistro_)
		{
            
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SALESFORCE);

            var servicoSalesForce = Activator.CreateInstance(type, (Int32)TipoWebServices.SalesForce_EstruturaComercial, PortalWeb.ContextoWebAtual.Url);
			object retornoWS = servicoSalesForce.GetType().GetMethod("ListarVolumePorIBM").Invoke(servicoSalesForce, new object[] { dataInicio_, dataFim_, tipoDeRegistro_ ,ibms_ });

			List<InformacoesIBMSalesForce> informacoesIBM = null;

            if (retornoWS != null && retornoWS is String)
				informacoesIBM = Serializacao.DeserializeFromJson<List<InformacoesIBMSalesForce>>(retornoWS.ToString());

			return informacoesIBM;
        }

        #endregion

		#region [A3F]
		public static DadosA3F ConsultarInformacoesSolicitacaoRnip(String codigoProposta, String fluxo)
		{
			Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
			Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_A3F);

			var servicoA3F = Activator.CreateInstance(type, (Int32)TipoWebServices.A3FWebService, PortalWeb.ContextoWebAtual.Url);
			object retornoWS = servicoA3F.GetType().GetMethod("ConsultarInformacoesSolicitacaoRnip").Invoke(servicoA3F, new object[] { codigoProposta, fluxo });

			DadosA3F retornoA3F = null;

			if (retornoWS != null && retornoWS is String)
				retornoA3F = Serializacao.DeserializeFromJson<DadosA3F>(retornoWS.ToString());

			return retornoA3F;
		}

		public static RetornoWF AtualizarInformacoesSolicitacaoRnip(String codigoProposta, String fluxo, String ibm, String status)
		{
			Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
			Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_A3F);
			RetornoWF retorno = null;

			var servicoA3F = Activator.CreateInstance(type, (Int32)TipoWebServices.A3FWebService, PortalWeb.ContextoWebAtual.Url);
			object retornoWS = servicoA3F.GetType().GetMethod("AtualizarInformacoesSolicitacaoRnip").Invoke(servicoA3F, new object[] { codigoProposta, fluxo, ibm, status });

			if (retornoWS != null && retornoWS is String)
				retorno = Serializacao.DeserializeFromJson<RetornoWF>(retornoWS.ToString());

			return retorno;
		}

		#endregion

		#region [SAE - RNIP]

		public static object EnviarPlanilhaSAE(String casoInvestimento, String casoBase
            , String emailColaborador, String titulo, String idProposta, String planilhaDoItem
            , String planilha, String pasta)
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SAE);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            return servicoSAE.GetType().GetMethod("EnviarPlanilhaSAE").Invoke(servicoSAE, new object[] { 
                                                                                          casoInvestimento,
                                                                                          casoBase,
                                                                                          emailColaborador,
                                                                                          titulo,
                                                                                          idProposta,
                                                                                          planilhaDoItem,
                                                                                          planilha,
                                                                                          pasta });

        }

        public static object CalculoBaseRNIP(String casoInvestimento, String casoBase
            , String idProposta, String planilhaDoItem
            , String planilha, String pasta)
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SAE);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            return servicoSAE.GetType().GetMethod("CalculoBaseRNIP").Invoke(servicoSAE, new object[] { 
                                                                                          casoInvestimento,
                                                                                          casoBase,
                                                                                          idProposta,
                                                                                          planilhaDoItem,
                                                                                          planilha,
                                                                                          pasta });

        }


        public static object CalculoBaseRNIPMargemFaixa(String casoInvestimento, String casoBase
            , String idProposta, String planilhaDoItem
            , String planilha, String pasta)
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SAE);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            return servicoSAE.GetType().GetMethod("CalculoBaseRNIPMargemFaixa").Invoke(servicoSAE, new object[] { 
                                                                                          casoInvestimento,
                                                                                          casoBase,
                                                                                          idProposta,
                                                                                          planilhaDoItem,
                                                                                          planilha,
                                                                                          pasta });

        }

        #endregion

        #region [SAE - B2B]

        public static object EnviarPlanilhaSAEB2B(String casoInvestimento, String casoBase
            , String emailColaborador, String titulo, String idProposta, String planilhaDoItem
            , String planilha, String pasta)
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SAEB2B);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            return servicoSAE.GetType().GetMethod("EnviarPlanilhaSAEB2B").Invoke(servicoSAE, new object[] { 
                                                                                          casoInvestimento,
                                                                                          casoBase,
                                                                                          emailColaborador,
                                                                                          titulo,
                                                                                          idProposta,
                                                                                          planilhaDoItem,
                                                                                          planilha,
                                                                                          pasta });

        }

        public static object CalculoBaseB2B(String casoInvestimento, String casoBase
            , String idProposta, String planilhaDoItem
            , String planilha, String pasta)
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SAEB2B);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            return servicoSAE.GetType().GetMethod("CalculoBaseB2B").Invoke(servicoSAE, new object[] { 
                                                                                          casoInvestimento,
                                                                                          casoBase,
                                                                                          idProposta,
                                                                                          planilhaDoItem,
                                                                                          planilha,
                                                                                          pasta });

        }

        #endregion

        #region SaeComum
		
		public static bool ObterSAE(	Dictionary<string, string> dicSae
									,	int idItem
									,	string versaoSAE
									,	string nomePasta
									,	string assuntoEmail
									,	string destinatarioEmail
									,	bool salvarPlanilha = false
									,	string cultureName = "")
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SaeComum);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);
            string camposEnviados = Serializacao.SerializeToJson(dicSae);

			object retorno = servicoSAE.GetType().GetMethod("CalcularSAE").Invoke(servicoSAE, 
							new object[] { 
											camposEnviados
										,	idItem.ToString()
										,	versaoSAE
										,	nomePasta
										,	PortalWeb.ContextoWebAtual.Url
										,	salvarPlanilha
										,	assuntoEmail
										,	destinatarioEmail
										,	true
										,	cultureName
							});

            bool sucesso = false;
			if (retorno != null && retorno is String)
				Boolean.TryParse(retorno.ToString(), out sucesso);
            return sucesso;

        }

		public static Dictionary<string, string> CalcularSAE(Dictionary<string, string> dicSae
												,	int idItem
												,	string versaoSAE
												,	string nomePasta
												,	bool salvarPlanilha
												,	string cultureName = "")
        {
            //Chamada WebServices
            Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
            Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_SaeComum);
            var servicoSAE = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);

            string camposEnviados = Serializacao.SerializeToJson(dicSae);
			object retorno = servicoSAE.GetType().GetMethod("CalcularSAE").Invoke(servicoSAE, 
										new object[] { 
                                            camposEnviados
										,	idItem.ToString()
										,	versaoSAE
										,	nomePasta
										,	PortalWeb.ContextoWebAtual.Url
										,	salvarPlanilha
										,	""
										,	""
										,	false
										,	cultureName });

            Dictionary<string, string> dicCamposRetornados = new Dictionary<string, string>();
            if (retorno != null && retorno is String)
                dicCamposRetornados = Serializacao.DeserializeFromJson<Dictionary<string, string>>(retorno.ToString());
            return dicCamposRetornados;
        }

		#endregion

		#region [Anexo Fácil]

		public static KeyValuePair<bool, string> CadastrarPropostaAnexoFacil(object proposta)
		{
			Assembly assembly = Assembly.Load(CONST_ASSEMBLY_SERVICO);
			Type type = assembly.GetType(CONST_CLASSE_ASSEMBLY_AnexoFacil);

			var servicoSalesForce = Activator.CreateInstance(type, PortalWeb.ContextoWebAtual.Url);
			object retornoWS = servicoSalesForce.GetType().GetMethod("CadastrarProposta").Invoke(servicoSalesForce, new object[] { proposta });
			KeyValuePair<bool, string> retorno = new KeyValuePair<bool, string>();

			if (retornoWS != null && retornoWS is KeyValuePair<bool, string>)
				retorno = (KeyValuePair<bool, string>)retornoWS;
			return retorno;
		}

		#endregion

	}
}
