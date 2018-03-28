if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

$scriptPath = (Resolve-Path .\).Path;
$folderPath = $scriptPath + "\Listas\";

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
$csvFolderPath = $scriptPath + "\CSV"

function AtribuirRoles($nomeLista,$nomeCsvGrupo,$visitantes){
    
    $lista = $web.Lists[$nomeLista];
    if (!$lista.HasUniqueRoleAssignments)
    {
        $lista.BreakRoleInheritance($false, $false);
        $lista.Update();
    }
    
    $sourceCsvPathAtual = $csvFolderPath + "\" + $nomeCsvGrupo

	Write-Host ""
	Write-Host ""
	Write-Host -ForegroundColor Green "Atribuindo grupos a Lista: " $nomeLista;

    Import-CSV $sourceCsvPathAtual -Encoding UTF8 -Delimiter ';' |
    ForEach-Object {
        $Name = $_.Nome;
        $arrayRoles = $_.Roles -split ","
        $grupo = $web.SiteGroups.GetByName($Name);

        for($i = 0; $i -le $arrayRoles.Count - 1; $i++)
        {
            $roleDefinition = $null
            $roleDefinition = $web.RoleDefinitions[$arrayRoles[$i]]
            if($roleDefinition -eq $null)
            {
                if ($arrayRoles[$i] -eq "Leitura" -or $arrayRoles[$i] -eq "Read")
				{
					$roleDefinition = $web.RoleDefinitions["Read"];
                    if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Leitura"];}
				}
                if ($arrayRoles[$i] -eq "Contribute" -or $arrayRoles[$i] -eq "Colaboração")
                {
                    $roleDefinition = $web.RoleDefinitions["Contribute"];
                    if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Colaboração"];}
                }
                if ($arrayRoles[$i] -eq "Full Control" -or $arrayRoles[$i] -eq "Acesso Total")
                {
                    $roleDefinition = $web.RoleDefinitions["Full Control"];
                    if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Acesso Total"];}
                }
            }
					

            Write-Host "Atribuindo ao grupo " $Name "a role " $arrayRoles[$i];
            $RoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($grupo);
            $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
            $lista.RoleAssignments.Add($roleAssignment);
            $lista.Update();
        }
    }


    if($visitantes)
    {
        for($i = 0; $i -le $web.SiteGroups.Count - 1; $i++)
        {
            if($web.SiteGroups[$i].Name.StartsWith("Visitante"))
            {
                $grupo = $web.SiteGroups[$i]
                $roleDefinition = $web.RoleDefinitions["Leitura"]
                if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Read"]}
                
                Write-Host "Atribuindo ao grupo " $grupo.Name "a role leitura";
                $RoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($grupo);
                $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
                $lista.RoleAssignments.Add($roleAssignment);
                $lista.Update();
            }
        }
    }
}

AtribuirRoles "RNIPs" "GruposLista.csv" $false
AtribuirRoles "Armazéns" "GruposListaArmazen.csv" $true
AtribuirRoles "Contratos" "GruposListaContrato.csv" $true
       
         

$web.Update();
$web.Dispose();
$site.Dispose();

Write-Host "Script encerrado."