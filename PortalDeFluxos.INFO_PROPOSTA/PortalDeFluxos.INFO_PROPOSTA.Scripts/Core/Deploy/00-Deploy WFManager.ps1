CLS

$services = "Workflow Manager Backend"
$statusStopped = "Stopped"
$statusRunning = "Running"
$currentPath = Resolve-Path .
$dllsPath = "{0}\{1}" -f $currentPath, 'DLLs'
$targetArtifacts = "C:\Program Files\Workflow Manager\1.0\Workflow\Artifacts"
$targetWDRoot = "C:\Program Files\Workflow Manager\1.0\Workflow\WFWebRoot\bin"
$dlls = @(
              "$dllsPath\PortalDeFluxos.Core.BLL.dll"
			 ,"$dllsPath\PortalDeFluxos.Core.ActivityLibrary.dll"
			 ,"$dllsPath\ImapX.dll"
           )

function WaitUntilServices($searchString, $status)
{
    # Get all services where DisplayName matches $searchString and loop through each of them.
    foreach($service in (Get-Service -DisplayName $searchString))
    {
        # Wait for the service to reach the $status or a maximum of 210 seconds
        $service.WaitForStatus($status, '00:03:30')
    }
}

Stop-Service $services

WaitUntilServices $services $statusStopped

foreach($dll in $dlls)
{
    Copy-Item $dll $targetArtifacts -Force
    Copy-Item $dll $targetWDRoot -Force
}

Start-Service "Workflow Manager Backend"

WaitUntilServices $services $statusRunning

iisreset.exe

[System.DateTime]::Now