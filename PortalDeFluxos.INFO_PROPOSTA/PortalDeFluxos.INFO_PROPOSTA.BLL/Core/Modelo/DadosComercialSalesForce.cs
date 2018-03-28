using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class DadosComercialSalesForce
    {
        public string Ibm { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string SiteCode { get; set; }
        public string RazaoSocial { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string GerenteTerritorio { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string GerenteRegiao { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string DiretorVendas { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string Cdr { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail
        public string Gdr { get; set; }//Utilizado para definir o internal name ou a informação enviada no e-mail

        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Bairro { get; set; }
        public string NumeroOportunidade { get; set; }
        public string TipoProposta { get; set; }
        public string PlataformaAtual { get; set; }
        public string InscricaoEstadual { get; set; }

        public DadosComercialSalesForce()
        {
        }
    }
}
