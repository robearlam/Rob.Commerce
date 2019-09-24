#Do json transforms
$azureDeployParamsJson = "./AzureDeployParams.json"
$azureDeployParams = Get-Content $azureDeployParamsJson -Raw | ConvertFrom-Json
$certificateNode = $azureDeployParams.FileTransforms

Foreach($fileTransform in $azureDeployParams.FileTransforms)
{
	Write-Host "Transforming: $($azureDeployParams.webroot)$($fileTransform.FileName)"
	$file = Get-Content "$($azureDeployParams.webroot)$($fileTransform.FileName)" -Raw | ConvertFrom-Json

	Foreach($transform in $fileTransform.Transforms)
	{
		Write-Host "	Transforming: $($transform.selector)"
		Foreach($policy in $file.Policies)
		{
			Write-Host $policy[0]
		}
	}
}


#$certificateNode.Thumbprint = $Thumbprint  
#$appSettingsNode = $originalJson.AppSettings
#$appSettingsNode.EnvironmentName = $EnvironmentName
#originalJson | ConvertTo-Json -Depth 100 -Compress | set-content $pathToJson


#Set correct Search configs