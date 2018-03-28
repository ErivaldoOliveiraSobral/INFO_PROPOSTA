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

namespace PortalDeFluxos.Core.BLL.Negocio
{
	public static class NegocioAnoContratual
	{
		public static Dictionary<Int32, Decimal?> ObterValores<TModel>(this List<TModel> listItens
			, Func<TModel, Decimal?> expDisplayValue, Boolean? casoBase = null)
			where TModel : AnoContratual
		{
			Dictionary<Int32, Decimal?> datasource = new Dictionary<Int32, Decimal?>();
			foreach (TModel item in listItens.Where(_ => _.CasoBase == casoBase).OrderBy(_ => _.Ano))
				datasource.Add(item.Ano, (Decimal?)expDisplayValue(item));

			return datasource;
		}

		public static void PopularValores<TModel>(this List<TModel> listItens, Expression<Func<TModel, object>> expression
			, Dictionary<Int32, Decimal?> valoresControle, int maxAno, Boolean? casoBase = null
			, Int32 idProposta = -1, TipoPropostaPai tipoPropostaPai = TipoPropostaPai.Rnip)
			where TModel : AnoContratual, new()
		{
			String propertyName = Reflexao.GetPropertyName((LambdaExpression)expression);
			Boolean possuiValores = !(valoresControle == null || valoresControle.Count == 0);

			if (String.IsNullOrEmpty(propertyName))
				return;

			#region [Sincroniza Quantidade de anos]


			Int32 maxAnoCadastrado = listItens.Where(_ => _.CasoBase == casoBase).OrderBy(_ => _.Ano).ToList().Count > 0 ?
				listItens.Where(_ => _.CasoBase == casoBase).OrderBy(_ => _.Ano).LastOrDefault().Ano : 0;

			Int32 diferenca = maxAno - maxAnoCadastrado;

			for (int i = 1; i <= diferenca; i++)
			{
				TModel novoValor;
				novoValor = new TModel()
				{
					IdProposta = idProposta > 0 ? idProposta : listItens.FirstOrDefault() != null ? listItens.FirstOrDefault().IdProposta : -1,
					IdTipoProposta = (Int32)tipoPropostaPai,
					Ano = maxAno - diferenca + i,
					CasoBase = casoBase,
				};
				//novoValor.Inserir();
				listItens.Add(novoValor);
			}

			Boolean controleAno0 = possuiValores ? valoresControle.Any(_ => _.Key == 0) : false;
			if (!controleAno0)//so elimina nos controles que não possuem ano 0 (sempre vai ter menos anos)
			{
				while (listItens.Any(_ => _.CasoBase == casoBase && _.Ano > maxAno && _.Ativo == true))
				{
					//listItens.Where(_ => _.CasoBase == casoBase && _.Ano > maxAno).OrderBy(_ => _.Ano).LastOrDefault().Excluir();
					listItens.Where(_ => _.CasoBase == casoBase && _.Ano > maxAno && _.Ativo == true).OrderBy(_ => _.Ano).LastOrDefault().Ativo = false;
					//listItens.Remove(listItens.Where(_ => _.CasoBase == casoBase && _.Ano > maxAno).OrderBy(_ => _.Ano).LastOrDefault());
				}
			}
			else if (!listItens.Any(_ => _.CasoBase == casoBase && _.Ano == 0))
			{
				TModel novoValor;
				novoValor = new TModel()
				{
					IdProposta = idProposta > 0 ? idProposta : listItens.FirstOrDefault() != null ? listItens.FirstOrDefault().IdProposta : -1,
					IdTipoProposta = (Int32)tipoPropostaPai,
					Ano = 0,
					CasoBase = casoBase,
				};
				//novoValor.Inserir();
				listItens.Add(novoValor);
			}


			#endregion

			if (listItens.Count > 0 && possuiValores)
			{
				Int32 _indexItem = 0;
				foreach (var valor in valoresControle)
				{
					if (listItens.Exists(_ => _.CasoBase == casoBase && _.Ano == valor.Key))
					{
						listItens.Where(_ => _.CasoBase == casoBase && _.Ano == valor.Key).FirstOrDefault().GetType().GetProperty(propertyName)
							.SetValue(listItens.Where(_ => _.CasoBase == casoBase && _.Ano == valor.Key).FirstOrDefault(), valor.Value, null);
						_indexItem++;
					}
				}
			}
		}

	}
}
