function PublishToMarketplace(
    [Parameter(Mandatory=$true)] [string] $VsixPath,
    [Parameter(Mandatory=$true)] [string] $VsixPublishManifestPath,
    [Parameter(Mandatory=$true)] [string] $VsixMarketplaceAccessToken
) {
    $visualStudioInstallation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.VisualStudio.Component.VSSDK -property installationPath

    $vsixPublisher = Join-Path $visualStudioInstallation 'VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe'
    & $vsixPublisher publish -payload $VsixPath -publishManifest $VsixPublishManifestPath -personalAccessToken $VsixMarketplaceAccessToken
}

function CreateGitHubRelease(
    [Parameter(Mandatory=$true)] [string] $GitHubAccessToken,
    [Parameter(Mandatory=$true)] [string] $Owner,
    [Parameter(Mandatory=$true)] [string] $Repository,
    [Parameter(Mandatory=$true)] [string] $Commit,
    [Parameter(Mandatory=$true)] [string] $Tag,
    [Parameter(Mandatory=$true)] [string] $Name,
    [string] $Body,
    [switch] $Draft,
    [switch] $Prerelease,
    [string[]] $Assets
) {
    [System.Net.ServicePointManager]::SecurityProtocol = 'tls12'

    $headers = @{
        Accept = 'application/vnd.github.v3+json'
        Authorization = "token $GitHubAccessToken"
        'User-Agent' = 'PowerShell'
    }

    # https://developer.github.com/v3/repos/releases/#create-a-release
    $creationResult = Invoke-RestMethod `
        -Method 'post' `
        -Uri "https://api.github.com/repos/$Owner/$Repository/releases" `
        -Headers $headers `
        -Body (ConvertTo-Json @{
            tag_name = $Tag
            target_commitish = $Commit
            name = $Name
            body = $Body
            draft = $Draft.IsPresent
            prerelease = $Prerelease.IsPresent
        })

    foreach ($asset in $Assets) {
        # https://developer.github.com/v3/repos/releases/#upload-a-release-asset
        $filename = Split-Path $asset -leaf
        $uploadUrl = $creationResult.upload_url -replace "\{\?[^}]+\}", "?name=$filename"

        $null = Invoke-RestMethod `
            -Method 'post' `
            -Uri $uploadUrl `
            -Headers $headers `
            -ContentType 'application/zip' `
            -InFile $asset
    }
}
