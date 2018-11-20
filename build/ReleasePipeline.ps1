# Until release pipelines have YAML support (https://dev.azure.com/mseng/Azure%20DevOps%20Roadmap/_workitems/edit/1221170),
# as much as possible goes here.

# Secret release variables are injected
Param(
    [Parameter(Mandatory=$true)] [string] $VsixMarketplaceAccessToken,
    [Parameter(Mandatory=$true)] [string] $GitHubReleaseAccessToken
)

. "$PSScriptRoot\Release.ps1"

# Calculate release version by scripping the build metadata from the build version
# (of the form ‘1.0.0+build.1234.commit.abcdef12’)
$releaseVersion = $env:BUILD_BUILDNUMBER.Substring(0, $env:BUILD_BUILDNUMBER.IndexOfAny(('-', '+')))

PublishToMarketplace `
    "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\VSIX\CopyFunctionBreakpointName.vsix" `
    "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\Marketplace\VsixPublishManifest.json" `
    $VsixMarketplaceAccessToken

CreateGitHubRelease `
    -GitHubAccessToken $GitHubReleaseAccessToken `
    -Owner 'jnm2' `
    -Repository 'CopyFunctionBreakpointName' `
    -Commit $env:BUILD_SOURCEVERSION `
    -Tag "v$releaseVersion" `
    -Name $releaseVersion `
    -Body 'https://marketplace.visualstudio.com/items?itemName=jnm2.CopyFunctionBreakpointName' `
    -Assets (
        "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\VSIX\CopyFunctionBreakpointName.vsix",
        "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\Symbols\CopyFunctionBreakpointName.pdb"
    )
