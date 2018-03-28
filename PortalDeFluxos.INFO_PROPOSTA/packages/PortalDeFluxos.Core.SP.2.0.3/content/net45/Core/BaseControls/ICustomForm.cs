
using System;
using System.Collections.Generic;
using System.Web.UI;
namespace PortalDeFluxos.Core.SP.Core.BaseControls
{
    public interface ICustomForm
    {
        void CarregarDadosComercial(Control control, String dadosComercial);

        void MudarZoneamentoPadrao(Control control, Boolean habilitado);

        List<KeyValuePair<String,String>> ObterConfiguracaoMenu();

        String ObterMenuAtivo();

        Boolean ExibirMenuTarefa();

        String ObterTituloFormulario();
    }
}
