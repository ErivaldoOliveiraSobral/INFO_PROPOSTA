add-pssnapin microsoft.sharepoint.powershell -EA 0


#################### Ambiente #########################
$migrateTo = "";

if($env:computername -eq "PI")
{
    $migrateTo = "http://pi"
}

if($env:computername -eq "RZAPPRNP01VD")
{
    $migrateTo = "http://fluxos-dev.raizen.com"
}

if($env:computername -eq "RZAPPRNP01VQ" -or $env:computername -eq "RZAPPRNP02VQ")
{
    $migrateTo = "http://fluxos-qas.raizen.com"
}

if($env:computername -eq "RZAPPRNP01V" -or $env:computername -eq "RZAPPRNP02V")
{
    $migrateTo = "http://fluxos.raizen.com"
}

if([string]::IsNullOrEmpty($migrateTo))
{
	Write-Host ""
	Write-Host -ForegroundColor Green "Por favor, informe os dados do ambiente ..."
	$migrateTo = Read-Host "Site Collection destino: "
}


Write-Host -ForegroundColor Green "Ambiente:" $url;
#################### Ambiente #########################

$ambiente = "";

if($migrateTo -match 'fluxos-dev.raizen.com'){$ambiente="dev"}
elseif($migrateTo -match 'fluxos-qas.raizen.com'){$ambiente="qas"}
elseif($migrateTo -match 'fluxos.raizen.com'){$ambiente="prd"}
else{$ambiente="vm"}

Write-Host "Configurando ambiente:" $ambiente

#################### WebService 2007 #########################

Write-Host ""

if($ambiente -eq "dev"){ $urlWsEmailAnexo2007="http://rzappwfe01vd/_layouts/Raizen.Workflow.AnexoEmailProposta/EmailAnexoPropostaService.asmx" }
elseif($ambiente -eq "qas"){$urlWsEmailAnexo2007="http://gestaodefluxos-sc-qas.cosan.com.br/_layouts/Raizen.Workflow.AnexoEmailProposta/EmailAnexoPropostaService.asmx"}
elseif($ambiente -eq "prd"){$urlWsEmailAnexo2007="http://gestaodefluxos-sc.cosan.com.br/_layouts/Raizen.Workflow.AnexoEmailProposta/EmailAnexoPropostaService.asmx"}
else{$urlWsEmailAnexo2007="http://172.18.2.152:37000/_layouts/Raizen.Workflow.AnexoEmailProposta/EmailAnexoPropostaService.asmx"}

Write-Host "Configurando WebService 2007:" $urlWsEmailAnexo2007

#################### WebService 2007 #########################

#################### WebService 2013 #########################

Write-Host ""

if($ambiente -eq "dev"){ $urlWsEmailAnexo2013="http://fluxos-dev.raizen.com/_layouts/15/PortalDeFluxos.Core.SP/Services/EmailAnexoPropostaService.asmx" }
elseif($ambiente -eq "qas"){$urlWsEmailAnexo2013="http://fluxos-qas.raizen.com/_layouts/15/PortalDeFluxos.Core.SP/Services/EmailAnexoPropostaService.asmx"}
elseif($ambiente -eq "prd"){$urlWsEmailAnexo2013="http://fluxos.raizen.com/_layouts/15/PortalDeFluxos.Core.SP/Services/EmailAnexoPropostaService.asmx"}
else{$urlWsEmailAnexo2013="http://pi/_layouts/15/PortalDeFluxos.Core.SP/Services/EmailAnexoPropostaService.asmx"}

Write-Host "Configurando WebService 2013:" $urlWsEmailAnexo2013

#################### WebService 2013 #########################


#################### Conta Admin #########################

Write-Host ""
$dominioAdm2007 = ""
$usuarioAdm2007 = ""
$senhaAdm2007 = ""
if($ambiente -eq "dev"){ 
	$dominioAdm2007 = "COSAN"
	$usuarioAdm2007 = "mosswebservices"
	$senhaAdm2007 = "Cos@n2010"}
elseif($ambiente -eq "qas"){ 
	$dominioAdm2007 = "COSAN"
	$usuarioAdm2007 = "mosswebservices"
	$senhaAdm2007 = "Cos@n2010"}
elseif($ambiente -eq "prd"){ 
	$dominioAdm2007 = "COSAN"
	$usuarioAdm2007 = "mosswebservices"
	$senhaAdm2007 = "Cos@n2010"}
else{ 
	$dominioAdm2007 = "ITERISNET"
	$usuarioAdm2007 = "Administrator"
	$senhaAdm2007 = "It3r1510"}

Write-Host "Configurandoconta do administrador Sp2007:" $usuarioAdm

#################### Conta Admin #########################


#################### Documento 2007 #########################

Write-Host ""
$documentSize = 5

#################### Documento 2007 #########################


#################### ConnectionString #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os dados da connectionString ..."
$instanciaDB = Read-Host "Digite a instância do DB (ex: RZAPPSHR01VD):"
$bancoDB = Read-Host "Digite o nome do banco (ex: PortalDeFluxoCore)"
$usuarioDB = Read-Host "Digite o usuário do Banco (ex: PortalDeFluxoCore)"
$senhaDB = Read-Host "Digite a senha:"

#################### ConnectionString #########################

#################### Conta Admin #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os dados da conta do administrador ..."
$dominioAdm = Read-Host "Digite o domínio (ex: cosan):"
$usuarioAdm = Read-Host "Digite a conta utilizada (ex: mosswebservices):"
$senhaAdm = Read-Host "Digite a senha:"

#################### Conta Admin #########################

################################ E-mail #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os dados de e-mail ..."
$servidorEmail = Read-Host "Digite o servidor de e-mail com suporte imap:"
$portaEmail = Read-Host "Digite a porta:"
$usuarioEmail = Read-Host "Digite a conta utilizada:"
$senhaEmail = Read-Host "Digite a senha:"

################################ E-mail #########################

#################### Url Record Center #########################

Write-Host ""

if($ambiente -eq "dev"){ $urlRecordCenter= "http://fluxos-dev.raizen.com/sites/AnexosPropostas" }
elseif($ambiente -eq "qas"){$urlRecordCenter= "http://fluxos-qas.raizen.com/sites/AnexosPropostas"}
elseif($ambiente -eq "prd"){$urlRecordCenter= "http://fluxos.raizen.com/sites/AnexosPropostas"}
else{$urlRecordCenter= "http://pi/sites/AnexosPropostas"}

Write-Host "Configurando Url Record Center:" $urlRecordCenter

#################### Url Record Center #########################

$sites = get-spsite $migrateTo

if(!$sites)
{
    Write-Host -ForegroundColor Red "Erro ao obter o Site Collection! O script será finalizado... "
    break
}

[void][System.Reflection.Assembly]::LoadWithPartialName("PortalDeFluxos.Core.BLL");

$rootWeb = $sites.RootWeb

#################### E-mail #########################

Write-Host "Iniciando configuração de e-mail ..."
Write-Host ""

$imap   = New-Object PortalDeFluxos.Core.BLL.Modelo.ConfiguracaoImap;
$imap.Servidor = $servidorEmail;
$imap.Porta = $portaEmail;
$imap.SSL = $true;
$imap.Usuario = $usuarioEmail;
$imap.Senha = $senhaEmail;
$imap.QuantidadeLote = 100;
$imap.ValidarRemetente = $true;

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebImap)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebImap, [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($imap));
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebImap] = [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($imap);
}

$rootWeb.Update();

Write-Host ""
Write-Host -ForegroundColor Green "Configuração de e-mail finalizada!"

#################### E-mail #########################

#################### Conta Admin #########################

Write-Host "Iniciando configuração do usuário Administrador ..."
Write-Host ""

$contaAdmin  = New-Object System.Net.NetworkCredential($usuarioAdm, $senhaAdm, $dominioAdm);

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaAdmin)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaAdmin, [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($contaAdmin));
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaAdmin] = [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($contaAdmin);
}

$rootWeb.Update();

Write-Host ""
Write-Host -ForegroundColor Green "Configuração do usuário Administrador finalizada!"
Write-Host ""

#################### Conta Admin #########################

#################### ConnectionString #########################

Write-Host "Iniciando configuração da connectionstring ..."
Write-Host ""

$connectionString = "data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework";
$connectionString = $connectionString -f $instanciaDB,$bancoDB,$usuarioDB,$senhaDB

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebConnectionString)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebConnectionString, $connectionString);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebConnectionString] = $connectionString;
}

$rootWeb.Update();

#################### ConnectionString #########################

#################### Url Record Center #########################

Write-Host ""
Write-Host "Iniciando configuração da Url Record Center ..."

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebAppURL)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebAppURL, $urlRecordCenter);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebAppURL] = $urlRecordCenter;
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração da Url Record Center finalizada!"
Write-Host ""

#################### Url Record Center #########################

#################### WebService 2007 #########################

Write-Host ""
Write-Host "Iniciando configuração do WebService 2007 ..."

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2007)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2007, $urlWsEmailAnexo2007);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2007] = $urlWsEmailAnexo2007;
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração do WebService 2007 finalizada!"
Write-Host ""

#################### WebService 2007 #########################

#################### WebService 2013 #########################

Write-Host ""
Write-Host "Iniciando configuração do WebService 2013 ..."


if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2013)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2013, $urlWsEmailAnexo2013);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveUrlWsAnexoEmail2013] = $urlWsEmailAnexo2013;
}

$rootWeb.Update();


Write-Host -ForegroundColor Green "Configuração do WebService 2013 finalizada!"
Write-Host ""

#################### WebService 2013 #########################

#################### Conta Admin #########################

Write-Host ""
Write-Host "Iniciando configuração do usuário administrador Sp2007 ..."


$contaAdmin  = New-Object System.Net.NetworkCredential($usuarioAdm2007, $senhaAdm2007, $dominioAdm2007);

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaSp2007)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaSp2007, [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($contaAdmin));
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebContaSp2007] = [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($contaAdmin);
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração do usuário Administrador finalizada!"
Write-Host ""

#################### Conta Admin #########################

#################### Conta Admin #########################

if($ambiente -eq "vm")
{
	#################### Usuarios Teste #######################
	Write-Host ""
	Write-Host "Iniciando configuração dos usuários teste..."

	$UsuariosTeste = New-Object PortalDeFluxos.Core.BLL.Modelo.UsuariosTeste
	$UsuariosTeste.Ativo = $true
	$UsuariosTeste.IndexUsuarioAtivo = 0
	$UsuariosTeste.Usuarios = New-Object System.Collections.Generic.List[PortalDeFluxos.Core.BLL.Modelo.UsuarioTeste]

	$UsuarioTeste0 = New-Object PortalDeFluxos.Core.BLL.Modelo.UsuarioTeste
	$UsuarioTeste0.Email = 'william.morita@iteris.com'
	$UsuarioTeste0.login = 'iterisnet\administrator'
	$UsuarioTeste0.Password = 'It3r1510'
	$UsuarioTeste0.Nome = 'Administrator'

	$UsuarioTeste1 = New-Object PortalDeFluxos.Core.BLL.Modelo.UsuarioTeste
	$UsuarioTeste1.Email = 'william.morita@iteris.com'
	$UsuarioTeste1.login = 'iterisnet\william.morita'
	$UsuarioTeste1.Password = 'It3r1509'
	$UsuarioTeste1.Nome = 'William Morita'

	$UsuarioTeste2 = New-Object PortalDeFluxos.Core.BLL.Modelo.UsuarioTeste
	$UsuarioTeste2.Email = 'bruno.mass@iteris.com'
	$UsuarioTeste2.login = 'iterisnet\bruno.mass'
	$UsuarioTeste2.Password = 'It3r1509'
	$UsuarioTeste2.Nome = 'Bruno Mass'

	$UsuariosTeste.Usuarios.Add($UsuarioTeste0)
	$UsuariosTeste.Usuarios.Add($UsuarioTeste1)
	$UsuariosTeste.Usuarios.Add($UsuarioTeste2)


	if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebUsariosTeste)) {
		$rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebUsariosTeste, [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($UsuariosTeste));
	}
	else {
		$rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebUsariosTeste] = [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($UsuariosTeste);
	}


	$rootWeb.Update();

	Write-Host -ForegroundColor Green "Configuração dos usuários teste finalizada!"
	Write-Host ""

	#################### Usuarios Teste #######################
}

#################### Documento 2007 #########################

Write-Host ""
Write-Host "Iniciando configuração documento 2007 ..."

$contaAdmin  = New-Object System.Net.NetworkCredential($usuarioAdm2007, $senhaAdm2007, $dominioAdm2007);

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebSizeDocumento2007)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebSizeDocumento2007, [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($documentSize));
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveWebSizeDocumento2007] = [PortalDeFluxos.Core.BLL.Utilitario.Serializacao]::SerializeToJson($documentSize);
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração documento 2007 finalizada!"
Write-Host ""

#################### Documento 2007 #########################


Write-Host ""
Write-Host -ForegroundColor Green "Configuração da connectionstring finalizada!"
Write-Host ""