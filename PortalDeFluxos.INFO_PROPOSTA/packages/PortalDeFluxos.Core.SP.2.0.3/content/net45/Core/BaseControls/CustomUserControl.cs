using Iteris;
using Microsoft.SharePoint;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using PortalDeFluxos.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public class CustomUserControl : ControleBase
    {
        #region [Propriedades]

        private ControleRaizenForm _wControleRaizenForm = null;

        private SPRoleDefinitionBindingCollection _userListRoles
        {
            get
            {
                return SPContext.Current.List.AllRolesForCurrentUser;
            }
        }

        private List<Int32> _userGroupsId = null;

        private Guid _codigoLista = Guid.Empty;

        private EntidadePropostaSP _proposta = null;

        public Int32 CodigoItem
        {
            get
            {
                Int32 codigoItem = 0;
                if (!Int32.TryParse(this.Page.Request.QueryString["ID"], out codigoItem))
                {
                    if (_wControleRaizenForm == null)
                        _wControleRaizenForm = new ControleRaizenForm(this);
                    codigoItem = !Int32.TryParse(this.Page.Request.QueryString["CodigoItem"], out codigoItem) ? _wControleRaizenForm.CodigoItemNovo() : codigoItem;
                }
                return codigoItem;
            }
            set
            {
                if (_wControleRaizenForm == null)
                    _wControleRaizenForm = new ControleRaizenForm(this);
                _wControleRaizenForm.SetCodigoItemNovo(value);
            }
        }

        public String UrlContextWeb
        {
            get
            {
                return SPContext.Current.Web.Url;
            }
        }

        public String Source
        {
            get
            {
                String source = String.Empty;
                if (this.Page.Request.QueryString["Source"] != null)
                    source = this.Page.Request.QueryString["Source"].ToString();
                else source = SPContext.Current.Web.Url;
                return source;
            }
        }

        public Guid CodigoLista
        {
            get
            {
                if (_codigoLista == Guid.Empty)
                    _codigoLista = SPContext.Current.List != null ? SPContext.Current.List.Title != "Páginas do Site" ? SPContext.Current.List.ID : Guid.Empty : Guid.Empty;

                if (_codigoLista != Guid.Empty)
                    return _codigoLista;

                if (ViewState["CodigoLista_" + Page.ClientID] == null)
                {
                    Guid guid = Guid.Empty;
                    if (Guid.TryParse(this.Page.Request.QueryString["List"], out guid))
                        ViewState["CodigoLista_" + Page.ClientID] = guid;
                    else if (Guid.TryParse(this.Page.Request.QueryString["CodigoLista"], out guid))
                        ViewState["CodigoLista_" + Page.ClientID] = guid;
                    else
                        ViewState["CodigoLista_" + Page.ClientID] = Guid.Empty.ToString();
                }
                return new Guid(ViewState["CodigoLista_" + Page.ClientID].ToString());
            }
            set
            {
                ViewState["CodigoLista_" + Page.ClientID] = value;
                _codigoLista = value;
            }
        }

        public Boolean EdicaoItem
        {
            get
            {
                return SPContext.Current.FormContext.FormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit;
            }
        }

        public Boolean PropostaEmAprovacao
        {
            get
            {
                Boolean _propostaEmAprovacao = false;
                using (PortalWeb.ContextoWebAtual == null ? new PortalWeb(SPContext.Current.Web.Url) : null)
                {
                    _propostaEmAprovacao = CodigoItem > 0 ? NegocioInstanciaFluxo.PropostaEmAprovacao(CodigoLista, CodigoItem) : false;
                }

                return _propostaEmAprovacao;
            }
        }

        public Boolean FluxoAtivo
        {
            get
            {
                Boolean _fluxoAtivo = false;
                using (PortalWeb.ContextoWebAtual == null ? new PortalWeb(SPContext.Current.Web.Url) : null)
                {
                    PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
                    {
                        _fluxoAtivo = CodigoItem > 0 ? NegocioInstanciaFluxo.FluxoAtivo(CodigoLista, CodigoItem) : false;
                    });
                }
                return _fluxoAtivo;
            }
        }

        public Boolean FluxoFinalizado
        {
            get
            {
                Boolean _fluxoFinalizado = false;
                using (PortalWeb.ContextoWebAtual == null ? new PortalWeb(SPContext.Current.Web.Url) : null)
                {
                    if (CodigoItem > 0)
                    {
                        EntidadePropostaSP proposta = NegocioComum.ObterProposta(CodigoLista, CodigoItem);
                        _fluxoFinalizado = proposta != null ? proposta.EstadoAtualFluxo == StatusProposta.Finalizada.ToString() ||
                            proposta.Etapa == StatusProposta.Finalizada.ToString() : false;
                    }
                }
                return _fluxoFinalizado;
            }
        }

        /// <summary>
        /// Para informações - se a mesma poderá ser editada
        /// </summary>
        public Boolean PermissaoEditar
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Contributor ||
                    item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.RaizenColaborador.GetTitle() ||
                    item.Name == PortalRoles.CancelarFluxo.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Para informações - se a mesma estará disponivel
        /// </summary>
        public Boolean PermissaoVisualizar
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Contributor ||
                    item.Type == SPRoleType.Administrator ||
                    item.Type == SPRoleType.Reader ||
                    item.Name == PortalRoles.RaizenColaborador.GetTitle() ||
                    item.Name == PortalRoles.CancelarFluxo.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Para fluxo - se o usuário pode cancelar o fluxo.
        /// </summary>
        public Boolean PermissaoCancelarFluxo
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.CancelarFluxo.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Para itens - se a opção de deletar estará disponivel
        /// </summary>
        public Boolean PermissaoDeletar
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Contributor ||
                    item.Type == SPRoleType.Administrator)
                    : false;
            }
        }

        /// <summary>
        /// Para arquivos - se a opção de deletar Anexo está disponível 
        /// </summary>
        public Boolean PermissaoExcluirAnexo
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Contributor ||
                    item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.ExcluirAnexo.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Para arquivos - se a opção de Incluir Anexo está disponível 
        /// </summary>
        public Boolean PermissaoIncluirAnexo
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Contributor ||
                    item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.IncluirAnexo.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Tarefa - Permissão para delegar qualquer tarefa da lista
        /// </summary>
        public Boolean PermissaoDelegarTarefa
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.DelegarTarefa.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Usuário Administrador - Se tem controle total na lista.
        /// </summary>
        public Boolean PermissaoAdministrador
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Administrator)
                    : false;
            }
        }

        /// <summary>
        /// SAE - se o usuário pode obter SAE
        /// </summary>
        public Boolean PermissaoObterSAE
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.ObterSAE.GetTitle())
                    : false;
            }
        }

        /// <summary>
        /// Estrutura Comercial 
        /// - Caso o usuário está na estrutura comercial do item.
        /// - Ou se está na role RaizenEstruturaComercial na criação do item
        /// </summary>
        public Boolean PermissaoEstruturaComercial
        {
            get
            {
                return _userListRoles != null ? PermissaoAdministrador || !_userListRoles.Cast<SPRoleDefinition>().Any(
                        item => item.Name == PortalRoles.RaizenEstruturaComercial.GetTitle()) || PermissaoItemEstruturaComercial()
                    : false;
            }
        }

        public List<Int32> PermissaoGruposUsuario
        {
            get
            {
                if (_userGroupsId == null)
                {
                    _userGroupsId = new List<Int32>();
                    foreach (SPGroup grupo in SPContext.Current.Web.CurrentUser.Groups)
                        _userGroupsId.Add(grupo.ID);
                }

                return _userGroupsId;
            }
        }

        public Boolean PermissaoGridviewDocumentos
        {
            get
            {
                return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
                    item => item.Type == SPRoleType.Administrator ||
                    item.Name == PortalRoles.GridviewDocumentos.GetTitle())
                    : false;
            }
        }

		public Boolean PermissaoIncluirComentario
		{
			get
			{
				return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
					item => item.Type == SPRoleType.Administrator ||
					item.Name == PortalRoles.IncluirComentario.GetTitle())
					: false;
			}
		}

		public Boolean PermissaoEnviarAprovacao
		{
			get
			{
				return _userListRoles != null ? _userListRoles.Cast<SPRoleDefinition>().Any(
					item => item.Type == SPRoleType.Administrator ||
					item.Name == PortalRoles.EnviarAprovacao.GetTitle())
					: false;
			}
		}

        public Boolean ExibirAnexoFacil
        {
            get
            {
                if (_wControleRaizenForm == null)
                    _wControleRaizenForm = new ControleRaizenForm(this);
                return _wControleRaizenForm.ExibirAnexoFacil();
            }
        }

        #endregion

        public CustomUserControl()
        {
        }

        #region [Comum]

        /// <summary>
        /// Se o Item já existe Verifica se o usuário atual está na estrutura comercial do item
        /// Se o item não existe retorna true
        /// Se o item não for do tipo Proposta retorna false
        /// </summary>
        /// <returns></returns>
        private Boolean PermissaoItemEstruturaComercial()
        {
            Boolean _permissaoItem = false;
            try
            {
                using (PortalWeb.ContextoWebAtual == null ? new PortalWeb(SPContext.Current.Web.Url) : null)
                {
                    if (EdicaoItem)
                    {
                        _proposta = NegocioComum.ObterProposta(SPContext.Current.ListId, SPContext.Current.ItemId);

                        _permissaoItem = (_proposta.Gdr != null && _proposta.Gdr.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
                           (_proposta.Cdr != null && _proposta.Cdr.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
                           (_proposta.DiretorVendas != null && _proposta.DiretorVendas.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
                           (_proposta.GerenteTerritorio != null && _proposta.GerenteTerritorio.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
                           (_proposta.GerenteRegiao != null && _proposta.GerenteRegiao.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login);
                    }
                    else
                        _permissaoItem = true;
                }
            }
            catch { }

            return _permissaoItem;
        }

        protected void LogarErro(String url, Exception ex,String mensagem = "")
        {
            using (PortalWeb pWeb = new PortalWeb(url))
            {
                if (String.IsNullOrEmpty(mensagem))
                    LogarErro(ex);
                else
                    LogarErro(ex,mensagem);
            }
        }

        protected void LogarErro(Exception ex, String mensagem)
        {
            new Log().Inserir(Origem.RaizenForm
                    , mensagem
                    , ex);
        }

        protected void LogarErro(Exception ex)
        {
            if (CodigoLista != Guid.Empty)
                new Log().Inserir(Origem.RaizenForm
                    , String.Format("ID:{0} List:{1}", CodigoItem.ToString(), CodigoLista.ToString())
                    , ex);
            else
                new Log().Inserir(Origem.RaizenForm
                    , "CustomForm"
                    , ex);
        }

        protected void MensagemAlerta(Boolean status, Boolean redirect = false)
        {
            String mensagem = status ? MensagemPortal.Sucesso.GetTitle() : MensagemPortal.Erro.GetTitle();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "raizen.message.ExibirNotificacao"
                , String.Format("raizen.message.ExibirNotificacao({0},'{1}','{2}');"
                    , status.ToString().ToLower()
                    , mensagem
                    , redirect ? string.IsNullOrWhiteSpace(Source) ? SPContext.Current.Web.Url : Source : String.Empty)
                , true);
        }

        protected void MensagemAlerta(Boolean status, String message, Boolean redirect = false)
        {
			String mensagem = message != String.Empty ? message : status ? MensagemPortal.Sucesso.GetTitle() : MensagemPortal.Erro.GetTitle();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "raizen.message.ExibirNotificacao"
                , String.Format("raizen.message.ExibirNotificacao({0},'{1}','{2}');"
                    , status.ToString().ToLower()
                    , mensagem
                    , redirect ? string.IsNullOrWhiteSpace(Source) ? SPContext.Current.Web.Url : Source : String.Empty)
                , true);

        }

        protected void ExecutarScript(String script, params String[] values)
        {
            if(values != null)
                script = String.Format(script,values);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ExecutarScript", script, true);
        }
		
		public static void DisableAllControls(Control parentControl, bool enabled, bool force = false)
		{
			force = force == false ? !enabled : force;
			if (parentControl is TextBox)
				((TextBox)parentControl).Enabled = ((TextBox)parentControl).Enabled != enabled && force ? enabled : ((TextBox)parentControl).Enabled;
			else if (!(parentControl is Button) &&
				!(parentControl is LinkButton) &&
				!(parentControl is ImageButton))
			{
				Type type = parentControl.GetType();
				PropertyInfo prop = type.GetProperty("Enabled");

				// Set it to False to disable the control.
				if (prop != null)
				{
					if ((Boolean)prop.GetValue(parentControl) != enabled && force)
						prop.SetValue(parentControl, enabled, null);
				}
			}
			foreach (Control control in parentControl.Controls)
				DisableAllControls(control, enabled, force);
		}

        #endregion

    }
}
