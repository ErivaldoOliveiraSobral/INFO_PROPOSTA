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

namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public class CustomApproveForm : CustomUserControl
    {
        #region [Atributos]

        private String IdBtnPesquisar = String.Empty;

        private Int32 _idTarefa = 0;

        public Int32 IdTarefa
        {
            get
            {
                if (_idTarefa > 0)
                    return _idTarefa;
                Int32.TryParse(this.Page.Request.QueryString["idTarefa"], out _idTarefa);
                return _idTarefa;
            }
            set
            {
                _idTarefa = value;
            }
        }

        protected Tarefa _tarefa;

        protected TarefaHist _tarefaHist;

        protected Boolean PermissaoAprovador
        {
            get
            {
                Boolean _permissao = false;
                if (_tarefa != null)
                {
                    _permissao = string.Equals(_tarefa.Contexto.UsuarioAtual.Login, _tarefa.LoginResponsavel, StringComparison.InvariantCultureIgnoreCase);
                    if (!_permissao)
                    {
                        Int32 idGrupoTarefa = 0;
                        if (Int32.TryParse(_tarefa.LoginResponsavel, out idGrupoTarefa))
                        {
                            _permissao = PermissaoGruposUsuario.Contains(idGrupoTarefa);
                            _tarefaGrupo = true;
                        }

                    }
                }
                return _permissao;
            }
        }

        private Boolean _tarefaGrupo = false;

        protected Boolean TarefaGrupo
        {
            get
            {
                return _tarefaGrupo;
            }
        }

        #endregion [Fim - Atributos]

        #region [ Eventos de Controle ]

        public event Action IniciandoFormulario;
        public event Action IniciandoPermissao;
        public event Action SalvandoDados;
        public event Action IniciandoFormularioDelegacao;
        public event Action SalvandoDelegacaoDados;
        public event Action IniciandoFormularioDiscussao;
        public event Action SalvandoDiscussao;

        #endregion

        #region [Fluxo]

        private void AndarFluxo()
        {
            if (_tarefa.AprovacaoPor != (Int32)TipoAprovacao.Discussao)
                new InstanciaFluxo().Obter(_tarefa.IdInstanciaFluxo).AndarFluxo(_tarefa,true);
        }

        #endregion

        #region [Comum]

        public void IniciarControle(Button btnPesquisar)
        {
            IdBtnPesquisar = String.Format("{0}{1}", "#", btnPesquisar.ClientID);
        }

        protected void CarregarTarefa(HiddenField hdnIdTarefa, UpdatePanel updatePanel)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pweb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;
                    IniciandoFormulario();
                    IniciandoPermissao();
                    updatePanel.Update();
                }
                AbrirModalAprovacao();
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno)
                MensagemAlertaApproveForm(retorno);
        }

        protected void CarregarDelegacao(HiddenField hdnIdTarefa, UpdatePanel updatePanelDelegacao)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pweb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;
                    IniciandoFormularioDelegacao();
                    updatePanelDelegacao.Update();
                }
                AbrirModalDelegacao();
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno)
                MensagemAlertaApproveForm(retorno);
        }

        protected void CarregarDiscussao(HiddenField hdnIdTarefa, UpdatePanel updatePanelDiscussao)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pweb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;
                    IniciandoFormularioDiscussao();
                    updatePanelDiscussao.Update();
                }
                AbrirModalDiscussao();
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno)
                MensagemAlertaApproveForm(retorno);
        }

        /// <summary>
        /// Registra os eventos comuns
        /// </summary>
        /// <param name="iniciandoFormulario"></param>
        /// <param name="iniciandoPermissao"></param>
        /// <param name="salvandoDados"></param>
        protected void CarregarFormulario(Action iniciandoFormulario, Action iniciandoPermissao, Action salvandoDados, Action iniciandoFormularioDelegacao
            , Action salvandoDelegacaoDados, Action iniciandoFormularioDiscussao, Action salvandoDiscussaoDados)
        {
            bool retorno = true;

            try
            {
                this.SalvandoDados += salvandoDados;
                this.IniciandoPermissao += iniciandoPermissao;
                this.IniciandoFormulario += iniciandoFormulario;
                this.IniciandoFormularioDelegacao += iniciandoFormularioDelegacao;
                this.SalvandoDelegacaoDados += salvandoDelegacaoDados;
                this.IniciandoFormularioDiscussao += iniciandoFormularioDiscussao;
                this.SalvandoDiscussao += salvandoDiscussaoDados;

                if (SalvandoDados == null)
                    throw new Exception("Evento Salvar não registrado!");
                if (IniciandoPermissao == null)
                    throw new Exception("Evento Permissao não registrado!");
                if (IniciandoFormulario == null)
                    throw new Exception("Evento Iniciar Formulario não registrado!");
                if (IniciandoFormularioDelegacao == null)
                    throw new Exception("Evento Iniciar Formulario Delegação não registrado!");
                if (SalvandoDelegacaoDados == null)
                    throw new Exception("Evento Salvar Delegacão não registrado!");
                if (IniciandoFormularioDiscussao == null)
                    throw new Exception("Evento Iniciar Formulario Discussão não registrado!");               
                if (SalvandoDiscussao == null)
                    throw new Exception("Evento Salvar Discussão não registrado!");

            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            if (!retorno)
                MensagemAlerta(retorno, false);
        }

        protected void Salvar(HiddenField hdnIdTarefa)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    PortalWeb.ContextoWebAtual.IniciarTransacao();//iniciar transação - Habilita a possibilidade de controlar transação SP

                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;

                    if (_tarefa != null)
                    {
                        SalvandoDados();
                        AndarFluxo();
                    }

                    PortalWeb.ContextoWebAtual.ConfirmarMudancas();//Confirma as mudanças
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            MensagemAlertaApproveForm(retorno);
        }

        protected void Delegar(HiddenField hdnIdTarefa)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    PortalWeb.ContextoWebAtual.IniciarTransacao();//iniciar transação - Habilita a possibilidade de controlar transação SP

                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;

                    if (_tarefa != null)
                        SalvandoDelegacaoDados();

                    PortalWeb.ContextoWebAtual.ConfirmarMudancas();//Confirma as mudanças
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            MensagemAlertaApproveForm(retorno);
        }

        protected void Discutir(HiddenField hdnIdTarefa)
        {
            Int32 idTarefa = 0;
            if (Int32.TryParse(hdnIdTarefa.Value, out idTarefa))
            {
                IdTarefa = idTarefa;
            }
            else
            {
                MensagemAlertaApproveForm("Formulário de aprovação não foi configurado corretamente.");
                return;
            }

            bool retorno = true;
            try
            {
                using (PortalWeb pWeb = new PortalWeb(SPContext.Current.Web.Url))
                {
                    PortalWeb.ContextoWebAtual.IniciarTransacao();//iniciar transação - Habilita a possibilidade de controlar transação SP

                    _tarefa = IdTarefa > 0 ? new Tarefa().Obter(IdTarefa) : null;

                    if (_tarefa != null)
                        SalvandoDiscussao();

                    PortalWeb.ContextoWebAtual.ConfirmarMudancas();//Confirma as mudanças
                }
            }
            catch (Exception ex)
            {
                retorno = false;
                LogarErro(SPContext.Current.Web.Url, ex);
            }

            MensagemAlertaApproveForm(retorno);
        }

        protected void MensagemAlertaApproveForm(String mensagem)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "aprovacaoDelegacaoTarefa.message.exibirNotificacao"
                , String.Format("aprovacaoDelegacaoTarefa.message.exibirNotificacao({0},'{1}','{2}');"
                    , false.ToString().ToLower()
                    , mensagem
                    , IdBtnPesquisar)
                , true);
        }

        protected void MensagemAlertaApproveForm(Boolean status)
        {
            String mensagem = status ? MensagemPortal.Sucesso.GetTitle() : MensagemPortal.Erro.GetTitle();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "aprovacaoDelegacaoTarefa.message.exibirNotificacao"
                , String.Format("aprovacaoDelegacaoTarefa.message.exibirNotificacao({0},'{1}','{2}','{3}');"
                    , status.ToString().ToLower()
                    , mensagem
                    , IdBtnPesquisar
                    , SPContext.Current.Web.Url)
                , true);
        }

        protected void AbrirModalAprovacao()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "aprovacaoDelegacaoTarefa.events.abrirModalAprovacao"
                , String.Format("aprovacaoDelegacaoTarefa.events.abrirModalAprovacao();")
                , true);
        }

        protected void AbrirModalDelegacao()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "aprovacaoDelegacaoTarefa.events.abrirModalDelegacao"
                , String.Format("aprovacaoDelegacaoTarefa.events.abrirModalDelegacao();")
                , true);
        }


        protected void AbrirModalDiscussao()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "aprovacaoDelegacaoTarefa.events.abrirModalDiscussao"
                , String.Format("aprovacaoDelegacaoTarefa.events.abrirModalDiscussao();")
                , true);
        }

        #endregion

    }
}
