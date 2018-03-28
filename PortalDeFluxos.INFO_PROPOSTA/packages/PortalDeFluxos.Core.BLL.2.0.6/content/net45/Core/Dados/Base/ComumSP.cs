using Iteris.SharePoint;
using Iteris.SharePoint.Design;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Iteris.SharePoint;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class ComumSP : BaseSP
    {
        #region [ Métodos de Inclusão ]
        /// <summary>Retorna o objeto solicitado</summary>
        /// <returns>Objeto</returns>
        public static void Inserir<T>(T item, Guid codigoLista = new Guid()) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (item == null)
                return;

            //Busca a lista
            List lista = codigoLista != Guid.Empty ? ObterLista(codigoLista) : ObterLista(item); 

            //Cria o item
            ListItemCreationInformation itemInfo = new ListItemCreationInformation();
            ListItem itemNovo = lista.AddItem(itemInfo);

            //Busca os valoes para inclusão
            DefinirValoresListItem(itemNovo, item);

            //Efetua a inclusão no Sharepoint
            itemNovo.Update();

            //Busca o item
            item.Contexto.SPClient.ExecuteQuery();

            //Define o ID do item incluído
            ((EntidadeSP)(object)item).ID = itemNovo.Id;
        }

        /// <summary>Efetua a inclusão da lista enviada</summary>
        /// <param name="type">lista de itens</param>
        /// <returns>Objeto</returns>
        public static void Inserir<T>(List<T> itens, Guid codigoLista = new Guid()) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            //Busca a lista
            List lista = codigoLista != Guid.Empty ? ObterLista(codigoLista) : ObterLista(itens[0]);

            //Insere todos os itens
            itens.ForEach(i =>
            {
                //Cria o item
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                ListItem itemNovo = lista.AddItem(itemInfo);

                //Define os valores dos campos
                DefinirValoresListItem(itemNovo, i);

                //Efetua a inclusão no Sharepoint
                itemNovo.Update();
            });

            itens[0].Contexto.SPClient.ExecuteQuery();
        }
        #endregion

        #region [ Atualizar ]
        /// <summary>Efetua a atualização do objeto informado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static void Atualizar<T>(T item, Guid codigoLista = new Guid()) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (item == null)
                return;

            //Busca a lista
            List lista = codigoLista != Guid.Empty ? ObterLista(codigoLista) : ObterLista(item);

            //Busca o item
            ListItem itemSP = lista.GetItemById(((EntidadeSP)(object)item).ID);

            //Define os valores dos campos
            DefinirValoresListItem(itemSP, item);

            //Efetua a inclusão no Sharepoint
            itemSP.Update();
            item.Contexto.SPClient.ExecuteQuery();
        }

        /// <summary>Atualiza a lista de objetos informados</summary>
        /// <typeparam name="T">Tipo de objeto para ser atualizado</typeparam>
        /// <param name="itens">Lista de entidades</param>
        /// <returns></returns>
        public static void Atualizar<T>(List<T> itens, Guid codigoLista = new Guid()) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            //Busca a lista
            List lista = codigoLista != Guid.Empty ? ObterLista(codigoLista) : ObterLista(itens[0]);

            //Insere todos os itens
            itens.ForEach(i =>
            {
                //Busca o item
                ListItem itemSP = lista.GetItemById(((EntidadeSP)(object)i).ID);

                //Define os valores dos campos
                DefinirValoresListItem(itemSP, i);

                //Efetua a inclusão no Sharepoint
                itemSP.Update();
            });

            //Envia as atualizações para o SP
            itens[0].Contexto.SPClient.ExecuteQuery();
        }
        #endregion

        #region [ Métodos para Excluir o item ]
        /// <summary>Exclui o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static Boolean Excluir<T>(T item) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (item == null)
                return false;

            //Busca a lista
            List lista = ObterLista(item);

            //Busca o item
            ListItem itemSP = lista.GetItemById(((EntidadeSP)(object)item).ID);

            //Efetua a exclusão no Sharepoint
            itemSP.DeleteObject();
            item.Contexto.SPClient.ExecuteQuery();

            return true;
        }

        /// <summary>Exclui a lista de objetos informados</summary>
        /// <param name="itens">Lista de entidades</param>
        /// <returns>Objeto</returns>
        public static void Excluir<T>(List<T> itens) where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            //Busca a lista
            List lista = ObterLista(itens[0]);

            itens.ForEach(i =>
            {
                //Busca o item
                ListItem itemSP = lista.GetItemById(((EntidadeSP)(object)i).ID);

                //Efetua a exclusão no Sharepoint
                itemSP.DeleteObject();
            });

            //Efetua as exclusões
            itens[0].Contexto.SPClient.ExecuteQuery();
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static void Excluir<T>(T entidade, List<int> chaves) where T : Entidade, new()
        {
            if (chaves.Count == 0)
                return;

            //Busca a lista
            List lista = ObterLista(entidade);

            chaves.ForEach(i =>
            {
                //Busca o item
                ListItem itemSP = lista.GetItemById(i);

                //Efetua a exclusão no Sharepoint
                itemSP.DeleteObject();
            });

            //Efetua as exclusões
            entidade.Contexto.SPClient.ExecuteQuery();
        }
        #endregion

        #region [ Métodos Obter Entidade]
        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static T Obter<T>(T entidade, Int32 chave) where T : Entidade, new()
        {
            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            ListItem itemSP = lista.GetItemById(chave);
            entidade.Contexto.SPClient.Load(itemSP);
            try
            {
                entidade.Contexto.SPClient.ExecuteQuery();
            }
            catch
            {
                itemSP = null;
            }


            //Converte para entidade
            return ConverterParaEntidade<T>(entidade.Contexto, itemSP);
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static T Obter<T>(T entidade, Expression<Func<T, Boolean>> filtro) where T : Entidade, new()
        {
            //Busca o filtro CAML
            CamlQuery caml = new CamlQuery();
            caml.ViewXml = new QueryParser().Parse(filtro.Body, 1);

            //Retorna o item
            return Obter<T>(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static T Obter<T>(T entidade, SPCamlCondition filtro) where T : Entidade, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade).FirstOrDefault();
            else
            {
                //Busca a lista
                List lista = ObterLista(entidade);

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, filtro);

                //Retorna o item
                return Obter<T>(entidade, caml);
            }
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static T Obter<T>(T entidade, CamlQuery filtro) where T : Entidade, new()
        {
            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            ListItemCollection itemSP = lista.GetItems(filtro);
            entidade.Contexto.SPClient.Load(itemSP);
            entidade.Contexto.SPClient.ExecuteQuery();

            //Converte para entidade
            if (itemSP.Count == 0)
                return default(T);

            //Efetua o cast
            return ConverterParaEntidade<T>(entidade.Contexto, itemSP.FirstOrDefault());
        }
        #endregion

        #region [ Métodos Obter Item]

        /// <summary>Retorna o objeto solicitado</summary>
        /// <returns>Objeto</returns>
        public static ListItem ObterItem(EntidadeSP entidade)
        {
            if (entidade.ID <= 0)
                return null;

            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            ListItem itemSP = lista.GetItemById(entidade.ID);
            entidade.Contexto.SPClient.Load(itemSP);
            try
            {
                entidade.Contexto.SPClient.ExecuteQuery();
            }
            catch
            {
                itemSP = null;
            }

            return itemSP;
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static ListItem ObterItem(EntidadeSP entidade, Int32 chave)
        {
            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            ListItem itemSP = lista.GetItemById(chave);
            entidade.Contexto.SPClient.Load(itemSP);
            try
            {
                entidade.Contexto.SPClient.ExecuteQuery();
            }
            catch
            {
                itemSP = null;
            }

            return itemSP;
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static ListItem ObterItem(EntidadeSP entidade, Expression<Func<EntidadeSP, Boolean>> filtro)
        {
            //Busca o filtro CAML
            CamlQuery caml = new CamlQuery();
            caml.ViewXml = new QueryParser().Parse(filtro.Body, 1);

            //Retorna o item
            return ObterItem(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static ListItem ObterItem(EntidadeSP entidade, SPCamlCondition filtro)
        {
            if (filtro == null)
                return ConsultarItem(entidade).FirstOrDefault();
            else
            {
                //Busca a lista
                List lista = ObterLista(entidade);

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, filtro);

                //Retorna o item
                return ObterItem(entidade, caml);
            }
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static ListItem ObterItem(EntidadeSP entidade, CamlQuery filtro)
        {
            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            ListItemCollection itemSP = lista.GetItems(filtro);
            entidade.Contexto.SPClient.Load(itemSP);
            entidade.Contexto.SPClient.ExecuteQuery();

            //Converte para entidade
            if (itemSP.Count == 0)
                return null;

            return itemSP.FirstOrDefault();
        }

        #endregion

        #region [ Métodos Listar ]

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<T> Consultar<T>(T entidade) where T : Entidade, new()
        {
            //Cria um filtro limitando por 1000 itens
            CamlQuery caml = CamlQuery.CreateAllItemsQuery(1000);

            //Efetua a pesquisa dos itens
            return Consultar<T>(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<T> Consultar<T>(T entidade, List<Int32> chaves) where T : Entidade, new()
        {
            //Se não tiver chaves, retorna a lista vazia
            if (chaves == null || chaves.Count == 0)
                return new List<T>();

            //Busca a lista
            List lista = ObterLista(entidade);

            //Cria o filtro pelos IDs
            CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, SPCamlComparisonOperator.In("ID", chaves.ToArray()));

            //Efetua a pesquisa dos itens
            return Consultar<T>(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<T> Consultar<T>(T entidade, Expression<Func<T, Boolean>> filtro) where T : Entidade, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade);
            else
            {
                //Busca o filtro CAML
                CamlQuery caml = new CamlQuery();
                caml.ViewXml = new QueryParser().Parse(filtro.Body);

                //Efetua a pesquisa dos itens
                return Consultar<T>(entidade, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<T> Consultar<T>(T entidade, SPCamlCondition filtro) where T : Entidade, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade);
            else
            {
                //Busca a lista
                List lista = ObterLista(entidade);

                //Valida se a lista existe
                entidade.Contexto.SPClient.Load(lista);
                entidade.Contexto.SPClient.ExecuteQuery();

                if (lista == null)
                    throw new NullReferenceException("Lista não encontrada.");

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, filtro);

                //Efetua a pesquisa dos itens
                return Consultar<T>(entidade, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<T> Consultar<T>(T entidade, CamlQuery filtro) where T : Entidade, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade);
            else
            {
                List<T> retorno = default(List<T>);

                //Busca a lista
                List lista = ObterLista(entidade);

                //Busca o item
                ListItemCollection itens = lista.GetItems(filtro);
                entidade.Contexto.SPClient.Load(itens);
                entidade.Contexto.SPClient.ExecuteQuery();

                //Converte todos os itens e retorna
                retorno = itens.ToList().ConvertAll((Converter<ListItem, T>)(i => ConverterParaEntidade<T>(entidade.Contexto, i))).ToList();

                return retorno;
            }
        }

        /// <summary>Retorna a quantidade de objetos encontrados com o filtro informado</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Quantidade de objetos encontrados</returns>
        public static Int32 ConsultarQtd<T>(T entidade, SPCamlCondition filtro) where T : Entidade, new()
        {
            Int32 retorno = 0;

            //Busca a lista
            List lista = ObterLista(entidade);

            //Busca o item
            if (filtro != null)
            {
                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, filtro);

                ListItemCollection itens = lista.GetItems(caml);
                entidade.Contexto.SPClient.Load(itens);
                entidade.Contexto.SPClient.ExecuteQuery();
                retorno = itens.Count;
            }
            else
            {
                entidade.Contexto.SPClient.Load(lista, i => i.ItemCount);
                entidade.Contexto.SPClient.ExecuteQuery();
                retorno = lista.ItemCount;
            }
            return retorno;
        }

        /// <summary>Retorna a quantidade de objetos encontrados com o filtro informado</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Quantidade de objetos encontrados</returns>
        public static Int32 ConsultarQtd(Guid codigoLista, SPCamlCondition filtro)
        {
            Int32 retorno = 0;

            //Busca a lista
            List lista = ObterLista(codigoLista);

            //Busca o item
            if (filtro != null)
            {
                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(PortalWeb.ContextoWebAtual, lista, filtro);

                ListItemCollection itens = lista.GetItems(caml);
                PortalWeb.ContextoWebAtual.SPClient.Load(itens);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                retorno = itens.Count;
            }
            else
            {
                PortalWeb.ContextoWebAtual.SPClient.Load(lista, i => i.ItemCount);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                retorno = lista.ItemCount;
            }
            return retorno;
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<EntidadePropostaSP> ConsultarPropostas(Guid codigoLista)
        {
            //Cria um filtro limitando por 1000 itens
            CamlQuery caml = CamlQuery.CreateAllItemsQuery(1000);

            //Efetua a pesquisa dos itens
            return ConsultarPropostas(codigoLista, caml);
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<EntidadePropostaSP> ConsultarPropostas(Guid codigoLista, List<Int32> chaves)
        {
            //Se não tiver chaves, retorna a lista vazia
            if (chaves == null || chaves.Count == 0)
                return new List<EntidadePropostaSP>();

            //Busca a lista
            List lista = ObterLista(codigoLista);

            //Cria o filtro pelos IDs
            CamlQuery caml = SPCamlBuilder.Build(PortalWeb.ContextoWebAtual, lista, SPCamlComparisonOperator.In("ID", chaves.ToArray()));

            //Efetua a pesquisa dos itens
            return ConsultarPropostas(codigoLista, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadePropostaSP> ConsultarPropostas(Guid codigoLista, Expression<Func<EntidadePropostaSP, Boolean>> filtro)
        {
            if (filtro == null)
                return ConsultarPropostas(codigoLista);
            else
            {
                //Busca o filtro CAML
                CamlQuery caml = new CamlQuery();
                caml.ViewXml = new QueryParser().Parse(filtro.Body);

                //Efetua a pesquisa dos itens
                return ConsultarPropostas(codigoLista, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadePropostaSP> ConsultarPropostas(Guid codigoLista, SPCamlCondition filtro)
        {
            if (filtro == null)
                return ConsultarPropostas(codigoLista);
            else
            {
                //Busca a lista
                List lista = ObterLista(codigoLista);

                //Valida se a lista existe
                PortalWeb.ContextoWebAtual.SPClient.Load(lista);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                if (lista == null)
                    throw new NullReferenceException("Lista não encontrada.");

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(PortalWeb.ContextoWebAtual, lista, filtro);

                //Efetua a pesquisa dos itens
                return ConsultarPropostas(codigoLista, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadePropostaSP> ConsultarPropostas(Guid codigoLista, CamlQuery filtro)
        {
            if (filtro == null)
                return ConsultarPropostas(codigoLista);
            else
            {
                List<EntidadePropostaSP> retorno = default(List<EntidadePropostaSP>);

                //Busca a lista
                List lista = ObterLista(codigoLista);

                //Busca o item
                ListItemCollection itens = lista.GetItems(filtro);
                PortalWeb.ContextoWebAtual.SPClient.Load(itens);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                //Converte todos os itens e retorna
                retorno = itens.ToList().ConvertAll((Converter<ListItem, EntidadePropostaSP>)(i => ConverterParaEntidade<EntidadePropostaSP>(PortalWeb.ContextoWebAtual, i))).ToList();

                return retorno;
            }
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<EntidadeHeadlineSP> ConsultarHeadline(String nomeLista)
        {
            //Cria um filtro limitando por 1000 itens
            CamlQuery caml = CamlQuery.CreateAllItemsQuery(1000);

            //Efetua a pesquisa dos itens
            return ConsultarHeadline(nomeLista, caml);
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<EntidadeHeadlineSP> ConsultarHeadline(String nomeLista, List<Int32> chaves)
        {
            //Se não tiver chaves, retorna a lista vazia
            if (chaves == null || chaves.Count == 0)
                return new List<EntidadeHeadlineSP>();

            //Busca a lista
            List lista = ObterLista(nomeLista);

            //Cria o filtro pelos IDs
            CamlQuery caml = SPCamlBuilder.Build(PortalWeb.ContextoWebAtual, lista, SPCamlComparisonOperator.In("ID", chaves.ToArray()));

            //Efetua a pesquisa dos itens
            return ConsultarHeadline(nomeLista, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadeHeadlineSP> ConsultarHeadline(String nomeLista, Expression<Func<EntidadePropostaSP, Boolean>> filtro)
        {
            if (filtro == null)
                return ConsultarHeadline(nomeLista);
            else
            {
                //Busca o filtro CAML
                CamlQuery caml = new CamlQuery();
                caml.ViewXml = new QueryParser().Parse(filtro.Body);

                //Efetua a pesquisa dos itens
                return ConsultarHeadline(nomeLista, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadeHeadlineSP> ConsultarHeadline(String nomeLista, SPCamlCondition filtro)
        {
            if (filtro == null)
                return ConsultarHeadline(nomeLista);
            else
            {
                //Busca a lista
                List lista = ObterLista(nomeLista);

                //Valida se a lista existe
                PortalWeb.ContextoWebAtual.SPClient.Load(lista);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                if (lista == null)
                    throw new NullReferenceException("Lista não encontrada.");

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(PortalWeb.ContextoWebAtual, lista, filtro);

                //Efetua a pesquisa dos itens
                return ConsultarHeadline(nomeLista, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<EntidadeHeadlineSP> ConsultarHeadline(String nomeLista, CamlQuery filtro)
        {
            if (filtro == null)
                return ConsultarHeadline(nomeLista);
            else
            {
                List<EntidadeHeadlineSP> retorno = default(List<EntidadeHeadlineSP>);

                //Busca a lista
                List lista = ObterLista(nomeLista);

                //Busca o item
                ListItemCollection itens = lista.GetItems(filtro);
                PortalWeb.ContextoWebAtual.SPClient.Load(itens);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                //Converte todos os itens e retorna
                retorno = itens.ToList().ConvertAll((Converter<ListItem, EntidadeHeadlineSP>)(i => ConverterParaEntidade<EntidadeHeadlineSP>(PortalWeb.ContextoWebAtual, i))).ToList();

                return retorno;
            }
        }

        #endregion

        #region [ Métodos Listar Item]
        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<ListItem> ConsultarItem(EntidadeSP entidade)
        {
            //Cria um filtro limitando por 1000 itens
            CamlQuery caml = CamlQuery.CreateAllItemsQuery(1000);

            //Efetua a pesquisa dos itens
            return ConsultarItem(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<ListItem> ConsultarItem(EntidadeSP entidade, List<Int32> chaves)
        {
            //Se não tiver chaves, retorna a lista vazia
            if (chaves == null || chaves.Count == 0)
                return new List<ListItem>();

            //Busca a lista
            List lista = ObterLista(entidade);

            //Cria o filtro pelos IDs
            CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, SPCamlComparisonOperator.In("ID", chaves.ToArray()));

            //Efetua a pesquisa dos itens
            return ConsultarItem(entidade, caml);
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<ListItem> ConsultarItem(EntidadeSP entidade, Expression<Func<EntidadeSP, Boolean>> filtro)
        {
            if (filtro == null)
                return ConsultarItem(entidade);
            else
            {
                //Busca o filtro CAML
                CamlQuery caml = new CamlQuery();
                caml.ViewXml = new QueryParser().Parse(filtro.Body);

                //Efetua a pesquisa dos itens
                return ConsultarItem(entidade, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<ListItem> ConsultarItem(EntidadeSP entidade, SPCamlCondition filtro)
        {
            if (filtro == null)
                return ConsultarItem(entidade);
            else
            {
                //Busca a lista
                List lista = ObterLista(entidade);

                //Valida se a lista existe
                entidade.Contexto.SPClient.Load(lista);
                entidade.Contexto.SPClient.ExecuteQuery();

                if (lista == null)
                    throw new NullReferenceException("Lista não encontrada.");

                //Monta a CAML
                CamlQuery caml = SPCamlBuilder.Build(entidade.Contexto, lista, filtro);

                //Efetua a pesquisa dos itens
                return ConsultarItem(entidade, caml);
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<ListItem> ConsultarItem(EntidadeSP entidade, CamlQuery filtro)
        {
            if (filtro == null)
                return ConsultarItem(entidade);
            else
            {
                List<ListItem> retorno = default(List<ListItem>);

                //Busca a lista
                List lista = ObterLista(entidade);

                //Busca o item
                ListItemCollection itens = lista.GetItems(filtro);
                entidade.Contexto.SPClient.Load(itens);
                entidade.Contexto.SPClient.ExecuteQuery();

                //Converte todos os itens e retorna
                retorno = itens.ToList();

                return retorno;
            }
        }

        #endregion

        #region [Folder]

        /// <summary>
        /// Cria uma pasta na RootFolder
        /// </summary>
        /// <param name="list"></param>
        /// <param name="folderName"></param>
        public static Folder CreateFolder(List list, string folderName)
        {
            Folder folder = null;
            if (!TryGetFolder(list, folderName, out folder))
            {
                if (folderName.Contains('/') && !TryGetFolder(list, folderName, out folder))
                    CreateFolder(list, folderName.Substring(0, folderName.LastIndexOf('/')));

                var info = new ListItemCreationInformation
                {
                    UnderlyingObjectType = FileSystemObjectType.Folder,
                    LeafName = folderName.Contains('/') ? folderName.Substring(folderName.LastIndexOf('/') + 1, folderName.Length - (folderName.LastIndexOf('/') + 1)) : folderName,
                    FolderUrl = folderName.Contains('/') ? string.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, folderName.Substring(0, folderName.LastIndexOf('/')))
                        : list.RootFolder.ServerRelativeUrl
                };

                var newItem = list.AddItem(info);
                newItem["Title"] = folderName;
                newItem.Update();
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                if (newItem != null)
                {
                    PortalWeb.ContextoWebAtual.SPClient.Load(newItem.Folder);
                    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                }
                folder = newItem.Folder;
            }

            return folder;
        }

        /// <summary>
        /// Verifica se uma pasta existe
        /// </summary>
        /// <param name="list"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool TryGetFolder(List list, string folderName, out Folder folder)
        {
            PortalWeb.ContextoWebAtual.SPClient.Load(list.RootFolder);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            var folderRelativeUrl = string.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, folderName);
            folder = PortalWeb.ContextoWebAtual.SPWeb.GetFolderByServerRelativeUrl(folderRelativeUrl);
            
            try
            {
                PortalWeb.ContextoWebAtual.SPClient.Load(folder);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
            }
            catch
            {
                folder = null;
            }

            return folder != null;
        }

        public static void ChangeFolderName(List list, Folder folder, String newFolderName)
        {
            ListItem folderItem = folder.ListItemAllFields;
            PortalWeb.ContextoWebAtual.SPClient.Load(folderItem, f => f["Name"]);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
            folderItem["Name"] = newFolderName;
            folder.Update();
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        }

        public static void MoveFilesTo(Folder fromfolder, Folder toFolder)
        {
            PortalWeb.ContextoWebAtual.SPClient.Load(fromfolder.Files);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            foreach (var file in fromfolder.Files)
            {
                var targetFileUrl = file.ServerRelativeUrl.Replace(fromfolder.ServerRelativeUrl, toFolder.ServerRelativeUrl);
                file.MoveTo(targetFileUrl, MoveOperations.Overwrite);
            }
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
        }

        #endregion

        #region [File]

        public static File CreateFile(List list, Folder folder, FileCreationInformation newFile)
        {
            File file = null;
            var fileRelativeUrl = string.Format("{0}/{1}", folder.ServerRelativeUrl, newFile.Url);

            if (TryGetFile(list, fileRelativeUrl, out file))
            {
                if (newFile.Overwrite)
                    file.DeleteObject();
                else
                    return file;
            }

            file = folder.Files.Add(newFile);
            PortalWeb.ContextoWebAtual.SPClient.Load(file);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            return file;
        }

        public static bool TryGetFile(List list, String fileRelativeUrl, out File file)
        {
            CamlQuery qry = new CamlQuery();
            qry.ViewXml = string.Format("<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name=\"FileRef\"/><Value Type=\"Url\">{0}</Value></Eq></Where></Query></View>", fileRelativeUrl);

            ListItemCollection items = list.GetItems(qry);
            PortalWeb.ContextoWebAtual.SPClient.Load(items);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
            file = items.FirstOrDefault() != null ? items.FirstOrDefault().File : null;

            return file != null;
        }

        #endregion
    }
}
