param 
( 
	[Parameter(Mandatory = $true)]
    [string]$RolePath 
)

Write-Host "Deleting existing instance files from $RolePath\*";
Remove-Item –Path "$RolePath\*" -Force -Recurse

Write-Host "Copying engine instance to role $RolePath";
Copy-Item "src\Project\Sitecore.Commerce.Engine\bin\publish\*" -Destination $RolePath -Recurse