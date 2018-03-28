using Iteris.Collections.Specialized;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Atributos;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PortalDeFluxos.Core.BLL.Dados
{
    #region [ Classe para manipular os objetos do Sharepoint ]
    public class BaseSP
    {
        #region [ Métodos para buscar objetos do Sharepoint ]
        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="web">Web</param>
        /// <param name="entidade">Entidade</param>
        /// <returns></returns>
        public static List ObterLista(Entidade entidade)
        {
            List lista = null;
            String nomeLista = ObterNomeLista(entidade);

            if (String.IsNullOrEmpty(nomeLista))
                throw new NullReferenceException("Nome da lista não encontrado.");

            lista = entidade.Contexto.SPWeb.Lists.GetByTitle(nomeLista);

            //Valida se a lista existe
            //entidade.Contexto.SPContext.Load(lista);

            if (lista == null)
                throw new NullReferenceException("Lista não encontrada.");

            return lista;
        }

        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="nomeLista">Obtem a lista através do nome da lista</param>
        /// <returns></returns>
        public static List ObterLista(String nomeLista)
        {
            List lista = null;

            if (String.IsNullOrEmpty(nomeLista))
                throw new NullReferenceException("Nome da lista não encontrado.");

            lista = PortalWeb.ContextoWebAtual.SPWeb.Lists.GetByTitle(nomeLista);
            
            //Valida se a lista existe
            PortalWeb.ContextoWebAtual.SPClient.Load(lista);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            if (lista == null)
                throw new NullReferenceException("Lista não encontrada.");

            return lista;
        }

        public static List ObterLista(Guid Id)
        {
            List lista = null;

            if (Id == null)
                throw new NullReferenceException("Id da lista não encontrado.");


            lista = PortalWeb.ContextoWebAtual.SPWeb.Lists.GetById(Id);

            //Valida se a lista existe
            PortalWeb.ContextoWebAtual.SPClient.Load(lista);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            if (lista == null)
                throw new NullReferenceException("Lista não encontrada.");

            return lista;
        }

        public static List ObterLista(EntidadeSP item)
        {
            return BaseSP.ObterLista(ComumSP.ObterNomeLista(item));
        }

        /// <summary>Obtém o objeto SPList que representa a entidade</summary>
        /// <param name="urlLista">Url lista: Lists/AditivosGerais</param>
        /// <returns></returns>
        public static List ObterListaPorUrl(String urlLista)
        {
            List lista = null;

            if (urlLista != null && urlLista.Split('/').Length > 1)
            {
                String lists = "Lists/";
                Int32 index1 = urlLista.IndexOf(lists);
                Int32 index2 = urlLista.IndexOf("/", index1 + lists.Length);
                urlLista = urlLista.Substring(index1, index2 - index1);
            }

            if (String.IsNullOrEmpty(urlLista))
                throw new NullReferenceException("Url da lista não encontrado.");

            var listFolder = PortalWeb.ContextoWebAtual.SPWeb.GetFolderByServerRelativeUrl(urlLista);
            PortalWeb.ContextoWebAtual.SPClient.Load(listFolder.Properties);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            var listId = new Guid(listFolder.Properties["vti_listname"].ToString());
            lista = ObterLista(listId);

            if (lista == null)
                throw new NullReferenceException("Lista não encontrada.");

            return lista;
        }

        /// <summary>Obtém o nome da lista que representa a entidade</summary>
        /// <param name="entidade">Entidade</param>
        /// <returns></returns>
        public static String ObterNomeLista(Entidade entidade)
        {
            Attribute attributo = entidade.GetType().GetCustomAttributes(typeof(InternalNameAttribute)).FirstOrDefault();
            if (attributo == null)
                return null;
            return ((InternalNameAttribute)attributo).Name;
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <returns>Objeto</returns>
        public static ListItem ObterItem(Guid codigoLista, Int32 codigoItem)
        {
            if (codigoItem <= 0)
                return null;

            List lista = ObterLista(codigoLista);

            ListItem itemSP = lista.GetItemById(codigoItem);
            PortalWeb.ContextoWebAtual.SPClient.Load(itemSP);
            try
            {
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
            }
            catch
            {
                itemSP = null;
            }

            return itemSP;
        }

        #endregion

        #region [ Métodos para conversão de objetos ]
        /// <summary>Converte o SPListItem para a Entidade</summary>
        /// <typeparam name="T">Tipo da Entidade de Retorno</typeparam>
        /// <param name="item">Item da Lista</param>
        /// <returns></returns>
        public static T ConverterParaEntidade<T>(PortalWeb contexto, ListItem item) where T : Entidade, new()
        {
            //Entidade para retorno
            T entidade = new T();

            //Define o mesmo contexto
            entidade.Contexto = contexto;

            //Verifica se existe algum objeto a ser convertido
            if (item == null)
                return entidade;
            InternalNameAttribute internalName = null;

            //Busca as propriedades de mapeamento
            PropertyInfo[] propriedades = entidade.GetType().GetProperties();

            try
            {
                foreach (PropertyInfo p in propriedades)
                {
                    internalName = null;
                    object valor = null;

                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;

                    //Busca o nome da coluna do Sharepoint
                    internalName = ((InternalNameAttribute)titulo.GetValue(0));

                    //Busca o valor retornado pelo Sharepoint
                    valor = item[internalName.Name];

                    //Caso não tenha valor, não preenche
                    if (valor == null)
                        continue;

                    //Preenche as propriedades do item de acordo com o tipo recebido
                    //ENUM
                    if (p.PropertyType.IsEnum)
                        p.SetValue(entidade, Enum.Parse(p.PropertyType, (String)valor));
                    //PeoplePicker
                    else if (valor is FieldUserValue)
                    {
                        UsuarioGrupoBase usuarioGrupo = contexto.BuscarUsuarioGrupo((FieldUserValue)valor, false);
                        if (usuarioGrupo != null && (p.PropertyType == typeof(Usuario) || p.PropertyType == typeof(Grupo)))
                            p.SetValue(entidade, usuarioGrupo);
                    }
                    //Lookup
                    else if (valor is FieldLookupValue && p.PropertyType.BaseType == typeof(EntidadeSP))
                    {
                        EntidadeSP lookup = (EntidadeSP)Activator.CreateInstance(p.PropertyType);
                        lookup.ID = ((FieldLookupValue)valor).LookupId;
                        lookup.Titulo = ((FieldLookupValue)valor).LookupValue;
                        p.SetValue(entidade, lookup);
                    }
                    //Multi Lookup - List<EntidadeSP>
                    else if (valor is FieldLookupValue[] &&
                            p.PropertyType.IsGenericType &&
                            p.PropertyType.GenericTypeArguments[0].BaseType == typeof(EntidadeSP))
                    {
                        //Busca a coleção de itens
                        FieldLookupValue[] lookups = valor as FieldLookupValue[];

                        //Instância uma nova lista
                        var tipoLista = typeof(List<>);
                        var contrutorLista = tipoLista.MakeGenericType(p.PropertyType.GenericTypeArguments[0]);
                        var listaEntidades = (IList)Activator.CreateInstance(contrutorLista);

                        //Inclui os itens retornados pelo Sharepoint
                        foreach (var lookup in lookups)
                        {
                            EntidadeSP entidadeNova = (EntidadeSP)Activator.CreateInstance(p.PropertyType.GenericTypeArguments[0]);
                            entidadeNova.ID = lookup.LookupId;
                            entidadeNova.Titulo = lookup.LookupValue;
                            listaEntidades.Add(entidadeNova);
                        }
                        p.SetValue(entidade, listaEntidades);
                    }
                    else if (valor is FieldUserValue[])
                    {
                        FieldUserValue[] users = valor as FieldUserValue[];

                        //Instância uma nova lista
                        var tipoLista = typeof(List<>);
                        var contrutorLista = tipoLista.MakeGenericType(p.PropertyType.GenericTypeArguments[0]);
                        var listaUsuarios = (IList)Activator.CreateInstance(contrutorLista);

                        foreach (var usuario in users)
                        {
                            UsuarioGrupoBase usuarioGrupo = contexto.BuscarUsuarioGrupo(usuario, false);
                            if (usuarioGrupo != null)
                                listaUsuarios.Add(usuarioGrupo);
                        }
                        p.SetValue(entidade, listaUsuarios);
                    }
                    else //Qualquer outro tipo
                    {
                        //Se for tipo nullable, busca o tipo base
                        Type type = p.PropertyType.GetGenericArguments().FirstOrDefault();
                        if (type == null)
                            type = p.PropertyType;

                        p.SetValue(entidade, Convert.ChangeType(valor, type));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Erro Campo: {0}", internalName != null ? internalName.Name : ""), ex);
            }

            return entidade;
        }

        /// <summary>Preenche o SPListItem com as propriedades da entidade</summary>
        /// <param name="item">SPListItem</param>
        /// <param name="entidade">Entidade</param>
        public static void ConverterParaListItem(ListItem item, Entidade entidade)
        {
            if (item == null || entidade == null)
                return;

            //Varre as colunas definindo o valor no ListItem
            PropertyInfo[] propriedades = entidade.GetType().GetProperties();
            foreach (PropertyInfo p in propriedades)
            {
                InternalNameAttribute internalName = null;
                object valor = null;

                object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                if (titulo == null || titulo.Length == 0)
                    continue;

                //Busca o nome da coluna do Sharepoint
                internalName = ((InternalNameAttribute)titulo.GetValue(0));

                //Busca o valor da coluna na entidade
                valor = p.GetValue(entidade);

                //Verifica o tipo
                //valor = (Convert.ToDecimal(valor) * 100);

                //Busca o valor do item
                if (valor == null && item.ParentList.Fields.GetByInternalNameOrTitle(internalName.Name).Required)
                    item[internalName.Name] = String.Empty;
                else
                    item[internalName.Name] = valor;
            }
        }

        /// <summary>Gera um NameObjectCollection com as propriedades da entidade</summary>
        /// <param name="entidade">Entidade</param>
        /// <returns></returns>
        public static NameObjectCollection ConverterParaNameObjectCollection(Entidade entidade)
        {
            NameObjectCollection itens = new NameObjectCollection();

            if (entidade == null)
                return itens;

            //Varre as colunas definindo o valor no ListItem
            PropertyInfo[] propriedades = entidade.GetType().GetProperties();
            foreach (PropertyInfo p in propriedades)
            {
                InternalNameAttribute internalName = null;
                object valor = null;

                object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                if (titulo == null || titulo.Length == 0)
                    continue;

                //Busca o nome da coluna do Sharepoint
                internalName = ((InternalNameAttribute)titulo.GetValue(0));

                if (internalName.Name.Equals("ID", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //Não envia campos somente leitura (O Sharepoint não permite atualizar)
                if (internalName.ReadOnly)
                    continue;

                //Busca o valor da coluna na entidade
                valor = p.GetValue(entidade);

                //Verifica o tipo
                //valor = (Convert.ToDecimal(valor) / 100);

                //Caso não tenha valor, troca para vazio
                if (valor == null)
                    valor = String.Empty;

                //Adiciona o valor na coleção
                itens.Add(internalName.Name, valor);
            }

            return itens;
        }

        /// <summary>Preenche as propriedades do item</summary>
        ///<param name="item">Item</param>
        ///<param name="entidade">Entidade</param>
        public static void DefinirValoresListItem(ListItem item, Entidade entidade)
        {
            if (entidade == null)
                return;

            //Busca a lista de campos do item para preenchimento
            PropertyInfo listaCampos = item.GetType().GetProperty("Item");

            //Varre as colunas definindo o valor no ListItem
            PropertyInfo[] propriedades = entidade.GetType().GetProperties();
            foreach (PropertyInfo p in propriedades)
            {
                InternalNameAttribute internalName = null;
                object valor = null;

                object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                if (titulo == null || titulo.Length == 0)
                    continue;

                //Busca o nome da coluna do Sharepoint
                internalName = ((InternalNameAttribute)titulo.GetValue(0));

                if (internalName.Name.Equals("ID", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //Não envia campos somente leitura (O Sharepoint não permite atualizar)
                if (internalName.ReadOnly)
                    continue;

                //Busca o valor da coluna na entidade
                valor = p.GetValue(entidade);

                // PeoplePicker
                if (valor is Usuario)
                {
                    //Verifica se foi informado algum item
                    if (valor == null)
                        continue;

                    //Busca a classe com as informações de usuário
                    Usuario usuario = (Usuario)valor;

                    //Define o valor
                    listaCampos.SetValue(item, usuario.Id > 0 ? usuario.Id : (Int32?)null, new[] { internalName.Name });
                }
                // Lookup
                else if (p.PropertyType.BaseType == typeof(EntidadeSP))
                {
                    //Verifica se foi informado algum item
                    if (valor == null)
                        continue;

                    //Cria o Lookup para inclusão e define o valor
                    FieldLookupValue lookup = new FieldLookupValue();
                    lookup.LookupId = ((EntidadeSP)valor).ID;

                    //Se tiver valor definido
                    if (((EntidadeSP)valor).ID > 0)
                        listaCampos.SetValue(item, lookup, new[] { internalName.Name });
                }
                //Multi Lookup - List<EntidadeSP>
                else if (p.PropertyType.IsGenericType && p.PropertyType.GenericTypeArguments[0].BaseType == typeof(EntidadeSP))
                {
                    IList itens = (IList)valor;

                    //Verifica se foi informado algum item
                    if (itens == null || itens.Count == 0)
                        continue;

                    //Busca a coleção de itens
                    List<FieldLookupValue> lookups = new List<FieldLookupValue>();

                    //Adiciona os itens
                    foreach (var itemLookup in itens)
                        if (((EntidadeSP)itemLookup).ID > 0)
                            lookups.Add(new FieldLookupValue() { LookupId = ((EntidadeSP)itemLookup).ID });

                    listaCampos.SetValue(item, lookups, new[] { internalName.Name });
                }
                else
                {
                    //Caso não tenha valor, troca para vazio
					if (valor == null && p.PropertyType != typeof(DateTime?))
						valor = String.Empty;

                    //Adiciona o valor na coleção
                    listaCampos.SetValue(item, valor, new[] { internalName.Name });
                }
            }
        }
        #endregion
    }
    #endregion
}
