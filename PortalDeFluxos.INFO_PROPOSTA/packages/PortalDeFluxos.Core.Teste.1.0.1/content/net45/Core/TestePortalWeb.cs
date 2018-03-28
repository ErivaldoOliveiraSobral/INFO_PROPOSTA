using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iteris;
using System.IO;
using Iteris.SharePoint;

namespace PortalDeFluxos.Core.Test
{
    [TestClass]

    public class TestePortalWeb
    {

        [TestMethod]
        public void TesteAtualizarEtapaParalela()
        {
            InstanciaFluxo instanciaAtualdoFluxo = new InstanciaFluxo();
            ListItem item;
            using (PortalWeb pWeb = new PortalWeb("http://pi"))
            {
                instanciaAtualdoFluxo = new InstanciaFluxo()
                {
                    CodigoInstanciaFluxo = Guid.Empty,
                    CodigoFluxo = Guid.Empty,
                    CodigoItem = Int32.Parse("1"),
                    CodigoLista = Guid.Parse("A7F5578F-FE0A-4F33-802F-2A7AE2961C90"),
                    NomeFluxo = "RNIPs",
                    EtapaParalela = true,
                    StatusFluxo = (int?)StatusFluxo.EmAndamento
                };

                item = pWeb.ObterItem(instanciaAtualdoFluxo.CodigoLista, instanciaAtualdoFluxo.CodigoItem);
                instanciaAtualdoFluxo.AtualizarInstanciaFluxo(DateTime.Now, item, null);
            }
            
        }

        [TestMethod]
        public void TesteGeradorAspxBD()
        {
            using (PortalWeb pWeb = new PortalWeb("http://pi"))
            {
                //using (ContextoBanco db = RetornarContextoDB(item.Contexto))
                //{

                //}
            }
            //TODO: Continuar - mascara decimal e INTMask

            String teste = NegocioGeradorTT.ControleAspxBd<AnoContratual>();
        }


        //[TestMethod]
        //public void TestePropriedadesWeb()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        Assert.IsTrue(pWeb.Configuracao.Imap != null && !String.IsNullOrWhiteSpace(pWeb.Configuracao.Imap.Servidor));
        //    }
        //}

        //[TestMethod]
        //public void TesteImpersonate()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        Log l = new Log();
        //        l.DescricaoDetalhe = "Teste";
        //        l.DescricaoMensagem = "Teste";
        //        l.DescricaoOrigem = "Teste";
        //        l.NomeProcesso = "Teste";
        //        l.Inserir();

        //        Log l2 = new Log();
        //        l2.DescricaoDetalhe = "Teste";
        //        l2.DescricaoMensagem = "Teste";
        //        l2.DescricaoOrigem = "Teste";
        //        l2.NomeProcesso = "Teste";
        //        pWeb.ExecutarComPrivilegioElevado(() =>
        //            {
        //                l2.Inserir();
        //            });

        //        Assert.IsTrue(l.LoginInclusao != l2.LoginInclusao);
        //    }
        //}

        //[TestMethod]
        //public void TesteLoadFiles()
        //{
        //    String teste1 = System.IO.Path.GetExtension("teste.aspx").Replace(".", String.Empty);
        //    String teste2 = System.IO.Path.GetExtension("teste").Replace(".", String.Empty);

        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        List currentList = pWeb.ObterLista("Aditivos Gerais");
        //        pWeb.SPClient.Load(currentList);
        //        pWeb.SPClient.ExecuteQuery();

        //        ListItem currentItem = currentList.GetItemById(1);
        //        pWeb.SPClient.Load(currentItem);
        //        pWeb.SPClient.ExecuteQuery();

        //        List<ConfiguracaoEmailAttachment> configuracoes = CarregarConfiguracoes(pWeb);
        //        ConfiguracaoEmailAttachment configuracao = configuracoes.FirstOrDefault(
        //                                                        i => i.NomeLista.Equals(currentList.Title, StringComparison.InvariantCultureIgnoreCase));

        //        var attInfo = new AttachmentCreationInformation
        //        {
        //            FileName = "2_Doc Regina.pdf",
        //            ContentStream =  new System.IO.MemoryStream(System.IO.File.ReadAllBytes(@"C:\AnexoLista\AditivosGerais\2_Doc Regina.pdf"))
        //        };

        //        pWeb.SPClient.Load(currentItem.AttachmentFiles);
        //        pWeb.SPClient.ExecuteQuery();

        //        for (int i = 0; i < currentItem.AttachmentFiles.Count; i++)
        //        {
        //            if (currentItem.AttachmentFiles[i].FileName == "2_Doc Regina.pdf")
        //                currentItem.AttachmentFiles[i].DeleteObject();
        //        }

        //        var att = currentItem.AttachmentFiles.Add(attInfo);
        //        pWeb.SPClient.Load(att);
        //        pWeb.SPClient.ExecuteQuery();

        //        //using (PortalWeb pWebDocumento = new PortalWeb(configuracao.SiteAnexo))
        //        //{
        //        //    List biblioteca = pWebDocumento.ObterLista(configuracao.NomeListaAnexo);
        //        //    pWebDocumento.SPClient.Load(biblioteca);
        //        //    pWebDocumento.SPClient.ExecuteQuery();

        //        //    FileCreationInformation newFile = new FileCreationInformation();
        //        //    newFile.Content = System.IO.File.ReadAllBytes(@"C:\AnexoLista\AditivosGerais\2_Doc Regina.pdf");
        //        //    newFile.Url = "2_Doc Regina.pdf";
        //        //    newFile.Overwrite = true;

        //        //    File documentoNovo = biblioteca.RootFolder.Files.Add(newFile);
        //        //    pWebDocumento.SPClient.Load(documentoNovo);
        //        //    pWebDocumento.SPClient.ExecuteQuery();

        //        //    pWebDocumento.SPClient.Load(documentoNovo.ListItemAllFields);
        //        //    pWebDocumento.SPClient.ExecuteQuery();

        //        //    documentoNovo.ListItemAllFields["Title"] = newFile.Url;
        //        //    DefinirValoresDocumento(documentoNovo.ListItemAllFields, currentItem, configuracao);
        //        //    documentoNovo.ListItemAllFields.Update();
        //        //    pWebDocumento.SPClient.ExecuteQuery();
        //        //}

        //    }
        //}

        ///// <summary>Busca a parametrização no banco, desserializa o objeto e retorna</summary>
        ///// <param name="url">Url de contexto</param>
        ///// <returns>Retorna a configuração do banco</returns>
        //private static List<ConfiguracaoEmailAttachment> CarregarConfiguracoes(PortalWeb pWeb)
        //{
        //    try
        //    {
        //        Parametro p = new Parametro().Obter((int)BLL.Utilitario.ParametroEnum.EmailAttachment);
        //        if (p == null || String.IsNullOrWhiteSpace(p.Valor))
        //            return new List<ConfiguracaoEmailAttachment>();

        //        return Serializacao.DeserializeFromJson<List<ConfiguracaoEmailAttachment>>(p.Valor);
        //    }
        //    catch (Exception ex)
        //    {
        //        new Log().Inserir("Serviço EmailApproval", "CarregarConfiguracoes", ex);
        //    }

        //    //Se der erro, retorna um objeto vazio para não tentar novamente
        //    return new List<ConfiguracaoEmailAttachment>();
        //}

        ///// <summary>Preenche as propriedades do item</summary>
        /////<param name="item">Item</param>
        /////<param name="entidade">Entidade</param>
        //public static void DefinirValoresDocumento(ListItem documentoNovo, ListItem currentItem, ConfiguracaoEmailAttachment configuracao)
        //{
        //    //Busca a lista de campos do item para preenchimento
        //    PropertyInfo listaCampos = documentoNovo.GetType().GetProperty("Item");
        //    //Varre as colunas definindo o valor no documentoNovo
        //    foreach (var dePara in configuracao.Mapeamento)
        //    {
        //        object valor = null;
        //        //Caso não tenha valor, troca para vazio
        //        if (dePara.Key != TipoAnexo.Item.ToString() && currentItem[dePara.Key] == null)
        //            valor = String.Empty;

        //        //Lookup
        //        if (dePara.Key == TipoAnexo.Item.ToString() || documentoNovo[dePara.Value] is FieldLookupValue)
        //        {
        //            FieldLookupValue lookup = new FieldLookupValue();

        //            if (dePara.Key == TipoAnexo.Item.ToString())//Caso específico (Lookup para o currentItem)
        //                lookup.LookupId = currentItem.Id;
        //            else if (currentItem[dePara.Key] is FieldLookupValue)
        //                lookup.LookupId = ((FieldLookupValue)currentItem[dePara.Key]).LookupId;

        //            if (lookup.LookupId > 0)//Se tiver valor definido
        //                listaCampos.SetValue(documentoNovo, lookup, new[] { dePara.Value });
        //        }
        //        else if (documentoNovo[dePara.Value] is FieldLookupValue[])
        //        {
        //            FieldLookupValue[] lookups = currentItem[dePara.Key] as FieldLookupValue[];

        //            if (lookups.Length > 0)//Se tiver valor definido
        //                listaCampos.SetValue(documentoNovo, lookups, new[] { dePara.Value });
        //        }
        //        else
        //            listaCampos.SetValue(documentoNovo, valor, new[] { dePara.Value });//Adiciona o valor na coleção
        //    }
        //}
        
        //[TestMethod]
        //public void TesteDeserializeNetworkCredential()
        //{
            
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        pWeb.SPClient.Load(pWeb.SPClient.Site.RootWeb, i => i.AllProperties);
        //        pWeb.SPClient.ExecuteQuery();

        //        System.Net.NetworkCredential teste1 = Serializacao.DeserializeFromJson<System.Net.NetworkCredential>(
        //            (String)pWeb.SPClient.Site.RootWeb.AllProperties[Constantes.ChaveWebContaSp2007]);
        //        Conta teste2 = new Conta(Serializacao.DeserializeFromJson<Dictionary<String, Object>>(
        //            (String)pWeb.SPClient.Site.RootWeb.AllProperties[Constantes.ChaveWebContaSp2007]));
        //    }
        //}

        //[TestMethod]
        //public void TesteGetListByUrl()
        //{
        //    String urlTeste = "http://pi/Lists/TesteTemplate/EditForm.aspx?ID=1&Source=http://pi/Lists/TesteTemplate/AllItems.aspx&ContentTypeId=0x01002689C9F1AB240848B1EDC01D9280E8AE&CodigoLista=B0781AAA-509A-4A2C-B706-929867CFA56E&CodigoItem=1";
            
        //    using (PortalWeb web = new PortalWeb("http://pi"))
        //    {
        //        List list = web.ObterListaPorUrl(urlTeste);
        //        Guid id = list.Id;
        //    }
        //}

        //[TestMethod]
        //public void TesteObterEstruturaField()
        //{
        //    using (PortalWeb web = new PortalWeb("http://pi"))
        //    {
        //        ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);

        //        ListaTesteTemplate listaTemplate = new ListaTesteTemplate();
        //        Field fieldDropdown = listaTemplate.ObterEstruturaCampo(i => i.Dropdownlist);
        //        Field fieldRadio = listaTemplate.ObterEstruturaCampo(i => i.RadioButton);
        //        Field fieldDropdownlistConsulta = listaTemplate.ObterEstruturaCampo(i => i.DropdownlistConsulta);

        //        Dictionary<String, String> dataSourceDropdown = listaTemplate.ObterDataSourceChoiceSP(i => i.Dropdownlist);
        //        Dictionary<String, String> dataSourceDropdownEmpty = listaTemplate.ObterDataSourceChoiceSP(i => i.Dropdownlist, true);
        //        Dictionary<String, String> dataSourceRadio = listaTemplate.ObterDataSourceChoiceSP(i => i.RadioButton);
        //        Dictionary<String, String> dataSourceRadioEmpty = listaTemplate.ObterDataSourceChoiceSP(i => i.RadioButton, true);
        //        Dictionary<String, String> dataSourceCheckEmpty = listaTemplate.ObterDataSourceChoiceSP(i => i.Checkbox, true);


        //        List<ListaTesteTemplate> listas = new ListaTesteTemplate().Consultar();

        //        Dictionary<String, String> dataSourceDropdownlistConsulta = listaTemplate.Consultar().ObterDataSourceLookup(true, i => i.Proposta);

        //        Dictionary<String, String> dataSourceDropdownlistConsulta2 = listaTemplate.ObterDataSourceChoiceSP(i => i.DropdownlistConsulta, false);

        //        ListaFeriado teste = new ListaFeriado().Obter(10);
        //    }
        //}

        //#region [Teste Template Form]

        //[TestMethod]
        //public void TesteTemplateForm()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);
        //        ListaTesteTemplate item2 = new ListaTesteTemplate().Obter(2);

        //        item.MultiText = "opa";
        //        item.Atualizar();
        //        item2.MultiText = "Opa";
        //        item2.Atualizar();
        //    }

        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        pWeb2.IniciarTransacao();

        //        ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);
        //        ListaTesteTemplate item2 = new ListaTesteTemplate().Obter(2);
        //        pWeb2.AdicionarTransacaoSP(item);

        //        item.MultiText = "opa1";
        //        item.Atualizar();
        //        item2.MultiText = "Opa1";
        //        item2.Atualizar();

        //        pWeb2.CancelarMudancas();
        //    }

        //    try
        //    {
        //        using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //        {
        //            ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);
        //            ListaTesteTemplate item2 = new ListaTesteTemplate().Obter(2);
        //            pWeb2.AdicionarTransacaoSP(item);

        //            item.MultiText = "opa2";
        //            item.Atualizar();
        //            item.MultiText = "opa2";
        //            item.Atualizar();
        //            pWeb2.ConfirmarMudancas();
        //        }
        //    }
        //    catch { }
            

        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        pWeb2.IniciarTransacao();

        //        ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);
        //        ListaTesteTemplate item2 = new ListaTesteTemplate().Obter(2);
        //        pWeb2.AdicionarTransacaoSP(item);

        //        item.MultiText = "opa3";
        //        item.Atualizar();
        //        item2.MultiText = "Opa3";
        //        item2.Atualizar();
        //        pWeb2.ConfirmarMudancas();
        //    }
        //}

        //[TestMethod]
        //public void TestObterUrl()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        String _urlPath = NegocioComum.ObterUrlFormUserControl(TipoUserControl.Proposta,new Guid("9EED43AD-F26A-47F7-8720-CE487EAF933D"));
        //        String _urlPathAprovacao = NegocioComum.ObterUrlFormUserControl(TipoUserControl.FormularioAprovacao, new Guid("9EED43AD-F26A-47F7-8720-CE487EAF933D"));
        //    }
        //}

        //#endregion

        //#region [Teste Anexos]

        //[TestMethod]
        //public void TesteUploadFile()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        FileCreationInformation newFile = new FileCreationInformation();
        //        newFile.Content = System.IO.File.ReadAllBytes(@"C:\Anexos\teste1.txt");
        //        newFile.Url = "teste1.txt";

        //        FileCreationInformation newFile2 = new FileCreationInformation();
        //        newFile2.Content = System.IO.File.ReadAllBytes(@"C:\Anexos\teste2.txt");
        //        newFile2.Url = "teste2.txt";
                
        //        ListaTesteTemplate item = new ListaTesteTemplate().Obter(1);
        //        ListaTesteTemplate item2 = new ListaTesteTemplate().Obter(2);

        //        item.UploadAnexo(newFile);
        //        item.UploadAnexo(newFile2);
        //        item2.UploadAnexo(newFile);
        //        item2.UploadAnexo(newFile2);

        //        Guid pastaTeste = new Guid("D0EF58D6-ACE5-4CAA-BB77-915C958F7998");
        //        Guid codigoLista = new Guid("9EED43AD-F26A-47F7-8720-CE487EAF933D");
                
        //        NegocioComum.UploadAnexo(codigoLista, newFile);
        //        List<Anexo> documentos = NegocioComum.ObterAnexos(codigoLista);
        //    }

        //}

        //[TestMethod]
        //public void TesteGetFiles()
        //{
        //    using (PortalWeb pWebDocumento = new PortalWeb("http://pi/sites/AnexosPropostas"))
        //    {
        //        List documentLibrary = BaseSP.ObterLista("Comodato");
        //    }
        //    //using (PortalWeb pweb = new PortalWeb("http://pi/sites/Anexos"))
        //    //{
        //    //    ListaAditivosGeraisSp item = new ListaAditivosGeraisSp().Obter(1);
        //    //}
            
        //    //using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    //{
        //    //    ListaAditivosGeraisSp item = new ListaAditivosGeraisSp().Obter(1283);
        //    //    List<Documento> documentos = item.GetDocuments();
        //    //}

        //    List<Anexo> documentos = new List<Anexo>();
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
        //            throw new Exception("Document Center não está configurado.");
        //        String nomeLista = "Aditivos Gerais";
        //        Guid codigoLista = new Guid("fb8bc33c-36bd-4333-a1c8-e698341d019a");
        //        Int32 codigoItem = 1283;

        //        if (nomeLista == String.Empty)
        //        {
        //            List listaOrigem = ComumSP.ObterLista(codigoLista);
        //            nomeLista = listaOrigem.Title;
        //        }

        //        Folder folder = null;
               

        //        using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
        //        {
        //            List documentLibrary = BaseSP.ObterLista(nomeLista);
        //            ComumSP.TryGetFolder(documentLibrary, codigoItem.ToString(), out folder);
        //            if (folder != null)
        //            {
        //                PortalWeb.ContextoWebAtual.SPClient.Load(folder.Files, files => files.Include(
        //                          f => f.Author
        //                        , f => f.Name
        //                        , f => f.TimeCreated
        //                        , f => f.ServerRelativeUrl
        //                        , f => f.ListItemAllFields.Id
        //                        , f => f.ListItemAllFields["EncodedAbsUrl"]
        //                        , f => f.ListItemAllFields["Usuario"]));
        //                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        //                foreach (Microsoft.SharePoint.Client.File file in folder.Files)
        //                {
        //                    if(file.ListItemAllFields["Usuario"] != null)
        //                        documentos.Add(new Anexo(file));
        //                }
        //            }
        //        }
        //    }

        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        FileInformation fileInformation = null;
        //        using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
        //        {
        //            foreach (var item in documentos)
        //            {
        //                String[] splitRelativeUrl = item.RelativeUrl.ToString().Split('/');
        //                String nomeArquivo = splitRelativeUrl[splitRelativeUrl.Length - 1];
        //                fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(PortalWeb.ContextoWebAtual.SPClient, item.RelativeUrl);
        //            }

        //        }
        //    }
        //}

        //[TestMethod]
        //public void TestFileName()
        //{
        //    char[] invalidPathChars = Path.GetInvalidPathChars();
        //}

        //#endregion

        //#region [Teste Fluxo]

        //[TestMethod]
        //public void TesteIniciarFluxo()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        //ListaAditivosGeraisSp item = new ListaAditivosGeraisSp().Obter(1281);
        //        //List<Anexo> documentos = item.ObterAnexos();
        //        //pWeb2.ExecutarComPrivilegioElevado(() =>
        //        //{
        //        //    IniciarFluxo(item);
        //        //});
        //    }
        //}

        //public void IniciarFluxo(EntidadeSP item, String nomeWorkflow = "")
        //{
        //    List lista = ComumSP.ObterLista(item);

        //    if (lista == null)
        //        throw new Exception("Lista não foi encontrada.");

        //    #region [Busca o fluxo]
            
        //    nomeWorkflow = nomeWorkflow == String.Empty ? ComumSP.ObterNomeLista(item) : nomeWorkflow; //Por default o nome do workflow é igual ao nome da lista
        //    WorkflowServicesManager wfServiceManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
        //    WorkflowSubscriptionService workflowSubscriptionService = wfServiceManager.GetWorkflowSubscriptionService();

        //    // Obtem workflow associations
        //    var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByList(lista.Id);
        //    PortalWeb.ContextoWebAtual.SPClient.Load(workflowAssociations);
        //    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //    var def = from defs in workflowAssociations
        //              where defs.Name == nomeWorkflow
        //              select defs;
        //    WorkflowSubscription wfSubscription = def.FirstOrDefault();

        //    if (wfSubscription == null)
        //        throw new Exception("Workflow não foi encontrado.");

        //    WorkflowInstanceService wfInstanceService = wfServiceManager.GetWorkflowInstanceService();
        //    if (FluxoAtivo(item, lista, wfInstanceService, wfSubscription))//Verifica se exite fluxo ativo
        //        return;
        //    #endregion

        //    #region [Inicia o workflow]

        //    var startParameters = new Dictionary<string, object>();
        //    wfInstanceService.StartWorkflowOnListItem(wfSubscription, item.ID, startParameters);
        //    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //    #endregion
        //}

        //public Boolean FluxoAtivo(EntidadeSP item, List lista, WorkflowInstanceService wfInstanceService, WorkflowSubscription wfSubscription)
        //{
        //    WorkflowInstanceCollection wfInstances    = wfInstanceService.EnumerateInstancesForListItem(lista.Id,item.ID);
        //    PortalWeb.ContextoWebAtual.SPClient.Load(wfInstances);
        //    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //    var def = from defs in wfInstances
        //              where     defs.WorkflowSubscriptionId == wfSubscription.Id
        //                    &&  defs.Status == WorkflowStatus.Started
        //              select defs;

        //    WorkflowInstance instance = def.FirstOrDefault();

        //    return instance != null;
        //}

        //[TestMethod]
        //public void TesteFluxoReiniciado()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        PortalWeb.ContextoWebAtual.IniciarTransacao();
                
        //        ListaAditivosGeraisSp item = new ListaAditivosGeraisSp().Obter(1283);
        //        item.ReiniciarFluxo();


        //        //PortalWeb.ContextoWebAtual.AdicionarTransacaoSP(item);
        //        //item.ReiniciarFluxo();
                
        //        //PortalWeb.ContextoWebAtual.ConfirmarMudancas();//Confirma as mudanças
        //    }
        //}


        //#endregion

        //#region [Teste Permissao]

        //[TestMethod]
        //public void TesteGetRolesList()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        User currentUser = PortalWeb.ContextoWebAtual.SPWeb.CurrentUser;
        //        PortalWeb.ContextoWebAtual.SPClient.Load(currentUser);
        //        PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //        GroupCollection gruposUsuario = currentUser.Groups;
        //        PortalWeb.ContextoWebAtual.SPClient.Load(gruposUsuario);
        //        PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //        ListaTemplateForm item = new ListaTemplateForm().Obter(2);
        //        List listaTesteTemplate = ComumSP.ObterLista(item);

        //        PortalWeb.ContextoWebAtual.SPClient.Load(listaTesteTemplate, l => l.RoleAssignments);
        //        PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

        //        String login = currentUser.LoginName;
        //        List<RoleDefinition> rolesUsuario = new List<RoleDefinition>();
        //        foreach (RoleAssignment grupo in listaTesteTemplate.RoleAssignments)
        //        {
        //            PortalWeb.ContextoWebAtual.SPClient.Load(grupo, g => g.RoleDefinitionBindings, g => g.PrincipalId);
        //            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        //            if(gruposUsuario.Any(g => g.Id == grupo.PrincipalId))
        //                foreach (var role in grupo.RoleDefinitionBindings)
        //                    rolesUsuario.Add(role);
        //        }


        //        Boolean estruturaComercial = rolesUsuario.Any(r =>
        //                        r.Name == PortalRoles.RaizenEstruturaComercial.GetTitle());
        //        Boolean permissao = false;
                
        //        if(estruturaComercial)
        //        {
        //            EntidadePropostaSP _proposta = NegocioComum.ObterProposta(listaTesteTemplate.Id, item.ID);
        //            permissao = (_proposta.Gdr != null && _proposta.Gdr.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
        //                   (_proposta.Cdr != null && _proposta.Cdr.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
        //                   (_proposta.DiretorVendas != null && _proposta.DiretorVendas.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
        //                   (_proposta.GerenteTerritorio != null && _proposta.GerenteTerritorio.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login) ||
        //                   (_proposta.GerenteRegiao != null && _proposta.GerenteRegiao.Login == PortalWeb.ContextoWebAtual.UsuarioAtual.Login);
        //        }else
        //        {
        //            permissao = rolesUsuario.Any(r =>
        //                        r.Name == PortalRoles.RaizenColaborador.GetTitle() ||
        //                        r.RoleTypeKind == RoleType.Administrator ||
        //                        r.RoleTypeKind == RoleType.Contributor ||
        //                        r.RoleTypeKind == RoleType.Editor);
        //        }
                               

        //    }

        //}

        //#endregion

        //#region [TesteTarefa]
        
        //  [TestMethod]
        //public void TesteAprovacaoTarefa()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        Tarefa tarefaAtual = new Tarefa().Obter(3513663);
        //        List<Tarefa> tarefas = new Tarefa().Consultar(t => t.CodigoTarefa == tarefaAtual.CodigoTarefa && t.IdTarefa != tarefaAtual.IdTarefa && t.Ativo == true);
        //        pWeb2.IniciarTransacao();
        //        tarefaAtual.TarefaCompleta = true;
        //        tarefaAtual.Atualizar();
        //        new InstanciaFluxo().Obter(tarefaAtual.IdInstanciaFluxo).AndarFluxo(tarefaAtual);
        //        pWeb2.ConfirmarMudancas();
        //        if (tarefas != null && tarefas.Count > 0)
        //        {
        //            tarefas.ForEach(item =>
        //            {
        //                item.TarefaCompleta = true;
        //            });
        //            tarefas.Atualizar();
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TesteCalculoSLA()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        ConfiguracaoExpediente config = new ConfiguracaoExpediente();
        //        config.HorarioExpedienteEntrada = new TimeSpan(8, 0, 0);
        //        config.HorarioExpedienteSaida = new TimeSpan(18, 0, 0);
        //        config.DiasUteisSemana = new List<DayOfWeek>();
        //        config.DiasUteisSemana.Add(DayOfWeek.Monday);
        //        config.DiasUteisSemana.Add(DayOfWeek.Tuesday);
        //        config.DiasUteisSemana.Add(DayOfWeek.Wednesday);
        //        config.DiasUteisSemana.Add(DayOfWeek.Thursday);
        //        config.DiasUteisSemana.Add(DayOfWeek.Friday);
        //        List<DayOfWeek> diasUteisSemana = config.DiasUteisSemana;
        //        List<DateTime> feriados = new List<DateTime>();
        //        feriados.Add(new DateTime(2015, 5, 2));
        //        config.Feriados = feriados;

        //        DateTime dataInicio = new DateTime(2017, 01, 06, 15, 49, 3);
        //        DateTime dataFim = new DateTime(2017, 01, 10, 16, 38, 3);

        //        double slaUtilizado = DataHelper.CalcularTempoUtil(config, dataInicio, dataFim);
                
        //    }
        //}

        //[TestMethod]
        //public void TesteObterSuperior()
        //{
        //    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        //    {
        //        ListaRaizenConfiguracaoFluxo configTarefa = new ListaRaizenConfiguracaoFluxo().Obter(24);
        //        ListItem item = null;
        //        List currentList = PortalWeb.ContextoWebAtual.ObterLista("RNIPs");

        //        item = currentList.GetItemById(14);

        //        PortalWeb.ContextoWebAtual.SPClient.Load(item);
        //        PortalWeb.ContextoWebAtual.SPClient.Load(item.ParentList);
        //        PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                
        //        ObterSuperior(configTarefa, item);
        //    }
        //}

        //#endregion

        //#region [Teste Controles]

        //  [TestMethod]
        //  public void TesteControle()
        //  {
        //      Object uc = new Object();
        //      uc.GetType().GetMethod("OPOP");
        //  }       

        //#endregion

        //#region [Teste Grupos]

        //[TestMethod]    
        //public void BuscarGruposPorLogin()
        //  {
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        pWeb.BuscarGruposPorLogin("SP2013\\Administrator");
        //    }
        //  }

        //#endregion

        ////#region [Teste Controle Ano Contratual]

        ////[TestMethod]
        ////public void ObterTipoAnoContratual()
        ////{

        ////    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        ////    {
        ////        #region [Eliminar valores]
        ////        List<AnoContratual> anoContratual = new AnoContratual().Consultar(i => i.IdProposta == 1
        ////                    && i.IdTipoProposta == (Int32)TipoPropostaPai.Rnip && i.Ativo);
        ////        Dictionary<Int32, Decimal?> valorBonificacaoEliminar = new Dictionary<int, Decimal?>();
        ////        valorBonificacaoEliminar.Add(1, (Decimal?)123.23);
        ////        anoContratual.PopularValores(i => i.Bonificacao, valorBonificacaoEliminar, true);
        ////        anoContratual.Atualizar();
        ////        #endregion
        ////    }

        ////    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        ////    {
        ////        #region [Adicionar novos Valores]

        ////        List<AnoContratual> anoContratual = new AnoContratual().Consultar(i => i.IdProposta == 1
        ////                    && i.IdTipoProposta == (Int32)TipoPropostaPai.Rnip && i.Ativo);
        ////        Dictionary<Int32, Decimal?> valorBonificacaoAdicionar = new Dictionary<int, Decimal?>();
        ////        valorBonificacaoAdicionar.Add(1, (Decimal?)123.23);
        ////        valorBonificacaoAdicionar.Add(2, (Decimal?)223.23);
        ////        valorBonificacaoAdicionar.Add(3, (Decimal?)323.23);
        ////        anoContratual.PopularValores(i => i.Bonificacao, valorBonificacaoAdicionar, true);
        ////        anoContratual.Atualizar();
        ////        #endregion
        ////    }
        ////    using (PortalWeb pWeb2 = new PortalWeb("http://pi"))
        ////    {
        ////        #region [Apenas Atualizar - quantidade de anos iguais]

        ////        List<AnoContratual> anoContratual = new AnoContratual().Consultar(i => i.IdProposta == 1
        ////                    && i.IdTipoProposta == (Int32)TipoPropostaPai.Rnip && i.Ativo);
        ////        Dictionary<Int32, Decimal?> valorBonificacaoModificar = new Dictionary<int, Decimal?>();
        ////        valorBonificacaoModificar.Add(1, (Decimal?)111);
        ////        valorBonificacaoModificar.Add(2, (Decimal?)222);
        ////        valorBonificacaoModificar.Add(3, (Decimal?)333);
        ////        anoContratual.PopularValores(i => i.Bonificacao, valorBonificacaoModificar, true);
        ////        anoContratual.Atualizar();
        ////        #endregion
        ////    }

        ////}

        ////#endregion

        //[TestMethod]
        //public void ConfiguracaoEmail()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        Parametro p = new Parametro().Obter((int)ParametroEnum.EmailAttachment);
        //    }
        //}

        //[TestMethod]
        //public void TesteHeadline()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        List<ListaRaizenAprovacoesHeadlineSize> aprovadoresHeadlines = new ListaRaizenAprovacoesHeadlineSize().Consultar();
        //        List<ListaHeadlineSizeVendas> aprovadoresHeadline3 = new ListaHeadlineSizeVendas().Consultar();

        //        List<ListaHeadlineSizeVendas> aprovadoresHeadline = new ListaHeadlineSizeVendas().Consultar(
        //                SPCamlLogicalOperator.And(
        //                    SPCamlComparisonOperator.LessOrEqual(PropriedadesItem.HeadlineInicial.GetTitle(), 800, SPCamlFieldOptions.None),
        //                    SPCamlComparisonOperator.GreaterOrEqual(PropriedadesItem.HeadlineFinal.GetTitle(), 800, SPCamlFieldOptions.None)
        //                )
        //            );
        //    }
        //}

        //[TestMethod]
        //public void EnviarEmailCustomAction()
        //{
        //    using (PortalWeb pWeb = new PortalWeb("http://pi"))
        //    {
        //        ListaRaizenTemplateEmails template = new ListaRaizenTemplateEmails().Obter(11);
        //        NegocioEmail.EnviarEmailTemplate(template);
        //    }
        //}

    }
}               
                
                

                
                