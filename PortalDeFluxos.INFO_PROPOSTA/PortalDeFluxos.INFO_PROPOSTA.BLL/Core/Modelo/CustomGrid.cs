using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class ColumnCore
    {
        public Int32 Index { get; set; }

        public String IdColumn { get; set; }

        public String Type { get; set; }

        public String Value { get; set; }

        public String IdDataSource { get; set; }

        public Boolean Required { get; set; }

        public String CustomClass { get; set; }

        public String CustomColumnClass { get; set; }

        public Boolean Enabled { get; set; }

        public Boolean SumEnabled { get; set; }

        public Boolean IsId { get; set; }

        public List<ActionCore> RowActions { get; set; }

        public List<ActionCore> CustomSumActions { get; set; }

        public List<ActionCore> SumRowActions { get; set; }

        public ColumnCore(String idColumn, Boolean isId)
        {
            this.IdColumn = idColumn;
            this.IsId = isId;
        }

        public ColumnCore(String idColumn, Int32 index, Boolean required
            , String customClass = "", Boolean enabled = true, Boolean sumEnabled = true, String type = "", String customColumnClass = "", String idDataSource = "")
        {
            this.CustomColumnClass = customColumnClass == "" ? "col-xs-12 col-sm-2 col-md-1" : customColumnClass;
            this.CustomClass = customClass == "" ? "form-control" : customClass;
            this.Type = type == "" ? "input" : type;
            this.Required = required;
            this.IdColumn = idColumn;
            this.IdDataSource = idDataSource;
            this.Index = index;
            this.IsId = false;
            this.Enabled = enabled;
            this.SumEnabled = sumEnabled;
            if (!required && this.CustomClass != null && !this.CustomClass.Contains("ignore-validate"))
                this.CustomClass += " ignore-validate";
        }

        public ColumnCore(ColumnCore modelo)
        {
            this.CustomColumnClass = modelo.CustomColumnClass;
            this.CustomClass = modelo.CustomClass;
            this.Type = modelo.Type;
            this.Required = modelo.Required;
            this.IdColumn = modelo.IdColumn;
            this.IsId = modelo.IsId;
            this.IdDataSource = modelo.IdDataSource;
            this.Index = modelo.Index;
            this.Enabled = modelo.Enabled;
            this.CustomSumActions = modelo.CustomSumActions;
            this.SumEnabled = modelo.SumEnabled;
            this.RowActions = modelo.RowActions;
            this.SumRowActions = modelo.SumRowActions;
            if (!this.Required && this.CustomClass != null && !this.CustomClass.Contains("ignore-validate"))
                this.CustomClass += " ignore-validate";
        }

        public ColumnCore()
        {

        }
    }

    public class ActionCore
    {
        public Int32 Index { get; set; }

        public Boolean SumRow { get; set; }

        public Int32 IndexOp1 { get; set; }

        public Int32 Operacao { get; set; }

        public Int32 IndexOp2 { get; set; }

        public Int32 IndexResult { get; set; }

        public String DefaultValue { get; set; }

        public ActionCore(Int32 index)
        {
            this.Index = index;
        }
        public ActionCore()
        {

        }
    }

    public class RowCore
    {
        public Int32 Index { get; set; }

        public Int32 IdRow { get; set; }

        public Boolean Deleted { get; set; }

        public String CustomRowClass { get; set; }

        public List<ColumnCore> Columns { get; set; }

        public RowCore()
        {
            this.Deleted = false;
            this.Index = 0;
            this.CustomRowClass = "row";
        }

    }

    public class DataSourceCore
    {
        public String IdDataSource { get; set; }

        public List<SourceCore> Source { get; set; }

        public DataSourceCore()
        {

        }

        public DataSourceCore(String idDataSource, List<KeyValuePair<String, String>> keyValues)
        {
            this.IdDataSource = idDataSource;
            Source = new List<SourceCore>();
            foreach (KeyValuePair<String, String> item in keyValues)
                Source.Add(new SourceCore(item));
        }
    }

    public class SourceCore
    {
        public String Key { get; set; }
        public String Value { get; set; }

        public SourceCore()
        {
        }

        public SourceCore(KeyValuePair<String, String> keyValue)
        {
            this.Key = keyValue.Key;
            this.Value = keyValue.Value;
        }
    }

    public class GridConfigurationCore
    {
        public Guid IdControl { get; set; }

        public String IdDivGridControl { get; set; }

        public String LabelRow { get; set; }

        public String CustomCssLabelColumn { get; set; }

        public Boolean SumEnabled { get; set; }

        public Boolean Enabled { get; set; }

        public String CustomCssAddRow { get; set; }

        public String CustomCssAddCol { get; set; }

        public String CustomCssAddBtn { get; set; }

        public String CustomCssDelCol { get; set; }

        public String CustomUrlDelBtn { get; set; }

        public Boolean AddEnabled { get; set; }

        public Boolean DelEnabled { get; set; }

        public List<RowCore> Rows { get; set; }

        public List<DataSourceCore> DataSources { get; set; }

        public RowCore SumRow { get; set; }

        public String Tootip { get; set; }

		public Int32 RowLimit { get; set; }

        public GridConfigurationCore(String labelRow)
        {
            this.AddEnabled = false;
            this.DelEnabled = true;
            this.SumEnabled = false;
            this.Enabled = true;
            this.LabelRow = labelRow;
            this.Tootip = Tootip;
            this.CustomCssLabelColumn = "col-xs-12 col-sm-2 col-md-2";
            this.CustomCssAddRow = "row form-group";
            this.CustomCssAddCol = "col-xs-12 col-sm-2 col-md-2";
            this.CustomCssAddBtn = "btn btn-primary btn-xs";
            this.CustomCssDelCol = "";
            this.CustomUrlDelBtn = "/_layouts/15/images/mewa_backToMainb.GIF";
			this.RowLimit = 10;
        }

        public GridConfigurationCore()
        {

        }
    }
}
