using Iteris;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioProposta
	{
        [Obsolete("Método utilizado apenas para Aditivos Gerais (migração). Utilizar")]
        public static DadosComercial ObterDadosComercial(List lista, ListItem item, List<DadosComercial> estruturaComercialListas)
        {
            DadosComercial estruturaComercialAtual = new DadosComercial();
            try
            {
                estruturaComercialAtual = estruturaComercialListas.FirstOrDefault(e => e.ListName == lista.Title);
                if (estruturaComercialAtual != null)
                {
                    PortalWeb.ContextoWebAtual.SPClient.Load(item.FieldValuesAsText);
                    PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                    estruturaComercialAtual.Ibm = lista.ContainsField(estruturaComercialAtual.Ibm) 
                        ? item.FieldValuesAsText[estruturaComercialAtual.Ibm] : String.Empty;
                    estruturaComercialAtual.RazaoSocial = lista.ContainsField(estruturaComercialAtual.RazaoSocial) 
                        ? item.FieldValuesAsText[estruturaComercialAtual.RazaoSocial]: String.Empty;
                    estruturaComercialAtual.GerenteTerritorio = lista.ContainsField(estruturaComercialAtual.GerenteTerritorio) 
                        ? item.FieldValuesAsText[estruturaComercialAtual.GerenteTerritorio] : String.Empty;
                    estruturaComercialAtual.GerenteRegiao = lista.ContainsField(estruturaComercialAtual.GerenteRegiao)
                        ? item.FieldValuesAsText[estruturaComercialAtual.GerenteRegiao] : String.Empty;
                    estruturaComercialAtual.DiretorVendas = lista.ContainsField(estruturaComercialAtual.DiretorVendas)
                        ? item.FieldValuesAsText[estruturaComercialAtual.DiretorVendas] : String.Empty;
                    estruturaComercialAtual.Cdr = lista.ContainsField(estruturaComercialAtual.Cdr)
                        ? item.FieldValuesAsText[estruturaComercialAtual.Cdr] : String.Empty;
                    estruturaComercialAtual.Gdr = lista.ContainsField(estruturaComercialAtual.Gdr)
                        ? item.FieldValuesAsText[estruturaComercialAtual.Gdr] : String.Empty;
                }
            }
            catch (Exception ex)
            {
                new Log().Inserir("NegocioProposta", "DadosComercial - ObterDadosComercial", ex);
            }
            return estruturaComercialAtual;
        }

	}
}
