 
if ( (Get-PSSnapin -Name "Microsoft.SharePoint.Powershell" -ErrorAction SilentlyContinue) -eq $null ) {
    Add-PsSnapin "Microsoft.SharePoint.Powershell"
}
cls;

$scriptPath = (Resolve-Path .\).Path;

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

function AtribuirRoles($nomeLista,$nomeCsvGrupo){   

    $lista = $web.Lists[$nomeLista];
    $sourceCsvPathAtual = $csvFolderPath + "\" + $nomeCsvGrupo

    Write-Host ""
	Write-Host ""
	Write-Host -ForegroundColor Green "Atribuindo grupos aos items da Lista: " $nomeLista;

  Import-CSV $sourceCsvPathAtual -Encoding UTF8 -Delimiter ';' |
    ForEach-Object {
        $nomePagina = $_.Nome;
        $arrayGrupoRoles = $_.GrupoRole

        foreach ($item in $lista.Items) 
        { 
            if ($item.DisplayName -eq $nomePagina) 
            { 
                $arrayGrupo =  $arrayGrupoRoles -split ","

                for($i = 0; $i -le $arrayGrupo.Count - 1; $i++)
                {
                    $grupoNameRoles = $arrayGrupo[$i] -split "-"
                    $grupoName =  $grupoNameRoles[0]
                    $arrayRoles = $grupoNameRoles[1] -split ":"
                    $grupo = $web.SiteGroups.GetByName($grupoName);

                    if ($item.HasUniqueRoleAssignments -eq $False)
                    {
                        $item.BreakRoleInheritance($false, $false);
                        $item.Update();
                    }
                  
                    for($j = 0; $j -le $arrayRoles.Count - 1; $j++)
                    {
                        $roleDefinition = $null
                        $roleDefinition = $web.RoleDefinitions[$arrayRoles[$j]]

                        if($roleDefinition -eq $null)
                        {
                            if ($arrayRoles[$j] -eq "Leitura" -or $arrayRoles[$j] -eq "Read")
				            {
					            $roleDefinition = $web.RoleDefinitions["Read"];
                                if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Leitura"];}
				            }

                            if ($arrayRoles[$j] -eq "Contribute" -or $arrayRoles[$j] -eq "Colaboração")
                            {
                                $roleDefinition = $web.RoleDefinitions["Contribute"];
                                if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Colaboração"];}
                            }

                            if ($arrayRoles[$j] -eq "Full Control" -or $arrayRoles[$j] -eq "Acesso Total")
                            {
                                $roleDefinition = $web.RoleDefinitions["Full Control"];
                                if($roleDefinition -eq $null){$roleDefinition = $web.RoleDefinitions["Acesso Total"];}
                            }
                        }

                        Write-Host "Atribuindo para a página:" $item.DisplayName "Grupo:" $grupoName "Role:" $arrayRoles[$j];
                        $RoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($grupo);
                        $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
                        $item.RoleAssignments.Add($roleAssignment);
                        $item.Update();
                    }
                }
            }
        } 
    }
}


AtribuirRoles "Páginas do Site" "GruposPaginas.csv"

$web.Update();
$web.Dispose();
$site.Dispose();

Write-Host "Script encerrado."
   
