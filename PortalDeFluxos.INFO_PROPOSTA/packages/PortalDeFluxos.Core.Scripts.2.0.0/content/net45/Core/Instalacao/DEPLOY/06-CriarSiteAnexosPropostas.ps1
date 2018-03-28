if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

$RecordCenterName = "AnexosPropostas";
$RecordCenterTemplate = "OFFILE#1";

#################### URL Site #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os dados do ambiente ..."
$url = Read-Host "WebCollection destino ex: http://fluxos-dev.raizen.com"

#################### URL Site #########################

$sites = Get-SPSite $url;
$web   = Get-SPWeb  $url;

#################### Proprietário do Site #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os login do proprietário do site ex:(COSAN\mosswebservices) ..."
$loginProprietario = Read-Host "Login do proprietário do Site: "

#################### Proprietário do Site #########################

#################### Proprietário Secundario do Site #########################

Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe os login do proprietário Secundario do site ex:(COSAN\TR007045) ..."
$loginProprietarioSecundario = Read-Host "Login do proprietário do Site: "

#################### Proprietário Secundario do Site #########################

$siteUrl = $sites.Url + "/sites/$RecordCenterName";
$databaseName = "WSS_Content_$RecordCenterName";
$databaseServer = $sites.ContentDatabase.DatabaseConnectionString.Split(";")[0].Split("=")[1];

#Create new content database for DMS department site collection
New-SPContentDatabase -Name $databaseName -DatabaseServer $databaseServer -WebApplication $url;

#Create new site collection for DMS department
New-SPSite -Url $siteUrl -OwnerAlias $loginProprietario -SecondaryOwnerAlias $loginProprietarioSecundario -ContentDatabase $databaseName -Template $RecordCenterTemplate -Name $RecordCenterName;

Write-Host "" 
Write-Host -ForegroundColor Green "Record Center criado!"



if($sites)
{
    $sites.Dispose()
    $sites.Close()
    $sites = $null
}