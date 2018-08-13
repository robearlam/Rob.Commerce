param 
( 
    [Parameter(Mandatory = $true)]
    [string]$DatabaseServer, 
	[Parameter(Mandatory = $true)]
    [string]$Thumbprint,
	[Parameter(Mandatory = $true)]
    [string]$EngineConnectIncludeDir,
	[Parameter(Mandatory = $true)]
    [string[]]$EngineRootPaths
)

Write-Host "Transforming CommerceConnect Certificate thumbprint";
$pathToConfig = $(Join-Path -Path $EngineConnectIncludeDir -ChildPath "\Sitecore.Commerce.Engine.Connect.config") 
$xml = [xml](Get-Content $pathToConfig)
$node = $xml.configuration.sitecore.commerceEngineConfiguration
$node.certificateThumbprint = $Thumbprint
$xml.Save($pathToConfig)  

Write-Host "Transforming Commerce Engine Certificate thumbprint";
foreach ($path in $EngineRootPaths) {
    $pathToJson = $(Join-Path -Path $path -ChildPath "wwwroot\config.json") 
    $originalJson = Get-Content $pathToJson -Raw | ConvertFrom-Json
    $certificateNode = $originalJson.Certificates.Certificates[0]
    $certificateNode.Thumbprint = $Thumbprint       
    $originalJson | ConvertTo-Json -Depth 100 -Compress | set-content $pathToJson
} 