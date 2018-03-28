Import-Module "WebAdministration"
$scriptPath = (Resolve-Path .\).Path
$wspFilesPath = $scriptPath + '\Dlls'
$wspRaizenName = "PortalDeFluxos.Core.SP.wsp"
$wspRaizenPath = "{0}\{1}" -f $wspFilesPath, $wspRaizenName
$AppPoolName="ServicosPool"
$BackUpPath = ".\Dlls\servicos.zip"
$SitesPath = "C:\Sites"
$Destination = "C:\Sites\Servicos"
$webConfigPath = "C:\Sites\Servicos\web.config"

Add-Type -assembly "system.io.compression.filesystem"

#################### Ambiente #########################
$stringconfig = 'http://sp2013/_vti_bin/ExcelService.asmx'

if($env:computername -eq "PI")
{
    $stringconfig = 'http://pi/_vti_bin/ExcelService.asmx'
}

if($env:computername -eq "RZAPPRNP01VD")
{
    $stringconfig = 'http://fluxos-dev.raizen.com/_vti_bin/ExcelService.asmx'
}

if($env:computername -eq "RZAPPRNP01VQ" -or $env:computername -eq "RZAPPRNP02VQ")
{
    $stringconfig = 'http://fluxos-qas.raizen.com/_vti_bin/ExcelService.asmx'
}

if($env:computername -eq "RZAPPRNP01V" -or $env:computername -eq "RZAPPRNP02V")
{
    $stringconfig = 'http://fluxos.raizen.com/_vti_bin/ExcelService.asmx'
}


Write-Host -ForegroundColor Green "Ambiente:" $env:computername

If(!(test-path $SitesPath))
{
	New-Item -ItemType Directory -Force -Path $SitesPath
}
If(test-path $Destination){
Remove-Item $Destination -recurse
}

[io.compression.zipfile]::ExtractToDirectory($BackUpPath, $destination)
Write-Host "Extraindo site configurando IIS" -ForegroundColor Green
$xml = [xml](get-content $webConfigPath)
$xml.configuration.applicationSettings.'PortalDeFluxos.Core.ServicosWeb.Properties.Settings'.setting.value= $stringconfig
$xml.Save($webConfigPath)


if(Test-Path IIS:\AppPools\$AppPoolName)
{
"Site configurado"
return $true;
}
else
{

$User = Read-Host -Prompt 'Insira o usuario para configuração do pool do IIS'
$Pass = Read-Host -Prompt 'Senha'

New-WebAppPool $AppPoolName -Force
Set-ItemProperty IIS:\AppPools\ServicosPool -name processModel -value @{userName=$User;password=$Pass;identitytype=3}
New-Item IIS:\Sites\Servicos -physicalPath C:\Sites\Servicos -bindings @{protocol="http";bindingInformation=":8085:"} -Force
Set-ItemProperty IIS:\Sites\Servicos -name applicationPool -value ServicosPool
return $false;
}