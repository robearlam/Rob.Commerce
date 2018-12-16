param 
( 
	[Parameter(Mandatory = $true)]
    [string]$EngineHostName,
	[Parameter(Mandatory = $true)]
    [int]$EnginePort,
	[Parameter(Mandatory = $true)]
    [string]$IdentityServerHostname,
	[Parameter(Mandatory = $true)]
    [int]$IdentityServerPort,
	[Parameter(Mandatory = $true)]
    [string]$AdminPassword,
	[Parameter(Mandatory = $true)]
    [string]$AdminUser
)

Function Get-IdServerToken {
    $UrlIdentityServerGetToken = ("https://{0}:{1}/connect/token" -f $IdentityServerHostname, $IdentityServerPort)

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
    $initializeUrl = ("https://{0}:{1}/commerceops/CleanEnvironment()" -f $EngineHostName, $EnginePort)
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
    Write-Host "BootStrapping Commerce Services."
    $UrlCommerceShopsServicesBootstrap = ("https://{0}:{1}/commerceops/Bootstrap()" -f $EngineHostName, $EnginePort)
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken)
    Invoke-RestMethod $UrlCommerceShopsServicesBootstrap -TimeoutSec 1200 -Method PUT -Headers $headers 
    Write-Host "Commerce Services BootStrapping completed"
}

Function InitializeCommerceServices {
    Write-Host "Initializing Environments"
    $initializeParam = "/commerceops/InitializeEnvironment(environment='envNameValue')"
    $UrlInitializeEnvironment = ("https://{0}:{1}{2}" -f $EngineHostName, $EnginePort, $initializeParam)
    $UrlCheckCommandStatus = ("https://{0}:{1}{2}" -f $EngineHostName, $EnginePort, "/commerceops/CheckCommandStatus(taskId=taskIdValue)")

    $Environments = @("HabitatAuthoring")

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken);

    foreach ($env in $Environments) {
        Write-Host "Initializing $($env) ..."

        $initializeUrl = $UrlInitializeEnvironment -replace "envNameValue", $env
        $result = Invoke-RestMethod $initializeUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"
        $checkUrl = $UrlCheckCommandStatus -replace "taskIdValue", $result.TaskId

        $sw = [system.diagnostics.stopwatch]::StartNew()
        $tp = New-TimeSpan -Minute 10
        do {
            Start-Sleep -s 30
            Write-Host "Checking if $($checkUrl) has completed ..."
            $result = Invoke-RestMethod $checkUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"

            if ($result.ResponseCode -ne "Ok") {
                $(throw Write-Host "Initialize environment $($env) failed, please check Engine service logs for more info.")
            }
            else {
                write-Host $result.ResponseCode
                Write-Host $result.Status
            }
        } while ($result.Status -ne "RanToCompletion" -and $sw.Elapsed -le $tp)

        Write-Host "Initialization for $($env) completed ..."
    }

    Write-Host "Initialization completed ..."
}

Write-Host "Begining Engine First Deploy Setup..."
Get-IdServerToken
BootStrapCommerceServices
CleanEnvironment
InitializeCommerceServices