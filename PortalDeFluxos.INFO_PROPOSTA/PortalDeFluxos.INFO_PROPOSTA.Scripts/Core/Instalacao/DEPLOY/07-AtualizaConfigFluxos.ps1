if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;
$scriptPath = (Resolve-Path .\).Path;
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
$listaConfigFluxo = $web.Lists["Raízen - Configurações de Fluxo"];



#################### Buscando CSV #########################    
$csvFolderPath = $scriptPath + "\CSV"
$sourceCsvPath = $csvFolderPath + "\ConfigFluxosGrupos.csv"
#################### Buscando CSV #########################

        
Import-CSV $sourceCsvPath -Encoding UTF8 -Delimiter ';' |
    ForEach-Object {
        $IdItem = $_.ID;
        $nomeGrupo = $_.Grupo
        $grupo = $web.SiteGroups.GetByName($nomeGrupo);

        Write-Host "Atualizando item " $IdItem ": inserindo grupo " $nomeGrupo;

        $itemConfig = $listaConfigFluxo.GetItemById($IdItem);
        $itemConfig["Grupo"] = $grupo;
        $itemConfig.Update();
                
        

}


$web.Dispose();
$site.Dispose();

Write-Host "Script encerrado."

