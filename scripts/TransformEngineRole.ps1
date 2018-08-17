param 
( 
	[Parameter(Mandatory = $true)]
    [string]$Thumbprint,
	[Parameter(Mandatory = $true)]
    [string]$EnvironmentName,
	[Parameter(Mandatory = $true)]
    [string]$RolePath 
)


Write-Host "Transforming Engine Role at:" + $RolePath;

$pathToJson = $(Join-Path -Path $RolePath -ChildPath "wwwroot\config.json") 
$originalJson = Get-Content $pathToJson -Raw | ConvertFrom-Json
$certificateNode = $originalJson.Certificates.Certificates[0]
$certificateNode.Thumbprint = $Thumbprint  
$appSettingsNode = $originalJson.AppSettings
$appSettingsNode.EnvironmentName = $EnvironmentName
$originalJson | ConvertTo-Json -Depth 100 -Compress | set-content $pathToJson