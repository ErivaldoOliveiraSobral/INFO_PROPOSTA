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

$scriptPath = (Resolve-Path .\).Path
$wspFilesPath = $scriptPath + '\WSP'
$wspRaizenName = "PortalDeFluxos.Core.SP.wsp"
$wspRaizenPath = "{0}\{1}" -f $wspFilesPath, $wspRaizenName
$sites = Get-SPSite $url;
$web   = Get-SPWeb  $url;
$listaPaginas = $web.Lists.TryGetList("Páginas do Site");

### Apga as páginas do sistema
function ApagarPagina($nomePagina){
    $pagina = $listaPaginas.Items | Where { $_.Name -eq $nomePagina}
    if ($pagina -ne $null) {
        $pagina.Delete();
    }
}

### Método para reativar features
function AtivarFeature($nome, $scopo, $somenteDesativa){
    $feature = Get-SPFeature | Where {$_.Displayname -eq $nome}
    If($feature)
    {
        Write-Host ""
        $ativado = $scopo.Features | Where {$_.DefinitionId -eq $feature.Id}
        If($ativado -eq $null)
        {
			if ($somenteDesativa -eq 0) {
				Write-Host -ForegroundColor Green "Ativando a feature $($nome)"
				Enable-SPFeature -Identity $nome -Url $url 
				Write-Host -ForegroundColor Green "Feature ativada!"
				}
        }else
        {
    	    Write-Host -ForegroundColor Green "Desativando a feature $($nome)"
            Disable-SPFeature -Identity $nome -Url $url -Confirm:$false -Force
            if ($somenteDesativa -eq 0) {
	    	    Write-Host "Ativando a feature $($nome)"
	    	    Enable-SPFeature -Identity $nome -Url $url
                Write-Host -ForegroundColor Green "Feature ativada!"
	        }
        }
    }
}

#Efetua o deploy da solution
$solution = Get-SPSolution -Identity $wspRaizenName -ErrorAction SilentlyContinue
if ($solution -eq $null -or $solution.Deployed -eq $false) {
    Add-SPSolution -LiteralPath $wspRaizenPath
	Install-SPSolution  -WebApplication $sites.WebApplication.Url -Identity $wspRaizenName -GACDeployment -Force
} else {
    Write-Host "Iniciando atualização"
    Update-SPSolution -GACDeployment -Identity $wspRaizenName -LiteralPath $wspRaizenPath -Force -Confirm:$false
    
    ## Espera a conclusão do deploy
    $Solution = Get-SPSolution -identity $wspRaizenName
    $lastStatus = ""
    ## Verifica se já foi feito o Deploy
    write-host "" 
    while ($Solution.JobExists -eq $True) 
    { 
        $currentStatus = $Solution.JobStatus 
        if ($currentStatus -ne $lastStatus) 
        { 
	        Write-Host ""
            Write-Host "$currentStatus…" -foreground Green -nonewline
            $lastStatus = $currentStatus 
        }
    
        write-host "." -nonewline -foreground Green
        sleep 1
    } 

    ## Deploy Realizado 
    Write-Host "" 
    Write-Host "     " $Solution.LastOperationDetails -foreground green

    #Reinicia os serviços
    Write-Host "Reiniciando os serviços do Sharepoint" 
    IISRESET
    NET STOP SPTIMERV4
    NET START SPTIMERV4
}

## Espera a conclusão do deploy
$Solution = Get-SPSolution -identity $wspRaizenName
$lastStatus = ""
## Verifica se já foi feito o Deploy
write-host "" 
while ($Solution.JobExists -eq $True) 
{ 
    $currentStatus = $Solution.JobStatus 
    if ($currentStatus -ne $lastStatus) 
    { 
	Write-Host ""
        Write-Host "$currentStatus…" -foreground Green -nonewline
        $lastStatus = $currentStatus 
    }
    
    write-host "." -nonewline -foreground Green
    sleep 1
} 

Write-Host ""
Write-Host -ForegroundColor Green "PortalDeFluxos.Core.SP Adicionado."

#Apaga as páginas

#ApagarPagina "PaginaAprovacao.aspx"
ApagarPagina "PainelAcompanhamento.aspx"
ApagarPagina "PainelHistorico.aspx"
ApagarPagina "DetalhesSolicitacao.aspx"
#ApagarPagina "PaginaDelegacao.aspx"
ApagarPagina "Log.aspx"
ApagarPagina "LogErro.aspx"
ApagarPagina "CancelarFluxo.aspx"
ApagarPagina "TesteWIF.aspx"
ApagarPagina "EmailAnexoProposta.aspx"
ApagarPagina "FluxosRaizen.aspx"
ApagarPagina "ServicosRaizen.aspx"
ApagarPagina "LogRaizen.aspx"
ApagarPagina "RelatorioOperacionalTarefas.aspx"
ApagarPagina "UsuariosGruposRaizen.aspx"
ApagarPagina "DelegacaoTarefaRaizen.aspx"

if ($sites -ne $null) {
    $sites.dispose();
}
if ($web -ne $null) {
    $web.dispose();
}
$sites = Get-SPSite $url;
$web   = Get-SPWeb  $url;

#reativa as features

AtivarFeature "PortalDeFluxos.Core.SP_PortalDeFluxos.Core.Infraestrutura" $web 0                 #Reativa feature de InfraEstrutura
AtivarFeature "PortalDeFluxos.Core.SP_PortalDeFluxos.Core.WebParts" $sites 0                     #Reativa feature de WebParts
AtivarFeature "PortalDeFluxos.Core.SP_PortalDeFluxos.Core.Paginas" $sites 0                      #Reativa feature de Paginas
AtivarFeature "PortalDeFluxos.Core.SP_PortalDeFluxos.Core.CustomActivity" $web 0                 #Reativa feature de CustomActivity - somente quando necessário - o fluxo deprecated ocorre nesse passo.
AtivarFeature "PortalDeFluxos.Core.SP_PortalDeFluxos.Core.Servicos" $web.Site.WebApplication 0   #Reativa feature de Servicos

#Ativa feature de fluxo App
#$feature1 = "WorkflowAppOnlyPolicyManager";
#Enable-SPFeature -Identity $feature1 -Url $url;

if ($sites -ne $null) {
    $sites.dispose();
}

if ($web -ne $null) {
    $web.dispose();
}

IISRESET
NET STOP SPTIMERV4
NET START SPTIMERV4


Write-Host "" 
Write-Host -ForegroundColor Green "Atualização Concluída!"
