function PublishToMarketplace(
    [Parameter(Mandatory=$true)] [string] $VsixPath,
    [Parameter(Mandatory=$true)] [string] $VsixPublishManifestPath,
    [Parameter(Mandatory=$true)] [string] $VsixMarketplaceAccessToken
) {
    $visualStudioInstallation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.VisualStudio.Component.VSSDK -property installationPath

    $vsixPublisher = Join-Path $visualStudioInstallation 'VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe'
    & $vsixPublisher publish -payload $VsixPath -publishManifest $VsixPublishManifestPath -personalAccessToken $VsixMarketplaceAccessToken
}
