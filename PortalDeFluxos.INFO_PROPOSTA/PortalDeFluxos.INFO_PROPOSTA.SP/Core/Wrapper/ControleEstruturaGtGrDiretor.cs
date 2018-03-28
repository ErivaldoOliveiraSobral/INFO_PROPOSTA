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
    public class ControleEstruturaGtGrDiretor : ControleBase
    {
        public ControleEstruturaGtGrDiretor(Control control)
        {
            _controle = control;
        }

        public void CarregarControle(Boolean rezoneamentoPadrao, String loginGt = "", String loginGr = "", String loginDiretor = ""
            , Boolean rezoneamentoHabilitado = true, Boolean gtHabilitado = false, Boolean grHabilitado = false, Boolean diretorHabilitado = false)
        {
            _controle.GetType().GetMethod("CarregarControle").Invoke(_controle, new object[] { rezoneamentoPadrao, loginGt, loginGr, loginDiretor
                ,rezoneamentoHabilitado , gtHabilitado, grHabilitado, diretorHabilitado });
        }

        public void ControlesObrigatorios(Boolean gtObrigatorio = true, Boolean grObrigatorio = true, Boolean diretorObrigatorio = true)
        {
            _controle.GetType().GetMethod("ControlesObrigatorios").Invoke(_controle, new object[] { gtObrigatorio, grObrigatorio, diretorObrigatorio });
        }

        public void ControlesHabilitados(Boolean rezoneamentoHabilitado = true, Boolean gtHabilidato = true, Boolean grHabilidato = true, Boolean diretorHabilidato = true)
        {
            _controle.GetType().GetMethod("ControlesHabilitados").Invoke(_controle, new object[] { rezoneamentoHabilitado, gtHabilidato, grHabilidato, diretorHabilidato });
        }

        public void MudarZoneamentoPadrao(Boolean utilizaZoneamento)
        {
            ControleRaizenForm controleRaizenForm = new ControleRaizenForm(_controle);
            controleRaizenForm.MudarZoneamentoPadrao(utilizaZoneamento);
        }

        public ClientPeoplePicker ObterGerenteTerritorio()
        {
            object gerenteTerritorio = _controle.GetType().GetProperty("GerenteTerritorio").GetValue(_controle, null);
            if (gerenteTerritorio is ClientPeoplePicker)
                return (ClientPeoplePicker)gerenteTerritorio;
            else
                return null;
        }

        public ClientPeoplePicker ObterGerenteRegiao()
        {
            object gerenteRegiao = _controle.GetType().GetProperty("GerenteRegiao").GetValue(_controle, null);
            if (gerenteRegiao is ClientPeoplePicker)
                return (ClientPeoplePicker)gerenteRegiao;
            else
                return null;
        }

        public ClientPeoplePicker ObterDiretorVendas()
        {
            object diretorVendas = _controle.GetType().GetProperty("DiretorVendas").GetValue(_controle, null);
            if (diretorVendas is ClientPeoplePicker)
                return (ClientPeoplePicker)diretorVendas;
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
