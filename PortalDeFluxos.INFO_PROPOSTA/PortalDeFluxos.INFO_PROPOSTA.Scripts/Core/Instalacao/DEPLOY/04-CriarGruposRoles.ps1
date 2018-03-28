
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

#################### Farm Admin #########################
Write-Host ""

# To get the Owner and member for other sites
$GroupOwner = $web.EnsureUser($farmAdmin);
$GroupMember = $web.EnsureUser($farmAdmin);
#################### Farm Admin #########################

#################### Buscando CSV #########################
$scriptPath = (Resolve-Path .\).Path
$folderPath = $scriptPath + "\CSV"
$sourceCsvPath = $folderPath + "\GruposSharepoint.csv"
#################### Buscando CSV #########################


Import-CSV $sourceCsvPath -Encoding UTF8 -Delimiter ';' |
 ForEach-Object {
        $Name = $_.Nome;
        $arrayRoles = $_.Roles -split ",";
        $grupo = $null;
        Write-Host "Procurando grupo " $Name;
        Try
        {
            $grupo = $web.SiteGroups.GetByName($Name);
        }
        Catch
        {
            Write-Host "Grupo não encontrado!";
        }
        if ($grupo -eq $null)
        {        
            Write-Host "Criando grupo " $Name;
            $web.SiteGroups.Add($Name, $GroupOwner, $GroupOwner, $Name);
            $web.Update();
            Write-Host "Grupo criado com sucesso!";
            $grupo = $web.SiteGroups.GetByName($Name);            
            $web.Update();
        }        
        $grupo.OnlyAllowMembersViewMembership = 0;
        $grupo.Update();
        for($i = 0; $i -le $arrayRoles.Count - 1; $i++)
        {
            $roleDefinition = $null
            $roleDefinition = $web.RoleDefinitions[$arrayRoles[$i]]
            if($roleDefinition -eq $null)
            {
                if ($arrayRoles[$i] -eq "Leitura")
				{
					$roleDefinition = $web.RoleDefinitions["Read"];
				}
                if ($arrayRoles[$i] -eq "Contribute")
                {
                    $roleDefinition = $web.RoleDefinitions["Colaboração"];
                }
            }
                        
            Write-Host "Atribuindo ao grupo " $Name "a role " $arrayRoles[$i];
            $RoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($grupo);
            $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
            $web.RoleAssignments.Add($roleAssignment);
        }

}

$web.Update();
$web.Dispose();
$site.Dispose();