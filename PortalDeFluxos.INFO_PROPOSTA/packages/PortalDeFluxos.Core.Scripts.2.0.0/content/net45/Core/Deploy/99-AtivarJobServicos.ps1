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

$sites = Get-SPSite $url;

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString  = $sites.RootWeb.AllProperties["PortalDeFluxos.Core.ConnectionString"];
$conn.open();

$cmd = New-Object System.Data.SqlClient.SqlCommand
$cmd.connection = $conn;
$cmd.commandtext = "
					IF EXISTS(select ativo from dbo.ServicoAgendado where NomeAssemblyType = 'PortalDeFluxos.Core.Servicos.SincronizarStatusFluxo')
					BEGIN
						UPDATE dbo.ServicoAgendado
						SET ativo = 1
						WHERE NomeAssemblyType = 'PortalDeFluxos.Core.Servicos.SincronizarStatusFluxo'
					END"
					
$cmd.executenonquery();
$cmd.Dispose();
$conn.Close();
$sites.Dispose();

$job = Get-SPTimerJob –WebApplication $url | Where-Object {$_.name –like "Core - Portal de Fluxos TimerJob" }
$job | Enable-SPTimerJob
$job | select DisplayName,LastRunTime,Status,Schedule, IsDisabled
Start-SPTimerJob $job

Write-Host -ForegroundColor Green "Iniciando "$job.DisplayName