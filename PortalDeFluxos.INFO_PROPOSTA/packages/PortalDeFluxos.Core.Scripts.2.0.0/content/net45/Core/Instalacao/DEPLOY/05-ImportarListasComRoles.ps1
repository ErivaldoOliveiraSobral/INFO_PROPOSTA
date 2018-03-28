if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;
$scriptPath = (Resolve-Path .\).Path;
$folderPath = $scriptPath + "\Listas\";

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

$site = Get-SPSite $url;
$web   = Get-SPWeb  $url;

#Para cada arquivo

Get-ChildItem $folderPath -Filter *.cmp | Sort-Object name |
Foreach-Object {
    $content = $_.Name;
    Write-Host "Importando lista " $content;
    $path = $folderPath + $content;
    Write-Host "Caminho do cmp: " $path;
    Import-SPWeb -Identity  $web.Url  -Path $path -NoLogFile -Force
    Write-Host "Importação concluída";
}

$web.Dispose();
$site.Dispose();

Write-Host "Script encerrado."