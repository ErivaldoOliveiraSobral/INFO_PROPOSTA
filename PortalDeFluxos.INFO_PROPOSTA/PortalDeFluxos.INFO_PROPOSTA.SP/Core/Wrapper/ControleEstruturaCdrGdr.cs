using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleEstruturaCdrGdr : ControleBase
    {
        public ControleEstruturaCdrGdr(Control control)
        {
            _controle = control;
        }

        public void CarregarControle(Boolean rezoneamentoPadrao,String loginCdr = "", String loginGdr = "",Boolean cdrHabilitado = false,Boolean gdrHabilitado = false)
        {
            _controle.GetType().GetMethod("CarregarControle").Invoke(_controle, new object[] { rezoneamentoPadrao, loginCdr, loginGdr, cdrHabilitado, gdrHabilitado });
        }

        public void ControlesObrigatorios(Boolean cdrObrigatorio = true, Boolean gdrObrigatorio = true)
        {
            _controle.GetType().GetMethod("ControlesObrigatorios").Invoke(_controle, new object[] { cdrObrigatorio, gdrObrigatorio });
        }

        public void ControlesHabilitados(Boolean cdrHabilidato = true, Boolean gdrHabilidato = true)
        {
            _controle.GetType().GetMethod("ControlesHabilitados").Invoke(_controle, new object[] { cdrHabilidato, gdrHabilidato });
        }

        public ClientPeoplePicker ObterCdr()
        {
            object cdr = _controle.GetType().GetProperty("Cdr").GetValue(_controle, null);
            if (cdr is ClientPeoplePicker)
                return (ClientPeoplePicker)cdr;
            else
                return null;
        }

        public ClientPeoplePicker ObterGdr()
        {
            object gdr = _controle.GetType().GetProperty("Gdr").GetValue(_controle, null);
            if (gdr is ClientPeoplePicker)
                return (ClientPeoplePicker)gdr;
            else
                return null;
        }

    }
}
