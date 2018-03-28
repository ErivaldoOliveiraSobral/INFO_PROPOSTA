using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public class CustomGridView : GridView
    {
        private const string _virtualItemCount = "virtualItemCount";
        private const string _currentPageIndex = "currentPageIndex";
        private const string _sortedField = "sortedField";
        private const string _sortedDirection = "sortedDirection";

        public const string colorOver = "#FFEAAA";
        public const string colorRow = "#F1F1F1";
        public const string colorRowAlternate = "#FFFFFF";

        #region [ Properties ]
        public int VirtualItemCount
        {
            get
            {
                if (ViewState[_virtualItemCount] == null)
                    ViewState[_virtualItemCount] = -1;
                return Convert.ToInt32(ViewState[_virtualItemCount]);
            }
            set
            {
                ViewState[_virtualItemCount] = value;
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                if (ViewState[_currentPageIndex] == null)
                    ViewState[_currentPageIndex] = 0;
                return Convert.ToInt32(ViewState[_currentPageIndex]);
            }
            set
            {
                ViewState[_currentPageIndex] = value;
            }
        }

        public override object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        public bool CustomPaging
        {
            get { return (this.VirtualItemCount != -1); }
        }

        public Int32 StartRecordIndex
        {
            get { return this.CurrentPageIndex * this.PageSize; }
        }

        public String SortedField
        {
            get { return (String)ViewState[_sortedField]; }
            set { ViewState[_sortedField] = value; }
        }

        public Int32 LastPageIndex
        {
            get
            {

                if (this.VirtualItemCount % this.PageSize == 0)
                    return (this.VirtualItemCount / this.PageSize) - 1;
                return this.VirtualItemCount / this.PageSize;
            }
        }

        public SortDirection SortedDirection
        {
            get
            {
                if (ViewState[_sortedDirection] == null)
                    ViewState[_sortedDirection] = SortDirection.Ascending;
                return (System.Web.UI.WebControls.SortDirection)ViewState[_sortedDirection];
            }
            set { ViewState[_sortedDirection] = value; }
        }

        public String PagingText
        {
            get
            {
                if (this.Rows == null || this.Rows.Count == 0)
                    return String.Empty;

                Int32 totalDe = (this.PageSize * this.CurrentPageIndex) + 1;
                Int32 totalAte = (this.PageSize * this.CurrentPageIndex) + (this.Rows.Count < this.PageSize ? this.Rows.Count : this.PageSize);

                return String.Format("Mostrando de {0} até {1} de {2} registros"
                    , totalDe.ToString()
                    , totalAte.ToString()
                    , this.VirtualItemCount.ToString());
            }
        }
        #endregion

        public CustomGridView()
        {
            //set defaults
            this.PagerStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            this.PagerStyle.CssClass = "pagerRow TABLE";
            this.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            this.PagerSettings.FirstPageText = "<<";
            this.PagerSettings.LastPageText = ">>";
            this.PagerSettings.NextPageText = ">";
            this.PagerSettings.PreviousPageText = "<";
            this.Width = Unit.Percentage(100);
            this.HeaderStyle.CssClass = "gridview-header";// "ms-viewheadertr";
            this.EmptyDataText = "Não há itens para mostrar nessa exibição";
            this.ShowFooter = true;
        }

        protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
        {
            if (CustomPaging)
            {
                pagedDataSource.AllowCustomPaging = true;
                pagedDataSource.AllowPaging = true;
                pagedDataSource.VirtualCount = this.VirtualItemCount;
                pagedDataSource.CurrentPageIndex = this.CurrentPageIndex;
            }
            base.InitializePager(row, columnSpan, pagedDataSource);
        }

        public void AddResultGridViewSortedColumnHeaderImage(GridView gridView)
        {
            Int32 sortedColumnIndex = 0;
            Image sortedImage = null;
            DataControlField field = null;

            if (gridView.HeaderRow == null || String.IsNullOrEmpty(SortedField))
                return;

            field = gridView.Columns.Cast<DataControlField>().FirstOrDefault(
                item => item.SortExpression.Equals(SortedField, StringComparison.OrdinalIgnoreCase)
            );

            sortedColumnIndex = gridView.Columns.IndexOf(field);

            if (sortedColumnIndex < 0)
                sortedColumnIndex = 0;

            sortedImage = new Image();
            sortedImage.ImageUrl = String.Concat(
                CultureInfo.InvariantCulture,
                "_layouts/15/images/PortalDeFluxos.Core.SP/",
                SortedDirection == SortDirection.Ascending ? "ARWUP.GIF" : "ARWDOWN.GIF"
            );
            sortedImage.CssClass = "headerSortedImage";

            gridView.HeaderRow.Cells[sortedColumnIndex].Controls.Add(sortedImage);
        }

        public void ChangeRowColor(GridViewRowEventArgs e, string colorOver, string colorRow, string colorRowAlternate)
        {
            e.Row.Attributes.Add("onmouseover", string.Format("this.style.backgroundColor='{0}'", colorOver));

            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", string.Format("this.style.backgroundColor='{0}'", colorRowAlternate));
            else
                e.Row.Attributes.Add("onmouseout", string.Format("this.style.backgroundColor='{0}'", colorRow));
        }

    }
}
