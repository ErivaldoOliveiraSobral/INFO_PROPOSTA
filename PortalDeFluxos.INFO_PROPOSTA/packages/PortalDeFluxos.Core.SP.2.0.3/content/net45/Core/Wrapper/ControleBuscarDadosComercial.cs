using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleBuscarDadosComercial : ControleBase
    {
        public ControleBuscarDadosComercial(Control control)
        {
            _controle = control;
        }

        public void CarregarControle(Int32? valorIbmSiteCode, Boolean buscarPorIbm = true, Boolean readOnly = false, String tipoProposta = "")
        {
            _controle.GetType().GetMethod("CarregarControle").Invoke(_controle, new object[] { valorIbmSiteCode, buscarPorIbm, readOnly, tipoProposta });
        }

        public void ChamadaWebServiceHabilitada(Boolean habilitado)
        {
            _controle.GetType().GetMethod("ChamadaWebServiceHabilitada").Invoke(_controle, new object[] { habilitado });
        }

        public void DesabilitarObrigatoriedade()
        {
            _controle.GetType().GetMethod("DesabilitarObrigatoriedade").Invoke(_controle, null);
        }
    
        public void EnviarDadosComercial(String dadosComercial)
        {
            ControleRaizenForm controleRaizenForm = new ControleRaizenForm(_controle);
            controleRaizenForm.CarregarDadosComercial(dadosComercial);
        }

        public void RecarregarEstruturaComercial()
        {
            _controle.GetType().GetMethod("RecarregarEstruturaComercial").Invoke(_controle,null);
        }

        public String ObterIbm()
        {
            object ibm = _controle.GetType().GetProperty("Ibm").GetValue(_controle, null);
            if (ibm is String)
                return (String)ibm;
            else
                return String.Empty;
        }
        public void LimparIbm()
        {
            _controle.GetType().GetProperty("Ibm").SetValue(_controle, null);
        }
    }
}
