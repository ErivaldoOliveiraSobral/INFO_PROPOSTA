
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
$sourceCsvPath = $folderPath + "\ListasBackup.csv"
$dataAtual = Get-Date -UFormat "%Y%m%d";

#################### Buscando CSV #########################


Import-CSV $sourceCsvPath -Encoding UTF8 -Delimiter ';' |
 ForEach-Object {
        $Name = $_.Nome;
		Write-Host -ForegroundColor Yellow "Exportando lista :" $Name;
        Echo "Exportando lista :" $Name >> BKP_Listas_Log.txt;

		$urlLista = "/lists/" + $Name;
		$folderPath = $scriptPath + "\Listas\" + $Name + "_" + $dataAtual;
        Export-SPWeb -Identity $web.Url -ItemUrl $urlLista -path $folderPath -NoLogFile -ErrorAction SilentlyContinue -ErrorVariable erro;

        if ($erro)
        {
            Write-Host -ForegroundColor Red "Erro ao exportar lista " $Name ". Erro: " $erro; 
            Echo "Erro ao exportar lista " $Name ". Erro: " $erro >> BKP_Listas_Log.txt; 
        }
        else
        {
            Write-Host -ForegroundColor Green "Lista exportada com sucesso."; 
            Echo "Lista exportada com sucesso." >> BKP_Listas_Log.txt; 
        }
}
$web.Dispose();
$site.Dispose();
