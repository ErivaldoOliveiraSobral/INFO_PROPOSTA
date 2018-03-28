using PortalDeFluxos.Core.BLL.Core.Modelo;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public class NegocioRelatorio
    {
		#region [Constantes]

		public const String InicioEstruturaExcel = @"<?xml version=""1.0""?>
	<?mso-application progid=""Excel.Sheet""?>
	<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
		xmlns:o=""urn:schemas-microsoft-com:office:office""
		xmlns:x=""urn:schemas-microsoft-com:office:excel""
		xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
		xmlns:html=""http://www.w3.org/TR/REC-html40"">
		<DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
		<Author>William Morita - WMO</Author>
		<LastAuthor>William Morita - WMO</LastAuthor>
		<Created>2017-06-30T19:05:26Z</Created>
		<LastSaved>2017-06-30T19:06:39Z</LastSaved>
		<Version>16.00</Version>
		</DocumentProperties>
		<OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">
		<AllowPNG/>
		</OfficeDocumentSettings>
		<ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
		<WindowHeight>7650</WindowHeight>
		<WindowWidth>20490</WindowWidth>
		<WindowTopX>0</WindowTopX>
		<WindowTopY>0</WindowTopY>
		<ActiveSheet>0</ActiveSheet>
		<ProtectStructure>False</ProtectStructure>
		<ProtectWindows>False</ProtectWindows>
		</ExcelWorkbook>
		<Styles>
		<Style ss:ID=""Default"" ss:Name=""Normal"">
		<Alignment ss:Vertical=""Bottom""/>
		<Borders/>
		<Font ss:FontName=""Calibri"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000""/>
		<Interior/>
		<NumberFormat/>
		<Protection/>
		</Style>
		</Styles>";

		public const String FimEstruturaExcel = "</Workbook>";

		public const String InicioEstruturaWorkSheet = @"<Worksheet ss:Name=""{0}"">
	<Table ss:ExpandedColumnCount=""{1}"" ss:ExpandedRowCount=""{2}"" x:FullColumns=""1"" x:FullRows=""1"" ss:DefaultRowHeight=""15"">";

		public const String FimEstruturaWorkSheet = @"</Table>
	<WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
	<Selected/>
	<ProtectObjects>False</ProtectObjects>
	<ProtectScenarios>False</ProtectScenarios>
	</WorksheetOptions>
	</Worksheet>";

		public const String RowFormat = @"<Row ss:AutoFitHeight=""0"">{0}</Row>";

		public const String CellFormat = @"<Cell><Data ss:Type=""String"">{0}</Data></Cell>";
		
		#endregion

		public static String ObterExcel(List<ExcelSheet> tables)
		{
			StringWriter sw = new StringWriter();
			if (tables == null || tables.Count == 0)
				return String.Empty;

			sw.WriteLine(InicioEstruturaExcel);
			foreach (var sheetConfig in tables)				
				CarregarWorkSheet(sheetConfig, sw);
			sw.WriteLine(FimEstruturaExcel);

			return sw.ToString();
		}

		private static void CarregarWorkSheet(ExcelSheet sheetConfig, StringWriter sw)
		{
			if (sheetConfig.ConteudoSheet == null || sheetConfig.ConteudoSheet.Columns == null )
				return;
			Int32 rowCount = sheetConfig.ConteudoSheet.Rows == null? 1 : sheetConfig.ConteudoSheet.Rows.Count + 1;
			sw.WriteLine(String.Format(InicioEstruturaWorkSheet, sheetConfig.NomeSheet, sheetConfig.ConteudoSheet.Columns.Count, rowCount));
			sw.WriteLine("");
			StringBuilder rows = new StringBuilder();

			#region [Titulo Tabela]

			foreach (DataColumn coluna  in sheetConfig.ConteudoSheet.Columns)
				rows.AppendLine(String.Format(CellFormat,coluna.ColumnName));
			sw.WriteLine(RowFormat, rows.ToString());

			#endregion

			#region [Conteúdo Tabela]

			rows = new StringBuilder();
			foreach (DataRow  row in sheetConfig.ConteudoSheet.Rows)
			{
				rows = new StringBuilder();
				sw.WriteLine("");
				foreach (DataColumn coluna  in sheetConfig.ConteudoSheet.Columns)
					rows.AppendLine(String.Format(CellFormat, row[coluna].ToString()));
				sw.WriteLine(RowFormat, rows.ToString());
				sw.WriteLine("");
			}

			#endregion
			
			sw.WriteLine(FimEstruturaWorkSheet);
		}

        public static String ObterHtmlTextWriter(DataTable dataTable)
        {
            StringWriter sw = new StringWriter();
            GridView gridView = new GridView();
            gridView.AllowPaging = false;
            gridView.DataSource = dataTable;
            gridView.DataBind();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gridView.RenderControl(hw);
            return sw.ToString();
        }


		public static DataSet ObterRelatorioTarefas(String periodoInicio, String periodoFim, Boolean consultaPorTarefa, string[] dsHeader)
        {
            return DadosRelatorio.ObterRelatorioTarefas(periodoInicio, periodoFim, consultaPorTarefa, dsHeader);
        }

    }
}
