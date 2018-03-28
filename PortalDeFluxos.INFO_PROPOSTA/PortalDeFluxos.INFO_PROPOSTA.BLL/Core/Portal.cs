using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Linq;
using System.Collections.Generic;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System.Security.Principal;
using Winnovative.WnvHtmlConvert;
using System.Web.Hosting;
using System.Web;
using System.Web.Caching;
using Microsoft.SharePoint.Client.Utilities;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace PortalDeFluxos.Core.BLL
{
    /// <summary>Classe para manipular o contexto dos objetos da thread</summary>
    public class PortalWeb : IDisposable
    {
        #region [ Propriedades do Portal Web ]
        /// <summary>Propriedade para controle do contexto web atual</summary>
        [ThreadStatic]
        public static PortalWeb ContextoWebAtual = null;

        [ThreadStatic]
        /// <summary>Contexto atual da thread</summary>
        private static Stack<PortalWeb> OldContext = null;

        /// <summary>Se deve executar a aplicação com o Contexto do usuário local</summary>
        private ContextoUsuarioSP _contextoUsuarioSP = null;

        /// <summary>Lista para cache de usuários no Contexto (Que foram carregados do Sharepoint)</summary>
        private List<Usuario> _usuarios = new List<Usuario>();

        public String Url { get; set; }

        private ClientContext _spClient = null;
        /// <summary>Objeto de contexto para acesso ao Sharepoint</summary>
        public ClientContext SPClient
        {
            get
            {
                if (_spClient == null)
                    _spClient = new ClientContext(Url);
                return _spClient;
            }
        }

        private Web _web = null;
        /// <summary>Objeto de acesso ao Portal Web</summary>
        public Web SPWeb
        {
            get
            {
                if (_web == null)
                    _web = SPClient.Web;
                return _web;
            }
        }

        private ContextoTransacional _transacao = null;
        /// <summary>Contexto para manipular a transação</summary>
        public ContextoTransacional Transacao { get { return _transacao; } }

        /// <summary>Contexto para manipular a transação SP - Apenas Update</summary>
        private TransacaoUpdateSP _transacaoUpdateSP = null;

        private Usuario _usuarioAtual;
        public Usuario UsuarioAtual
        {
            get
            {
                if (_usuarioAtual == null)
                {
                    String usuarioAtual = System.Threading.Thread.CurrentPrincipal.Identity != null && !String.IsNullOrWhiteSpace(System.Threading.Thread.CurrentPrincipal.Identity.Name) ?
                                  System.Threading.Thread.CurrentPrincipal.Identity.Name :
                                  Environment.UserName;

                    _usuarioAtual = BuscarUsuarioPorNomeLogin(usuarioAtual);
                }

                return _usuarioAtual;
            }
        }

        private String _userContentTypeId = null;
        /// <summary>Objeto de acesso ao Portal Web</summary>
        public String UserContentTypeId
        {
            get
            {
                if (_userContentTypeId == null)
                {
                    _userContentTypeId = NegocioComum.ObterContentTypeId("Person");
                    _userContentTypeId = _userContentTypeId == "" ?
                        NegocioComum.ObterContentTypeId("Pessoa") : _userContentTypeId;
                }
                return _userContentTypeId;
            }

        }

        public DirectoryEntry DiretorioADRaiz
        {
            get
            {
                return new DirectoryEntry(Domain.GetCurrentDomain().GetDirectoryEntry().Path
                    , String.Format("{0}\\{1}",this.Configuracao.ContaPrivilegioElevado.Domain,this.Configuracao.ContaPrivilegioElevado.UserName)
                    , this.Configuracao.ContaPrivilegioElevado.Password
                    , AuthenticationTypes.Secure);
            }
        }

        #region [ Configurações básicas da aplicação ]
        /// <summary>Bloqueador para implementação do Singleton</summary>
        private static readonly Object bloqueador = new Object();

        /// <summary>Configuração da aplicação</summary>
        private static volatile ConfiguracaoPortal _configuracao = null;

        /// <summary>Configuração da aplicação</summary>
        public ConfiguracaoPortal Configuracao
        {
            get
            {
                if (_configuracao == null)
                {
                    //Valida a URL da aplicação
                    if (String.IsNullOrWhiteSpace(this.Url))
                        throw new NullReferenceException("Não foi possível encontrar a URL da aplicação");

                    lock (bloqueador)
                    {
                        if (_configuracao == null)
                            _configuracao = new ConfiguracaoPortal(this);
                    }
                }
                return _configuracao;
            }
        }

        private PdfConverter _pdfConverter = null;

        /// <summary>Configuração da aplicação</summary>
        public PdfConverter ConfiguracaoPdf
        {
            get
            {
                if (_pdfConverter == null)
                {
                    _pdfConverter = new PdfConverter
                    {
                        ScriptsEnabled = true,
                        ScriptsEnabledInImage = true,
                        NavigationTimeout = 10000
                    };
                    _pdfConverter.PdfDocumentOptions.SinglePage = true;
                    _pdfConverter.PdfDocumentOptions.FitHeight = true;
                    _pdfConverter.PdfDocumentOptions.FitWidth = true;
                    _pdfConverter.PdfDocumentOptions.StretchToFit = false;

                    _pdfConverter.LicenseKey = Configuracao.PdfKey;
                    _pdfConverter.AuthenticationOptions.Username = Configuracao.ContaPrivilegioElevado.UserName;
                    _pdfConverter.AuthenticationOptions.Password = Configuracao.ContaPrivilegioElevado.Password;
                }
                return _pdfConverter;
            }
        }
        #endregion
        #endregion

        /// <summary>Cria um novo contexto Web (Executa as operações no Sharepoint com o usuário do AppPool / Conta de Serviço)</summary>
        /// <param name="url">URL de contexto</param>
        public PortalWeb(String url)
            : this(url, false)
        {
        }

        /// <summary>Cria um novo contexto Web (Permite executar as operações no Sharepoint usando o contexto do usuário logado)</summary>
        /// <param name="url">URL de contexto</param>
        /// <param name="usarContextoUsuarioSP">
        /// Define se as chamadas para o Sharepoint devem usar o contexto do usuário logado (Default não - usa o usuário configurado no AppPool / Seviço).
        /// Para ativar, é necessário que o WIF esteja configurado no servidor e que o serviço (Claims to Windows Token Service) esteja executando.
        /// </param>
        public PortalWeb(String url, Boolean usarContextoUsuarioSP)
        {
            //Permite a abertura de N níveis de Contexto
            if (ContextoWebAtual != null)
            {
                if (OldContext == null)
                    OldContext = new Stack<PortalWeb>();
                OldContext.Push(ContextoWebAtual);
            }

            //Inicia o contexto
            this.Url = url;

            //Define o contexto atual
            ContextoWebAtual = this;

            //Se solicitado, cria o contexto do usuário local
            if (usarContextoUsuarioSP)
                _contextoUsuarioSP = new ContextoUsuarioSP();
        }

        /// <summary>
        /// Obtém a instância da classe modelo solicitada
        /// </summary>
        /// <typeparam name="T">Classe do tipo entidade</typeparam>
        /// <returns>Objeto solicitado</returns>
        public T Obter<T>()
            where T : Entidade, new()
        {
            T objeto = Activator.CreateInstance<T>();
            objeto.Contexto = this;
            return objeto;
        }

        #region [ Manipulação de Grupos ]

        /// <summary>Efetua a busca de Grupos no Sharepoint</summary>
        /// <param name="nomeOuLogin">Nome ou ID para a Busca</param>
        /// <param name="usarCache">Se deve usar o cache de Contexto</param>
        /// <returns>Retorna o grupo encontrado</returns>
        public Grupo BuscarGrupo(int idGrupo, Boolean logar = true)
        {
            //Usuário para ser retornado
            Grupo item = null;

            if (idGrupo <= 0)
                return item;

            //Busca o grupo no Sharepoint e retorna
            var grupoSP = this.SPWeb.SiteGroups.GetById(idGrupo);

            try
            {
                this.SPClient.Load(grupoSP);
                this.SPClient.Load(grupoSP.Users);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarGrupo", String.Format("Falha ao buscar grupo de ID: {0}", idGrupo.ToString()), ex);
                grupoSP = null;
            }

            return grupoSP;
        }

        public List<Grupo> BuscarGrupos(int? idGrupo = null, Boolean logar = true)
        {
            List<Grupo> grupos = new List<Grupo>();

            if (idGrupo != null)
            {
                Grupo grupoFiltro = BuscarGrupo((Int32)idGrupo, logar);
                grupos.Add(grupoFiltro);
                return grupos;
            }

            GroupCollection groups = this.SPWeb.SiteGroups;
            try
            {
                this.SPClient.Load(groups);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarGrupos", "Falha ao buscar grupos.", ex);
                groups = null;
            }

            if (groups != null)
            {
                foreach (Group group in groups)
                {
                    this.SPClient.Load(group.Users);
                    this.SPClient.ExecuteQuery();
                    Grupo grupo = new Grupo(group);
                    grupos.Add(grupo);
                }
            }

            return grupos;
        }

        public List<Grupo> BuscarGruposPorEmail(String emailUsuario)
        {
            if (String.IsNullOrEmpty(emailUsuario))
                return null;

            List<Grupo> gruposUsuario = new List<Grupo>();

            //Busca o usuário no Sharepoint e retorna
            User usuarioSP = this.SPWeb.SiteUsers.GetByEmail(emailUsuario);
            // TODO: Verificar se faz sentido a próxima linha existir. Ela não existia no projeto RNIP. Diferença detectada durante o merging dos códigos.
            User usuarioAuthenticated = PortalWeb.ContextoWebAtual.SPWeb.EnsureUser("NT AUTHORITY\\authenticated users");

            try
            {
                GroupCollection gruposSP = usuarioSP.Groups;
                this.SPClient.Load(gruposSP);
                this.SPClient.ExecuteQuery();

                foreach (Group grupoSP in gruposSP)
                {
                    try
                    {
                        this.SPClient.Load(grupoSP.Users);
                        this.SPClient.ExecuteQuery();
                        gruposUsuario.Add(grupoSP);
                    }
                    catch (Exception ex)
                    {
                        new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar um dos grupos do usuário : {0}", emailUsuario), ex);
                        throw;
                    }
                }
                // TODO: Verificar se faz sentido as próximas 17 linhas existirem. Elas não existiam no projeto RNIP. Diferença detectada durante o merging dos códigos.
                GroupCollection gruposUsuarioAuthenticated = usuarioAuthenticated.Groups;
                PortalWeb.ContextoWebAtual.SPClient.Load(gruposUsuarioAuthenticated);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                foreach (Group grupoSP in gruposUsuarioAuthenticated)
                {
                    try
                    {
                        this.SPClient.Load(grupoSP.Users);
                        this.SPClient.ExecuteQuery();
                        gruposUsuario.Add(grupoSP);
                    }
                    catch (Exception ex)
                    {
                        new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar um dos grupos do usuário : {0}", emailUsuario), ex);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar grupos do usuário: {0}", emailUsuario), ex);
                throw;
            }
            return gruposUsuario;
        }

        public List<Grupo> BuscarGruposPorLogin(String loginUsuario)
        {
            if (String.IsNullOrEmpty(loginUsuario))
                return null;

            List<Grupo> gruposUsuario = new List<Grupo>();

            //Busca o usuário no Sharepoint e retorna
            User usuarioSP = this.SPWeb.EnsureUser(loginUsuario.RemoverClaimsAndDomain());
            // TODO: Verificar se faz sentido a próxima linha existir. Ela não existia no projeto RNIP. Diferença detectada durante o merging dos códigos.            
            User usuarioAuthenticated = PortalWeb.ContextoWebAtual.SPWeb.EnsureUser("NT AUTHORITY\\authenticated users");

            try
            {
                GroupCollection gruposSP = usuarioSP.Groups;
                this.SPClient.Load(gruposSP);
                this.SPClient.ExecuteQuery();

                foreach (Group grupoSP in gruposSP)
                {
                    try
                    {
                        this.SPClient.Load(grupoSP.Users);
                        this.SPClient.ExecuteQuery();
                        gruposUsuario.Add(grupoSP);
                    }
                    catch (Exception ex)
                    {
                        new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar um dos grupos do usuário : {0}", loginUsuario), ex);
                        throw;
                    }

                }

                // TODO: Verificar se faz sentido as próximas 17 linhas existirem. Elas não existiam no projeto RNIP. Diferença detectada durante o merging dos códigos.
                GroupCollection gruposUsuarioAuthenticated = usuarioAuthenticated.Groups;
                PortalWeb.ContextoWebAtual.SPClient.Load(gruposUsuarioAuthenticated);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                foreach (Group grupoSP in gruposUsuarioAuthenticated)
                {
                    try
                    {
                        this.SPClient.Load(grupoSP.Users);
                        this.SPClient.ExecuteQuery();
                        gruposUsuario.Add(grupoSP);
                    }
                    catch (Exception ex)
                    {
                        new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar um dos grupos do usuário : {0}", loginUsuario), ex);
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                new Log().Inserir("BuscarGruposPorLogin", String.Format("Falha ao buscar grupos do usuário: {0}", loginUsuario), ex);
                throw;
            }
            return gruposUsuario;
        }

        public UsuarioGrupoBase BuscarUsuarioGrupo(FieldUserValue value, Boolean logar = true)
        {
            UsuarioGrupoBase usuarioGrupo = null;
            String tipoFieldUser = GetUserFieldType(value);
                
            if (tipoFieldUser == "SharePointGroup")
                usuarioGrupo = BuscarGrupo(((FieldUserValue)value).LookupId, logar);
			else
				usuarioGrupo = BuscarUsuario(((FieldUserValue)value).LookupId, true, logar);

            return usuarioGrupo;
        }

        private string GetUserFieldType(FieldUserValue value, Boolean reloadCache)
        {
            const String cacheKey = "UsersContentType";
            bool hasHttpContext = HttpContext.Current != null;
            Boolean isCached =
                HostingEnvironment.IsHosted &&
                hasHttpContext &&
                HttpContext.Current.Cache[cacheKey] != null;
            Dictionary<Int32, ContentTypeId> userContentTypeCacheDictionary = null;
            if (!isCached || reloadCache)
            {
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"
                    <View>
                    <ViewFields><FieldRef Name='ID'/><FieldRef Name='ContentTypeId'/></ViewFields>
                    </View>
                ";
                ListItemCollection users = SPClient.Web.SiteUserInfoList.GetItems(query);
                SPClient.Load(users);
                SPClient.ExecuteQuery();
                userContentTypeCacheDictionary = new Dictionary<Int32, ContentTypeId>();
                foreach (ListItem user in users)
                    userContentTypeCacheDictionary.Add(user.Id, (ContentTypeId)user["ContentTypeId"]);
                if (HostingEnvironment.IsHosted && hasHttpContext)
                    HttpContext.Current.Cache.Add(
                        cacheKey,
                        userContentTypeCacheDictionary,
                        null,
                        Cache.NoAbsoluteExpiration,
                        Cache.NoSlidingExpiration,
                        CacheItemPriority.Normal,
                        null
                    );
            }
            Dictionary<Int32, ContentTypeId> userContentTypeIds = HttpContext.Current != null ?
                HttpContext.Current.Cache[cacheKey] as Dictionary<Int32, ContentTypeId> :
                userContentTypeCacheDictionary;
            if (userContentTypeIds == null)
                return null;
            if (!userContentTypeIds.ContainsKey(value.LookupId) && !reloadCache)
                return GetUserFieldType(value, true);
            return userContentTypeIds.ContainsKey(value.LookupId) ?
                (userContentTypeIds[value.LookupId].ToString().StartsWith(UserContentTypeId) ?
                "Person" : "SharePointGroup") : null;
        }

        public string GetUserFieldType(FieldUserValue value)
        {
            return GetUserFieldType(value, false);
        }

        #endregion

        #region [ Manipulação de Usuário ]

        public List<Usuario> BuscarUsuarios(int? idUsuario = null, Boolean logar = true)
        {
            List<Usuario> usuarios = new List<Usuario>();

            if (idUsuario != null)
            {
                Usuario usuarioFiltro = BuscarUsuario((Int32)idUsuario, logar: logar);
                usuarios.Add(usuarioFiltro);
                return usuarios;
            }

            UserCollection users = this.SPWeb.SiteUsers;
            try
            {
                this.SPClient.Load(users);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarUsuarios", "Falha ao buscar usuarios.", ex);
                users = null;
            }

            if (users != null)
            {
                foreach (Usuario user in users)
                    usuarios.Add(user);
            }

            return usuarios;
        }

        /// <summary>Efetua a busca de usuários no Sharepoint</summary>
        /// <param name="idUsuario"id do usuário</param>
        /// <param name="usarCache">Se deve usar o cache de Contexto</param>
        /// <returns>Retorna o usuário encontrado</returns>
        public Usuario BuscarUsuario(Int32 idUsuario, Boolean usarCache = true, Boolean logar = true)
        {
            //Usuário para ser retornado
            Usuario item = null;
            if (idUsuario <= 0)
                return item;

            //Verifica se deve usar Cache do contexto local
            if (usarCache)
                item = _usuarios.Where(i => i.Id == idUsuario).FirstOrDefault();

            //Caso tenha encontrado no contexto, retorna
            if (item != null)
                return item;

            //Busca o usuário no Sharepoint e retorna
            var usuarioSP = this.SPWeb.SiteUsers.GetById(idUsuario);
            try
            {
                this.SPClient.Load(usuarioSP);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarUsuario", String.Format("Falha ao buscar usuário: {0}", idUsuario.ToString()), ex);
                usuarioSP = null;
            }

            if (usuarioSP == null)
                return null;

            //Adiciona na lista de Cache local e retorna
            _usuarios.Add(usuarioSP);
            return usuarioSP;
        }

        /// <summary>Efetua a busca de usuários no Sharepoint</summary>
        /// <param name="nomeOuLogin">Nome ou Login para a Busca</param>
        /// <param name="usarCache">Se deve usar o cache de Contexto</param>
        /// <returns>Retorna o usuário encontrado - pode retornar usuário errado caso o existir usuários com o mesmo nome</returns>
        public Usuario BuscarUsuarioPorPropriedadeItem(ListItem item, String propriedade, Boolean usarCache = true, Boolean logar = true)
        {

            String valor = String.Empty;
            if (String.IsNullOrEmpty(propriedade))
                return null;
            if (item[propriedade] == null)
                return null;
            Int32 idUsuario = -1;
            if (item[propriedade] is FieldUserValue)
                idUsuario = (item[propriedade] as FieldUserValue).LookupId;

            return BuscarUsuario(idUsuario, usarCache, logar);
        }

        /// <summary>Efetua a busca de usuários no Sharepoint</summary>
        /// <param name="nomeOuLogin">Nome ou Login para a Busca</param>
        /// <param name="usarCache">Se deve usar o cache de Contexto</param>
        /// <returns>Retorna o usuário encontrado - pode retornar usuário errado caso o existir usuários com o mesmo nome</returns>
        public Usuario BuscarUsuarioPorNomeLogin(String nomeOuLogin, Boolean usarCache = true, Boolean logar = true)
        {
            //Usuário para ser retornado
            Usuario item = null;

            if (String.IsNullOrWhiteSpace(nomeOuLogin))
                return item;

            //Verifica se deve usar Cache do contexto local
            if (usarCache)
                item = _usuarios.Where(i => String.Equals(i.Login, nomeOuLogin.RemoverClaimsAndDomain(), StringComparison.InvariantCultureIgnoreCase) ||
                                           String.Equals(i.Nome, nomeOuLogin.RemoverClaimsAndDomain(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            //Caso tenha encontrado no contexto, retorna
            if (item != null)
                return item;

            //Busca o usuário no Sharepoint e retorna
            var usuarioSP = this.SPWeb.EnsureUser(nomeOuLogin.RemoverClaimsAndDomain());
            try
            {
                this.SPClient.Load(usuarioSP);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarUsuario", String.Format("Falha ao buscar usuário: {0}", nomeOuLogin), ex);
                usuarioSP = null;
            }

            if (usuarioSP == null)
                return null;

            //Adiciona na lista de Cache local e retorna
            _usuarios.Add(usuarioSP);
            return usuarioSP;
        }

        public Usuario BuscarUsuarioPorEmail(String email, Boolean usarCache = true, Boolean logar = true)
        {
            //Usuário para ser retornado
            Usuario item = null;

            if (String.IsNullOrWhiteSpace(email) || !email.IsEmail())
                return item;

            //Verifica se deve usar Cache do contexto local
            if (usarCache)
                item = _usuarios.Where(i => String.Equals(i.Email, email.RemoverClaimsAndDomain(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            //Caso tenha encontrado no contexto, retorna
            if (item != null)
                return item;

            //Busca o usuário no Sharepoint e retorna
            #region [Buscar login por email]
            var result = Utility.ResolvePrincipal(PortalWeb.ContextoWebAtual.SPClient
                        , PortalWeb.ContextoWebAtual.SPWeb, email
                        , Microsoft.SharePoint.Client.Utilities.PrincipalType.User
                        , Microsoft.SharePoint.Client.Utilities.PrincipalSource.All
                        , PortalWeb.ContextoWebAtual.SPWeb.SiteUsers, true);

            String loginName = String.Empty;
            try
            {
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                loginName = result.Value != null ? result.Value.LoginName : email;

                if (loginName == email)
                {
                    var usuarioTemp = PortalWeb.ContextoWebAtual.SPWeb.SiteUsers.GetByEmail(email);
                    PortalWeb.ContextoWebAtual.SPClient.Load(usuarioTemp);
                    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                    loginName = usuarioTemp != null ? usuarioTemp.LoginName : "";
                }
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarUsuarioPorEmail", String.Format("Falha ao buscar usuário: {0}", email), ex);
            }
            #endregion

            #region [Buscar Usuario]
            if (String.IsNullOrEmpty(loginName))
                return null;

            var usuarioSP = this.SPWeb.EnsureUser(loginName.RemoverClaimsAndDomain());
            try
            {
                this.SPClient.Load(usuarioSP);
                this.SPClient.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (logar)
                    new Log().Inserir("BuscarUsuario", String.Format("Falha ao buscar usuário: {0}", loginName), ex);
                usuarioSP = null;
            }
            #endregion

            if (usuarioSP == null)
                return null;

            //Adiciona na lista de Cache local e retorna
            _usuarios.Add(usuarioSP);
            return usuarioSP;
        }

        #endregion

        #region [ Manipula a transação das operações ]
        /// <summary>Inicia a transação</summary>
        public Boolean IniciarTransacao()
        {
            if (_transacao != null)
                return false;

            //Inicia a transação
            _transacao = new ContextoTransacional(this);
            _transacaoUpdateSP = new TransacaoUpdateSP();

            return true;
        }

        public void AdicionarTransacaoSP(EntidadeSP item)
        {
            if (_transacaoUpdateSP == null)
                throw new Exception("Transação não foi iniciada previamente.");
            _transacaoUpdateSP.Adicionar(item);
        }

        /// <summary>Efetua o commit</summary>
        public void ConfirmarMudancas()
        {
            if (_transacao == null)
                return;

            //Destroi o objeto
            _transacao.ConfirmarMudancas();
            _transacao.Dispose();
            _transacao = null;
            _transacaoUpdateSP = null;
        }

        /// <summary>Efetua o rollback</summary>
        public void CancelarMudancas()
        {
            if (_transacao == null)
                return;

            if (_transacaoUpdateSP != null)
                _transacaoUpdateSP.Cancelar();

            //OldContext.ToList().ForEach(i => i.CancelarMudancas());

            //Destroi o objeto
            _transacao.CancelarMudancas();
            _transacao.Dispose();
            _transacao = null;
            _transacaoUpdateSP = null;
        }
        #endregion

        #region [Tradutores]

        public String TraduzirTags(Int32 idTarefa, String texto)
        {
            return NegocioTradutorTags.TraduzirTags(idTarefa, texto);
        }

        public String TraduzirTags(Dictionary<TipoTag, Object> objetos, String texto)
        {
            return NegocioTradutorTags.TraduzirTags(objetos, texto);
        }
        #endregion [Fim - Tradutores]

        #region [Manipulação de objetos Sharepoint]
        /// <summary>Executa a ação com uma conta de usuário admin</summary>
        /// <param name="acao">Ação</param>
        public void ExecutarComPrivilegioElevado(Action acao)
        {
            if (acao == null)
                return;

            using (PortalWeb pWebAdmin = new PortalWeb(PortalWeb.ContextoWebAtual.Url))
            {
                PortalWeb.ContextoWebAtual.SPClient.Credentials = new System.Net.NetworkCredential(this.Configuracao.ContaPrivilegioElevado.UserName, this.Configuracao.ContaPrivilegioElevado.Password, this.Configuracao.ContaPrivilegioElevado.Domain);
                acao();
            }
        }

        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="nomeLista">Obtem a lista através do nome da lista</param>
        /// <returns></returns>
        public List ObterLista(String nomeLista)
        {
            return BaseSP.ObterLista(nomeLista);
        }

        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="Id"></param>
        /// <returns>Obtem a lista através do nome da lista</returns>
        public List ObterLista(Guid Id)
        {
            return BaseSP.ObterLista(Id);
        }

        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="urlLista"></param>
        /// <returns>Url lista: Lists/AditivosGerais</returns>
        public List ObterListaPorUrl(String urlLista)
        {
            return BaseSP.ObterListaPorUrl(urlLista);
        }

        public ListItem ObterItem(Guid codigoLista, Int32 codigoItem)
        {
            return BaseSP.ObterItem(codigoLista, codigoItem);
        }

        #endregion [Fim - Manipulação de objetos Sharepoint]

        #region [ Liberação do objeto da memória ]
        ~PortalWeb()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        /// <param name="disposing">Define se a destruição foi realizada explicitamente.</param>
        internal void Dispose(bool disposing)
        {
            if (_transacao != null)
            {
                _transacao.Dispose();
                _transacao = null;
            }

            if (_transacaoUpdateSP != null)
            {
                _transacaoUpdateSP.Cancelar();
                _transacaoUpdateSP = null;
            }

            if (_contextoUsuarioSP != null)
            {
                _contextoUsuarioSP.Dispose();
                _contextoUsuarioSP = null;
            }

            if (_spClient != null)
            {
                _spClient.Dispose();
                _spClient = null;
            }

            if (ContextoWebAtual != null)
                ContextoWebAtual = null;

            if (OldContext != null && OldContext.Count > 0)
                ContextoWebAtual = OldContext.Pop();
        }
        #endregion
        
    }
}