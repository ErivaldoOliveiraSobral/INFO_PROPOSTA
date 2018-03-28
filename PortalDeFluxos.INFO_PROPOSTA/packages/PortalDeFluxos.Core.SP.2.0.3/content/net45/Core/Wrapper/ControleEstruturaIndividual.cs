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
    public class ControleEstruturaIndividual : ControleBase
    {
        public ControleEstruturaIndividual(Control control)
        {
            _controle = control;
        }

        public void CarregarControle(Boolean rezoneamentoPadrao, String nomeNivel = "", String loginResponsavelNivel = ""
            , Boolean rezoneamentoHabilitado = true, Boolean rezoneamentoVisivel = false)
        {
            _controle.GetType().GetMethod("CarregarControle").Invoke(_controle, new object[] { rezoneamentoPadrao, nomeNivel, loginResponsavelNivel, rezoneamentoHabilitado, rezoneamentoVisivel });
        }

        public void ControlesObrigatorios(Boolean nivelObrigatorio = true)
        {
            _controle.GetType().GetMethod("ControlesObrigatorios").Invoke(_controle, new object[] { nivelObrigatorio });
        }

        public void ControlesHabilitados(Boolean rezoneamentoPadrao, Boolean rezoneamentoHabilitado = true)
        {
            _controle.GetType().GetMethod("ControlesHabilitados").Invoke(_controle, new object[] { rezoneamentoPadrao, rezoneamentoHabilitado });
        }

        public void MudarZoneamentoPadrao(Boolean utilizaZoneamento)
        {
            ControleRaizenForm controleRaizenForm = new ControleRaizenForm(_controle);
            controleRaizenForm.MudarZoneamentoPadrao(utilizaZoneamento);
        }

        public ClientPeoplePicker ObterResponsavelNivel()
        {
            object responsavelNivel = _controle.GetType().GetProperty("ResponsavelNivel").GetValue(_controle, null);
            if (responsavelNivel is ClientPeoplePicker)
                return (ClientPeoplePicker)responsavelNivel;
            else
                return null;
        }

        public Boolean UtilizaRezoneamento()
        {
            object utilizaZoneamento = _controle.GetType().GetProperty("UtilizaRezoneamento").GetValue(_controle, null);
            if (utilizaZoneamento is Boolean)
                return (Boolean)utilizaZoneamento;
            else
                return false;
        }

    }
}
