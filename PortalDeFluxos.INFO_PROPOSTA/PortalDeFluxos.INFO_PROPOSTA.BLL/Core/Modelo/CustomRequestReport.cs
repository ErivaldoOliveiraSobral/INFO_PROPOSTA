using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Iteris;
using System.Web;
using Microsoft.SharePoint;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class CustomPainel
    {
        public Int32 TotalRecordCount { get; set; }

        private Collection<Object> _entries = new Collection<Object>();
        public Collection<Object> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        #region [Solicitacao]
        public static DataControlField GetGridViewColumnDefinition(SolicitacaoExecucaoReportGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitacao:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitacao);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitacao.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink() { ID = name + controlMetaId });
                    break;
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeGerenteRegiao:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeGerenteRegiao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeGerenteRegiao.GetTitle();
                    break;
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeDiretorVendas:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeDiretorVendas);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeDiretorVendas.GetTitle();
                    break;
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitante:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitante);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitante.GetTitle();
                    break;
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeEtapa:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeEtapa);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeEtapa.GetTitle();
                    break;
                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.StatusFluxo:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.StatusFluxo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = SolicitacaoExecucaoReportGridViewFieldDefinitionType.StatusFluxo.GetTitle();
                    break;

                case SolicitacaoExecucaoReportGridViewFieldDefinitionType.DescricaoUrlDetalhe:
                    name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.DescricaoUrlDetalhe);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridViewField centralized noClick";
                    field.HeaderStyle.CssClass = "gridViewHeader2 noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    field.SortExpression = "";
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink()
                    {
                        ID = name + controlMetaId,
                        ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/PortalDeFluxos.Core.SP/IMPITEM.GIF"),
                    });
                    field.HeaderText = "Detalhes";
                    break;
            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<SolicitacaoExecucaoReportGridViewFieldDefinitionType> _fieldDefinitions,
            Solicitacao solicitacao,
            string frame2007
        )
        {
            String controlMetaId = null;
            foreach (SolicitacaoExecucaoReportGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {

                    case SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitacao:
                        name = Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.NomeSolicitacao);
                        HyperLink link = ((HyperLink)row.FindControl(name + controlMetaId));
                        link.NavigateUrl = RedirectSharePoint2013(solicitacao.DescricaoUrlItem, frame2007);
                        link.Text = solicitacao.NomeSolicitacao;

                        break;
                    case SolicitacaoExecucaoReportGridViewFieldDefinitionType.DescricaoUrlDetalhe:
                        ((HyperLink)row.FindControl(Enum.GetName(typeof(SolicitacaoExecucaoReportGridViewFieldDefinitionType), SolicitacaoExecucaoReportGridViewFieldDefinitionType.DescricaoUrlDetalhe) + controlMetaId))
                            .NavigateUrl =
                            RedirectSharePoint2013(String.Format("{0}?CodigoLista={1}&CodigoItem={2}", solicitacao.DescricaoUrlDetalhes, solicitacao.CodigoLista, solicitacao.CodigoItem), frame2007);

                        break;
                }
            }
        }
        #endregion [Fim - Solicitacao]

        #region [Minhas Tarefas]
        public static DataControlField GetGridViewColumnDefinition(MinhasTarefasPendenteReportGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitacao:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitacao);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitacao.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink() { ID = name + controlMetaId });
                    break;
                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeGerenteRegiao:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeGerenteRegiao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeGerenteRegiao.GetTitle();
                    break;
                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeDiretorVendas:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeDiretorVendas);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeDiretorVendas.GetTitle();
                    break;
                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitante:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitante);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewFieldUserName";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitante.GetTitle();
                    break;
                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.TempoDecorrido:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.TempoDecorrido);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.TempoDecorrido.GetTitle();
                    break;

                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeTarefa:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeTarefa);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridViewField";
                    field.HeaderStyle.CssClass = "gridViewHeader2";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderText = MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeTarefa.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink() { ID = name + controlMetaId });
                    break;

                case MinhasTarefasPendenteReportGridViewFieldDefinitionType.DescricaoUrlDetalhe:
                    name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.DescricaoUrlDetalhe);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridViewField centralized noClick";
                    field.HeaderStyle.CssClass = "gridViewHeader2 noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    field.SortExpression = "";
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink()
                     {
                         ID = name + controlMetaId,
                         ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/PortalDeFluxos.Core.SP/IMPITEM.GIF"),
                     });
                    field.HeaderText = "Detalhes";
                    break;
            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<MinhasTarefasPendenteReportGridViewFieldDefinitionType> _fieldDefinitions,
            MinhasTarefasPendente minhasTarefasPendente,
            string frame2007
        )
        {
            String controlMetaId = null;
            foreach (MinhasTarefasPendenteReportGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                HyperLink link = null;
                switch (fieldDefinition)
                {

                    case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitacao:
                        name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeSolicitacao);
                        link = ((HyperLink)row.FindControl(name + controlMetaId));
                        link.NavigateUrl = RedirectSharePoint2013(String.Format("{0}&Ambiente2007={1}", minhasTarefasPendente.DescricaoUrlItem, minhasTarefasPendente.Ambiente2007), frame2007);
                        link.Text = minhasTarefasPendente.NomeSolicitacao;
                        break;
                    case MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeTarefa:
                        name = Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.NomeTarefa);
                        link = ((HyperLink)row.FindControl(name + controlMetaId));
                        link.NavigateUrl = RedirectSharePoint2013(String.Format("{0}&Ambiente2007={1}", minhasTarefasPendente.DescricaoUrlTarefa, minhasTarefasPendente.Ambiente2007), frame2007);
                        link.Text = minhasTarefasPendente.NomeTarefa;
                        break;
                    case MinhasTarefasPendenteReportGridViewFieldDefinitionType.DescricaoUrlDetalhe:
                        ((HyperLink)row.FindControl(Enum.GetName(typeof(MinhasTarefasPendenteReportGridViewFieldDefinitionType), MinhasTarefasPendenteReportGridViewFieldDefinitionType.DescricaoUrlDetalhe) + controlMetaId)).NavigateUrl =
                            RedirectSharePoint2013(String.Format("{0}?CodigoLista={1}&CodigoItem={2}&Ambiente2007={3}", minhasTarefasPendente.DescricaoUrlDetalhes, minhasTarefasPendente.CodigoLista, minhasTarefasPendente.CodigoItem, minhasTarefasPendente.Ambiente2007), frame2007);
                        break;
                }
            }
        }
        #endregion [Minhas Tarefas]

        #region [Tarefas Pendentes]
        public static DataControlField GetGridViewColumnDefinition(TarefasPendentesReportGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case TarefasPendentesReportGridViewFieldDefinitionType.NomeTarefa:
                    name = Enum.GetName(typeof(TarefasPendentesReportGridViewFieldDefinitionType), TarefasPendentesReportGridViewFieldDefinitionType.NomeTarefa);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderText = TarefasPendentesReportGridViewFieldDefinitionType.NomeTarefa.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new HyperLink() { ID = name + controlMetaId });
                    field.HeaderStyle.Width = Unit.Percentage(37);
                    break;

                case TarefasPendentesReportGridViewFieldDefinitionType.NomeResponsavel:
                    name = Enum.GetName(typeof(TarefasPendentesReportGridViewFieldDefinitionType), TarefasPendentesReportGridViewFieldDefinitionType.NomeResponsavel);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasPendentesReportGridViewFieldDefinitionType.NomeResponsavel.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(37);
                    break;

                case TarefasPendentesReportGridViewFieldDefinitionType.DataInclusao:
                    name = Enum.GetName(typeof(TarefasPendentesReportGridViewFieldDefinitionType), TarefasPendentesReportGridViewFieldDefinitionType.DataInclusao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasPendentesReportGridViewFieldDefinitionType.DataInclusao.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(13);
                    break;

                case TarefasPendentesReportGridViewFieldDefinitionType.DataSLA:
                    name = Enum.GetName(typeof(TarefasPendentesReportGridViewFieldDefinitionType), TarefasPendentesReportGridViewFieldDefinitionType.DataSLA);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasPendentesReportGridViewFieldDefinitionType.DataSLA.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(13);
                    break;

            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<TarefasPendentesReportGridViewFieldDefinitionType> _fieldDefinitions,
            TarefasPendentes tarefasPendente,
            string frame2007,
            Boolean modal = false
        )
        {
            String controlMetaId = null;
            foreach (TarefasPendentesReportGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                HyperLink link = null;
                switch (fieldDefinition)
                {
                    case TarefasPendentesReportGridViewFieldDefinitionType.NomeTarefa:
                        name = Enum.GetName(typeof(TarefasPendentesReportGridViewFieldDefinitionType), TarefasPendentesReportGridViewFieldDefinitionType.NomeTarefa);

                        if (!modal)
                        {
                            link = ((HyperLink)row.FindControl(name + controlMetaId));
                            link.NavigateUrl = RedirectSharePoint2013(tarefasPendente.DescricaoUrlTarefa, frame2007);
                            if (tarefasPendente.DescricaoUrlTarefa == "#")
                            {
                                link.Enabled = false;
                                link.Style.Add("color", "#444");
                                link.ToolTip = "Não foi possível localizar a tarefa. Acesse diretamente no ambiente 2007.";
                            }
                            link.Text = tarefasPendente.NomeTarefa;
                        }
                        else
                        {
                            HyperLink linkModalTarefa = ((HyperLink)row.FindControl(name + controlMetaId));
                            linkModalTarefa.Text = tarefasPendente.NomeTarefa;
                            String clickEvent = String.Format("aprovacaoDelegacaoTarefa.events.carregarAprovacao({0});", tarefasPendente.IdTarefa);
                            linkModalTarefa.Style.Add("cursor", "pointer");
                            linkModalTarefa.Attributes.Add("onClick", clickEvent);
                        }

                        break;
                }
            }
        }
        #endregion [Tarefas Pendentes]

        #region [Minhas Realizadas]
        public static DataControlField GetGridViewColumnDefinition(TarefasRealizadasReportGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case TarefasRealizadasReportGridViewFieldDefinitionType.NomeTarefa:
                    name = Enum.GetName(typeof(TarefasRealizadasReportGridViewFieldDefinitionType), TarefasRealizadasReportGridViewFieldDefinitionType.NomeTarefa);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasRealizadasReportGridViewFieldDefinitionType.NomeTarefa.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    break;

                case TarefasRealizadasReportGridViewFieldDefinitionType.NomeCompletadoPor:
                    name = Enum.GetName(typeof(TarefasRealizadasReportGridViewFieldDefinitionType), TarefasRealizadasReportGridViewFieldDefinitionType.NomeCompletadoPor);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasRealizadasReportGridViewFieldDefinitionType.NomeCompletadoPor.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    break;

                case TarefasRealizadasReportGridViewFieldDefinitionType.DescricaoAcaoEfetuada:
                    name = Enum.GetName(typeof(TarefasRealizadasReportGridViewFieldDefinitionType), TarefasRealizadasReportGridViewFieldDefinitionType.DescricaoAcaoEfetuada);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasRealizadasReportGridViewFieldDefinitionType.DescricaoAcaoEfetuada.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    break;

                case TarefasRealizadasReportGridViewFieldDefinitionType.ComentarioAprovacao:
                    name = Enum.GetName(typeof(TarefasRealizadasReportGridViewFieldDefinitionType), TarefasRealizadasReportGridViewFieldDefinitionType.ComentarioAprovacao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasRealizadasReportGridViewFieldDefinitionType.ComentarioAprovacao.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(42);
                    break;

                case TarefasRealizadasReportGridViewFieldDefinitionType.DataFinalizado:
                    name = Enum.GetName(typeof(TarefasRealizadasReportGridViewFieldDefinitionType), TarefasRealizadasReportGridViewFieldDefinitionType.DataFinalizado);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = TarefasRealizadasReportGridViewFieldDefinitionType.DataFinalizado.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(13);
                    break;

            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<TarefasRealizadasReportGridViewFieldDefinitionType> _fieldDefinitions,
            TarefasRealizadas tarefasRealizadas
        )
        {
        }
        #endregion [Minhas Realizadas]

        #region [Log]

        public static DataControlField GetGridViewColumnDefinition(LogReportGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case LogReportGridViewFieldDefinitionType.IdLog:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.IdLog);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.IdLog.GetTitle();
                    break;

                case LogReportGridViewFieldDefinitionType.NomeProcesso:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.NomeProcesso);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.NomeProcesso.GetTitle();
                    break;

                case LogReportGridViewFieldDefinitionType.DescricaoOrigem:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.DescricaoOrigem);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.DescricaoOrigem.GetTitle();
                    break;

                case LogReportGridViewFieldDefinitionType.DescricaoMensagem:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.DescricaoMensagem);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(35);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.DescricaoMensagem.GetTitle();
                    break;
                case LogReportGridViewFieldDefinitionType.LoginInclusao:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.LoginInclusao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.LoginInclusao.GetTitle();
                    break;
                case LogReportGridViewFieldDefinitionType.DataInclusao:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.DataInclusao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.DataInclusao.GetTitle();
                    break;
                case LogReportGridViewFieldDefinitionType.Erro:
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.Erro);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = LogReportGridViewFieldDefinitionType.Erro.GetTitle();
                    break;
                case LogReportGridViewFieldDefinitionType.DescricaoDetalhe:
                    field = new TemplateField();
                    name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.DescricaoDetalhe);
                    field.ItemStyle.CssClass = "gridview-column-row centralized noClick";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right align-center noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    field.SortExpression = "";
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = LogReportGridViewFieldDefinitionType.DescricaoDetalhe.ToString(),
                    });
                    field.HeaderText = LogReportGridViewFieldDefinitionType.DescricaoDetalhe.GetTitle();
                    break;

            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<LogReportGridViewFieldDefinitionType> _fieldDefinitions,
            Log log)
        {
            String controlMetaId = null;
            foreach (LogReportGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {

                    case LogReportGridViewFieldDefinitionType.DescricaoDetalhe:
                        name = Enum.GetName(typeof(LogReportGridViewFieldDefinitionType), LogReportGridViewFieldDefinitionType.DescricaoDetalhe);
                        LinkButton linkDetalhe = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkDetalhe.Controls.Add(new Image
                        {
                            ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/PortalDeFluxos.Core.SP/IMPITEM.GIF")
                        });
                        linkDetalhe.Style.Add("cursor", "pointer");
                        linkDetalhe.OnClientClick = "$('[id*=descricaoDetalheModal]').html(\"" + log.DescricaoDetalhe.ToJScriptString() + "\");$('#myModal').modal('show');return false;";
                        break;
                }
            }
        }

        #endregion [Fim - Log]

        #region [ Redirect 2007 / 2013 ]
        private static string RedirectSharePoint2013(string redirectUri, string frame2007)
        {
            if (!string.IsNullOrEmpty(frame2007))
            {
                string urlDetalheSP2013 = redirectUri.ToString().IndexOf("/") == 0 ? SPContext.Current.Web.Url + redirectUri : redirectUri;
                if (urlDetalheSP2013.Contains("DetalhesSolicitacao"))
                    urlDetalheSP2013 = frame2007 + "/Paginas/DetalhesSolicitacao.aspx?urlDetalhesSolicitacao=" + urlDetalheSP2013 + "&isdlg=1";
                else
                    urlDetalheSP2013 = frame2007 + "/Paginas/DetalhesSolicitacao.aspx?urlFinal=" + urlDetalheSP2013;

                redirectUri = urlDetalheSP2013.Replace("Id=", "idItemSP=");
            }
            redirectUri = redirectUri.Contains("PaginaAprovacao") ? redirectUri.Replace("Id=", "idItemSP=") : redirectUri;
            return redirectUri;
        }
        #endregion

        #region [Anexo]
        public static DataControlField GetGridViewColumnDefinition(DocumentosGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case DocumentosGridViewFieldDefinitionType.Usuario:
                    name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.Usuario);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = DocumentosGridViewFieldDefinitionType.Usuario.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    break;

                case DocumentosGridViewFieldDefinitionType.NomeArquivo:
                    name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.NomeArquivo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderText = DocumentosGridViewFieldDefinitionType.NomeArquivo.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(30);
                    break;

				case DocumentosGridViewFieldDefinitionType.DataUploadString:
					name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.DataUploadString);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
					field.HeaderText = DocumentosGridViewFieldDefinitionType.DataUploadString.GetTitle();
                    field.HeaderStyle.Width = Unit.Percentage(8);
                    field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    break;

                case DocumentosGridViewFieldDefinitionType.Download:
                    name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.Download);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized noClick";
                    field.HeaderStyle.CssClass = "gridview-column-header noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    field.SortExpression = "";
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = DocumentosGridViewFieldDefinitionType.Download.ToString(),
                    });
                    field.HeaderText = DocumentosGridViewFieldDefinitionType.Download.GetTitle();
                    break;

                case DocumentosGridViewFieldDefinitionType.Excluir:
                    field = new TemplateField();
                    name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.Excluir);
                    field.ItemStyle.CssClass = "gridview-column-row centralized noClick";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right align-center noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    field.SortExpression = "";
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = DocumentosGridViewFieldDefinitionType.Excluir.ToString(),
                    });
                    field.HeaderText = DocumentosGridViewFieldDefinitionType.Excluir.GetTitle();
                    break;
            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<DocumentosGridViewFieldDefinitionType> _fieldDefinitions,
            Anexo documento,
            Boolean permissaoExluir = false
        )
        {
            String controlMetaId = null;
            foreach (DocumentosGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {

                    case DocumentosGridViewFieldDefinitionType.Download:
                        name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.Download);
                        LinkButton linkDownload = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkDownload.Controls.Add(new Image
                        {
                            ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/mewa_downloadb.GIF")
                        });
                        linkDownload.CommandArgument = documento.RelativeUrl;
                        break;
                    case DocumentosGridViewFieldDefinitionType.Excluir:
                        if (permissaoExluir)
                        {
                            name = Enum.GetName(typeof(DocumentosGridViewFieldDefinitionType), DocumentosGridViewFieldDefinitionType.Excluir);
                            LinkButton linkExcluir = ((LinkButton)row.FindControl(name + controlMetaId));
                            linkExcluir.Controls.Add(new Image
                            {
                                ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/mewa_backToMainb.GIF")
                            });
                            linkExcluir.OnClientClick = "return confirm('Esta ação excluirá definitivamente o documento! Deseja continuar?')";
                            linkExcluir.CommandArgument = documento.IdAnexo.ToString();
                        }
                        break;
                }
            }
        }

        #endregion [Minhas Realizadas]

        #region [Serviços]

        public static DataControlField GetGridViewColumnDefinition(ServicosGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case ServicosGridViewFieldDefinitionType.IdServicoAgendado:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.IdServicoAgendado);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.IdServicoAgendado.GetTitle();
                    break;

                case ServicosGridViewFieldDefinitionType.NomeAssemblyType:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.NomeAssemblyType);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.NomeAssemblyType.GetTitle();
                    break;

                case ServicosGridViewFieldDefinitionType.DescricaoAgenda:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.DescricaoAgenda);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.DescricaoAgenda.GetTitle();
                    break;

                case ServicosGridViewFieldDefinitionType.DataProximaExecucao:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.DataProximaExecucao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.DataProximaExecucao.GetTitle();
                    break;
                case ServicosGridViewFieldDefinitionType.DataUltimaExecucao:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.DataUltimaExecucao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.DataUltimaExecucao.GetTitle();
                    break;
                case ServicosGridViewFieldDefinitionType.LoginAlteracao:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.LoginAlteracao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = ServicosGridViewFieldDefinitionType.LoginAlteracao.GetTitle();
                    break;
                case ServicosGridViewFieldDefinitionType.Logar:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Logar);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = ServicosGridViewFieldDefinitionType.Logar.ToString(),
                    });
                    field.HeaderText = ServicosGridViewFieldDefinitionType.Logar.GetTitle();
                    break;
                case ServicosGridViewFieldDefinitionType.Ativo:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Ativo);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = ServicosGridViewFieldDefinitionType.Ativo.ToString(),
                    });
                    field.HeaderText = ServicosGridViewFieldDefinitionType.Ativo.GetTitle();
                    break;
                case ServicosGridViewFieldDefinitionType.Executar:
                    name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Executar);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized noClick";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right align-center noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.SortExpression = "";
                    field.HeaderText = ServicosGridViewFieldDefinitionType.Executar.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = ServicosGridViewFieldDefinitionType.Executar.ToString(),
                    });
                    field.HeaderText = ServicosGridViewFieldDefinitionType.Executar.GetTitle();
                    break;

            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<ServicosGridViewFieldDefinitionType> _fieldDefinitions,
            ServicoAgendado servico)
        {
            String controlMetaId = null;
            foreach (ServicosGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {
                    case ServicosGridViewFieldDefinitionType.Executar:
                        name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Executar);
                        LinkButton linkExecutar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkExecutar.Controls.Add(new Image
                        {
                            ImageUrl = String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/PLPLAY1.GIF")
                        });
                        linkExecutar.OnClientClick = "return confirm('Esta ação executará este serviço! Deseja continuar?')";
                        linkExecutar.CommandArgument = servico.IdServicoAgendado.ToString();
                        break;
                    case ServicosGridViewFieldDefinitionType.Ativo:
                        name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Ativo);
                        LinkButton linkAtivar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkAtivar.Controls.Add(new Image
                        {
                            ImageUrl = servico.Ativo ? 
                                String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbChecked.gif") :
                                String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbUnChecked.gif")
                        });
                        linkAtivar.OnClientClick = servico.Ativo ? 
                            "return confirm('Esta ação desativará este serviço! Deseja continuar?')" :
                            "return confirm('Esta ação ativará este serviço! Deseja continuar?')";

                        linkAtivar.CommandArgument = servico.IdServicoAgendado.ToString();
                        break;
                    case ServicosGridViewFieldDefinitionType.Logar:
                        name = Enum.GetName(typeof(ServicosGridViewFieldDefinitionType), ServicosGridViewFieldDefinitionType.Logar);
                        LinkButton linkLogar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkLogar.Controls.Add(new Image
                        {
                            ImageUrl = servico.Logar != null && (Boolean)servico.Logar ?
                                String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbChecked.gif") :
                                String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbUnChecked.gif")
                        });
                        linkLogar.OnClientClick = servico.Logar != null && (Boolean)servico.Logar ?
                            "return confirm('Esta ação desabilitará o log deste serviço! Deseja continuar?')" :
                            "return confirm('Esta ação habilitará o log deste serviço! Deseja continuar?')";
                        linkLogar.CommandArgument = servico.IdServicoAgendado.ToString();
                        break;
                }
            }
        }

        #endregion [Fim - Serviços]

        #region [Fluxos]

        public static DataControlField GetGridViewColumnDefinition(FluxosGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case FluxosGridViewFieldDefinitionType.IdInstanciaFluxo:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.IdInstanciaFluxo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.IdInstanciaFluxo.GetTitle();
                    break;

                case FluxosGridViewFieldDefinitionType.CodigoItem:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.CodigoItem);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.CodigoItem.GetTitle();
                    break;

                case FluxosGridViewFieldDefinitionType.NomeLista:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.NomeLista);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.NomeLista.GetTitle();
                    break;

                case FluxosGridViewFieldDefinitionType.NomeSolicitacao:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.NomeSolicitacao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(20);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.NomeSolicitacao.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.NomeEtapa:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.NomeEtapa);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.NomeEtapa.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.DataAlteracao:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.DataAlteracao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.DataAlteracao.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.DataRestartWorkflow:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.DataRestartWorkflow);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.DataRestartWorkflow.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.NomeStatusFluxo:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.NomeStatusFluxo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.NomeStatusFluxo.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.NumeroTentativaInicio:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.NumeroTentativaInicio);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.NumeroTentativaInicio.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.Ativo:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.Ativo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = FluxosGridViewFieldDefinitionType.Ativo.GetTitle();
                    break;
                case FluxosGridViewFieldDefinitionType.Iniciar:
                    name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.Iniciar);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized noClick";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right align-center noClick";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.SortExpression = "";
                    field.HeaderText = FluxosGridViewFieldDefinitionType.Iniciar.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = FluxosGridViewFieldDefinitionType.Iniciar.ToString(),
                    });
                    field.HeaderText = FluxosGridViewFieldDefinitionType.Iniciar.GetTitle();
                    break;

            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<FluxosGridViewFieldDefinitionType> _fieldDefinitions,
            InstanciaFluxo instanciaFluxo)
        {
            String controlMetaId = null;
            foreach (FluxosGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {
                    case FluxosGridViewFieldDefinitionType.Iniciar:
                        name = Enum.GetName(typeof(FluxosGridViewFieldDefinitionType), FluxosGridViewFieldDefinitionType.Iniciar);
                        LinkButton linkExecutar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkExecutar.Controls.Add(new Image
                        {
                            ImageUrl = instanciaFluxo.StatusFluxo == (Int32)StatusFluxo.Erro ?
                                String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/PLPLAY1.GIF") :
                                ""
                        });
                        linkExecutar.OnClientClick = "return confirm('Esta ação iniciará este fluxo! Deseja continuar?')";
                        linkExecutar.CommandArgument = instanciaFluxo.IdInstanciaFluxo.ToString();
                        break;
                }
            }
        }

        #endregion [Fim - Fluxos]

        #region [UsuariosGrupos]

        public static DataControlField GetGridViewColumnDefinition(UsuarioGruposGridViewFieldDefinitionType columnDefinitionType)
        {
            DataControlField field = null;
            String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
            String name = String.Empty;

            switch (columnDefinitionType)
            {
                case UsuarioGruposGridViewFieldDefinitionType.Id:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.Id);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.Id.GetTitle();
                    break;

                case UsuarioGruposGridViewFieldDefinitionType.Nome:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.Nome);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(25);
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.Nome.GetTitle();
                    break;

                case UsuarioGruposGridViewFieldDefinitionType.Login:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.Login);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.Login.GetTitle();
                    break;

                case UsuarioGruposGridViewFieldDefinitionType.Tipo:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.Tipo);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(15);
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.Tipo.GetTitle();
                    break;

                case UsuarioGruposGridViewFieldDefinitionType.AtivoDescricao:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.AtivoDescricao);
                    field = new BoundField();
                    field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
                    field.HeaderStyle.CssClass = "gridview-column-header";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    ((BoundField)field).DataField = name;
                    field.SortExpression = name;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.AtivoDescricao.GetTitle();
                    break;
                case UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized";
                    field.HeaderStyle.CssClass = "gridview-column-header align-center";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.HeaderStyle.Width = Unit.Percentage(10);
                    field.SortExpression = name;
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios.ToString(),
                    });
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios.GetTitle();
                    break;
                case UsuarioGruposGridViewFieldDefinitionType.QtdTarefa:
                    name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.QtdTarefa);
                    field = new TemplateField();
                    field.ItemStyle.CssClass = "gridview-column-row centralized";
                    field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-right align-center";
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    field.HeaderStyle.Width = Unit.Percentage(5);
                    field.SortExpression = name;
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.QtdTarefa.GetTitle();
                    ((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
                    {
                        ID = name + controlMetaId,
                        CommandName = UsuarioGruposGridViewFieldDefinitionType.QtdTarefa.ToString(),
                    });
                    field.HeaderText = UsuarioGruposGridViewFieldDefinitionType.QtdTarefa.GetTitle();
                    break;
            }

            return field;
        }

        public static void BindValuesToCells(
            GridViewRow row,
            List<UsuarioGruposGridViewFieldDefinitionType> _fieldDefinitions,
            UsuariosGruposSP usuarioGruposSp)
        {
            String controlMetaId = null;
            foreach (UsuarioGruposGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
            {
                Int32 idColumn = ((Int32)fieldDefinition);
                controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
                String name = String.Empty;
                switch (fieldDefinition)
                {
                    case UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios:
                        name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.QtdUsuarios);
                        LinkButton linkVisualizar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkVisualizar.Text = usuarioGruposSp.QtdUsuarios;
                        if (usuarioGruposSp.Login == "-")
                        {
                            linkVisualizar.Style.Add("cursor", "pointer");
                            linkVisualizar.CommandArgument = usuarioGruposSp.Id.ToString();
                        }
                        else
                        {
                            linkVisualizar.Style.Add("cursor", "default");
                            linkVisualizar.OnClientClick = "return false";
                        }

                        break;
                    case UsuarioGruposGridViewFieldDefinitionType.QtdTarefa:
                        name = Enum.GetName(typeof(UsuarioGruposGridViewFieldDefinitionType), UsuarioGruposGridViewFieldDefinitionType.QtdTarefa);
                        LinkButton linkEditar = ((LinkButton)row.FindControl(name + controlMetaId));
                        linkEditar.Text = usuarioGruposSp.QtdTarefa.ToString();
                        if (usuarioGruposSp.QtdTarefa == 0)
                        {
                            linkEditar.Style.Add("cursor", "default");
                            linkEditar.OnClientClick = "return false";
                        }else
                            linkEditar.Style.Add("cursor", "pointer");
                        
                        linkEditar.CommandArgument = usuarioGruposSp.Login == "-" ? usuarioGruposSp.Id.ToString() : usuarioGruposSp.Login;
                        break;
                }
            }
        }

        #endregion [Fim - Grupo]

		#region [Delegação Programada]

		public static DataControlField GetGridViewColumnDefinition(DelegacaoProgramadaGridViewFieldDefinitionType columnDefinitionType)
		{
			DataControlField field = null;
			String controlMetaId = ((Int32)columnDefinitionType).ToString(CultureInfo.InvariantCulture);
			String name = String.Empty;

			switch (columnDefinitionType)
			{
				case DelegacaoProgramadaGridViewFieldDefinitionType.LoginDe:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.LoginDe);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row";
					field.HeaderStyle.CssClass = "gridview-column-header gridview-column-header-border-left";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(10);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.LoginDe.GetTitle();
					break;

				case DelegacaoProgramadaGridViewFieldDefinitionType.NomeDe:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.NomeDe);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(25);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.NomeDe.GetTitle();
					break;

				case DelegacaoProgramadaGridViewFieldDefinitionType.LoginPara:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.LoginPara);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(10);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.LoginPara.GetTitle();
					break;

				case DelegacaoProgramadaGridViewFieldDefinitionType.NomePara:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.NomePara);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(25);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.NomePara.GetTitle();
					break;
				case DelegacaoProgramadaGridViewFieldDefinitionType.DataInicio:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.DataInicio);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row gridViewFieldComments";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(10);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.DataInicio.GetTitle();
					break;
				case DelegacaoProgramadaGridViewFieldDefinitionType.DataFim:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.DataFim);
					field = new BoundField();
					field.ItemStyle.CssClass = "gridview-column-row";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					((BoundField)field).DataField = name;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(10);
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.DataFim.GetTitle();
					break;
				case DelegacaoProgramadaGridViewFieldDefinitionType.Ativo:
					name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.Ativo);
					field = new TemplateField();
					field.ItemStyle.CssClass = "gridview-column-row";
					field.HeaderStyle.CssClass = "gridview-column-header";
					field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
					field.SortExpression = name;
					field.HeaderStyle.Width = Unit.Percentage(5);
					((TemplateField)field).ItemTemplate = new TemplateProxy(() => new LinkButton()
					{
						ID = name + controlMetaId,
						CommandName = DelegacaoProgramadaGridViewFieldDefinitionType.Ativo.ToString(),
					});
					field.HeaderText = DelegacaoProgramadaGridViewFieldDefinitionType.Ativo.GetTitle();
					break;
			}

			return field;
		}

		public static void BindValuesToCells(
			GridViewRow row,
			List<DelegacaoProgramadaGridViewFieldDefinitionType> _fieldDefinitions,
			Delegacao delegacao,
            Boolean permissaoDesativar)
		{
			String controlMetaId = null;
			foreach (DelegacaoProgramadaGridViewFieldDefinitionType fieldDefinition in _fieldDefinitions)
			{
				Int32 idColumn = ((Int32)fieldDefinition);
				controlMetaId = idColumn.ToString(CultureInfo.InvariantCulture);
				String name = String.Empty;
				switch (fieldDefinition)
				{
					case DelegacaoProgramadaGridViewFieldDefinitionType.Ativo:
						name = Enum.GetName(typeof(DelegacaoProgramadaGridViewFieldDefinitionType), DelegacaoProgramadaGridViewFieldDefinitionType.Ativo);
						LinkButton linkAtivar = ((LinkButton)row.FindControl(name + controlMetaId));
						linkAtivar.Controls.Add(new Image
						{
							ImageUrl = delegacao.Ativo ?
								String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbChecked.gif") :
								String.Concat(Microsoft.SharePoint.SPContext.Current.Web.Url.AppendIfMissing("/"), "_layouts/15/images/CbUnChecked.gif")
						});

                        if (permissaoDesativar)
                            linkAtivar.OnClientClick = delegacao.Ativo ?
                                "return confirm('Esta ação desativará esta delegação! Deseja continuar?')" :
                                "return confirm('Esta ação ativará esta delegação! Deseja continuar?')";
                        else
                            linkAtivar.OnClientClick = "alert('Você não tem permissão para realizar esta operação!')";

						linkAtivar.CommandArgument = delegacao.IdDelegacao.ToString();
                        linkAtivar.Enabled = permissaoDesativar;
						break;
				}
			}
		}

		#endregion [Fim - Delegação Programada]
    }
}
