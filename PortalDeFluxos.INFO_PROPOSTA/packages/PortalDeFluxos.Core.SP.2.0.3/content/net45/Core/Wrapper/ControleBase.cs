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
    public class ControleBase : UserControl
    {
        protected Control _controle = null;
        
        /// <summary>
        /// Retorna o controle que possui o método com o nome indicado
        /// </summary>
        /// <param name="controle"></param>
        /// <param name="nomeMetodo"></param>
        /// <returns></returns>
        protected Control ObterControle(Control controle,String nomeMetodo)
        {
            if (controle == null)
                return null;

            Control controleAlvo = null;
            if (controle.GetType().GetMethod(nomeMetodo) != null)
                controleAlvo = controle;
            else
                controleAlvo = ObterControleFilho(controle.Controls, nomeMetodo);

            if (controleAlvo == null)
                controleAlvo = ObterControle(controle.Parent, nomeMetodo);

            return controleAlvo;
        }

        protected Control ObterControleFilho(ControlCollection controlesFilhos, String nomeMetodo)
        {
            if (controlesFilhos == null || controlesFilhos.Count == 0)
                return null;

            Control controleFilho = null;
            foreach (Control controleAtual in controlesFilhos)
            {
                if (controleAtual.GetType().GetMethod(nomeMetodo) != null)
                {
                    return controleAtual;
                }
                controleFilho = ObterControleFilho(controleAtual.Controls, nomeMetodo);
                if (controleFilho != null)
                    return controleFilho;
            }


            return controleFilho;
        }
    }
}
