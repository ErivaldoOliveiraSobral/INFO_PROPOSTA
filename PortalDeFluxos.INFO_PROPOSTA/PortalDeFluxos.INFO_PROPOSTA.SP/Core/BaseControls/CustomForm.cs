using Iteris;
using Microsoft.SharePoint;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using PortalDeFluxos.Core.Wrapper;
using System.Linq;

namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public class CustomForm : CustomUserControl
    {
        protected const string _ascxPathAnexos = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucUploadFile.ascx";
        protected const string _ascxPathBuscarEstruturaComercial = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucBuscarDadosComercial.ascx";
        protected const string _ascxPathEstruturaCdrGdr = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucEstruturaCdrGdr.ascx";
        protected const string _ascxPathEstruturaGtGrDiretor = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucEstruturaGtGrDiretor.ascx";
        protected const string _ascxPathEstruturaIndividual = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucEstruturaIndividual.ascx";
        protected const string _ascxPathAnoContratual = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucControleAnoContratual.ascx";
        protected const string _ascxPathCustomGrid = @"~/_CONTROLTEMPLATES/15/PortalDeFluxos.Core.SP/ucCustomGrid.ascx";

        protected Control controlAnexo;
        protected Boolean BtnVisible = true;
        protected EntidadeSP _propostaSP;
        protected String _urlPdf;
        protected double _pdfWidth = 850;

        #region [Configuração Menu]

        protected List<KeyValuePair<String, String>> _listaConfiguracaoMenu;
        protected String _idMenuAtivo;
        protected Boolean _exibirMenuTarefa = true;
        protected String TituloFormulario;

        #endregion

        #region [ Eventos de Controle ]

        public event Action IniciandoFormulario;
        public event Action IniciandoPermissao;
        public event Func<Boolean, KeyValuePair<Boolean, String>> SalvandoDados;

        #endregion

        #region [Fluxo]

		protected bool AcaoFluxo(EntidadePropostaSP entidade, Boolean iniciarFluxo, String nomeFluxo = "", Guid idLista = new Guid())
		{
			bool retorno = false;
			PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
			{
				if (iniciarFluxo)
					retorno = entidade.IniciarFluxo(nomeFluxo, idLista);
				else
				{
					ReiniciarFluxo(entidade, nomeFluxo, idLista);
					retorno = true;
				}
			});
			return retorno;
		}

		public void ReiniciarFluxo(EntidadePropostaSP entidade, String nomeFluxo = "", Guid idLista = new Guid())
		{
			ControleRaizenForm controleAtual = new ControleRaizenForm(this);
			if (controleAtual.ReiniciarWorkflow())
				entidade.ReiniciarFluxo(nomeFluxo, idLista);
		}

        #endregion

        #region [Comum]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salvarFormulario"></param>
        /// <param name="?"></param>
        protected void CarregarFormulario(List<KeyValuePair<String, String>> listaConfiguracaoMenu, String idMenuAtivo, String tituloFormulario, Action loadForm, Action loadPermissao, Func<Boolean, KeyValuePair<Boolean, String>> salvarFormulario)
        {
            bool retorno = true;

            try
            {
                this.SalvandoDados += salvarFormulario;
                this.IniciandoPermissao += loadPermissao;
                this.IniciandoFormulario += loadForm;

                if (SalvandoDados == null)
                    throw new Exception("Evento Salvar não registrado!");
                if (IniciandoPermissao == null)
                    throw new Exception("Evento Permissao não registrado!");
                if (IniciandoFormulario == null)
                    throw new Exception("Evento Iniciar Formulario não registrado!");

                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
                {
                    _listaConfiguracaoMenu = listaConfiguracaoMenu;
                    _idMenuAtivo = idMenuAtivo;
                    TituloFormulario = tituloFormulario;
                    IniciandoPermissao();           

                    if (!this.IsPostBack)
                        IniciandoFormulario();
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno)
                MensagemAlerta(retorno, false);
        }

		protected void Salvar(Boolean iniciarFluxo, bool exibirMensagem = true, bool anexoFacil = false)
        {
            bool retorno = true;
            String messageCustom = String.Empty;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
                {
                    PortalWeb.ContextoWebAtual.IniciarTransacao();//iniciar transação - Habilita a possibilidade de controlar transação SP

                    KeyValuePair<Boolean, String> retornoSalvar = SalvandoDados(iniciarFluxo);

					if (!anexoFacil)
						new ControleAnexo(controlAnexo).SalvarAnexos(SPContext.Current.Web.Url);
                    retorno = retornoSalvar.Key;

                    if (retornoSalvar.Key)
                        PortalWeb.ContextoWebAtual.ConfirmarMudancas();//Confirma as mudanças
                    else
                    {
                        iniciarFluxo = retornoSalvar.Key;
                        messageCustom = retornoSalvar.Value;
                    }

                    if (iniciarFluxo && _propostaSP != null)
                        NegocioPdf.GerarPdf(_propostaSP.ObterNomeLista(), _propostaSP.ID, _propostaSP.Titulo.ToFileName("pdf")
                            , String.Format(_urlPdf, SPContext.Current.Web.Url, _propostaSP.ID), _pdfWidth);
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (exibirMensagem)
                MensagemAlerta(retorno, messageCustom, iniciarFluxo);
        }

        /// <summary>
        /// Chamada tratada - Abre contexto e loga erros - se ocorre erro - mostra sempre a mensagem
        /// </summary>
        /// <param name="evento"></param>
        /// <param name="exibirMensagem"></param>
        protected void EventoTratado(Func<KeyValuePair<Boolean, String>> evento, bool exibirMensagem = true)
        {
            bool retorno = true;
            String messageCustom = String.Empty;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
                {
                    KeyValuePair<Boolean, String> retornoSalvar = evento();
                    messageCustom = retornoSalvar.Value;
                    retorno = retornoSalvar.Key;
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }


            if (!retorno || exibirMensagem)
                MensagemAlerta(retorno, messageCustom);
            else
                RegisterClientScript("raizen.goTopOnEndRequest = false;");
        }

        /// <summary>
        /// Chamada tratada - Abre contexto e loga erros - se ocorre erro - mostra sempre a mensagem
        /// </summary>
        /// <param name="evento"></param>
        /// <param name="objeto1">Objeto necessário pelo método</param>
        /// <param name="exibirMensagem"></param>
        protected void EventoTratado(Func<object, KeyValuePair<Boolean, String>> evento, bool exibirMensagem, object objeto1)
        {
            bool retorno = true;
            String messageCustom = String.Empty;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
                {
                    KeyValuePair<Boolean, String> retornoSalvar = evento(objeto1);
                    messageCustom = retornoSalvar.Value;
                    retorno = retornoSalvar.Key;
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno || exibirMensagem)
                MensagemAlerta(retorno, messageCustom);
            else
                RegisterClientScript("raizen.goTopOnEndRequest = false;");
        }

        /// <summary>
        /// Chamada tratada - Abre contexto e loga erros - se ocorre erro - mostra sempre a mensagem
        /// </summary>
        /// <param name="evento"></param>
        /// <param name="objeto1">Objeto necessário pelo método</param>
        /// <param name="objeto2">Objeto necessário pelo método</param>
        /// <param name="exibirMensagem"></param>
        protected void EventoTratado(Func<object, object, KeyValuePair<Boolean, String>> evento, bool exibirMensagem, object objeto1, object objeto2)
        {
            bool retorno = true;
            String messageCustom = String.Empty;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
                {
                    KeyValuePair<Boolean, String> retornoSalvar = evento(objeto1, objeto2);
                    messageCustom = retornoSalvar.Value;
                    retorno = retornoSalvar.Key;
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno || exibirMensagem)
                MensagemAlerta(retorno, messageCustom);
            else
                RegisterClientScript("raizen.goTopOnEndRequest = false;");
        }

		protected void ReiniciarFluxoForm()
		{
			bool retorno = false;
			try
			{
				using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url, true))
				{
					EntidadePropostaSP item = null;
					if (CodigoItem > 0)
						item = NegocioComum.ObterProposta(CodigoLista, CodigoItem);

					retorno = item != null ? AcaoFluxo(item, true, idLista: CodigoLista) : false;
				}
			}
			catch (Exception ex)
			{
				LogarErro(SPContext.Current.Web.Url, ex);
			}

			if (retorno)
				MensagemAlerta(retorno, "Fluxo reiniciado.", true);
			else
				MensagemAlerta(retorno, "Falha ao reiniciar o fluxo. Entre em contato com um administrador.", false);
		}

        protected void Cancelar()
        {
            this.Page.Response.Redirect(Source);
        }

        protected void EnviarEmailAnexo()
        {
            String emailUsuario = SPContext.Current.Web.CurrentUser.Email;
            Boolean retorno = true;
            if (String.IsNullOrEmpty(emailUsuario))
            {
                MensagemAlerta(false, MensagemPortal.UsuarioSemEmail.GetTitle());
            }
            else
            {
                try
                {
                    if (CodigoItem > 0)
                        using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url))
                        {
                            NegocioComum.EnviarEmailAttachment(CodigoLista, CodigoItem, emailUsuario);
                        }
                }
                catch (Exception ex)
                {
                    retorno = false;
                    LogarErro(SPContext.Current.Web.Url, ex);
                }

                if (retorno)
                    MensagemAlerta(true, MensagemPortal.EmailAnexoSucesso.GetTitle());
                else
                    MensagemAlerta(retorno);
            }
        }

        protected void RegisterClientScript(string script)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page
                , this.GetType()
                , "Script-block", "$(document).ready(function () {" + script + "});", true);

        }

        #endregion

        #region [Anexo]

        protected void CarregarUcAnexos(Control divAnexos)
        {
            controlAnexo = (Control)this.Page.LoadControl(_ascxPathAnexos);
            divAnexos.Controls.Add(controlAnexo);
        }

        #endregion

        #region [Estrutura Comercial]

        #region [Buscar Estrutura Comercial]

        protected void CarregarDadosUcBuscarEstruturaComercial(Control ucBuscarEstruturaComercial, Int32? valorIbmSiteCode, Boolean buscarPorIbm = true, Boolean readOnly = false, String tipoProposta = "")
        {
            if (ucBuscarEstruturaComercial != null)
            {
                ControleBuscarDadosComercial wControleBuscarEstruturaComercial = new ControleBuscarDadosComercial(ucBuscarEstruturaComercial);
                wControleBuscarEstruturaComercial.CarregarControle(valorIbmSiteCode, buscarPorIbm, readOnly, tipoProposta);
            }
        }

        protected Control CarregarUcBuscarEstruturaComercial(Control divBuscarEstruturaComercial, Boolean obrigatorio = true)
        {
            Control controle = (Control)this.Page.LoadControl(_ascxPathBuscarEstruturaComercial);
            divBuscarEstruturaComercial.Controls.Add(controle);

            if (!obrigatorio)
            {
                ControleBuscarDadosComercial wControleBuscarEstruturaComercial = new ControleBuscarDadosComercial(controle);
                wControleBuscarEstruturaComercial.DesabilitarObrigatoriedade();
            }

            return controle;
        }

        #endregion

        #region [Estrutura Cdr Gdr]

        protected void CarregarDadosUcEstruturaCdrGdr(Control ucEstruturaCdrGdr
            , String loginCdr = "", String loginGdr = ""
            , Boolean rezoneamentoPadrao = true, Boolean cdrHabilitado = false, Boolean gdrHabilitado = false)
        {
            if (ucEstruturaCdrGdr != null)
            {
                ControleEstruturaCdrGdr wControleEstruturaCdrGdr = new ControleEstruturaCdrGdr(ucEstruturaCdrGdr);
                wControleEstruturaCdrGdr.CarregarControle(rezoneamentoPadrao, loginCdr, loginGdr, cdrHabilitado, gdrHabilitado);
            }
        }

        protected Control CarregarUcEstruturaCdrGdr(Control divEstruturaCdrGdr, Boolean cdrObrigatorio = true, Boolean gdrObrigatorio = true)
        {
            Control controle = (Control)this.Page.LoadControl(_ascxPathEstruturaCdrGdr);
            divEstruturaCdrGdr.Controls.Add(controle);

            ControleEstruturaCdrGdr wControleEstruturaCdrGdr = new ControleEstruturaCdrGdr(controle);
            wControleEstruturaCdrGdr.ControlesObrigatorios(cdrObrigatorio, gdrObrigatorio);

            return controle;
        }

        #endregion

        #region [Estrutura GtGrDiretor]

        protected void CarregarDadosUcEstruturaGtGrDiretor(Control ucEstruturaGtGrDiretor, Boolean rezoneamentoPadrao = true
            , String loginGt = "", String loginGr = "", String loginDiretor = ""
            , Boolean rezoneamentoHabilidato = true, Boolean gtHabilitado = false, Boolean grHabilitado = false, Boolean diretorHabilitado = false)
        {
            if (ucEstruturaGtGrDiretor != null)
            {
                ControleEstruturaGtGrDiretor wControleEstruturaGtGrDiretor = new ControleEstruturaGtGrDiretor(ucEstruturaGtGrDiretor);
                wControleEstruturaGtGrDiretor.CarregarControle(rezoneamentoPadrao
                        , loginGt, loginGr, loginDiretor, rezoneamentoHabilidato, gtHabilitado, grHabilitado, diretorHabilitado);
            }
        }

        protected Control CarregarUcEstruturaGtGrDiretor(Control divEstruturaGtGrDiretor, Boolean gtObrigatorio = true, Boolean grObrigatorio = true, Boolean diretorObrigatorio = true)
        {
            Control controle = (Control)this.Page.LoadControl(_ascxPathEstruturaGtGrDiretor);
            divEstruturaGtGrDiretor.Controls.Add(controle);

            ControleEstruturaGtGrDiretor wControleEstruturaGtGrDiretor = new ControleEstruturaGtGrDiretor(controle);
            wControleEstruturaGtGrDiretor.ControlesObrigatorios(gtObrigatorio, grObrigatorio, diretorObrigatorio);

            return controle;
        }

        #endregion

        #endregion

        #region [Estrutura Individual]

        protected void HabilitarUcEstruturaindividual(Control ucEstruturaIndividual, Boolean habilitado)
        {
            if (ucEstruturaIndividual != null)
            {
                ControleEstruturaIndividual wControleEstruturaIndividual = new ControleEstruturaIndividual(ucEstruturaIndividual);
                wControleEstruturaIndividual.ControlesHabilitados(wControleEstruturaIndividual.UtilizaRezoneamento(), habilitado);
            }
        }

		protected void RecarregarDadosUcEstruturaindividual(Control ucEstruturaIndividual, String nomeNivel,
			String loginResponsavelNivel = "", Boolean rezoneamentoHabilidato = true, Boolean rezoneamentoVisivel = false)
		{
			if (ucEstruturaIndividual != null)
			{
				ControleEstruturaIndividual wControleEstruturaIndividual = new ControleEstruturaIndividual(ucEstruturaIndividual);
				wControleEstruturaIndividual.CarregarControle(wControleEstruturaIndividual.UtilizaRezoneamento()
					, nomeNivel, loginResponsavelNivel, rezoneamentoHabilidato, rezoneamentoVisivel);
			}

		}
        protected void CarregarDadosUcEstruturaindividual(Control ucEstruturaIndividual, String nomeNivel,
            String loginResponsavelNivel = "", Boolean rezoneamentoHabilidato = true, Boolean rezoneamentoVisivel = false,
            Boolean? rezoneamentoPadrao = null)
        {
            if (ucEstruturaIndividual != null)
            {
                ControleEstruturaIndividual wControleEstruturaIndividual = new ControleEstruturaIndividual(ucEstruturaIndividual);
                wControleEstruturaIndividual.CarregarControle(rezoneamentoPadrao == null ?
                    wControleEstruturaIndividual.UtilizaRezoneamento() : (Boolean)rezoneamentoPadrao
                    , nomeNivel, loginResponsavelNivel, rezoneamentoHabilidato, rezoneamentoVisivel);
            }
        }

        protected Control CarregarUcEstruturaIndividual(PlaceHolder plhEstruturaIndividual, Boolean nivelObrigatorio = true)
        {
            Control controle = (Control)this.Page.LoadControl(_ascxPathEstruturaIndividual);
            plhEstruturaIndividual.Controls.Add(controle);

            ControleEstruturaIndividual wControleEstruturaIndividual = new ControleEstruturaIndividual(controle);
            wControleEstruturaIndividual.ControlesObrigatorios(nivelObrigatorio);

            return controle;
        }

        #endregion

        #region [Ano Contratual]

        protected Control CarregarAnoContratual(Control controleAnoContratual)
        {
            Control control = (Control)this.Page.LoadControl(_ascxPathAnoContratual);
            controleAnoContratual.Controls.Add(control);
            return control;
        }

        protected ControleAnoContratual CarregarControlAnoContratual(Control control, bool obrigatorio = false, int? qntCamposHabilitados = null)
        {
            ControleAnoContratual controlAnos = new ControleAnoContratual(control);
            if (obrigatorio)
                controlAnos.DefinirCamposRequired(qntCamposHabilitados);
            return controlAnos;
        }

        protected Dictionary<int, decimal?> ObterValoresAnoContratual(Control controle, Boolean anoZero = false)
        {
            return CarregarControlAnoContratual(controle).ObterValores(anoZero);
        }

        #endregion

        #region [Custom Grid]

        protected Control CarregarCustomGrid(Control controleCustomGrid)
        {
            Control control = (Control)this.Page.LoadControl(_ascxPathCustomGrid);
            controleCustomGrid.Controls.Add(control);
            return control;
        }

        protected ControleCustomGrid GetControlCustomGridWrapper(Control control)
        {
            ControleCustomGrid controlGrid = new ControleCustomGrid(control);
            return controlGrid;
        }

        #endregion
    }
}
