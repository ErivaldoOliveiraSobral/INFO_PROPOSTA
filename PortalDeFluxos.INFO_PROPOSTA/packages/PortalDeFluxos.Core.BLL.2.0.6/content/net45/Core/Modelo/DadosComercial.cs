using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class DadosComercial
    {
        public string ListName { get; set; }//Utilizado identificar a lista alvo da configuração.

        public string Ibm { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string RazaoSocial { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string GerenteTerritorio { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string GerenteRegiao { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string DiretorVendas { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string Cdr { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string Gdr { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail

        public DadosComercial()
        { 
        }
    }
}
