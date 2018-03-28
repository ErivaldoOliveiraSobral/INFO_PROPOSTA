if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

#################### Ambiente #########################
$url = "http://pi";

if($env:computername -eq "PI")
{
    $url = "http://pi"if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

$nomeLista = "RNIPs"
$nomeColuna = "RNIPs"

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
$wspRaizenName = "PortalDeFluxos.RNIP.SP.wsp"
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
            Disable-SPFeature -Identity $nome -Url $url -Confirm:$false
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
	Install-SPSolution  -Identity $wspRaizenName -GACDeployment -Force
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
Write-Host -ForegroundColor Green "PortalDeFluxos.RNIP.SP Adicionado."


#Apaga as páginas
#ApagarPagina "PaginaAprovacao.aspx"
#ApagarPagina "PainelAcompanhamento.aspx"
#ApagarPagina "PainelHistorico.aspx"
#ApagarPagina "DetalhesSolicitacao.aspx"
#ApagarPagina "PaginaDelegacao.aspx"
#ApagarPagina "Log.aspx"
#ApagarPagina "CancelarFluxo.aspx"
#ApagarPagina "TesteWIF.aspx"
#ApagarPagina "EmailAnexoProposta.aspx"


if ($sites -ne $null) {
    $sites.dispose();
}
if ($web -ne $null) {
    $web.dispose();
}
$sites = Get-SPSite $url;
$web   = Get-SPWeb  $url;


##Elimina Colunas Status Fluxos
$list = $web.Lists[$nomeLista] ;
$column = $list.Fields[$nomeColuna] ;
$loop = $false;
if ($column -ne $null)
{
    $loop = $true;
}
$cont = 0;
while($loop -eq $true)
{
    $cont++;
    $column = $list.Fields[$nomeColuna] ;
    if ($column -eq $null)
    {
        $loop = $false;
    }
    else
    {
        $list.Fields.Delete($column);
        Write-Host "Coluna excluída!" $cont
        $list.Update();
    }
}

##reativa as features
AtivarFeature "PortalDeFluxos.RNIP.SP_EventReceiver" $web 0        #Reativa feature de WebParts
AtivarFeature "PortalDeFluxos.RNIP.SP_Fluxo" $web 0                #Reativa feature de Fluxo

##Atualizando tabela Lista

$list = $sites.RootWeb.Lists.TryGetList($nomeLista)
$DescricaoUrlItem = "{0}{1}{2}" -f $migrateTo, ($list.DefaultDisplayFormUrl -replace $list.ParentWeb.Url,""), "?ID="
$urlTarefa2013 = "{0}{1}{2}" -f $migrateTo, ($list.DefaultDisplayFormUrl -replace $list.ParentWeb.Url,""), "?IdTarefa="

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString  = $sites.RootWeb.AllProperties["PortalDeFluxos.Core.ConnectionString"]
$conn.open()

$cmd = New-Object System.Data.SqlClient.SqlCommand
$cmd.connection = $conn
$cmd.commandtext = "IF(NOT EXISTS(SELECT 1 FROM Lista WHERE CodigoLista = '{0}')) BEGIN INSERT INTO Lista (CodigoLista,Nome,DescricaoUrlLista,DescricaoUrlItem,DescricaoUrlTarefa,Ambiente2007) VALUES('{0}','{1}','{2}','{3}','{4}','{5}') END;" -f $list.ID,$nomeLista,$nomeLista,$DescricaoUrlItem,$DescricaoUrlItem,0
$cmd.executenonquery()

$cmd.connection = $conn
$cmd.commandtext = "UPDATE Lista SET DescricaoUrlTarefa='{0}' Where Nome='{1}';" -f $urlTarefa2013,$nomeLista
$cmd.executenonquery()

$conn.close()
##Atualizando tabela Lista

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