using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleMenuSuperior : ControleBase
    {
        public ControleMenuSuperior(Control control)
        {
            _controle = control;
        }

        public void PopularMenu()
        {
            Control controleAlvo = ObterControle(_controle, "ObterConfiguracaoMenu");
            if(controleAlvo != null)
            {
                object listaConfiguracaoMenu = controleAlvo.GetType().GetMethod("ObterConfiguracaoMenu").Invoke(controleAlvo, null);
                if (!(listaConfiguracaoMenu is List<KeyValuePair<String, String>>))
                    throw new Exception("Propriedade ListaConfiguracaoMenu está incorreta.");
                object idMenuAtivo = controleAlvo.GetType().GetMethod("ObterMenuAtivo").Invoke(controleAlvo, null);
                if (!(idMenuAtivo is String))
                    throw new Exception("Propriedade IdMenuAtivo está incorreta.");
                object exibirMenuTarefa = controleAlvo.GetType().GetMethod("ExibirMenuTarefa").Invoke(controleAlvo, null);
                if (!(exibirMenuTarefa is Boolean))
                    throw new Exception("Propriedade ExibirMenuTarefa está incorreta.");
                object tituloFormulario = controleAlvo.GetType().GetMethod("ObterTituloFormulario").Invoke(controleAlvo, null);
                _controle.GetType().GetMethod("PopularMenu").Invoke(_controle, new object[] { (List<KeyValuePair<String, String>>)listaConfiguracaoMenu
                    , (String)idMenuAtivo, (Boolean)exibirMenuTarefa,(String) tituloFormulario});
            }
        }
    }
}
