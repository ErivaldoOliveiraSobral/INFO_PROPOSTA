using Iteris;
using Iteris.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioTarefaCustom
    {
        #region [Atributos]

        private static Object _semaforo = new Object();

        #endregion [Fim - Atributos]

        #region [E-mails - Tarefa/Lembrete]

        /// <summary>Busca os e-mails pendentes para envio</summary>
        /// <param name="item">Item</param>
        /// <returns>Lista de e-mails para envio</returns>
        public static List<MensagemEmail> ConsultarEmailsPendentes(this Tarefa item)
        {
            //Busca os e-mails
            return DadosTarefa.ConsultarEmailsPendentes(item);
        }

        /// <summary>Busca os e-mails de lembrete pendentes para envio</summary>
        /// <param name="item">Item</param>
        /// <returns>Lista de e-mails para envio</returns>
        public static List<MensagemEmail> ConsultarLembretesPendentes(this Tarefa item)
        {
            //Busca os e-mails
            return DadosTarefa.ConsultarLembretesPendentes(item);
        }

        /// <summary>Define como enviado os e-mails do lembrete de tarefa</summary>
        /// <param name="item">Item</param>
        /// <param name="chaves">Código das tarefas que o e-mail foi enviado</param>
        public static void AtualizarLembreteEnviado(this Tarefa item, List<int> lembretes)
        {
            //Atualiza os itens
            DadosTarefa.AtualizarLembreteEnviado(item, lembretes);
        }

        /// <summary>Define como enviado os e-mails das tarefas informadas</summary>
        /// <param name="item">Item</param>
        /// <param name="chaves">Código das tarefas que o e-mail foi enviado</param>
        public static void AtualizarEmailEnviado(this Tarefa item, List<int> chaves)
        {
            //Atualiza os itens
            DadosTarefa.AtualizarEmailEnviado(item, chaves);
        }

        #endregion

        #region [Lembrete]

        private static void InserirLembrete(
           Int32 idTarefa,
           List<Lembrete> lembreteTemplate,
           Dictionary<TipoTag, Object> objetos,
           Usuario usuarioAprovacao,
           Grupo grupoAprovacao)
        {
            List<Lembrete> lembretes = new List<Lembrete>();
            lembreteTemplate.ForEach(lembrete =>
            {
                lembretes.Add(
                            new Lembrete()
                            {
                                IdTarefa = idTarefa,
                                LoginPara = usuarioAprovacao != null ? usuarioAprovacao.Login : grupoAprovacao.Id.ToString(),
                                NomePara = usuarioAprovacao != null ? usuarioAprovacao.Nome : grupoAprovacao.Nome,
                                EmailPara = usuarioAprovacao != null ? usuarioAprovacao.Email : grupoAprovacao.ObterEmailUsuarios(),
                                DescricaoAssunto = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, lembrete.DescricaoAssunto),
                                DescricaoMensagem = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, lembrete.DescricaoMensagem),
                                DataEnvio = lembrete.DataEnvio,
                                CopiarSuperior = lembrete.CopiarSuperior
                            }
                          );
            });

            lembretes.Inserir();
        }

        private static List<Lembrete> ObterConfiguracaoTemplate(
            List<ListaSP_RaizenLembretes> lembretes,
            ConfiguracaoExpediente configuracaoExpediente,
            DateTime dataCriacaoTarefa,
            Double slaUtilizado)
        {
            List<Lembrete> lembreteTemplate = new List<Lembrete>();
            if (lembretes != null && lembretes.Count > 0)
            {
                lembreteTemplate = ObterLembreteTemplate(configuracaoExpediente, dataCriacaoTarefa, lembretes, slaUtilizado);
            }

            return lembreteTemplate;
        }
        private static List<Lembrete> ObterLembreteTemplate(
            ConfiguracaoExpediente configuracaoExpediente,
            DateTime dataCriacaoTarefa,
            List<ListaSP_RaizenLembretes> configuracaoLembrete,
            Double slaUtilizado)
        {
            List<Lembrete> lembreteTemplate = new List<Lembrete>();

            foreach (ListaSP_RaizenLembretes configuracao in configuracaoLembrete)
            {
                TimeSpan slaLembrete = new TimeSpan((Int32)configuracao.Dias, (Int32)configuracao.Horas, (Int32)configuracao.Minutos, 0);
                DateTime dataLembrete = dataCriacaoTarefa;
                configuracao.TemplateDeEmail.CarregarDados();

                for (int numeroRepeticoes = 1; numeroRepeticoes <= configuracao.NumeroDeRepeticoes; numeroRepeticoes++)
                {
                    // Cálcular a data que expira o lembrete
                    dataLembrete = DataHelper.CalcularSLA(configuracaoExpediente
                        , dataLembrete
                        , slaLembrete.TotalMinutes
                        , slaUtilizado);

                    // Para Cada item de lembrete gerar um template que será alimentado no momente da criação da tarefa
                    Lembrete lembrete = new Lembrete()
                    {
                        DataEnvio = dataLembrete,
                        DescricaoAssunto = System.Web.HttpUtility.HtmlDecode(configuracao.TemplateDeEmail.Assunto),
                        DescricaoMensagem = System.Web.HttpUtility.HtmlDecode(configuracao.TemplateDeEmail.Corpo),
                        CopiarSuperior = configuracao.CCSuperior == null ? false : (Boolean)configuracao.CCSuperior
                    };

                    lembreteTemplate.Add(lembrete);
                }
            }

            return lembreteTemplate;
        }

        #endregion

        #region [Tarefas]

        public static void AdicionarItemTarefaFluxoReiniciado(String descricaoAcaoEfetuada, String nomeEtapa, String comentarioAprovacao, int IdInstanciaFluxo)
        {
            Tarefa tarefaFluxoReiniciado = new Tarefa();
            tarefaFluxoReiniciado.TarefaCompleta = true;
            tarefaFluxoReiniciado.EmailEnviado = true;
            tarefaFluxoReiniciado.CopiarSuperior = false;
            tarefaFluxoReiniciado.TarefaEscalonada = false;
            tarefaFluxoReiniciado.DataFinalizado = DateTime.Now;
            tarefaFluxoReiniciado.EmailResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Email;
            tarefaFluxoReiniciado.NomeResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Nome;
            tarefaFluxoReiniciado.LoginResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
            tarefaFluxoReiniciado.NomeCompletadoPor = PortalWeb.ContextoWebAtual.UsuarioAtual.Nome;
            tarefaFluxoReiniciado.LoginCompletadoPor = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
            tarefaFluxoReiniciado.DataAtribuido = DateTime.Now;
            tarefaFluxoReiniciado.DescricaoAcaoEfetuada = descricaoAcaoEfetuada;
            tarefaFluxoReiniciado.NomeEtapa = nomeEtapa;
            tarefaFluxoReiniciado.NomeTarefa = nomeEtapa;
            tarefaFluxoReiniciado.ComentarioAprovacao = comentarioAprovacao;
            tarefaFluxoReiniciado.IdInstanciaFluxo = IdInstanciaFluxo;
            tarefaFluxoReiniciado.Inserir();
        }

        public static bool ConsultarTarefasHeadlinePendentes(Tarefa tarefaHeadline, InstanciaFluxo fluxo)
        {
            bool tarefasHeadlinePendentes = false;
            DateTime dataCriacaoTarefa = DateTime.Now;
            Dictionary<String, List<TarefaHist>> tarefaHist = new Dictionary<string, List<TarefaHist>>();

            int headlineIndex = tarefaHeadline.IndexAprovacao ?? default(int);
            headlineIndex++;

            ListItem item;
            ObterInformacoesItemAtual(PortalWeb.ContextoWebAtual.Url, fluxo.CodigoLista, fluxo.CodigoItem, out item);
            int configuracaoID = 0;
            configuracaoID = tarefaHeadline.CodigoConfiguracao ?? default(int);

            //Obter Configurações da Tarefa
            ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(configuracaoID);
            int headlineSize = 0;
            int.TryParse(ObterValorItem(item, configuracaoTarefa.CampoHeadlineSize), out headlineSize);

            ConfiguracaoExpediente configuracaoExpediente = ObterConfiguracaoExpediente(configuracaoTarefa);
            Double slaUtilizado = ObterSlaUtilizado(fluxo, configuracaoTarefa, configuracaoExpediente);

            TipoTarefa tipoTarefa = ObterTipoTarefa(configuracaoTarefa.TipoDeAprovacao);

            //Obter Configurações do Lembrete
            configuracaoTarefa.Lembretes.CarregarDados();
            List<Lembrete> lembreteTemplate = ObterConfiguracaoTemplate(configuracaoTarefa.Lembretes, configuracaoExpediente, dataCriacaoTarefa, slaUtilizado);

            //Se a resposta da tarefa for decisiva para a etapa (encerra a etapa), não é necessário buscar outro aprovador e o fluxo deve andar para tomar a decisão.
            if (configuracaoTarefa.SaidaFinalizarEtapa != null)
            {
                if (!configuracaoTarefa.SaidaFinalizarEtapa.Contains(tarefaHeadline.DescricaoAcaoEfetuada))
                {
                    KeyValuePair<Int32, List<ListaSP_RaizenAprovadores>> infoHeadLine = ObterRaizenAprovadorHeadlineSize(headlineSize, tarefaHeadline.NomeTarefa, headlineIndex, item);

                    if (infoHeadLine.Value.Count > 0)
                    {
                        CriarTarefa(
                            infoHeadLine.Value,
                            (Guid)tarefaHeadline.CodigoTarefa,
                            configuracaoID,
                            dataCriacaoTarefa,
                            item,
                            tipoTarefa,
                            TipoAprovacao.Alcada,
                            lembreteTemplate,
                            PortalWeb.ContextoWebAtual.Url,
                            configuracaoTarefa,
                            configuracaoExpediente,
                            tarefaHist,
                            fluxo,
                            slaUtilizado,
                            infoHeadLine.Key,
                            headlineIndex
                        );

                        tarefasHeadlinePendentes = infoHeadLine.Value.Exists(_ => _.Responsavel != null || _.Grupo != null);
                    }
                }
            }
            return tarefasHeadlinePendentes;
        }

        public static String CriarTarefa(
           String siteURL,
           InstanciaFluxo instanciaAtualDoFluxo,
           Int32 configuracaoTarefaID,
           out String ultimaAcaoRealizada,
           String estadoAtual = "")
        {
            #region [Variáveis]

            #region [Tarefas]
            Guid codigoTarefa;
            DateTime dataCriacaoTarefa = DateTime.Now;
            TipoTarefa tipoTarefa;
            TipoAprovacao tipoAprovacaoPor;
            String nomeTarefa = String.Empty;
            ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa;
            List<ListaSP_RaizenAprovadores> aprovadores;
            Dictionary<String, List<TarefaHist>> tarefaHist = new Dictionary<string, List<TarefaHist>>();
            ConfiguracaoExpediente configuracaoExpediente;
            #endregion [Fim Tarefas]

            #region [Lembretes]

            List<Lembrete> lembreteTemplate = new List<Lembrete>();

            #endregion [Fim - Lembretes]

            #region [Sharepoint]

            ListItem item;

            #endregion [Fim - Sharepoint]

            #endregion [Fim - Variáveis]

            // Obter item atual e lista atual
            ObterInformacoesItemAtual(siteURL, instanciaAtualDoFluxo.CodigoLista, instanciaAtualDoFluxo.CodigoItem, out item);
            if (item == null)
            {
                throw new Exception("Falha ao obter ListItem.");
            }

            //Obter Configurações da Tarefa
            configuracaoTarefa = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(configuracaoTarefaID);

            // Obter a instancia do Fluxo
            instanciaAtualDoFluxo = instanciaAtualDoFluxo.ValidarInstanciaFluxo(dataCriacaoTarefa, item, configuracaoTarefa, "", estadoAtual);

            if (configuracaoTarefa == null)
                throw new Exception(String.Format(MensagemPortal.IDConfiguracaoTarefaNaoEncontrato.GetTitle(), configuracaoTarefaID));

            String codigoTarefaAnterior = String.Empty;
            Tarefa ultimaTarefa = null;
            if (!CriarNovaTarefa(instanciaAtualDoFluxo, configuracaoTarefa, out codigoTarefaAnterior, out ultimaAcaoRealizada, ref ultimaTarefa))//verifica se é necessário criar a tarefa
                return codigoTarefaAnterior;

            //Trata o caso em que a aprovação por tipo alçada deu erro antes de ser criado a proxima tarefa
            //Cria Tarefa Headline quando for necessário
            if (instanciaAtualDoFluxo.ErroCancelado != null && (Boolean)instanciaAtualDoFluxo.ErroCancelado && ultimaTarefa != null
                && ultimaTarefa.CodigoTarefa != null && ConsultarTarefasHeadlinePendentes(ultimaTarefa, instanciaAtualDoFluxo))
                return ultimaTarefa.CodigoTarefa.ToString();

            // Gerar o identificador da tarefa
            lock (_semaforo)
            {
                codigoTarefa = Guid.NewGuid();
            }

            tipoTarefa = ObterTipoTarefa(configuracaoTarefa.TipoDeAprovacao);

            tipoAprovacaoPor = ObterTipoAprovacaoPor(configuracaoTarefa.AprovacaoPor);

            Int32 indexHeadlineFinal = default(int);
            // Obtendo aprovadores
            aprovadores = NegocioTarefaCustom.ObterRaizenAprovadores(configuracaoTarefa, dataCriacaoTarefa, item, tarefaHist, ref indexHeadlineFinal);
            if (aprovadores == null || aprovadores.Count <= 0)
                throw new Exception(MensagemPortal.AprovadorNaoEncontrado.GetTitle());

            // Obter configuração do expediente para o cálculo do SLA, lembrete, etc...
            configuracaoExpediente = ObterConfiguracaoExpediente(configuracaoTarefa);

            Double slaUtilizado = ObterSlaUtilizado(instanciaAtualDoFluxo, configuracaoTarefa, configuracaoExpediente);

            //Obter Configurações do Lembrete
            configuracaoTarefa.Lembretes.CarregarDados();
            //Obter Configurações do TemplateDeEmail
            configuracaoTarefa.TemplateDeEmail.CarregarDados();
            lembreteTemplate = ObterConfiguracaoTemplate(configuracaoTarefa.Lembretes, configuracaoExpediente, dataCriacaoTarefa, slaUtilizado);

            CriarTarefa(
                    aprovadores,
                    codigoTarefa,
                    configuracaoTarefaID,
                    dataCriacaoTarefa,
                    item,
                    tipoTarefa,
                    tipoAprovacaoPor,
                    lembreteTemplate,
                    siteURL,
                    configuracaoTarefa,
                    configuracaoExpediente,
                    tarefaHist,
                    instanciaAtualDoFluxo,
                    slaUtilizado,
                    indexHeadlineFinal
                    );

            return codigoTarefa.ToString();
        }

        private static void CriarTarefa(
            List<ListaSP_RaizenAprovadores> aprovadores,
            Guid codigoTarefa,
            Int32 configuracaoTarefaID,
            DateTime dataCriacaoTarefa,
            ListItem item,
            TipoTarefa tipoTarefa,
            TipoAprovacao tipoAprovacaoPor,
            List<Lembrete> lembreteTemplate,
            String siteURL,
            ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa,
            ConfiguracaoExpediente configuracaoExpediente,
            Dictionary<String, List<TarefaHist>> tarefaHist,
            InstanciaFluxo instanciaAtualDoFluxo,
            Double slaUtilizado,
            Int32? headlineFinal = null,
            Int32? headlineIndex = null
            )
        {
            Dictionary<TipoTag, Object> objetos = new Dictionary<TipoTag, Object>();
            Boolean enviarEmail = true; // Caso no futuro exista configuração de enviar ou não e-mail de tarefa.
            Boolean enviarLembrete = lembreteTemplate != null && lembreteTemplate.Count > 0;
            Boolean aprovadorValido = false;
            String listaDescricaoUrlTarefa = String.Empty;

            // Objetos que serão utilizados na tradução de tags
            if (enviarEmail || enviarLembrete)
            {
                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Item, item);
                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Lista, item.ParentList);

                // Obter Histórico do Fluxo Somente para as tarefas
                instanciaAtualDoFluxo.CarregarHistoricoWorkflow();

                // Monta url da web part de aprovação
                Lista lista = new Lista().Consultar(l => l.CodigoLista == instanciaAtualDoFluxo.CodigoLista).FirstOrDefault();
                if (lista != null)
                    listaDescricaoUrlTarefa = String.Format("{0}", new Uri(new Uri(siteURL), lista.DescricaoUrlItem));
            }

            #region [Cálcular a data que expira o SLA - SLA acumulativo]

            TimeSpan slaTarefa = new TimeSpan(configuracaoTarefa.SLA, 0, 0, 0);
            DateTime dataSLA = DataHelper.CalcularSLA(
                configuracaoExpediente,
                dataCriacaoTarefa,
                slaTarefa.TotalMinutes,
                slaUtilizado
                );

            #endregion

            Usuario superiorTarefa = ObterSuperior(configuracaoTarefa, item);

            String contentType = item.ObterContentTypeId();
            Boolean fecharTransacao = PortalWeb.ContextoWebAtual.IniciarTransacao();

            foreach (ListaSP_RaizenAprovadores aprov in aprovadores)
            {
                Tarefa tarefa = new Tarefa();
                tarefa.IdInstanciaFluxo = instanciaAtualDoFluxo.IdInstanciaFluxo;
                tarefa.CodigoTarefa = codigoTarefa;
                tarefa.CodigoConfiguracao = configuracaoTarefaID; //Id do Item da Lista de configuracao
                tarefa.NomeEtapa = instanciaAtualDoFluxo.NomeEtapa;
                tarefa.TipoTarefa = (byte)tipoTarefa;
                tarefa.AprovacaoPor = (byte)tipoAprovacaoPor;
                tarefa.EmailResponsavel = aprov.Responsavel != null ? aprov.Responsavel.Email : aprov.Grupo.ObterEmailUsuarios();
                tarefa.NomeResponsavel = aprov.Responsavel != null ? aprov.Responsavel.Nome : aprov.Grupo.Nome;
                tarefa.LoginResponsavel = aprov.Responsavel != null ? aprov.Responsavel.Login : aprov.Grupo.Id.ToString();
                tarefa.SLA = configuracaoTarefa.SLA;
                tarefa.DataSLA = dataSLA;
                tarefa.DescricaoAcao = configuracaoTarefa.Saidas != null && configuracaoTarefa.Saidas.Length > 0 ? String.Join(";", configuracaoTarefa.Saidas) : null;
                tarefa.CopiarSuperior = configuracaoTarefa.CopiarSuperior == null ? false : (bool)configuracaoTarefa.CopiarSuperior;
                tarefa.NomeSuperior = superiorTarefa != null ? superiorTarefa.Nome : String.Empty;
                tarefa.EmailSuperior = superiorTarefa != null ? superiorTarefa.Email : String.Empty;
                tarefa.LoginSuperior = superiorTarefa != null ? superiorTarefa.Login : String.Empty;
                tarefa.EmailEnviado = false;
                tarefa.DataAtribuido = dataCriacaoTarefa;
                tarefa.NomeTarefa = configuracaoTarefa.NomeDaTarefa; // Sem traduzir
                tarefa.DescricaoAcaoFinalizarEtapa = configuracaoTarefa.SaidaFinalizarEtapa != null && configuracaoTarefa.SaidaFinalizarEtapa.Length > 0 ? String.Join(";", configuracaoTarefa.SaidaFinalizarEtapa) : null;
                tarefa.IndexAprovacao = 0;
                tarefa.AprovacoesAtuais = true;
                tarefa.IndexAprovacao = headlineIndex;
                tarefa.IndexHeadLineFinal = headlineFinal;
                tarefa.EnviarPdf = configuracaoTarefa.TemplateDeEmail.EnviarPdf;
                tarefa.ParametrosUrl = String.Format("&ContentTypeId={0}", contentType);
				tarefa.ComentarioAprovacaoAutomatica = configuracaoTarefa.ComentarioDaAprovacaoAutomatica;
				tarefa.AprovacaoAutomatica = configuracaoTarefa.AprovacaoAutomatica;
				tarefa.DescricaoRespostaAutomatica = configuracaoTarefa.RespostaAutomatica;
                tarefa.Inserir();

                tarefa.DescricaoUrlTarefa = String.Format("{0}{1}&IdTarefa={2}{3}", listaDescricaoUrlTarefa, item.Id.ToString(), tarefa.IdTarefa, tarefa.ParametrosUrl);

                if (!string.IsNullOrWhiteSpace(tarefa.LoginResponsavel))
                    aprovadorValido = true;

                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Tarefa, tarefa);

                // Atualiza o objeto Tarefa utilizado na tradução de texto, caso exista o nome da tarefa no corpo do e-mail.
                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Tarefa, tarefa);

                if (enviarEmail || enviarLembrete)
                {
                    if (enviarEmail)
                    {
                        NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Fluxo, instanciaAtualDoFluxo);
                        configuracaoTarefa.TemplateDeEmail.CarregarDados();

                        if (configuracaoTarefa.TemplateDeEmail == null)
                        {
                            tarefa.DescricaoAssuntoEmail = String.Format(MensagemPortal.TemplateEmailNaoCadastrado.GetTitle(), configuracaoTarefa.NomeDaTarefa);
                            tarefa.DescricaoMensagemEmail = String.Format(MensagemPortal.TemplateEmailNaoCadastrado.GetTitle(), configuracaoTarefa.NomeDaTarefa);
                        }
                        else
                        {
                            // Traduzir Tags
                            tarefa.DescricaoAssuntoEmail = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(configuracaoTarefa.TemplateDeEmail.Assunto));
                            tarefa.DescricaoMensagemEmail = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(configuracaoTarefa.TemplateDeEmail.Corpo));
                        }

                        //Mensagem Padrão de delegação
                        if (tarefaHist.ContainsKey(tarefa.LoginResponsavel) && tarefaHist[tarefa.LoginResponsavel] != null && tarefaHist[tarefa.LoginResponsavel].Count > 0)
                            tarefa.DescricaoMensagemEmail += ObterTextoDelegacao(tarefaHist[tarefa.LoginResponsavel].First());
                    }

                    // Para os e-mails de lembrete, o histórico do fluxo deve ser carregado no momento de enviar o e-mail
                    NegocioTradutorTags.LimparObjetoTag(objetos, TipoTag.Fluxo);

                    if (enviarLembrete)
                    {
                        InserirLembrete(tarefa.IdTarefa, lembreteTemplate, objetos, aprov.Responsavel, aprov.Grupo);
                    }
                }

                // Atualiza o corpo dos e-mails
                tarefa.Atualizar();

                // Adiciona os históricos de delegação.
                if (tarefaHist.ContainsKey(tarefa.LoginResponsavel) && tarefaHist[tarefa.LoginResponsavel] != null)
                {
                    tarefaHist[tarefa.LoginResponsavel].ForEach(l => l.IdTarefa = tarefa.IdTarefa);
                    tarefaHist[tarefa.LoginResponsavel].Inserir();
                }
            }

            if (aprovadorValido)
            {
                if (fecharTransacao)
                    PortalWeb.ContextoWebAtual.ConfirmarMudancas();
            }
            else
            {
                PortalWeb.ContextoWebAtual.CancelarMudancas();
                throw new Exception(String.Format("Nenhum aprovador válido encontrado."));
            }
        }

		public static List<Tarefa> ConsultarTarefasAprovacaoAutomatica()
		{
			return DadosTarefa.ConsultarTarefasAprovacaoAutomatica();
		}

        #region [Tipo Tarefa e Tipo Aprovador]

        private static TipoTarefa ObterTipoTarefa(String texto)
        {
            TipoTarefa tipoTarefa;

            if (texto.Equals(TipoTarefa.Votacao.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoTarefa = TipoTarefa.Votacao;
            }
            else if (texto.Equals(TipoTarefa.Todos.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoTarefa = TipoTarefa.Todos;
            }
            else
            {
                tipoTarefa = TipoTarefa.Primeiro;
            }

            return tipoTarefa;
        }

        private static TipoAprovacao ObterTipoAprovacaoPor(String texto)
        {
            TipoAprovacao tipoAprovacaoPor;

            if (texto.Equals(TipoAprovacao.Item.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoAprovacaoPor = TipoAprovacao.Item;
            }
            else if (texto.Equals(TipoAprovacao.Grupo.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoAprovacaoPor = TipoAprovacao.Grupo;
            }
            else
            {
                tipoAprovacaoPor = TipoAprovacao.Alcada;
            }

            return tipoAprovacaoPor;
        }

        private static TipoAprovacao ObterTipoAprovacao(String texto)
        {
            TipoAprovacao tipoAprovacao;

            if (texto.Equals(TipoAprovacao.Item.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoAprovacao = TipoAprovacao.Item;
            }
            else if (texto.Equals(TipoAprovacao.Grupo.GetTitle().ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                tipoAprovacao = TipoAprovacao.Grupo;
            }
            else
            {
                tipoAprovacao = TipoAprovacao.Alcada;
            }

            return tipoAprovacao;
        }

        private static List<Tarefa> ConsultarTarefasPorTipo(InstanciaFluxo instanciaAtualDoFluxo, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa)
        {
            return new Tarefa().Consultar(t =>
                            t.IdInstanciaFluxo == instanciaAtualDoFluxo.IdInstanciaFluxo
                        && t.CodigoConfiguracao == configuracaoTarefa.ID
                        && t.Ativo).OrderByDescending(i => i.IdTarefa).ToList();
        }

        #endregion

        #region [Status Tarefas]

        /// <summary>
        /// Retorna true se a tarefa com o CodigoConfiguracao estiver ativa
        /// true: preenche o código Tarefa
        /// false: código tarefa Empty
        /// </summary>
        /// <param name="tarefas">Lista de tarefas de um CodigoConfiguracao específico </param>
        /// <param name="codigoTarefa"></param>
        /// <returns></returns>
        private static Boolean TryGetTarefaAtiva(List<Tarefa> tarefasPorTipo, out String codigoTarefa)
        {
            codigoTarefa = String.Empty;

            if (tarefasPorTipo != null && tarefasPorTipo.FindAll(t => t.Ativo && !t.TarefaCompleta && String.IsNullOrEmpty(t.DescricaoAcaoEfetuada)
                && t.AprovacoesAtuais != null && (Boolean)t.AprovacoesAtuais).Count > 0)//se existe alguma tarefa pendente do tipo CodigoConfiguracao retorna o ultimo CodigoTarefa
                codigoTarefa = tarefasPorTipo.FirstOrDefault().CodigoTarefa.ToString();

            return codigoTarefa != String.Empty;

        }

        /// <summary>
        /// Retorna true se a tarefa com o CodigoConfiguracao já foi realizada alguma vez
        /// true: preenche a ação realizada
        /// false: ação realizada - new acao(){DescricaoAcaoEfetuada="",ComentarioAprovacao=""}
        /// </summary>
        /// <param name="tarefas">Lista de tarefas de um CodigoConfiguracao específico </param>
        /// <param name="configuracaoTarefa"></param>
        /// <param name="ultimaAcaoRealizada"></param>
        /// <returns></returns>
        private static Boolean TryGetUltimaAcaoRealizada(List<Tarefa> tarefasPorTipo, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa, out String ultimaAcaoRealizada, out Tarefa ultimaTarefa)
        {
            Boolean retorno = false;
            ultimaAcaoRealizada = String.Empty;
            List<Tarefa> tarefasRealizadas = null;
            ultimaTarefa = null;

            if (tarefasPorTipo != null)
                tarefasRealizadas = tarefasPorTipo.FindAll(t => t.Ativo && t.TarefaCompleta
                    && !String.IsNullOrEmpty(t.DescricaoAcaoEfetuada)
                    && t.AprovacoesAtuais != null && (Boolean)t.AprovacoesAtuais);

            if (tarefasPorTipo != null && tarefasRealizadas != null && tarefasRealizadas.Count > 0)//se existe alguma tarefa pendente do tipo CodigoConfiguracao retorna o ultimo CodigoTarefa
            {
                if (configuracaoTarefa.SaidaFinalizarEtapa == null
                    || !configuracaoTarefa.SaidaFinalizarEtapa.Contains(tarefasRealizadas.FirstOrDefault().DescricaoAcaoEfetuada))//Caso a ultima tarefa foi uma que finaliza as etapas, ela deve ser criada novamente.
                {
                    retorno = true;
                    ultimaTarefa = tarefasRealizadas.FirstOrDefault();
                    var acao = new
                    {
                        DescricaoAcaoEfetuada = ultimaTarefa.DescricaoAcaoEfetuada,
                        ComentarioAprovacao = ultimaTarefa.ComentarioAprovacao
                    };
                    ultimaAcaoRealizada = new JavaScriptSerializer().Serialize(acao).ToString();
                }
            }

            return retorno;
        }

        /// <summary>
        /// Retorna se é necessario criar uma nova tarefa.
        /// </summary>
        /// <param name="instanciaAtualDoFluxo"></param>
        /// <param name="configuracaoTarefa"></param>
        /// <param name="codigoTarefaAnterior">Utilizada para recriar ação anterior que não tinha sido completada</param>
        /// <param name="acaoRealizadaPreviamente">Ação realizada - utilizado para dar andamento no fluxo sem esperar pela ação do usuário</param>
        /// <returns></returns>
        private static Boolean CriarNovaTarefa(InstanciaFluxo instanciaAtualDoFluxo, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa
            , out String codigoTarefaAnterior, out String ultimaAcaoRealizada, ref Tarefa ultimaTarefa)
        {
            codigoTarefaAnterior = String.Empty;
            ultimaAcaoRealizada = String.Empty;
            Boolean retorno = false;

            List<Tarefa> tarefasPorTipo = ConsultarTarefasPorTipo(instanciaAtualDoFluxo, configuracaoTarefa);
            Boolean acaoRealizadaPreviamente = TryGetUltimaAcaoRealizada(tarefasPorTipo, configuracaoTarefa, out ultimaAcaoRealizada, out ultimaTarefa);
            Boolean tarefaObrigatoria = configuracaoTarefa.Obrigatoria != null && (Boolean)configuracaoTarefa.Obrigatoria;
            Boolean erroCancelado = instanciaAtualDoFluxo.ErroCancelado != null && (Boolean)instanciaAtualDoFluxo.ErroCancelado;

            if (instanciaAtualDoFluxo.FluxoReprovado != null && (Boolean)instanciaAtualDoFluxo.FluxoReprovado
                && instanciaAtualDoFluxo.EtapaParalela != null && (Boolean)instanciaAtualDoFluxo.EtapaParalela)//Alguma tarefa em paralelo reprovou a etapa (precisa andar automaticamente)
                retorno = false;
            else if (TryGetTarefaAtiva(tarefasPorTipo, out codigoTarefaAnterior))//Caso tarefa está ativa, não recriar.
                retorno = false;
            else if (acaoRealizadaPreviamente)
            {
                if (erroCancelado)
                {
                    retorno = false;
                    TipoAprovacao tipoAprovacao = ObterTipoAprovacaoPor(configuracaoTarefa.AprovacaoPor);
                    if (tipoAprovacao == TipoAprovacao.Alcada && ultimaTarefa != null)//Trata o caso em que a aprovação por tipo alçada deu erro antes de ser criado a proxima tarefa
                    {
                        Int32 indexAtual = ultimaTarefa.IndexAprovacao == null ? 0 : (Int32)ultimaTarefa.IndexAprovacao;
                        Int32 indexFinal = ultimaTarefa.IndexHeadLineFinal == null ? 0 : (Int32)ultimaTarefa.IndexHeadLineFinal;
                        retorno = indexAtual != indexFinal;
                    }
                }
                else if (tarefaObrigatoria)
                    retorno = true;
                else
                    retorno = false;
            }
            else
                retorno = true;

            return retorno;
        }

        #endregion

        #endregion

        #region [Superior & aprovador]

        private static List<ListaSP_RaizenAprovadores> ObterRaizenAprovadores(
            ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa,
            DateTime dataCriacaoTarefa,
            ListItem item,
            Dictionary<String, List<TarefaHist>> tarefaHist,
            ref Int32 indexHeadLineFinal)
        {
            List<ListaSP_RaizenAprovadores> aprovadores = new List<ListaSP_RaizenAprovadores>();
            List<ListaSP_RaizenAprovadores> listaAprovadores = ObterRaizenAprovadores(configuracaoTarefa, item, ref indexHeadLineFinal);

            foreach (ListaSP_RaizenAprovadores listaAprovador in listaAprovadores)
            {
                if (listaAprovador == null || (listaAprovador.Responsavel == null && listaAprovador.Grupo == null)) //Não existe nenhum aprovador ou Grupo responsável pela tarefa
                {
                    throw new Exception(String.Format("Aprovador nulo ou em branco. Id Item Lista Aprovadores: {0}", listaAprovador == null ? String.Empty : listaAprovador.ID.ToString()));
                }

                List<TarefaHist> aprovadorHistorico = new List<TarefaHist>();
                //Se não for uma tarefa de grupo, ou seja, o responsável é uma pessoa específica, temos que verificar se existe uma delegação de férias configurada
                if (listaAprovador.Responsavel != null)
                {
                    ListaSP_RaizenAprovadores aprovador = ObterRaizenAprovador(listaAprovador, dataCriacaoTarefa, aprovadorHistorico);
                    aprovadores.Add(aprovador);
					if (!tarefaHist.ContainsKey(aprovador.Responsavel.Login))
						tarefaHist.Add(aprovador.Responsavel.Login, aprovadorHistorico);
                }
                else
                    aprovadores.Add(listaAprovador);

            }

            return aprovadores;
        }

        private static List<ListaSP_RaizenAprovadores> ObterRaizenAprovadores(ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa, ListItem item, ref Int32 indexHeadlineFinal, Int32 indexHeadline = 0)
        {
            List<ListaSP_RaizenAprovadores> listaAprovadores = new List<ListaSP_RaizenAprovadores>();
            if (configuracaoTarefa.AprovacaoPor.Equals(TipoAprovacao.Alcada.GetTitle(), StringComparison.InvariantCultureIgnoreCase))
            {
                int headlineSizeTarefa = 0;
                if (String.IsNullOrEmpty(configuracaoTarefa.CampoHeadlineSize))
                    throw new NotImplementedException(MensagemPortal.HeadlineSizeMalConfigurado.GetTitle());
                else
                    int.TryParse(ObterValorItem(item, configuracaoTarefa.CampoHeadlineSize), out headlineSizeTarefa);

                //As tarefa de headline size devem ter o mesmo nome da lista de Headline Size
                KeyValuePair<Int32, List<ListaSP_RaizenAprovadores>> infoHeadline = ObterRaizenAprovadorHeadlineSize(headlineSizeTarefa, configuracaoTarefa.NomeDaTarefa, indexHeadline, item);
                List<ListaSP_RaizenAprovadores> aprovadores = infoHeadline.Value;
                if (aprovadores != null && aprovadores.Count > 0)
                {
                    indexHeadlineFinal = infoHeadline.Key;
                    foreach (ListaSP_RaizenAprovadores aprovador in aprovadores)
                        listaAprovadores.Add(aprovador);
                }
                else
                    throw new NotImplementedException(MensagemPortal.HeadlineSizeMalConfigurado.GetTitle());

            }
            else if (configuracaoTarefa.AprovacaoPor.Equals(TipoAprovacao.Grupo.GetTitle(), StringComparison.InvariantCultureIgnoreCase))
            {
                //PRIMEIRO verificar se o grupo trabalha com Gerencias especificas. CASO contrário, atribuir a tarefa para o GRUPO
                Usuario gerente = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, PropriedadesItem.GerenteRegiao.GetTitle());

                listaAprovadores = new ListaSP_RaizenAprovadores().Consultar(
                        SPCamlLogicalOperator.And(
                            SPCamlComparisonOperator.Equal(PropriedadesItem.Grupo.GetTitle(), configuracaoTarefa.Grupo.Id, SPCamlFieldOptions.LookupFieldById),
                            SPCamlComparisonOperator.Equal(PropriedadesItem.Gerencias.GetTitle(), gerente.Id, SPCamlFieldOptions.LookupFieldById)
                        )
                    );

                if (listaAprovadores.Count < 1) //Não é uma tarefa com aprovadores por gerencia configurados
                {
                    listaAprovadores.Add(new ListaSP_RaizenAprovadores()
                    {
                        Responsavel = null,
                        Grupo = configuracaoTarefa.Grupo,
                        Gerencias = null
                    });
                }
            }
            else
            {
				String[] propriedadesUsuario = configuracaoTarefa.ResponsavelDoTipoItem.Split(';');
				foreach (var propriedade in propriedadesUsuario)
					listaAprovadores.Add(new ListaSP_RaizenAprovadores()
					{
						Responsavel = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, propriedade),
						Grupo = null,
						Gerencias = null
					});
            }

            return listaAprovadores;
        }

        private static ListaSP_RaizenAprovadores ObterRaizenAprovador(ListaSP_RaizenAprovadores aprovador, DateTime dataCriacaoTarefa, List<TarefaHist> tarefaHist)
        {
            Delegacao delegacao = new Delegacao().Consultar(item =>
                item.LoginDe.Equals(aprovador.Responsavel.Login, StringComparison.InvariantCultureIgnoreCase)
				&& item.Ativo == true
                && item.DataFim >= dataCriacaoTarefa
                && item.DataInicio <= dataCriacaoTarefa).FirstOrDefault();

            if (delegacao != null && !tarefaHist.Exists(h => h.LoginDe == delegacao.LoginPara))
            {
                tarefaHist.Add(new TarefaHist()
                {
                    LoginDe = delegacao.LoginDe,
                    LoginPara = delegacao.LoginPara,
                    ComentarioDelegacao = "Delegação devido à ausência temporária",
                    TipoTarefaHist = (Byte)TipoTarefaHist.DelegacaoAutomatica
                });

                ListaSP_RaizenAprovadores novoAprovador = new ListaSP_RaizenAprovadores();
                novoAprovador.Responsavel = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(delegacao.LoginPara);

                return ObterRaizenAprovador(novoAprovador, dataCriacaoTarefa, tarefaHist);
            }

            return aprovador;
        }

        private static Usuario ObterSuperior(ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa, ListItem item)
        {
            Usuario superior = null;
            if (configuracaoTarefa.AprovacaoPor == TipoAprovacao.Grupo.GetTitle())
            {
                List<ListaSP_CadastroDeGrupos> gruposAprovadores = new ListaSP_CadastroDeGrupos().Consultar(
                                    SPCamlComparisonOperator.Equal(PropriedadesItem.Grupo.GetTitle(), configuracaoTarefa.Grupo.Id, SPCamlFieldOptions.LookupFieldById)
                                );

                if (gruposAprovadores.Count < 1) //Não ter um grupo/superior cadastrado não deve ser um impeditivo para a criação da tarefa.
                {
                    return null;
                }
                else
                {
                    superior = gruposAprovadores[0].Superior; //No caso de duplicidade de cadastro, considerar o primeiro retornado.
                }
            }
            else if (configuracaoTarefa.AprovacaoPor == TipoAprovacao.Item.GetTitle())
                superior = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, configuracaoTarefa.SuperiorDoTipoItem);

            return superior;
        }

        private static List<ListaSP_RaizenAprovadores> ConsultarAprovadorHeadline(string nomeTarefa, ListItem item)
        {
            List<ListaSP_RaizenAprovadores> listaAprovadores = new List<ListaSP_RaizenAprovadores>();
            List<ListaSP_RaizenAprovacoesHeadlineSize> listaConfigHeadline = new ListaSP_RaizenAprovacoesHeadlineSize().Consultar(
                    SPCamlComparisonOperator.Equal(PropriedadesItem.Title.GetTitle(), nomeTarefa, SPCamlFieldOptions.None)
                );
            if (listaConfigHeadline.Count < 1)
                throw new NotImplementedException(MensagemPortal.HeadlineSizeMalConfigurado.GetTitle());
            else
            {
                ListaSP_RaizenAprovacoesHeadlineSize aprovadorHeadline = listaConfigHeadline.First();
                Usuario responsavel = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, aprovadorHeadline.ResponsavelItem);
                if (responsavel != null)
                    listaAprovadores.Add(new ListaSP_RaizenAprovadores()
                    {
                        Responsavel = responsavel,
                        Grupo = null,
                        Gerencias = null
                    });
                
                //PRIMEIRO verificar se o grupo trabalha com Gerencias especificas. CASO contrário, atribuir a tarefa para o GRUPO
                Usuario gerente = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, PropriedadesItem.GerenteRegiao.GetTitle());

                if (aprovadorHeadline.Grupo != null && gerente != null)
                {
                    List<ListaSP_RaizenAprovadores> listaAprovadoresEspecificos = new ListaSP_RaizenAprovadores().Consultar(
                            SPCamlLogicalOperator.And(
                                SPCamlComparisonOperator.Equal(PropriedadesItem.Grupo.GetTitle(), aprovadorHeadline.Grupo.Id, SPCamlFieldOptions.LookupFieldById),
                                SPCamlComparisonOperator.Equal(PropriedadesItem.Gerencias.GetTitle(), gerente.Id, SPCamlFieldOptions.LookupFieldById)
                            )
                        );

                    if (listaAprovadoresEspecificos.Count < 1) //Não é uma tarefa com aprovadores por gerencia configurados
                    {
                        listaAprovadores.Add(new ListaSP_RaizenAprovadores()
                        {
                            Responsavel = null,
                            Grupo = aprovadorHeadline.Grupo,
                            Gerencias = null
                        });
                    }
                    else
                        foreach (ListaSP_RaizenAprovadores aprovador in listaAprovadoresEspecificos)
                            listaAprovadores.Add(aprovador);
                }

                return listaAprovadores;
            }

        }

        private static KeyValuePair<Int32, List<ListaSP_RaizenAprovadores>> ObterRaizenAprovadorHeadlineSize(int headlineSize, string nomeTarefa, int indexHeadline, ListItem item)
        {

            Int32 indexHeadLineFinal = default(int);
            List<ListaSP_RaizenAprovadores> aprovadores = new List<ListaSP_RaizenAprovadores>();

            List<EntidadeHeadlineSP> aprovadoresHeadline = ComumSP.ConsultarHeadline(nomeTarefa,
                    SPCamlLogicalOperator.And(
                        SPCamlComparisonOperator.LessOrEqual(PropriedadesItem.HeadlineInicial.GetTitle(), headlineSize, SPCamlFieldOptions.None),
                        SPCamlComparisonOperator.GreaterOrEqual(PropriedadesItem.HeadlineFinal.GetTitle(), headlineSize, SPCamlFieldOptions.None)
                    )
                );

            if (aprovadoresHeadline.Count < 1)
                throw new NotImplementedException(MensagemPortal.HeadlineSizeMalConfigurado.GetTitle());
            else
            {
                EntidadeHeadlineSP aprovadorHeadline = aprovadoresHeadline.First();
                aprovadorHeadline.Aprovadores.CarregarDados();
                if (aprovadorHeadline.Aprovadores.Count > indexHeadline)
                {
                    indexHeadLineFinal = aprovadorHeadline.Aprovadores.Count - 1;
                    aprovadores = ConsultarAprovadorHeadline(aprovadorHeadline.Aprovadores[indexHeadline].Titulo, item);
                }
            }

            return new KeyValuePair<Int32, List<ListaSP_RaizenAprovadores>>(indexHeadLineFinal, aprovadores);
        }

        #endregion

        #region [SLA]

        private static Double ObterSlaUtilizado(InstanciaFluxo instanciaAtualDoFluxo, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa, ConfiguracaoExpediente configuracaoExpediente)
        {
            Double slaUtilizado = 0;

            List<Tarefa> tarefasRespondidas = new Tarefa().Consultar(t =>
                            t.IdInstanciaFluxo == instanciaAtualDoFluxo.IdInstanciaFluxo
                        && t.CodigoConfiguracao == configuracaoTarefa.ID
                        && t.TarefaCompleta && t.DataFinalizado != null
                && t.AprovacoesAtuais != null && (Boolean)t.AprovacoesAtuais).OrderByDescending(i => i.IdTarefa).ToList();

            foreach(Tarefa tarefaRespondida in tarefasRespondidas)
            {
                double slaUtilizadoParcial = 0;
                slaUtilizadoParcial = DataHelper.CalcularTempoUtil(configuracaoExpediente, tarefaRespondida.DataAtribuido, (DateTime)tarefaRespondida.DataFinalizado, true);
                slaUtilizado += slaUtilizadoParcial;
            }
            
            return slaUtilizado;
        }

        private static ConfiguracaoExpediente ObterConfiguracaoExpediente(ListaSP_RaizenConfiguracoesDeFluxo tarefa) //Tarefa como parâmetro
        {
            String horarioInicio;
            String horarioSaida;
            ObterHorarioExpediente(tarefa, out horarioInicio, out horarioSaida);

            ConfiguracaoExpediente configuracaoExpediente = new ConfiguracaoExpediente();
            configuracaoExpediente.HorarioExpedienteEntrada = DataHelper.ConverterTextoHoras(horarioInicio, ConfiguracaoExpediente.ConfiguracaoDefault.HorarioExpedienteEntrada);
            configuracaoExpediente.HorarioExpedienteSaida = DataHelper.ConverterTextoHoras(horarioSaida, ConfiguracaoExpediente.ConfiguracaoDefault.HorarioExpedienteSaida);

            configuracaoExpediente.DiasUteisSemana = ObterDiasUteisSemana(tarefa.DiasUteis.ToList());

            configuracaoExpediente.Feriados = new List<DateTime>();
            List<ListaSP_Feriados> feriados = new ListaSP_Feriados().Consultar();
            if (feriados != null && feriados.Count > 0)
            {
                configuracaoExpediente.Feriados = (from feriado in feriados
                                                   select feriado.Data).ToList();
            }

            return configuracaoExpediente;
        }

        private static void ObterHorarioExpediente(ListaSP_RaizenConfiguracoesDeFluxo tarefa, out String horarioInicio, out String horarioSaida)
        {
            String[] horarioExpediente = tarefa.Expediente.Split('-');

            horarioInicio = horarioExpediente.Length > 0 ? horarioExpediente[0] : String.Empty;
            horarioSaida = horarioExpediente.Length > 1 ? horarioExpediente[1] : String.Empty;
        }

        private static List<DayOfWeek> ObterDiasUteisSemana(List<String> semana)
        {
            List<DayOfWeek> diasSemana = new List<DayOfWeek>();

            foreach (DayOfWeek dia in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (semana.Contains(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(dia))
                    || semana.Contains(CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedDayName(dia))
                    || semana.Contains(CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedDayName(dia))
                    || semana.Contains(String.Format("{0},", dia.ToString()))
                )
                    diasSemana.Add(dia);
            }

            if (diasSemana == null || diasSemana.Count <= 0)
                diasSemana = ConfiguracaoExpediente.ConfiguracaoDefault.DiasUteisSemana;

            return diasSemana;

        }

        #endregion

        #region [Delegação]

        public static String ObterTextoDelegacao(TarefaHist tarefaHistorico)
        {
            Usuario responsavelOriginal = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(tarefaHistorico.LoginDe);
            String template = "<br /><br />{0} delegou esta tarefa para você. Por favor, veja os comentários abaixo:<br />{1}<br />";
            String textoDelegacao = String.Format(template, responsavelOriginal != null ?
                responsavelOriginal.Nome : tarefaHistorico.LoginDe, tarefaHistorico.ComentarioDelegacao.Replace("\n", "<br/>"));

            return textoDelegacao;
        }


		public static CustomPainel ConsultarDelegacaoProgramada(DateTime? dataInicio, DateTime? dataFim, String usuarioDeLogin = ""
			, String usuarioParaLogin = "", Boolean? ativo = null, String ordenarPor = "", Boolean descedente = false, Int32 itensPagina = 100, Int32 paginaAtual = 1)
		{
			#region [Pesquisa]
			List<Delegacao> delegacoes = (String.IsNullOrEmpty(ordenarPor) || ordenarPor == "LoginDe") ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.LoginDe
							, paginaAtual, itensPagina, descedente) : ordenarPor == "LoginPara" ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.LoginPara
							, paginaAtual, itensPagina, descedente) : ordenarPor == "NomeDe" ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.NomeDe
							, paginaAtual, itensPagina, descedente) : ordenarPor == "NomePara" ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.NomePara
							, paginaAtual, itensPagina, descedente) : ordenarPor == "DataInicio" ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.DataInicio
							, paginaAtual, itensPagina, descedente) : ordenarPor == "DataFim" ?
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.DataFim
							, paginaAtual, itensPagina, descedente) :
						new Delegacao().Consultar(_ => (dataInicio == null || _.DataInicio >= dataInicio)
							&& (dataFim == null || _.DataFim <= dataFim)
							&& (String.IsNullOrEmpty(usuarioDeLogin) || _.LoginDe == usuarioDeLogin)
							&& (String.IsNullOrEmpty(usuarioParaLogin) || _.LoginPara == usuarioParaLogin)
							&& (ativo == null || _.Ativo == ativo)
							, _ => _.Ativo
							, paginaAtual, itensPagina, descedente); 
			#endregion

			Int32 totalRecordCount = delegacoes != null ? delegacoes.Count : 0;
			if (totalRecordCount > 0)
			{
				Delegacao delegacao = delegacoes.FirstOrDefault();
				if (delegacao != null)
					totalRecordCount = delegacao.TotalRecordCount ?? totalRecordCount;
			}

			Collection<Object> colecao = new Collection<Object>();
			colecao.AddRange(delegacoes);
			CustomPainel report = null;
			report = new CustomPainel()
			{
				Entries = colecao,
				TotalRecordCount = totalRecordCount
			};

			return report;
		}

        #endregion

        #region [Discussão]

        public static void FormatarRespostaDiscussao(Tarefa tarefaResposta)
        {
            Tarefa _discussaoPergunta = new Tarefa().Obter((Int32)tarefaResposta.IdTarefaPai);
            tarefaResposta.ComentarioAprovacao = String.Format("{0} em {1} abriu a discussão:\n\n{2}\n\n{3} em {4} respondeu a discussão:\n\n{5}"
                , _discussaoPergunta.NomeCompletadoPor, ((DateTime)_discussaoPergunta.DataFinalizado).ToString("dd/MM/yyyy"), _discussaoPergunta.ComentarioAprovacao
                , tarefaResposta.NomeCompletadoPor, ((DateTime)tarefaResposta.DataFinalizado).ToString("dd/MM/yyyy"), tarefaResposta.ComentarioAprovacao);
        }

        public static void PopularEmailDiscussaoPergunta(Tarefa tarefaResposta, Tarefa tarefaPergunta)
        {
            String tipoTemplate = TipoTemplate.DiscutirPergunta.GetTitle();
            List<ListaSP_RaizenTemplateDeEmails> templatesDiscussaoPergunta = new ListaSP_RaizenTemplateDeEmails().Consultar(t => t.TipoTemplate == tipoTemplate);
            templatesDiscussaoPergunta.CarregarDados();

            InstanciaFluxo instancia = new InstanciaFluxo().Obter(tarefaResposta.IdInstanciaFluxo);
            Lista listaInstancia = new Lista().Obter(_ => _.CodigoLista == instancia.CodigoLista);
            ListItem item = ComumSP.ObterItem(instancia.CodigoLista, instancia.CodigoItem);

            ListaSP_RaizenTemplateDeEmails template = templatesDiscussaoPergunta.Find(t => t.Fluxo != null && t.Fluxo.Titulo == listaInstancia.Nome);
            template = template != null ? template : templatesDiscussaoPergunta.Find(t => t.Fluxo == null) != null ? templatesDiscussaoPergunta.Find(t => t.Fluxo == null) : templatesDiscussaoPergunta.FirstOrDefault();

            Dictionary<TipoTag, Object> objetos = new Dictionary<TipoTag, Object>();
            NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Fluxo, instancia);

            NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Item, item);

            String listaDescricaoUrlTarefa = String.Format("{0}", new Uri(new Uri(PortalWeb.ContextoWebAtual.Url), listaInstancia.DescricaoUrlItem));
            tarefaResposta.DescricaoUrlItem = String.Format("{0}{1}", listaDescricaoUrlTarefa, item.Id.ToString());
            tarefaResposta.DescricaoUrlTarefa = String.Format("{0}{1}&IdTarefa={2}", listaDescricaoUrlTarefa, item.Id.ToString(), tarefaResposta.IdTarefa);
            tarefaResposta.ComentarioAprovacao = tarefaPergunta.ComentarioAprovacao;
            NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Tarefa, tarefaResposta);

            String emailCorpo = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(template.Corpo));
            String emailAssunto = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(template.Assunto));

            tarefaResposta.ComentarioAprovacao = null;
            tarefaResposta.DescricaoMensagemEmail = emailCorpo;
            tarefaResposta.DescricaoAssuntoEmail = emailAssunto;

        }

        public static void AtualizarDiscussoesPendentes(Tarefa tarefaOriginal)
        {
            List<Tarefa> discussoesPergunta = new Tarefa().Consultar(_ => _.IdTarefaPai == tarefaOriginal.IdTarefa);
            discussoesPergunta.ForEach(pergunta =>
            {
                List<Tarefa> discussoesResposta = new Tarefa().Consultar(_ => _.IdTarefaPai == pergunta.IdTarefa && _.TarefaCompleta == false);
                discussoesResposta.ForEach(resposta => { resposta.Ativo = false; });
                discussoesResposta.Atualizar();
            });

        }

        #endregion

        #region [Item]

        /// <summary>Efetua uma cópia das propriedades</summary>
        /// <param name="origem">Origem</param>
        /// <param name="destino">Destino</param>
        //public static void EfetuarCopia(this Tarefa origem, Tarefa destino)
        //{
        //    destino.DescricaoAcaoEfetuada = origem.DescricaoAcaoEfetuada;
        //    destino.ComentarioAprovacao = origem.ComentarioAprovacao;
        //    destino.TarefaCompleta = origem.TarefaCompleta;
        //    destino.NomeCompletadoPor = origem.NomeCompletadoPor;
        //    destino.LoginCompletadoPor = origem.LoginCompletadoPor;
        //    destino.DataFinalizado = origem.DataFinalizado;
        //}

        public static void ObterInformacoesItemAtual(String siteURL, Guid currentListId, Int32 currentItemID, out ListItem item)
        {
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            List currentList = PortalWeb.ContextoWebAtual.ObterLista(currentListId);

            item = currentList.GetItemById(currentItemID);

            PortalWeb.ContextoWebAtual.SPClient.Load(item);
            PortalWeb.ContextoWebAtual.SPClient.Load(item.ParentList);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        }

        /// <summary>
        /// Tipo FieldUserValue - retorna o id do usuário
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propriedade"></param>
        /// <returns></returns>
        public static String ObterValorItem(ListItem item, String propriedade)
        {
            String valor = String.Empty;
            if (String.IsNullOrEmpty(propriedade))
                return valor;
            if (item[propriedade] == null)
                return valor;

            if (item[propriedade] is FieldUserValue)
            {
                valor = (item[propriedade] as FieldUserValue).LookupId.ToString();
            }
            else if (item[propriedade] is FieldLookupValue)
            {
                valor = (item[propriedade] as FieldLookupValue).LookupValue;
            }
            else
            {
                valor = item[propriedade].ToString();
            }

            return valor;
        }

        #endregion

        #region [Lista]

        /// <summary>Busca os dados da lista referente a tarefa informada</summary>
        /// <param name="item">Item</param>
        /// <returns>Objeto com as informações da lista</returns>
        public static Lista ObterLista(this Tarefa item)
        {
            //Busca os e-mails
            return DadosTarefa.ObterLista(item);
        }
        #endregion

    }
}