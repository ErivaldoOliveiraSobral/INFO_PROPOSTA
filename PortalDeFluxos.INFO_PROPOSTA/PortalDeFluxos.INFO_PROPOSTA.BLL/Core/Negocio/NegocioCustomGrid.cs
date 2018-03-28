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
    public static class NegocioCustomGrid
    {
        /// <summary>
        /// Configura as rows para serem utilizadas pelo controle ucCustomGrid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="itens">Serão transformados em linhas</param>
        /// <param name="configuracao">Configuração Base</param>
        /// <param name="columnsConf">Configuração das colunas</param>
        public static void CarregarRows<TModel, TValue1>(this List<TModel> itens
            , ref GridConfigurationCore configuracao
            , List<ColumnCore> columnsConf
            , Expression<Func<TModel, TValue1>> idSelector)
            where TModel : EntidadeDB, new()
        {
            #region [Rows]

            configuracao.Rows = new List<RowCore>();

            if (itens != null && itens.Count > 0)
            {
                foreach (var item in itens)
                {
                    RowCore row = new RowCore();
                    row.Columns = new List<ColumnCore>();
                    row.Deleted = Reflexao.BuscarValorPropriedade(item, "Ativo") != null ?
                        !(Boolean)Reflexao.BuscarValorPropriedade(item, "Ativo") : true;
                    row.Index = configuracao.Rows.Count;

                    foreach (var confCol in columnsConf.OrderBy(_ => _.Index))
                    {
                        if (item.GetType().GetProperties().Any(_ => _.Name == confCol.IdColumn))
                        {
                            ColumnCore col = new ColumnCore(confCol);
                            if (item.GetPropertyName(idSelector) == confCol.IdColumn)
                            {
                                col.IsId = true;
                                row.IdRow = (Int32)item.GetType().GetProperties().First(_ => _.Name == confCol.IdColumn).GetValue(item);
                            }
                            else
                            {
                                Object value = Reflexao.BuscarValorPropriedade(item, confCol.IdColumn);
                                col.Value = value != null ? value.ToString() : String.Empty;
                            }

                            if (!col.IsId)
                                row.Columns.Add(col);
                        }
                    }

                    configuracao.Rows.Add(row);
                }

            }
            else//Row default
            {
                RowCore row = new RowCore();
                row.Columns = new List<ColumnCore>();

                foreach (ColumnCore col in columnsConf)
                    if (!col.IsId)
                        row.Columns.Add(col);

                configuracao.Rows.Add(row);
            }

            #endregion
        }

        /// <summary>
        /// Configura as rows para serem utilizadas pelo controle ucCustomGrid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="itens">Serão transformados em linhas</param>
        /// <param name="configuracao">Configuração Base</param>
        /// <param name="columnsConf">Configuração das colunas</param>
        public static void CarregarSumRow<TModel, TValue1>(this TModel item
            , ref GridConfigurationCore configuracao
            , List<ColumnCore> columnsConf
            , Expression<Func<TModel, TValue1>> idSelector)
            where TModel : EntidadeDB, new()
        {
            #region [Row]

            configuracao.SumRow = configuracao.SumEnabled ? new RowCore() : null;
            if (configuracao.SumRow == null)
                return;
            configuracao.SumRow.Columns = new List<ColumnCore>();

            if (item != null)
            {
                foreach (var confCol in columnsConf.OrderBy(_ => _.Index))
                {
                    if (item.GetType().GetProperties().Any(_ => _.Name == confCol.IdColumn))
                    {
                        ColumnCore col = new ColumnCore(confCol);
                        if (item.GetPropertyName(idSelector) == confCol.IdColumn)
                        {
                            col.IsId = true;
                            configuracao.SumRow.IdRow = (Int32)item.GetType().GetProperties().First(_ => _.Name == confCol.IdColumn).GetValue(item);
                        }
                        else
                        {
                            Object value = Reflexao.BuscarValorPropriedade(item, confCol.IdColumn);
                            col.Value = value != null ? value.ToString() : String.Empty;
                        }

                        if (!col.IsId)
                            configuracao.SumRow.Columns.Add(col);
                    }
                }
            }
            else//Row default
            {
                foreach (ColumnCore col in columnsConf)
                    if (!col.IsId)
                        configuracao.SumRow.Columns.Add(col);
            }

            #endregion
        }

        /// <summary>
        /// Carrega os datasources utilizados pelo controle ucCustomGrid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="configuracao">Configuração Base</param>
        /// <param name="datasources">Datasource que estará disponível no controle - 
        /// key do datasource deve ser o mesmo key IdDataSource utilizado na coluna  do tipo select
        /// </param>
        public static void CarregarDataSource(ref GridConfigurationCore configuracao
           , Dictionary<String, List<KeyValuePair<String, String>>> datasources)
        {

            #region [DataSource]
            if (datasources != null)
            {
                configuracao.DataSources = new List<DataSourceCore>();
                foreach (var item in datasources)
                    configuracao.DataSources.Add(new DataSourceCore(item.Key, item.Value));
            }
            #endregion

        }

		public static void CarregarEntidade<TModel, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(this List<TModel> itens, GridConfigurationCore configuracao, Expression<Func<TModel, TValue1>> idSelector
			, Expression<Func<TModel, TValue2>> ativoSelector, Expression<Func<TModel, TValue3>> idPropostaSelector, int idProposta
			, Expression<Func<TModel, TValue4>> idTipoSelector, Expression<Func<TModel, TValue5>> casoBaseSelector
			, Expression<Func<TModel, TValue6>> sumSelector, Int32 idTipo = -1, Boolean? casoBase = null, Boolean? deletar = null)
			where TModel : EntidadeDB, new()
		{

			#region [Rows]
			foreach (var rowAtual in configuracao.Rows)
			{
				if (rowAtual.IdRow <= 0 && (rowAtual.Deleted || (deletar != null ? (bool)deletar : rowAtual.Deleted)))
					continue;

				TModel entidade = rowAtual.IdRow > 0 ? new TModel().Obter(rowAtual.IdRow) : new TModel();

				Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(ativoSelector), !rowAtual.Deleted && (deletar != null ? !(bool)deletar : !rowAtual.Deleted));
				SetCommonProperties<TModel, TValue3, TValue4, TValue5>(idPropostaSelector, idProposta, idTipoSelector, casoBaseSelector, idTipo, casoBase, entidade);
				PopularEntidade<TModel>(rowAtual.Columns, entidade);

				if (rowAtual.IdRow > 0)
					Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(idSelector), rowAtual.IdRow);

				itens.Add(entidade);
			}
			#endregion

			#region [Sum Row]

			if (sumSelector != null && configuracao.SumRow != null && configuracao.SumEnabled)
			{
				if (configuracao.SumRow.IdRow <= 0 && deletar != null ? (bool)deletar : false)
					return;

				TModel entidade = configuracao.SumRow.IdRow > 0 ? new TModel().Obter(configuracao.SumRow.IdRow) : new TModel();

				Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(ativoSelector), deletar != null ? !(bool)deletar : true);
				Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(sumSelector), true);

				SetCommonProperties<TModel, TValue3, TValue4, TValue5>(idPropostaSelector, idProposta, idTipoSelector, casoBaseSelector, idTipo, casoBase, entidade);
				PopularEntidade<TModel>(configuracao.SumRow.Columns, entidade);

				if (configuracao.SumRow.IdRow > 0)
					Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(idSelector), configuracao.SumRow.IdRow);

				itens.Add(entidade);
			}

			#endregion
		} 

        public static Dictionary<Int32, Decimal?> ObterValoresCustomGrid<TModel>(this List<TModel> listItens
            , Func<TModel, Decimal?> expDisplayValue, Int32 anoIncial = 1, Func<TModel, Int32> expKeyValue = null)
            where TModel : EntidadeDB
        {
            Dictionary<Int32, Decimal?> datasource = new Dictionary<Int32, Decimal?>();
            for (int i = anoIncial; i <= listItens.Count; i++)
                datasource.Add(expKeyValue != null ? expKeyValue(listItens[i - 1]) : i, (Decimal?)expDisplayValue(listItens[i - 1]));

            return datasource;
        }

        public static Dictionary<Int32, Int32?> ObterValoresCustomGrid<TModel>(this List<TModel> listItens
            , Func<TModel, Int32?> expDisplayValue, Int32 anoIncial = 1, Func<TModel, Int32> expKeyValue = null)
            where TModel : EntidadeDB
        {
            Dictionary<Int32, Int32?> datasource = new Dictionary<Int32, Int32?>();
            for (int i = anoIncial; i <= listItens.Count; i++)
                datasource.Add(expKeyValue != null ? expKeyValue(listItens[i - 1]) : i, (Int32?)expDisplayValue(listItens[i - 1]));

            return datasource;
        }

        public static Dictionary<Int32, String> ObterValoresCustomGrid<TModel>(this List<TModel> listItens
            , Func<TModel, Int32?> expDisplayValue, Dictionary<String, String> datasourceOrigem, Int32 anoIncial = 1, Func<TModel, Int32> expKeyValue = null)
            where TModel : EntidadeDB
        {
            Dictionary<Int32, String> datasource = new Dictionary<Int32, String>();
            for (int i = anoIncial; i <= listItens.Count; i++)
                datasource.Add(expKeyValue != null ? expKeyValue(listItens[i - 1]) : i,
                    datasourceOrigem.ContainsKey(expDisplayValue(listItens[i - 1]).ToString()) ? datasourceOrigem[expDisplayValue(listItens[i - 1]).ToString()] : String.Empty);

            return datasource;
        }


        #region [Helpers]

        private static void PopularEntidade<TModel>(List<ColumnCore> columns, TModel entidade) where TModel : EntidadeDB, new()
        {
            foreach (var columnAtual in columns)
            {
                if (columnAtual.IsId)
                    continue;

                if (String.IsNullOrEmpty(columnAtual.Value) || String.IsNullOrEmpty(columnAtual.Value.Trim())
                        || (columnAtual.Type == "select" && columnAtual.Value.Trim() == "-1"))
                    Reflexao.DefinePropriedade(entidade, columnAtual.IdColumn, null);
                else if (columnAtual.CustomClass.Contains("numero"))
                    Reflexao.DefinePropriedade(entidade, columnAtual.IdColumn, columnAtual.Value.ToDecimal().ToString());
                else
                    Reflexao.DefinePropriedade(entidade, columnAtual.IdColumn, columnAtual.Value);

            }
        }

        private static void SetCommonProperties<TModel, TValue3, TValue4, TValue5>(Expression<Func<TModel, TValue3>> idPropostaSelector, int idProposta, Expression<Func<TModel, TValue4>> idTipoSelector, Expression<Func<TModel, TValue5>> casoBaseSelector, Int32 idTipo, Boolean? casoBase, TModel entidade) where TModel : EntidadeDB, new()
        {
            if (idTipoSelector != null && idTipo > 0)
                Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(idTipoSelector), idTipo);
            if (casoBaseSelector != null)
                Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(casoBaseSelector), casoBase);
            Reflexao.DefinePropriedade(entidade, entidade.GetPropertyName(idPropostaSelector), idProposta);
        }

        #endregion

    }
}
