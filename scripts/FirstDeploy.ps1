param 
( 
	[Parameter(Mandatory = $true)]
    [string]$EngineHostName,
	[Parameter(Mandatory = $true)]
    [string]$IdentityServerHostname,
	[Parameter(Mandatory = $true)]
    [string]$AdminPassword,
	[Parameter(Mandatory = $true)]
    [string]$AdminUser
)

Function Get-IdServerToken {
    $UrlIdentityServerGetToken = ("https://{0}/connect/token" -f $IdentityServerHostname)

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", 'application/x-www-form-urlencoded')
    $headers.Add("Accept", 'application/json')

    $body = @{
        password   = "$AdminPassword"
        grant_type = 'password'
        username   = ("sitecore\{0}" -f $adminUser)
        client_id  = 'postman-api'
        scope      = 'openid EngineAPI postman_api'
    }
    Write-Host "Getting Identity Token From Sitecore.IdentityServer"
    $response = Invoke-RestMethod $UrlIdentityServerGetToken -Method Post -Body $body -Headers $headers
    $sitecoreIdToken = "Bearer {0}" -f $response.access_token
    $global:sitecoreIdToken = $sitecoreIdToken
	Write-Host $global:sitecoreIdToken
}

Function CleanEnvironment {
    Write-Host "Cleaning Environments"
    $initializeUrl = ("https://{0}/commerceops/CleanEnvironment()" -f $EngineHostName)
    $Environments = @("AdventureWorksAuthoring","AdventureWorksMinions","AdventureWorksShops","HabitatAuthoring","HabitatMinions","HabitatShops")
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken);

    foreach ($env in $Environments) {
        Write-Host "Cleaning $($env) ..."
        $body = @{
            environment = $env
        }

        $result = Invoke-RestMethod $initializeUrl -TimeoutSec 1200 -Method Post -Headers $headers -Body ($body | ConvertTo-Json) -ContentType "application/json"
        if ($result.ResponseCode -eq "Ok") {
            Write-Host "Cleaning for $($env) completed successfully"
        }
        else {
            Write-Host "Cleaning for $($env) failed"
            Exit -1
        }
    }
}

Function BootStrapCommerceServices {
    Write-Host "BootStrapping Commerce Services..."
    $UrlCommerceShopsServicesBootstrap = ("https://{0}/commerceops/Bootstrap()" -f $EngineHostName)
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken)
    $result = Invoke-RestMethod $UrlCommerceShopsServicesBootstrap -TimeoutSec 1200 -Method PUT -Headers $headers 
	if ($result.ResponseCode -eq "Ok") {
        Write-Host "BootStrapping completed successfully"
    }
    else {
        Write-Host "BootStrapping failed"
        Exit -1
    }

    Write-Host "Commerce Services BootStrapping completed..."
}

Function SyncDefaultContentPaths {
	Write-Host "Syncing Default Content Paths..."
    
	#Make Request
	$UrlCommerceShopsServicesBootstrap = ("https://{0}/commerceops/EnsureSyncDefaultContentPaths(environment='HabitatAuthoring',shopName='CommerceEngineDefaultStorefront')" -f $EngineHostName)
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken)
    $result = Invoke-RestMethod $UrlCommerceShopsServicesBootstrap -TimeoutSec 1200 -Method PUT -Headers $headers 

	#Check for completion
	$UrlCheckCommandStatus = ("https://{0}{1}" -f $EngineHostName, "/commerceops/CheckCommandStatus(taskId=taskIdValue)")
	$checkUrl = $UrlCheckCommandStatus -replace "taskIdValue", $result.TaskId
	$sw = [system.diagnostics.stopwatch]::StartNew()
    $tp = New-TimeSpan -Minute 10
    do {
        Start-Sleep -s 10
        Write-Host "Checking if $($checkUrl) has completed ..."
        $result = Invoke-RestMethod $checkUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"

        if ($result.ResponseCode -ne "Ok") {
            $(throw Write-Host "Syncing Default Content Paths failed, please check Engine service logs for more info.")
        }
        else {
            write-Host $result.ResponseCode
            Write-Host $result.Status
        }
    } while ($result.Status -ne "RanToCompletion" -and $sw.Elapsed -le $tp)

    Write-Host "Syncing Default Content Paths completed"
}

Function InitializeCommerceServices {
    Write-Host "Initializing HabitatAuthoring Environment..."

	#Make request
	$UrlInitializeEnvironment = ("https://{0}/commerceops/InitializeEnvironment()" -f $EngineHostName)
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Authorization", $global:sitecoreIdToken);
	$body = @{
        environment = "HabitatAuthoring"
    }
    $result = Invoke-RestMethod $UrlInitializeEnvironment -TimeoutSec 1200 -Method Post -Headers $headers -Body ($body | ConvertTo-Json) -ContentType "application/json"

	#Check for completion
	$UrlCheckCommandStatus = ("https://{0}{1}" -f $EngineHostName, "/commerceops/CheckCommandStatus(taskId=taskIdValue)")
	$checkUrl = $UrlCheckCommandStatus -replace "taskIdValue", $result.TaskId
    $sw = [system.diagnostics.stopwatch]::StartNew()
    $tp = New-TimeSpan -Minute 10
    do {
        Start-Sleep -s 10
        Write-Host "Checking if $($checkUrl) has completed ..."
        $result = Invoke-RestMethod $checkUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"

        if ($result.ResponseCode -ne "Ok") {
            $(throw Write-Host "Initialize environment HabitatAuthoring failed, please check Engine service logs for more info.")
        }
        else {
            write-Host $result.ResponseCode
            Write-Host $result.Status
        }
    } while ($result.Status -ne "RanToCompletion" -and $sw.Elapsed -le $tp)

    Write-Host "Initializing HabitatAuthoring Environment completed ..."
}

Write-Host "Begining Engine First Deploy Setup..."
Get-IdServerToken
CleanEnvironment
BootStrapCommerceServices
SyncDefaultContentPaths
InitializeCommerceServices