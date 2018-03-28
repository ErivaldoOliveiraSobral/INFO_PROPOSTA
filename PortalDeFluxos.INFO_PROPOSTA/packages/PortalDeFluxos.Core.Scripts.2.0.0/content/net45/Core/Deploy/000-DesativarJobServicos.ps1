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


$job = Get-SPTimerJob –WebApplication $url | Where-Object {$_.name –like "Core - Portal de Fluxos TimerJob"}
$job | Disable-SPTimerJob
$job | select DisplayName,LastRunTime,Status,Schedule, IsDisabled

$sites = Get-SPSite $url;

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString  = $sites.RootWeb.AllProperties["PortalDeFluxos.Core.ConnectionString"]
$conn.open()

$cmd = New-Object System.Data.SqlClient.SqlCommand
$cmd.connection = $conn
$cmd.commandtext = "
					IF EXISTS(select ativo from dbo.ServicoAgendado where NomeAssemblyType = 'PortalDeFluxos.Core.Servicos.SincronizarStatusFluxo')
					BEGIN
						UPDATE dbo.ServicoAgendado
						SET ativo = 0
						WHERE NomeAssemblyType = 'PortalDeFluxos.Core.Servicos.SincronizarStatusFluxo'
					END"

$cmd.executenonquery();
$cmd.Dispose()
$conn.Close();
$sites.Dispose();

Write-Host -ForegroundColor Green "`n Desativando "$job.DisplayName
Write-Host -ForegroundColor Yellow "`n Informar ao analista responsável e aguardar o OK para continuar!"]]

NET STOP SPTIMERV4
NET START SPTIMERV4