
if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

#################### Ambiente #########################
$url = "http://pi";
$farmAdmin = "Administrator"

if($env:computername -eq "PI")
{
    $url = "http://pi"
	$farmAdmin = "Administrator"
}

if($env:computername -eq "RZAPPRNP01VD")
{
    $url = "http://fluxos-dev.raizen.com"
	$farmAdmin = "SPRNIPDEV_FARM"
}

if($env:computername -eq "RZAPPRNP01VQ" -or $env:computername -eq "RZAPPRNP02VQ")
{
    $url = "http://fluxos-qas.raizen.com"
	$farmAdmin = "SPRNIPPOC_FARM"
}

if($env:computername -eq "RZAPPRNP01V" -or $env:computername -eq "RZAPPRNP02V")
{
    $url = "http://fluxos.raizen.com"
	$farmAdmin = "SPRNIPPRD_FARM"
}

Write-Host -ForegroundColor Green "Ambiente:" $url;
#################### Ambiente #########################

$site = Get-SPSite $url;
$web   = Get-SPWeb  $url;


#################### Buscando CSV #########################
$scriptPath = (Resolve-Path .\).Path
$folderPath = $scriptPath + "\CSV"
$sourceCsvPath = $folderPath + "\ListasDeletar.csv"
#################### Buscando CSV #########################


Import-CSV $sourceCsvPath -Encoding UTF8 -Delimiter ';' |
 ForEach-Object {
        $Name = $_.Nome;
        $lista = $web.Lists.TryGetList($Name);
        if ($lista -ne $null)
        {
            Write-Host "Lista" $Name "existe. Deletando lista.";
            $lista.Delete();
        }
}

$web.Update();
$web.Dispose();
$site.Dispose();