using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    /// <summary>Classe que possui a coleção de configurações disponíveis no contexto do portal</summary>
    public class ConfiguracaoPortal
    {
        /// <summary>Def</summary>
        private PortalWeb _parentWeb = null;

        /// <summary>String de conexão para o banco de dados</summary>
        internal string ConnectionString { get; private set; }
                
        /// <summary>Conta para elevar o privilégio</summary>
        internal NetworkCredential ContaPrivilegioElevado { get; set; }

        /// <summary>Conta ambiente2007</summary>
        public NetworkCredential ContaSp2007 { get; set; }

        /// <summary>Tamanho limite em mb dos documentos no Sp2007</summary>
        public Int32 SizeDocument2007 { get; set; }

        /// <summary>Webservice para adicionar anexo no ambiente 2007</summary>
        public string UrlWsAnexoEmail2007 { get; set; }

        /// <summary>Webservice para adicionar anexo no ambiente 2013</summary>
        public string UrlWsAnexoEmail2013 { get; set; }

        /// <summary>Usuários utilizado somente para testes</summary>
        public UsuariosTeste UsuariosTeste { get; set; }

        /// <summary>Configuração de IMAP (EmailApproval)</summary>
        public ConfiguracaoImap Imap { get; private set; }

        /// <summary>Configuração de IMAP (EmailAttachment)</summary>
        public ConfiguracaoImap ImapAttachment { get; private set; }
        /// <summary>Site utilizado para armazenar os documentos do portal</summary>
        public String UrlRecordCenter { get; set; }

        /// <summary>Key utilizado no Pdf Converter
        internal String PdfKey { get; private set; }

        /// <summary>Ambiente do contexto</summary>
        public Ambiente AmbienteAtual { get; set; }

        /// <summary>Carrega as configurações da aplicação</summary>
        public ConfiguracaoPortal(PortalWeb pWeb)
        {
            //Define o Contexto
            this._parentWeb = pWeb;
            Web web = _parentWeb.SPClient.Site.RootWeb;

            //Busca as configurações definidas no SiteCollection
            _parentWeb.SPClient.Load(web, i => i.AllProperties);
            _parentWeb.SPClient.ExecuteQuery();

            //Carrega a ConnectionString
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebConnectionString))
            {
                ConnectionString = (String)web.AllProperties[Constantes.ChaveWebConnectionString];

                if (ConnectionString.ToUpper().IndexOf("METADATA=RES://*//;") == -1)
                    ConnectionString = String.Format("metadata=res://{0}/Core.Modelo.PortalDeFluxo.csdl|res://{0}/Core.Modelo.PortalDeFluxo.ssdl|res://{0}/Core.Modelo.PortalDeFluxo.msl|res://{0}/Modelo.BancoDados.csdl|res://{0}/Modelo.BancoDados.ssdl|res://{0}/Modelo.BancoDados.msl;provider=System.Data.SqlClient;provider connection string='{1}'"
                        , Assembly.GetCallingAssembly().FullName, ConnectionString);
            }

            //Carrega a configuração de Admin
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebContaAdmin))
            {
                Conta _contaPrivilegioElevado = new Conta(Serializacao.DeserializeFromJson<Dictionary<String, Object>>(
                           (String)pWeb.SPClient.Site.RootWeb.AllProperties[Constantes.ChaveWebContaAdmin]));
                ContaPrivilegioElevado = _contaPrivilegioElevado.NetWorkCredential;
            }

            //Carrega a configuração de Sp2007
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebContaSp2007))
            {
                Conta _contaSp2007 = new Conta(Serializacao.DeserializeFromJson<Dictionary<String, Object>>(
                        (String)pWeb.SPClient.Site.RootWeb.AllProperties[Constantes.ChaveWebContaSp2007]));
                ContaSp2007 = _contaSp2007.NetWorkCredential;
            }

            //Carrega a url ws AnexoEmail
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveUrlWsAnexoEmail2007))
                UrlWsAnexoEmail2007 = (String)web.AllProperties[Constantes.ChaveUrlWsAnexoEmail2007];

            //Carrega a url ws AnexoEmail
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveUrlWsAnexoEmail2013))
                UrlWsAnexoEmail2013 = (String)web.AllProperties[Constantes.ChaveUrlWsAnexoEmail2013];

            //Carrega a configuração do IMAP
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebImap))
                Imap = Serializacao.DeserializeFromJson<ConfiguracaoImap>((String)web.AllProperties[Constantes.ChaveWebImap]);

            //Carrega a configuração dos usuários utilizados nos testes
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebUsariosTeste))
                UsuariosTeste = Serializacao.DeserializeFromJson<UsuariosTeste>((String)web.AllProperties[Constantes.ChaveWebUsariosTeste]);

            //Carrega a configuração do tamanho limite dos arquivo no Sp2007
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveWebSizeDocumento2007))
                SizeDocument2007 = Serializacao.DeserializeFromJson<Int32>((String)web.AllProperties[Constantes.ChaveWebSizeDocumento2007]);

            //Carrega a url Document Center
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveUrlRecordCenter))
                UrlRecordCenter = (String)web.AllProperties[Constantes.ChaveUrlRecordCenter];

            //Carrega Pdf Key
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChavePdf))
                PdfKey = (String)web.AllProperties[Constantes.ChavePdf];

            //Carregar Ambiente configurado
            if (web.AllProperties.FieldValues.ContainsKey(Constantes.ChaveAmbiente))
                AmbienteAtual = (Ambiente)Enum.Parse(typeof(Ambiente), (String)web.AllProperties[Constantes.ChaveAmbiente]);

            //Verifica se a ConnectionString está preenchida corretamente
            if (String.IsNullOrWhiteSpace(this.ConnectionString))
                throw new NullReferenceException("Não foi possível carregar a string de conexão com o banco de dados.");
        }

    }
}