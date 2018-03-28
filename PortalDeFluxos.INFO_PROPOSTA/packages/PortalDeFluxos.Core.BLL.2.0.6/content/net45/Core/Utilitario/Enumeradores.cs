using Iteris;
using System.ComponentModel;

namespace PortalDeFluxos.Core.BLL.Utilitario
{
    public enum Ambiente
    {
        [Title("VM")]
        VM = 1,

        [Title("DEV")]
        DEV = 2,

        [Title("QAS")]
        QAS = 3,

        [Title("PRD")]
        PRD = 4
    }

    public enum Operacao
    {
        [Title("raizenForm.calculateOperation.addition")]
        Adicao = 1,

        [Title("raizenForm.calculateOperation.subtraction")]
        Subtracao = 2,

        [Title("raizenForm.calculateOperation.division")]
        Divisao = 3,

        [Title("raizenForm.calculateOperation.multiplication")]
        Multiplicacao = 4
    }

	public enum SaePastas
	{
		[Title("PASTA_PLANILHAS_PROPOSTAS_SAE_RNIPS")]
		RNIPs,
		[Title("PASTA_PLANILHAS_PROPOSTAS_SAE_RNIPS_ANTIGA")]
		RNIPsAntigo,
		[Title("PASTA_PLANILHAS_PROPOSTAS_SAE_B2BIP")]
		B2B,
		[Title("PASTA_PLANILHAS_PROPOSTAS_SAE_B2BIP_ANTIGA")]
		B2BAntigo,
	}

	public enum SaePlanilhas
	{
		[Title("PLANILHA_ANTIGA_B2BIP")]
		B2BAntigo,
		[Title("PLANILHA_ATUAL_B2BIP")]
		B2B,
		[Title("PLANILHA_ANTIGA_RNIPS")]
		RNIPsAntigo,
		[Title("PLANILHA_RNIPS")]
		RNIPs,
	}

    public enum ParametroEnum
    {
        [Title("Listas sincronizadas no banco")]
        SincronizarLista = 22,
        [Title("Configurações Restart Fluxos")]
        RestartFluxos = 34,
        [Title("Configurações E-mail Attachment")]
        EmailAttachment = 40,
        [Title("Configurações E-mail - Mensagem")]
        EmailMensagem = 41,
        [Title("Estrutura comercial das listas.")]
        EstruturaComercial = 42,
    }

    public enum TipoTarefa
    {
        [Title("Primeiro")]
        Primeiro = 1,

        [Title("Todos")]
        Votacao = 2,

        [Title("Votação")]
        Todos = 3
    }

    public enum TipoUserControl
    {
        [Title("Proposta")]
        Proposta = 1,

        [Title("Formulário de Aprovação")]
        FormularioAprovacao = 2
    }

    public enum TipoTemplate
    {
        [Title("Normal")]
        Normal = 1,

        [Title("Cancelar")]
        Cancelar = 2,

        [Title("Discutir Pergunta")]
        DiscutirPergunta = 3,

        [Title("Discutir Resposta")]
        DiscutirResposta = 4,

        [Title("Resultado Econômico")]
        ResultadoEconomico = 5,

        [Title("Reprovar")]
        Reprovar = 6,

		[Title("PEP")]
		PEP = 7,
    }

    public enum TipoAprovacaoPor
    {
        [Title("Item")]
        Item = 1,

        [Title("Papel")]
        Papel = 2,

        [Title("Alçada")]
        Alcada = 3
    }

    public enum TipoAprovacao
    {
        [Title("Item")]
        Item = 1,

        [Title("Grupo")]
        Grupo = 2,

        [Title("Alçada")]
        Alcada = 3,

        [Title("Discussão")]
        Discussao = 4
    }

    public enum TipoAnexo
    {
        [Title("Item")]
        Item = 1,

        [Title("Biblioteca")]
        Biblioteca = 2,
    }

    public enum TipoEmail
    {
        [Title("Enviado")]
        Enviado = 1,

        [Title("Recebido")]
        Recebido = 2,
    }

    public enum TipoWebServices
    {
        [Title("Inválido")]
        Invalido = 0,

        [Title("SalesForce - EstruturaComercial")]
        SalesForce_EstruturaComercial = 1,

        [Title("SalesForce - Radar")]
        SalesForce_Radar = 2,

        [Title("SalesForce - PropostaInvestimento")]
        SalesForce_PropostaInvestimento = 3,

        [Title("SAE")]
        SAE = 4,

        [Title("FichaCadastral")]
        FichaCadastral = 5,

        [Title("SAEB2B")]
        SAEB2B = 6,

        [Title("SaeComum")]
        SaeComum = 7,

		[Title("RBC")]
		RBC = 8,

        [Title("Anexo Fácil - API")]
        AnexoFacilAPI = 9,

        [Title("Anexo Fácil - Url Proposta")]
        AnexoFacilUrlProposa = 10,

        [Title("Anexo Fácil - Chave")]
        AnexoFacilChave = 11,

		[Title("A3F - Url Proposta")]
		A3FUrlProposta = 12,

		[Title("A3F - WebService")]
		A3FWebService = 13,
    }

    public enum StatusFluxo
    {
        [Title("Default")]
        Default = 0,

        [Title("EmAndamento")]
        EmAndamento = 1,

        [Title("Erro")]
        Erro = 2,

        [Title("Cancelado")]
        Cancelado = 3,

        [Title("Finalizado")]
        Finalizado = 4,
    }

    public enum StatusProposta
    {
        [Title("Andamento({0})")]
        Andamento = 1,

        [Title("Erro")]
        Erro = 2,

        [Title("Cancelada")]
        Cancelada = 3,

        [Title("Finalizada")]
        Finalizada = 4,

        [Title("Reprovada")]
        Reprovada = 5,
    }

	public enum StatusA3F
	{

		[Title("Em Rascunho")]
		EmRascunho = 1,

		[Title("Em Andamento")]
		EmAndamento = 2,

		[Title("Aprovado")]
		Aprovado = 3,

		[Title("Reprovado")]
		Reprovado = 4,

		[Title("Finalizado")]
		Finalizado = 4,
	}
    public enum TipoTarefaHist
    {
        DelegacaoIndividual = 1,
        DelegacaoAutomatica = 2,
        Escalonamento = 3
    }

    public enum Origem
    {
        [Title("EmailAttachment")]
        EmailAttachment =1,
        [Title("RaizenForm")]
        RaizenForm = 2,
        [Title("Serviço")]
        Servico = 3
    }

    public enum MensagemErro
    {
        [Title("O ID {0} não foi encontrado na lista de configurações.")]
        IDConfiguracaoTarefaNaoEncontrato = 1,

        [Title("Nenhum aprovador encontrado.")]
        AprovadorNaoEncontrado = 2,

        [Title("O ID {0} da Tarefa não foi encontrado.")]
        TarefaNaoEncontrada = 3,

        [Title("Instância do fluxo {0} não encontrada.")]
        InstanciaFluxoNaoEncontrada = 4,

        [Title("Tipo de aprovação por {0} não implementada.")]
        TipoAprovacaoNaoImplementada = 5,

        [Title("Formulário customizado não está configurado para esta lista.")]
        LoadFormularioCustomizado = 6
    }

    public enum MensagemPortal
    {
        [Title("O ID {0} não foi encontrado na lista de configurações.")]
        IDConfiguracaoTarefaNaoEncontrato = 1,

        [Title("Nenhum aprovador encontrado.")]
        AprovadorNaoEncontrado = 2,

        [Title("O ID {0} da Tarefa não foi encontrado.")]
        TarefaNaoEncontrada = 3,

        [Title("Instância do fluxo {0} não encontrada.")]
        InstanciaFluxoNaoEncontrada = 4,

        [Title("Tipo de aprovação por {0} não implementada.")]
        TipoAprovacaoNaoImplementada = 5,

        [Title("A solicitação foi recebida! O e-mail será enviado em breve.")]
        EmailAnexoSucesso = 6,

        [Title("Este usuário não possui e-mail.")]
        UsuarioSemEmail = 7,

        EmailAnexoAssunto = 8,//Tabela de parametros- mensagem Email
        EmailAnexoCorpo = 9,//Tabela de parametros - mensagem Email
        [Title("Sem Permissão login:{0} proposta:{1}")]//Apenas para log
        ErroPermissao = 10, //Tabela de parametros - mensagem Email
        [Title("Sem anexo proposta:{0}")]//Apenas para log
        EmailSemAnexo = 11,//Tabela de parametros - mensagem Email
        EmailAnexoErro = 12,//Tabela de parametros- mensagem Email
        EmailAnexoProcessado = 13,//Tabela de parametros- mensagem Email
        [Title("E-mail está fora do padrão assunto:{0}")]//Apenas para log
        EmailAnexoFormato = 14,//Tabela de parametros- mensagem Email
        EmailForaPadrao = 15,//Tabela de parametros- mensagem Email

        [Title("Sucesso.")]
        EmailSucessoUpload = 16,

        [Title("Falha, arquivo ultrapassa o limite de {0} mb.")]
        EmailAnexoTamanho = 17,

        [Title("Falha no Upload.")]
        EmailErroUpload = 18,

        [Title("Falha, tipo de arquivo não permitido.")]
        EmailErroExtensao = 19,

        [Title("Falha, arquivo em branco.")]
        EmailAnexoBranco = 20,

        [Title("A resposta não foi encontrada. Por favor, valide e envie novamente.")]
        EmailOutcome = 21,

        [Title("<tr><td>{0}</td><td>{1}</td></tr>")]
        EmailTr2TD = 22,

        [Title("Dados salvos com sucesso!")]
        Sucesso = 23,

        [Title("Um erro ocorreu! Por favor, contacte o administrador.")]
        Erro = 24,

        [Title("Fluxo reiniciado.")]
        FluxoReiniciado = 25,

        [Title("Fluxo foi reiniciado automaticamente. Dados importantes da proposta foram alterados.")]
        FluxoReiniciadoMensagem = 26,

        [Title("O grupo {0} não está cadastrado na lista Cadastro de Grupos.")]
        GrupoNaoCadastrado = 27,

        [Title("Não existe nenhum template de e-mail cadastrado para a tarefa {0}.")]
        TemplateEmailNaoCadastrado = 28,

        [Title("A lista de headline size {0} não foi encontrada.")]
        ListaHeadlineSizeNaoEncontrada = 29,

        [Title("Headline Size configurado incorretamente.")]
        HeadlineSizeMalConfigurado = 30,

        [Title("Fluxo cancelado.")]
        FluxoCancelado = 31,
    }

    public enum PropriedadesItem
    {
        [Title("Title")]
        Title = 1,

        [Title("Gerencia")]
        Gerencia = 2,

        [Title("Papeis")]
        Papeis = 3,

        [Title("Gerencias")]
        Gerencias = 4,

        [Title("Author")]
        Criador = 5,

        [Title("EstadoAtualFluxo")]
        EstadoAtualFluxo = 6,

        [Title("Grupo")]
        Grupo = 7,

        [Title("Title")]
        NomeTemplate = 8,

        [Title("GerenteRegiao")]
        GerenteRegiao = 9,

        [Title("HeadlineInicial")]
        HeadlineInicial = 10,

        [Title("HeadlineFinal")]
        HeadlineFinal = 11
    }

    public enum TipoTag
    {
        [Title("{Item:")]
        Item = 1,

        [Title("{Lista:")]
        Lista = 2,

        [Title("{Tarefa:")]
        Tarefa = 3,

        [Title("{Fluxo:")]
        Fluxo = 4,
        
        [Title("{ResultadoEconomico:")]
        ResultadoEconomico = 5,

		[Title("{PEP:")]
		PEP = 6
    }

    public enum PropriedadesFluxo
    {
        [Title("Microsoft.SharePoint.ActivationProperties.ContextListId")]
        ListId = 1,
        [Title("Microsoft.SharePoint.ActivationProperties.CurrentItemUrl")]
        ItemUrl = 2,
        [Title("{Microsoft.SharePoint.ActivationProperties.InitiatorUserId")]
        InitiatorUserId = 3,
        [Title("Microsoft.SharePoint.ActivationProperties.ItemGuid")]
        ItemGuid = 4,
        [Title("Microsoft.SharePoint.ActivationProperties.ItemId")]
        ItemId = 5,
        [Title("Microsoft.SharePoint.ActivationProperties.ParentContentTypeId")]
        ParentContentTypeId = 6,
        [Title("Microsoft.SharePoint.ActivationProperties.RetryCod")]
        RetryCode = 7,
        [Title("{Microsoft.SharePoint.ActivationProperties.UniqueId")]
        UniqueId = 8,
        [Title("UserStatus")]
        WorkflowStatus = 9
    }

    public enum PortalRoles
    {
        [Title("Administradores CS")]
        [Description("Role utilizada para controlar permissões dos administradores CS.")]
        AdministradoresCS = 1,

        [Title("Cancelar Fluxo")]
        [Description("Role utilizada para controlar quem possui permissão para cancelar fluxo no menu de contexto.")]
        CancelarFluxo = 2,

        [Title("Raizen Colaborador")]
        [Description("Pode exibir, adicionar e atualizar itens de lista e documentos.")]
        RaizenColaborador = 3,

        [Title("Obter SAE")]
        [Description("Role utilizada para controlar quem possui permissão para obter SAE.")]
        ObterSAE = 4,

        [Title("Excluir Anexo")]
        [Description("Role utilizada para controlar quem possui permissão para excluir anexo.")]
        ExcluirAnexo = 5,

        [Title("Delegar Tarefa")]
        [Description("Role utilizada para controlar quem possui permissão para delegar qualquer tarefa - o dono da tarefa também pode delegar.")]
        DelegarTarefa = 6,

        [Title("Raizen Estrutura Comercial")]
        [Description("Pode exibir, adicionar e atualizar itens de lista e documentos, mas é restrito às propostas que ele está envolvido (como GT, GR, Diretor, CDR ou GDR).")]
        RaizenEstruturaComercial = 7,

        [Title("Incluir Anexo")]
        [Description("Role utilizada para controlar quem possui permissão para incluir anexo.")]
        IncluirAnexo = 8,

        [Title("Editar Gridview Documentos")]
        [Description("Role utilizada para controlar quem possui permissão para editar os itens do gridview de documentos.")]
        GridviewDocumentos = 9,

		[Title("Incluir Comentário")]
		[Description("Role utilizada para controlar quem possui permissão para editar os comentários da proposta (mesmo quando o fluxo está em andamento).")]
		IncluirComentario = 10,

		[Title("Enviar Aprovação")]
		[Description("Role utilizada para controlar quem possui permissão para enviar a proposta para aprovação.")]
		EnviarAprovacao = 11,
    }

    public enum PortalGrupos
    {
        [Title("Administradores CS")]
        [Description("Grupo dos administradores CS.")]
        AdministradoresCS = 1,
        [Title("Grupo Administradores Portal - TI")]
        [Description("Grupo Administradores Portal - TI.")]
        AdministradoresTI = 2,
        [Title("Grupo Ativo")]
        [Description("Grupo Ativo.")]
        Ativo = 3,
        [Title("Grupo Contrato Emissao")]
        [Description("Grupo Contrato Emissao.")]
        ContratoEmissao = 4,
        [Title("Grupo Contrato Recebimento")]
        [Description("Grupo Contrato Recebimento.")]
        ContratoRecebimento = 5,
        [Title("Grupo Coordenador de Engenharia")]
        [Description("Grupo Coordenador de Engenharia.")]
        CoordenadorEngenharia = 6,
        [Title("Grupo Diretor de Engenharia")]
        [Description("Grupo Diretor de Engenharia.")]
        DiretorEngenharia = 7,
        [Title("Grupo Estrutura Comercial")]
        [Description("Grupo Estrutura Comercial.")]
        EstruturaComercial = 8,
        [Title("Grupo Gerente de Engenharia")]
        [Description("Grupo Gerente de Engenharia.")]
        GerenteEngenharia = 9,
        [Title("Grupo Gerente Financas")]
        [Description("Grupo Gerente Financas.")]
        GerenteFinancas = 10,
        [Title("Grupo Gerente Planejamento de Rede")]
        [Description("Grupo Gerente Planejamento de Rede.")]
        GerentePlanejamentoRede = 11,
        [Title("Grupo Juridico")]
        [Description("Grupo Juridico.")]
        Juridico = 12,
        [Title("Grupo Planner")]
        [Description("Grupo Planner.")]
        Planner = 13,
        [Title("Grupo Sup Vendas")]
        [Description("Grupo Sup Vendas.")]
        SupVendas = 14,
        [Title("Grupo Planejamento Backoffice")]
        [Description("Grupo Planejamento Backoffice.")]
        PlanejamentoBackOff = 15
    }

    public enum ListasHeadlineSize
    {
        [Title("Headline Size - Vendas")]
        HeadlineVendas,
        [Title("Headline Size - Planejamento")]
        HeadlinePlanejamento,
        [Title("Headline Size - Financeiro")]
        HeadlineFinanceiro
    }

    #region [Painel]

    public enum SolicitacaoExecucaoReportGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Número da Solicitação")]
        NomeSolicitacao = 1,

        [Title("Gerente Região")]
        NomeGerenteRegiao = 2,

        [Title("Diretor")]
        NomeDiretorVendas = 3,

        [Title("Solicitante")]
        NomeSolicitante = 5,

        [Title("Etapa Atual")]
        NomeEtapa = 6,

        [Title("Status Fluxo")]
        StatusFluxo = 7,

        [Title("Detalhes")]
        DescricaoUrlDetalhe = 8
    }

    public enum MinhasTarefasPendenteReportGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Número da Solicitação")]
        NomeSolicitacao = 1,

        [Title("Gerente Região")]
        NomeGerenteRegiao = 2,

        [Title("Diretor")]
        NomeDiretorVendas = 3,

        [Title("Solicitante")]
        NomeSolicitante = 5,

        [Title("Tempo Decorrido")]
        TempoDecorrido = 6,

        [Title("Aprovar / Revisar")]
        NomeTarefa = 7,

        [Title("Detalhes")]
        DescricaoUrlDetalhe = 8
    }

    public enum TarefasPendentesReportGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Ação")]
        NomeTarefa = 1,

        [Title("Participante")]
        NomeResponsavel = 2,

        [Title("Data de Início")]
        DataInclusao = 3,

        [Title("SLA")]
        DataSLA = 4
    }

    public enum TarefasRealizadasReportGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Ação")]
        NomeTarefa = 1,

        [Title("Executor")]
        NomeCompletadoPor = 2,

        [Title("Tipo")]
        DescricaoAcaoEfetuada = 3,

        [Title("Comentários")]
        ComentarioAprovacao = 4,

        [Title("Concluído em")]
        DataFinalizado = 5
    }

    public enum LogReportGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Id")]
        IdLog = 1,

        [Title("Processo")]
        NomeProcesso = 2,

        [Title("Origem")]
        DescricaoOrigem = 3,

        [Title("Mensagem")]
        DescricaoMensagem = 4,

        [Title("Login Inclusão")]
        LoginInclusao = 5,

        [Title("Data Inclusão")]
        DataInclusao = 6,

        [Title("Erro")]
        Erro = 7,

        [Title("")]
        DescricaoDetalhe = 8,
        
    }

    public enum DocumentosGridViewFieldDefinitionType
    {
        [Title("")]
        None = 0,

        [Title("Usuário")]
        Usuario = 1,

        [Title("Arquivo")]
        NomeArquivo = 2,

        [Title("Data")]
		DataUploadString = 3,

        [Title("")]
        Download = 4,

        [Title("")]
        Excluir = 5,

    }

    public enum ServicosGridViewFieldDefinitionType
    {
        [Title("Id")]
        IdServicoAgendado = 1,

        [Title("Nome Serviço")]
        NomeAssemblyType = 2,

        [Title("Agenda")]
        DescricaoAgenda = 3,

        [Title("Data Ultima Execução")]
        DataUltimaExecucao = 4,

        [Title("Data Proxima Execução")]
        DataProximaExecucao = 5,

        [Title("Login Alteração")]
        LoginAlteracao = 6,

        [Title("Logar")]
        Logar = 7,

        [Title("Ativo")]
        Ativo = 8,

        [Title("Executar")]
        Executar = 9,

    }

    public enum FluxosGridViewFieldDefinitionType
    {
        [Title("Id Fluxo")]
        IdInstanciaFluxo = 1,

        [Title("ID Proposta")]
        CodigoItem = 2,

        [Title("Lista")]
        NomeLista = 3,

        [Title("Nome Solicitação")]
        NomeSolicitacao = 4,

        [Title("Etapa")]
        NomeEtapa = 5,

        [Title("Data Alteração")]
        DataAlteracao = 6,

        [Title("Data Restart Workflow")]
        DataRestartWorkflow = 7,

        [Title("Status")]
        NomeStatusFluxo = 8,

        [Title("Tentativas Início")]
        NumeroTentativaInicio = 9,

        [Title("Ativo")]
        Ativo = 10,

        [Title("Iniciar")]
        Iniciar = 11,

    }

    public enum UsuarioGruposGridViewFieldDefinitionType
    {
        [Title("Id")]
        Id = 1,

        [Title("Nome")]
        Nome = 2,

        [Title("Login")]
        Login = 3,

        [Title("Tipo")]
        Tipo = 4,

        [Title("Ativo")]
        AtivoDescricao = 5,

        [Title("Nr Usuários Ativos/Inativos")]
        QtdUsuarios = 6,

        [Title("Nr Tarefas")]
        QtdTarefa = 7

    }

	public enum DelegacaoProgramadaGridViewFieldDefinitionType
	{
		[Title("Login De")]
		LoginDe = 1,

		[Title("Nome De")]
		NomeDe = 2,

		[Title("Login Para")]
		LoginPara = 3,

		[Title("Nome Para")]
		NomePara = 4,

		[Title("Data Início")]
		DataInicio = 5,

		[Title("Data Fim")]
		DataFim = 6,

		[Title("Ativo")]
		Ativo = 7

	}

    #endregion [Painel]

    public enum TipoPropostaPai
    {
		[Title("RNIP")]
		Rnip = 1,

		[Title("RNIP - Combo")]
		Combo = 10,

		[Title("B2B")]
		B2B = 12,

		[Title("Comodato")]
		Comodato = 17,

		[Title("Contratos Unificados")]
		ContratosUnificados = 18,

		[Title("Sucessao")]
		Sucessao = 19,

		[Title("Baixa De Garantias")]
		BaixaDeGarantias = 20,

		[Title("Bandeira Branca")]
		BandeiraBranca = 21,
	}

	public enum PrefixoSAE
	{
		[Title("CasoInv")]
		CasoInv,
		[Title("CasoBase")]
		CasoBase,
		[Title("ValorAno")]
		ValorAno
	}

	public enum SentidoSAE
	{
		[Title("Input")]
		Input = 0,
		[Title("Output")]
		Output = 1,
	}

	public enum TipoCampoSAE
	{
		[Title("Porcentagem")]
		Porcentagem = 0,
		[Title("Decimal")]
		Decimal = 1,
		[Title("String")]
		String = 2,
		[Title("Int")]
		Int = 3,
	}
}
