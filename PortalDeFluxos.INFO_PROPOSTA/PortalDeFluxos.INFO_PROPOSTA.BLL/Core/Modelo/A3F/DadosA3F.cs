using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Core.Modelo.A3F
{
	public class DadosA3F
	{
		public String CodigoProposta { get; set; }
		public String Fluxo { get; set; }
		public String Ibm { get; set; }
		public String Status { get; set; }
		public String ValorContabil { get; set; }
		public String ValorVenda { get; set; }
		public String ValorTerreno { get; set; }
		public String Mensagem { get; set; }
	}

	[Serializable]
	public class RetornoWF
	{
		public String Retorno { get; set; }
	}
}
