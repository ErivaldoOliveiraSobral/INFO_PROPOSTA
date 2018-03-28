if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

#################### Ambiente #########################
$url = "http://pi";
$nomeGrupoAdmin = "Grupo Administradores Grupos";
$usersAdmin = "tr012192,tr007045,cs131641,cs243594,tr014908";

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


$grupoAdmin = $web.SiteGroups.GetByName($nomeGrupoAdmin);

for($i = 0; $i -le $web.SiteGroups.Count - 1; $i++)    
{
    $grupoAtual = $web.SiteGroups[$i]
    if($grupoAtual.Name -ne $nomeGrupoAdmin)
    {
        Write-Host "Atualizando ... " $grupoAtual.Name;
        $grupoAtual.Owner = $grupoAdmin
        $grupoAtual.Update()
        Write-Host -ForegroundColor Green  "Grupo atualizado com sucesso!";
    }
}

$arrayUsers = $usersAdmin -split ","
for($i = 0; $i -le $arrayUsers.Count - 1; $i++)    
{
    Write-Host "Adicionando ... " $arrayUsers[$i];

    $userAtual = $web.EnsureUser($arrayUsers[$i])
    Set-SPUser -Identity $userAtual -Web $url -Group $grupoAdmin

    Write-Host -ForegroundColor Green  "Atualizado com sucesso!";
}


if ($sites -ne $null) {
    $sites.dispose();
}

if ($web -ne $null) {
    $web.dispose();
}