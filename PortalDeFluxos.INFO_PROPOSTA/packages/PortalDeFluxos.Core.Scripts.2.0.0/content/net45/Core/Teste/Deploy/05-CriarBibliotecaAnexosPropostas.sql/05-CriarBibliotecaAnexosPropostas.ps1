if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

$RecordCenterName = "AnexosPropostas";
$listTemplate = [Microsoft.SharePoint.SPListTemplateType]::DocumentLibrary
$spFieldUserType = [Microsoft.SharePoint.SPFieldType]::User

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
$web   = Get-SPWeb  $url;
$urlDocumentCenter = $sites.Url + "/sites/$RecordCenterName";

$sitesDocumentCenter = Get-SPSite $urlDocumentCenter;
$webDocumentCenter   = Get-SPWeb  $urlDocumentCenter;

#################### Lista Alvo #########################

Write-Host ""
#Write-Host -ForegroundColor Green "Por favor, informe o nome da lista a ser configurada ..."
#$displayName = Read-Host "Display Name da lista ex: Aditivos Gerais"
$displayName = "FormularioTeste"

#################### Lista Alvo #########################

#################### Verifica se o script Template Existe #########################

$list = $sites.RootWeb.Lists.TryGetList($displayName)
if($list)
{
    Write-Host ""
    Write-Host "Iniciando o processo de configuração ... "
    Write-Host ""

    $internalName = $list.DefaultViewUrl.Split("//")[2]
    $documentLibrary = $sitesDocumentCenter.RootWeb.Lists.TryGetList($displayName);
    if ($documentLibrary -eq $null)
    {
        $documentLibrary = $sitesDocumentCenter.RootWeb.Lists.Add($internalName,$internalName,$listTemplate)
        $documentLibrary = $sitesDocumentCenter.RootWeb.Lists.TryGetList($internalName)
        
        if($documentLibrary)
        {
            ForEach($culture in $sitesDocumentCenter.RootWeb.SupportedUICultures)
            {
                [System.Threading.Thread]::CurrentThread.CurrentUICulture = $culture;
                $documentLibrary.Title = $list.Title
                $documentLibrary.Description = "Biblioteca de anexos da lista: " + $list.Description
                $a = $documentLibrary.Fields.Add("Usuario",$spFieldUserType,$false)
                $documentLibrary.Update();
            }
        }

        Write-Host -ForegroundColor Green "A lista:" $displayName "foi configurada com sucesso!"
    }
    else
    {
        Write-Host -ForegroundColor Green "A lista" $displayName "já existe! Encerrando o script."
    }
}else 
{
    Write-Host -ForegroundColor Red "Esta lista não existe! O script será finalizado... "
    break
}

#################### Verifica se o script Template Existe #########################


if($sites)
{
    $sites.Dispose()
    $sites.Close()
    $sitesDocumentCenter.Dispose()
    $sitesDocumentCenter.Close()
    $sites = $null
    $sitesDocumentCenter = $null
}
