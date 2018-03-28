using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleAnexo : ControleBase
    {
        public ControleAnexo(Control control)
        {
            _controle = control;
        }

        public void SalvarAnexos(String url)
        {
            _controle.GetType().GetMethod("SalvarAnexos").Invoke(_controle, new object[] { url });
        }

        public Int32 ObterQuantidadeAnexos()
        {
            object quantidade = _controle.GetType().GetMethod("ObterQuantidadeAnexos").Invoke(_controle, null);
            return quantidade is int ? (int)quantidade : 0;
        }
		
	}
}
