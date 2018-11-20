<#
Until release pipelines have YAML support (https://dev.azure.com/mseng/Azure%20DevOps%20Roadmap/_workitems/edit/1221170),
as much as possible goes here.

Setup instructions:

1. Create secret release variables:

   - VsixMarketplaceAccessToken, value obtained from Azure DevOps user settings (Security/Personal Access Tokens).
     Organization *must* be changed to ‘All accessible organizations’ at creation time. Editing later doesn’t work.
     Under Scopes, choose ‘Custom defined,’ ‘Show all scopes,’ and click Publish under the Marketplace group.

   - GitHubReleaseAccessToken, value obtained from https://github.com/settings/tokens.
     Only scope needed is public_repo.

2. Add PowerShell task to the release pipeline job and set:

   - Display name: Delegate to source-controlled file

   - Script path: $(System.DefaultWorkingDirectory)\CI\ReleasePipelineSource\ReleasePipeline.ps1
     This assumes the build producing the artifacts is named CI and that one of the artifacts contains this file
     and is named ReleasePipelineSource. Refer to azure_pipelines.yml to see how the artifacts are created.

   - Arguments: "$(VsixMarketplaceAccessToken)" "$(GitHubReleaseAccessToken)"

3. If you like, set Options > General > Release name format: Release $(Build.BuildNumber)
   Should be good to go!
#>

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
