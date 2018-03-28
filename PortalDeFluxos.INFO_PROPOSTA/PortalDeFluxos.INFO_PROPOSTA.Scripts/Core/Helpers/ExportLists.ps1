if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

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


#################### Nome da lista #########################
Write-Host ""
Write-Host -ForegroundColor Green "Por favor, informe o nome da lista ..."
$nomeLista = Read-Host "Nome da lista: ex:Aditivos Gerais"
#################### Nome da lista #########################


$site = Get-SPSite $url;
$web   = Get-SPWeb  $url;
$scriptPath = (Resolve-Path .\).Path


$lista = $web.Lists.TryGetList($nomeLista);  

if ($lista)
{
    Write-Host "Exportando lista " $nomeLista
    $urlLista = "/lists/" + $lista.DefaultViewUrl.Split("//")[2]
    $scriptPath = $scriptPath +"\\"+ $lista.DefaultViewUrl.Split("//")[2]
	Export-SPWeb -Identity $web.Url -ItemUrl $urlLista -path $scriptPath -NoLogFile
       
    Write-Host "Concluído"
}

Write-Host "Script encerrado."

$web.Dispose();
$site.Dispose();