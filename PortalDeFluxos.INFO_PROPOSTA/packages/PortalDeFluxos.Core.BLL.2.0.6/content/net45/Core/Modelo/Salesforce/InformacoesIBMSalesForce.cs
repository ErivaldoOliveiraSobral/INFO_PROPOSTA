using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Core.Modelo.Salesforce
{
    [Serializable]
    public class InformacoesIBMSalesForce
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public String TipoRegistro { get; set; }
        public int IBM { get; set; }
        public String StatusLoja { get; set; }
        public String TipoProduto { get; set; }
        public Decimal VolumeTotal { get; set; }

        public InformacoesIBMSalesForce() { }

		public InformacoesIBMSalesForce(int ibm, String statusLoja, Decimal valorMedio)
		{
			this.IBM = ibm;
			this.StatusLoja = statusLoja;
			this.VolumeTotal = valorMedio;
		}
    }
}
