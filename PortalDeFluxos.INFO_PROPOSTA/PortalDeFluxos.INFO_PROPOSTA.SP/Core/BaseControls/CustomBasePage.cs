using Microsoft.SharePoint;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Web.UI;
using Iteris;
using Microsoft.SharePoint.WebControls;
using PortalDeFluxos.Core.BLL;

namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public class CustomBasePage : LayoutsPageBase
    {

        #region [Propriedades]
        public String UrlLista
        {
            get
            {
                String codigoLista = String.Empty;
                return codigoLista;
            }
        }

        public Int32 CodigoItem
        {
            get
            {
                Int32 codigoItem = 0;
                Int32.TryParse(this.Page.Request.QueryString["ID"], out codigoItem);
                return codigoItem;
            }
        }

        public Guid CodigoLista
        {
            get
            {
                
                if (ViewState["CodigoLista_" + Page.ClientID] == null)
                {
                    Guid guid = Guid.Empty;
                    if (Guid.TryParse(this.Page.Request.QueryString["List"], out guid))
                        ViewState["CodigoLista_" + Page.ClientID] = guid;
                    else
                        ViewState["CodigoLista_" + Page.ClientID] = Guid.Empty.ToString();
                }
                return new Guid(ViewState["CodigoLista_" + Page.ClientID].ToString());
            }
            set { ViewState["CodigoLista_" + Page.ClientID] = value; }
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

        #endregion

        public CustomBasePage()
        {
            
        }

        #region [Comum]

        protected void LogarErro(Exception ex)
        {
            new Log().Inserir(Origem.RaizenForm
                    ,   String.Format("ID:{0} List:{1}", CodigoItem.ToString(), CodigoLista.ToString())
                    ,   ex);
        }

        protected void LogarErro(String url,Exception ex)
        {
            using(PortalWeb pweb = new PortalWeb(url))
            {
                new Log().Inserir(Origem.RaizenForm
                    , String.Format("ID:{0} List:{1}", CodigoItem.ToString(), CodigoLista.ToString())
                    , ex);
            }
        }

        protected void MensagemAlerta(Boolean status)
        {
            String mensagem = status ? MensagemPortal.Sucesso.GetTitle() : MensagemPortal.Erro.GetTitle();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "raizen.message.ExibirNotificacao"
                , String.Format("raizen.message.ExibirNotificacao({0},'{1}','{2}');"
                    , status.ToString().ToLower()
                    , mensagem
                    , string.IsNullOrWhiteSpace(Source) ? SPContext.Current.Web.Url : Source)
                , true);

        }

        protected void MensagemAlerta(Boolean status, String message)
        {
            String mensagem = message;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "raizen.message.ExibirNotificacao"
                , String.Format("raizen.message.ExibirNotificacao({0},'{1}','{2}');"
                    , status.ToString().ToLower()
                    , mensagem
                    , string.IsNullOrWhiteSpace(Source) ? SPContext.Current.Web.Url : Source)
                , true);

        }
        #endregion
    }
}
