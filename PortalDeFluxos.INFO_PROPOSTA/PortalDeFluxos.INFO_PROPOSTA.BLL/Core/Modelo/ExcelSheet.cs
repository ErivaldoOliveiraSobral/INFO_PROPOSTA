using System;
using System.Data;

namespace PortalDeFluxos.Core.BLL.Core.Modelo
{
	public class ExcelSheet
	{
		public String NomeSheet { get; set; }

		public DataTable ConteudoSheet { get; set; }

		public ExcelSheet()
		{

		}

		public ExcelSheet(String nome,DataTable dataTable)
		{
			this.NomeSheet = nome;
			this.ConteudoSheet = dataTable;
		}
	}
}
