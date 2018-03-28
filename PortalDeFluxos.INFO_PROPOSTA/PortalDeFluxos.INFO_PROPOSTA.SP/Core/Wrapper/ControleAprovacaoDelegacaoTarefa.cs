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
    public class ControleAprovacaoDelegacaoTarefa : ControleBase
    {
        public ControleAprovacaoDelegacaoTarefa(Control control)
        {
            _controle = control;
        }

        public void IniciarControle(Button btnPesquisar)
        {
            _controle.GetType().GetMethod("IniciarControle").Invoke(_controle, new object[] { btnPesquisar });
        }
    }
}
