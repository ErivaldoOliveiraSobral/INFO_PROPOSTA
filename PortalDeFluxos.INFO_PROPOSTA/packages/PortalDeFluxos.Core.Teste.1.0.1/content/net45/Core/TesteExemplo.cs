using Iteris.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using Iteris;
using System.Text;
using System.Linq;
using Microsoft.SharePoint.Client;
using System.Web;
using System.Globalization;

namespace PortalDeFluxos.Core.Test
{

    [TestClass]
    public class TesteExemplo
    {
        //int itemIncluido = 0;

        //public TesteExemplo()
        //{
        //}

        //[TestMethod]
        //public void Teste1Exemplo()
        //{
        //    String objectIdEnabled = "{{Id:\"{0}\",Enabled:{1}}}";

        //    String teste = String.Format(objectIdEnabled, "teste1", true.ToString());

        //    StringBuilder arrayObjects = new StringBuilder();
        //    arrayObjects.Append("[");
        //    arrayObjects.Append(String.Format(objectIdEnabled, "teste1", true.ToString()));
        //    arrayObjects.Append(",");
        //    arrayObjects.Append(String.Format(objectIdEnabled, "teste2", false.ToString()));
        //    arrayObjects.Append("]");
        //    String teste1 = arrayObjects.ToString();
        //}

        //#region [ Testes de Banco ]

        //[TestMethod]
        //public void TesteProcBD()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi/sites/WMO"))
        //    {
        //        Assert.AreEqual("Aditivos Gerais - CT-ADITIVOS GERAIS-000002 - COML DERIV DE PETR SAO CARLOS LTDA", NegocioPainel.ConsultarTituloDetalheSolicitacao(new Guid("16AB57FF-6815-4C3E-B521-0A1A383DBB6D"), 2));
        //        Assert.AreEqual("Item - [IP B2B] CANABRAVA - 1035693 - 1116", NegocioPainel.ConsultarTituloDetalheSolicitacao(new Guid("17AA134E-8BF9-4A33-B25C-B92935F6E067"), 1116));
        //        pWeb.ObterLista("Aditivos Gerais").GetItemById(1);
        //    }
        //}

        //[TestMethod]
        //public void TesteAtualizarTarefa()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi/sites/WMO"))
        //    {
        //        Tarefa t = new Tarefa().Obter(12);
        //        t.NomeResponsavel = "Candido Teste 123";
        //        t.Atualizar();
        //    }
        //}

        //[TestMethod]
        //public void TesteIncluirItemDB()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        //Pode iniciar o Web via Reflection
        //        Delegacao d1 = pWeb.Obter<Delegacao>();
        //        d1.LoginDe = "11111";
        //        d1.LoginPara = "111111";
        //        d1.DataInicio = DateTime.Now.AddDays(-5);
        //        d1.DataFim = DateTime.Now;
        //        d1.Inserir();

        //        Assert.IsTrue(d1.IdDelegacao > 0);

        //        //Ou pode iniciar diretamente pelo modelo
        //        Delegacao d2 = new Delegacao();
        //        d2.LoginDe = "22222";
        //        d2.LoginPara = "22222";
        //        d2.DataInicio = DateTime.Now.AddDays(-5);
        //        d2.DataFim = DateTime.Now;
        //        d2.Inserir();

        //        Assert.IsTrue(d2.IdDelegacao > 0);
        //    }
        //}


        //[TestMethod]
        //public void TesteBuscarItemDB()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        List<Delegacao> d = pWeb.Obter<Delegacao>().Consultar();

        //        Assert.IsTrue(d.Count > 0);
        //    }
        //}

        //[TestMethod]
        //public void TesteLogDB()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi", true))
        //    {
        //        try
        //        {
        //            throw new NullReferenceException("Teste de log");
        //        }
        //        catch (Exception ex)
        //        {
        //            //Via reflection
        //            pWeb.Obter<Log>().Inserir(ex);

        //            //Diretamente
        //            new Log().Inserir(ex);
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TesteTransacaoDB()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        pWeb.IniciarTransacao();

        //        //Iniciado via reflection
        //        Delegacao d = pWeb.Obter<Delegacao>();
        //        d.LoginDe = "aaaaa";
        //        d.LoginPara = "bbbbb";
        //        d.DataInicio = DateTime.Now.AddDays(-5);
        //        d.DataFim = DateTime.Now;
        //        d.Inserir();

        //        //Mesmo intercalando a transação é do contexto
        //        Log l = new Log();
        //        l.DescricaoMensagem = "Teste de Transação";
        //        l.DescricaoDetalhe = "Teste de Transação";
        //        l.Inserir();

        //        pWeb.ConfirmarMudancas();
        //    }
        //}
        //#endregion

        //#region [ Testes de Listas ]
        //[TestMethod]
        //public void TesteCarregarLookup()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        TesteLookup t = new TesteLookup().Obter(4);
        //        t.ColunaB.CarregarDados();
        //        t.ColunaA.CarregarDados();
        //    }
        //}

        //[TestMethod]
        //public void TesteIncluirComLookup()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        TesteLookup t = new TesteLookup();
        //        t.ColunaA = new Lookup() { ID = 1 };
        //        t.ColunaB = new List<Lookup>()
        //        {
        //            new Lookup(){ID = 1},
        //            new Lookup(){ID = 3}
        //        };
        //        t.Titulo = "Teste1";
        //        t.Inserir();
        //    }
        //}

        //[TestMethod]
        //public void TesteIncluirItemSP()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        //Pode iniciar o objeto via reflection
        //        //ListaExemplo c1 = pWeb.Obter<ListaExemplo>();
        //        //c1.Descricao = "Teste 1";
        //        //c1.Fornecedor = "Iteris";
        //        //c1.PercCapex = 10;
        //        //c1.Titulo = "Titulo1";
        //        //c1.Inserir();

        //        ////Busca o id do item incluído
        //        //itemIncluido = c1.ID;

        //        ////Verifica se o item foi incluído
        //        //Assert.IsTrue(c1.ID != 0);

        //        ////Ou pode iniciar diretamente pelo modelo
        //        //ListaExemplo c2 = new ListaExemplo();
        //        //c2.Descricao = "Teste 2";
        //        //c2.Fornecedor = "Iteris";
        //        //c2.PercCapex = 20;
        //        //c2.Titulo = "Titulo 2";
        //        //c2.Inserir();

        //        ////Busca o id do item incluído
        //        //itemIncluido = c2.ID;

        //        ////Verifica se o item foi incluído
        //        //Assert.IsTrue(c2.ID != 0);
        //    }
        //}

        //[TestMethod]
        //public void TesteObterItemSP()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        //List<ListaExemplo> itens1 = pWeb.Obter<ListaExemplo>().Consultar(
        //        //    SPCamlComparisonOperator.Equal("Descricao", "Teste 1")
        //        //    );

        //        ////Verifica se o item foi retornado
        //        //Assert.IsTrue(itens1.Count > 0);

        //        ////Pesquisando via lambda no Sharepoint
        //        //List<ListaExemplo> itens2 = pWeb.Obter<ListaExemplo>().Consultar(i => i.Descricao == "Teste 1");

        //        ////Verifica se o item foi retornado
        //        //Assert.IsTrue(itens2.Count > 0);

        //    }
        //}

        //[TestMethod]
        //public void TesteExcluirItemSP()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        //List<ListaExemplo> itens = new ListaExemplo().Consultar();

        //        ////Verifica se o item foi retornado
        //        //Assert.IsTrue(itens.Count > 0);

        //        ////Exclui os itens
        //        //itens.Excluir();

        //        ////Busca a lista
        //        //itens = pWeb.Obter<ListaExemplo>().Consultar();

        //        ////Verifica se o item foi excluído
        //        //Assert.IsTrue(itens.Count == 0);
        //    }
        //}

        //[TestMethod]
        //public void AtualizarItemSP()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi/sites/WMO"))
        //    {
        //        Microsoft.SharePoint.Client.List lista = pWeb.ObterLista("Aditivos Gerais");
        //        pWeb.SPClient.Load(lista);
        //        pWeb.SPClient.ExecuteQuery();

        //        Microsoft.SharePoint.Client.ListItem item = lista.GetItemById(1);
        //        pWeb.SPClient.Load(item);
        //        pWeb.SPClient.ExecuteQuery();

        //        item["EstadoAtualFluxo"] = StatusFluxo.Cancelado.GetTitle();
        //        item.Update();

        //        pWeb.SPClient.ExecuteQuery();
        //    }
        //}

        //#endregion

        //#region [Teste Usuario]

        //[TestMethod]
        //public void TesteUser()
        //{
        //    String teste = StatusProposta.Andamento.ToString();

        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
        //        {
        //            PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
        //            {
        //                String login = "sp2013\\tr012192";
        //                var usuarioSP = PortalWeb.ContextoWebAtual.SPWeb.EnsureUser(login);
        //                PortalWeb.ContextoWebAtual.SPClient.Load(usuarioSP);
        //                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //            });
        //        }
        //    }

        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
        //        {
        //            PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
        //            {
        //                Int32 idUsuario = 0;
        //                String emailUsuario = "william.morita@iteris.com.br";

        //                if (emailUsuario != String.Empty)
        //                {
        //                    var usuarioSP = PortalWeb.ContextoWebAtual.SPWeb.SiteUsers.GetByEmail(emailUsuario);
        //                    PortalWeb.ContextoWebAtual.SPClient.Load(usuarioSP);
        //                    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        //                    idUsuario = usuarioSP.Id;
        //                }
        //                else
        //                    idUsuario = PortalWeb.ContextoWebAtual.UsuarioAtual.Id;
        //            });
        //        }
        //    }
        //}

        //#endregion

        //#region Teste Tarefa Grupo

        //[TestMethod]
        //public void TesteGruposConfig()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        ListaRaizenConfiguracaoFluxo configuracaoTarefa = new ListaRaizenConfiguracaoFluxo().Obter(3);
        //        Grupo grupo = configuracaoTarefa.GrupoTarefa;

        //        String usuarioEmails = String.Empty;

        //        if (grupo != null)
        //        {
        //            foreach (Usuario usuario in grupo.Usuarios)
        //                usuarioEmails += usuario.Email + ";";
        //        }
        //    }

        //}

        //#endregion

        //#region [Rezoneamento]


        //[TestMethod]
        //public void TesteRezoneamento()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        Int32? ibm = Convert.ToInt32("0001012687");
        //        List<EntidadePropostaSP> propostas = NegocioComum.ConsultarProposta(new Guid("C20F52BB-96F9-4374-9D0F-B3926EF2B2E0"), _ => _.Ibm == ibm
        //                       && _.UtilizaZoneamentoPadrao == true);
        //        propostas.Atualizar(new Guid("C20F52BB-96F9-4374-9D0F-B3926EF2B2E0"));
        //    }
        //}

        //#endregion

        //#region [TesteTemplateEmail]

        //[TestMethod]
        //public void TesteTemplate()
        //{
        //    String testes = String.Format("{0:0.00}", 11223.23);

        //    //using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    //{
        //    //    Parametro teste = new Parametro().Obter(1);

        //    //    String tipoTemplate = TipoTemplate.Cancelar.GetTitle();
        //    //    List<ListaRaizenTemplateEmails> templatesCancelar = new ListaRaizenTemplateEmails().Consultar(t => t.TipoTemplate == tipoTemplate);
        //    //    templatesCancelar.CarregarDados();

        //    //    ListaRaizenTemplateEmails template = templatesCancelar.Find(t => t.Fluxo != null && t.Fluxo.Titulo == "Aditivos Gerais");
        //    //    template = template != null ? template : templatesCancelar.Find(t => t.Fluxo == null) != null ? templatesCancelar.Find(t => t.Fluxo == null) : templatesCancelar.FirstOrDefault();
        //    //}
        //}

        //#endregion

        //#region [Teste FormatacaoEmail]

        //[TestMethod]
        //public void TesteFormatacaoEmail()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {

        //        List<Tarefa> tarefas = new Tarefa().Consultar(t =>
        //                    t.IdInstanciaFluxo == 616902
        //                && t.CodigoConfiguracao == 28
        //                && t.Ativo == true).OrderByDescending(i => i.IdTarefa).ToList();
        //        List<Tarefa> tarefas2 = tarefas.FindAll(t => t.Ativo && t.TarefaCompleta
        //        && !String.IsNullOrEmpty(t.DescricaoAcaoEfetuada)
        //        && t.AprovacoesAtuais != null && (Boolean)t.AprovacoesAtuais);

        //        Dictionary<TipoTag, Object> objetos = new Dictionary<TipoTag, Object>();
        //        ListaRaizenConfiguracaoFluxo configuracaoTarefa = new ListaRaizenConfiguracaoFluxo().Obter(23);
        //        InstanciaFluxo instancia = new InstanciaFluxo().Obter(616898);
        //        instancia.CarregarHistoricoWorkflow();

        //        NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Fluxo, instancia);
        //        List currentList = PortalWeb.ContextoWebAtual.ObterLista("RNIPs");
        //        ListItem item = currentList.GetItemById(instancia.CodigoItem);

        //        PortalWeb.ContextoWebAtual.SPClient.Load(item);
        //        PortalWeb.ContextoWebAtual.SPClient.Load(item.ParentList);
        //        PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //        NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Item, item);
        //        NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Lista, item.ParentList);
        //        configuracaoTarefa.TemplateEmailTarefa.CarregarDados();
        //        String emailFinal = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(configuracaoTarefa.TemplateEmailTarefa.Corpo));

        //        MensagemEmail mensagem = new MensagemEmail();
        //        mensagem.Corpo = emailFinal;
        //        mensagem.Para = "william.morita@Iteris.com.br";
        //        mensagem.Assunto = "teste";
        //        NegocioEmail.Enviar(mensagem);

        //    }
        //}

        //#endregion

        //#region [Teste Pdf]

        //[TestMethod]
        //public void TestePdf()
        //{
        //    using(PortalWeb pweb = new PortalWeb("http://pi"))
        //    {
        //        NegocioPdf.GerarPdf("RNIPs", 1, "teste.pdf", "www.google.com.br");
        //    }
        //}

        //#endregion

        //[TestMethod]
        //public void TestesSLAUtilizado()
        //{
        //    double slaUtilizado = 0;
        //    using (PortalWeb pweb = new PortalWeb("http://pi"))
        //    {
        //        ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(24);
        //        ConfiguracaoExpediente configExpediente = ObterConfiguracaoExpediente(configuracaoTarefa);
        //        //DateTime dataInicio = new DateTime(2017, 01, 24, 8, 48, 18);
        //        DateTime dataInicio = new DateTime(2017, 01, 20, 15, 9, 32);
        //        DateTime dataFim = new DateTime(2017, 01, 25, 15, 22, 38);
                
        //        slaUtilizado = DataHelper.CalcularTempoUtil(configExpediente, dataInicio, dataFim, true);

        //    }
        //}

        //[TestMethod]
        //public void TestesSlaPendente()
        //{
        //    using (PortalWeb pweb = new PortalWeb("http://pi"))
        //    {
        //        ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(24);
        //        ConfiguracaoExpediente configExpediente = ObterConfiguracaoExpediente(configuracaoTarefa);
        //        InstanciaFluxo instancia = new InstanciaFluxo().Obter(622910);

        //        Double slaUtilizado = ObterSlaUtilizado(instancia, configuracaoTarefa, configExpediente);

        //        //DateTime dataInicio = new DateTime(2017, 01, 26, 9, 27, 9);
        //        DateTime dataInicio = new DateTime(2017, 01, 27, 14, 9, 2);
        //        TimeSpan slaTarefa = new TimeSpan(15, 0, 0, 0);
        //        DateTime dataSLA = DataHelper.CalcularSLA(
        //            configExpediente,
        //            dataInicio,
        //            slaTarefa.TotalMinutes,
        //            1840
        //            );
        //    }
        //}

        //private static ConfiguracaoExpediente ObterConfiguracaoExpediente(ListaSP_RaizenConfiguracoesDeFluxo tarefa) //Tarefa como parâmetro
        //{
        //    String horarioInicio;
        //    String horarioSaida;
        //    ObterHorarioExpediente(tarefa, out horarioInicio, out horarioSaida);

        //    ConfiguracaoExpediente configuracaoExpediente = new ConfiguracaoExpediente();
        //    configuracaoExpediente.HorarioExpedienteEntrada = DataHelper.ConverterTextoHoras(horarioInicio, ConfiguracaoExpediente.ConfiguracaoDefault.HorarioExpedienteEntrada);
        //    configuracaoExpediente.HorarioExpedienteSaida = DataHelper.ConverterTextoHoras(horarioSaida, ConfiguracaoExpediente.ConfiguracaoDefault.HorarioExpedienteSaida);

        //    configuracaoExpediente.DiasUteisSemana = ObterDiasUteisSemana(tarefa.DiasUteis.ToList());

        //    configuracaoExpediente.Feriados = new List<DateTime>();
        //    List<ListaSP_Feriados> feriados = new ListaSP_Feriados().Consultar();
        //    if (feriados != null && feriados.Count > 0)
        //    {
        //        configuracaoExpediente.Feriados = (from feriado in feriados
        //                                           select feriado.Data).ToList();
        //    }

        //    return configuracaoExpediente;
        //}

        //private static void ObterHorarioExpediente(ListaSP_RaizenConfiguracoesDeFluxo tarefa, out String horarioInicio, out String horarioSaida)
        //{
        //    String[] horarioExpediente = tarefa.Expediente.Split('-');

        //    horarioInicio = horarioExpediente.Length > 0 ? horarioExpediente[0] : String.Empty;
        //    horarioSaida = horarioExpediente.Length > 1 ? horarioExpediente[1] : String.Empty;
        //}

        //private static List<DayOfWeek> ObterDiasUteisSemana(List<String> semana)
        //{
        //    List<DayOfWeek> diasSemana = new List<DayOfWeek>();

        //    foreach (DayOfWeek dia in Enum.GetValues(typeof(DayOfWeek)))
        //    {
        //        if (semana.Contains(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(dia))
        //            || semana.Contains(CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedDayName(dia))
        //            || semana.Contains(CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedDayName(dia))
        //            || semana.Contains(String.Format("{0},", dia.ToString()))
        //        )
        //            diasSemana.Add(dia);
        //    }

        //    if (diasSemana == null || diasSemana.Count <= 0)
        //        diasSemana = ConfiguracaoExpediente.ConfiguracaoDefault.DiasUteisSemana;

        //    return diasSemana;

        //}

        //private static Double ObterSlaUtilizado(InstanciaFluxo instanciaAtualDoFluxo, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa, ConfiguracaoExpediente configuracaoExpediente)
        //{
        //    Double slaUtilizado = 0;

        //    List<Tarefa> tarefasRespondidas = new Tarefa().Consultar(t =>
        //                    t.IdInstanciaFluxo == instanciaAtualDoFluxo.IdInstanciaFluxo
        //                && t.CodigoConfiguracao == configuracaoTarefa.ID
        //                && t.TarefaCompleta && t.DataFinalizado != null
        //        && t.AprovacoesAtuais != null && (Boolean)t.AprovacoesAtuais).OrderByDescending(i => i.IdTarefa).ToList();

        //    foreach (Tarefa tarefaRespondida in tarefasRespondidas)
        //    {
        //        double slaUtilizadoParcial = 0;
        //        slaUtilizadoParcial = DataHelper.CalcularTempoUtil(configuracaoExpediente, tarefaRespondida.DataAtribuido, (DateTime)tarefaRespondida.DataFinalizado, true);
        //        slaUtilizado += slaUtilizadoParcial;
        //    }

        //    return slaUtilizado;
        //}
    }
}
