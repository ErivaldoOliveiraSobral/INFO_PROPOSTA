using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleRaizenForm : ControleBase
    {
        public ControleRaizenForm(Control control)
        {
            _controle = control;
        }

        public void CarregarDadosComercial(String dadosComercial)
        {
            Control controlePai = ObterControle(_controle.Parent, "CarregarDadosComercial");
            if (controlePai == null)
                throw new Exception("Controle Raizen Form - não foi localizado.");
            controlePai.GetType().GetMethod("CarregarDadosComercial").Invoke(controlePai, new object[] { _controle, dadosComercial });
        }

        public void MudarZoneamentoPadrao(Boolean zoneamentoPadrao)
        {
            Control controlePai = ObterControle(_controle.Parent, "MudarZoneamentoPadrao");
            if (controlePai == null)
                throw new Exception("Controle Raizen Form - não foi localizado.");
            controlePai.GetType().GetMethod("MudarZoneamentoPadrao").Invoke(controlePai, new object[] { _controle, zoneamentoPadrao });
        }

        public Boolean ReiniciarWorkflow()
        {
            Control controlePai = ObterControle(_controle.Parent, "ReiniciarWorkflow");
            if (controlePai == null)
                throw new Exception("Controle Raizen Form - não foi localizado.");
            Object retorno = controlePai.GetType().GetMethod("ReiniciarWorkflow").Invoke(controlePai, null);
            
            Boolean reiniciar = false;
            if (retorno is Boolean)
                reiniciar = Convert.ToBoolean(retorno);

            return reiniciar;
        }

        public Int32 CodigoItemNovo()
        {
            Control controlePai = ObterControle(_controle.Parent, "CodigoItemNovo");
            if (controlePai == null)
                throw new Exception("Controle Raizen Form - não foi localizado.");
            Object retorno = controlePai.GetType().GetMethod("CodigoItemNovo").Invoke(controlePai, null);

            Int32 codigoItemNovo = -1;
            Int32.TryParse(retorno.ToString(),out codigoItemNovo);

            return codigoItemNovo;
        }

        public void SetCodigoItemNovo(Int32 codigoItemNovo)
        {
            Control controlePai = ObterControle(_controle.Parent, "SetCodigoItemNovo");
            if (controlePai == null)
                throw new Exception("Controle Raizen Form - não foi localizado.");
            controlePai.GetType().GetMethod("SetCodigoItemNovo").Invoke(controlePai, new object[] { codigoItemNovo });
        }

        public Boolean ExibirAnexoFacil()
        {
            Control controlePai = ObterControle(_controle.Parent, "ExibirAnexoFacil");
            if (controlePai == null)
                return false;
            Object retorno = controlePai.GetType().GetMethod("ExibirAnexoFacil").Invoke(controlePai, null);

            Boolean exibirAnexoFacil = false;
            Boolean.TryParse(retorno.ToString(), out exibirAnexoFacil);
            return exibirAnexoFacil;
        }

		public Boolean ObterA3F()
		{
			Control controlePai = ObterControle(_controle.Parent, "ObterA3F");
			if (controlePai == null)
				return false;
			Object retorno = controlePai.GetType().GetMethod("ObterA3F").Invoke(controlePai, null);

			Boolean obterA3F = false;
			Boolean.TryParse(retorno.ToString(), out obterA3F);
			return obterA3F;
		}

		public String ObterA3FUrlProposta()
		{
			Control controlePai = ObterControle(_controle.Parent, "ObterA3FUrlProposta");
			if (controlePai == null)
				return String.Empty;
			Object retorno = controlePai.GetType().GetMethod("ObterA3FUrlProposta").Invoke(controlePai, null);

			return retorno.ToString();
		}
    }
}
