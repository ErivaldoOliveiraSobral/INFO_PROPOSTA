using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleDocumento : ControleBase
    {
        public ControleDocumento(Control control)
        {
            _controle = control;
        }

        public void CarregarDocumentos(int idTipoProposta, string agrupador)
        {
            _controle.GetType().GetMethod("CarregarDocumentos").Invoke(_controle, new object[] { idTipoProposta, agrupador });
        }

        public void CarregarDocumentosProposta(int IdProposta, int idTipoProposta, string agrupador, Boolean recarregar = false)
        {
            _controle.GetType().GetMethod("CarregarDocumentosProposta").Invoke(_controle, new object[] { IdProposta, idTipoProposta, agrupador, recarregar });
        }

        public  GridView ObterGridView()
        {
            object grv = _controle.GetType().GetProperty("GridViewDocumento").GetValue(_controle, null);
            if (grv is GridView)
                return (GridView)grv;
            else
                return null;
        }

    }
}
