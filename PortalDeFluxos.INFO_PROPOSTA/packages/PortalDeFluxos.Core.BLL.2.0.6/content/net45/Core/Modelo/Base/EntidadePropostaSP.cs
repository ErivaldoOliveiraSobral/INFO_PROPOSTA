using PortalDeFluxos.Core.BLL.Atributos;
using System;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [Serializable]
    public class EntidadePropostaSP : EntidadeSP
    {
        [InternalName("Title")]
        public String NumeroSolicitacao { get; set; }

        /// <summary>Razão Social</summary>
        [InternalName("RazaoSocial")]
        public String RazaoSocial { get; set; }

        /// <summary>Ibm da proposta</summary>
		[InternalName("IBM")]
		public Int32? Ibm { get; set; }

        /// <summary>SiteCode da proposta</summary>
        [InternalName("SiteCode")]
        public Int32? SiteCode { get; set; }

        ///<summary>Etapa atual</summary>
        [InternalName("Etapa")]
        public String Etapa { get; set; }

        /// <summary>Estado do fluxo - Utilizado para reiniciar o fluxo na etapa certa</summary>
        [InternalName("EstadoAtualFluxo")]
        public String EstadoAtualFluxo { get; set; }

        /// <summary>Utilizado no fluxo - decidir se vai para etapa busca de documentos</summary>
        [InternalName("BuscaDocumentos")]
        public Boolean BuscaDocumentos { get; set; }

        /// <summary>Utilizado no fluxo - decidir se é um contrato padrão</summary>
        [InternalName("ContratoPadrao")]
        public Boolean ContratoPadrao { get; set; }

        /// <summary>Utilizado no rezoneamento - decidir se é automatico (se estiver marcado ignora os individuais)</summary>
        [InternalName("UtilizaZoneamentoPadrao")]
        public Boolean UtilizaZoneamentoPadrao { get; set; }
        
        /// <summary>Estrutura comercial - GT</summary>
        [InternalName("GerenteTerritorio")]
        public Usuario GerenteTerritorio { get; set; }

        /// <summary>Estrutura comercial - GR</summary>
        [InternalName("GerenteRegiao")]
        public Usuario GerenteRegiao { get; set; }

        /// <summary>Estrutura comercial - Diretor de Vendas</summary>
        [InternalName("DiretorVendas")]
        public Usuario DiretorVendas { get; set; }

        /// <summary>Estrutura comercial - CDR</summary>
        [InternalName("CDR")]
        public Usuario Cdr { get; set; }

        /// <summary>Estrutura comercial - Gdr</summary>
        [InternalName("GDR")]
        public Usuario Gdr { get; set; }

        /// <summary>Utilizado no rezoneamento GT- decidir se é automatico</summary>
        [InternalName("UtilizaZoneamentoGT")]
        public Boolean UtilizaZoneamentoGT { get; set; }

        /// <summary>Utilizado no rezoneamento GR- decidir se é automatico</summary>
        [InternalName("UtilizaZoneamentoGR")]
        public Boolean UtilizaZoneamentoGR { get; set; }

        /// <summary>Utilizado no rezoneamento Diretor- decidir se é automatico</summary>
        [InternalName("UtilizaZoneamentoDiretor")]
        public Boolean UtilizaZoneamentoDiretor { get; set; }

        /// <summary>Utilizado no rezoneamento Cdr - decidir se é automatico</summary>
        [InternalName("UtilizaZoneamentoCdr")]
        public Boolean UtilizaZoneamentoCdr { get; set; }

        /// <summary>Utilizado no rezoneamento Gdr- decidir se é automatico</summary>
        [InternalName("UtilizaZoneamentoGdr")]
        public Boolean UtilizaZoneamentoGdr { get; set; }

		public String ContentType { get; set; }
	}
}
