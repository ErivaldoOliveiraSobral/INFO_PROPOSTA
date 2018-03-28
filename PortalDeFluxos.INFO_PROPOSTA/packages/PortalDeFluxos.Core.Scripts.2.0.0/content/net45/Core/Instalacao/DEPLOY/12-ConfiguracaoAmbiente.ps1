## Configuração ServicosEmailAttachment ##

add-pssnapin microsoft.sharepoint.powershell -EA 0

#################### Ambiente #########################
$url = "http://pi";
$ambiente = 1;

if($env:computername -eq "PI")
{
    $url = "http://pi"
	$ambiente = "1";
}

if($env:computername -eq "RZAPPRNP01VD")
{
    $url = "http://fluxos-dev.raizen.com"
	$ambiente = "2";
}

if($env:computername -eq "RZAPPRNP01VQ" -or $env:computername -eq "RZAPPRNP02VQ")
{
    $url = "http://fluxos-qas.raizen.com"
	$ambiente = "3";
}

if($env:computername -eq "RZAPPRNP01V" -or $env:computername -eq "RZAPPRNP02V")
{
    $url = "http://fluxos.raizen.com"
	$ambiente = "4";
}

Write-Host -ForegroundColor Green "Ambiente:" $url;
#################### Ambiente #########################

$sites = get-spsite $url

if(!$sites)
{
    Write-Host -ForegroundColor Red "Erro ao obter o Site Collection! O script será finalizado... "
    break
}

[void][System.Reflection.Assembly]::LoadWithPartialName("PortalDeFluxos.Core.BLL");

$rootWeb = $sites.RootWeb

#################### Ambiente #########################

Write-Host ""
Write-Host "Iniciando configuração do Ambiente ..."

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveAmbiente)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveAmbiente, $ambiente);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChaveAmbiente] = $ambiente;
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração do Ambiente finalizada!"
Write-Host ""

#################### Url Record Center #########################

if ($sites -ne $null) {
    $sites.dispose();
}

if ($rootWeb -ne $null) {
    $rootWeb.dispose();
}


Write-Host "Reiniciando os serviços do Sharepoint" 
IISRESET
NET STOP SPTIMERV4
NET START SPTIMERV4