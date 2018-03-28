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
$sourceCsvPathAtual = $csvFolderPath + "\QuickLaunch.csv"

Import-CSV $sourceCsvPathAtual -Encoding UTF8 -Delimiter ';' |
    ForEach-Object {
        $name = $_.Name;
        $link = $_.Link;
        $node = $_.Node;
        $afterNode = $_.AfterNode
        
        $quickLaunch = $web.navigation.quicklaunch
        $libheading = $quickLaunch | where { $_.Title -eq $node }
        $newnode = New-Object Microsoft.SharePoint.Navigation.SPNavigationNode($name, $link, $false)

        if($afterNode  -eq ""){
            $existingLink = $heading.Children | where {$_.title -eq $afterNode}
            $libheading.Children.Add($newnode,$existingLink)
        }
        else{
            $libheading.Children.AddAsLast($newnode)
        }
		
		Write-Host -ForegroundColor Green "Atribuindo ao Quick Launch o link: " $name;
}

$web.Update();
$web.Dispose();
$site.Dispose();