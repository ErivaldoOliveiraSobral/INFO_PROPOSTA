using Iteris.SharePoint.Design;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Atributos;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Iteris;
using PortalDeFluxos.Core.BLL.Modelo.AnexoFacilAPI;

namespace PortalDeFluxos.Core.BLL.Negocio
{
	public static class NegocioAnexoFacil
	{
        /// <summary>
        /// Popula as propriedades (Link e EstruturaComercial e Id ) do PropostaApiModelCollection
        /// 
        /// </summary>
        /// <param name="propostasEnviar"></param>
        /// <returns></returns>
		public static KeyValuePair<bool, string> AtualizarPropostaAnexoFacil(PropostaApiModelCollection propostasEnviar, EntidadePropostaSP propostaSP)
        {
            PropostaApiModelCollection apiRetorno = null;
            Lista lista = null;
            String listaDescricaoItem = String.Empty;
            foreach (var proposta in propostasEnviar.PropostaApi)
            {
                AnexoFacil anexoFacil = new AnexoFacil().Consultar(_ => _.CodigoLista == proposta.CodigoLista 
					&& _.CodigoItem == proposta.CodigoItem
					&& _.CodigoIntegracao == proposta.CodigoIntegracao).FirstOrDefault();
                if (anexoFacil != null)
                    proposta.Id = anexoFacil.CodigoAnexoFacil;
                else
                    proposta.Id = -1;

                #region [Link]

                #region [Descrição Url Item]
                if (lista == null && String.IsNullOrEmpty(listaDescricaoItem))
                    lista = new Lista().Consultar(l => l.CodigoLista == proposta.CodigoLista).FirstOrDefault();
                if (lista != null && String.IsNullOrEmpty(listaDescricaoItem))
                    listaDescricaoItem = String.Format("{0}", new Uri(new Uri(PortalWeb.ContextoWebAtual.Url), lista.DescricaoUrlItem));
                #endregion

                String parametroContentType = String.IsNullOrEmpty(propostaSP.ContentType) ? String.Empty : String.Format("&ContentTypeId={0}", propostaSP.ContentType);
                String parametroIntegracao = String.IsNullOrEmpty(propostaSP.ContentType) ? String.Empty : String.Format("&Integracao={0}", proposta.CodigoIntegracao);
                proposta.Link = String.Format("{0}{1}{2}{3}", listaDescricaoItem, proposta.CodigoItem, parametroContentType, parametroIntegracao);

                #endregion

                #region [Estrutura Comercial]

				proposta.EstruturaComercial = new EstruturaComercialApiModel
				{
					CDR = propostaSP.Cdr != null ? propostaSP.Cdr.Login.StripDomain() : String.Empty,
					GDR = propostaSP.Gdr != null ? propostaSP.Gdr.Login.StripDomain() : String.Empty,
					Diretor = propostaSP.DiretorVendas != null ? propostaSP.DiretorVendas.Login.StripDomain() : String.Empty,
					GR = propostaSP.GerenteRegiao != null ? propostaSP.GerenteRegiao.Login.StripDomain() : String.Empty,
					GT = propostaSP.GerenteTerritorio != null ? propostaSP.GerenteTerritorio.Login.StripDomain() : String.Empty
				};

                #endregion
            }

			KeyValuePair<bool, string> retornoWS = NegocioServicos.CadastrarPropostaAnexoFacil(Serializacao.SerializeToJson(propostasEnviar));
			String retornoTratado = retornoWS.Value.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "");
			try
			{
				apiRetorno = Serializacao.DeserializeFromJson<PropostaApiModelCollection>(retornoTratado);
			}
			catch
			{
				apiRetorno = null;
				retornoWS = new KeyValuePair<bool, string>(false, "Ocorreu um erro na integração com o Anexo Fácil.");
				new Log().Inserir("NegocioAnexoFacil", "AtualizarPropostaAnexoFacil", new Exception(retornoTratado));
			}

            if (apiRetorno != null)
                foreach (var propostaRecebida in apiRetorno.PropostaApi)
                {
                    PropostaApiModel propostaEnviada = propostasEnviar.PropostaApi.FirstOrDefault(_ => _.CodigoLista == propostaRecebida.CodigoLista
                    && _.CodigoItem == propostaRecebida.CodigoItem && _.CodigoIntegracao == propostaRecebida.CodigoIntegracao);

                    if (propostaEnviada != null && propostaEnviada.Id <= 0 && propostaRecebida.Id > 0)//Pode salvar apenas algumas propostas no anexo facil.
                    {
                        AnexoFacil novoRegistro = new AnexoFacil();
                        novoRegistro.CodigoItem = propostaRecebida.CodigoItem;
                        novoRegistro.CodigoLista = propostaRecebida.CodigoLista;
                        novoRegistro.CodigoAnexoFacil = propostaRecebida.Id;
                        novoRegistro.CodigoIntegracao = propostaRecebida.CodigoIntegracao;
                        novoRegistro.Inserir();
                    }
                }
			
			return retornoWS;
        }
    }
}
