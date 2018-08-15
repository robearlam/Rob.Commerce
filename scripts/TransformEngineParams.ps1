param 
( 
    [Parameter(Mandatory = $true)]
    [string]$DatabaseServer, 
	[Parameter(Mandatory = $true)]
    [string]$Thumbprint,
	[Parameter(Mandatory = $true)]
    [string]$EngineConnectIncludeDir,
	[Parameter(Mandatory = $true)]
    [string]$EngineRolesJson
)

Write-Host "Transforming CommerceConnect Certificate thumbprint";
$pathToConfig = $(Join-Path -Path $EngineConnectIncludeDir -ChildPath "\Sitecore.Commerce.Engine.Connect.config") 
$xml = [xml](Get-Content $pathToConfig)
$node = $xml.configuration.sitecore.commerceEngineConfiguration
$node.certificateThumbprint = $Thumbprint
$xml.Save($pathToConfig)  

Write-Host "Transforming Commerce Engine Certificate thumbprint";

Write-Host $EngineRolesJson;

$engineRoles = $EngineRolesJson | ConvertFrom-Json


Write-Host $engineRoles

Write-Host "End";
<#
foreach ($engineInstance in $engineRoles) {
    $pathToJson = $(Join-Path -Path $engineInstance.path -ChildPath "wwwroot\config.json") 
    $originalJson = Get-Content $pathToJson -Raw | ConvertFrom-Json
    $certificateNode = $originalJson.Certificates.Certificates[0]
    $certificateNode.Thumbprint = $Thumbprint  
	$appSettingsNode = $originalJson.AppSettings
	$appSettingsNode.EnvironmentName = $engineInstance.environmentName
    $originalJson | ConvertTo-Json -Depth 100 -Compress | set-content $pathToJson
} 
#>