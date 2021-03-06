USE [PortalDeFluxo]
GO
/****** Object:  Table [dbo].[ListaFormularioTeste]    Script Date: 15/02/2017 13:19:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ListaFormularioTeste](
	[CodigoItem] [int] NOT NULL,
	[CodigoLista] [uniqueidentifier] NOT NULL,
	[LoginInclusao] [varchar](255) NOT NULL,
	[DataInclusao] [datetime] NOT NULL,
	[LoginAlteracao] [varchar](255) NULL,
	[DataAlteracao] [datetime] NULL,
	[LoginInclusaoSP] [varchar](255) NULL,
	[DataInclusaoSP] [datetime] NULL,
	[LoginAlteracaoSP] [varchar](255) NULL,
	[DataAlteracaoSP] [datetime] NULL,
	[Ativo] [bit] NOT NULL,
	[DescricaoRazaoSocial] [varchar](255) NULL,
	[NumeroIBM] [int] NULL,
	[NumeroSiteCode] [int] NULL,
	[DescricaoEstadoAtualFluxo] [varchar](255) NULL,
	[BuscaDocumentos] [bit] NULL,
	[ContratoPadrao] [bit] NULL,
	[LoginGerenteTerritorio] [varchar](255) NULL,
	[LoginGerenteRegiao] [varchar](255) NULL,
	[LoginDiretorVendas] [varchar](255) NULL,
	[LoginCDR] [varchar](255) NULL,
	[LoginGDR] [varchar](255) NULL,
	[DescricaoEtapa] [varchar](255) NULL,
	[UtilizaZoneamentoPadrao] [bit] NULL,
	[UtilizaZoneamentoCdr] [bit] NULL,
	[UtilizaZoneamentoDiretor] [bit] NULL,
	[UtilizaZoneamentoGdr] [bit] NULL,
	[UtilizaZoneamentoGR] [bit] NULL,
	[UtilizaZoneamentoGT] [bit] NULL,
	[LoginFormularioTeste2] [varchar](255) NULL,
	[NumeroDecimal] [decimal](13, 5) NULL,
	[NumeroInt] [int] NULL,
	[FormValor1] [decimal](13, 5) NULL,
	[FormValor2] [decimal](13, 2) NULL,
	[IdFormularioTesteTipoProposta] [int] NULL,
 CONSTRAINT [PK_ListaFormularioTeste] PRIMARY KEY CLUSTERED 
(
	[CodigoItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ListaFormularioTeste]  WITH CHECK ADD  CONSTRAINT [FK_ListaFormularioTeste_FormularioTesteTipoProposta] FOREIGN KEY([IdFormularioTesteTipoProposta])
REFERENCES [dbo].[FormularioTesteTipoProposta] ([IdFormularioTesteTipoProposta])
GO
ALTER TABLE [dbo].[ListaFormularioTeste] CHECK CONSTRAINT [FK_ListaFormularioTeste_FormularioTesteTipoProposta]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID do item no Sharepoint' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ListaFormularioTeste', @level2type=N'COLUMN',@level2name=N'CodigoItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID da lista no Shaepoint' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ListaFormularioTeste', @level2type=N'COLUMN',@level2name=N'CodigoLista'
GO
