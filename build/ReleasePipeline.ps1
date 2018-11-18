# Until release pipelines have YAML support (https://dev.azure.com/mseng/Azure%20DevOps%20Roadmap/_workitems/edit/1221170),
# as much as possible goes here.

# Secret release variables are injected
Param(
    [Parameter(Mandatory=$true)] [string] $VsixMarketplaceAccessToken
)

. "$PSScriptRoot\Release.ps1"

PublishToMarketplace `
    "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\VSIX\CopyFunctionBreakpointName.vsix" `
    "$env:SYSTEM_ARTIFACTSDIRECTORY\CI\Marketplace\VsixPublishManifest.json" `
    $VsixMarketplaceAccessToken
