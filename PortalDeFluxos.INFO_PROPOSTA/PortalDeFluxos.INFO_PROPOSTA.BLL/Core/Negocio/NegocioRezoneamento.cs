using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using Iteris;
using Microsoft.SharePoint.Client;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioRezoneamento
    {
        public static void Rezonear()
        {
			List<EstruturaComercial_Modificada> estruturasComerciaisModificadas = new EstruturaComercial_Modificada().Consultar(_ => _.Processado == false);
			if (estruturasComerciaisModificadas == null || estruturasComerciaisModificadas.Count == 0)
			{
				DadosRezoneamento.ObterEstruturasComerciaisModificadas();
				estruturasComerciaisModificadas = new EstruturaComercial_Modificada().Consultar(_ => _.Processado == false);
			}
				
            
            List<Lista> listasRezoneamento = new Lista().Consultar(_ => _.Ambiente2007 == false);
            Usuario gerenteRegiaoAtual = null;
            Usuario gerenteTerritorioAtual = null;
            Usuario diretorVendasAtual = null;
            Usuario cdrAtual = null;
            Usuario gdrAtual = null;
            Boolean devQAS = PortalWeb.ContextoWebAtual.Url.ToLower().Contains("dev") || PortalWeb.ContextoWebAtual.Url.ToLower().Contains("qas");

            foreach (EstruturaComercial_Modificada estruturaComercial in estruturasComerciaisModificadas)
            {
                try
                {
                    #region [Obter Estrutura Comercial]

                    String mensagemObterEstrutura = String.Empty;
                    Boolean grCorreto = true;
                    Boolean gtCorreto = true;
                    Boolean dvCorreto = true;
                    Boolean cdrCorreto = true;
                    Boolean gdrCorreto = true;

                    gerenteRegiaoAtual = ObterResponsavel(estruturaComercial.GR, ref grCorreto, ref mensagemObterEstrutura);
                    gerenteTerritorioAtual = ObterResponsavel(estruturaComercial.GT, ref gtCorreto, ref mensagemObterEstrutura);
                    diretorVendasAtual = ObterResponsavel(estruturaComercial.DV, ref dvCorreto, ref mensagemObterEstrutura);
                    cdrAtual = ObterResponsavel(estruturaComercial.CDR, ref cdrCorreto, ref mensagemObterEstrutura);
                    gdrAtual = ObterResponsavel(estruturaComercial.GDR, ref gdrCorreto, ref mensagemObterEstrutura);

                    #endregion

                    Int32 _ibm = 0;
                    int.TryParse(estruturaComercial.IBM, out _ibm);
                    Int32 _siteCode = 0;
                    int.TryParse(estruturaComercial.SiteCode, out _siteCode);
                    Int32? ibm = _ibm; //Os filtros de busca devem ser nullables
                    Int32? siteCode = _siteCode;

                    foreach (Lista lista in listasRezoneamento)
                    {
                        List<EntidadePropostaSP> propostas = null;

                        if (ibm > 0) //A prioridade é rezonear por IBM
                        {
                            propostas = NegocioComum.ConsultarProposta(lista.CodigoLista, _ => _.Ibm == ibm);
                        }
                        else //Mas caso seja uma proposta sem IBM (Embandeiramento, NTI ou NTR), rezonear pelo Site Code.
                        {
                            if (siteCode > 0)
                            {
                                propostas = NegocioComum.ConsultarProposta(lista.CodigoLista, _ => _.SiteCode == siteCode);
                            }
                        }

                        if (propostas != null)
                        {
                            propostas.Where(_ => _.UtilizaZoneamentoPadrao == false).ToList().ForEach(proposta =>
                            {
                                proposta.GerenteTerritorio = gtCorreto && proposta.UtilizaZoneamentoGT == true ? gerenteTerritorioAtual : proposta.GerenteTerritorio;
                                proposta.GerenteRegiao = grCorreto && proposta.UtilizaZoneamentoGR == true ? gerenteRegiaoAtual : proposta.GerenteRegiao;
                                proposta.DiretorVendas = dvCorreto && proposta.UtilizaZoneamentoDiretor == true ? diretorVendasAtual : proposta.DiretorVendas;
                                proposta.Cdr = cdrCorreto && proposta.UtilizaZoneamentoCdr == true ? cdrAtual : proposta.Cdr;
                                proposta.Gdr = gdrCorreto && proposta.UtilizaZoneamentoGdr == true ? gdrAtual : proposta.Gdr;
                            });
                            propostas.Where(_ => _.UtilizaZoneamentoPadrao == true).ToList().ForEach(proposta =>
                            {
                                proposta.GerenteTerritorio = gtCorreto ? gerenteTerritorioAtual : proposta.GerenteTerritorio;
                                proposta.GerenteRegiao = grCorreto ? gerenteRegiaoAtual : proposta.GerenteRegiao;
                                proposta.DiretorVendas = dvCorreto ? diretorVendasAtual : proposta.DiretorVendas;
                                proposta.Cdr = cdrCorreto ? cdrAtual : proposta.Cdr;
                                proposta.Gdr = gdrCorreto ? gdrAtual : proposta.Gdr;
                            });
                            propostas.Atualizar(lista.CodigoLista);
                        }
                    }

                    estruturaComercial.Processado = true;
                    estruturaComercial.Atualizar();
                }
                catch (Exception ex)
                {
                    new Log().Inserir("NegocioRezoneamento", "Rezonear", ex);
                }
            }

            DadosRezoneamento.AtualizarEstruturaComercial();
            DadosRezoneamento.LimparEstruturaComercialModificada();
        }

		public static void Rezonear2()
		{
			List<Lista> listasRezoneamento = new Lista().Consultar(_ => _.Ambiente2007 == false);
			List<EstruturaComercial_Modificada> estruturasComerciaisModificadas = new EstruturaComercial_Modificada().Consultar(_ => _.Processado == false);

			List<EntidadePropostaSP> propostas = new List<EntidadePropostaSP>();

			foreach (Lista lista in listasRezoneamento)
			{
				List<EntidadePropostaSP> propostasLista = NegocioComum.ConsultarProposta(lista.CodigoLista, _ => _.Ibm != null || _.SiteCode != null);
				List<String> ibms = propostasLista
					.GroupBy(i => i.Ibm)
					.Select(p => p.First())
					.Where(i => i.Ibm != null)
					.Select(i => FormatarValor(i.Ibm.ToString())).ToList();
				List<String> siteCodes = propostasLista
						.GroupBy(i => i.SiteCode)
						.Select(p => p.First())
						.Where(i => i.SiteCode != null && (i.Ibm == null || i.Ibm == 0))
						.Select(s => FormatarValor(s.SiteCode.ToString())).ToList();

				List<EstruturaComercial_Modificada> estruturaProcessarIbm = estruturasComerciaisModificadas.Where(_ => ibms.Contains(_.IBM)).ToList();
				ProcessarEstruturaComercial(estruturaProcessarIbm, lista, propostasLista);
				List<EstruturaComercial_Modificada> estruturaProcessarSite = estruturasComerciaisModificadas.Where(_ => siteCodes.Contains(_.SiteCode)).ToList();
				ProcessarEstruturaComercial(estruturaProcessarSite, lista, propostasLista);
			}

			DadosRezoneamento.AtualizarEstruturaComercial();
			DadosRezoneamento.LimparEstruturaComercialModificada();
		}

        /// <summary>
        /// Atualiza a tabela EstruturaComercial_Salesforce com os dados encontrados no WS
        /// </summary>
        /// <param name="estruturasComerciaisSF_"></param>
        public static void AtualizarDadosSalesforce(List<DadosComercialSalesForce> estruturasComerciaisSF_)
        {
            DadosRezoneamento.LimparEstruturaComercialSalesForce();

            foreach (var estruturaComercial in estruturasComerciaisSF_)
            {
                try
                {
                    new EstruturaComercial_Salesforce()
                    {
                        IBM = estruturaComercial.Ibm,
                        SiteCode = estruturaComercial.SiteCode,
                        GT = estruturaComercial.GerenteTerritorio,
                        GR = estruturaComercial.GerenteRegiao,
                        DV = estruturaComercial.DiretorVendas,
                        CDR = estruturaComercial.Cdr,
                        GDR = estruturaComercial.Gdr
                    }.Inserir();
                }
                catch (Exception ex)
                {
                    new Log().Inserir(Origem.Servico, "AtualizarDadosSalesForce",
                        String.Format("IBM:{0},SiteCode:{1},GT:{2},GR:{3},DV:{4},CDR:{5},GDR:{6}"
                        , estruturaComercial.Ibm
                        , estruturaComercial.SiteCode
                        , estruturaComercial.GerenteTerritorio
                        , estruturaComercial.GerenteRegiao
                        , estruturaComercial.DiretorVendas
                        , estruturaComercial.Cdr
                        , estruturaComercial.Gdr), ex);
                }

            }
        }

        public static Usuario ObterResponsavel(String email, ref Boolean usuarioValido, ref String mensagem)
        {
            Usuario usuario = PortalWeb.ContextoWebAtual.BuscarUsuarioPorEmail(email, true,false);

            if (usuario ==  null)
            {
                usuarioValido = false;
                mensagem += "Email Incorreto:" + email + ";";
            }

            return usuario;
        }

		#region [Rezonear2]

		public static void ProcessarEstruturaComercial(List<EstruturaComercial_Modificada> estruturaComercialSales
			, Lista lista
			, List<EntidadePropostaSP> propostasLista)
		{
			Usuario gerenteRegiaoAtual = null;
			Usuario gerenteTerritorioAtual = null;
			Usuario diretorVendasAtual = null;
			Usuario cdrAtual = null;
			Usuario gdrAtual = null;

			foreach (EstruturaComercial_Modificada estruturaComercial in estruturaComercialSales)
			{
				try
				{
					#region [Obter Estrutura Comercial]

					String mensagemObterEstrutura = String.Empty;
					Boolean grCorreto = true;
					Boolean gtCorreto = true;
					Boolean dvCorreto = true;
					Boolean cdrCorreto = true;
					Boolean gdrCorreto = true;

					gerenteRegiaoAtual = ObterResponsavel(estruturaComercial.GR, ref grCorreto, ref mensagemObterEstrutura);
					gerenteTerritorioAtual = ObterResponsavel(estruturaComercial.GT, ref gtCorreto, ref mensagemObterEstrutura);
					diretorVendasAtual = ObterResponsavel(estruturaComercial.DV, ref dvCorreto, ref mensagemObterEstrutura);
					cdrAtual = ObterResponsavel(estruturaComercial.CDR, ref cdrCorreto, ref mensagemObterEstrutura);
					gdrAtual = ObterResponsavel(estruturaComercial.GDR, ref gdrCorreto, ref mensagemObterEstrutura);

					#endregion

					#region [SiteCode/IBM]
					Int32 _ibm = 0;
					int.TryParse(estruturaComercial.IBM, out _ibm);
					Int32 _siteCode = 0;
					int.TryParse(estruturaComercial.SiteCode, out _siteCode);
					Int32? ibm = _ibm; //Os filtros de busca devem ser nullables
					Int32? siteCode = _siteCode;
					#endregion

					List<EntidadePropostaSP> propostasFiltradas = null;
					if (ibm > 0) //A prioridade é rezonear por IBM
						propostasFiltradas = propostasLista.Where(_ => _.Ibm == ibm).ToList();
					else if (siteCode > 0)//Mas caso seja uma proposta sem IBM (Embandeiramento, NTI ou NTR), rezonear pelo Site Code.
						propostasFiltradas = propostasLista.Where(_ => _.SiteCode == siteCode).ToList();

					AtualizarEstruturaComercial(estruturaComercial, propostasFiltradas, lista
						, grCorreto
						, gerenteRegiaoAtual
						, gtCorreto
						, gerenteTerritorioAtual
						, dvCorreto
						, diretorVendasAtual
						, cdrCorreto
						, cdrAtual
						, gdrCorreto
						, gdrAtual);

					estruturaComercial.Processado = true;
					estruturaComercial.Atualizar();
				}
				catch (Exception ex)
				{
					new Log().Inserir("NegocioRezoneamento", "Rezonear", ex);
				}
			}
		}

		public static void AtualizarEstruturaComercial(EstruturaComercial_Modificada estruturaComercial
			, List<EntidadePropostaSP> propostas
			, Lista lista
			, Boolean grCorreto
			, Usuario gerenteRegiaoAtual
			, Boolean gtCorreto
			, Usuario gerenteTerritorioAtual
			, Boolean dvCorreto
			, Usuario diretorVendasAtual
			, Boolean cdrCorreto
			, Usuario cdrAtual
			, Boolean gdrCorreto
			, Usuario gdrAtual)
		{
			if (propostas != null)
			{
				propostas.Where(_ => _.UtilizaZoneamentoPadrao == false).ToList().ForEach(proposta =>
				{
					proposta.GerenteTerritorio = gtCorreto && proposta.UtilizaZoneamentoGT == true ? gerenteTerritorioAtual : proposta.GerenteTerritorio;
					proposta.GerenteRegiao = grCorreto && proposta.UtilizaZoneamentoGR == true ? gerenteRegiaoAtual : proposta.GerenteRegiao;
					proposta.DiretorVendas = dvCorreto && proposta.UtilizaZoneamentoDiretor == true ? diretorVendasAtual : proposta.DiretorVendas;
					proposta.Cdr = cdrCorreto && proposta.UtilizaZoneamentoCdr == true ? cdrAtual : proposta.Cdr;
					proposta.Gdr = gdrCorreto && proposta.UtilizaZoneamentoGdr == true ? gdrAtual : proposta.Gdr;
				});
				propostas.Where(_ => _.UtilizaZoneamentoPadrao == true).ToList().ForEach(proposta =>
				{
					proposta.GerenteTerritorio = gtCorreto ? gerenteTerritorioAtual : proposta.GerenteTerritorio;
					proposta.GerenteRegiao = grCorreto ? gerenteRegiaoAtual : proposta.GerenteRegiao;
					proposta.DiretorVendas = dvCorreto ? diretorVendasAtual : proposta.DiretorVendas;
					proposta.Cdr = cdrCorreto ? cdrAtual : proposta.Cdr;
					proposta.Gdr = gdrCorreto ? gdrAtual : proposta.Gdr;
				});
				propostas.Atualizar(lista.CodigoLista);
			}
		}
		
		public static string FormatarValor(string ibmSitecode_)
		{
			string valorFormatado = ibmSitecode_;

			if (!String.IsNullOrEmpty(ibmSitecode_))
			{
				for (int i = 0; i < (10 - ibmSitecode_.Length); i++)
				{
					valorFormatado = String.Concat("0", valorFormatado);
				}
			}

			return valorFormatado;
		}

		#endregion
    }
}
