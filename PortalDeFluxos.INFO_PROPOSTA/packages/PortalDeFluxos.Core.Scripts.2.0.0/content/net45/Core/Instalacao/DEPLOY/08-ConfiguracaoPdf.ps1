## Configuração ServicosEmailAttachment ##

add-pssnapin microsoft.sharepoint.powershell -EA 0

#################### Ambiente #########################
$url = "http://pi";

if($env:computername -eq "PI")
{
    $url = "http://pi"
}

if($env:computername -eq "RZAPPRNP01VD")
{
    $url = "http://fluxos-dev.raizen.com"
}

if($env:computername -eq "RZAPPRNP01VQ" -or $env:computername -eq "RZAPPRNP02VQ")
{
    $url = "http://fluxos-qas.raizen.com"
}

if($env:computername -eq "RZAPPRNP01V" -or $env:computername -eq "RZAPPRNP02V")
{
    $url = "http://fluxos.raizen.com"
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

#################### Url Record Center #########################
$pdfKey = "rIedjJ2Mnp+YjJmCnIyfnYKdnoKVlZWV"
Write-Host ""
Write-Host "Iniciando configuração do Pdf ..."

if (!$rootWeb.AllProperties.ContainsKey([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChavePdf)) {
    $rootWeb.AllProperties.Add([PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChavePdf, $pdfKey);
}
else {
    $rootWeb.AllProperties[[PortalDeFluxos.Core.BLL.Utilitario.Constantes]::ChavePdf] = $pdfKey;
}

$rootWeb.Update();

Write-Host -ForegroundColor Green "Configuração do Pdf finalizada!"
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