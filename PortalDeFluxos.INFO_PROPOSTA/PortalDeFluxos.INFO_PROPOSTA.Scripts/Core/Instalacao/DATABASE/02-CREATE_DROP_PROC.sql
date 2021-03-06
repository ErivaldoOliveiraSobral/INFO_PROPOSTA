USE [PortalDeFluxo]
GO
/****** Object:  StoredProcedure [dbo].[spObterTituloSolicitacao]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spObterTituloSolicitacao]
GO
/****** Object:  StoredProcedure [dbo].[spObterEstruturasComerciaisModificadas]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spObterEstruturasComerciaisModificadas]
GO
/****** Object:  StoredProcedure [dbo].[spObterDocumentosProposta]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spObterDocumentosProposta]
GO
/****** Object:  StoredProcedure [dbo].[spObterDocumentos]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spObterDocumentos]
GO
/****** Object:  StoredProcedure [dbo].[spLimparLog]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spLimparLog]
GO
/****** Object:  StoredProcedure [dbo].[spLimparEstruturaComercialSalesForce]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spLimparEstruturaComercialSalesForce]
GO
/****** Object:  StoredProcedure [dbo].[spLimparEstruturaComercialModificada]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spLimparEstruturaComercialModificada]
GO
/****** Object:  StoredProcedure [dbo].[spEscalonarTarefas]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spEscalonarTarefas]
GO
/****** Object:  StoredProcedure [dbo].[spDelegarTarefa]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spDelegarTarefa]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarTarefasRealizadas]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarTarefasRealizadas]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarTarefasPendentes]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarTarefasPendentes]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarMinhasTarefas]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarMinhasTarefas]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarLog]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarLog]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarLembretesPendentes]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarLembretesPendentes]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarInstanciaFluxo]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarInstanciaFluxo]
GO
/****** Object:  StoredProcedure [dbo].[spConsultarEmailsPendentes]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spConsultarEmailsPendentes]
GO
/****** Object:  StoredProcedure [dbo].[spAtualizarEstruturaComercial]    Script Date: 17/11/2016 18:06:10 ******/
DROP PROCEDURE [dbo].[spAtualizarEstruturaComercial]
GO
/****** Object:  StoredProcedure [dbo].[spAtualizarEstruturaComercial]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spAtualizarEstruturaComercial]
AS
BEGIN

	BEGIN
		DELETE FROM [dbo].[EstruturaComercial]  
		WHERE IBM NOT IN (
			SELECT 
				IBM 
			FROM EstruturaComercial_Modificada
			WHERE Processado = 0) 
	END
	
	BEGIN
		INSERT INTO
			[dbo].[EstruturaComercial]
		SELECT
			ec_salesforce.IBM,
			ec_salesforce.SiteCode,
			ec_salesforce.GT,
			ec_salesforce.GR,
			ec_salesforce.DV,
			ec_salesforce.CDR,
			ec_salesforce.GDR			
		FROM 
			[dbo].[EstruturaComercial_Salesforce] ec_salesforce WITH(NOLOCK)
		EXCEPT
			SELECT 
				ec_local.IBM AS 'IBM',
				ec_local.SiteCode AS 'SiteCode',
				ec_local.GT AS 'GT',
				ec_local.GR AS 'GR',
				ec_local.DV AS 'DV',
				ec_local.CDR AS 'CDR',
				ec_local.GDR AS 'GDR'
			FROM 
				EstruturaComercial_Modificada ec_local WITH(NOLOCK)
			WHERE ec_local.Processado = 0
			
	END

END
GO
/****** Object:  StoredProcedure [dbo].[spConsultarEmailsPendentes]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spConsultarEmailsPendentes]
AS
SET NOCOUNT ON

	SELECT 
		t.IdTarefa,
		t.EmailResponsavel,
		t.DescricaoAssuntoEmail,
		t.DescricaoMensagemEmail,
		CASE WHEN t.CopiarSuperior = 1 THEN EmailSuperior ELSE null END as EmailSuperior,
		CASE WHEN t.EnviarPdf = 1 THEN EnviarPdf ELSE null END as EnviarPdf
	FROM	
		Tarefa t
		INNER JOIN InstanciaFluxo i ON t.IdInstanciaFluxo = i.IdInstanciaFluxo
		INNER JOIN Lista l			ON i.CodigoLista = l.CodigoLista
	WHERE
		--Busca tarefas não completas, não escalonadas que passaram da data de escalonar
		t.TarefaCompleta = 0
		AND t.EmailEnviado = 0
		--Somente busca se possuir e-mail
		AND t.EmailResponsavel is not null
		AND isnull(t.EmailResponsavel,'') <> ''
		AND i.StatusFluxo  = 1	-- Somente trata fluxos Em Andamento
		AND l.Ambiente2007 = 0  -- Somente trata listas do 2013
		AND t.Ativo = 1
		AND i.Ativo = 1

SET NOCOUNT OFF

GO
/****** Object:  StoredProcedure [dbo].[spConsultarInstanciaFluxo]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spConsultarInstanciaFluxo] 
	@filtro varchar(200) = NULL,
	@filtro2 varchar(200) = NULL,
	@login	 varchar(200) = NULL,
	@statusFluxo INT = NULL,
	@indicePagina INT = 0,
	@registrosPorPagina INT = 1000,
	@ordernarPor VARCHAR(100) = NULL,
	@ordernarDirecao VARCHAR(4) = 'ASC',
	@isAdmin bit = 0
AS
BEGIN
	
SET NOCOUNT ON;

SELECT	@filtro  = CASE WHEN  @filtro IS NOT NULL THEN '%' + @filtro + '%' ELSE @filtro END,
		@filtro2  = CASE WHEN @filtro2 IS NOT NULL THEN '%' + @filtro2 + '%' ELSE @filtro2 END,
		@login  = CASE WHEN @login IS NOT NULL THEN @login ELSE '-' END;

-------------------------	PERMISSÃO	----------------------------------------------------
--------------------------------------------------------------------------------------------

DECLARE  @PermissaoSujaFinal TABLE
 ( 
	Escopo	varchar(300), 
	Grupo	varchar(300) 
 ) 

DECLARE @PermissaoFinal  TABLE
 ( 
	Grupo	varchar(200) ,
	List	varchar(200),
	ID		Int,
	UNIQUE NONCLUSTERED (List, ID, Grupo)
 ) 
 IF(@isAdmin = 0)
BEGIN
																			DECLARE	@LinkedServer					nvarchar(500)
,	@OPENQUERY						nvarchar(MAX)
,	@TSQLPermissao					varchar(MAX) 
,	@GroupMembership				varchar(500)
,	@Groups							varchar(500)
,	@UserInfo						varchar(500)
,	@Perms							varchar(500)
,	@Roles							varchar(500)
,	@RoleAssignment					varchar(500)
,	@permissaoGrupo					int

	SELECT @LinkedServer		= Valor FROM Parametro WHERE IdParametro = 2; 
	SELECT @UserInfo			= Valor FROM Parametro WHERE IdParametro = 4;

	SELECT @GroupMembership		= Valor FROM Parametro WHERE IdParametro = 27; 
	SELECT @Groups				= Valor FROM Parametro WHERE IdParametro = 28;
	SELECT @Perms				= Valor FROM Parametro WHERE IdParametro = 29; 
	SELECT @Roles				= Valor FROM Parametro WHERE IdParametro = 30;
	SELECT @RoleAssignment		= Valor FROM Parametro WHERE IdParametro = 31;

	--####### Define os dados das tabelas que serão utilizadas #######
	SET @OPENQUERY			  = 'SELECT * FROM OPENQUERY('+ @LinkedServer + ','''

																																																																																							SET @TSQLPermissao  = REPLACE (REPLACE (REPLACE (REPLACE (REPLACE (REPLACE (REPLACE ('
SELECT 
	DISTINCT 
		ScopeUrl 
	,	g.Title  
	FROM (
		SELECT
			PrincipalId 
		,	ra.RoleId 
		,	ra.ScopeId 
		,	ScopeUrl 
		FROM			@RoleAssignment	ra	with (nolock) 
		LEFT OUTER JOIN @Roles			r	with (nolock) 
		ON				ra.RoleId = r.RoleId 
		LEFT OUTER JOIN @Perms			p	with (nolock) 
		ON				ra.ScopeId = p.ScopeId 
	) ttScope 
	LEFT OUTER JOIN		@GroupMembership gm with (nolock) 
	ON					gm.GroupId = ttScope.PrincipalId 
	LEFT OUTER JOIN		@Groups g with (nolock) 
	ON					gm.GroupId = g.ID 
	LEFT OUTER JOIN		@UserInfo ui with (nolock) 
	ON					gm.MemberId = ui.tp_ID 
	WHERE				ui.tp_Login = ''''@login''''
UNION
SELECT 
		DISTINCT ScopeUrl 
	,	''''USER''''
	FROM (
		SELECT
			PrincipalId 
		,	ra.RoleId 
		,	ra.ScopeId 
		,	ScopeUrl 
		FROM			@RoleAssignment ra	with (nolock) 
		LEFT OUTER JOIN @Roles			r	with (nolock) 
		ON				ra.RoleId = r.RoleId 
		LEFT OUTER JOIN @Perms			p with (nolock) 
		ON				ra.ScopeId = p.ScopeId 
	) ttScope  
	LEFT OUTER JOIN		@UserInfo		ui with (nolock) 
	ON					ttScope.PrincipalId = ui.tp_ID 
	WHERE 
	ui.tp_Login = ''''@login'''' ;'')'
	,'@login', @login)
	,'@RoleAssignment',@RoleAssignment)
	,'@Roles',@Roles)
	,'@Perms',@Perms)
	,'@GroupMembership',@GroupMembership)
	,'@Groups',@Groups)
	,'@UserInfo',@UserInfo);

	INSERT INTO @PermissaoSujaFinal
	EXEC (@OPENQUERY+@TSQLPermissao) ;

	UPDATE @PermissaoSujaFinal SET Grupo = NULL WHERE Escopo IS NOT NULL AND Escopo <> '';

										INSERT INTO @PermissaoFinal
SELECT 
		GRUPO
	,	SUBSTRING(Escopo,0,CHARINDEX('/',Escopo,(charindex('/',Escopo)+1)))	AS	List
	,	LTRIM(RTRIM(REPLACE(REPLACE(Escopo,SUBSTRING(Escopo,0,CHARINDEX('/',Escopo,(CHARINDEX('/',Escopo)+1))) + '/',''),'_.000','')))	AS	ID 
FROM @PermissaoSujaFinal
GROUP BY Escopo,Grupo
HAVING	
		GRUPO IS NOT NULL
	OR (SUBSTRING(Escopo,0,CHARINDEX('/',Escopo,(CHARINDEX('/',Escopo)+1))) <> '');

	SET @permissaoGrupo = (SELECT COUNT(1) FROM @PermissaoFinal WHERE UPPER(Grupo)='COORDENADOR' OR UPPER(Grupo)='VISUALIZAÇÃO FULL');	
END
--------------------------------------------------------------------------------------------
-------------------------	PERMISSÃO	----------------------------------------------------
--------------------------------------------------------------------------------------------


SELECT
	FinalTable.CodigoLista,
	FinalTable.CodigoItem,
	FinalTable.NomeSolicitacao,
	FinalTable.NomeFluxo,
	FinalTable.NomeSolicitante,
	FinalTable.NomeEtapa,
	FinalTable.StatusFluxo,
	FinalTable.TotalRecordCount,
	FinalTable.DescricaoUrlItem,
	FinalTable.NomeDiretorVendas,
	FinalTable.NomeGerenteRegiao
FROM (
	SELECT 
		CASE @ordernarDirecao
			WHEN 'ASC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
							WHEN 'NomeFluxo' THEN inst.NomeFluxo
							WHEN 'NomeSolicitante' THEN inst.NomeSolicitante
							WHEN 'NomeEtapa' THEN inst.NomeEtapa
							WHEN 'NomeGerenteRegiao' THEN inst.NomeGerenteRegiao
							WHEN 'NomeDiretorVendas' THEN inst.NomeDiretorVendas
							WHEN 'StatusFluxo' THEN CASE 
														WHEN inst.StatusFluxo = 1 THEN 'Em andamento'
														WHEN inst.StatusFluxo = 2 THEN 'Erro'
														WHEN inst.StatusFluxo = 3 THEN 'Cancelado'
														WHEN inst.StatusFluxo = 4 THEN 'Finalizado'
													END
							ELSE inst.NomeSolicitacao
						END ASC
				) 
			WHEN 'DESC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
							WHEN 'NomeFluxo' THEN inst.NomeFluxo
							WHEN 'NomeSolicitante' THEN inst.NomeSolicitante
							WHEN 'NomeEtapa' THEN inst.NomeEtapa
							WHEN 'NomeSolicitante' THEN inst.NomeSolicitante
							WHEN 'NomeEtapa' THEN inst.NomeEtapa
							WHEN 'NomeGerenteRegiao' THEN inst.NomeGerenteRegiao
							WHEN 'NomeDiretorVendas' THEN inst.NomeDiretorVendas
							WHEN 'StatusFluxo' THEN CASE 
														WHEN inst.StatusFluxo = 1 THEN 'Em andamento'
														WHEN inst.StatusFluxo = 2 THEN 'Erro'
														WHEN inst.StatusFluxo = 3 THEN 'Cancelado'
														WHEN inst.StatusFluxo = 4 THEN 'Finalizado'
													END
							ELSE inst.NomeSolicitacao
						END DESC
				)
		END AS [Index],
		inst.CodigoLista,
		inst.CodigoItem,
		inst.NomeSolicitacao,
		inst.NomeFluxo,
		inst.NomeSolicitante,
		inst.NomeEtapa,
		inst.NomeGerenteRegiao,
		inst.NomeDiretorVendas,
		CASE 
			WHEN inst.StatusFluxo = 1 THEN 'Em andamento'
			WHEN inst.StatusFluxo = 2 THEN 'Erro'
			WHEN inst.StatusFluxo = 3 THEN 'Cancelado'
			WHEN inst.StatusFluxo = 4 THEN 'Finalizado'
		END AS StatusFluxo,
		CONCAT(list.DescricaoUrlItem, inst.CodigoItem) DescricaoUrlItem,
		COUNT(*) OVER () AS TotalRecordCount
	FROM
		[InstanciaFluxo] inst
		INNER JOIN
		[Lista]				AS	list
		ON	list.CodigoLista	=	inst.CodigoLista
	WHERE 
		(
			(@filtro IS NULL) OR 
			(inst.NomeSolicitacao  LIKE @filtro) OR
			(inst.NomeFluxo	LIKE @filtro) OR
			(inst.NomeSolicitante  LIKE @filtro) OR
			(inst.NomeDiretorVendas	LIKE @filtro) OR
			(inst.NomeGerenteRegiao	LIKE @filtro) OR
			(inst.NomeEtapa LIKE @filtro) OR
			(
				(CASE 
					WHEN inst.StatusFluxo = 1 THEN 'Em andamento'
					WHEN inst.StatusFluxo = 2 THEN 'Erro'
					WHEN inst.StatusFluxo = 3 THEN 'Cancelado'
					WHEN inst.StatusFluxo = 4 THEN 'Finalizado'
				 END) LIKE @filtro
			)
		) AND
		(
			(@filtro2 IS NULL) OR 
			(inst.NomeSolicitacao  LIKE @filtro2) OR
			(inst.NomeFluxo	LIKE @filtro2) OR
			(inst.NomeSolicitante  LIKE @filtro2) OR
			(inst.NomeDiretorVendas	LIKE @filtro2) OR
			(inst.NomeGerenteRegiao  LIKE @filtro2) OR
			(inst.NomeEtapa LIKE @filtro2) OR
			(
				(CASE 
					WHEN inst.StatusFluxo = 1 THEN 'Em andamento'
					WHEN inst.StatusFluxo = 2 THEN 'Erro'
					WHEN inst.StatusFluxo = 3 THEN 'Cancelado'
					WHEN inst.StatusFluxo = 4 THEN 'Finalizado'
				 END) LIKE @filtro2
			)
		) AND
		(inst.StatusFluxo = @statusFluxo OR @statusFluxo IS NULL)
		AND inst.Ativo = 1

		AND (		
					@isAdmin = 1 OR inst.LoginSolicitante = @Login OR @permissaoGrupo > 0 OR EXISTS(
					SELECT 1 FROM @PermissaoFinal	
					WHERE ID		=	inst.CodigoItem AND	List	=	list.DescricaoUrlLista
					)
			)
	) FinalTable
WHERE
	[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
	@indicePagina IS NULL OR
	@registrosPorPagina IS NULL
ORDER BY [Index];


END


GO
/****** Object:  StoredProcedure [dbo].[spConsultarLembretesPendentes]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spConsultarLembretesPendentes]
AS
SET NOCOUNT ON

	SELECT 
		t.IdTarefa,
		lb.IdLembrete,
		lb.EmailPara,
		lb.DescricaoAssunto,
		lb.DescricaoMensagem,
		CASE WHEN lb.CopiarSuperior = 1 THEN t.EmailSuperior ELSE null END as EmailSuperior,
		CASE WHEN t.EnviarPdf = 1 THEN t.EnviarPdf ELSE null END as EnviarPdf
	FROM	
		Lembrete lb
		INNER JOIN Tarefa t			ON lb.IdTarefa = t.IdTarefa
		INNER JOIN InstanciaFluxo i ON t.IdInstanciaFluxo = i.IdInstanciaFluxo
		INNER JOIN Lista l			ON i.CodigoLista = l.CodigoLista
	WHERE
		--Busca tarefas não completas
		t.TarefaCompleta = 0
		-- Que o lembrete não foi enviado
		AND lb.EmailEnviado = 0
		AND lb.DataEnvio < getdate()
		AND lb.Ativo = 1
		--Somente busca se possuir e-mail
		AND lb.EmailPara is not null
		AND isnull(lb.EmailPara,'') <> ''
		AND i.StatusFluxo  = 1	-- Somente trata fluxos Em Andamento
		AND l.Ambiente2007 = 0  -- Somente trata listas do 2013
		AND t.Ativo = 1
		AND i.Ativo = 1

SET NOCOUNT OFF

GO
/****** Object:  StoredProcedure [dbo].[spConsultarLog]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spConsultarLog]
 
	@indicePagina INT = 0,
	@registrosPorPagina INT = 1000,
	@ordernarPor VARCHAR(100) = NULL,
	@ordernarDirecao VARCHAR(4) = 'DESC'
AS
BEGIN
	
SET NOCOUNT ON;

SELECT
	FinalTable.IdLog,
	FinalTable.NomeProcesso,
	FinalTable.DescricaoOrigem,
	FinalTable.DescricaoMensagem,
	FinalTable.DescricaoDetalhe,
	FinalTable.LoginInclusao,
	FinalTable.DataInclusao,
	FinalTable.Erro,
	FinalTable.TotalRecordCount 
FROM (
	SELECT 
		CASE @ordernarDirecao
			WHEN 'ASC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeProcesso' THEN  l.NomeProcesso
							WHEN 'DescricaoOrigem' THEN l.DescricaoOrigem
							WHEN 'DescricaoMensagem' THEN l.DescricaoMensagem
							WHEN 'DescricaoDetalhe' THEN l.DescricaoDetalhe
							WHEN 'Erro' THEN CASE 
														WHEN l.Erro = 0 THEN '0'
														WHEN l.Erro = 1 THEN '1'
													END
							WHEN 'LoginInclusao' THEN l.LoginInclusao
							WHEN 'DataInclusao ' THEN FORMAT(l.DataInclusao ,'dd/MM/yyyy HH:mm:ss')
							ELSE RIGHT(REPLICATE('0', 30) + CONVERT(VARCHAR, CAST(l.IdLog AS INT)), 30)
						END ASC
				) 
			WHEN 'DESC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeProcesso' THEN  l.NomeProcesso
							WHEN 'DescricaoOrigem' THEN l.DescricaoOrigem
							WHEN 'DescricaoMensagem' THEN l.DescricaoMensagem
							WHEN 'DescricaoDetalhe' THEN l.DescricaoDetalhe
							WHEN 'Erro' THEN CASE 
														WHEN l.Erro = 0 THEN '0'
														WHEN l.Erro = 1 THEN '1'
													END
							WHEN 'LoginInclusao' THEN l.LoginInclusao
							WHEN 'DataInclusao ' THEN FORMAT(l.DataInclusao ,'dd/MM/yyyy HH:mm:ss')
							ELSE RIGHT(REPLICATE('0', 30) + CONVERT(VARCHAR, CAST(l.IdLog AS INT)), 30)
						END DESC
				)
		END AS [Index],	
		l.IdLog,
		l.NomeProcesso,
		l.DescricaoOrigem,
		l.DescricaoMensagem,
		l.DescricaoDetalhe,
		l.LoginInclusao,
		l.DataInclusao, 
		l.Erro,
		COUNT(*) OVER () AS TotalRecordCount
	FROM
		[Log] l
	) FinalTable
WHERE
	[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
	@indicePagina IS NULL OR
	@registrosPorPagina IS NULL
ORDER BY [Index];


END



GO
/****** Object:  StoredProcedure [dbo].[spConsultarMinhasTarefas]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spConsultarMinhasTarefas] 
	@filtro VARCHAR(200) = NULL,
	@login					varchar(500) = NULL,
	@indicePagina			INT = 0,
	@registrosPorPagina		INT = 1000,
	@ordernarPor			VARCHAR(100) = NULL,
	@ordernarDirecao		VARCHAR(4) = 'ASC'
AS
BEGIN
	
SET NOCOUNT ON;

SELECT	@filtro  = CASE WHEN  @filtro IS NOT NULL THEN '%' + @filtro + '%' ELSE @filtro END;

SELECT
	FinalTable.IdTarefa,
	FinalTable.CodigoLista,
	FinalTable.CodigoItem,
	FinalTable.NomeSolicitacao,
	FinalTable.NomeFluxo,
	FinalTable.NomeSolicitante,
	FinalTable.NomeEtapa,
	FinalTable.NomeTarefa,
	FinalTable.NomeDiretorVendas,
	FinalTable.NomeGerenteRegiao,
	FinalTable.TotalRecordCount,
	FinalTable.DescricaoUrlItem,
	[dbo].fnConvertDataPainel(DataInclusao) AS TempoDecorrido,
	FinalTable.DescricaoUrlTarefa,
	FinalTable.Ambiente2007
FROM (
	SELECT
		CASE @ordernarDirecao
			WHEN 'ASC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
							WHEN 'NomeFluxo' THEN inst.NomeFluxo
							WHEN 'NomeSolicitante' THEN inst.NomeSolicitante
							WHEN 'NomeTarefa' THEN tar.NomeTarefa
							WHEN 'TempoDecorrido' THEN CONVERT(VARCHAR, tar.DataInclusao, 126)
							WHEN 'NomeDiretorVendas' THEN inst.NomeDiretorVendas
							WHEN 'NomeGerenteRegiao' THEN inst.NomeGerenteRegiao
							ELSE  CONVERT(VARCHAR, tar.DataInclusao, 126)
						END ASC
				) 
			WHEN 'DESC' THEN
				ROW_NUMBER() OVER (
					ORDER BY
						CASE @ordernarPor
							WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
							WHEN 'NomeFluxo' THEN inst.NomeFluxo
							WHEN 'NomeSolicitante' THEN inst.NomeSolicitante
							WHEN 'NomeTarefa' THEN tar.NomeTarefa
							WHEN 'TempoDecorrido' THEN CONVERT(VARCHAR, tar.DataInclusao, 126)
							WHEN 'NomeDiretorVendas' THEN inst.NomeDiretorVendas
							WHEN 'NomeGerenteRegiao' THEN inst.NomeGerenteRegiao
							ELSE CONVERT(VARCHAR, tar.DataInclusao, 126)
						END DESC
				)
		END AS [Index],
		tar.IdTarefa,
		inst.CodigoLista,
		inst.CodigoItem,
		inst.NomeSolicitacao,
		inst.NomeFluxo,
		inst.NomeSolicitante,
		inst.NomeEtapa,
		inst.NomeDiretorVendas,
		inst.NomeGerenteRegiao,
		tar.NomeTarefa,
		tar.DataInclusao,
		list.Ambiente2007,
		CONCAT(list.DescricaoUrlItem, inst.CodigoItem) DescricaoUrlItem,
		CASE 
			WHEN list.Ambiente2007 = 1	THEN CONCAT(list.DescricaoUrlTarefa,'?List=', tar.CodigoListaTarefa,'&Id=', tar.CodigoTarefaSP) 
		ELSE CONCAT(list.DescricaoUrlTarefa, tar.IdTarefa,'&Id=',inst.CodigoItem,tar.ParametrosUrl)
		END DescricaoUrlTarefa,
		COUNT(*) OVER () AS TotalRecordCount
	FROM		
		[InstanciaFluxo]	AS	inst
		INNER JOIN	
		(
			SELECT IdInstanciaFluxo, LoginResponsavel, NomeTarefa,  MAX(IdTarefa) as IdTarefa  FROM Tarefa 
			WHERE 
				TarefaCompleta = 0
				AND ativo  = 1
			GROUP BY IdInstanciaFluxo, LoginResponsavel, NomeTarefa
		) t0  ON	inst.IdInstanciaFluxo = t0.IdInstanciaFluxo
		INNER JOIN	[Tarefa]			AS	tar  ON tar.IdTarefa = t0.IdTarefa
		INNER JOIN	[Lista]				AS	list ON	list.CodigoLista		=	inst.CodigoLista
	WHERE 
		(
				inst.Ativo				=	1
			AND	(@login IS NULL OR tar.LoginResponsavel	IN	((SELECT value FROM fn_Split(@login, ','))))
			AND	tar.TarefaCompleta		=	0
			AND	tar.Ativo				=	1
			AND inst.StatusFluxo = 1
			AND(
				(@filtro IS NULL) OR 
				(inst.NomeSolicitacao  LIKE @filtro) OR
				(inst.NomeFluxo	LIKE @filtro) OR
				(inst.NomeSolicitante  LIKE @filtro) OR
				(inst.NomeGerenteRegiao  LIKE @filtro) OR
				(inst.NomeDiretorVendas  LIKE @filtro) OR
				(tar.NomeTarefa LIKE @filtro) OR
				(inst.NomeSolicitacao  LIKE @filtro) OR
				(inst.NomeFluxo	LIKE @filtro)
			)
			 
		)
	) FinalTable
WHERE
	[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
	@indicePagina IS NULL OR
	@registrosPorPagina IS NULL
ORDER BY [Index];

END

GO
/****** Object:  StoredProcedure [dbo].[spConsultarTarefasPendentes]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spConsultarTarefasPendentes] 
	@filtro VARCHAR(200) = NULL,
	@CodigoLista			uniqueidentifier,
	@CodigoItem				int,
	@indicePagina			INT = 0,
	@registrosPorPagina		INT = 1000,
	@ordernarPor			VARCHAR(100) = NULL,
	@ordernarDirecao		VARCHAR(4) = 'ASC'
AS
BEGIN
	SET NOCOUNT ON;

	--Ajusta o filtro para a consulta
	SELECT	@filtro  = CASE WHEN  @filtro IS NOT NULL THEN '%' + REPLACE(@filtro, '''', '') + '%' ELSE @filtro END;

	--Verifica se existe o registro na Gestão de Fluxo, se existir, deve buscar as informações de lá
	IF EXISTS(SELECT 1 AS Retorno FROM [dbo].[WFGestaoFluxo_Fluxo] (NOLOCK) 
				WHERE CD_LISTA = @CodigoLista AND CD_ITEM = @CodigoItem)
	BEGIN
		--Variáveis para pesquisa na Gestão de fluxo
		DECLARE @DescricaoUrlTarefa nvarchar(4000); 

		--Busca a URL da Tarefa
		SELECT @DescricaoUrlTarefa = CONCAT(DescricaoUrlTarefa,'?List=') FROM Lista WHERE CodigoLista = @CodigoLista

		--Monta o select para buscar os dados de Gestão de Fluxo
		SELECT
			FinalTable.IdTarefa,
			FinalTable.CodigoLista,
			FinalTable.CodigoItem,
			FinalTable.NomeSolicitacao,
			FinalTable.NomeFluxo,
			FinalTable.NomeEtapa,
			FinalTable.NomeTarefa,
			FinalTable.NomeResponsavel,
			FORMAT(FinalTable.DataInclusao,'dd/MM/yyyy HH:mm:ss') as DataInclusao,
			FORMAT(FinalTable.DataSLA,'dd/MM/yyyy HH:mm:ss')	  as DataSLA,
			FinalTable.TotalRecordCount,
			FinalTable.NomeLista,
			FinalTable.DescricaoUrlTarefa,
			FinalTable.Ambiente2007,
			1 as GestaoFluxo
		FROM (
			SELECT 
					CASE @ordernarDirecao
						WHEN 'ASC' THEN
							ROW_NUMBER() OVER (
								ORDER BY
									CASE @ordernarPor
										WHEN 'NomeSolicitacao' THEN  fluxo.NM_SOLICITACAO
										WHEN 'NomeFluxo' THEN fluxo.NM_LISTA
										WHEN 'NomeResponsavel' THEN tarefas.NM_RESPONSAVEL
										WHEN 'NomeTarefa' THEN tarefas.NM_TAREFA
										WHEN 'DataInclusao' THEN RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, tarefas.DT_CRIACAO), 20)
										WHEN 'DataSLA' THEN CONVERT(VARCHAR, tarefas.DT_SLA, 126)
										ELSE RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, CAST(tarefas.DT_SLA AS INT)), 20)
									END ASC
							) 
						WHEN 'DESC' THEN
							ROW_NUMBER() OVER (
								ORDER BY
									CASE @ordernarPor
										WHEN 'NomeSolicitacao' THEN  fluxo.NM_SOLICITACAO
										WHEN 'NomeFluxo' THEN fluxo.NM_LISTA
										WHEN 'NomeResponsavel' THEN tarefas.NM_RESPONSAVEL
										WHEN 'NomeTarefa' THEN tarefas.NM_TAREFA
										WHEN 'DataInclusao' THEN RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, tarefas.DT_CRIACAO), 20)
										WHEN 'DataSLA' THEN CONVERT(VARCHAR, tarefas.DT_SLA, 126)
										ELSE RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, CAST(tarefas.DT_SLA AS INT)), 20)
									END DESC
							)
					END AS [Index],
					tarefas.CD_TAREFA		AS IdTarefa 
					, fluxo.CD_LISTA		AS CodigoLista
					, fluxo.CD_ITEM			AS CodigoItem
					, fluxo.NM_SOLICITACAO  AS NomeSolicitacao
					, fluxo.NM_LISTA		AS NomeFluxo
					, tarefas.NM_TAREFA		AS NomeTarefa 
					, tarefas.NM_PAPEL		AS NomeEtapa 
					, tarefas.NM_RESPONSAVEL AS NomeResponsavel 
					, tarefas.DT_CRIACAO	AS DataInclusao
					, tarefas.DT_SLA		AS DataSLA
					, COUNT(*) OVER ()		AS TotalRecordCount
					, fluxo.NM_LISTA		AS NomeLista
					, CASE WHEN tarefas.CD_ITEM_TAREFA is null THEN 
						'#' 
					  ELSE 
						@DescricaoUrlTarefa + CONVERT(VARCHAR(500), tarefas.CD_LISTA_TAREFA) + '&Id=' + CONVERT(VARCHAR(500),tarefas.CD_ITEM_TAREFA) 
					END AS DescricaoUrlTarefa
					,CAST (1 AS BIT)		AS Ambiente2007
			FROM 
				[dbo].[WFGestaoFluxo_Tarefa] tarefas		 (NOLOCK)
				INNER JOIN [dbo].[WFGestaoFluxo_Fluxo] fluxo (NOLOCK) ON tarefas.CD_FLUXO = fluxo.CD_SEQ_FLUXO 
			WHERE 
				fluxo.CD_LISTA		= @CodigoLista
				AND fluxo.CD_ITEM	= @CodigoItem
				AND tarefas.DT_SAIDA is NULL 
				AND tarefas.NM_STATUS <>  'Cancelado'
				AND (
					(@filtro IS NULL) OR 
					(tarefas.NM_RESPONSAVEL  LIKE @filtro) OR
					(tarefas.NM_TAREFA LIKE @filtro) OR
					(FORMAT(tarefas.DT_CRIACAO,'dd/MM/yyyy HH:mm:ss') LIKE @filtro) OR
					(FORMAT(tarefas.DT_SLA,'dd/MM/yyyy HH:mm:ss') LIKE @filtro)
				)
			) FinalTable
		WHERE
			[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
			@indicePagina IS NULL OR
			@registrosPorPagina IS NULL
		ORDER BY 
			[Index];
	END
	ELSE
	BEGIN
		SELECT
			FinalTable.IdTarefa,
			FinalTable.CodigoLista,
			FinalTable.CodigoItem,
			FinalTable.NomeSolicitacao,
			FinalTable.NomeFluxo,
			FinalTable.NomeEtapa,
			CASE 
				WHEN FinalTable.Ambiente2007 = 1
				THEN
					RTRIM(LTRIM(REPLACE(REPLACE(REPLACE( SUBSTRING(REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN'), CHARINDEX('-', REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN')) + 1, LEN		(REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN'))) ,'Aprovação','') ,'Revisão',''), 'CS#ADMIN','CS-ADMIN'))) 
			ELSE 
					FinalTable.NomeTarefa
			END AS NomeTarefa,
			FinalTable.NomeResponsavel,
			FORMAT(FinalTable.DataInclusao,'dd/MM/yyyy HH:mm:ss') AS DataInclusao,
			FORMAT(FinalTable.DataSLA,'dd/MM/yyyy HH:mm:ss') AS DataSLA,
			FinalTable.TotalRecordCount,
			FinalTable.NomeLista,
			FinalTable.DescricaoUrlTarefa,
			FinalTable.Ambiente2007,
			0 AS GestaoFluxo
		FROM (
			SELECT
				CASE @ordernarDirecao
					WHEN 'ASC' THEN
						ROW_NUMBER() OVER (
							ORDER BY
								CASE @ordernarPor
									WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
									WHEN 'NomeFluxo' THEN inst.NomeFluxo
									WHEN 'NomeResponsavel' THEN tar.NomeResponsavel
									WHEN 'NomeTarefa' THEN tar.NomeTarefa
									WHEN 'DataInclusao' THEN RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, tar.IdTarefa), 20)
									WHEN 'DataSLA' THEN CONVERT(VARCHAR, tar.DataSLA, 126)
									ELSE RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, CAST(tar.DataSLA AS INT)), 20)
								END ASC
						) 
					WHEN 'DESC' THEN
						ROW_NUMBER() OVER (
							ORDER BY
								CASE @ordernarPor
									WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
									WHEN 'NomeFluxo' THEN inst.NomeFluxo
									WHEN 'NomeResponsavel' THEN tar.NomeResponsavel
									WHEN 'NomeTarefa' THEN tar.NomeTarefa
									WHEN 'DataInclusao' THEN RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, tar.IdTarefa), 20)
									WHEN 'DataSLA' THEN CONVERT(VARCHAR, tar.DataSLA, 126)
									ELSE RIGHT(REPLICATE('0', 20) + CONVERT(VARCHAR, CAST(tar.DataSLA AS INT)), 20)
								END DESC
						)
				END AS [Index],
				tar.IdTarefa,
				inst.CodigoLista,
				inst.CodigoItem,
				inst.NomeSolicitacao,
				inst.NomeFluxo,
				inst.NomeEtapa,
				tar.NomeTarefa,
				tar.NomeResponsavel,
				tar.DataInclusao,
				tar.DataSLA,
				list.Nome AS NomeLista,
				CASE 
					WHEN list.Ambiente2007 = 1	THEN CONCAT(list.DescricaoUrlTarefa,'?List=', tar.CodigoListaTarefa,'&Id=', tar.CodigoTarefaSP) 
					ELSE CONCAT(list.DescricaoUrlTarefa, tar.IdTarefa,'&Id=',inst.CodigoItem,tar.ParametrosUrl) 
				END DescricaoUrlTarefa,
				list.Ambiente2007,
				COUNT(*) OVER () AS TotalRecordCount
			FROM		
				[InstanciaFluxo] AS	inst (NOLOCK)
				INNER JOIN	[Tarefa]			AS	tar  (NOLOCK) ON inst.IdInstanciaFluxo = tar.IdInstanciaFluxo
				INNER JOIN	[Lista]				AS	list (NOLOCK) ON list.CodigoLista	   = inst.CodigoLista
			WHERE 
				(
						inst.Ativo				=	1
					AND inst.CodigoLista = @CodigoLista
					AND inst.CodigoItem  = @CodigoItem
					AND	tar.TarefaCompleta		=	0
					AND	tar.Ativo				=	1
					AND inst.StatusFluxo = 1
					AND(
						(@filtro IS NULL) OR 
						(tar.NomeResponsavel  LIKE @filtro) OR
						(tar.NomeTarefa LIKE @filtro) OR
						(FORMAT(tar.DataInclusao,'dd/MM/yyyy HH:mm:ss') LIKE @filtro)  OR
						(FORMAT(tar.DataSLA,'dd/MM/yyyy HH:mm:ss') LIKE @filtro)
					)
				)
			) FinalTable
		WHERE
			[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
			@indicePagina IS NULL OR
			@registrosPorPagina IS NULL
		ORDER BY [Index];
	END
END

GO
/****** Object:  StoredProcedure [dbo].[spConsultarTarefasRealizadas]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spConsultarTarefasRealizadas] 
	@filtro					VARCHAR(200) = NULL,
	@CodigoLista			uniqueidentifier,
	@CodigoItem				int,
	@indicePagina			INT = 0,
	@registrosPorPagina		INT = 1000,
	@ordernarPor			VARCHAR(100) = NULL,
	@ordernarDirecao		VARCHAR(4) = 'ASC'
AS
BEGIN
SET NOCOUNT ON;
	
	--Ajusta o Filtro
	SELECT	@filtro  = CASE WHEN  @filtro IS NOT NULL THEN '%' + REPLACE(@filtro, '''', '') + '%' ELSE @filtro END;

	--Verifica se existe o registro na Gestão de Fluxo, se existir, deve buscar as informações de lá
	IF EXISTS(SELECT 1 AS Retorno FROM [dbo].[WFGestaoFluxo_Fluxo] (NOLOCK) WHERE CD_LISTA = @CodigoLista AND CD_ITEM = @CodigoItem)
	BEGIN
		--Monta o select para buscar os dados de Gestão de Fluxo
		SELECT
			FinalTable.IdTarefa,
			FinalTable.CodigoLista,
			FinalTable.CodigoItem,
			FinalTable.NomeSolicitacao,
			FinalTable.NomeFluxo,
			FinalTable.NomeEtapa,
			FinalTable.NomeTarefa,
			FinalTable.NomeCompletadoPor,
			FinalTable.DescricaoAcaoEfetuada,
			--Usada a mesma regra do 2007
			CASE WHEN  LEN(FinalTable.ComentarioAprovacao) > 3900 THEN 
				SUBSTRING(FinalTable.ComentarioAprovacao, 0, LEN(FinalTable.ComentarioAprovacao) - 30)
			ELSE 
				FinalTable.ComentarioAprovacao
			END AS ComentarioAprovacao,
			FORMAT(FinalTable.DataFinalizado,'dd/MM/yyyy HH:mm:ss')  DataFinalizado,
			FinalTable.TotalRecordCount,
			1 as GestaoFluxo
		FROM (
			SELECT 
					CASE @ordernarDirecao
						WHEN 'ASC' THEN
							ROW_NUMBER() OVER (
								ORDER BY
									CASE @ordernarPor
										WHEN 'NomeSolicitacao' THEN  fluxo.NM_SOLICITACAO
										WHEN 'NomeFluxo' THEN fluxo.NM_LISTA
										WHEN 'NomeCompletadoPor' THEN tarefas.NM_RESPONSAVEL
										WHEN 'NomeTarefa' THEN tarefas.NM_TAREFA
										WHEN 'DescricaoAcaoEfetuada' THEN tarefas.NM_STATUS
										WHEN 'ComentarioAprovacao' THEN tarefas.NM_COMENTARIOS
										WHEN 'DataFinalizado' THEN CONVERT(VARCHAR, tarefas.DT_SAIDA, 126)
										ELSE CONVERT(VARCHAR, tarefas.DT_SAIDA, 126)
									END ASC
							) 
						WHEN 'DESC' THEN
							ROW_NUMBER() OVER (
								ORDER BY
									CASE @ordernarPor
										WHEN 'NomeSolicitacao' THEN  fluxo.NM_SOLICITACAO
										WHEN 'NomeFluxo' THEN fluxo.NM_LISTA
										WHEN 'NomeCompletadoPor' THEN tarefas.NM_RESPONSAVEL
										WHEN 'NomeTarefa' THEN tarefas.NM_TAREFA
										WHEN 'DescricaoAcaoEfetuada' THEN tarefas.NM_STATUS
										WHEN 'ComentarioAprovacao' THEN tarefas.NM_COMENTARIOS
										WHEN 'DataFinalizado' THEN CONVERT(VARCHAR, tarefas.DT_SAIDA, 126)
										ELSE CONVERT(VARCHAR, tarefas.DT_SAIDA, 126)
									END DESC
							)
					END AS [Index],
					 tarefas.CD_TAREFA		 AS IdTarefa 
					, fluxo.CD_LISTA		 AS CodigoLista
					, fluxo.CD_ITEM			 AS CodigoItem
					, fluxo.NM_SOLICITACAO   AS NomeSolicitacao
					, fluxo.NM_LISTA		 AS NomeFluxo
					, tarefas.NM_PAPEL		 AS NomeEtapa 
					, tarefas.NM_TAREFA		 AS NomeTarefa 
					, tarefas.NM_RESPONSAVEL AS NomeCompletadoPor
					, tarefas.NM_STATUS		 AS DescricaoAcaoEfetuada
					, tarefas.NM_COMENTARIOS AS ComentarioAprovacao
					, tarefas.DT_SAIDA		 AS DataFinalizado
					, COUNT(*) OVER ()		 AS TotalRecordCount
					, CAST (1 AS BIT)		 AS Ambiente2007
			FROM 
				[dbo].[WFGestaoFluxo_Tarefa] tarefas		 (NOLOCK)
				INNER JOIN [dbo].[WFGestaoFluxo_Fluxo] fluxo (NOLOCK) ON tarefas.CD_FLUXO = fluxo.CD_SEQ_FLUXO 
			WHERE 
				fluxo.CD_LISTA		= @CodigoLista
				AND fluxo.CD_ITEM	= @CodigoItem
				AND tarefas.DT_SAIDA is NOT NULL 
				AND tarefas.NM_STATUS NOT IN ('Cancelado', 'Não Obrigatório')
				AND (
						(@filtro IS NULL) OR 
						(tarefas.NM_TAREFA LIKE @filtro) OR
						(tarefas.NM_RESPONSAVEL LIKE @filtro) OR
						(tarefas.NM_STATUS LIKE @filtro) OR
						(tarefas.NM_COMENTARIOS LIKE @filtro) OR
						(FORMAT(tarefas.DT_SAIDA,'dd/MM/yyyy HH:mm:ss') LIKE @filtro)
					) 
			) FinalTable
		WHERE
			[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
			@indicePagina IS NULL OR
			@registrosPorPagina IS NULL
		ORDER BY 
			[Index];
	END
	ELSE
	BEGIN
		SELECT
			FinalTable.IdTarefa,
			FinalTable.CodigoLista,
			FinalTable.CodigoItem,
			FinalTable.NomeSolicitacao,
			FinalTable.NomeFluxo,
			FinalTable.NomeEtapa,
			CASE 
				WHEN FinalTable.Ambiente2007 = 1
				THEN
					RTRIM(LTRIM(REPLACE(REPLACE(REPLACE( SUBSTRING(REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN'), CHARINDEX('-', REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN')) + 1, LEN		(REPLACE(FinalTable.NomeTarefa,'CS-ADMIN', 'CS#ADMIN'))) ,'Aprovação','') ,'Revisão',''), 'CS#ADMIN','CS-ADMIN'))) 
			ELSE 
					FinalTable.NomeTarefa
			END AS NomeTarefa,
			FinalTable.NomeCompletadoPor,
			FinalTable.DescricaoAcaoEfetuada,
			CASE WHEN CHARINDEX(char(10)+char(13), FinalTable.ComentarioAprovacao) > 0
			THEN 
				--SUBSTRING(FinalTable.ComentarioAprovacao, 0,CHARINDEX(char(10)+char(13),FinalTable.ComentarioAprovacao)+1)
				REPLACE(FinalTable.ComentarioAprovacao, char(10)+char(13), '')
			ELSE 
				FinalTable.ComentarioAprovacao
			END ComentarioAprovacao,
			FORMAT(FinalTable.DataFinalizado,'dd/MM/yyyy HH:mm:ss')  DataFinalizado,
			FinalTable.TotalRecordCount,
			0 as GestaoFluxo
		FROM (
			SELECT
				CASE @ordernarDirecao
					WHEN 'ASC' THEN
						ROW_NUMBER() OVER (
							ORDER BY
								CASE @ordernarPor
									WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
									WHEN 'NomeFluxo' THEN inst.NomeFluxo
									WHEN 'NomeCompletadoPor' THEN tar.NomeCompletadoPor
									WHEN 'NomeTarefa' THEN tar.NomeTarefa
									WHEN 'DescricaoAcaoEfetuada' THEN 
																		CASE	WHEN tar.DescricaoAcaoEfetuada = 'Aprovar' THEN 'Aprovado' 
																				WHEN tar.DescricaoAcaoEfetuada = 'Continuar' THEN 'Continuar' 
																				WHEN tar.DescricaoAcaoEfetuada = 'Reprovar' THEN 'Rejeitado'  
																				WHEN tar.DescricaoAcaoEfetuada = 'Disctutir' THEN 'Discussão'
																				ELSE tar.DescricaoAcaoEfetuada 
																		END 
									WHEN 'ComentarioAprovacao' THEN tar.ComentarioAprovacao
									WHEN 'DataFinalizado' THEN CONVERT(VARCHAR, tar.DataFinalizado, 126)
									ELSE CONVERT(VARCHAR, tar.DataFinalizado, 126)
								END ASC
						) 
					WHEN 'DESC' THEN
						ROW_NUMBER() OVER (
							ORDER BY
								CASE @ordernarPor
									WHEN 'NomeSolicitacao' THEN  inst.NomeSolicitacao
									WHEN 'NomeFluxo' THEN inst.NomeFluxo
									WHEN 'NomeCompletadoPor' THEN tar.NomeCompletadoPor
									WHEN 'NomeTarefa' THEN tar.NomeTarefa
									WHEN 'DescricaoAcaoEfetuada' THEN 
																		CASE	WHEN tar.DescricaoAcaoEfetuada = 'Aprovar' THEN 'Aprovado' 
																				WHEN tar.DescricaoAcaoEfetuada = 'Continuar' THEN 'Continuar' 
																				WHEN tar.DescricaoAcaoEfetuada = 'Reprovar' THEN 'Rejeitado'  
																				WHEN tar.DescricaoAcaoEfetuada = 'Disctutir' THEN 'Discussão'
																				ELSE tar.DescricaoAcaoEfetuada 
																		END 
									WHEN 'ComentarioAprovacao' THEN tar.ComentarioAprovacao
									WHEN 'DataFinalizado' THEN CONVERT(VARCHAR, tar.DataFinalizado, 126)
									ELSE CONVERT(VARCHAR, tar.DataFinalizado, 126)
								END DESC
						)
				END AS [Index],
				tar.IdTarefa,
				inst.CodigoLista,
				inst.CodigoItem,
				inst.NomeSolicitacao,
				inst.NomeFluxo,
				inst.NomeEtapa,
				tar.NomeTarefa,
				tar.NomeCompletadoPor,
				CASE	WHEN tar.DescricaoAcaoEfetuada = 'Aprovar' THEN 'Aprovado' 
						WHEN tar.DescricaoAcaoEfetuada = 'Continuar' THEN 'Continuar' 
						WHEN tar.DescricaoAcaoEfetuada = 'Reprovar' THEN 'Rejeitado'  
						WHEN tar.DescricaoAcaoEfetuada = 'Disctutir' THEN 'Discussão'
						ELSE tar.DescricaoAcaoEfetuada 
				END DescricaoAcaoEfetuada,
				tar.ComentarioAprovacao,
				tar.DataFinalizado,
				list.Ambiente2007,
				COUNT(*) OVER () AS TotalRecordCount
			FROM		
				[InstanciaFluxo] AS	inst
				INNER JOIN	[Tarefa] AS	tar  ON inst.IdInstanciaFluxo = tar.IdInstanciaFluxo
				INNER JOIN	[Lista]	 AS	list ON	list.CodigoLista = inst.CodigoLista
			WHERE
						inst.Ativo				=	1
					AND inst.CodigoLista = @CodigoLista
					AND inst.CodigoItem = @CodigoItem
					AND	tar.TarefaCompleta		=	1
					AND tar.DescricaoAcaoEfetuada IS NOT NULL
					AND	tar.Ativo				=	1
					AND
					(
						(@filtro IS NULL) OR 
						(tar.NomeTarefa LIKE @filtro) OR
						(tar.NomeCompletadoPor  LIKE @filtro) OR
						(
							CASE	WHEN tar.DescricaoAcaoEfetuada = 'Aprovar' THEN 'Aprovado' 
									WHEN tar.DescricaoAcaoEfetuada = 'Continuar' THEN 'Continuar' 
									WHEN tar.DescricaoAcaoEfetuada = 'Reprovar' THEN 'Rejeitado'  
									WHEN tar.DescricaoAcaoEfetuada = 'Discutir' THEN 'Discussão'
									ELSE tar.DescricaoAcaoEfetuada
							END LIKE @filtro
						) OR
						(tar.ComentarioAprovacao  LIKE @filtro) OR
						(FORMAT(tar.DataFinalizado,'dd/MM/yyyy HH:mm:ss') LIKE @filtro)
					) 
			) FinalTable
		WHERE
			[Index] BETWEEN @indicePagina + 1 AND @indicePagina + @registrosPorPagina OR
			@indicePagina IS NULL OR
			@registrosPorPagina IS NULL
		ORDER BY [Index];
	END
END

GO
/****** Object:  StoredProcedure [dbo].[spDelegarTarefa]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spDelegarTarefa] 
	@IdTarefa	    INT,
	@LoginOrigem    VARCHAR(255) = Null, -- Login a ser delegado (Se não informado busca da tarefa)
	@LoginOperacao  VARCHAR(255)		 -- Usuário que está efetuando a operação
AS
SET NOCOUNT ON

--Dados de destino
DECLARE @EmailPara	    VARCHAR(200) 
DECLARE @NomePara	    VARCHAR(255)
DECLARE @LoginPara	    VARCHAR(255)

DECLARE @max		    INT	 -- Máximo de recursão permitida
DECLARE @TransacaoLocal	BIT	 -- Se existe transação local

--Temporária para armazenar as delegações
DECLARE @TMP_DELEGACAO  TABLE (Ordem int identity(1,1), LoginDe VARCHAR(255), LoginPara VARCHAR(255) primary key, NomePara VARCHAR(255), EmailPara VARCHAR(200))

--Define a recursão atual
SET @max = 1

--Se necessário, busca o responsável da tarefa
IF (@LoginOrigem is null)
	SELECT @LoginOrigem = LoginResponsavel FROM Tarefa WHERE IdTarefa = @IdTarefa

INSERT INTO @TMP_DELEGACAO
SELECT LoginDe, LoginPara, NomePara, EmailPara FROM Delegacao 
WHERE 
	LoginDe = @LoginOrigem
	AND GETDATE() BETWEEN DataInicio AND DataFim 
	AND Ativo = 1

--Varre as delegações
WHILE (@@ROWCOUNT <> 0 AND @max < 50)
BEGIN
	SET @max = @max + 1
	
	--Busca o último login incluído
	SELECT TOP 1 @LoginPara = LoginPara FROM @TMP_DELEGACAO ORDER BY Ordem desc;
	
	INSERT INTO @TMP_DELEGACAO
	SELECT LoginDe, LoginPara, NomePara, EmailPara FROM Delegacao 
	WHERE 
		LoginDe = @LoginPara 
		-- Não insere logins de forma recursiva
		AND LoginPara not in (Select LoginPara FROM @TMP_DELEGACAO)
		-- Não permite voltar para a mesma pessoa
		AND LoginPara <> @LoginOrigem
		-- No período válido
		AND GETDATE() BETWEEN DataInicio AND DataFim
		-- Somente registros com LoginPara preenchidos
		AND ISNULL(LoginPara, '') <> '' 
		AND Ativo = 1
END

--Busca a última delegação feita
SELECT TOP 1 @LoginPara = LoginPara, @EmailPara = EmailPara, @NomePara = NomePara 
FROM 
	@TMP_DELEGACAO 
ORDER BY 
	Ordem 
DESC;

-- Verifica se deve cadastrar alguma delegação
IF (@LoginPara is null OR ltrim(rtrim(@LoginPara)) = '')
	RETURN;

BEGIN TRY
	--Inicia a transação se não possuir
	IF (@@TRANCOUNT = 0)
	BEGIN
		BEGIN TRANSACTION 
		SET @TransacaoLocal = 1
	END

	--Inclui todas as delegações efetuadas
	INSERT INTO TarefaHist
	(
		IdTarefa,
		TipoTarefaHist,
		LoginDe,
		LoginPara,
		LoginInclusao,
		DataInclusao,
		Ativo
	)
	SELECT
		@IdTarefa,
		2, -- Delegação automática
		LoginDe,
		LoginPara,
		@LoginOperacao,
		getdate(),
		1
	FROM
		@TMP_DELEGACAO
	ORDER BY
		Ordem

	--Atualiza a tarefa com a última delegação feita
	UPDATE Tarefa SET
		EmailResponsavel = @EmailPara,
		LoginResponsavel = @LoginPara,
		NomeResponsavel  = @NomePara,
		DataAlteracao	 = getdate(),
		LoginAlteracao	 = @LoginOperacao
	WHERE	
		IdTarefa = @IdTarefa

	IF (@TransacaoLocal = 1)
		COMMIT TRANSACTION;

END TRY
BEGIN CATCH
	IF (@TransacaoLocal = 1)
		ROLLBACK TRANSACTION;

	DECLARE @ErrorMessage	NVARCHAR(4000);
    DECLARE @ErrorSeverity	INT;
    DECLARE @ErrorState		INT;

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    -- Retorna o erro original
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;

SET NOCOUNT OFF;

GO
/****** Object:  StoredProcedure [dbo].[spEscalonarTarefas]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spEscalonarTarefas]
AS
SET NOCOUNT ON
DECLARE @LoginOperacao		VARCHAR(255)
--Variáveis para o Cursor
DECLARE @IdTarefa			INT
DECLARE @LoginResponsavel	VARCHAR(255)
DECLARE @LoginSuperior		VARCHAR(255)

--Define o login do sistema como login de operação
SET @LoginOperacao = 'SISTEMA';

--Temp para as Tarefas que serão escalonadas
DECLARE @TEMP_ESCALONAMENTO TABLE(IdTarefa int primary key, LoginResponsavel varchar(150), LoginSuperior varchar(150))

--Busca as tarefas que serão escalonadas 
INSERT INTO @TEMP_ESCALONAMENTO
SELECT 
	IdTarefa,
	LoginResponsavel,
	LoginSuperior
FROM	
	Tarefa t
	INNER JOIN InstanciaFluxo i ON t.IdInstanciaFluxo = i.IdInstanciaFluxo
	INNER JOIN Lista l			ON i.CodigoLista = l.CodigoLista
WHERE
	--Busca tarefas não completas, não escalonadas que passaram da data de escalonar
	t.TarefaCompleta = 0
	AND t.TarefaEscalonada = 0
	AND t.DataEscalonamento < getdate()
	AND t.LoginSuperior IS NOT NULL
	AND LTRIM(RTRIM(t.LoginSuperior)) <> ''
	AND t.NomeSuperior IS NOT NULL
	AND i.StatusFluxo  = 1	-- Somente trata fluxos Em Andamento
	AND l.Ambiente2007 = 0  -- Somente trata listas do 2013
	AND t.Ativo = 1
	AND i.Ativo = 1


--Cria um Cursor para processar as tarefas
DECLARE Tarefas CURSOR FAST_FORWARD FOR SELECT IdTarefa, LoginResponsavel, LoginSuperior FROM @TEMP_ESCALONAMENTO;
OPEN Tarefas;

--Inicia a transação
BEGIN TRANSACTION

BEGIN TRY
	--Para cada tarefa
	FETCH NEXT FROM Tarefas INTO @IdTarefa, @LoginResponsavel, @LoginSuperior;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--Inclui o escalonamento
		INSERT INTO TarefaHist
		(
			IdTarefa,
			TipoTarefaHist,
			LoginDe,
			LoginPara,
			LoginInclusao,
			DataInclusao,
			Ativo
		)
		VALUES
		(
			@IdTarefa,
			3, -- Histórico de Escalonamento
			@LoginResponsavel,
			@LoginSuperior,
			@LoginOperacao,
			getdate(),
			1
		)

		--Atualiza a tarefa
		UPDATE Tarefa SET
			EmailResponsavel = EmailSuperior,
			LoginResponsavel = LoginSuperior,
			NomeResponsavel  = NomeSuperior,
			EmailSuperior    = null,
			LoginSuperior	 = null,
			NomeSuperior	 = null,
			DataAlteracao	 = getdate(),
			LoginAlteracao	 = @LoginOperacao,
			TarefaEscalonada = 1
		WHERE	
			IdTarefa = @IdTarefa
    
		--Efetua a delegação da tarefa quando necessário
		EXECUTE spDelegarTarefa 
				@IdTarefa		= @IdTarefa, 
				@LoginOrigem	= @LoginSuperior, 
				@LoginOperacao	= @LoginOperacao

		--Processa a próxoma tarefa
		FETCH NEXT FROM Tarefas INTO @IdTarefa, @LoginResponsavel, @LoginSuperior;
	END;
	--Salva as operações realizadas
	COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	--Cancela as alterações
	ROLLBACK TRANSACTION;

	DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT 
        @ErrorMessage   = ERROR_MESSAGE(),
        @ErrorSeverity  = ERROR_SEVERITY(),
        @ErrorState		= ERROR_STATE();

    -- Retorna o erro original
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH

CLOSE Tarefas;
DEALLOCATE Tarefas;

--Retorna as tarefas processadas para o envio de e-mail
SELECT
	t0.IdTarefa,
	t1.EmailResponsavel,
	t1.DescricaoAssuntoEmailEscalonamento,
	t1.DescricaoMensagemEmailEscalonamento	
FROM	
	@TEMP_ESCALONAMENTO t0
	INNER JOIN Tarefa t1 ON t0.IdTarefa = t1.IdTarefa
	 
SET NOCOUNT OFF

GO
/****** Object:  StoredProcedure [dbo].[spLimparEstruturaComercialModificada]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spLimparEstruturaComercialModificada]
AS
BEGIN
	
	TRUNCATE TABLE [dbo].[EstruturaComercial_Modificada]
	
END


GO
/****** Object:  StoredProcedure [dbo].[spLimparEstruturaComercialSalesForce]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spLimparEstruturaComercialSalesForce]
AS
BEGIN
	
	TRUNCATE TABLE [dbo].[EstruturaComercial_Salesforce]
	
END


GO
/****** Object:  StoredProcedure [dbo].[spLimparLog]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spLimparLog]
AS
SET NOCOUNT ON

	--Efetua a limpeza do log com mais de X dias
	DELETE FROM [Log] WHERE datediff(dd, DataInclusao , getdate()) > 30

SET NOCOUNT OFF


GO
/****** Object:  StoredProcedure [dbo].[spObterDocumentos]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spObterDocumentos]
	@idTipoProposta int,
	@agrupador varchar(255)
AS
BEGIN

	SET NOCOUNT ON;
	SELECT d.IdDocumento,d.Nome, d.Agrupador  FROM Documento d 
	INNER JOIN PropostaDocumento p ON D.IdDocumento = p.IdDocumento
	WHERE d.Agrupador = @agrupador AND p.IdTipoProposta = @idTipoProposta
	
END
GO
/****** Object:  StoredProcedure [dbo].[spObterDocumentosProposta]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spObterDocumentosProposta]
	@idItem int,
	@idTipoProposta int,
	@agrupador varchar(255)

AS
BEGIN

	SET NOCOUNT ON;
	SELECT 
		t.IdItem,
		t.IdTipoPropostaDocumento,
		t.IdTipoProposta,
		t.IdDocumento,
		t.Tem, 
		t.Atende,
		t.Dispensado,
		t.Excecao,
		d.Nome,
		d.Agrupador
	FROM TipoPropostaDocumento t 	INNER JOIN 	
		Documento d ON d.IdDocumento = t.IdDocumento
	WHERE 
		(t.IdItem = @idItem AND t.IdTipoProposta = @idTipoProposta) AND d.Agrupador = @agrupador 
	
END
GO
/****** Object:  StoredProcedure [dbo].[spObterEstruturasComerciaisModificadas]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spObterEstruturasComerciaisModificadas]
AS
BEGIN
	
	-- Insere as estruturas comerciais do Salesforce que foram modificadas na tabela que o timerjob de rezoneamento utilizará
	INSERT INTO
		EstruturaComercial_Modificada
	SELECT 
		ec_salesforce.IBM AS 'IBM',
		ec_salesforce.SiteCode AS 'SiteCode',
		ec_salesforce.GT AS 'GT',
		ec_salesforce.GR AS 'GR',
		ec_salesforce.DV AS 'DV',
		ec_salesforce.CDR AS 'CDR',
		ec_salesforce.GDR AS 'GDR',
		0 AS 'Processado'
	FROM 
		EstruturaComercial_Salesforce ec_salesforce WITH(NOLOCK)
	EXCEPT
		SELECT 
			ec_local.IBM AS 'IBM',
			ec_local.SiteCode AS 'SiteCode',
			ec_local.GT AS 'GT',
			ec_local.GR AS 'GR',
			ec_local.DV AS 'DV',
			ec_local.CDR AS 'CDR',
			ec_local.GDR AS 'GDR',
			0 AS 'Processado'
		FROM 
			EstruturaComercial ec_local WITH(NOLOCK)
	
END


GO
/****** Object:  StoredProcedure [dbo].[spObterTituloSolicitacao]    Script Date: 17/11/2016 18:06:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[spObterTituloSolicitacao] 
		@listId uniqueidentifier
	,	@itemId int
AS
BEGIN
	
SET NOCOUNT ON;

		DECLARE		@Result				NVARCHAR(MAX)
				,	@RazaoSocial		NVARCHAR(MAX)
				,	@ContentType		NVARCHAR(MAX);
		DECLARE @TabelaLista TABLE ([NomeLista] [NVARCHAR](500),[CodigoLista] [NVARCHAR](500),[TituloItem] [NVARCHAR](500));

		IF EXISTS (SELECT TOP 1 1 FROM LISTA WHERE Ambiente2007 = 0 AND CodigoLista = @listId)
		BEGIN
			SELECT 
				L.Nome + ' - ' + I.NomeSolicitacao + ' - ' + Item.NomeRazaoSocial  AS TituloSolicitacao
			FROM 			ListaAditivosGerais Item
			INNER JOIN	InstanciaFluxo	I
			ON		I.CodigoLista		=	Item.CodigoLista
				AND	I.CodigoItem		=	Item.CodigoItem
			INNER JOIN LISTA			L
			ON		L.CodigoLista		=	I.CodigoLista
			WHERE 
				Item.CodigoItem			= @itemId
				AND	Item.CodigoLista	= @listId;
		END
		ELSE
		BEGIN

			INSERT INTO @TabelaLista([NomeLista],[CodigoLista],[TituloItem])
			SELECT
					A.Nome			AS NomeLista
				,	A.CodigoLista	AS CodigoLista
				,	B.CodigoLista	AS TituloItem 
			FROM LISTA A
			LEFT JOIN (
				SELECT 
					CodigoLista 
				FROM LISTA WHERE DescricaoUrlLista IN (
				'Lists/CadastroB2B',
				'Lists/B2BIPonline',
				'Lists/RenovacaoContratoRNIP')
				AND CodigoLista		=	@listId
				AND Ambiente2007	=	0
			) AS B
			ON	A.CodigoLista = B.CodigoLista
			WHERE
				A.CodigoLista = @listId ;
		
			EXEC	[dbo].[spConsultarColunaSp2007]
					@internalName = N'ContentType',
					@listId = @listId,
					@itemId = @itemId,
					@valorColuna = @ContentType output;
		
			EXEC	[dbo].[spConsultarColunaSp2007]
					@internalName = N'RazaoSocial',
					@listId = @listId,
					@itemId = @itemId,
					@valorColuna = @RazaoSocial output;
							
			IF EXISTS (SELECT TOP 1 1 FROM @TabelaLista WHERE TituloItem IS NOT NULL) 
			BEGIN
				 SET @RazaoSocial = ''
			END
			ELSE
			BEGIN
				IF @RazaoSocial IS NULL 
					BEGIN
						SET @RazaoSocial = ''
					END
				ELSE
				BEGIN
					SET @RazaoSocial = ' - ' + @RazaoSocial
				END
			END

			IF @ContentType IS NULL 
			BEGIN
				SELECT 
					T.NomeLista + ' - ' + I.NomeSolicitacao + @RazaoSocial  AS TituloSolicitacao
				FROM @TabelaLista			T
				INNER JOIN	InstanciaFluxo	I
				ON		I.CodigoLista	=	@listId
					AND	I.CodigoItem	=	@itemId
			END
			ELSE
			BEGIN
				SELECT 
					@ContentType + ' - ' + I.NomeSolicitacao + @RazaoSocial AS TituloSolicitacao
				FROM @TabelaLista			T
				INNER JOIN	InstanciaFluxo	I
				ON		I.CodigoLista	=	@listId
					AND	I.CodigoItem	=	@itemId
			END
		END
END




GO
