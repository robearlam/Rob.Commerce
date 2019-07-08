param 
( 
	[Parameter(Mandatory = $true)]
    [string]$Thumbprint,
	[Parameter(Mandatory = $true)]
    [string]$EngineConnectIncludeDir
)


Write-Host "Transforming CommerceConnect Certificate thumbprint";
$pathToConfig = $(Join-Path -Path $EngineConnectIncludeDir -ChildPath "\Sitecore.Commerce.Engine.Connect.config") 
$xml = [xml](Get-Content $pathToConfig)
$node = $xml.configuration.sitecore.commerceEngineConfiguration
$node.certificateThumbprint = $Thumbprint
$xml.Save($pathToConfig)  