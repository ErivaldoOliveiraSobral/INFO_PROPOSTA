using Iteris.SharePoint.Design;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Atributos;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Iteris;
using System.Globalization;
using System.Text;

namespace PortalDeFluxos.Core.BLL.Negocio
{
	public static class NegocioComum
	{
		#region [ Inserir ]
		/// <summary>Efetua a inclusão do objeto</summary>
		/// <param name="value">Objeto a ser incluído</param>
		public static void Inserir<T>(this T item, Guid codigoLista = new Guid())
			where T : Entidade, new()
		{
			if (item.GetType().IsSubclassOf(typeof(EntidadeSP)))
				ComumSP.Inserir<T>(item, codigoLista);
			else
				ComumDB.Inserir<T>(item);
		}

		/// <summary>Efetua a inclusão do objeto</summary>
		/// <param name="itens">Lista de itens para serem incluídos</param>
		public static void Inserir<T>(this List<T> itens, Guid codigoLista = new Guid())
			where T : Entidade, new()
		{
			if (itens == null || itens.Count == 0)
				return;

			if (itens.First().GetType().IsSubclassOf(typeof(EntidadeSP)))
				ComumSP.Inserir<T>(itens, codigoLista);
			else
				ComumDB.Inserir<T>(itens);
		}
		#endregion

		#region [ Atualizar ]
		/// <summary>Atualiza o item informado</summary>
		/// <param name="item">Item</param>
		public static void Atualizar<T>(this T item, Guid codigoLista = new Guid())
			where T : Entidade, new()
		{
			if (item.GetType().IsSubclassOf(typeof(EntidadeSP)))
				ComumSP.Atualizar<T>(item, codigoLista);
			else
				ComumDB.Atualizar<T>(item);
		}

		/// <summary>Inclui / Atualiza os itens da lista</summary>
		/// <param name="itens">Itens a serem Incluídos/Atualizados</param>
		public static void Atualizar<T>(this List<T> itens, Guid codigoLista = new Guid())
			where T : Entidade, new()
		{
			if (itens != null && itens.Count <= 0)
				return;

			if (itens.First().GetType().IsSubclassOf(typeof(EntidadeSP)))
			{
				ComumSP.Atualizar<T>(itens.Where(i => ((EntidadeSP)(object)i).ID != 0).ToList(), codigoLista);
				ComumSP.Inserir<T>(itens.Where(i => ((EntidadeSP)(object)i).ID == 0).ToList(), codigoLista);
			}
			else
				ComumDB.Atualizar<T>(itens);
		}

		/// <summary>Atualiza o item informado</summary>
		/// <param name="item">Item</param>
		public static void AtualizarPropEspecifico<T>(this T item, List<String> propriedades)
			where T : EntidadeDB, new()
		{
			ComumDB.Atualizar<T>(item, propriedades);
		}

		public static void AtualizarPropEspecifico<T>(this T item, T itemOriginal)
			where T : EntidadeDB, new()
		{
			ComumDB.Atualizar<T>(item, item.ObterPropriedadesAlteradas(itemOriginal));
		}

		public static List<String> ObterPropriedadesAlteradas(this EntidadeDB entidade, EntidadeDB entidadeOriginal)
		{
			List<String> propriedades = new List<String>();
			if (entidadeOriginal.GetType() != entidade.GetType())
				return null;

			foreach (var oProperty in entidadeOriginal.GetType().GetProperties())
			{
				try
				{
					var oOldValue = oProperty.GetValue(entidade, null);
					var oNewValue = oProperty.GetValue(entidadeOriginal, null);
					// this will handle the scenario where either value is null
					if (!object.Equals(oOldValue, oNewValue))
						propriedades.Add(oProperty.Name);
				}
				catch { }

			}
			return propriedades;
		}

		#endregion

		#region [ Métodos para Excluir o item ]
		/// <summary>Exclui o item</summary>
		/// <param name="item">Item</param>
		/// <returns>Se o item foi excluído com sucesso</returns>
		public static void Excluir<T>(this T item)
			where T : Entidade, new()
		{
			if (item.GetType().IsSubclassOf(typeof(EntidadeSP)))
				ComumSP.Excluir<T>(item);
			else
				ComumDB.Excluir<T>(item);
		}

		/// <summary>Exclui a lista de itens</summary>
		/// <param name="itens">Lista de itens</param>
		public static void Excluir<T>(this List<T> itens)
			where T : Entidade, new()
		{
			if (itens != null && itens.Count > 0)
			{

				if (itens.First().GetType().IsSubclassOf(typeof(EntidadeSP)))
					ComumSP.Excluir<T>(itens);
				else
					ComumDB.Excluir<T>(itens);
			}
			else
				return;
		}

		/// <summary>Exclui a lista de itens informados</summary>
		/// <param name="entidade">Entidade que será excluída</param>
		/// <param name="chaves">Chaves a serem excluídas</param>
		public static void Excluir<T>(this T entidade, List<int> chaves)
			where T : Entidade, new()
		{
			if (entidade.GetType().IsSubclassOf(typeof(EntidadeSP)))
				ComumSP.Excluir<T>(entidade, chaves);
			else
				ComumDB.Excluir<T>(entidade, chaves);
		}
		#endregion

		#region [ Métodos Obter ]
		/// <summary>Retorna o item pela chave</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chave">Chave do item para ser retornado</param>
		/// <returns>Item</returns>
		public static T Obter<T>(this T entidade, Int32 chave)
			where T : Entidade, new()
		{
			if (entidade.GetType().IsSubclassOf(typeof(EntidadeSP)))
				return ComumSP.Obter<T>(entidade, chave);
			else
				return ComumDB.Obter<T>(entidade, chave);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Item</returns>
		public static T Obter<T>(this T entidade, Expression<Func<T, Boolean>> filtro)
			where T : Entidade, new()
		{
			if (entidade.GetType().IsSubclassOf(typeof(EntidadeSP)))
				return ComumSP.Obter<T>(entidade, filtro);
			else
				return ComumDB.Obter<T>(entidade, filtro);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Item</returns>
		public static T Obter<T>(this T entidade, CamlQuery filtro)
			where T : EntidadeSP, new()
		{
			return ComumSP.Obter<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static T Obter<T>(this T entidade, SPCamlCondition filtro)
			where T : EntidadeSP, new()
		{
			return ComumSP.Obter<T>(entidade, filtro);
		}

		/// <summary>Retorna o item pela chave</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chave">Chave do item para ser retornado</param>
		/// <returns>Item</returns>
		public static ListItem ObterItem(this EntidadeSP entidade)
		{
			return ComumSP.ObterItem(entidade);
		}

		/// <summary>Retorna o item pela chave</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chave">Chave do item para ser retornado</param>
		/// <returns>Item</returns>
		public static ListItem ObterItem(this EntidadeSP entidade, Int32 chave)
		{
			return ComumSP.ObterItem(entidade, chave);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Item</returns>
		public static ListItem ObterItem(this EntidadeSP entidade, Expression<Func<EntidadeSP, Boolean>> filtro)
		{
			return ComumSP.ObterItem(entidade, filtro);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Item</returns>
		public static ListItem ObterItem(this EntidadeSP entidade, CamlQuery filtro)
		{
			return ComumSP.ObterItem(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static ListItem ObterItem(this EntidadeSP entidade, SPCamlCondition filtro)
		{
			return ComumSP.ObterItem(entidade, filtro);
		}

		public static EntidadePropostaSP ObterProposta(Guid codigoLista, Int32 codigoItem)
		{
			ListItem itemProposta = BaseSP.ObterItem(codigoLista, codigoItem);
			EntidadePropostaSP proposta = null;

			try
			{
				proposta = BaseSP.ConverterParaEntidade<EntidadePropostaSP>(PortalWeb.ContextoWebAtual, itemProposta);
				proposta.ContentType = itemProposta.ObterContentTypeId();

			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return proposta;
		}

		#endregion

		#region [ Métodos Consultar ]
		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T>(this T entidade)
			where T : Entidade, new()
		{
			if (typeof(T).IsSubclassOf(typeof(EntidadeSP)) || typeof(T) == typeof(EntidadeSP))
				return ComumSP.Consultar<T>(entidade);
			else
				return ComumDB.Consultar<T>(entidade);
		}

		/// <summary>Retorna a lista de items de acordo com as chaves</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chaves">Chaves para pesquisa</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T>(this T entidade, List<Int32> chaves)
			where T : Entidade, new()
		{
			if (typeof(T).IsSubclassOf(typeof(EntidadeSP)) || typeof(T) == typeof(EntidadeSP))
				return ComumSP.Consultar<T>(entidade, chaves);
			else
				return ComumDB.Consultar<T>(entidade, chaves);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T>(this T entidade, Expression<Func<T, Boolean>> filtro)
			where T : Entidade, new()
		{
			if (typeof(T).IsSubclassOf(typeof(EntidadeSP)) || typeof(T) == typeof(EntidadeSP))
				return ComumSP.Consultar<T>(entidade, filtro);
			else
				return ComumDB.Consultar<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T, TOrdem>(this T entidade, Expression<Func<T, Boolean>> filtro, Expression<Func<T, TOrdem>> ordenacao, Int32 paginaAtual, Int32 itensPagina, Boolean descendente = false)
			where T : EntidadeDB, new()
		{
			return ComumDB.Consultar<T, TOrdem>(entidade, filtro, ordenacao, paginaAtual, itensPagina, descendente);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <param name="distinct">Coluna usada no distinct</param>
		/// <returns>Itens</returns>
		public static List<TColumn> Consultar<T, TColumn>(this T entidade, Expression<Func<T, Boolean>> filtro, Expression<Func<T, TColumn>> distinct)
			where T : EntidadeDB, new()
		{
			return ComumDB.Consultar<T, TColumn>(entidade, filtro, distinct);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T>(this T entidade, CamlQuery filtro)
			where T : EntidadeSP, new()
		{
			return ComumSP.Consultar<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static List<T> Consultar<T>(this T entidade, SPCamlCondition filtro)
			where T : EntidadeSP, new()
		{
			return ComumSP.Consultar<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static int ConsultarQtd<T>(this T entidade)
			where T : Entidade, new()
		{
			if (typeof(T).IsSubclassOf(typeof(EntidadeSP)) || typeof(T) == typeof(EntidadeSP))
				return ComumSP.ConsultarQtd<T>(entidade, null);
			else
				return ComumDB.ConsultarQtd<T>(entidade, null);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static int ConsultarQtd<T>(this T entidade, Expression<Func<T, Boolean>> filtro)
			where T : EntidadeDB, new()
		{
			return ComumDB.ConsultarQtd<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static int ConsultarQtd<T>(this T entidade, SPCamlCondition filtro)
			where T : EntidadeSP, new()
		{
			return ComumSP.ConsultarQtd<T>(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<ListItem> ConsultarItem(this EntidadeSP entidade)
		{
			return ComumSP.ConsultarItem(entidade);
		}

		/// <summary>Retorna a lista de items de acordo com as chaves</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chaves">Chaves para pesquisa</param>
		/// <returns>Itens</returns>
		public static List<ListItem> ConsultarItem(this EntidadeSP entidade, List<Int32> chaves)
		{
			return ComumSP.ConsultarItem(entidade, chaves);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<ListItem> ConsultarItem(this EntidadeSP entidade, Expression<Func<EntidadeSP, Boolean>> filtro)
		{
			return ComumSP.ConsultarItem(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<ListItem> ConsultarItem(this EntidadeSP entidade, CamlQuery filtro)
		{
			return ComumSP.ConsultarItem(entidade, filtro);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static List<ListItem> ConsultarItem(this EntidadeSP entidade, SPCamlCondition filtro)
		{
			return ComumSP.ConsultarItem(entidade, filtro);
		}


		/// <summary>Retorna a lista de propostas</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<EntidadePropostaSP> ConsultarProposta(Guid codigoLista)
		{
			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();
			try
			{
				propostas = ComumSP.ConsultarPropostas(codigoLista);
			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return propostas;
		}

		/// <summary>Retorna a lista de propostas de acordo com as chaves</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="chaves">Chaves para pesquisa</param>
		/// <returns>Itens</returns>
		public static List<EntidadePropostaSP> ConsultarProposta(Guid codigoLista, List<Int32> chaves)
		{
			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();
			try
			{
				propostas = ComumSP.ConsultarPropostas(codigoLista, chaves);
			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return propostas;
		}

		/// <summary>Retorna a lista de propostas</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<EntidadePropostaSP> ConsultarProposta(Guid codigoLista, Expression<Func<EntidadePropostaSP, Boolean>> filtro)
		{
			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();
			try
			{
				propostas = ComumSP.ConsultarPropostas(codigoLista, filtro);
			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return propostas;
		}

		/// <summary>Retorna a lista de propostas</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<EntidadePropostaSP> ConsultarProposta(Guid codigoLista, CamlQuery filtro)
		{
			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();
			try
			{
				propostas = ComumSP.ConsultarPropostas(codigoLista, filtro);
			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return propostas;
		}

		/// <summary>Retorna a lista de propostas</summary>
		/// <param name="entidade">Proposta a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static List<EntidadePropostaSP> ConsultarProposta(Guid codigoLista, SPCamlCondition filtro)
		{
			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();
			try
			{
				propostas = ComumSP.ConsultarPropostas(codigoLista, filtro);
			}
			catch { } //Não se trata de um item de uma lista de proposta.
			return propostas;
		}

		#endregion

		#region [ Métodos para recarregar da origem os dados do objeto ]
		/// <summary>Recarrega buscando na fonte de dados as informações da entidade</summary>
		/// <typeparam name="T">Entidade</typeparam>
		/// <param name="entidades">Retorna a entidade recarregada</param>
		public static void CarregarDados<T>(this T entidade)
			where T : EntidadeSP, new()
		{
			Reflexao.CopiarObjeto(Obter<T>(entidade, entidade.ID), entidade);
		}

		/// <summary>Recarrega buscando na fonte de dados as informações da entidade</summary>
		/// <typeparam name="T">Entidade</typeparam>
		/// <param name="entidades">Retorna a lista recarregada</param>
		public static void CarregarDados<T>(this List<T> entidades)
			where T : EntidadeSP, new()
		{
			for (int i = 0; i < entidades.Count; i++)
				entidades[i] = Obter<T>(entidades[i], entidades[i].ID);
		}
		#endregion

		#region [List]

		public static Field ObterEstruturaCampo<TModel, TValue>(this TModel item, Expression<Func<TModel, TValue>> expressao)
			where TModel : EntidadeSP
		{
			InternalNameAttribute attributoCampo = (InternalNameAttribute)((MemberExpression)(expressao.Body)).Member.
												GetCustomAttributes(typeof(InternalNameAttribute), false).FirstOrDefault();

			if (attributoCampo == null)
				return null;

			List lista = BaseSP.ObterLista(item);
			var ctx = lista.Context;
			FieldCollection fieldColl = lista.Fields;
			ctx.Load(fieldColl);
			ctx.ExecuteQuery();
			var result = fieldColl.Where(f => f.InternalName == attributoCampo.Name);

			return result.FirstOrDefault();
		}

		/// <summary>
		/// Obtem datasource de campos tipos Choice do sharepoint (checkbox,radiobutton,dropdownlist)
		/// Não utilizar este método para popular dropdownlist de lookup
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="item"></param>
		/// <param name="expressao"></param>
		/// <param name="emptyValue"></param>
		/// <returns></returns>
		public static Dictionary<String, String> ObterDataSourceString<TModel>(this TModel item, Func<TModel, String> expValue,
			Boolean emptyValue = false, char separator = ';')
			where TModel : Entidade
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			string[] valores = expValue(item).Split(separator);

			foreach (String valor in valores)
				datasource.Add(valor, valor);

			return datasource;
		}

		/// <summary>
		/// Obtem datasource de campos tipos Choice do sharepoint (checkbox,radiobutton,dropdownlist)
		/// Não utilizar este método para popular dropdownlist de lookup
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="item"></param>
		/// <param name="expressao"></param>
		/// <param name="emptyValue"></param>
		/// <returns></returns>
		public static Dictionary<String, String> ObterDataSourceChoiceSP<TModel, TValue>(this TModel item, Expression<Func<TModel, TValue>> expressao,
			Boolean emptyValue = false)
			where TModel : EntidadeSP
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			Field field = item.ObterEstruturaCampo(expressao);
			if (field != null && field is Microsoft.SharePoint.Client.FieldChoice)
			{
				String[] choices = ((Microsoft.SharePoint.Client.FieldChoice)(field)).Choices;
				foreach (String choice in choices)
					datasource.Add(choice, choice);
			}
			else if (field != null && field is Microsoft.SharePoint.Client.FieldMultiChoice)
			{
				String[] choices = ((Microsoft.SharePoint.Client.FieldMultiChoice)(field)).Choices;
				foreach (String choice in choices)
					datasource.Add(choice, choice);
			}

			return datasource;
		}

		/// <summary>
		/// Obtem datasource de campos tipos  dropdownlist de lookup do sharepoint
		/// Não utilizar este método para popular Choice do sharepoint
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="item"></param>
		/// <param name="expressao"></param>
		/// <param name="emptyValue"></param>
		/// <returns></returns>
		public static Dictionary<String, String> ObterDataSourceLookup<TModel>(this List<TModel> listItens,
			Boolean emptyValue = false, Func<TModel, String> expDisplayValue = null)
			where TModel : EntidadeSP
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			foreach (TModel item in listItens)
				datasource.Add(item.ID.ToString(), expDisplayValue != null ? expDisplayValue(item) : item.Titulo);

			return datasource;
		}

		/// <summary>
		/// Obtem datasource de campos tipos  dropdownlist de uma entidadeDB
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="listItens"></param>
		/// <param name="expKeyValue"></param>
		/// <param name="expDisplayValue"></param>
		/// <param name="emptyValue"></param>
		/// <returns></returns>
		public static Dictionary<String, String> ObterDataSource<TModel>(this List<TModel> listItens,
			Func<TModel, object> expKeyValue, Func<TModel, String> expDisplayValue, Boolean emptyValue = false)
			where TModel : EntidadeDB
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			foreach (TModel item in listItens)
				datasource.Add(expKeyValue(item).ToString(), expDisplayValue(item));

			return datasource;
		}

		/// <summary>
		/// Obtem datasource de campos tipos  dropdownlist de uma entidadeDB
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="listItens"></param>
		/// <param name="expKeyValue"></param>
		/// <param name="expDisplayValue"></param>
		/// <param name="emptyValue"></param>
		/// <returns></returns>
		public static Dictionary<String, String> ObterDataSource(this List<String> listItens, Boolean emptyValue = false)
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			foreach (String item in listItens)
				datasource.Add(item, item);

			return datasource;
		}

		public static Dictionary<string, string> GetDictionaryFromEnum<T>(Boolean emptyValue = false)
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();
			Type objectType = null;
			Object[] attributes = null;
			String attributeName = "Title";
			String title = string.Empty;
			Type attributeType = typeof(TitleAttribute);

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			foreach (T item in Enum.GetValues(typeof(T)))
			{
				objectType = item.GetType();
				if (item.GetType().BaseType == typeof(Enum))
				{
					attributes = objectType.GetField(item.ToString()).GetCustomAttributes(attributeType, false);

					if (attributes.Length > 0)
					{
						title = attributeType.GetProperty(attributeName).GetValue(attributes[0], null).ToString();
						datasource.Add(Convert.ToInt32(item).ToString(), title.ToString());
					}
				}
			}

			return datasource;
		}

		public static String GetTitleFromEnum<T>(Int32 value)
		{
			String title = string.Empty;
			Type objectType = null;
			Object[] attributes = null;
			String attributeName = "Title";
			Type attributeType = typeof(TitleAttribute);

			foreach (T item in Enum.GetValues(typeof(T)))
			{
				if (Convert.ToInt32(item) == value)
				{
					objectType = item.GetType();
					if (item.GetType().BaseType == typeof(Enum))
					{
						attributes = objectType.GetField(item.ToString()).GetCustomAttributes(attributeType, false);
						if (attributes.Length > 0)
							return attributeType.GetProperty(attributeName).GetValue(attributes[0], null).ToString();
					}
				}
			}

			return title;
		}

		public static bool ContainsField(this List list, string fieldName)
		{
			var ctx = list.Context;
			var result = ctx.LoadQuery(list.Fields.Where(f => f.InternalName == fieldName));
			ctx.ExecuteQuery();
			return result.Any();
		}

		public static String ObterNomeLista(this EntidadeSP item)
		{
			return ComumSP.ObterNomeLista(item);
		}

		#endregion

		#region [Anexo]

		/// <summary>
		/// Acessa o record center configurado
		/// Adiciona/Acessa uma lista com o mesmo nome da lista do item
		/// Adiciona/Acessa uma pasta com o mesmo nome do id do item
		/// Adiciona documentos nesta pasta
		/// </summary>
		/// <param name="item"></param>
		/// <param name="newFile"></param>
		public static void UploadAnexo(this EntidadeSP item, FileCreationInformation newFile)
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			String nomeLista = ComumSP.ObterNomeLista(item);

			UploadAnexo(nomeLista, item.ID.ToString(), newFile);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Adiciona/Acessa uma lista com o mesmo nome da lista encontrada
		/// Adiciona/Acessa uma pasta com o mesmo nome do login do usuário
		/// Adiciona/Acessa uma pasta com o mesmo nome da pasta Temporária
		/// Adiciona documento temporário nesta pasta
		/// </summary>
		/// <param name="guidLista"></param>
		/// <param name="guidPastaTemporaria"></param>
		/// <param name="newFile"></param>
		public static void UploadAnexo(Guid guidLista, FileCreationInformation newFile, String pastaTemporaria = "Temporario")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(guidLista);
			String nomeLista = listaOrigem.Title;
			String login = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
			login = login.Contains("\\") ? login.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[1] : login;
			String nomePasta = String.Format("{0}/{1}", login, pastaTemporaria);

			UploadAnexo(nomeLista, nomePasta, newFile);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Adiciona/Acessa uma lista com o mesmo nome da lista encontrada
		/// Adiciona/Acessa uma pasta com nome igual ao id do item
		/// Adiciona documento nesta pasta
		/// Caso o idUsuario não seja informado, utiliza o usuário atual como responsável pelo documento
		/// </summary>
		/// <param name="guidLista"></param>
		/// <param name="idItem"></param>
		/// <param name="newFile"></param>
		/// <param name="idUsuario"></param>
		public static void UploadAnexo(Guid guidLista, Int32 idItem, FileCreationInformation newFile, String emailUsuario = "")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(guidLista);
			String nomeLista = listaOrigem.Title;

			UploadAnexo(nomeLista, idItem.ToString(), newFile, emailUsuario);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Adiciona/Acessa uma lista com o mesmo nome da lista encontrada
		/// Adiciona/Acessa uma pasta com nome igual a nomePasta
		/// Adiciona documento nesta pasta 
		/// Caso o idUsuario não seja informado, utiliza o usuário atual como responsável pelo documento
		/// </summary>
		/// <param name="nomeLista"></param>
		/// <param name="nomePasta"></param>
		/// <param name="newFile"></param>
		/// <param name="idUsuario"></param>
		/// <param name="removerArquivos">Remove arquivos antigos no diretório escolhido</param>
		public static void UploadAnexo(String nomeLista, String nomePasta, FileCreationInformation newFile, String emailUsuario = "", Boolean removerArquivos = false)
		{
			String loginUsuario = String.Empty;
			Usuario usuarioAtual = null;
			if (emailUsuario != String.Empty)
			{
				usuarioAtual = PortalWeb.ContextoWebAtual.BuscarUsuarioPorEmail(emailUsuario);
				loginUsuario = usuarioAtual != null ? usuarioAtual.Login : String.Empty;
			}

			using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
			{
				PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
				{
					Int32 idUsuario = 0;

					if (loginUsuario != String.Empty)
						usuarioAtual = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(loginUsuario, true);//Busca o usuario (Ensure User)

					idUsuario = usuarioAtual != null ? usuarioAtual.Id : PortalWeb.ContextoWebAtual.UsuarioAtual.Id;

					FieldUserValue userValue = new FieldUserValue()
					{
						LookupId = idUsuario
					};

					List documentLibrary = BaseSP.ObterLista(nomeLista);
					Folder folder = ComumSP.CreateFolder(documentLibrary, nomePasta);

					if (removerArquivos)
					{
						FileCollection arquivos = folder.Files;

						PortalWeb.ContextoWebAtual.SPClient.Load(arquivos);
						PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

						for (int i = arquivos.Count - 1; i >= 0; i--)
						{
							arquivos[i].DeleteObject();
						}

						PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
					}

					File novoAnexo = ComumSP.CreateFile(documentLibrary, folder, newFile);
					novoAnexo.CheckOut();
					ListItem itemAnexo = novoAnexo.ListItemAllFields;
					PortalWeb.ContextoWebAtual.SPClient.Load(novoAnexo);
					PortalWeb.ContextoWebAtual.SPClient.Load(itemAnexo);
					PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
					itemAnexo["Usuario"] = userValue;
					itemAnexo.Update();
					novoAnexo.CheckIn("Upload de documento - Usuario Atualizado", CheckinType.OverwriteCheckIn);
					PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
				});
			}
		}

		public static void MoverAnexos(Guid guidLista, Int32 idItem, String pastaTemporaria = "Temporario")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(guidLista);
			String nomeLista = listaOrigem.Title;

			String login = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
			login = login.Contains("\\") ? login.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[1] : login;
			String urlPastaTemporaria = String.Format("{0}/{1}", login, pastaTemporaria);

			using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
			{
				PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
				{
					List documentLibrary = BaseSP.ObterLista(nomeLista);
					Folder toFolder = ComumSP.CreateFolder(documentLibrary, idItem.ToString());
					Folder fromFolder = ComumSP.CreateFolder(documentLibrary, urlPastaTemporaria);
					ComumSP.MoveFilesTo(fromFolder, toFolder);
				});
			}
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o mesmo nome da lista do item
		/// Acessa uma pasta com o mesmo nome do id do item
		/// retorna todos os documentos desta pasta
		/// </summary>
		/// <param name="item"></param>
		/// <returns>Documentos relacionados ao item</returns>
		public static List<Anexo> ObterAnexos(this EntidadeSP item)
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(item);
			String nomeLista = listaOrigem.Title;

			return ObterAnexos(listaOrigem.Id, item.ID, nomeLista);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o mesmo nome da lista do item
		/// Acessa uma pasta com o mesmo nome do id do item
		/// retorna todos os documentos desta pasta
		/// </summary>
		/// <param name="item"></param>
		/// <returns>Documentos relacionados ao item</returns>
		public static List<Anexo> ObterAnexos(Guid codigoLista, Int32 codigoItem, String nomeLista = "")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			if (nomeLista == String.Empty)
			{
				List listaOrigem = ComumSP.ObterLista(codigoLista);
				nomeLista = listaOrigem.Title;
			}

			return ObterAnexos(nomeLista, codigoItem.ToString());
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o mesmo nome da lista encontrada
		/// Acessa uma pasta com o mesmo nome do login do usuário
		/// Acessa uma pasta com o mesmo nome da pasta Temporária
		/// retorna todos os documentos desta pasta
		/// </summary>
		/// <param name="guidLista"></param>
		/// <param name="guidPastaTemporaria"></param>
		/// <returns></returns>
		/// </summary>
		public static List<Anexo> ObterAnexos(Guid codigoLista, String pastaTemporaria = "Temporario")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(codigoLista);
			String nomeLista = listaOrigem.Title;
			String login = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
			login = login.Contains("\\") ? login.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[1] : login;
			String folderName = String.Format("{0}/{1}", login, pastaTemporaria);

			return ObterAnexos(nomeLista, folderName);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o nome igual à nomeLista
		/// Acessa uma pasta com o nome igual à nomePasta
		/// </summary>
		/// <param name="nomeLista"></param>
		/// <param name="nomePasta"></param>
		/// <returns></returns>
		public static List<Anexo> ObterAnexos(String nomeLista, String nomePasta)
		{
			Folder folder = null;
			List<Anexo> documentos = new List<Anexo>();

			using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
			{
				PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
				{
					List documentLibrary = BaseSP.ObterLista(nomeLista);
					ComumSP.TryGetFolder(documentLibrary, nomePasta, out folder);
					if (folder != null)
					{
						PortalWeb.ContextoWebAtual.SPClient.Load(folder.Files, files => files.Include(
								  f => f.Author
								, f => f.Name
								, f => f.TimeLastModified
								, f => f.TimeCreated
								, f => f.ServerRelativeUrl
								, f => f.ListItemAllFields.Id
								, f => f.ListItemAllFields["EncodedAbsUrl"]
								, f => f.ListItemAllFields["Usuario"]));
						PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
						foreach (File file in folder.Files)
							documentos.Add(new Anexo(file));
					}
				});
			}

			return documentos;
		}

		/// <summary>
		/// retorna o link para abrir a propsota do anexo fácil.
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static List<KeyValuePair<string, string>> ObterLinksAnexoFacil(Guid codigoLista, Int32 codigoItem, String userName)
		{
			List<KeyValuePair<string, string>> links = new List<KeyValuePair<string, string>>();

			#region [Código Verificação]
			String chave = TipoWebServices.AnexoFacilUrlProposa.GetTitle();
			ListaSP_RaizenConfiguracoes parametroAnexoFacilUrl = new ListaSP_RaizenConfiguracoes().Obter(p => p.Chave == chave);
			String chaveAnexoFacil = TipoWebServices.AnexoFacilChave.GetTitle();
			ListaSP_RaizenConfiguracoes parametroChaveAnexoFacil = new ListaSP_RaizenConfiguracoes().Obter(p => p.Chave == chaveAnexoFacil);

			string codigoIntegracao = string.Format("{0}{1}{2}"
					, userName.StripDomain().ToLower()
					, new Guid(parametroChaveAnexoFacil.Valor)
					, DateTime.Today.ToString(new CultureInfo("en-US")));

			System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
			System.Text.StringBuilder hash = new System.Text.StringBuilder();
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(codigoIntegracao), 0, Encoding.UTF8.GetByteCount(codigoIntegracao));
			foreach (byte theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}
			#endregion
			List<AnexoFacil> anexosFacil = new AnexoFacil().Consultar(_ => _.CodigoLista == codigoLista && _.CodigoItem == codigoItem);

			if (anexosFacil != null)
				foreach (var anexo in anexosFacil)
					links.Add(new KeyValuePair<string, string>(
						  anexo.AnexoFacilIntegracao == null || String.IsNullOrEmpty(anexo.AnexoFacilIntegracao.DescricaoLink) ? "www.anexofacil.raizen.com" : anexo.AnexoFacilIntegracao.DescricaoLink
						, parametroAnexoFacilUrl != null && anexosFacil != null ? String.Format(parametroAnexoFacilUrl.Valor
							, anexo.CodigoAnexoFacil
							, hash.ToString())
							: String.Empty));

			return links;
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o nome igual à nomeLista
		/// Deleta o item com id = idAnexo
		/// </summary>
		/// <param name="item"></param>
		/// <param name="idAnexo"></param>
		public static void DeletarAnexo(this EntidadeSP item, Int32 idAnexo)
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(item);
			String nomeLista = listaOrigem.Title;

			DeletarAnexo(listaOrigem.Id, idAnexo, nomeLista);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o nome igual à lista de id = codigoLista
		/// Deleta o item com id = idAnexo 
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="idAnexo"></param>
		public static void DeletarAnexo(Guid codigoLista, Int32 idAnexo)
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			List listaOrigem = ComumSP.ObterLista(codigoLista);
			String nomeLista = listaOrigem.Title;

			DeletarAnexo(codigoLista, idAnexo, nomeLista);
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Acessa uma lista com o nome igual à lista de id = codigoLista
		/// Deleta o item com id = idAnexo  
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="idAnexo"></param>
		/// <param name="nomeLista"></param>
		public static void DeletarAnexo(Guid codigoLista, Int32 idAnexo, String nomeLista = "")
		{
			if (String.IsNullOrEmpty(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				throw new Exception("Document Center não está configurado.");

			if (nomeLista == String.Empty)
			{
				List listaOrigem = ComumSP.ObterLista(codigoLista);
				nomeLista = listaOrigem.Title;
			}

			using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
			{
				PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
				{
					List documentLibrary = BaseSP.ObterLista(nomeLista);
					ListItem anexo = documentLibrary.GetItemById(idAnexo);
					PortalWeb.ContextoWebAtual.SPClient.Load(anexo);
					PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
					anexo.DeleteObject();
					PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
				});
			}
		}

		/// <summary>
		/// Acessa o record center configurado
		/// Retorna o arquivo com o fileRef
		/// </summary>
		/// <param name="fileRef"></param>
		/// <returns></returns>
		public static FileInformation DownloadAnexo(String fileRef)
		{
			FileInformation fileInformation = null;
			using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
			{
				PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
				{
					fileInformation = File.OpenBinaryDirect(PortalWeb.ContextoWebAtual.SPClient, fileRef);
				});
			}
			return fileInformation;
		}

		[Obsolete("Método utilizado apenas para Aditivos Gerais (migração). Utilizar")]
		public static void EnviarEmailAttachmentAditivos(List listaAditivo, Int32 codigoItem, String emailUsuario)
		{
			ListItem currentItem = listaAditivo.GetItemById(codigoItem);
			PortalWeb.ContextoWebAtual.SPClient.Load(currentItem);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			EmailAttachment emailAttachment = new EmailAttachment();
			emailAttachment.TipoEmail = (Int32)TipoEmail.Enviado;
			emailAttachment.Processado = false;
			emailAttachment.Ambiente2013 = true;
			emailAttachment.IdItem = codigoItem;
			emailAttachment.CodigoLista = listaAditivo.Id;
			emailAttachment.EmailUsuario = emailUsuario;
			emailAttachment.NomeProposta = currentItem["Title"].ToString();

			List<DadosComercial> estruturaComercialListas = Serializacao.CarregarEstruturaComercial();
			DadosComercial dadosComercial = NegocioProposta.ObterDadosComercial(listaAditivo, currentItem, estruturaComercialListas);

			emailAttachment.RazaoSocial = dadosComercial.RazaoSocial;
			emailAttachment.Ibm = dadosComercial.Ibm;
			emailAttachment.GerenteTerritorio = dadosComercial.GerenteTerritorio;
			emailAttachment.GerenteRegiao = dadosComercial.GerenteRegiao;
			emailAttachment.DiretorVendas = dadosComercial.DiretorVendas;
			emailAttachment.Cdr = dadosComercial.Cdr;
			emailAttachment.Gdr = dadosComercial.Gdr;

			emailAttachment.Inserir();
		}

		public static void EnviarEmailAttachment(Guid codigoLista, Int32 codigoItem, String emailUsuario)
		{
			EntidadePropostaSP proposta = NegocioComum.ObterProposta(codigoLista, codigoItem);

			EmailAttachment emailAttachment = new EmailAttachment();
			emailAttachment.TipoEmail = (Int32)TipoEmail.Enviado;
			emailAttachment.Processado = false;
			emailAttachment.Ambiente2013 = true;
			emailAttachment.IdItem = codigoItem;
			emailAttachment.CodigoLista = codigoLista;
			emailAttachment.EmailUsuario = emailUsuario;


			if (proposta != null)
			{
				emailAttachment.NomeProposta = proposta.Titulo;
				emailAttachment.RazaoSocial = proposta.RazaoSocial;
				emailAttachment.Ibm = proposta.Ibm != null ? proposta.Ibm.ToString() : "-";
				emailAttachment.GerenteTerritorio = proposta.GerenteTerritorio != null ? proposta.GerenteTerritorio.Nome : "-";
				emailAttachment.GerenteRegiao = proposta.GerenteRegiao != null ? proposta.GerenteRegiao.Nome : "-";
				emailAttachment.DiretorVendas = proposta.DiretorVendas != null ? proposta.DiretorVendas.Nome : "-";
				emailAttachment.Cdr = proposta.Cdr != null ? proposta.Cdr.Nome : "-";
				emailAttachment.Gdr = proposta.Gdr != null ? proposta.Gdr.Nome : "-";
			}
			else
			{
				ListItem item = ComumSP.ObterItem(codigoLista, codigoItem);
				emailAttachment.NomeProposta = item["Title"].ToString();
			}

			emailAttachment.Inserir();
		}

		#endregion

		#region [CustomForm]

		public static String ObterUrlFormUserControl(TipoUserControl tipoUserControl, Guid guidLista = new Guid(), String contentTypeId = "")
		{
			String nomeLista = String.Empty;
			String nomeUserControl = tipoUserControl.GetTitle();
			if (guidLista != Guid.Empty)
			{
				List listaConfiguracao = BaseSP.ObterLista(guidLista);
				if (listaConfiguracao == null)
					throw new Exception("Lista de confuração - Configuração CustomForm - não encontrada.");
				nomeLista = listaConfiguracao.Title;
			}

			ListaSP_RaizenUserControl configuracao = null;
			if (contentTypeId != "")
				configuracao = new ListaSP_RaizenUserControl().Obter(i => i.Titulo == contentTypeId);

			if (configuracao == null)
				configuracao = new ListaSP_RaizenUserControl().Consultar(item => item.TipoUserControl == nomeUserControl).Where(
					item => (item.Titulo == nomeLista || item.Titulo == null)).OrderByDescending(item => item.Titulo).FirstOrDefault();
			return configuracao != null ? configuracao.UrlUserControl : String.Empty;
		}

		public static Boolean ExibirAnexoFacil(Int32 codigoItem, Guid guidLista = new Guid())
		{
			String chaveConfiguracaoLista = String.Empty;
			Boolean exibirControle = false;
			EntidadePropostaSP proposta = null;
			if (guidLista != Guid.Empty)
			{
				List listaConfiguracao = BaseSP.ObterLista(guidLista);
				if (listaConfiguracao == null)
					throw new Exception("Lista de confuração - Configuração CustomForm - não encontrada.");
				chaveConfiguracaoLista = String.Concat(listaConfiguracao.Title, " - Anexo Fácil");
				proposta = codigoItem > 0 ? NegocioComum.ObterProposta(guidLista, codigoItem) : null;
			}

			ListaSP_RaizenConfiguracoes configuracao = new ListaSP_RaizenConfiguracoes().Consultar(item => item.Chave == chaveConfiguracaoLista).FirstOrDefault();

			if (configuracao == null)
				exibirControle = true;
			else
			{
				DateTime dataExibirAnexoFacil = DateTime.MinValue;
				if (DateTime.TryParse(configuracao.Valor, out dataExibirAnexoFacil)
					&& ((proposta == null && dataExibirAnexoFacil <= DateTime.Now)
						|| (proposta != null && dataExibirAnexoFacil <= proposta.DataInclusao)
						))
					exibirControle = true;
				else
					exibirControle = false;
			}
			return exibirControle;
		}
		#endregion		

		#region [Permissao]

		/// <summary>
		/// Verifica se o usuário que possui o email tem permissão para editar proposta
		/// </summary>
		/// <param name="email"></param>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static Boolean UsuarioPossuiPermissaoEditar(String email, Guid codigoLista, Int32 codigoItem)
		{
			#region [Obtem usuário ]

			User usuario = PortalWeb.ContextoWebAtual.SPWeb.SiteUsers.GetByEmail(email);
			PortalWeb.ContextoWebAtual.SPClient.Load(usuario);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			#endregion

			return UsuarioPossuiPermissaoEditar(usuario, codigoLista, codigoItem);
		}

		/// <summary>
		/// Verifica se o usuário atual tem permissão para editar proposta
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static Boolean UsuarioAtualPossuiPermissaoEditar(Guid codigoLista, Int32 codigoItem)
		{
			#region [Obtem usuário atual ]

			User usuarioAtual = PortalWeb.ContextoWebAtual.SPWeb.CurrentUser;
			PortalWeb.ContextoWebAtual.SPClient.Load(usuarioAtual);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			#endregion

			return UsuarioPossuiPermissaoEditar(usuarioAtual, codigoLista, codigoItem);
		}


		/// <summary>
		///  Verifica se o usuário possui permissão para editar proposta
		/// </summary>
		/// <param name="usuario"></param>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static Boolean UsuarioPossuiPermissaoEditar(User usuario, Guid codigoLista, Int32 codigoItem)
		{
			Boolean permissao = false;
			Boolean estruturaComercial = false;
			List<RoleDefinition> rolesUsuarioLista = ObterUsuarioRoles(usuario, codigoLista);

			#region [Verifica permissão]

			estruturaComercial = rolesUsuarioLista.Any(r =>
								r.Name == PortalRoles.RaizenEstruturaComercial.GetTitle());

			if (estruturaComercial)//Caso for da estrutura comercial, verifica se no item ele pertence à estrutura
			{
				String usuarioLogin = usuario.LoginName.RemoverClaims();

				EntidadePropostaSP _proposta = NegocioComum.ObterProposta(codigoLista, codigoItem);
				if (_proposta != null)
					permissao = (_proposta.Gdr != null && _proposta.Gdr.Login == usuarioLogin) ||
						   (_proposta.Cdr != null && _proposta.Cdr.Login == usuarioLogin) ||
						   (_proposta.DiretorVendas != null && _proposta.DiretorVendas.Login == usuarioLogin) ||
						   (_proposta.GerenteTerritorio != null && _proposta.GerenteTerritorio.Login == usuarioLogin) ||
						   (_proposta.GerenteRegiao != null && _proposta.GerenteRegiao.Login == usuarioLogin);
				else
					permissao = rolesUsuarioLista.Any(r =>
							r.Name == PortalRoles.RaizenColaborador.GetTitle() ||
							r.RoleTypeKind == RoleType.Administrator ||
							r.RoleTypeKind == RoleType.Contributor ||
							r.RoleTypeKind == RoleType.Editor);
			}
			else//Caso contrário, verifica apenas se o mesmo possui permissão de editar
			{
				permissao = rolesUsuarioLista.Any(r =>
							r.Name == PortalRoles.RaizenColaborador.GetTitle() ||
							r.RoleTypeKind == RoleType.Administrator ||
							r.RoleTypeKind == RoleType.Contributor ||
							r.RoleTypeKind == RoleType.Editor);
			}

			#endregion

			return permissao;
		}

		/// <summary>
		/// Verifica se o usuário possui a role
		/// </summary>
		/// <param name="usuario"></param>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="rolePortal"></param>
		/// <returns></returns>
		public static Boolean UsuarioPossuiRole(User usuario, Guid codigoLista, Int32 codigoItem, PortalRoles rolePortal)
		{
			Boolean permissao = false;
			List<RoleDefinition> rolesUsuarioLista = ObterUsuarioRoles(usuario, codigoLista);

			#region [Verifica permissão]

			permissao = rolesUsuarioLista.Any(r =>
								r.Name == rolePortal.GetTitle());

			#endregion

			return permissao;
		}

		/// <summary>
		///  Verifica se o usuário possui permissão para editar proposta
		/// </summary>
		/// <param name="usuario"></param>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static Boolean UsuarioPossuiPermissaoExcluirAnexo(User usuario, Guid codigoLista, Int32 codigoItem)
		{
			Boolean permissao = false;
			List<RoleDefinition> rolesUsuarioLista = ObterUsuarioRoles(usuario, codigoLista);

			#region [Verifica permissão]

			permissao = rolesUsuarioLista.Any(r =>
							r.Name == PortalRoles.ExcluirAnexo.GetTitle());

			#endregion

			return permissao;
		}

		/// <summary>
		/// Obtem todas as roles que o usuário possui na lista
		/// </summary>
		/// <param name="usuario"></param>
		/// <param name="codigoLista"></param>
		/// <returns></returns>
		public static List<RoleDefinition> ObterUsuarioRoles(User usuario, Guid codigoLista)
		{
			List<RoleDefinition> rolesUsuarioLista = new List<RoleDefinition>();

			#region [Obtem os grupos do usuário]

			GroupCollection gruposUsuario = usuario.Groups;
			PortalWeb.ContextoWebAtual.SPClient.Load(gruposUsuario);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			// TODO: Verificar se as próximas 6 linhas fazem sentido existir, pois, durante o merging
			// com as classes Core do RNIP, foi detectado que as mesmas não existiam naquele projeto.
			var usuarioAuthenticated = PortalWeb.ContextoWebAtual.SPWeb.EnsureUser("NT AUTHORITY\\authenticated users");
			PortalWeb.ContextoWebAtual.SPClient.Load(usuarioAuthenticated);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			GroupCollection gruposUsuarioAuthenticated = usuarioAuthenticated.Groups;
			PortalWeb.ContextoWebAtual.SPClient.Load(gruposUsuarioAuthenticated);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			#endregion

			#region [Obtem gupos da lista]

			List lista = ComumSP.ObterLista(codigoLista);
			PortalWeb.ContextoWebAtual.SPClient.Load(lista, l => l.RoleAssignments);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			#endregion

			#region [Obtem as roles que o usuário possui na lista]

			foreach (RoleAssignment grupo in lista.RoleAssignments)
			{
				PortalWeb.ContextoWebAtual.SPClient.Load(grupo, g => g.RoleDefinitionBindings, g => g.PrincipalId);
				PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
				if (gruposUsuario.Any(g => g.Id == grupo.PrincipalId))
					foreach (var role in grupo.RoleDefinitionBindings)
						rolesUsuarioLista.Add(role);
				// TODO: Verificar se as próximas 3 linhas fazem sentido existir, pois, durante o merging
				// com as classes Core do RNIP, foi detectado que as mesmas não existiam naquele projeto.
				if (gruposUsuarioAuthenticated.Any(g => g.Id == grupo.PrincipalId))
					foreach (var role in grupo.RoleDefinitionBindings)
						rolesUsuarioLista.Add(role);
			}

			#endregion

			return rolesUsuarioLista;
		}

		#endregion

		#region [Grupo]

		public static String ObterEmailUsuarios(this Grupo grupo)
		{
			String usuarioEmails = String.Empty;

			if (grupo != null)
			{
				foreach (Usuario usuario in grupo.Usuarios)
					usuarioEmails += usuario.Email + ";";
			}

			return usuarioEmails;
		}

		#endregion

		#region [ContentType]

		public static String ObterContentTypeId(this ListItem item)
		{
			PortalWeb.ContextoWebAtual.SPClient.Load(item.ContentType);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			return item.ContentType != null ? item.ContentType.StringId : String.Empty;
		}

		public static String ObterContentTypeId(String nomeContentType)
		{
			ContentTypeCollection contentTypeColl = PortalWeb.ContextoWebAtual.SPClient.Web.ContentTypes;
			PortalWeb.ContextoWebAtual.SPClient.Load(contentTypeColl);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			ContentType ct = contentTypeColl.Where(c => c.Name == nomeContentType).FirstOrDefault();
			return ct == null ? "" : ct.StringId;
		}

		#endregion

	}
}
